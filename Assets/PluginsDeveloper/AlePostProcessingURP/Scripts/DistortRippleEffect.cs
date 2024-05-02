using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/DistortRipple")]
	public class DistortRippleEffect : CustomVolumeComponent
    {
		//水面扰动
		public TextureParameter m_DistortTex = new TextureParameter(null);
		public Vector2Parameter m_DistortTiling = new Vector2Parameter(Vector2.one);
		[Range(0f, 1f)]
		public FloatParameter m_DistortAmount = new FloatParameter(0.1f); //扰动强度
		public Vector2Parameter m_DistortVelocity = new Vector2Parameter(new Vector2(0.1f, 0.1f)); //扰动速度
		[Range(0f, 1f)]
		public FloatParameter m_GradualLengthX = new FloatParameter(1f); //渐变长度 水平方向
		[Range(-1f, 1f)]
		public FloatParameter m_GradualAnchorX = new FloatParameter(0f); //渐变锚点 水平方向
		[Range(0f, 1f)]
		public FloatParameter m_GradualLengthY = new FloatParameter(1f); //渐变长度 垂直方向
		[Range(-1f, 1f)]
		public FloatParameter m_GradualAnchorY = new FloatParameter(0f); //渐变锚点 垂直方向

		//水波纹
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

	[CustomPostProcess("Able/DistortRipple", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class DistortRippleEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/DistortRipple");
		}

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<DistortRippleEffect>();

            //水面扰动
            SetTexture("_DistortTex", volume.m_DistortTex);
            SetVector("_DistortTiling", volume.m_DistortTiling);
            SetFloat("_DistortAmount", volume.m_DistortAmount);
            SetVector("_DistortVelocity", volume.m_DistortVelocity);
            SetFloat("_GradualLengthX", volume.m_GradualLengthX);
            SetFloat("_GradualAnchorX", volume.m_GradualAnchorX);
            SetFloat("_GradualLengthY", volume.m_GradualLengthY);
            SetFloat("_GradualAnchorY", volume.m_GradualAnchorY);
            //水波纹
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
