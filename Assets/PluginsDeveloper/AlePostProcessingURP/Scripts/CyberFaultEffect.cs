using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AlePostProcessUPR
{
    [System.Serializable, VolumeComponentMenu("Able/CyberFault")]
    public class CyberFaultEffect : CustomVolumeComponent
    {
        //主纹理偏移
        [Range(0f, 1f), Tooltip("MainTex Offest Intensity")]
        public FloatParameter m_MainTexOffestIntensity = new FloatParameter(0.01f); //偏移强度
        [Tooltip("MainTex Offest Random")]
        public FloatParameter m_MainTexOffestRandom = new FloatParameter(1f); //偏移随机

        //像素块
        [Tooltip("Pixel Size")]
        public FloatParameter m_PixelSize = new FloatParameter(20f); //像素尺寸
        [Tooltip("Pixel Ratio")]
        public Vector2Parameter m_PixelRatio = new Vector2Parameter(Vector2.one); //像素高宽比
        [Tooltip("Pixel Random")]
        public FloatParameter m_PixelRandom = new FloatParameter(1f); //像素块随机值

        //像素块剔除
        [Range(0f, 1f), Tooltip("Cull Amount")]
        public FloatParameter m_CullAmount = new FloatParameter(0.9f); //剔除数
        [Tooltip("Cull Random")]
        public FloatParameter m_CullRandom = new FloatParameter(1f); //剔除随机值

        //色彩分离
        [Range(0f, 1f), Tooltip("ColorSplit MainTexture Amount")]
        public FloatParameter m_ColorSplitMainTexAmount = new FloatParameter(0.01f); //色彩分离 强度 主纹理
        [Range(0f, 1f), Tooltip("ColorSplit Pixel Amount")]
        public FloatParameter m_ColorSplitPixelAmount = new FloatParameter(0.01f); //色彩分离 强度 像素块
        [Range(0f, 1f), Tooltip("ColorSplit Add Random")]
        public FloatParameter m_ColorSplitAddRandom = new FloatParameter(0.01f); //色彩分离 强度 随机

        //动画
        [Tooltip("Anim Enable")]
        public BoolParameter m_AnimEnable = new BoolParameter(false); //动画 开关
        [Tooltip("Anim Speed")]
        public FloatParameter m_AnimSpeed = new FloatParameter(10f); //动画 速度
        [Tooltip("Anim PixelScale Random")]
        public FloatParameter m_AnimPixelScaleRandom = new FloatParameter(0.5f); //像素块尺寸 随机变化
    }

    [CustomPostProcess("Able/CyberFault", CustomPostProcessInsertPoint.AfterPostProcess)]
    public class CyberFaultEffectRenderer : CustomPostProcessRenderer
    {
        public override void Initialize()
        {
            InitPostProcessMaterial("Hidden/Custom/CyberFault");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<CyberFaultEffect>();

            SetFloat("_MainTexOffestIntensity", volume.m_MainTexOffestIntensity);
            SetFloat("_MainTexOffestRandom", volume.m_MainTexOffestRandom);

            SetFloat("_PixelSize", volume.m_PixelSize);
            SetVector("_PixelRatio", volume.m_PixelRatio);
            SetFloat("_PixelRandom", volume.m_PixelRandom);

            SetFloat("_CullAmount", volume.m_CullAmount);
            SetFloat("_CullRandom", volume.m_CullRandom);

            SetFloat("_ColorSplitMainTexAmount", volume.m_ColorSplitMainTexAmount);
            SetFloat("_ColorSplitPixelAmount", volume.m_ColorSplitPixelAmount);
            SetFloat("_ColorSplitAddRandom", volume.m_ColorSplitAddRandom);

            SetBool("_AnimEnable", volume.m_AnimEnable);
            SetFloat("_AnimSpeed", volume.m_AnimSpeed);
            SetFloat("_AnimPixelScaleRandom", volume.m_AnimPixelScaleRandom);

            DrawFullScreen();
        }
    }
}



