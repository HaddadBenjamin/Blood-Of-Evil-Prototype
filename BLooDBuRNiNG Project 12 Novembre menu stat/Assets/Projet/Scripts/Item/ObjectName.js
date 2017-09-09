var target : Transform;		// Object that this label should follow
var offset = Vector3.up;	// Units in world space to offset; 1 unit above object by default
var clampToScreen = false;	// If true, label will be visible even if object is off screen
var clampBorderSize = .05;	// How much viewport space to leave at the borders when a label is being clamped
var useMainCamera = true;	// Use the camera tagged MainCamera
var cameraToUse : Camera;	// Only use this if useMainCamera is false
private var cam : Camera;
private var thisTransform : Transform;
private var camTransform : Transform;
private var cameraMMORPG : GameObject;
private var cameraHackAndSlash : GameObject;
private var boolCamera : boolean = true;
 
function Start () 
{
	cameraMMORPG = gameObject.Find("Camera MMORPG 1");
	cameraHackAndSlash = gameObject.Find("Camera Hack & Slash 2");
	guiText.material.color = Vector4(1, 0, 0, 1);
	guiText.text = PlayerCharacteristic.playerName;
	thisTransform = transform;
	if (useMainCamera)
		cam = Camera.main;
	else
		cam = cameraToUse;
	camTransform = cam.transform;
}
 
function Update () 
{
	if (MultipleCamera.startCamera % 2 == 1)
	{
		cameraToUse = cameraMMORPG.camera;
		cam = cameraMMORPG.camera;
	}
	else
	{
		cameraToUse = cameraHackAndSlash.camera;
		cam = cameraHackAndSlash.camera;
	}
	if (clampToScreen) 
	{
		var relativePosition = camTransform.InverseTransformPoint(target.position);
		relativePosition.z = Mathf.Max(relativePosition.z, 1.0);
		if (MultipleCamera.startCamera % 2 == 1)
		{
			thisTransform.position = cam.WorldToViewportPoint(camTransform.TransformPoint(relativePosition + offset));
			thisTransform.position = Vector3(Mathf.Clamp(thisTransform.position.x, clampBorderSize, 1.0 - clampBorderSize),
			Mathf.Clamp(thisTransform.position.y, clampBorderSize, 1.0 - clampBorderSize),
			thisTransform.position.z);
			boolCamera = true;
		}
		else if (MultipleCamera.startCamera % 2 == 0 && boolCamera == true)
		{
			thisTransform.position.y = offset.y * 0.2625;
			thisTransform.position.x = offset.x * 5.31;
			boolCamera = false;
		}
	}
	else
		thisTransform.position = cam.WorldToViewportPoint(offset);
}
 
@script RequireComponent(GUIText)