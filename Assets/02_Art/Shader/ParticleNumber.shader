Shader "DamageText/ParticleNumber"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    //The number of rows and columns in theory can be less than 10, but definitely not more
        _Cols ("Columns Count", Int) = 10
        _Rows ("Rows Count", Int) = 10
    }
    SubShader
    {            
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                float4 cd1 : TEXCOORD1;
                float4 cd2 : TEXCOORD2;
            };           

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                float4 cd1 : TEXCOORD1;
                float4 cd2 : TEXCOORD2;
            };
            
            uniform sampler2D _MainTex;
            uniform uint _Cols;
            uniform uint _Rows;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                o.color = v.color;

                o.cd1 = floor(v.cd1);
                o.cd2 = floor(v.cd2);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float textLen = floor(i.cd2.w + 1e-3); // (정확)
                uint digitIndex = min((uint)floor(i.uv.x * textLen), (uint)(textLen - 1)); // (정확)

                // digitIndex에 따른 CustomData Value 선택
                float value = 0;

                // CustomData 1
                if     (digitIndex == 0)  value = i.cd1.x;
                else if(digitIndex == 1)  value = i.cd1.y;
                else if(digitIndex == 2)  value = i.cd1.z;
                else if(digitIndex == 3)  value = i.cd1.w;

                // CustomData 2
                else if(digitIndex == 4)  value = i.cd2.x;
                else if(digitIndex == 5)  value = i.cd2.y;
                else if(digitIndex == 6)  value = i.cd2.z;

                else value = 0; // 오류 ~

                // packed → xIndex, yIndex 분리 작업
                float xIndex = floor((value + 1e-2) / 10); 
                float yIndex = floor((value + 1e-2) % 10);

                // 현재 자리 내부의 로컬 UV ( 0 ~ 1 )
                float localX = (i.uv.x * textLen) - digitIndex;

                float2 uvTile;

                uvTile.x = (xIndex + localX) * 0.1;
                uvTile.y = (yIndex + i.uv.y) * 0.1;
                
                /*if     (yIndex <=  0.01)  return fixed4(1.0,    0.5,    0.5,      1.0); // 밝은 빨강 0
                else if(yIndex <=  1.01)  return fixed4(0.5,    1.0,    0.5,      1.0); // 밝은 초록 1
                else if(yIndex <=  2.01)  return fixed4(0.5,    0.5,    1.0,      1.0); // 밝은 파랑 2
                else if(yIndex <=  3.01)  return fixed4(0.2,    0.2,    0.2,      1.0); // 살짝 밝은 검정 3
                else if(yIndex <=  4.01)  return fixed4(1.0,    0.0,    0.0,      1.0); // 빨강 4
                else if(yIndex <=  5.01)  return fixed4(0.0,    1.0,    0.0,      1.0); // 초록 5
                else if(yIndex <=  6.01)  return fixed4(0.0,    0.0,    0.0,      1.0); // 검정 6
                else if(yIndex <=  7.01)  return fixed4(0.8,    0.8,    0.8,      1.0); // 살짝 어두운 하양 7
                else if(yIndex <=  8.01)  return fixed4(0.5,    0.0,    0.0,      1.0); // 어두운 빨강 8
                else if(yIndex <=  9.01)  return fixed4(0.0,    0.5,    0.0,      1.0); // 어두운 초록 9
                else if(yIndex <= 10.01) return fixed4(0.0,    0.0,    0.5,      1.0); // 어두운 파랑 10
                else return fixed4(0,   0,    0,      1);*/

                return tex2D(_MainTex, uvTile) * i.color;
            }

            ENDCG
        }
    }
}


/* Shader "DamageText/ParticleNumber" float로 생각할 때
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    //The number of rows and columns in theory can be less than 10, but definitely not more
        _Cols ("Columns Count", Int) = 10
        _Rows ("Rows Count", Int) = 10
    }
    SubShader
    {            
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                float4 cd1 : TEXCOORD1;
                float4 cd2 : TEXCOORD2;
            };           

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                float4 cd1 : TEXCOORD1;
                float4 cd2 : TEXCOORD2;
            };
            
            uniform sampler2D _MainTex;
            uniform uint _Cols;
            uniform uint _Rows;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                o.color = v.color;

                o.cd1 = floor(v.cd1);
                o.cd2 = floor(v.cd2);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float textLen = ceil(fmod(round(i.cd2.w), 100)); // (정확)
                uint digitIndex = min((uint)floor(i.uv.x * textLen), (uint)(textLen-1)); // (정확)

                // digitIndex에 따른 CustomData 선택
                float packed = 0;
                uint elementIndex = digitIndex / 3;       // 한 CustomData 요소당 3자리 (정확)
                uint localDigit = digitIndex % 3;         // 요소 안에서 몇 번째 자리인지 (정확)

                if(elementIndex == 0)       packed = i.cd1[0];
                else if(elementIndex == 1)  packed = i.cd1[1];
                else if(elementIndex == 2)  packed = i.cd1[2];
                else if(elementIndex == 3)  packed = i.cd1[3];
                else if(elementIndex == 4)  packed = i.cd2[0];
                else if(elementIndex == 5)  packed = i.cd2[1];
                else if(elementIndex == 6)  packed = i.cd2[2];
                else if(elementIndex == 7)  packed = i.cd2[3];
                else packed = 0;

                // packed → xIndex, yIndex 분리 작업
                float divisor = 1;
                for(uint k = 2; k > localDigit; k--) divisor = divisor * 100;
                packed = (packed / divisor) % 100;

                float xIndex = floor(packed / 10); // 앞 자리 수 가져오기(정확)
                // float yIndex = floor(packed + 1e-3) % 10; 뒷 자리 수 가져오기()
                float yIndex = floor(packed) % 10;

                // Atlas UV 계산

                // float2 uvTemp;

                // uvTemp.x = 0.1 * (xIndex - 1) + (i.uv.x * textLen) - ((digitIndex - 1) * 0.1);
                // uvTemp.y = 0.1 * (yIndex - 1) + i.uv.y;

                float2 tileSize = float2(0.1, 0.1);
                
                // 현재 자리 내부의 로컬 UV ( 0 ~ 1 )
                float localX = (i.uv.x * textLen) - digitIndex;

                float2 uvTile;
                uvTile.x = (xIndex + localX) * tileSize.x;
                uvTile.y = (yIndex + i.uv.y) * tileSize.y;
                
                 
                if     (yIndex <=  0.01)  return fixed4(1.0,    0.5,    0.5,      1.0); // 밝은 빨강 0
                else if(yIndex <=  1.01)  return fixed4(0.5,    1.0,    0.5,      1.0); // 밝은 초록 1
                else if(yIndex <=  2.01)  return fixed4(0.5,    0.5,    1.0,      1.0); // 밝은 파랑 2
                else if(yIndex <=  3.01)  return fixed4(0.2,    0.2,    0.2,      1.0); // 살짝 밝은 검정 3
                else if(yIndex <=  4.01)  return fixed4(1.0,    0.0,    0.0,      1.0); // 빨강 4
                else if(yIndex <=  5.01)  return fixed4(0.0,    1.0,    0.0,      1.0); // 초록 5
                else if(yIndex <=  6.01)  return fixed4(0.0,    0.0,    0.0,      1.0); // 검정 6
                else if(yIndex <=  7.01)  return fixed4(0.8,    0.8,    0.8,      1.0); // 살짝 어두운 하양 7
                else if(yIndex <=  8.01)  return fixed4(0.5,    0.0,    0.0,      1.0); // 어두운 빨강 8
                else if(yIndex <=  9.01)  return fixed4(0.0,    0.5,    0.0,      1.0); // 어두운 초록 9
                else if(yIndex <= 10.01) return fixed4(0.0,    0.0,    0.5,      1.0); // 어두운 파랑 10
                else return fixed4(0,   0,    0,      1);

                return tex2D(_MainTex, uvTile) * i.color;
            }

            ENDCG
        }
    }
}*/