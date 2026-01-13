using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DamageTextVfxBatchEmitter : MonoBehaviour
{
    private const int MaxChars = 30;
    private const int MaxTextCount = 125;
    private const int MaxTotalChars = MaxChars * MaxTextCount;

    [Header("VFX Graph")]
    [SerializeField] private VisualEffect textVFX;

    [Header("Defaults")]
    [SerializeField]
    private DamageTextRequest defaultRequest =
        new DamageTextRequest
        {
            Message = "temp",
            Position = Vector3.zero,
            Lifetime = 1f,
            FontSize = 10f,
            FontColor = Color.white
        };

    [Header("Character Mapping")]
    [SerializeField] private SymbolsTextureData textureData;

    // 문자 정보 (x = charIndexInRow, y = textIndex)
    private Texture2D charIndexInRowTable;

    // 문자 glyph 테이블 (x = charIndex, y = textIndex)
    private Texture2D charTable;

    // 텍스트 파라미터 테이블
    private Texture2D paramTable;

    private readonly List<DamageTextRequest> pendingRequests = new();

    private void Awake()
    {
        InitializeTextures();
    }

    [Button]
    public void Test()
    {
        DamageTextRequest request = new DamageTextRequest
        {
            Message = "0",
            Position = new Vector3(0, 0, 0),
            Lifetime = 1.5f,
            FontSize = 1f,
            FontColor = Color.red
        };
        int number = 0;

        for (int i = 0; i < 10; i++)
        {
            request.Position = new Vector3(0, i * 2f, 0f);

            for (int j = 0; j < 10; j++)
            {
                request.Message = number.ToString();

                request.Position += new Vector3(10, 0, 0);

                EnqueueText(request);

                number++;
            }
            number *= 10;
        }
    }
    private void InitializeTextures()
    {
        // 전체 문자 → (textIndex, charIndexInRow)
        charIndexInRowTable = new Texture2D(
            MaxTotalChars, 1,
            TextureFormat.RGFloat, false, true
        );
        charIndexInRowTable.filterMode = FilterMode.Point;
        charIndexInRowTable.wrapMode = TextureWrapMode.Clamp;

        // 문자 glyph 테이블
        charTable = new Texture2D(
            MaxChars, MaxTextCount,
            TextureFormat.RFloat, false, true
        );
        charTable.filterMode = FilterMode.Point;
        charTable.wrapMode = TextureWrapMode.Clamp;

        // Row 0: Position.xyz + Lifetime
        // Row 1: Color.rgb + FontSize
        // Row 2: CharLength
        paramTable = new Texture2D(
            MaxTextCount, 3,
            TextureFormat.RGBAFloat, false, true
        );
        paramTable.filterMode = FilterMode.Point;
        paramTable.wrapMode = TextureWrapMode.Clamp;
    }

    public void EnqueueText(DamageTextRequest request)
    {
        if (textVFX == null || string.IsNullOrEmpty(request.Message))
            return;

        pendingRequests.Add(request);
    }

    private void LateUpdate()
    {
        if (pendingRequests.Count > 0)
            FlushRequests();
    }

    private void FlushRequests()
    {
        int textCount = Mathf.Min(pendingRequests.Count, MaxTextCount);

        int spawnIndex = 0;

        for (int textIndex = 0; textIndex < textCount; textIndex++)
        {
            var req = pendingRequests[textIndex];
            req.ApplyDefaults(defaultRequest);

            int length = Mathf.Min(req.Message.Length, MaxChars);

            WriteCharRow(req.Message, length, textIndex);
            WriteParamRow(req, length, textIndex);

            // 전체 문자 → (textIndex, charIndexInRow) 매핑
            for (int charIndex = 0; charIndex < length; charIndex++)
            {
                if (spawnIndex >= MaxTotalChars) break;

                charIndexInRowTable.SetPixel(
                    spawnIndex, 0,
                    new Color(textIndex, charIndex, 0, 0)
                );

                spawnIndex++;
            }
        }

        charIndexInRowTable.Apply(false, false);
        charTable.Apply(false, false);
        paramTable.Apply(false, false);

        textVFX.SetInt("TextCount", textCount);
        textVFX.SetInt("TotalCharCount", spawnIndex);

        textVFX.SetTexture("CharIndexInRowTable", charIndexInRowTable);
        textVFX.SetTexture("CharTable", charTable);
        textVFX.SetTexture("ParamTable", paramTable);

        textVFX.SendEvent("SpawnBatchText");

        pendingRequests.Clear();
    }
    private void WriteCharRow(string message, int length, int row)
    {
        for (int x = 0; x < MaxChars; x++)
        {
            float packed = 0f;

            if (x < length)
            {
                var uv = textureData.GetTextureCoordinates(message[x]);
                packed = Mathf.RoundToInt(uv.x) * 10 + Mathf.RoundToInt(uv.y);
            }

            charTable.SetPixel(x, row, new Color(packed, 0, 0, 0));
        }
    }

    private void WriteParamRow(DamageTextRequest req, int length, int index)
    {
        paramTable.SetPixel(
            index, 0,
            new Color(req.Position.x, req.Position.y, req.Position.z, req.Lifetime)
        );

        paramTable.SetPixel(
            index, 1,
            new Color(req.FontColor.r, req.FontColor.g, req.FontColor.b, req.FontSize)
        );

        paramTable.SetPixel(
            index, 2,
            new Color(length, 0, 0, 0)
        );
    }

    // -----------------------------

    [Serializable]
    public struct DamageTextRequest
    {
        public string Message;
        public Vector3 Position;
        public float Lifetime;
        public float FontSize;
        public Color FontColor;

        public void ApplyDefaults(DamageTextRequest fallback)
        {
            Message ??= fallback.Message;
            if (Lifetime <= 0f) Lifetime = fallback.Lifetime;
            if (FontSize <= 0f) FontSize = fallback.FontSize;
            if (FontColor.a <= 0f) FontColor = fallback.FontColor;
        }
    }

    [Serializable]
    public class SymbolsTextureData
    {
        public char[] chars;
        private Dictionary<char, Vector2> charsDict;

        public void Initialize()
        {
            charsDict = new Dictionary<char, Vector2>();
            for (int i = 0; i < chars.Length; i++)
            {
                var c = char.ToLowerInvariant(chars[i]);
                if (charsDict.ContainsKey(c)) continue;

                var uv = new Vector2(i % 10, 9 - i / 10);
                charsDict.Add(c, uv);
            }
        }

        public Vector2 GetTextureCoordinates(char c)
        {
            c = char.ToLowerInvariant(c);
            if (charsDict == null) Initialize();
            return charsDict.TryGetValue(c, out var uv) ? uv : Vector2.zero;
        }
    }
}
