using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextParticleEmitter : MonoBehaviour
{
    public static DamageTextParticleEmitter Instance { get; private set; } = null;

    // 파티클 시스템 참조
    [SerializeField] private ParticleSystem particle;

    // 문자 텍스처 데이터
    [SerializeField] private SymbolsTextureData textureData;

    [SerializeField] private float startSize = 1f;
    [SerializeField] private Color overrideColor = Color.white;

    private void Awake()
    {
        Instance = this;
    }

    [Button]
    public void Test()
    {
        float a = 1;
        float h = 1.5f;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                SpawnParticle(new Vector3(0, (j * h) + (i * h * 10f), 0), (a * j).ToString(), Color.red);
            }
            a = a * 10 + 1;
        }
        SpawnParticle(new Vector3(-10f, 10, 0), " !/?ABC", Color.red);
    }

    // 벡터 배열을 하나의 플로트 값으로 패킹하는 기능
    public float PackFloat(Vector2[] vecs)
    {
        if (vecs == null || vecs.Length == 0) return 0;

        var result = vecs[0].y * 10000 + vecs[0].x * 100000;

        if (vecs.Length > 1) result += vecs[1].y * 100 + vecs[1].x * 1000;
        if (vecs.Length > 2) result += vecs[2].y + vecs[2].x * 10;

        return result;
    }

    // 커스텀 데이터 생성 Vector4 (xyxyxy, xyxyxy, xyxyxy, xyxyww)
    private Vector4 CreateCustomData(Vector2[] texCoords, int offset = 0)
    {
        Vector4 data = Vector4.zero;

        for (int i = 0; i < 4; i++)
        {
            var vecs = new Vector2[3];

            // 각 Vector4 요소에 3개의 Vector2 좌표를 할당
            for (int j = 0; j < 3; j++)
            {
                // 인덱스 계산
                var ind = i * 3 + j + offset;

                // 텍스처 좌표가 존재하면 할당, 없으면 패킹 종료
                if (texCoords.Length > ind)
                {
                    vecs[j] = texCoords[ind];
                }
                else
                {
                    data[i] = PackFloat(vecs);
                    i = 5;
                    break;
                }
            }
            if (i < 4) data[i] = PackFloat(vecs);
        }
        return data;
    }

    // 커스텀 데이터 생성 Vector4 (xy, xy, xy, ww)
    private Vector4 CreateCustomData_Custom(Vector2[] texCoords, int offset = 0)
    {
        Vector4 data = Vector4.zero;

        for (int i = 0; i < 4; i++)
        {
            // 인덱스 계산
            int ind = i + offset;

            if (texCoords.Length > ind)
            {
                data[i] = (texCoords[ind].x * 10) + (texCoords[ind].y);
            }
            else
            {
                break;
            }
        }
        return data;
    }

    // 파티클 스폰 메서드
    public void SpawnParticle(Vector3 position, string message, Color color)
    {
        // 최대 7자까지만 표시
        Vector2[] texCords = new Vector2[8];

        // 마지막 원소는 길이 정보 저장용
        int messageLenght = Mathf.Min(7, message.Length);


        // 길이 정보 저장
        texCords[texCords.Length - 1] = new Vector2(0, messageLenght);

        // 각 문자에 해당하는 텍스처 좌표 계산
        for (int i = 0; i < texCords.Length; i++)
        {
            if (i >= messageLenght) break;

            texCords[i] = textureData.GetTextureCoordinates(message[i]);
        }

        // Custom Data 생성
        Vector4 custom1Data = CreateCustomData_Custom(texCords);
        Vector4 custom2Data = CreateCustomData_Custom(texCords, 4);

        // 파티클 방출 설정
        var emitParams = new ParticleSystem.EmitParams
        {
            startColor = overrideColor,
            position = position,
            applyShapeToPosition = true,
            startSize3D = new Vector3(messageLenght, 1, 1)
        };

        emitParams.startSize3D *= startSize * particle.main.startSizeMultiplier;

        particle.Emit(emitParams, 1);

        List<Vector4> customData = new List<Vector4>();
        Debug.Log("Value : " + message);
        Debug.Log($"C1 : ({custom1Data.x}, {custom1Data.y}, {custom1Data.z}, {custom1Data.w}   / " +
            $"C2 : ({custom2Data.x}, {custom2Data.y}, {custom2Data.z}, {custom2Data.w}");


        // 파티클의 Custom Data를 가져와서 마지막에 추가된 파티클의 데이터를 수정합니다.
        particle.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        customData[customData.Count - 1] = custom1Data;
        particle.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

        particle.GetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
        customData[customData.Count - 1] = custom2Data;
        particle.SetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
    }
    [Serializable]
    public class SymbolsTextureData // 문자 텍스처 데이터 클래스
    {
        // 텍스처에 포함된 문자 배열 ( 왼쪽 위에서 오른쪽 아래로 10x10 그리드 )
        public char[] chars;

        // 문자와 텍스처 좌표를 매핑하는 딕셔너리
        private Dictionary<char, Vector2> charsDict;

        // 딕셔너리 초기화 메서드
        public void Initialize()
        {
            charsDict = new Dictionary<char, Vector2>();
            for (int i = 0; i < chars.Length; i++)
            {
                // 소문자로 변환하여 중복 방지
                var c = char.ToLowerInvariant(chars[i]);

                if (charsDict.ContainsKey(c)) continue;

                // 텍스처 좌표 계산 (0~9 범위의 x, y 좌표)
                var uv = new Vector2(i % 10, 9 - i / 10);

                charsDict.Add(c, uv);
            }
        }

        // 문자에 해당하는 텍스처 좌표를 반환하는 메서드
        public Vector2 GetTextureCoordinates(char c)
        {
            // 소문자로 변환하여 검색
            c = char.ToLowerInvariant(c);

            if (charsDict == null) Initialize();

            // 딕셔너리에서 텍스처 좌표를 검색 후 반환
            if (charsDict.TryGetValue(c, out Vector2 texCoord)) return texCoord;

            // 이 경우는 있으면 안된다 ~
            return Vector2.zero;
        }
    }
}



