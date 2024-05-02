# 【AleToonURP Ver-1.0.0】
- ***阅读中文版用户手册 [中文版本](UserManual.md)***
- ***Read this user manual in [English](UserManual_en.md)***

## 【概述】
---
// TODO

<br/>
<br/>

## 【基础设置】Basic
---
<img src="Image/ShaderGUI_1.0_Basic.jpg">

### 1.渲染队列 Render Queue
<img src="Image/ShaderGUI_1.1_RenderQueue.jpg">

### 2.裁剪 Clip
<img src="Image/ShaderGUI_1.2_Clip.jpg">

### 3.模板测试 Stencil
<img src="Image/ShaderGUI_1.3_Stencil.jpg">

<br/>
<br/>

## 【基础贴图】 BaseMap
---
<img src="Image/ShaderGUI_2.0_BaseMap.jpg">

| `项目` | 类型 | 功能 |
|:----------------------|:----------------------|:----------------------|
| `基础贴图` | 纹理 Texture | 主要的纹理贴图，这将决定材质球外观的主要美术效果。 |
| `混合颜色` | 颜色 Color | 以插值的方式与基础贴图颜色进行混合。可以很方便得调整基础贴图的颜色。 |
| `混合强度` | 浮点数 Float | 与基础贴图进行颜色混合的强度。 |
| `暗部1颜色` | 颜色 Color | 设置材质球暗部1的颜色。 |
| `暗部2颜色` | 颜色 Color | 设置材质球暗部2的颜色。 |
| `亮部→暗部1 : 位置` | 浮点数 Float | 亮部到暗部1的分界线的位置，通常设置在0.5的位置。 |
| `亮部→暗部1 : 模糊` | 浮点数 Float | 亮部到暗部1的分界线的模糊程度。 |
| `暗部1→暗部2 : 位置` | 浮点数 Float | 暗部1到暗部2的分界线的位置。表现效果始终小于等于“亮部→暗部1 : 位置”。 |
| `暗部1→暗部2 : 模糊` | 浮点数 Float | 暗部1到暗部2的分界线的模糊程度。 |

### 1.暗部阈值贴图 Shade ThresholdMap
<img src="Image/ShaderGUI_2.1_ShadeThreshold.jpg">

<br/>
<br/>

## 【法线贴图】 NormalMap
---
<img src="Image/ShaderGUI_3.0_NormalMap.jpg">

<br/>
<br/>

## 【外描边】 Outline
---
<img src="Image/ShaderGUI_4.0_Outline.jpg">

### 1.纹理贴图 TexMap
<img src="Image/ShaderGUI_4.1_OutlineTexMap.jpg">

<br/>
<br/>

## 【边缘光】 RimLight
---
<img src="Image/ShaderGUI_5.0_RimLight.jpg">

### 1.暗部遮罩 ShadeMask
<img src="Image/ShaderGUI_5.1_RimLightShadeMask.jpg">

### 2.遮罩贴图 MaskMap
<img src="Image/ShaderGUI_5.2_RimLightMaskMap.jpg">

<br/>
<br/>

## 【高光】 HighLight
---
<img src="Image/ShaderGUI_6.0_HighLight.jpg">

### 1.遮罩贴图 MaskMap
<img src="Image/ShaderGUI_6.1_HighLightMaskMap.jpg">

<br/>
<br/>

## 【自发光】 Emissive
---
<img src="Image/ShaderGUI_7.0_Emissive.jpg">

### 1.自发光动画 Emissive Animation
<img src="Image/ShaderGUI_7.1_EmissiveAnim.jpg">

<br/>
<br/>

## 【材质捕获】 MatCap
---
<img src="Image/ShaderGUI_8.0_MatCap.jpg">

### 1.遮罩贴图 MaskMap
<img src="Image/ShaderGUI_8.1_MatCapMaskMap.jpg">

<br/>
<br/>

## 【光照设置】 Light Setting
---
<img src="Image/ShaderGUI_9.0_LightSetting.jpg">

### 1.附加光照 Additive Light
<img src="Image/ShaderGUI_9.1_LightSettingAdd.jpg">

### 2.光照开关 Light Toggle
<img src="Image/ShaderGUI_9.2_LightSettingToggle.jpg">

### 3.阴影设置 Shadow Setting
<img src="Image/ShaderGUI_9.3_LightSettingBuiltIn.jpg">

### 4.内置光照 BuiltIn Light
<img src="Image/ShaderGUI_9.3_LightSettingShadow.jpg">

### 5.光照方向锁定 Direction Lock
<img src="Image/ShaderGUI_9.4_LightSettingDirLock.jpg">


