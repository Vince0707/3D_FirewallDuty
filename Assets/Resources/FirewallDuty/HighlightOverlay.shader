// Translucent rim-lit overlay drawn on top of an object's own mesh by InteractableHighlight.
// The object's real materials are never touched. Lives in a Resources folder so builds include it.
Shader "Hidden/FirewallDuty/HighlightOverlay"
{
    Properties
    {
        _Color ("Color", Color) = (0.2, 0.9, 1, 1)
        _FillAlpha ("Fill Alpha", Range(0, 1)) = 0.25
        _RimPower ("Rim Power", Range(0.5, 8)) = 2.5
        _Intensity ("Intensity", Range(0, 2)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Highlight"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back
            Offset -1, -1

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half _FillAlpha;
                half _RimPower;
                half _Intensity;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs pos = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = pos.positionCS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(pos.positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half3 n = normalize(input.normalWS);
                half3 v = normalize(input.viewDirWS);
                half rim = pow(1.0h - saturate(dot(n, v)), _RimPower);
                half alpha = saturate(_FillAlpha + rim) * _Intensity;
                return half4(_Color.rgb, saturate(alpha));
            }
            ENDHLSL
        }
    }
}
