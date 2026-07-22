<p align="center">
  <img width="1000" src="Document~/Image/Demo_Kafka_1.jpg">
</p>

<p align="center">
  <img alt="Version" src="https://img.shields.io/badge/version-1.4.0-blue">
  <img alt="Unity" src="https://img.shields.io/badge/Unity-2022.3-black?logo=unity">
  <img alt="URP" src="https://img.shields.io/badge/URP-14.x-blue">
  <img alt="License" src="https://img.shields.io/badge/license-MIT-blueviolet">
</p>

<p align="center">
  🌍
  <a href="./README.md">中文</a> |
  <a href="./README_EN.md">English</a> |
  日本語
</p>

<p align="center">
  📥
  <a href="#-ユーザーマニュアル">ユーザーマニュアル</a> |
  <a href="#-豊富なレンダリング機能">機能一覧</a> |
  <a href="CHANGELOG.md">変更履歴</a>
</p>

# AleToonURP
Unity URP 向けの `高性能` な `NPR`（二次元トゥーン）レンダリングパイプライン & シェーダーです。\
ほとんどの二次元レンダリングでよく使われる美術表現を実装し、PC・モバイルを問わず幅広いプラットフォームで動作します。

## 📜 目次
- [概要](#-概要)
- [主な特徴](#-主な特徴)
- [環境要件](#-環境要件)
- [インストール](#-インストール)
- [ユーザーマニュアル](#-ユーザーマニュアル)
- [多様なアートスタイル](#-多様なアートスタイル)
- [NPRスタイルの水面](#-nprスタイルの水面)
- [効率的なパフォーマンス](#-効率的なパフォーマンス)
  - [1.UberShader](#1ubershader)
  - [2.SRP-Batcher](#2srp-batcher)
- [豊富なレンダリング機能](#-豊富なレンダリング機能)
  - [1.基本設定 Basic](#1基本設定-basic)
  - [2.基本マップ BaseMap](#2基本マップ-basemap)
  - [3.法線マップ NormalMap](#3法線マップ-normalmap)
  - [4.外側の描線 Outline](#4外側の描線-outline)
  - [5.縁辺の光 RimLight](#5縁辺の光-rimlight)
  - [6.ハイライト HighLight](#6ハイライト-highlight)
  - [7.発光 Emissive](#7発光-emissive)
  - [8.マテリアルキャプチャ MatCap](#8マテリアルキャプチャ-matcap)
  - [9.ライトの設定 Light Setting](#9ライトの設定-light-setting)
- [変更履歴](#-変更履歴)
- [ライセンス](#-ライセンス)

## 📋 概要
これは`Unity2022.3(URP 14.x)`に基づいて開発された`NPR`のレンダリングパイプラインとシェーダー。\
ほとんどの二次元レンダリングでよく使用される美術効果を実現しました。\
できるだけ`高性能`のシェーダーを開発し，ほとんどのプラットフォーム（パソコン、モバイル等）で使用できるようにしました。\
シェーダーの`GUI`も開発し，アーティストにとって使いやすく、便利になりました。

## ✨ 主な特徴
| 特徴 | 説明 |
| :--- | :--- |
| NPRトゥーンレンダリング | 暗部・外描線・リムライト・ハイライト・発光・MatCap など、二次元スタイルでよく使われる美術効果を網羅した `AleToonURP/Lit` シェーダー。 |
| NPRスタイルの水面 | 深度による水色の変化、多層波形、反射、屈折、岸辺の泡沫を備えた `AleToonURP/Water Plane` シェーダー。 |
| UberShader | 前処理マクロにより、マテリアルの効果の開閉に応じたシェーダー変体を自動生成し、必要な機能だけを有効化。 |
| カスタム ShaderGUI | 折りたたみ式の分かりやすい GUI で機能を順に設定でき、アーティストが効率よくマテリアルを制作可能。 |
| 高効率なパフォーマンス | SRP-Batcher / GPU-Instance などのバッチレンダリングに対応し、DrawCall を最適化。 |
| マルチプラットフォーム | PC・モバイル等、ほとんどのプラットフォームで動作するように高性能に設計。 |

## 💻 環境要件
- `Unity 2022.3`（URP 14.x）に基づいて開発・検証されています。
- レンダリングパイプライン: `Universal Render Pipeline (URP) 14.x`
- 対応プラットフォーム: シェーダーに対応した PC・モバイル等のほとんどのプラットフォーム。

## 📦 インストール
Unity で `Window → Package Manager` を開き、左上の `+ → Add package from git URL...` をクリックして、以下の URL を入力します：

```
https://github.com/AleFeng/AleToonURP.git?path=Assets/PluginsDeveloper/AleToonURP
```

インストール後、Package Manager の本パッケージのページで `Samples` を展開し、サンプルのマテリアルとテクスチャをインポートできます。

> [!IMPORTANT]
> **アウトライン機能は [OutlineSmoothNormalsGenerator](https://github.com/AleFeng/OutlineSmoothNormalsGenerator) に依存します**（`1.8.1` 推奨、最低 `1.7.0`）。同じ方法で先にインストールしてください：
> `https://github.com/AleFeng/OutlineSmoothNormalsGenerator.git?path=/Packages/com.alefeng.outlinesmoothnormalsgenerator`
>
> 未インストールの場合、`AleToonURP/Lit` はコンパイルできません。このツールはスムース法線をメッシュにベイクします。**ベイク時に選んだ「保存方式」と「保存空間」は、マテリアルのアウトラインパネルの「スムース法線ソース」と「保存空間」と一致させる必要があります** —— 不一致でもエラーにはならず、アウトラインが乱れる／全体的に傾くだけです。

<br/>
<br/>

## 📖 ユーザーマニュアル
**[中文版用户手册](Document~/UserManual.md)**\
**[English User Manual](Document~/UserManual_en.md)**\
ユーザーマニュアルはシェーダーの全て機能の`特性`や`使い方`を詳しく説明します。

> [!TIP]
> AleToonURPの使用を始める前に、ユーザーマニュアルを参照することをお勧めします。`速く理解し`、習得するために。

<br/>
<br/>

## 🎨 多様なアートスタイル
マテリアルのプロパティを調整することで、様々な`アートスタイル`のレンダリングを便利に実現できます。

> [!TIP]
> 作成を始める前に、まずは`目標とするアートスタイル`か、`適切なリファレンスイメージ`を見つけることをお勧めします。\
> そして、目標とするアートスタイルに合わせて、マテリアルのプロパティを`対応的に調整します`。\
> それにより、確実で優れた美術効果を達成し易くなります。

▼ AleToonURP/Lit\
<img width = "1000" src="Document~/Image/Demo_Kafka_1.jpg">\
▼ Universal Render Pipeline/Lit（汎用レンダリングパイプライン）\
<img width = "1000" src="Document~/Image/Demo_Kafka_0.jpg">

▼ AleToonURP/Lit\
<img width = "1000" src="Document~/Image/Demo_Klee_1.jpg">\
▼ Universal Render Pipeline/Lit（汎用レンダリングパイプライン）\
<img width = "1000" src="Document~/Image/Demo_Klee_0.jpg">

<br/>
<br/>

## 🌊 NPRスタイルの水面
NPRスタイルの水面のレンダリングを実現しました。\
`水中深度`のおうじて、水の色を徐々に`変化させる`。浅水の色や深水の色、透明度等を自由に調整できます。\
`複数の波`を重ねることで、繊細で自然な水面の`波浪の効果`を得られます。第一と第二波形の大小や強度、移動速度等を調整できます。\
キューブマップを使用して水面の`シーン反射効果`を表現します。反射の強度や模糊度、フレネル効果等を調整できます。\
カメラのフレームバッファをサンプリングし、画面の捩れと変位をすることで、水中の物を`屈折させる効果`を実現しました。\
カメラのデプスバッファをサンプリングし、水面と岸辺に接するの`辺縁部分の泡沫`を実現しました。マスクマップや範囲、距離、模糊度、透明度等を調整できます。\
▼ AleToonURP/Water Plane\
<img width = "1000" src="Document~/Gif/Demo_WaterPlane.gif">

<br/>
<br/>

## ⚡ 効率的なパフォーマンス
効率な方法でシェーダーを開発し、性能の最適化も行いました。SRP-BatcherやGPU-Instance等のバッチレンダリング方式もできます。

### 1.UberShader
前処理マクロの方法を使用し、Uberシェーダーを開発しました。\
マテリアルの効果の開閉に基づいて、対応なシェーダーの変体を`自動的に生成`します。\
シェーダーの便利さを向上し、アーティストが`簡単で効率的`にマテリアルを作成できるようになりました。\
<img src="Document~/Image/Macro Def_0.jpg">

### 2.SRP-Batcher
UnityのSRP-Batcher機能もでき、CPU側でDrawCallのプリセットの効率を向上させます。\
<img width = "1000" src="Document~/Image/SRPBatcher_0.jpg">

<br/>
<br/>

## 🧩 豊富なレンダリング機能
ほとんどの二次元スタイルのレンダリング効果を実現し、カスタムのシェーダーGUIも開発しました。
マテリアルを制作する時のワークフローを`確実で効率的`になりました。\
インタフェースの折りたたみバーの順番を基づいて、マテリアルのレンダリング機能を`逐次設定する`こともできます。\
美術効果の要望を応じて、`個別`に`開ける`又は`閉める`こともできます。

<img src="Document~/Image/ShaderGUI_0.1_All.jpg">

<br/>
<br/>

### 1.基本設定 Basic
<img src="Document~/Image/ShaderGUI_1.0_Basic.jpg">

#### 1.1.レンダリング隊列 Render Queue
<img src="Document~/Image/ShaderGUI_1.1_RenderQueue.jpg">

#### 1.2.裁断 Clip
<img src="Document~/Image/ShaderGUI_1.2_Clip.jpg">

#### 1.3.ステンシルテスト　Stencil
<img src="Document~/Image/ShaderGUI_1.3_Stencil.jpg">

<br/>
<br/>

### 2.基本マップ BaseMap
<img src="Document~/Image/ShaderGUI_2.0_BaseMap.jpg">

#### 2.1.暗部の閾値マップ Shade ThresholdMap
<img src="Document~/Image/ShaderGUI_2.1_ShadeThreshold.jpg">

<br/>
<br/>

### 3.法線マップ NormalMap
<img src="Document~/Image/ShaderGUI_3.0_NormalMap.jpg">

<br/>
<br/>

### 4.外側の描線 Outline
<img src="Document~/Image/ShaderGUI_4.0_Outline.jpg">

#### 4.1.描線の肌理マップ TexMap
<img src="Document~/Image/ShaderGUI_4.1_OutlineTexMap.jpg">

<br/>
<br/>

### 5.縁辺の光 RimLight
<img src="Document~/Image/ShaderGUI_5.0_RimLight.jpg">

#### 5.1.暗部のマスクマップ ShadeMask
<img src="Document~/Image/ShaderGUI_5.1_RimLightShadeMask.jpg">

#### 5.2.マスクマップ MaskMap
<img src="Document~/Image/ShaderGUI_5.2_RimLightMaskMap.jpg">

<br/>
<br/>

### 6.ハイライト HighLight
<img src="Document~/Image/ShaderGUI_6.0_HighLight.jpg">

#### 6.1.マスクマップ MaskMap
<img src="Document~/Image/ShaderGUI_6.1_HighLightMaskMap.jpg">

<br/>
<br/>

### 7.発光 Emissive
<img src="Document~/Image/ShaderGUI_7.0_Emissive.jpg">

#### 7.1.発光のアニメーション Emissive Animation
<img src="Document~/Image/ShaderGUI_7.1_EmissiveAnim.jpg">

<br/>
<br/>

### 8.マテリアルキャプチャ MatCap
<img src="Document~/Image/ShaderGUI_8.0_MatCap.jpg">

#### 8.1.マスクマップ MaskMap
<img src="Document~/Image/ShaderGUI_8.1_MatCapMaskMap.jpg">

<br/>
<br/>

### 9.ライトの設定 Light Setting
<img src="Document~/Image/ShaderGUI_9.0_LightSetting.jpg">

#### 9.1.アディショナルライト Additive Light
<img src="Document~/Image/ShaderGUI_9.1_LightSettingAdd.jpg">

#### 9.2.ライトの開閉 Light Toggle
<img src="Document~/Image/ShaderGUI_9.2_LightSettingToggle.jpg">

#### 9.3.陰影の設定 Shadow Setting
<img src="Document~/Image/ShaderGUI_9.3_LightSettingShadow.jpg">

#### 9.4.内蔵ライト BuiltIn Light
<img src="Document~/Image/ShaderGUI_9.3_LightSettingBuiltIn.jpg">

#### 9.5.ライト方向の固定 Direction Lock
<img src="Document~/Image/ShaderGUI_9.4_LightSettingDirLock.jpg">

<br/>
<br/>

## 📝 変更履歴
更新内容の詳細は [CHANGELOG.md](CHANGELOG.md) を参照してください。

## 📄 ライセンス
本プロジェクトは [MIT License](LICENSE) の下で公開されています。
