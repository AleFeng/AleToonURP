using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;
using static Unity.VisualScripting.Member;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/EdgeMask")]
	public class EdgeMaskEffect : CustomVolumeComponent
    {
		[ColorUsage(true, true)]
		public ColorParameter m_Color = new ColorParameter(new Color(0.0f, 0.0f, 0.0f, 1));
		[Range(0.0f, 1.0f)]
		public FloatParameter m_Distance = new FloatParameter(0.4f);
		[Range(-1f, 1f)]
		public FloatParameter m_Sharpness = new FloatParameter(0.1f);
		[Range(-10f, 10f)]
		public FloatParameter m_Radian = new FloatParameter(0.2f);
		public Vector2Parameter m_Center = new Vector2Parameter(new Vector2(0.5f, 0.5f));
		public BoolParameter m_ToggleVertical = new BoolParameter(true);
		public BoolParameter m_ToggleHorizontal = new BoolParameter(true);
	}

	[CustomPostProcess("Able/EdgeMask", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class EdgeMaskEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/EdgeMask");
		}

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<EdgeMaskEffect>();

            SetColor("_Color", volume.m_Color);
            SetFloat("_Distance", volume.m_Distance);
            SetFloat("_Sharpness", volume.m_Sharpness);
            SetFloat("_Radian", volume.m_Radian);
            SetVector("_Center", volume.m_Center);

            Vector2 toggle;
            if (volume.m_ToggleVertical.value)
                toggle.y = 1;
            else
                toggle.y = 0;

            if (volume.m_ToggleHorizontal.value)
                toggle.x = 1;
            else
                toggle.x = 0;
            SetVector("_Toggle", toggle);

            DrawFullScreen();
        }
    }
}


