// A thin outline drawn round a character by InteractableHighlight while the crosshair is on it: the character's mesh is drawn again, pushed outward along its
// normals, with only the back faces showing, so what is left to see is a line round the edge of the body (the body itself is not tinted or covered).
// The object's real materials are never touched. Lives in a Resources folder so builds include it.
Shader "Hidden/FirewallDuty/OutlineHull"
{
    Properties
    {
        _Color ("Color", Color) = (0.2, 0.9, 1, 1)
        _Width ("Width (metres)", Float) = 0.016
        _Intensity ("Intensity", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _Width;
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
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));
                positionWS += normalWS * _Width;
                output.positionCS = TransformWorldToHClip(positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return half4(_Color.rgb, saturate(_Intensity));
            }
            ENDHLSL
        }
    }
}
