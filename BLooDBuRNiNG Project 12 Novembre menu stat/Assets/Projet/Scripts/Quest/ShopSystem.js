var hit : RaycastHit;
static var shopOpen : boolean = false;
var yTab : int = 0;
var xTab : int = 0;
var offsetWidth : float = 0.02;var offsetHeight : float = 0.044;
static var boolCanOpen : boolean = false;
var textuWeapon : Texture;

 
function OnGUI ()
{
	if(shopOpen == true) 
	{
		GUI.Box(Rect(Screen.width * 0.25, Screen.height * 0.25, Screen.width * 0.15, Screen.height * 0.415), "Vendeur d'arme"); //1er menu
		GUI.Box(Rect(Screen.width * 0.2623, Screen.height * 0.290, Screen.width * 0.123, Screen.height * 0.33), "Marché"); // 2eme menu
		if (GUI.Button(Rect(Screen.width * 0.39, Screen.height * 0.25, Screen.width * 0.01, Screen.height * 0.022), "X"))//croix pour fermer
			shopOpen = false;
		for (yTab = 0; yTab < 6; yTab++)
		{
			for (xTab = 0; xTab < 5; xTab++)
			{
				if (GUI.Button(Rect(Screen.width * (0.2713 + (xTab * (offsetWidth + 0.0015))), Screen.height * (0.325  + (yTab * (offsetHeight + 0.004))), Screen.width * offsetWidth, Screen.height * offsetHeight), ""))
				{
				
				}
				//if (yTab == 0)
				GUI.DrawTexture(Rect(Screen.width * (0.2713 + (xTab * (offsetWidth + 0.0015))), Screen.height * (0.325  + (yTab * (offsetHeight + 0.004))), Screen.width * offsetWidth, Screen.height * offsetHeight), textuWeapon);
			}
		}
		GUI.Box(Rect(Screen.width * 0.305, Screen.height * 0.630, Screen.width * 0.01, Screen.height * 0.022), ""); //fleche de gauche
		GUI.Box(Rect(Screen.width * 0.332, Screen.height * 0.630, Screen.width * 0.01, Screen.height * 0.022), ""); // fleche de droite
	}
}

function Update () 
{
	if (boolCanOpen == true)
		if(Input.GetMouseButtonDown(0) && collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), hit, Mathf.Infinity))
			shopOpen = true;
}