//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 结构体定义 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
    //顶点着色器 输入结构
    struct VertexInput
    {
        float4 positionOS : POSITION;
        float4 color : COLOR;
        float2 uv0 : TEXCOORD0;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
    };

    //顶点着色器 输出结构
    struct VertexOutput 
    {
        float4 pos : SV_POSITION;
        float4 color : COLOR;
        float2 uv0 : TEXCOORD0;
        float3 positionWS : TEXCOORD1;
        float4 shadowCoord : TEXCOORD2;
    };
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 结构体定义 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

    //顶点着色器
    VertexOutput vert (VertexInput IN) 
    {
        VertexOutput OUT = (VertexOutput)0;

        //SRP-Batcher优先于GPU-Instance 互斥
        UNITY_SETUP_INSTANCE_ID(IN);
        UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

    #if defined(_OUTLINE_ON)

        VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
        VertexNormalInputs normalInput = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

        //坐标转换
        OUT.uv0 = IN.uv0;
        OUT.positionWS = vertexInput.positionWS;
        //阴影坐标
        OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);

        //描边宽度
        half outlineWidth = _FloatOutlineWidth * 0.001;

        //切线
        real sign = IN.tangentOS.w * GetOddNegativeScale();
        half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
        
        //顶点色法线
        float3 colorDir = IN.color.rgb;
        float3 binormalWS = cross(normalInput.normalWS, colorDir) * tangentWS.w; //副切线
        colorDir = normalize(mul(colorDir, half3x3(tangentWS.xyz, binormalWS, normalInput.normalWS)));

        //描边类型 法线/顶点色/切线
        float3 moveDir = lerp(normalInput.normalWS.rgb, lerp(colorDir, tangentWS, step(1.01, _FloatOutlineType)), step(0.01, _FloatOutlineType));
        moveDir = normalize(moveDir);
        #if defined(_OUTLINE_WIDTH_SAME) //◆◆◆ 等宽
            //沿法线方向外扩
            OUT.pos = TransformWorldToHClip(OUT.positionWS.xyz + moveDir * outlineWidth);
        #elif defined(_OUTLINE_WIDTH_SCALING) //◆◆◆ 变化
            half3 vertDir = normalize(IN.positionOS).xyz;
            half signVertNormal = dot(vertDir, moveDir) + 0.3;
            OUT.pos = TransformWorldToHClip(OUT.positionWS.xyz + moveDir * outlineWidth * signVertNormal);
        #endif //◆◆◆

        //实时光照
        //主光照
        Light lightMain = GetMainLight(OUT.shadowCoord);
        half3 colorLightMain = lightMain.color * lightMain.distanceAttenuation;
        float3 lightDirWS = lightMain.direction; //光照方向
        //实时光照颜色
        half3 realtimeLightColor = colorLightMain;
        //附加光照
        #if defined(_ADDLIGHT_ON)
            half3 colorLightAdd;
            uint lightsCount = GetAdditionalLightsCount();
            LIGHT_LOOP_BEGIN(lightsCount)
                Light light = GetAdditionalLight(lightIndex, OUT.positionWS);
                half3 lightColor = light.color * light.distanceAttenuation;
                colorLightAdd += lightColor;
            LIGHT_LOOP_END
            //自定义 附加光照强度
            colorLightAdd *= _FloatAddLightIntensity;
            //混合实时光照颜色
            realtimeLightColor += colorLightAdd;
        #endif

        //阴影接收
        float shadowAttenuation = saturate(lightMain.shadowAttenuation - _FloatShadowIntensity); //阴影衰减
        realtimeLightColor = lerp(realtimeLightColor, realtimeLightColor * shadowAttenuation, _ToggleShadowReceive);

        //光照开关
        realtimeLightColor = lerp(half3(1, 1, 1), realtimeLightColor * _FloatGlobalExposureIntensity, _ToggleGlobalLightOutline);
        //记录光照颜色
        OUT.color.rgb = realtimeLightColor;

    #endif

        return OUT;
    }

    //片元着色器
    float4 frag(VertexOutput IN) : SV_Target
    {
        //SRP-Batcher优先于GPU-Instance 互斥
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        half4 colorFinal = half4(1, 1, 1, 1);

    #if defined(_OUTLINE_ON)

        //外描边 光照色 混合
        half4 colorOutlineLightBlend = _ColorOutlineColor * IN.color;

        //基础贴图颜色混合
        half4 colorBaseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, TRANSFORM_TEX(IN.uv0, _BaseMap)) * _BaseColor;
        half4 colorBaseMapBlend = lerp(colorOutlineLightBlend, colorOutlineLightBlend * colorBaseMap, _FloatOutlineBaseMapBlendIntensity);
        colorFinal = lerp(colorOutlineLightBlend, colorBaseMapBlend, _ToggleOutlineBaseMapBlend); //开关 混合基础色

        //纹理贴图 颜色混合
        #if defined(_OUTLINE_TEXMAP_ON)
            half4 colorTexMap = SAMPLE_TEXTURE2D(_TexOutlineTexMap, sampler_TexOutlineTexMap, TRANSFORM_TEX(IN.uv0, _TexOutlineTexMap));
            colorFinal = lerp(colorFinal, colorTexMap, _FloatOutlineTexMapIntensity);
        #endif

        //透明度
        half alphaValue = colorBaseMap.a;

        #if defined(_CLIP_OFF) //◆◆◆ 裁剪关闭
            //
        #elif defined(_CLIP_DITHER) //◆◆◆ 裁剪_挖孔
            half clipValue = SAMPLE_TEXTURE2D(_TexClipMaskMap, sampler_TexClipMaskMap, TRANSFORM_TEX(IN.uv0, _TexClipMaskMap)); //阈值采样
            clipValue = clipValue - _FloatClipIntensity;
            clip(clipValue);
        #elif defined(_CLIP_ALPHA) //◆◆◆ 裁剪_透明度
            half clipValue = SAMPLE_TEXTURE2D(_TexClipMaskMap, sampler_TexClipMaskMap, TRANSFORM_TEX(IN.uv0, _TexClipMaskMap)); //阈值采样
            //应用 基础纹理的透明度
            clipValue = lerp(clipValue, (clipValue + colorBaseMap.a) * 0.5, _ToggleClipTransBaseMapAlpha);
            //裁剪强度计算
            clipValue = clipValue - _FloatClipTransIntensity;
            clip(clipValue);

            alphaValue = saturate(clipValue);
        #endif //◆◆◆

        //表面类型
        #if defined(_SURFACETYPE_OPAQUE)
            colorFinal = half4(colorFinal.rgb, 1);
        #elif defined(_SURFACETYPE_TRANSPARENT)
            colorFinal = half4(colorFinal.rgb, alphaValue);
        #else
            colorFinal = half4(colorFinal.rgb, 1);
        #endif

    #endif

        return colorFinal;
    }

