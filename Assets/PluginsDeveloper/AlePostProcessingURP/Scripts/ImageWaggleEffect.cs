using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/ImageWaggle")]
	public class ImageWaggleEffect : CustomVolumeComponent
    {
		[Range(0f, 0.5f), Tooltip("Horizontal Offset Range")]
		public FloatParameter m_HorOffsetRange = new FloatParameter(0.08f);
		[Range(0f, 80f), Tooltip("Horizontal Offset Speed")]
		public FloatParameter m_HorOffsetSpeed = new FloatParameter(7f);
		[Range(0f, 0.5f), Tooltip("Vertical Offset Range")]
		public FloatParameter m_VerOffsetRange = new FloatParameter(0.02f);
		[Range(0f, 80f), Tooltip("Vertical Offset Speed")]
		public FloatParameter m_VerOffsetSpeed = new FloatParameter(14f);

		public Vector2Parameter m_ScaCenter = new Vector2Parameter(new Vector2(0.5f, 0.5f));
		[Tooltip("Scaling Start Time")]
		public FloatParameter m_StartTime = new FloatParameter(0f);
		[Range(0f, 1f), Tooltip("Scaling Visible Start Scale")]
		public FloatParameter m_ScaStartScale = new FloatParameter(0.8f);
		[Range(0f, 1f), Tooltip("Scaling Visible End Scale")]
		public FloatParameter m_ScaEndScale = new FloatParameter(0.4f);
		[Range(0f, 30f), Tooltip("Scaling Total Time")]
		public FloatParameter m_ScaTotalTime = new FloatParameter(5f);
	}

	[CustomPostProcess("Able/ImageWaggle", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class ImageWaggleEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/ImageWaggle");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<ImageWaggleEffect>();

            //偏移参数
            SetFloat("_HorOffsetRange", volume.m_HorOffsetRange);
            SetFloat("_HorOffsetSpeed", volume.m_HorOffsetSpeed);
            SetFloat("_VerOffsetRange", volume.m_VerOffsetRange);
            SetFloat("_VerOffsetSpeed", volume.m_VerOffsetSpeed);
            //缩放参数
            SetFloat("_StartTime", volume.m_StartTime);
            SetVector("_ScaCenter", volume.m_ScaCenter);
            SetFloat("_ScaStartScale", volume.m_ScaStartScale);
            SetFloat("_ScaEndScale", volume.m_ScaEndScale);
            SetFloat("_ScaTotalTime", volume.m_ScaTotalTime);

            DrawFullScreen();
        }
    }
}



