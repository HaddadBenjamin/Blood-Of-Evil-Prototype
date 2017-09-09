private var hit: RaycastHit;
private var myColor : Color;
private var boolFirstColor : boolean = true;
private var n : GameObject;

function	Start()
{
	myColor = Vector4(0, 1, 1, 1);
}

function Update () 
{
	if(Input.GetKey(KeyCode.Mouse0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), hit, Mathf.Infinity)) 
	{
		if (hit.collider.CompareTag("Enemy"))
		{
			n = hit.collider.gameObject;
			//if (boolFirstColor)
				//myColor = n.renderer.materials[0].color;
			boolFirstColor = false;
			this.n.renderer.materials[0].color = Vector4(1, 0, 0, 0.5);
			boolFirstColor = false;
		}
		else
			if (boolFirstColor == false)
				this.n.renderer.materials[0].color = myColor;
	}
	else
		if (boolFirstColor == false)
			n.renderer.materials[0].color = myColor;
}