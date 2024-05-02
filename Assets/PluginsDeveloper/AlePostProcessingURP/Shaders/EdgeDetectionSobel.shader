Shader "Hidden/Custom/EdgeDetectionSobel"
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

	TEXTURE2D_X(_MainTex);
	float4 _MainTex_TexelSize;

	half2 _Params;
	half4 _EdgeColor;

	#define _EdgeWidth _Params.x

	float intensity(in float4 color)
	{
		return sqrt((color.x * color.x) + (color.y * color.y) + (color.z * color.z));
	}

	float sobel(float stepx, float stepy, float2 center)
	{
		// get samples around pixel
		float topLeft = intensity(SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, center + float2(-stepx, stepy)));
		float midLeft = intensity(SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, center + float2(-stepx, 0)));
		float bottomLeft = intensity(SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, center + float2(-stepx, -stepy)));
		float midTop = intensity(SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, center + float2(0, stepy)));
		float midBottom = intensity(SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, center + float2(0, -stepy)));
		float topRight = intensity(SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, center + float2(stepx, stepy)));
		float midRight = intensity(SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, center + float2(stepx, 0)));
		float bottomRight = intensity(SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, center + float2(stepx, -stepy)));

		// Sobel masks (see http://en.wikipedia.org/wiki/Sobel_operator)
		//        1 0 -1     -1 -2 -1
		//    X = 2 0 -2  Y = 0  0  0
		//        1 0 -1      1  2  1

		// Gx = sum(kernelX[i][j]*image[i][j])
		float Gx = topLeft + 2.0 * midLeft + bottomLeft - topRight - 2.0 * midRight - bottomRight;
		// Gy = sum(kernelY[i][j]*image[i][j]);
		float Gy = -topLeft - 2.0 * midTop - topRight + bottomLeft + 2.0 * midBottom + bottomRight;
		float sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
		return sobelGradient;
	}


	half4 Frag(PostProcessVaryings i) : SV_Target
	{
		half4 sceneColor = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord);

		float sobelGradient = sobel(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y , i.texcoord);

		float3 edgeColor = lerp(sceneColor.rgb, _EdgeColor.rgb, sobelGradient);

		return float4(edgeColor, 1);
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

