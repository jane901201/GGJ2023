Shader "Unlit/BackgroundShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SkyTex ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderQueue"="GeometryLast" }
        LOD 100

        Pass
        {
            ZWrite Off
            ZTest Less
            
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            #if SHADER_API_GLES
            struct appdata
            {
                float4 positionOS : POSITION;
            };
            #else
            struct appdata
            {
                uint vertexID : SV_VertexID;
            };
            #endif

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 positionWS : TEXCOORD0;
                //float4 positionCS : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _SkyTex;
            float4 _MainTex_ST;
            float2 _TexParam;

            // 0 - 0,1
            // 1 - 0,0
            // 2 - 1,0
            // 3 - 1,1
            float4 GetQuadVertexPosition(uint vertexID, float z = UNITY_NEAR_CLIP_VALUE)
            {
                uint topBit = vertexID >> 1;
                uint botBit = (vertexID & 1);
                float x = topBit;
                float y = 1 - (topBit + botBit) & 1; // produces 1 for indices 0,3 and 0 for 1,2
                return float4(x, y, z, 1.0);
            }

            float4x4 Inverse(float4x4 m)
            {
                float n11 = m[0][0], n12 = m[1][0], n13 = m[2][0], n14 = m[3][0];
                float n21 = m[0][1], n22 = m[1][1], n23 = m[2][1], n24 = m[3][1];
                float n31 = m[0][2], n32 = m[1][2], n33 = m[2][2], n34 = m[3][2];
                float n41 = m[0][3], n42 = m[1][3], n43 = m[2][3], n44 = m[3][3];

                float t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
                float t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
                float t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
                float t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

                float det = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;
                float idet = 1.0f / det;

                float4x4 ret;

                ret[0][0] = t11 * idet;
                ret[0][1] = (n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44) * idet;
                ret[0][2] = (n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44) * idet;
                ret[0][3] = (n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43) * idet;

                ret[1][0] = t12 * idet;
                ret[1][1] = (n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44) * idet;
                ret[1][2] = (n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44) * idet;
                ret[1][3] = (n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43) * idet;

                ret[2][0] = t13 * idet;
                ret[2][1] = (n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44) * idet;
                ret[2][2] = (n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44) * idet;
                ret[2][3] = (n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43) * idet;

                ret[3][0] = t14 * idet;
                ret[3][1] = (n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34) * idet;
                ret[3][2] = (n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34) * idet;
                ret[3][3] = (n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33) * idet;

                return ret;
            }
            
            v2f vert (const appdata v)
            {
                v2f o;
                float2 tex;
#if SHADER_API_GLES
                tex = v.positionOS;
#else
                tex = GetQuadVertexPosition(v.vertexID);
#endif 
                float z = 0.001f; 
#if defined(UNITY_REVERSED_Z) 
                z = 1.0f - z;
#else
                float t = tex.x;
                tex.x = tex.y;
                tex.y = t;
#endif
                float2 quadPos = 2.0f * tex - 1.0f;
                o.vertex = float4(quadPos.x, quadPos.y, z, 1);
                const float4x4 s = Inverse(UNITY_MATRIX_VP);
                o.positionWS = mul(s, o.vertex);
                //o.positionCS = o.vertex;
                return o;
            }
            
            float4 _GetGroundSkyUv(const float x, const float y)
            {
                float2 skyUv = float2(x / _TexParam.x, y / _TexParam.x);
                return float4(skyUv.x, skyUv.y, skyUv.x, skyUv.y);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                const float yPos = i.positionWS.y - _TexParam.y; 
                float4 groundUvSkyUv = _GetGroundSkyUv(i.positionWS.x, yPos);
                
                // sample the texture
                fixed4 col = yPos <= 0 ? tex2D(_MainTex, groundUvSkyUv.xy) : tex2D(_SkyTex, groundUvSkyUv.zw);
                return col;
            }

            ENDCG
        }
    }
}
