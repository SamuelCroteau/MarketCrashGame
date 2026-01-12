// L'IA a été utilisé pour faire ce shader avec une prompt:
// "Fait moi un shader billboard sur l'axe des Y pour un matériel qui vas être placé sur un quad contenant une image transparente"
Shader "Custom/Billboard"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap ("Base Map", 2D) = "white" {}

        _BillboardEnabled ("Billboard Enabled", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "FORWARD"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _BaseMap;

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float  _BillboardEnabled;
            CBUFFER_END

            static float3 ObjectWorldPosition()
            {
                float4 objOriginOS = float4(0,0,0,1);
                return mul(unity_ObjectToWorld, objOriginOS).xyz;
            }

            static float3 ObjectScaleWS()
            {
                float3 sx = unity_ObjectToWorld[0].xyz;
                float3 sy = unity_ObjectToWorld[1].xyz;
                float3 sz = unity_ObjectToWorld[2].xyz;
                return float3(length(sx), length(sy), length(sz));
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 localPos = IN.vertex.xyz;

                // Option désactivée → simplement transformer normalement
                if (_BillboardEnabled < 0.5)
                {
                    OUT.pos = TransformObjectToHClip(IN.vertex);
                    OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                    return OUT;
                }

                float3 worldCenter = ObjectWorldPosition();

                float3 camPos = _WorldSpaceCameraPos;
                float2 dirXZ = camPos.xz - worldCenter.xz;
                float len = length(dirXZ);
                float2 forwardXZ = (len > 1e-6) ? dirXZ / len : float2(0,1);
                float3 forward = float3(forwardXZ.x, 0.0, forwardXZ.y);

                float3 right = float3(-forward.z, 0.0, forward.x);
                float3 up = float3(0,1,0);

                float3 objScale = ObjectScaleWS();

                float3 worldPos = worldCenter
                                + right * (localPos.x * objScale.x)
                                + up    * (localPos.y * objScale.y);

                OUT.pos = TransformWorldToHClip(worldPos);

                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 tex = tex2D(_BaseMap, IN.uv);
                half4 col = tex * _BaseColor;
                clip(col.a - 0.001);
                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Unlit/Transparent"
}
