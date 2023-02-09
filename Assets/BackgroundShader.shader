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
                //float4 positionCS : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _SkyTex;
            float4 _MainTex_ST;
            float2 _TexParam;
            
            v2f vert (const appdata v)
            {
                v2f o;
                float2 tex = GetQuadVertexPosition(v.vertexID);
                float z = 0.0f; 
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
