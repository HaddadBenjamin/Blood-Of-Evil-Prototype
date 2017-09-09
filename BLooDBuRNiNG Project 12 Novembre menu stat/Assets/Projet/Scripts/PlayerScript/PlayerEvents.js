#pragma strict
private var paused : boolean = false;
var teleportationSound : AudioClip;
var inventorySound : AudioClip;
private var cameraMinimap : GameObject; 
static var indexMinimap : int = 0;

function	Start()
{
	cameraMinimap = gameObject.Find("Camera Minimap");
	cameraMinimap.camera.enabled = false;

}

function	Update() 
{
	InputSystem();
}

function	InputSystem()
{
	if (ChatSystem.selectTextfield == true)
	{
		InputSystemKeyCode();
		InputSystemGetKeyDown();
		InputSystemGetButtonDown();	
	}
}

function	InputSystemKeyCode()
{
	if (Input.GetKey(KeyCode.J)) 
		gameObject.GetComponent(PlayerHealthManaXp).curHp -= 10;
	if (Input.GetKey(KeyCode.X))
	{
		gameObject.GetComponent(PlayerHealthManaXp).curXp += 15 * gameObject.GetComponent(PlayerHealthManaXp).xpPercent;
		gameObject.GetComponent(PlayerHealthManaXp).totalXp += 15 * gameObject.GetComponent(PlayerHealthManaXp).xpPercent;
	}
	if (Input.GetKey(KeyCode.T))
	{
 		audio.PlayOneShot(teleportationSound);
 		transform.Translate(0, 15, 0);
 	}
	if (Input.GetKey(KeyCode.F) && gameObject.GetComponent(PlayerHealthManaXp).curMp >= 22 && gameObject.GetComponent(PlayerCast).boolFastCast == true)
	{
		gameObject.GetComponent(PlayerCast).fireBallObject = Instantiate(gameObject.GetComponent(PlayerCast).fireBall, transform.position, transform.rotation);
		Destroy(gameObject.GetComponent(PlayerCast).fireBallObject, 5);	
	}
	if (Input.GetKey(KeyCode.H) && gameObject.GetComponent(PlayerHealthManaXp).curMp >= 35 && gameObject.GetComponent(PlayerCast).boolFastCast == true && gameObject.GetComponent(PlayerHealthManaXp).curHp < gameObject.GetComponent(PlayerHealthManaXp).xMaxHp)
		PlayerSkill.boolHealing = true;
	if (Input.GetKey(KeyCode.G) && gameObject.GetComponent(PlayerDementiaMode).boolCanGodMode == true && gameObject.GetComponent(PlayerDementiaMode).boolGodMode == false)
	{
		gameObject.Find("WindZone").GetComponent(SmoothFollow).enabled = true;
		gameObject.GetComponent(PlayerDementiaMode).DementiaModeBonus();
	}
}

function	InputSystemGetKeyDown()
{
	if (Input.GetKeyDown("f") && gameObject.GetComponent(PlayerHealthManaXp).curMp <= 22)
	{
		audio.PlayOneShot(gameObject.GetComponent(PlayerHealthManaXp).mpSound);
		yield WaitForSeconds(0.5);
	}
	else if (Input.GetKeyDown("m") && indexMinimap == 0)
    {
    	cameraMinimap.camera.enabled = true;
    	indexMinimap++;
    }	
	else if (Input.GetKeyDown ("m") && indexMinimap == 1)
	{
    	cameraMinimap.camera.enabled = false; 
	  	indexMinimap = 0;
	}
	else if (Input.GetKeyDown("p") && paused == false)
    {
    	paused = true;
  	    Time.timeScale = 0;
    }
    else if (Input.GetKeyDown("p") && paused == true) 
    {
    	paused = false;
   		Time.timeScale = 1;
    }
    else if (Input.GetKeyDown("escape"))
    {
    	if (ShopSystem.shopOpen == false && PlayerInventory.boolShowInventory == false && gameObject.GetComponent(PlayerWaypoint).showGUI == false)// || xx.xx == false etc..
			gameObject.GetComponent(PlayerOptionsMenu).boolMainMenu = !gameObject.GetComponent(PlayerOptionsMenu).boolMainMenu;
		else
		{
			ShopSystem.shopOpen = false;
			PlayerInventory.boolShowInventory = false;
			gameObject.GetComponent(PlayerWaypoint).showGUI = false;
		}
	}
	if (Input.GetKeyDown("i"))
	{
 		audio.PlayOneShot(inventorySound);
		PlayerInventory.boolShowInventory = !PlayerInventory.boolShowInventory;
	}
}

function	InputSystemGetButtonDown()
{	

			
	if (Input.GetMouseButtonDown(0))
	{
		
		if (detect_click_on_box(Screen.width * 0.1, Screen.width * 0.068, Screen.height * 0.83, Screen.height * 0.15))
			gameObject.GetComponent(PlayerHealthManaXp).boolHpTextInPercent = !gameObject.GetComponent(PlayerHealthManaXp).boolHpTextInPercent;
		
		if (detect_click_on_box(Screen.width * 0.83541, Screen.width * 0.068, Screen.height * 0.83, Screen.height * 0.15))
			gameObject.GetComponent(PlayerHealthManaXp).boolMpTextInPercent = !gameObject.GetComponent(PlayerHealthManaXp).boolMpTextInPercent;			
		
		if (detect_click_on_box(Screen.width * 0.262, Screen.width * 0.48, Screen.height * 0.89, Screen.height * 0.02))
			gameObject.GetComponent(PlayerHealthManaXp).boolXpTextInPercent = !gameObject.GetComponent(PlayerHealthManaXp).boolXpTextInPercent;
	}

}

function	OnGUI()
{
//	GUI.Box(Rect(Screen.width * 0.262, Screen.height * 0.89, Screen.width * 0.48, Screen.height * 0.02), "test");	
}

function	detect_click_on_box(x1 : float, x2 : float, y1 : float, y2 : float)
{
	var mousePos = Input.mousePosition;

	if (mousePos.x >= x1 && mousePos.x <= (x2 + x1) && mousePos.y <= Screen.height - y1 && mousePos.y >= (Screen.height - (y1 + y2)))
		return true;
	
	return false;
}