Shader "Hidden/Custom/ViewportTrans"
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);

    float4 _ViewportRect;

    PostProcessVaryings Vert(FullScreenTrianglePostProcessAttributes v)
    {
        PostProcessVaryings o;
        UNITY_SETUP_INSTANCE_ID(v);
    	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.positionCS = GetFullScreenTriangleVertexPosition(v.vertexID);
        o.texcoord = GetFullScreenTriangleTexCoord(v.vertexID);

        //uv偏移
        o.texcoord = o.texcoord - _ViewportRect.xy;
        //nv缩放
        o.texcoord = o.texcoord / _ViewportRect.zw;

        return o;
    }

    float4 Frag(PostProcessVaryings i) : SV_Target
    {
        float4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord);
        return color;
    }
ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
        HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
        ENDHLSL
        }
    }
}
