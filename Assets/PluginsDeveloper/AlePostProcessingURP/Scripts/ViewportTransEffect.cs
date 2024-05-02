using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/ViewportTrans")]
	public class ViewportTransEffect : CustomVolumeComponent
    {
        [Range(0f, 1f), Tooltip("Start Point UV")]
        public Vector2Parameter m_RectStartPoint = new Vector2Parameter(new Vector2(0f, 0f));
        [Range(0f, 1f), Tooltip("Rect Width Height")]
        public Vector2Parameter m_RectWH = new Vector2Parameter(new Vector2(0f, 0f));
    }

	[CustomPostProcess("Able/ViewportTrans", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class ViewportTransEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/ViewportTrans");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<ViewportTransEffect>();

            //设置参数
            var uv = volume.m_RectStartPoint.value;
            var wh = volume.m_RectWH.value;
            SetVector("_ViewportRect", new Vector4(uv.x, uv.y, wh.x, wh.y));

            DrawFullScreen();
        }
    }
}



