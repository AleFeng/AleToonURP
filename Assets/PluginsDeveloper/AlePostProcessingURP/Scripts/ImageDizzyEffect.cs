using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;
using static Unity.VisualScripting.Member;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/ImageDizzy")]
	public class ImageDizzyEffect : CustomVolumeComponent
    {
		[Range(0f, 1f), Tooltip("Dizzy effect Range.")]
		public FloatParameter range = new FloatParameter(0.5f);
		[Range(0f, 2f), Tooltip("Dizzy effect Speed.")]
		public FloatParameter speed = new FloatParameter(0.5f);
		[Tooltip("完整时间")]
		public FloatParameter time = new FloatParameter(1.5f);
		[Range(0f, 0.5f), Tooltip("淡入时间占比")]
		public FloatParameter fadeIn = new FloatParameter(0.2f);
		[Range(0.5f, 1f), Tooltip("淡出时间占比")]
		public FloatParameter fadeOut = new FloatParameter(0.8f);
		[Tooltip("是否循环")]
		public BoolParameter m_IsLoop = new BoolParameter(false);

		[Tooltip("开始时间")]
		public FloatParameter m_StartTime = new FloatParameter(0f);
	}

	[CustomPostProcess("Able/ImageDizzy", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class ImageDizzyEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/ImageDizzy");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<ImageDizzyEffect>();

            float passTime = Time.realtimeSinceStartup - volume.m_StartTime.value;
            float strenth = 1;
            if (passTime > volume.time.value)
            {
                if (volume.m_IsLoop.value)
                    volume.m_StartTime.value = Time.realtimeSinceStartup;
                else
                    strenth = 0;
            }
            else
            {
                float progress = passTime / volume.time.value;

                if (progress < volume.fadeOut.value)
                {
                    if (progress < volume.fadeIn.value)
                        strenth = progress / volume.fadeIn.value;
                    else
                        strenth = 1;
                }
                else
                    strenth = 1.0f - (progress - volume.fadeOut.value) / (1.0f - volume.fadeOut.value);
            }

            SetFloat("_Range", volume.range.value * strenth);
            SetFloat("_Speed", volume.speed);

            DrawFullScreen();
        }
    }
}
