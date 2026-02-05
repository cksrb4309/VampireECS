using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Profiling;
using Sirenix.OdinInspector;

/// <summary>
/// VFX Graph의 메쉬 연동 기능을 활용하여, 대량의 텍스트 데이터를 텍스처로 베이킹(Baking)하고
/// 한 번의 드로우 콜로 방출(Batching)하는 고성능 이미터 클래스입니다.
/// </summary>
public class DamageTextVfxBatchEmitter : MonoBehaviour
{
    /// <summary>
    /// 전역 접근을 위한 싱글톤 인스턴스
    /// </summary>
    public static DamageTextVfxBatchEmitter Instance { get; private set; }

    [Header("시뮬레이션 설정")]
    [Tooltip("한 개별 텍스트가 가질 수 있는 최대 문자 수입니다.")]
    public int MaxChars = 15;
    [Tooltip("한 배치(Batch)에서 동시에 관리할 수 있는 최대 텍스트 개수입니다.")]
    public int MaxTextCount = 100;
    [Tooltip("텍스처 메모리 할당을 위한 총 문자 수의 한도입니다.")]
    public int MaxTotalChars = 1500;

    [Header("VFX Graph")]
    [SerializeField] private VisualEffect textVFX;

    [Header("문자 맵핑 데이터")]
    [SerializeField] private SymbolsTextureData textureData;

    // 문자 인덱스 매핑 테이블 (x = 전역 문자 인덱스, y = 0)
    // - GPU에서 각 파티클이 자신이 속한 텍스트 행(Text Index)과
    //   해당 행 내에서의 순서(Char Index)를 찾기 위한 룩업 테이블
    // - 데이터 구성: R(텍스트 인덱스), G(내부 문자 순서)
    private Texture2D charIndexInRowTable;

    // 문자 데이터 테이블 (x = 내부 문자 순서, y = 텍스트 인덱스)
    // - 각 문자의 폰트 아틀라스 내 좌표(UV)를 패킹하여 저장
    // - 패킹 방식: (U_coord * 10) + V_coord (10x10 아틀라스 기준)
    private Texture2D charTable;

    // 텍스트 속성 파라미터 테이블 (x = 텍스트 인덱스, y = 파라미터 타입)
    // - y=0: 가상 위치(RGB: xyz) 및 입자 수명(A: lifetime)
    // - y=1: 글꼴 색상(RGB: rgb) 및 글꼴 크기(A: fontSize)
    // - y=2: 외곽선 색상(RGB: rgb) 및 유효 문자 길이(A: length)
    private Texture2D paramTable;

    // 대기 중인 텍스트 요청 리스트
    private readonly List<DefaultTextRequest> pendingDefaultRequests = new();
    private readonly List<DamageTextRequest> pendingDamageRequests = new();

    // 성능 분석용 프로파일러 마커
    static readonly ProfilerMarker marker_WriteTextTexture_String = new("TextVFX.WriteTexture.String");
    static readonly ProfilerMarker marker_WriteTextTexture_Damage = new("TextVFX.WriteTexture.Damage");

    private int testCounter = 0;

    /// <summary>
    /// 인스펙터 버튼을 통해 기능을 테스트합니다. 다양한 위치에 데미지 수치를 생성합니다.
    /// </summary>
    [Button]
    public void Test()
    {
        Profiler.BeginSample("Editor.DamageTextTest");
        try
        {
            testCounter++;
            for (int i = 0; i < 20; i++)
            {
                EnqueueText(
                    testCounter + i * 10,
                    new TextEmitParams(
                        position: UnityEngine.Random.insideUnitSphere * 5f,
                        lifetime: 2f,
                        fontSize: 1f,
                        fontColor: Color.yellow,
                        outlineColor: Color.red
                    )
                );
            }
        }
        finally
        {
            Profiler.EndSample();
        }
    }

    /// <summary>
    /// GPU 데이터 전송을 위한 텍스처 메모리를 할당하고 필터링 모드를 설정합니다.
    /// 정밀한 데이터 룩업을 위해 Point 필터링을 사용합니다.
    /// </summary>
    private void InitializeTextures()
    {
        // 전역 문자 인덱스 맵 (RGFloat: 텍스트ID, 문자ID)
        charIndexInRowTable = new Texture2D(MaxTotalChars, 1, TextureFormat.RGFloat, false, true);
        charIndexInRowTable.filterMode = FilterMode.Point;
        charIndexInRowTable.wrapMode = TextureWrapMode.Clamp;

        // 문자 아틀라스 좌표 맵 (RFloat: 패킹된 UV)
        charTable = new Texture2D(MaxChars, MaxTextCount, TextureFormat.RFloat, false, true);
        charTable.filterMode = FilterMode.Point;
        charTable.wrapMode = TextureWrapMode.Clamp;

        // 텍스트별 기하/시각 파라미터 맵 (RGBAFloat: 위치, 색상 등)
        paramTable = new Texture2D(MaxTextCount, 3, TextureFormat.RGBAFloat, false, true);
        paramTable.filterMode = FilterMode.Point;
        paramTable.wrapMode = TextureWrapMode.Clamp;
    }

    /// <summary>
    /// 문자열 형태의 일반 텍스트 출력을 예약합니다.
    /// </summary>
    public void EnqueueText(string message, TextEmitParams emitParams)
    {
        pendingDefaultRequests.Add(new DefaultTextRequest(message, emitParams));
    }

    /// <summary>
    /// 정수 형태의 데미지 수치 출력을 예약합니다. (자동 자릿수 분할 처리)
    /// </summary>
    public void EnqueueText(int damage, TextEmitParams emitParams)
    {
        pendingDamageRequests.Add(new DamageTextRequest(damage, emitParams));
    }

    private void LateUpdate()
    {
        // 프레임당 한 번만 몰아서 처리(Flush)하여 오버헤드를 최소화합니다.
        if (pendingDefaultRequests.Count > 0 || pendingDamageRequests.Count > 0)
        {
            FlushRequests();
        }
    }

    /// <summary>
    /// 수집된 모든 텍스트 데이터를 텍스처로 베이킹하고, VFX Graph에 이벤트를 보내 입자를 생성합니다.
    /// </summary>
    private void FlushRequests()
    {
        int textCount = Mathf.Min(pendingDefaultRequests.Count, MaxTextCount);
        int textIndex = 0;
        int spawnIndex = 0;

        #region 문자열 요청 처리
        for (; textIndex < textCount; textIndex++)
        {
            DefaultTextRequest req = pendingDefaultRequests[textIndex];
            int length = Mathf.Min(req.Message.Length, MaxChars);

            WriteCharRow(req.Message, length, textIndex);
            WriteParamRow(req.TextEmitParams, length, textIndex);

            // 해당 글자가 어떤 텍스트의 몇 번째 글자인지 룩업 데이터 기록
            for (int charIndex = 0; charIndex < length; charIndex++)
            {
                if (spawnIndex >= MaxTotalChars) break;
                charIndexInRowTable.SetPixel(spawnIndex, 0, new Color(textIndex, charIndex, 0, 0));
                spawnIndex++;
            }
        }
        #endregion

        #region 데미지 수치 요청 처리
        int currentDamageRequests = pendingDamageRequests.Count;
        int totalTargetCount = Mathf.Min(textIndex + currentDamageRequests, MaxTextCount);

        for (; textIndex < totalTargetCount; textIndex++)
        {
            var req = pendingDamageRequests[textIndex - pendingDefaultRequests.Count];
            int damage = req.Damage;
            int digitCount = Mathf.Min(GetSignedDigitCount(damage), MaxChars);

            WriteDigitRow(damage, digitCount, textIndex);
            WriteParamRow(req.TextEmitParams, digitCount, textIndex);

            // 수치 텍스트의 각 자릿수에 대한 룩업 데이터 기록
            for (int charIndex = 0; charIndex < digitCount; charIndex++)
            {
                if (spawnIndex >= MaxTotalChars) break;
                charIndexInRowTable.SetPixel(spawnIndex, 0, new Color(textIndex, charIndex, 0, 0));
                spawnIndex++;
            }
        }
        #endregion

        #region 데이터 업로드 및 VFX 실행
        charIndexInRowTable.Apply(false, false);
        charTable.Apply(false, false);
        paramTable.Apply(false, false);

        // VFX 파라미터 업데이트
        textVFX.SetInt("TextCount", textIndex);
        textVFX.SetInt("TotalCharCount", spawnIndex);
        textVFX.SetTexture("CharIndexInRowTable", charIndexInRowTable);
        textVFX.SetTexture("CharTable", charTable);
        textVFX.SetTexture("ParamTable", paramTable);

        // 배치 생성 이벤트 발송 (GPU 사이드에서 입자 방출 시작)
        textVFX.SendEvent("SpawnBatchText");

        pendingDamageRequests.Clear();
        pendingDefaultRequests.Clear();
        #endregion
    }

    /// <summary>
    /// 개별 문자를 UV 아틀라스 좌표로 변환하여 텍스처 행에 할당합니다.
    /// </summary>
    private void WriteCharRow(string message, int length, int row)
    {
        for (int x = 0; x < length; x++)
        {
            float packed = 0f;
            var uv = textureData.GetTextureCoordinates(message[x]);

            // 좌표 패킹: X는 10의 자리, Y는 1의 자리로 합쳐서 데이터 손실 없이 전송
            packed = Mathf.RoundToInt(uv.x) * 10 + Mathf.RoundToInt(uv.y);
            charTable.SetPixel(x, row, new Color(packed, 0, 0, 0));
        }
    }

    /// <summary>
    /// 정수를 각 자릿수별 문자로 분해하여 텍스처 행에 할당합니다. 음수 부호를 지원합니다.
    /// </summary>
    private void WriteDigitRow(int value, int length, int row)
    {
        bool isNegative = value < 0;
        int absValue = isNegative ? -value : value;

        for (int x = 0; x < length; x++)
        {
            float packed = 0f;
            if (isNegative && x == 0)
            {
                var uv = textureData.GetTextureCoordinates('-');
                packed = Mathf.RoundToInt(uv.x) * 10 + Mathf.RoundToInt(uv.y);
            }
            else
            {
                int digitIndex = length - 1 - x;
                if (isNegative) digitIndex--;
                int digit = GetDigitAt(absValue, digitIndex);
                char c = (char)('0' + digit);
                var uv = textureData.GetTextureCoordinates(c);
                packed = Mathf.RoundToInt(uv.x) * 10 + Mathf.RoundToInt(uv.y);
            }
            charTable.SetPixel(x, row, new Color(packed, 0, 0, 0));
        }
    }

    /// <summary>
    /// 숫자의 특정 자릿수 값을 추출합니다.
    /// </summary>
    private int GetDigitAt(int value, int indexFromRight)
    {
        for (int i = 0; i < indexFromRight; i++) value /= 10;
        return value % 10;
    }

    /// <summary>
    /// 음수 부호를 포함한 숫자의 총 문자 자릿수를 계산합니다.
    /// </summary>
    private int GetSignedDigitCount(int value)
    {
        if (value == 0) return 1;
        int count = 0;
        if (value < 0)
        {
            count++;
            value = -value;
        }
        while (value != 0)
        {
            value /= 10;
            count++;
        }
        return count;
    }

    /// <summary>
    /// 텍스트 개별 객체의 물리적/시각적 파라미터를 파라미터 테이블의 열에 기록
    /// </summary>
    private void WriteParamRow(TextEmitParams emitParams, int length, int index)
    {
        // 0행: 위치 정보(world pos)와 수명
        paramTable.SetPixel(index, 0, new Color(emitParams.Position.x, emitParams.Position.y, emitParams.Position.z, emitParams.Lifetime));
        
        // 1행: 주요 색상과 텍스트 크기 스케일
        paramTable.SetPixel(index, 1, new Color(emitParams.FontColor.r, emitParams.FontColor.g, emitParams.FontColor.b, emitParams.FontSize));
        
        // 2행: 외곽선 색상과 사용되는 문자의 길이
        paramTable.SetPixel(index, 2, new Color(emitParams.OutlineColor.r, emitParams.OutlineColor.g, emitParams.OutlineColor.b, length));
    }

    #region Unity Callbacks
    private void Awake()
    {
        Instance = this;
        InitializeTextures();
    }

    private void OnValidate()
    {
        // 에디터 수정 시 총 문자 수 한도 자동 갱신
        MaxTotalChars = MaxChars * MaxTextCount;
        InitializeTextures();
        EditorUtility.SetDirty(this);
    }
    #endregion

    #region 데이터 구조체 및 요청 정의
    /// <summary>
    /// 텍스트 생성에 필요한 시각적 파라미터 모음
    /// </summary>
    public struct TextEmitParams
    {
        public Color FontColor;
        public Color OutlineColor;
        public float FontSize;
        public float Lifetime;
        public Vector3 Position;

        public TextEmitParams(Vector3 position, float lifetime, float fontSize, Color fontColor, Color? outlineColor)
        {
            Position = position;
            Lifetime = lifetime;
            FontSize = fontSize;
            FontColor = fontColor;
            OutlineColor = outlineColor ?? fontColor;
        }
    }

    public struct DefaultTextRequest
    {
        public string Message;
        public TextEmitParams TextEmitParams;
        public DefaultTextRequest(string message, TextEmitParams emitParams)
        {
            Message = message;
            TextEmitParams = emitParams;
        }
    }

    public struct DamageTextRequest
    {
        public int Damage;
        public TextEmitParams TextEmitParams;
        public DamageTextRequest(int damage, TextEmitParams emitParams)
        {
            Damage = damage;
            TextEmitParams = emitParams;
        }
    }
    #endregion

    /// <summary>
    /// 폰트 아틀라스 텍스처 내의 문자 좌표 데이터를 관리하는 보조 클래스
    /// </summary>
    [Serializable]
    public class SymbolsTextureData
    {
        [Tooltip("아틀라스에 포함된 문자 리스트 (순서 중요: 10x10 그리드 기준)")]
        public char[] chars;
        private Dictionary<char, Vector2> charsDict;

        /// <summary>
        /// 빠른 조회를 위해 딕셔너리를 초기화합니다.
        /// </summary>
        public void Initialize()
        {
            charsDict = new Dictionary<char, Vector2>();
            charsDict.EnsureCapacity(56);
            for (int i = 0; i < chars.Length; i++)
            {
                var c = char.ToLowerInvariant(chars[i]);
                if (charsDict.ContainsKey(c)) continue;
                // 10x10 그리드 기반 좌표 계산 (X: 0~9, Y: 0~9)
                var uv = new Vector2(i % 10, 9 - i / 10);
                charsDict.Add(c, uv);
            }
        }

        /// <summary>
        /// 특정 문자의 아틀라스 내 UV 정수 좌표를 반환합니다.
        /// </summary>
        public Vector2 GetTextureCoordinates(char c)
        {
            c = char.ToLowerInvariant(c);
            if (charsDict == null) Initialize();
            return charsDict.TryGetValue(c, out var uv) ? uv : Vector2.zero;
        }
    }
}
