using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/EdgeDetectionSobel")]
	public class EdgeDetectionSobelEffect : CustomVolumeComponent
    {
		[Range(0.05f, 5.0f)]
		public Vector2Parameter m_EdgeWidth = new Vector2Parameter(new Vector2(0.3f, 0f));
		[ColorUsage(true, true)]
		public ColorParameter m_EdgeColor = new ColorParameter(new Color(0.0f, 0.0f, 0.0f, 1));
	}

	[CustomPostProcess("Able/EdgeDetectionSobel", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class EdgeDetectionSobelEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/EdgeDetectionSobel");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<EdgeDetectionSobelEffect>();

            SetVector("_Params", volume.m_EdgeWidth);
            SetColor("_EdgeColor", volume.m_EdgeColor);

			DrawFullScreen();
        }
    }
}

