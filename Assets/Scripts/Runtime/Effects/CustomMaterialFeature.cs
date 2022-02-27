using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Adopted from https://samdriver.xyz/article/scriptable-render

namespace SharedUnityMischief
{
	public class CustomMaterialFeature : ScriptableRendererFeature
	{
		// MUST be named "settings" (lowercase) to be shown in the Render Features inspector
		public CustomMaterialSettings settings = new CustomMaterialSettings();

		private CustomMaterialRenderPass _renderPass;

		public override void Create()
		{
			_renderPass = new CustomMaterialRenderPass(
				settings.material,
				settings.renderPassEvent,
				"CustomMaterialFeature pass"
			);
		}

		// Called every frame once per camera
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (!settings.isEnabled)
				return;

			if (renderingData.cameraData.isSceneViewCamera && !settings.showInSceneView)
				return;

			// Gather up and pass any extra information our pass will need
			RenderTargetIdentifier  cameraColorTargetIdent = renderer.cameraColorTarget;
			_renderPass.Setup(cameraColorTargetIdent);

			// Ask the renderer to add our pass
			renderer.EnqueuePass(_renderPass);
		}

		[Serializable]
		public class CustomMaterialSettings
		{
			public bool isEnabled = true;
			[ShowIfBool("isEnabled")] public bool showInSceneView = true;
			public Material material;
			public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
		}
	}
}