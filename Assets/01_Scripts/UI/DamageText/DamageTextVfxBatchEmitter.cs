using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;
using Unity.Mathematics;

using TextInstanceData = DamageTextBufferLayout.TextInstanceData;
using GlyphData = DamageTextBufferLayout.GlyphData;

public class DamageTextVfxBatchEmitter : MonoBehaviour
{
    #region Singleton
    public static DamageTextVfxBatchEmitter Instance { get; private set; }
    #endregion


    #region Config
    [Header("시뮬레이션 설정")]
    public int MaxChars = 15;
    public int MaxTextCount = 100;
    public int MaxTotalChars = 1500;

    [Header("VFX Graph")]
    [SerializeField] private VisualEffect textVFX;

    [Header("문자 맵핑 데이터")]
    [SerializeField] private SymbolsTextureData textureData;
    #endregion

    private readonly char[] digitBuffer = new char[16];

    #region Buffers
    private GraphicsBuffer textInstanceBuffer;
    private GraphicsBuffer glyphBuffer;

    private TextInstanceData[] textInstanceArray;
    private GlyphData[] glyphArray;
    #endregion


    #region Requests
    private readonly Queue<DefaultTextRequest> pendingDefaultRequests = new(128);
    private readonly Queue<DamageTextRequest> pendingDamageRequests = new(128);
    #endregion


    #region Unity Events
    private void Awake()
    {
        Instance = this;
        MaxTotalChars = MaxChars * MaxTextCount;
        InitializeBuffers();
    }

    private void OnValidate()
    {
        MaxTotalChars = MaxChars * MaxTextCount;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    private void OnDestroy()
    {
        ReleaseBuffers();

        if (Instance == this)
            Instance = null;
    }

    private void LateUpdate()
    {
        if (pendingDefaultRequests.Count > 0 || pendingDamageRequests.Count > 0)
        {
            FlushRequests();
        }
    }
    #endregion


    #region Buffer Lifecycle
    private void InitializeBuffers()
    {
        textInstanceArray = new TextInstanceData[MaxTextCount];
        glyphArray = new GlyphData[MaxTotalChars];

        textInstanceBuffer = new GraphicsBuffer(
            GraphicsBuffer.Target.Structured,
            MaxTextCount,
            DamageTextBufferLayout.TextInstanceStride
        );

        glyphBuffer = new GraphicsBuffer(
            GraphicsBuffer.Target.Structured,
            MaxTotalChars,
            DamageTextBufferLayout.GlyphStride
        );

        textVFX.SetGraphicsBuffer("TextInstanceBuffer", textInstanceBuffer);
        textVFX.SetGraphicsBuffer("GlyphBuffer", glyphBuffer);
    }

    private void ReleaseBuffers()
    {
        textInstanceBuffer?.Release();
        textInstanceBuffer = null;

        glyphBuffer?.Release();
        glyphBuffer = null;
    }
    #endregion


    #region Public API
    public void EnqueueText(string message, TextEmitParams emitParams)
    {
        pendingDefaultRequests.Enqueue(new DefaultTextRequest(message, emitParams));
    }

    public void EnqueueText(int damage, TextEmitParams emitParams)
    {
        pendingDamageRequests.Enqueue(new DamageTextRequest(damage, emitParams));
    }
    #endregion


    #region Flush Logic
    private void FlushRequests()
    {
        int textIndex = 0;
        int glyphIndex = 0;

        while (pendingDefaultRequests.Count > 0 && textIndex < MaxTextCount)
        {
            DefaultTextRequest req = pendingDefaultRequests.Peek();
            int length = Mathf.Min(req.Message.Length, MaxChars);

            if (length <= 0)
            {
                pendingDefaultRequests.Dequeue();
                continue;
            }

            if (glyphIndex + length > MaxTotalChars)
                break;

            WriteTextInstance(req.TextEmitParams, length, textIndex);
            WriteStringGlyphs(req.Message, length, textIndex, ref glyphIndex);

            textIndex++;
            pendingDefaultRequests.Dequeue();
        }

        while (pendingDamageRequests.Count > 0 && textIndex < MaxTextCount)
        {
            DamageTextRequest req = pendingDamageRequests.Peek();
            int length = Mathf.Min(GetSignedDigitCount(req.Damage), MaxChars);

            if (length <= 0)
            {
                pendingDamageRequests.Dequeue();
                continue;
            }

            if (glyphIndex + length > MaxTotalChars)
                break;

            WriteTextInstance(req.TextEmitParams, length, textIndex);
            WriteDamageGlyphs(req.Damage, textIndex, ref glyphIndex);

            textIndex++;
            pendingDamageRequests.Dequeue();
        }

        if (textIndex <= 0)
            return;

        textInstanceBuffer.SetData(textInstanceArray, 0, 0, textIndex);
        glyphBuffer.SetData(glyphArray, 0, 0, glyphIndex);

        textVFX.SetInt("TextCount", textIndex);
        textVFX.SetInt("TotalCharCount", glyphIndex);
        textVFX.SendEvent("SpawnBatchText");
    }
    #endregion


    #region Write Data
    private void WriteTextInstance(TextEmitParams emitParams, int length, int index)
    {
        textInstanceArray[index] = new TextInstanceData
        {
            Position = emitParams.Position,
            Lifetime = emitParams.Lifetime,
            FontColor = emitParams.FontColor,
            OutlineColor = emitParams.OutlineColor,
            FontSize = emitParams.FontSize,
            Length = length,
            Padding = Vector2.zero
        };
    }

    private void WriteStringGlyphs(string message, int length, int textIndex, ref int glyphIndex)
    {
        for (int i = 0; i < length; i++)
        {
            int glyphId = GetGlyphId(message[i]);
            float offsetX = i - ((length - 1) * 0.5f);

            glyphArray[glyphIndex++] = new GlyphData
            {
                Offset = new Vector2(offsetX, 0f),
                GlyphId = glyphId,
                Scale = 1f,
                TextIndex = textIndex
            };
        }
    }

    private void WriteDamageGlyphs(int value, int textIndex, ref int glyphIndex)
    {
        bool isNegative = value < 0;
        long absValueLong = isNegative ? -(long)value : value;

        int len = WriteDigitsToBuffer(absValueLong, isNegative);

        for (int i = 0; i < len; i++)
        {
            char c = digitBuffer[len - 1 - i];
            int glyphId = GetGlyphId(c);
            float offsetX = i - ((len - 1) * 0.5f);

            glyphArray[glyphIndex++] = new GlyphData
            {
                Offset = new Vector2(offsetX, 0f),
                GlyphId = glyphId,
                Scale = 1f,
                TextIndex = textIndex
            };
        }
    }

    private int WriteDigitsToBuffer(long value, bool isNegative)
    {
        int len = 0;

        do
        {
            digitBuffer[len++] = (char)('0' + (value % 10));
            value /= 10;
        } while (value > 0);

        if (isNegative)
            digitBuffer[len++] = '-';

        return len;
    }
    #endregion


    #region Utility
    private int GetGlyphId(char c) => textureData.GetGlyphId(c);

    private int GetSignedDigitCount(int value)
    {
        if (value == 0)
            return 1;

        long v = value;
        int count = 0;

        if (v < 0)
        {
            count++;
            v = -v;
        }

        while (v != 0)
        {
            v /= 10;
            count++;
        }

        return count;
    }
    #endregion


    #region Data Structs
    public struct TextEmitParams
    {
        public Vector3 Position;

        public Vector4 FontColor;
        public Vector4 OutlineColor;

        public float FontSize;
        public float Lifetime;

        public TextEmitParams(Vector3 position, float lifetime, float fontSize, float4 fontColor, float4? outlineColor)
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


    #region Symbol Mapping
    [Serializable]
    public class SymbolsTextureData
    {
        public char[] chars;
        private Dictionary<char, int> glyphIdDict;

        public void Initialize()
        {
            glyphIdDict = new Dictionary<char, int>(56);

            for (int i = 0; i < chars.Length; i++)
            {
                char c = char.ToLowerInvariant(chars[i]);
                if (glyphIdDict.ContainsKey(c))
                    continue;

                int x = i % 10;
                int y = 9 - i / 10;
                int glyphId = x * 10 + y;

                glyphIdDict.Add(c, glyphId);
            }
        }

        public int GetGlyphId(char c)
        {
            c = char.ToLowerInvariant(c);

            if (glyphIdDict == null)
                Initialize();

            return glyphIdDict.TryGetValue(c, out int glyphId) ? glyphId : 0;
        }
    }
    #endregion
}
