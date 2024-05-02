using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/Ripple")]
	public class RippleEffect : CustomVolumeComponent
    {
		public Vector4Parameter m_Center1 = new Vector4Parameter(Vector4.zero);
		public Vector4Parameter m_Center2 = new Vector4Parameter(Vector4.zero);
		public Vector4Parameter m_Center3 = new Vector4Parameter(Vector4.zero);
		[Range(0f, 0.5f)]
		public FloatParameter m_Height = new FloatParameter(0.05f);
		[Range(0f, 0.5f)]
		public FloatParameter m_Width = new FloatParameter(0.1f);
		[Range(0f, 1f)]
		public FloatParameter m_Speed = new FloatParameter(0.5f);
		[Range(0f, 10f)]
		public FloatParameter m_HeightAttenuation = new FloatParameter(2f);
		[Range(0f, 10f)]
		public FloatParameter m_WidthAttenuation = new FloatParameter(2f);
	}

	[CustomPostProcess("Able/Ripple", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class RippleEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/Ripple");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<RippleEffect>();

            SetVector("_Center1", volume.m_Center1);
            SetVector("_Center2", volume.m_Center2);
            SetVector("_Center3", volume.m_Center3);
            SetFloat("_Height", volume.m_Height);
            SetFloat("_Width", volume.m_Width);
            SetFloat("_Speed", volume.m_Speed);
            SetFloat("_HeightAtten", volume.m_HeightAttenuation);
            SetFloat("_WidthAtten", volume.m_WidthAttenuation);

			DrawFullScreen();
        }
    }
}
