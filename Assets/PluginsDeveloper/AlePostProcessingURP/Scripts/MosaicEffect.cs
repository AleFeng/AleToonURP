using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/Mosaic")]
	public class MosaicEffect : CustomVolumeComponent
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
	}

	[CustomPostProcess("Able/Mosaic", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class MosaicEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/Mosaic");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<MosaicEffect>();

            SetFloat("_MainTexOffestIntensity", volume.m_MainTexOffestIntensity);
            SetFloat("_MainTexOffestRandom", volume.m_MainTexOffestRandom);

            SetFloat("_PixelSize", volume.m_PixelSize);
            SetVector("_PixelRatio", volume.m_PixelRatio);
            SetFloat("_PixelRandom", volume.m_PixelRandom);

			DrawFullScreen();
        }
    }
}



