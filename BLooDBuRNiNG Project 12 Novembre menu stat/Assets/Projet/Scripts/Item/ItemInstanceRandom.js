function Update() 
{
	var randX = Random.Range(.5,-.5); 
	var randY = Random.Range(.5,-.5);
	var randZ = Random.Range(.5,-.5);
	transform.position += Vector3(randX,randY,randZ);
}