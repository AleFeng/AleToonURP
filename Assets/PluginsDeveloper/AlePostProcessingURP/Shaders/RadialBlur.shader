Shader "Hidden/Custom/RadialBlur"
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

	TEXTURE2D_X(_MainTex);
	float4 _MainTex_TexelSize;

	uniform half4 _Params;
	half4 _GrayColor;
	float _TransparentAmount; //透叠强度

	#define _BlurRadius _Params.x
	#define _Iteration _Params.y
	#define _RadialCenter _Params.zw


	half4 RadialBlur(PostProcessVaryings i)
	{
		float2 blurVector = (_RadialCenter - i.texcoord.xy) * _BlurRadius;
		
		//首次采样
		half4 colorOri = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord);
		i.texcoord.xy += blurVector;

		//多次采样
		half4 acumulateColor = colorOri;
		[unroll(30)]
		for (int j = 1; j < _Iteration; j++)
		{
			acumulateColor += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord);
			i.texcoord.xy += blurVector;
		}

		//透叠强度
		half4 colorRadial =  acumulateColor / _Iteration;
		half4 color = lerp(colorOri, colorRadial, _TransparentAmount);

		return color;
	}

	half4 Frag(PostProcessVaryings i) : SV_Target
	{
		half4 color = RadialBlur(i);
		
		if (_GrayColor.a == 1)
		{
			color.rgb = dot(color.rgb, half3(0.222, 0.707, 0.071));
			color.rgb += _GrayColor.rgb;
		}
		return color;
	}
ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
		HLSLPROGRAM
			#pragma vertex FullScreenTrianglePostProcessVertexProgram
			#pragma fragment Frag
		ENDHLSL
		}
	}
}