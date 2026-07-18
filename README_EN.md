<p align="center">
  <img width="1000" src="Document~/Image/Demo_Kafka_1.jpg">
</p>

<p align="center">
  <img alt="Version" src="https://img.shields.io/badge/version-1.2.2-blue">
  <img alt="Unity" src="https://img.shields.io/badge/Unity-2022.3-black?logo=unity">
  <img alt="URP" src="https://img.shields.io/badge/URP-14.x-blue">
  <img alt="License" src="https://img.shields.io/badge/license-MIT-blueviolet">
</p>

<p align="center">
  🌍
  <a href="./README.md">中文</a> |
  English |
  <a href="./README_JA.md">日本語</a>
</p>

<p align="center">
  📥
  <a href="#-user-manual">User Manual</a> |
  <a href="#-rich-rendering-features">Features</a> |
  <a href="CHANGELOG.md">Changelog</a>
</p>

# AleToonURP
A `high-performance` `NPR` (anime / toon) rendering pipeline & shader for Unity URP.\
It implements most of the art effects commonly used in anime-style rendering, and runs on most platforms including PC and mobile.

## 📜 Table of Contents
- [Overview](#-overview)
- [Key Features](#-key-features)
- [Requirements](#-requirements)
- [Installation](#-installation)
- [User Manual](#-user-manual)
- [Diverse Art Styles](#-diverse-art-styles)
- [NPR-Style Water Surface](#-npr-style-water-surface)
- [Efficient Performance](#-efficient-performance)
  - [1.UberShader](#1ubershader)
  - [2.SRP-Batcher](#2srp-batcher)
- [Rich Rendering Features](#-rich-rendering-features)
  - [1.Basic](#1basic)
  - [2.BaseMap](#2basemap)
  - [3.NormalMap](#3normalmap)
  - [4.Outline](#4outline)
  - [5.RimLight](#5rimlight)
  - [6.HighLight](#6highlight)
  - [7.Emissive](#7emissive)
  - [8.MatCap](#8matcap)
  - [9.Light Setting](#9light-setting)
- [Changelog](#-changelog)
- [License](#-license)

## 📋 Overview
AleToonURP is an `NPR` anime / toon rendering pipeline and shader developed on `Unity 2022.3 (URP 14.x)`.\
It implements most of the art effects commonly used in anime-style rendering.\
The shaders are written to be as `high-performance` as possible, so they can run on most platforms (console and mobile).\
A custom `Shader GUI` is also provided, making the shaders convenient and easy to use for artists.

## ✨ Key Features
| Feature | Description |
| :--- | :--- |
| NPR Toon Rendering | The `AleToonURP/Lit` shader covers common anime-style art effects: shade, outline, rim light, highlight, emissive, MatCap, and more. |
| NPR-Style Water | The `AleToonURP/Water Plane` shader features depth-based water color, multi-layer waves, reflection, refraction, and shoreline foam. |
| UberShader | Uses preprocessor macros to auto-generate the matching shader variant based on the effect toggles on the material, enabling only what you need. |
| Custom ShaderGUI | A clear, foldout-based GUI lets you configure features step by step, so artists can author materials efficiently. |
| Efficient Performance | Supports batched rendering such as SRP-Batcher / GPU-Instance to optimize DrawCalls. |
| Multi-Platform | Designed for high performance to run on most platforms, including PC and mobile. |

## 💻 Requirements
- Developed and validated on `Unity 2022.3` (URP 14.x).
- Render pipeline: `Universal Render Pipeline (URP) 14.x`
- Supported platforms: most shader-compatible platforms, including PC and mobile.

## 📦 Installation
Open `Window → Package Manager` in Unity, click `+ → Add package from git URL...` in the top-left corner, and enter:

```
https://github.com/AleFeng/AleToonURP.git?path=Assets/PluginsDeveloper/AleToonURP
```

After installation, expand `Samples` on the package page in Package Manager to import the example materials and textures.

<br/>
<br/>

## 📖 User Manual
**[中文版用户手册](Document~/UserManual.md)**\
**[English User Manual](Document~/UserManual_en.md)**\
The user manual explains in detail the `features` and `usage` of every function in the Shader GUI.

> [!TIP]
> Before you start using AleToonURP, we recommend reading the user manual to `get up to speed` quickly and master it.

<br/>
<br/>

## 🎨 Diverse Art Styles
By editing the material properties, you can conveniently achieve a wide variety of `art styles`.

> [!TIP]
> Before you start creating, we recommend first `defining your target art style` or finding `a suitable reference image`.\
> Then edit the material properties `specifically` to match that target art style.\
> This makes it easier to achieve accurate and excellent art results.

▼ AleToonURP/Lit\
<img width = "1000" src="Document~/Image/Demo_Kafka_1.jpg">\
▼ Universal Render Pipeline/Lit\
<img width = "1000" src="Document~/Image/Demo_Kafka_0.jpg">

▼ AleToonURP/Lit\
<img width = "1000" src="Document~/Image/Demo_Klee_1.jpg">\
▼ Universal Render Pipeline/Lit\
<img width = "1000" src="Document~/Image/Demo_Klee_0.jpg">

<br/>
<br/>

## 🌊 NPR-Style Water Surface
Implements NPR-style water surface rendering.\
The water color changes by `interpolation` according to the `underwater depth`. Shallow-water color, deep-water color, and transparency are all customizable.\
Overlapping `multiple wave layers` produces delicate and natural `surface waves`. The scale, intensity, and movement speed of the primary and secondary waves are adjustable.\
A CubeMap is used to represent the water's `scene reflection`. Reflection intensity, blur, and Fresnel effect are adjustable.\
By sampling the camera's frame buffer and applying distortion and offset, the `refraction` of underwater objects is achieved.\
Using the camera's depth buffer to obtain underwater depth, `edge foam` is achieved where the water meets the shore. Threshold map, threshold clip, distance, blur, and transparency are all customizable.\
▼ AleToonURP/Water Plane\
<img width = "1000" src="Document~/Gif/Demo_WaterPlane.gif">

<br/>
<br/>

## ⚡ Efficient Performance
The shaders are written efficiently and performance-optimized. Batched rendering methods such as SRP-Batcher and GPU-Instance are supported.

### 1.UberShader
The UberShader is built using preprocessor macro branching.\
Based on the effect toggles on the material, the corresponding shader variant is `generated automatically`.\
This improves the shader's usability, letting artists author materials more `simply and efficiently`.\
<img src="Document~/Image/Macro Def_0.jpg">

### 2.SRP-Batcher
Supports Unity's SRP-Batcher, improving the efficiency of CPU-side DrawCall preparation.\
<img width = "1000" src="Document~/Image/SRPBatcher_0.jpg">

<br/>
<br/>

## 🧩 Rich Rendering Features
Implements most mainstream toon-rendering effects, along with a custom ShaderGUI, making the material authoring workflow `clear and efficient`.\
You can configure each `rendering feature` of the material `step by step` following the order of the foldout interface. Enable or disable each one `individually` as your art needs require.

<img src="Document~/Image/ShaderGUI_0.1_All.jpg">

<br/>
<br/>

### 1.Basic
<img src="Document~/Image/ShaderGUI_1.0_Basic.jpg">

#### 1.1.Render Queue
<img src="Document~/Image/ShaderGUI_1.1_RenderQueue.jpg">

#### 1.2.Clip
<img src="Document~/Image/ShaderGUI_1.2_Clip.jpg">

#### 1.3.Stencil
<img src="Document~/Image/ShaderGUI_1.3_Stencil.jpg">

<br/>
<br/>

### 2.BaseMap
<img src="Document~/Image/ShaderGUI_2.0_BaseMap.jpg">

#### 2.1.Shade ThresholdMap
<img src="Document~/Image/ShaderGUI_2.1_ShadeThreshold.jpg">

<br/>
<br/>

### 3.NormalMap
<img src="Document~/Image/ShaderGUI_3.0_NormalMap.jpg">

<br/>
<br/>

### 4.Outline
<img src="Document~/Image/ShaderGUI_4.0_Outline.jpg">

#### 4.1.TexMap
<img src="Document~/Image/ShaderGUI_4.1_OutlineTexMap.jpg">

<br/>
<br/>

### 5.RimLight
<img src="Document~/Image/ShaderGUI_5.0_RimLight.jpg">

#### 5.1.ShadeMask
<img src="Document~/Image/ShaderGUI_5.1_RimLightShadeMask.jpg">

#### 5.2.MaskMap
<img src="Document~/Image/ShaderGUI_5.2_RimLightMaskMap.jpg">

<br/>
<br/>

### 6.HighLight
<img src="Document~/Image/ShaderGUI_6.0_HighLight.jpg">

#### 6.1.MaskMap
<img src="Document~/Image/ShaderGUI_6.1_HighLightMaskMap.jpg">

<br/>
<br/>

### 7.Emissive
<img src="Document~/Image/ShaderGUI_7.0_Emissive.jpg">

#### 7.1.Emissive Animation
<img src="Document~/Image/ShaderGUI_7.1_EmissiveAnim.jpg">

<br/>
<br/>

### 8.MatCap
<img src="Document~/Image/ShaderGUI_8.0_MatCap.jpg">

#### 8.1.MaskMap
<img src="Document~/Image/ShaderGUI_8.1_MatCapMaskMap.jpg">

<br/>
<br/>

### 9.Light Setting
<img src="Document~/Image/ShaderGUI_9.0_LightSetting.jpg">

#### 9.1.Additive Light
<img src="Document~/Image/ShaderGUI_9.1_LightSettingAdd.jpg">

#### 9.2.Light Toggle
<img src="Document~/Image/ShaderGUI_9.2_LightSettingToggle.jpg">

#### 9.3.Shadow Setting
<img src="Document~/Image/ShaderGUI_9.3_LightSettingShadow.jpg">

#### 9.4.BuiltIn Light
<img src="Document~/Image/ShaderGUI_9.3_LightSettingBuiltIn.jpg">

#### 9.5.Direction Lock
<img src="Document~/Image/ShaderGUI_9.4_LightSettingDirLock.jpg">

<br/>
<br/>

## 📝 Changelog
For detailed update notes, see [CHANGELOG.md](CHANGELOG.md).

## 📄 License
This project is open-sourced under the [MIT License](LICENSE).
