
Shader "AleToonURP/Water Plane"
{
    Properties
    {
//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 基础 Basic ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        [HDR]_ColorShallow("Color Shallow", Color) = (0, 0.9, 1, 0) //颜色 浅水
        [HDR]_ColorDeep("Color Deep", Color) = (0, 0.3, 0.5, 0.3) //颜色 深水
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 基础 Basic ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 水波 Wave ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        _BumpMap ("Bump Map", 2D) = "bump" {} //法线贴图
        _BumpScale ("Bump Scale", Range(0.001, 5)) = 1 //强度
        _BumpScaleFir("BumpMap Scale First", Range(0, 5)) = 1 //纹理缩放 主要
        _BumpScaleSec("BumpMap Scale Second", Range(0, 5)) = 1 //纹理缩放 次要
        _FloatWaveSpeedX("Wave Speed X", Range(-1, 1)) = 0.5 //速度 X轴
        _FloatWaveSpeedY("Wave Speed Y", Range(-1, 1)) = 0.5 //速度 Y轴
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 水波 Wave ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 反射 Reflect ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        [NoScaleOffset]_ReflectCubeMap ("Reflect Cube Map", CUBE) = "white" {} //反射贴图
        _ReflectIntensity ("Reflect Intensity", Range(0, 2)) = 0.1 //反射强度
        _ReflectBulrIntensity ("Reflect Bulr Intensity",Range(0, 7)) = 0.1 //模糊强度
        _FresnelFactor ("Fresnel Factor", Range(0.001, 10)) = 5 //菲涅尔 系数
        _FresnelIntensity ("Fresnel Intensity", Range(0, 5)) = 2 //菲涅尔 强度
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 反射 Reflect ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 折射 Refract ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        _RefractIntensity ("Refract Intensity", Range(0, 1)) = 0.1 //折射强度
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 反射 Reflect ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 边缘泡沫 EdgeFoam ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        _EdgeThresholdMap ("Edge Threshold Map", 2D) = "white" {} //阈值贴图
        [HDR]_EdgeFoamColor ("Edge Foam Color", Color) = (1, 1, 1, 1) //泡沫颜色
        _EdgeFoamThresholdCutoff("Threshold Cutoff", Range(0, 5)) = 0.8 //阈值裁剪
        _EdgeFoamDis ("Edge Foam Distance", Range(0.001, 2)) = 0.4 //边缘泡沫 距离
        _EdgeFoamBlur ("Edge Foam Blur", Range(0, 2)) = 0.3 //边缘泡沫 模糊
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 边缘泡沫 EdgeFoam ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
    }

    SubShader 
    {
        Tags 
        {
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass 
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}
            ZWrite On
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

        HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            //顶点/片元着色器
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            #include "Function.hlsl"
            #include "WaterPlaneMain.hlsl"
        ENDHLSL
        }
    }

    //自定义Shader编辑器面板
    CustomEditor "UnityEditor.AleToonURP.ShaderGUI.ShaderGUIWaterPlane"
}
