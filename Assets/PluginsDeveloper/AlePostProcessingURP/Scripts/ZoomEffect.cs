using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/Zoom")]
	public class ZoomEffect : CustomVolumeComponent
    {
		// 放大强度
		[Range(-2.0f, 2.0f)]
		public FloatParameter ZoomFactor = new FloatParameter(0.4f);
		// 放大镜大小
		[Range(0.0f, 0.2f)]
		public FloatParameter Size = new FloatParameter(0.15f);
		// 遮罩中心位置
		public Vector2Parameter Pos = new Vector2Parameter(Vector2.one * 0.5f);

	}
	[CustomPostProcess("Able/Zoom", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class ZoomEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/Zoom");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<ZoomEffect>();

            SetFloat("_ZoomFactor", volume.ZoomFactor);
            SetFloat("_Size", volume.Size);
            SetVector("_Pos", volume.Pos);

			DrawFullScreen();
        }
    }
}

