#pragma strict

var fireMan : Transform;
private var relativePos : Vector3;
var player : Transform;
var tmp : Transform;
function Update () 
{
		//relativePos = fireMan.rotation.eulerAngles - player.rotation.eulerAngles;

		tmp.transform.rotation = transform.rotation;
		tmp.LookAt(fireMan);
		relativePos = tmp.rotation.eulerAngles;
		relativePos.x = 270;
		relativePos.y -= 180;
		//relativePos = fireMan.transform.position - transform.position;
		//transform.rotation = Quaternion.LookRotation(relativePos, new Vector3(0, 0, 0));
		//var rotation = Quaternion.Lerp(transform.rotation, fireMan.rotation, Time.deltaTime * 5);
		//rotation.y -= 90;
		//transform.rotation = rotation;
		//transform.rotation = Quaternion.Euler(fireMan.rotation.x, fireMan.rotation.y, fireMan.rotation.z);
		//transform.rotation = fireMan.rotation;
		
	transform.rotation = Quaternion.Euler(relativePos);
}
