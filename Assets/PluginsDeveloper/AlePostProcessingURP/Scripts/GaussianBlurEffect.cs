using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlePostProcessUPR;
using static Unity.VisualScripting.Member;

namespace AlePostProcessUPR
{
	[System.Serializable, VolumeComponentMenu("Able/GaussianBlur")]
	public class GaussianBlurEffect : CustomVolumeComponent
    {
		[Range(0f, 50f)]
		public FloatParameter BlurRadius = new FloatParameter(3f);
		[Range(1, 15)]
		public IntParameter Iteration = new IntParameter(6);
	}

	[CustomPostProcess("Able/GaussianBlur", CustomPostProcessInsertPoint.AfterPostProcess)]
	public class GaussianBlurEffectRenderer : CustomPostProcessRenderer
	{
        static class ShaderIDs
        {
            internal static readonly int Input = Shader.PropertyToID("_MainTex");

            internal static readonly int BlurRadius = Shader.PropertyToID("_BlurOffset");
            internal static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
            internal static readonly int BufferRT2 = Shader.PropertyToID("_BufferRT2");
        }

        public override void Initialize()
		{
            InitPostProcessMaterial("Hidden/Custom/GaussianBlur");
		}

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            base.Render(cmd, src, dest, ref renderingData, insertPoint);

            var volume = GetVolumeComponent<GaussianBlurEffect>();

            //获取RT
            int RTWidth = (int)(Screen.width / 4f);
            int RTHeight = (int)(Screen.height / 4f);
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


