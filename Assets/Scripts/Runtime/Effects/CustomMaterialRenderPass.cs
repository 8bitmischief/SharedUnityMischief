using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Adapted from https://samdriver.xyz/article/scriptable-render

namespace SharedUnityMischief
{
	public class CustomMaterialRenderPass : ScriptableRenderPass
	{
		private string profilerTag; // Used to label this pass in Unity's Frame Debug utility

		private Material _material;
		private RenderTargetIdentifier _cameraColorTargetIdent;
		private RenderTargetHandle _tempTexture;

		public CustomMaterialRenderPass(Material material, RenderPassEvent renderPassEvent, string profilerTag)
		{
			_material = material;
			this.renderPassEvent = renderPassEvent;
			this.profilerTag = profilerTag;
		}

		public void Setup(RenderTargetIdentifier cameraColorTargetIdent)
		{
			_cameraColorTargetIdent = cameraColorTargetIdent;
		}

		// Called each frame before Execute, use it to set up things the pass will need
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			// Create a temporary render texture that matches the camera
			cmd.GetTemporaryRT(_tempTexture.id, cameraTextureDescriptor);
		}

		// Execute is called for every eligible camera every frame. It's not called at the moment that
		// rendering is actually taking place, so don't directly execute rendering commands here.
		// Instead use the methods on ScriptableRenderContext to set up instructions.
		// RenderingData provides a bunch of (not very well documented) information about the scene
		// and what's being rendered.
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			// Fetch a command buffer to use
			CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
			cmd.Clear();

			// Apply our material while blitting to a temporary texture
			cmd.Blit(_cameraColorTargetIdent, _tempTexture.Identifier(), _material, 0);

			// ...then blit it back again
			cmd.Blit(_tempTexture.Identifier(), _cameraColorTargetIdent);

			// Don't forget to tell ScriptableRenderContext to actually execute the commands
			context.ExecuteCommandBuffer(cmd);

			// Tidy up after ourselves
			cmd.Clear();
			CommandBufferPool.Release(cmd);
		}

		// Called after Execute, use it to clean up anything allocated in Configure
		public override void FrameCleanup(CommandBuffer cmd)
		{
			cmd.ReleaseTemporaryRT(_tempTexture.id);
		}
	}
}