#pragma strict

static var boolMainMenu : boolean = false;
private var boolShowGraphicMenu : boolean = false;
private var boolShowSoundMenu : boolean = false;
private var boolShowResolutionMenu : boolean = false;
private var boolShowOptionsMenu : boolean = false;

private var currentResolutionX : int;
private var currentResolutionY : int;

private var hSliderValue : float = 1.00;
private var fullScreen : boolean = false;
private var editing : boolean = false;

function	OnGUI()
{
	if (boolMainMenu) 
	{
		Time.timeScale = 0;
		GUI.Box(Rect(Screen.width / 2 - 100, Screen.height / 2 - 150, 250, 200), "Main Menu");
		//Make Main Menu button
//		if (GUI.Button(Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 250, 50), "Main Menu"))
//			Application.LoadLevel(mainMenuSceneName);	
		if (GUI.Button(Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 250, 50), "Options"))
		{
			boolShowGraphicMenu = false;
			boolShowSoundMenu = false;
			boolShowOptionsMenu = true;
		}
		if (boolShowOptionsMenu == true)
		{
			if (GUI.Button(Rect(Screen.width / 2 + 150, Screen.height / 2 - 100, 250, 50), "Sound"))
			{
				boolShowSoundMenu = true;
				boolShowGraphicMenu = false;
				boolShowResolutionMenu = false;
			}
			else if (GUI.Button(Rect(Screen.width / 2 + 150, Screen.height / 2 - 50, 250, 50), "Graphics Quality"))
			{
				boolShowGraphicMenu = true;
				boolShowSoundMenu = false;
				boolShowResolutionMenu = false;
			}
			else if (GUI.Button(Rect(Screen.width / 2 + 150, Screen.height / 2, 250, 50), "Resolution"))
			{
				boolShowResolutionMenu = true;
				boolShowGraphicMenu = false;
				boolShowSoundMenu = false;
			}
		}
		if (boolShowSoundMenu == true)
		{
			var intSliderVolume : int = hSliderValue * 100;
			GUI.Box(Rect (Screen.width / 2 + 400, Screen.height / 2 - 100, 250, 50), "Sound Volume : " + intSliderVolume + "%");
		 	hSliderValue = GUI.HorizontalScrollbar(Rect (Screen.width / 2 + 400, Screen.height / 2 - 75, 250, 50), hSliderValue, 0.0, 0.00, 1.0);
		 		AudioListener.volume = hSliderValue ;
			/*if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2 - 100, 250, 50), "Mute"))
				 AudioListener.volume = 0.00;
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2 - 50, 250, 50), "Low"))
				 AudioListener.volume = 0.25;
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2, 250, 50), "Medium"))
				 AudioListener.volume = 0.50;
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2 + 50, 250, 50), "Strong"))
				 AudioListener.volume = 0.75;
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2 + 100, 250, 50), "Max"))
				 AudioListener.volume = 1.00;*/
			boolShowGraphicMenu = false;  
		    boolShowSoundMenu = true;
		    boolShowResolutionMenu = false;
		}
		if (boolShowGraphicMenu == true)
		{
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2 - 50, 250, 50), "Fastest"))
				QualitySettings.currentLevel = QualityLevel.Fastest;
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2, 250, 50), "Fast"))
				QualitySettings.currentLevel = QualityLevel.Fast;
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2 + 50, 250, 50), "Simple"))
				QualitySettings.currentLevel = QualityLevel.Simple;
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2 + 100, 250, 50), "Good"))
				QualitySettings.currentLevel = QualityLevel.Good;
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2 + 150, 250, 50), "Beautiful"))
				QualitySettings.currentLevel = QualityLevel.Beautiful;
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2 + 200, 250, 50), "Fantastic"))
				QualitySettings.currentLevel = QualityLevel.Fantastic;
		}
		if (boolShowResolutionMenu == true)
		{
			fullScreen = GUILayout.Toggle(fullScreen, "FullScren");
			
			if (GUI.Button(Rect(Screen.width / 2 + 400, Screen.height / 2 - 0, 250, 50), "Full Screen"))
				Screen.fullScreen = true;
			else
				Screen.fullScreen = false;
				
			if (GUILayout.Button(currentResolutionX + " x " + currentResolutionY))
				editing = !editing;
			for (var x : int = 0; x < Screen.resolutions.Length; x++)
			{
				Screen.SetResolution(Screen.resolutions[x].width, Screen.resolutions[x].height, fullScreen);
				currentResolutionX = Screen.resolutions[x].width;
				currentResolutionY = Screen.resolutions[x].height;
				editing = false;
			}
		}
		if (GUI.Button(Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 250, 50), "Help"))
		{
		    boolShowGraphicMenu = false;  
		    boolShowSoundMenu = false;
		    boolShowResolutionMenu = false;
		}		
		if (GUI.Button(Rect(Screen.width / 2 - 100, Screen.height / 2, 250, 50), "Exit"))
			boolMainMenu = !boolMainMenu;
	}
	if (!boolMainMenu)
	{
		Time.timeScale = 1;	
		boolShowGraphicMenu = false;
		boolShowSoundMenu = false;
		boolShowResolutionMenu = false;
		boolShowOptionsMenu = false;
	}
	
}