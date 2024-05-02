using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;
using static Unity.VisualScripting.Member;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/GaussianBlurGradient")]
	public class GaussianBlurGradientEffect : CustomVolumeComponent
    {
		[Range(0f, 50f)]
		public FloatParameter BlurRadius = new FloatParameter(3f);
		[Range(1, 15)]
		public IntParameter Iteration = new IntParameter(6);

		//渐变模糊
		[Tooltip("Gradient Center")]
		public Vector2Parameter m_GradientCenter = new Vector2Parameter(new Vector2(0.5f, 0.5f)); //渐变中心点
		[Tooltip("Gradient RangeX")]
		public Vector2Parameter m_GradientRangeX = new Vector2Parameter(new Vector2(0f, 0.5f)); //渐变开始与结束的范围 X轴
		[Tooltip("Gradient RangeY")]
		public Vector2Parameter m_GradientRangeY = new Vector2Parameter(new Vector2(0f, 0.5f)); //渐变开始与结束的范围 Y轴
	}

	[CustomPostProcess("Able/GaussianBlurGradient", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class GaussianBlurGradientEffectRenderer : CustomPostProcessRenderer
	{
        static class ShaderIDs
        {
            internal static readonly int Input = Shader.PropertyToID("_MainTex");
            internal static readonly int BlurRadius = Shader.PropertyToID("_BlurOffset");
            internal static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
            internal static readonly int BufferRT2 = Shader.PropertyToID("_BufferRT2");
        }

        //初始化
        public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/GaussianBlurGradient");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<GaussianBlurGradientEffect>();

            //渐变参数
            SetVector("_GradientCenter", volume.m_GradientCenter);
            SetVector("_GradientRangeX", volume.m_GradientRangeX);
            SetVector("_GradientRangeY", volume.m_GradientRangeY);

            //获取RT
            int RTWidth = Screen.width;
            int RTHeight = Screen.height;
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT2, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            //降采样复制
            cmd.Blit(src, ShaderIDs.BufferRT1);

            for (int i = 0; i < volume.Iteration.value; i++)
            {
                //水平模糊
                cmd.SetGlobalVector(ShaderIDs.BlurRadius, new Vector4(volume.BlurRadius.value / Screen.width, 0, 0, 0));
                cmd.SetGlobalTexture(ShaderIDs.Input, ShaderIDs.BufferRT1);
                CoreUtils.DrawFullScreen(cmd, m_PostProcessMaterial, ShaderIDs.BufferRT2);

                //垂直模糊
                cmd.SetGlobalVector(ShaderIDs.BlurRadius, new Vector4(0, volume.BlurRadius.value / Screen.height, 0, 0));
                cmd.SetGlobalTexture(ShaderIDs.Input, ShaderIDs.BufferRT2);
                CoreUtils.DrawFullScreen(cmd, m_PostProcessMaterial, ShaderIDs.BufferRT1);
            }

            DrawFullScreen(1, ShaderIDs.BufferRT1);

            //释放RT
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT2);
        }
    }
}


