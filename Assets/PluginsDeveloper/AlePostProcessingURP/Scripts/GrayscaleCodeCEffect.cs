using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/Grayscale")]
	public class GrayscaleAbleEffect : CustomVolumeComponent
    {
		[Range(0f, 1f), Tooltip("Grayscale effect intensity")]
		public FloatParameter m_Blend = new FloatParameter(0.5f);
	}

	[CustomPostProcess("Able/Grayscale", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class GrayscaleEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/ImageGrayscale");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<GrayscaleAbleEffect>();

            SetFloat("_Blend", volume.m_Blend);

			DrawFullScreen();
        }
    }
}


