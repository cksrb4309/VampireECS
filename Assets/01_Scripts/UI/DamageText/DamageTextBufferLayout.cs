using System.Runtime.InteropServices;
using UnityEngine.VFX;
using UnityEngine;

public static class DamageTextBufferLayout
{
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    [StructLayout(LayoutKind.Sequential)]
    public struct TextInstanceData
    {
        public Vector3 Position;     // 12 | 12
        public float Lifetime;       // 4  | 16

        public Vector4 FontColor;    // 16 | 32
        public Vector4 OutlineColor; // 16 | 48

        public float FontSize;       // 4  | 52
        public int Length;           // 4  | 56
        public Vector2 Padding;      // 8  | 64
    }

    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    [StructLayout(LayoutKind.Sequential)]
    public struct GlyphData
    {
        public Vector2 Offset;       // 8  | 8
        public int GlyphId;          // 4  | 12
        public float Scale;          // 4  | 16
                                           
        public int TextIndex;        // 4  | 20
        public int CharIndex;        // 4  | 24
        public int Reserved0;        // 4  | 28
        public int Reserved1;        // 4  | 32
    }

    public static readonly int TextInstanceStride = Marshal.SizeOf<TextInstanceData>(); // 64
    public static readonly int GlyphStride = Marshal.SizeOf<GlyphData>();               // 32
}
