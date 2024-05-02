//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 结构体定义 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
    struct AttributesAleToon
    {
        float4 positionOS : POSITION;
        float3 normalOS   : NORMAL;
        float4 tangentOS  : TANGENT;
        float2 texcoord   : TEXCOORD0;
        float2 staticLightmapUV  : TEXCOORD1;
        float2 dynamicLightmapUV : TEXCOORD2;

        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VaryingsAleToon
    {
        float2 uv : TEXCOORD0;

    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
        float3 positionWS : TEXCOORD1;
    #endif

        float3 normalWS : TEXCOORD2;
        float4 tangentWS : TEXCOORD3;
        float3 viewDirWS : TEXCOORD4;

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        half4 fogFactorAndVertexLight : TEXCOORD5; // x: fogFactor, yzw: vertex light
    #else
        half  fogFactor : TEXCOORD5;
    #endif

    //#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord : TEXCOORD6;
    //#endif

    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        half3 viewDirTS : TEXCOORD7;
    #endif

        DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 8);
    #ifdef DYNAMICLIGHTMAP_ON
        float2  dynamicLightmapUV : TEXCOORD9; // Dynamic lightmap UVs
    #endif

        float4 positionCS : SV_POSITION;

        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
    };
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 结构体定义 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
    
    //顶点着色器
    VaryingsAleToon vert (AttributesAleToon IN) 
    {
        VaryingsAleToon OUT = (VaryingsAleToon)0;

        //SRP-Batcher优先于GPU-Instance 互斥
        UNITY_SETUP_INSTANCE_ID(IN);
        UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

        VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
        VertexNormalInputs normalInput = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

        //不使用TRANSFORM_TEX()进行缩放和偏移
        //根据不同贴图各自的设定进行转换
        OUT.uv = IN.texcoord;

//START URP坐标转换与数据传递
        half fogFactor = 0;
    #if !defined(_FOG_FRAGMENT)
        fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    #endif
        // already normalized from normal transform to WS.
        OUT.normalWS = normalInput.normalWS;

        real sign = IN.tangentOS.w * GetOddNegativeScale();
        half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
        OUT.tangentWS = tangentWS;

    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        half3 viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
        half3 viewDirTS = GetViewDirectionTangentSpace(tangentWS, OUT.normalWS, viewDirWS);
        OUT.viewDirTS = viewDirTS;
    #endif

        OUTPUT_LIGHTMAP_UV(IN.staticLightmapUV, unity_LightmapST, OUT.staticLightmapUV);
    #ifdef DYNAMICLIGHTMAP_ON
        OUT.dynamicLightmapUV = IN.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif
        OUTPUT_SH(OUT.normalWS.xyz, OUT.vertexSH);
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        OUT.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
    #else
        OUT.fogFactor = fogFactor;
    #endif

        OUT.positionWS = vertexInput.positionWS;
        OUT.positionCS = vertexInput.positionCS;
        //阴影坐标转换
        OUT.shadowCoord = TransformWorldToShadowCoord(vertexInput.positionWS);
//END
        return OUT;
    }

    //片元着色器
    float4 frag(VaryingsAleToon IN) : SV_TARGET 
    {
        //SRP-Batcher优先于GPU-Instance 互斥
        UNITY_SETUP_INSTANCE_ID(IN);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

        float2 uv0 = IN.uv; //uv0
        float3 viewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS); //观察方向
        float3 normalDirWS = IN.normalWS; //法线方向

        //基础贴图 采样
        float4 colorBaseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv0) * _BaseColor;

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 裁剪 Clip ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        #if defined(_CLIP_OFF) //◆◆◆1 关闭
            
        #elif defined(_CLIP_DITHER) //◆◆◆1 挖孔
            half clipValue = SAMPLE_TEXTURE2D(_TexClipMaskMap, sampler_TexClipMaskMap, TRANSFORM_TEX(uv0, _TexClipMaskMap)); //阈值采样
            //裁剪强度计算
            clipValue = clipValue - _FloatClipIntensity;
            clip(clipValue);
        #elif defined(_CLIP_ALPHA) //◆◆◆1 透明度
            half clipValue = SAMPLE_TEXTURE2D(_TexClipMaskMap, sampler_TexClipMaskMap, TRANSFORM_TEX(uv0, _TexClipMaskMap)); //阈值采样
            clipValue = lerp(clipValue, clipValue * colorBaseMap.a, _ToggleClipTransBaseMapAlpha);
            //裁剪强度计算
            clipValue = clipValue - _FloatClipTransIntensity;
            clip(clipValue);
        #endif //◆◆◆1
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 裁剪 Clip ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 法线贴图 NormalMap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        //法线贴图 采样
        float3 normalDirTex = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, TRANSFORM_TEX(uv0, _BumpMap)), _BumpScale); //法线方向 纹理
        //将法线转换至 世界空间
        float3 binormalWS = cross(normalDirWS, normalDirTex.xyz) * IN.tangentWS.w; //副切线 世界空间
        normalDirTex = normalize(mul(normalDirTex, half3x3(IN.tangentWS.xyz, binormalWS, normalDirWS)));
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 法线贴图 NormalMap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 光照计算 Light ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        //环境光照（Lighting中设置的环境光照等）
        half3 envLightColor = half3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
        //自定义 环境光照强度
        envLightColor *= _FloatEnvLightIntensity;

        //实时光照
        //主光照
        Light lightMain = GetMainLight(TransformWorldToShadowCoord(IN.positionWS));
        half3 colorLightMain = lightMain.color * lightMain.distanceAttenuation;
        float shadowAttenuation = saturate(lightMain.shadowAttenuation - _FloatShadowIntensity); //阴影衰减
        float3 lightDirWS = lightMain.direction; //光照方向
        //实时光照颜色
        half3 realtimeLightColor = colorLightMain;
        //附加光照
        #if defined(_ADDLIGHT_ON)
            half3 colorLightAdd;
            uint lightsCount = GetAdditionalLightsCount();
            LIGHT_LOOP_BEGIN(lightsCount)
                Light light = GetAdditionalLight(lightIndex, IN.positionWS);
                half3 lightColor = light.color * light.distanceAttenuation;
                colorLightAdd += lightColor;
            LIGHT_LOOP_END
            //自定义 附加光照强度
            colorLightAdd *= _FloatAddLightIntensity;
            //混合实时光照颜色
            realtimeLightColor += colorLightAdd;
        #endif
        //自定义 实时光照强度
        realtimeLightColor *= _FloatRealtimeLightIntensity * 0.1;
        
        //混合所有光照
        half3 colorLightBlend = envLightColor + realtimeLightColor;
        //自定义曝光强度
        colorLightBlend *=  _FloatGlobalExposureIntensity;
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 光照计算 Light ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 内置光照 BuiltInLight ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        #if defined(_BUILTINLIGHT_ON)
            //内置光照 方向
            float3 builtInLightDir = float3(_FloatBuiltInLightXAxis, _FloatBuiltInLightYAxis, _FloatBuiltInLightZAxis);
            builtInLightDir = normalize(builtInLightDir);
            lightDirWS = builtInLightDir; //内置光照方向 开关
            //内置光照 颜色
            half3 colorBuiltInLight = lerp(colorLightBlend, _ColorBuiltInLightColor.rgb, _FloatBuiltInLightColorBlend); //混合强度
            colorLightBlend = lerp(colorLightBlend, colorBuiltInLight, _ToggleBuiltInLightColor); //开关 内置光照颜色
        #endif
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 内置光照 BuiltInLight ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 基础贴图 BaseMap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        //基础贴图颜色
        half3 colorBaseMapFinal = colorBaseMap.rgb;
        //颜色混合强度
        colorBaseMapFinal = lerp(colorBaseMap, _ColorBaseMapBlendColor, _BaseMapBlendColorIntensity).rgb;
        colorBaseMapFinal = lerp(colorBaseMapFinal, colorBaseMapFinal * colorLightBlend  * _FloatBaseMapExposureIntensity, _ToggleGlobalLightBaseMap); //光照开关
        //暗部1颜色
        half3 colorBaseMapShade1 = colorBaseMap.rgb * _ColorShade1Color.rgb;
        colorBaseMapShade1 = lerp(colorBaseMapShade1, colorBaseMapShade1 * colorLightBlend * _FloatBaseMapShade1ExposureIntensity, _ToggleGlobalLightBaseShade1); //光照开关
        //暗部2颜色
        half3 colorBaseMapShade2 = colorBaseMap.rgb * _ColorShade2Color.rgb;
        colorBaseMapShade2 = lerp(colorBaseMapShade2, colorBaseMapShade2 * colorLightBlend * _FloatBaseMapShade2ExposureIntensity, _ToggleGlobalLightBaseShade2); //光照开关

        //光照强度
        //开关 光照水平锁定
        float3 lightDirOnBaseMap = lightDirWS;
        lightDirOnBaseMap.y = lerp(lightDirOnBaseMap.y, 0, _ToggleLightHorLockBaseMap);
        //基础贴图法线方向 法线开关
        float3 normalDirOnBaseMap = lerp(normalDirWS, normalDirTex, _ToggleNormalMapOnBaseMap);
        float halfLambert = dot(normalDirOnBaseMap, lightDirOnBaseMap) * 0.5 + 0.5; //半兰伯特
        //阴影接收 开关
        halfLambert = lerp(halfLambert, halfLambert * shadowAttenuation, _ToggleShadowReceive);
            
        //暗部阈值贴图
        #if defined(_BASEMAP_SHADE_THRESHOLDMAP_ON)
            //阈值强度采样
            float shadeThresholdValue = 1 - SAMPLE_TEXTURE2D(_TexShadeThresholdMap, sampler_TexShadeThresholdMap, TRANSFORM_TEX(uv0, _TexShadeThresholdMap)).r;
            shadeThresholdValue *= _FloatShadeThresholdMapIntensity; //自定义强度
            halfLambert = saturate(halfLambert - shadeThresholdValue);
        #endif

        //暗部1 光照强度
        _FloatBrightShade1Blur *= 0.1;
        float lightIntensityShade1 = 1 - saturate((1 + (halfLambert - _FloatBrightShade1Step) / _FloatBrightShade1Blur));
        //暗部2 光照强度
        _FloatShade1Shade2Blur *= 0.05;
        float lightIntensityShade2 = 1 - saturate((1 + (halfLambert - _FloatShade1Shade2Step) / _FloatShade1Shade2Blur));

        //混合三色阶的颜色
        float3 colorFinalBlend = lerp(colorBaseMapFinal, lerp(colorBaseMapShade1, colorBaseMapShade2, lightIntensityShade2), lightIntensityShade1);
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 基础贴图 BaseMap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 高光 HighLight ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        #if defined(_HIGHLIGHT_ON)
            //开关 光照水平锁定
            float3 lightDirOnHighLight = lightDirWS;
            lightDirOnHighLight.y = lerp(lightDirOnHighLight.y, 0, _ToggleLightHorLockHighLight);
            //高光法线方向 法线开关
            float3 normalDirOnHighLight = lerp(normalDirWS, normalDirTex, _ToggleNormalMapOnHighLight);
            //高光强度
            float3 halfDirection = normalize(viewDirWS + lightDirOnHighLight); //半程向量
            float highLightFactor = saturate(dot(halfDirection, normalDirOnHighLight) * 0.5 + 0.5); //高光计算

            //色阶高光
            float highLightStep = 1.0 - step(highLightFactor, 1 - pow(_FloatHighLightSize, 5));
            //柔边高光
            float highLightSoft = pow(highLightFactor, exp2(lerp(11, 1, _FloatHighLightSize)));
            //开关切换 色阶/柔边
            highLightFactor = lerp(highLightStep, highLightSoft, _ToggleHighLightSoft);

            //高光遮罩
            float highLightMaskValue = SAMPLE_TEXTURE2D(_TexHighLightMaskMap, sampler_TexHighLightMaskMap, TRANSFORM_TEX(uv0, _TexHighLightMaskMap)).r;
            highLightMaskValue = saturate(highLightMaskValue + _FloatHighLightMaskMapIntensity); //强度修正
            highLightFactor *= highLightMaskValue;
                
            //高光颜色
            float3 colorHighLight = SAMPLE_TEXTURE2D(_TexHighLightMap, sampler_TexHighLightMap, TRANSFORM_TEX(uv0, _TexHighLightMap)).rgb * _ColorHighLightColor.rgb;
            colorHighLight *= highLightFactor; //高光强度 计算
            //光照开关
            colorHighLight = lerp(colorHighLight, colorHighLight * colorLightBlend, _ToggleGlobalLightHighLight);
            //阴影遮罩 开关
            colorHighLight = lerp(colorHighLight, colorHighLight * (1 - lightIntensityShade1 * _FloatHighLightShadowMaskIntensity), _ToggleHighLightShadowMask);

            //混合颜色 基础颜色+高光颜色
            colorFinalBlend += colorHighLight * _ColorHighLightColor.a;
        #endif
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 高光 HighLight ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
            
//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 边缘光 RimLight ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        #if defined(_RIMLIGHT_ON)
            //边缘光颜色 光照开关
            half3 colorRimLight = lerp(_ColorRimLightColor.rgb, _ColorRimLightColor.rgb * colorLightBlend, _ToggleGlobalLightRimLight);
            colorRimLight *= _ColorRimLightColor.a; //透明度
            //法线方向
            float3 normalDirOnRimLight = lerp(normalDirWS, normalDirTex, _ToggleNormalMapOnRimLight); //法线开关

            //边缘光系数
            float rimLightNdotV = saturate(1 - dot(normalDirOnRimLight, viewDirWS)); //NdotV
            float rimLightFactor = pow(rimLightNdotV, exp2(lerp(3, 0, _FloatRimLightIntensity))); //强度控制
            //距离计算 & 硬边缘开关
            rimLightFactor = saturate(lerp((rimLightFactor - _FloatRimLightInsideDistance) / (1 - _FloatRimLightInsideDistance), step(_FloatRimLightInsideDistance, rimLightFactor), _ToggleRimLightHard));

            #if defined(_RIMLIGHT_SHADEMASK_ON)
                //开关 光照水平锁定
                float3 lightDirOnRimLight = lightDirWS;
                lightDirOnRimLight.y = lerp(lightDirOnRimLight.y, 0, _ToggleLightHorLockRimLight);
                float rimLightShadeIntensity = dot(normalDirWS, lightDirOnRimLight);
                //暗部遮罩
                float distanceOffset = exp2((4 * (1 - _FloatRimLightInsideDistance))); //根据内部距离 变化遮罩强度
                float shadeMaskIntensity = min(rimLightShadeIntensity * _FloatRimLightShadeMaskIntensity * distanceOffset, 0); //暗部遮罩强度
                float shadeMaskFactor = saturate(rimLightFactor + shadeMaskIntensity); //暗部遮罩强度 计算
                //暗部遮罩开关
                colorRimLight *= shadeMaskFactor;

                //暗部遮罩颜色
                #if defined(_RIMLIGHT_SHADEMASK_COLOR_ON)
                    //暗部颜色 全局光照开关
                    half3 colorRimLightShade = lerp(_ColorRimLightShadeColor.rgb, _ColorRimLightShadeColor.rgb * colorLightBlend, _ToggleGlobalLightRimLightShade);
                    colorRimLightShade *= _ColorRimLightShadeColor.a; //透明度
                    //暗部边缘光系数
                    float rimLightShadeFactor = pow(rimLightNdotV, exp2(lerp(3, 0, _FloatRimLightShadeColorIntensity)));
                    //距离计算 & 硬边缘开关
                    rimLightShadeFactor = saturate(lerp((rimLightShadeFactor - _FloatRimLightInsideDistance) / (1 - _FloatRimLightInsideDistance), step(_FloatRimLightInsideDistance, rimLightShadeFactor), _ToggleRimLightShadeColorHard));
                    //暗部遮罩强度 计算
                    rimLightShadeFactor = saturate(rimLightShadeFactor - saturate(rimLightShadeIntensity)) * (1 - shadeMaskFactor); //计算rimLightShadeIntensity近对暗部生效 计算shadeMaskFactor使暗部颜色强度随暗部遮罩强度变化
                    //暗部颜色开关
                    colorRimLight = colorRimLight + colorRimLightShade * rimLightShadeFactor;
                #endif
            #else
                //边缘光颜色 计算
                colorRimLight *= rimLightFactor;
            #endif

            #if defined(_RIMLIGHT_MASKMAP_ON)
            //边缘光 遮罩贴图
            float rimlightMaskValue = SAMPLE_TEXTURE2D(_TexRimLightMaskMap, sampler_TexRimLightMaskMap, TRANSFORM_TEX(uv0, _TexRimLightMaskMap)).r;
            //遮罩贴图强度 计算
            colorRimLight = lerp(colorRimLight, colorRimLight * rimlightMaskValue, _FloatRimLightMaskMapIntensity);
            #endif

            //混合颜色 +=边缘光颜色
            colorFinalBlend = lerp(colorFinalBlend, colorFinalBlend + colorRimLight, _ToggleRimLight);
        #endif
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 边缘光 RimLight ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
            
//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 材质捕获 Matcap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        #if defined(_MATCAP_ON)
            //uv坐标计算
            float3 normalDirOnMatCap = lerp(normalDirWS, normalDirTex, _ToggleNormalMapOnMatCap); //法线贴图开关
            float2 uvMatCap = mul(UNITY_MATRIX_V, float4(normalDirOnMatCap, 1)).xy; //转换至观察空间
            uvMatCap = uvMatCap * 0.5 + 0.5; //1~1映射至0~1 用于UV采样
            uvMatCap = RotateUV(uvMatCap, _FloatMatCapRotate * 3.141592654, float2(0.5, 0.5)); //uv旋转
            //MatCap贴图采样
            half3 colorMatCapMap = SAMPLE_TEXTURE2D(_TexMatCapMap, sampler_TexMatCapMap, TRANSFORM_TEX(uvMatCap, _TexMatCapMap)).rgb;
            colorMatCapMap *= _ColorMatCapMapColor.rgb * _ColorMatCapMapColor.a; //自定义色 混合

            //全局光照开关
            colorMatCapMap = lerp(colorMatCapMap, colorMatCapMap * colorLightBlend, _ToggleGlobalLightMatCapMap);

            //阴影遮罩
            float matcapShadowMaskIntensity = (1 - lightIntensityShade1) + (lightIntensityShade1 * (1 - _FloatMatCapShadowMaskIntensity)); //阴影遮罩强度
            colorMatCapMap = lerp(colorMatCapMap, colorMatCapMap * matcapShadowMaskIntensity, _ToggleMatCapShadowMask); //阴影遮罩 开关

            //颜色混合 模式
            half3 colorMatCapFinal = half3(1, 1, 1);
            #if defined(_MATCAP_COLORBLEND_ADDITIVE)
                colorMatCapFinal = colorFinalBlend + colorMatCapMap * _FloatMatCapColorBlendIntensity;
            #elif defined(_MATCAP_COLORBLEND_MULTIPLY)
                colorMatCapFinal = lerp(colorFinalBlend, colorFinalBlend * colorMatCapMap, _FloatMatCapColorBlendIntensity);
            #elif defined(_MATCAP_COLORBLEND_LERP)
                colorMatCapFinal = lerp(colorFinalBlend, colorMatCapMap, _FloatMatCapColorBlendIntensity);
            #endif

            //遮罩贴图
            float matCapMaskIntensity = SAMPLE_TEXTURE2D(_TexMatCapMaskMap, sampler_TexMatCapMaskMap, TRANSFORM_TEX(uv0, _TexMatCapMaskMap)).r;
            matCapMaskIntensity = saturate(matCapMaskIntensity + _FloatMatCapMaskMapIntensity); //遮罩强度偏移
            colorFinalBlend = lerp(colorFinalBlend, colorMatCapFinal, matCapMaskIntensity);
        #endif
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 材质捕获 Matcap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 自发光 Emissive ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        #if defined(_EMISSIVE_ON)
            #if defined(_EMISSIVE_FIXED) //◆◆◆2 自发光固定
                //纹理采样
                float4 colorEmissive = SAMPLE_TEXTURE2D(_TexEmissiveMap, sampler_TexEmissiveMap, TRANSFORM_TEX(uv0, _TexEmissiveMap));
                //自发光强度
                float emissiveIntensity = colorEmissive.a;
                //计算 光照颜色
                half3 colorEmissiveFinal = colorEmissive.rgb * _ColorEmissiveMapColor.rgb * emissiveIntensity;

                //混合颜色 +=自发光
                colorFinalBlend += colorEmissiveFinal;
            #elif defined(_EMISSIVE_ANIM) //◆◆◆2 自发光动画
                //UV比例模式
                float2 uvEmissiveMatCap = mul(UNITY_MATRIX_V, float4(normalDirWS, 1)).xy; //转换至观察空间
                uvEmissiveMatCap = uvEmissiveMatCap * 0.5 + 0.5; //1~1映射至0~1 用于UV采样
                float2 uvEmissive = lerp(uv0, uvEmissiveMatCap, _FloatEmissiveAnimUVType);

                //移动速度
                float timeValue = _Time.y;
                float emissiveMoveSpeed = timeValue * _FloatEmissiveAnimSpeed;
                //开关 来回移动
                emissiveMoveSpeed = lerp(emissiveMoveSpeed, sin(emissiveMoveSpeed), _ToggleEmissiveAnimPingpong);
                //旋转速度
                float uvEmissiveRotate = _FloatEmissiveAnimRotate * 3.141592654 * emissiveMoveSpeed;
                uvEmissive = RotateUV(uvEmissive, uvEmissiveRotate, float2(0.5, 0.5));
                //移动方向
                float2 emissiveMoveDir = float2(_FloatEmissiveAnimDirU, _FloatEmissiveAnimDirV);
                uvEmissive = uvEmissive - emissiveMoveDir * emissiveMoveSpeed;

                //自发光颜色
                half3 colorEmissiveBlend = _ColorEmissiveMapColor;
                    
                //变化颜色
                float colorChangeFactor = cos(_FloatEmissiveChangeSpeed * timeValue) * 0.5 + 0.5; //cos曲线动画 映射至0-1来回变化
                half3 colorChange = lerp(colorEmissiveBlend, _ColorEmissiveChangeColor, colorChangeFactor); //颜色混合
                //变化颜色 开关
                colorEmissiveBlend = lerp(colorEmissiveBlend, colorChange, _ToggleEmissiveChangeColor);

                //视角变化颜色
                float3 normalDirOnEmissive = lerp(normalDirWS, normalDirTex, _ToggleNormalMapOnEmissive); //基础贴图法线方向 法线开关
                float colorViewChangeNdotV = 1 - saturate(dot(normalDirOnEmissive, viewDirWS));
                half3 colorViewChange = lerp(colorEmissiveBlend, _ColorEmissiveViewChangeColor, colorViewChangeNdotV);
                //视角变化颜色 开关
                colorEmissiveBlend = lerp(colorEmissiveBlend, colorViewChange, _ToggleEmissiveViewChangeColor);

                //纹理采样 动画uv
                half3 colorEmissive = SAMPLE_TEXTURE2D(_TexEmissiveMap, sampler_TexEmissiveMap, TRANSFORM_TEX(uvEmissive, _TexEmissiveMap)).rgb;
                //自发光强度
                float emissiveIntensity = SAMPLE_TEXTURE2D(_TexEmissiveMap, sampler_TexEmissiveMap, TRANSFORM_TEX(uv0, _TexEmissiveMap)).a;

                //自发光颜色 混合
                half3 colorEmissiveFinal = colorEmissive * colorEmissiveBlend * emissiveIntensity;

                //最小Alpha值限制
                colorEmissiveFinal *= step(0.005, emissiveIntensity);

                //混合颜色 +=自发光
                colorFinalBlend += colorEmissiveFinal;
            #endif //◆◆◆2
        #endif
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 自发光 Emissive ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

        //透明度
        half alphaValue = colorBaseMap.a;
        half4 colorFinal = half4(colorFinalBlend, alphaValue);

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 裁剪 Clip ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        
        #if defined(_CLIP_OFF) || defined(_CLIP_DITHER) //◆◆◆1 关闭 || 挖孔
            //
        #elif defined(_CLIP_ALPHA) //◆◆◆1 透明度
            alphaValue = saturate(clipValue);
        #endif //◆◆◆1
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 裁剪 Clip ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

        //表面类型
        #if defined(_SURFACETYPE_OPAQUE)
            colorFinal = half4(colorFinal.rgb, 1);
        #elif defined(_SURFACETYPE_TRANSPARENT)
            colorFinal = half4(colorFinal.rgb, alphaValue);
        #else
            colorFinal = half4(colorFinal.rgb, 1);
        #endif

        return colorFinal;
    }

