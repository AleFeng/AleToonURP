using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/Distort")]
	public class DistortEffect : CustomVolumeComponent
	{
		public TextureParameter m_DistortTex = new TextureParameter(null);
		public Vector2Parameter m_DistortTiling = new Vector2Parameter(Vector2.one);
		[Range(0f, 1f)]
		public FloatParameter m_DistortAmount = new FloatParameter(0.1f); //扰动强度
		public Vector2Parameter m_DistortVelocity = new Vector2Parameter(new Vector2(0.1f, 0.1f)); //扰动速度
		[Range(0f, 1f)]
		public FloatParameter m_GradualLengthX = new FloatParameter(1f); //渐变长度 水平方向
		[Range(-1f, 1f)]
		public FloatParameter m_GradualAnchorX = new FloatParameter(1f); //渐变锚点 水平方向
		[Range(0f, 1f)]
		public FloatParameter m_GradualLengthY = new FloatParameter(1f); //渐变长度 垂直方向
		[Range(-1f, 1f)]
		public FloatParameter m_GradualAnchorY = new FloatParameter(1f); //渐变锚点 垂直方向

		//水面反光
		[ColorUsage(true, true)]
		public ColorParameter m_SpecularColor = new ColorParameter(new Color(0.0f, 0.0f, 0.0f, 1)); //反光颜色
		[Range(0f, 1f)]
		public FloatParameter m_SpecularAmount = new FloatParameter(1f); //反光强度
		[Range(0f, 1f)]
		public FloatParameter m_SpecularScale = new FloatParameter(1f); //反光纹理大小
	}

	[CustomPostProcess("Able/Distort", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class DistortEffectRenderer : CustomPostProcessRenderer
	{
		public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/Distort");
		}

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<DistortEffect>();

            SetTexture("_DistortTex", volume.m_DistortTex);
            SetVector("_DistortTiling", volume.m_DistortTiling);
            SetFloat("_DistortAmount", volume.m_DistortAmount);
            SetVector("_DistortVelocity", volume.m_DistortVelocity);
            SetFloat("_GradualLengthX", volume.m_GradualLengthX);
            SetFloat("_GradualAnchorX", volume.m_GradualAnchorX);
            SetFloat("_GradualLengthY", volume.m_GradualLengthY);
            SetFloat("_GradualAnchorY", volume.m_GradualAnchorY);
            SetColor("_SpecularColor", volume.m_SpecularColor);
            SetFloat("_SpecularAmount", volume.m_SpecularAmount);
            SetFloat("_SpecularScale", volume.m_SpecularScale);

			DrawFullScreen();
        }
    }
}
