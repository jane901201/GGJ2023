Shader "Unlit/BackgroundShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            #if SHADER_API_GLES
            struct appdata
            {
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TexMagnifier;
            
            v2f vert (const appdata v)
            {
                v2f o;
                const float2 tex = GetQuadTexCoord(v.vertexID);
                float2 quadPos = 2.0f * tex - 1.0f;
                o.vertex = float4(quadPos.x, quadPos.y, 1, 1);
                const float4x4 s = Inverse(UNITY_MATRIX_VP);
                o.positionWS = mul(s, o.vertex);
                return o;
            }
            
            float2 _GetUv(const float x, const float y)
            {
                return float2(x / _TexMagnifier, y / _TexMagnifier);
                 //return float2(x * _CamSize.x + _CamCenter.x, y * _CamSize.y + _CamCenter.y);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                const float2 uv = _GetUv(i.positionWS.x, i.positionWS.y);
                
                // sample the texture
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }

            ENDCG
        }
    }
}
