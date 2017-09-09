#pragma strict
private var hit : RaycastHit;
static var playerItemCircleGameObject : GameObject;

/*function Update () 
{
	if (collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), hit, Mathf.Infinity))
	{
		PlayerCharacteristic.score++;
		Destroy(gameObject);
	}
}*/

function	OnTriggerEnter(collision : Collider)
{
	if (collision.gameObject == playerItemCircleGameObject)
	{
		playerItemCircleGameObject.transform.parent.GetComponent(PlayerScore).score++;
		//print("gold amount : " + playerGoldCircleGameObject.transform.parent.GetComponent(PlayerCharacteristic).gold);
		Destroy(gameObject);
    }

}