using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UICamera : Singleton<UICamera>
{
	public Camera main;
	private Camera gameCamera;
	private UniversalAdditionalCameraData gameCamData;
	private UniversalAdditionalCameraData camData;
	private bool initialized = false;

	public void Initialize()
	{
		if (initialized) return;
		initialized = true;
		Camera tempMain;
		UniversalAdditionalCameraData tempCamData;
		TryGetComponent<Camera>(out tempMain);
		if (tempMain == null)
		{
			tempMain = gameObject.AddComponent<Camera>();
			tempCamData = tempMain.gameObject.AddComponent<UniversalAdditionalCameraData>();

			tempCamData.renderType = CameraRenderType.Base;
			tempMain.orthographic = true;
			tempMain.orthographicSize = 5;
			tempCamData.SetRenderer(1);
			tempCamData.renderPostProcessing = false;
			tempCamData.antialiasing = AntialiasingMode.None;
			tempCamData.stopNaN = false;
			tempCamData.dithering = false;
			tempCamData.renderShadows = false;
			tempCamData.requiresDepthOption = CameraOverrideOption.Off;
			tempCamData.requiresColorOption = CameraOverrideOption.Off;
			tempMain.cullingMask = LayerMask.GetMask("UI", "EditorTile", "EditorObject", "EditorMarker");
			tempMain.useOcclusionCulling = false;
			tempMain.clearFlags = CameraClearFlags.SolidColor;
			tempMain.backgroundColor = new Color(0, 0, 0, 0);
			tempCamData.volumeLayerMask = 0;
			tempMain.allowHDR = false;
			tempMain.allowMSAA = false;
			tempMain.allowDynamicResolution = false;

			main = tempMain;
			camData = tempCamData;
		}
		else
		{
			main = tempMain;
			camData = main.GetComponent<UniversalAdditionalCameraData>();
		}
	}

	public void SetOverlay(bool isOverlay)
	{
		if (isOverlay)
		{
			gameCamera = Camera.main;
			gameCamData = gameCamera.GetComponent<UniversalAdditionalCameraData>();
			camData.renderType = CameraRenderType.Overlay;
			gameCamData.cameraStack.Add(main);
		}
		else if (!isOverlay && Camera.main != null)
		{
			AudioListenerManager.Instance.MoveListener(transform);
			gameCamData.cameraStack.Remove(main);
			camData.renderType = CameraRenderType.Base;
		}
		else
		{
			throw new Exception("[UICamera] Can't change UICamera into overlay while no game camera is present.");
		}
	}
}
