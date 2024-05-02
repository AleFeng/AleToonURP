using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/RadialBlur")]
	public class RadialBlurEffect : CustomVolumeComponent
    {
		[Range(0f, 1f)]
		public FloatParameter BlurRadius = new FloatParameter(0.6f);
		[Range(2, 30)]
		public IntParameter Iteration = new IntParameter(10);
		[Range(0f, 1f)]
		public FloatParameter TransparentAmount = new FloatParameter(1f);
		[Range(0f, 1f)]
		public FloatParameter RadialCenterX = new FloatParameter(0.5f);
		[Range(0f, 1f)]
		public FloatParameter RadialCenterY = new FloatParameter(0.5f);
		[ColorUsage(true, true)]
		public ColorParameter GrayColor = new ColorParameter(new Color(0.0f, 0.0f, 0.0f, 1));
	}

	[CustomPostProcess("Able/RadialBlur", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class RadialBlurEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/RadialBlur");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<RadialBlurEffect>();

            SetVector("_Params", new Vector4(volume.BlurRadius.value * 0.02f, volume.Iteration.value, volume.RadialCenterX.value, volume.RadialCenterY.value));
            SetFloat("_TransparentAmount", volume.TransparentAmount);
            SetColor("_GrayColor", volume.GrayColor);

			DrawFullScreen();
        }
    }
}
