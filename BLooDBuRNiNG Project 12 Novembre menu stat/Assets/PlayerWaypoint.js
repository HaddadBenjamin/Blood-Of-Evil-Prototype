#pragma strict
var numberOfWaypoint : int = 10;
var Waypoint : Waypoint[] = new Waypoint[numberOfWaypoint];

var showGUI : boolean = false;
static var waypointName : String;
var waypointTexture : Texture2D;
var waypointSound : AudioClip;

//GUI.skin = GUISkin.default;

function		Start()
{
	Waypoint[0].waypoint = GameObject.Find("Waypoint1");
	Waypoint[1].waypoint = GameObject.Find("Waypoint2");
	Waypoint[2].waypoint = GameObject.Find("Waypoint3");
	Waypoint[3].waypoint = GameObject.Find("Waypoint4");
	Waypoint[4].waypoint = GameObject.Find("Waypoint5");
	Waypoint[5].waypoint = GameObject.Find("Waypoint6");
	Waypoint[6].waypoint = GameObject.Find("Waypoint7");
	Waypoint[7].waypoint = GameObject.Find("Waypoint8");
	Waypoint[8].waypoint = GameObject.Find("Waypoint9");
	Waypoint[9].waypoint = GameObject.Find("Waypoint10"); 
	
	Waypoint[0].waypointName = "La Terre De Feu";
	Waypoint[1].waypointName = "Antre De La Terreur";
	Waypoint[2].waypointName = "Grotte De L'incapacité";
	Waypoint[3].waypointName = "La Gorge Du Diable";
	Waypoint[4].waypointName = "Au Coeur Du Volcan";
	Waypoint[5].waypointName = "La Foret Vierge";
	Waypoint[6].waypointName = "Ciel Et Merveille";
	Waypoint[7].waypointName = "Lac Mica";
	Waypoint[8].waypointName = "Suma, Le Village Maya";
	Waypoint[9].waypointName = "Temple Satanique";
}

private var exit_button_width :float = 0.012 * Screen.width;
private var exit_button_height : float = 0.03 * Screen.height;

private var box1_pos_x   : float = 0.075 * Screen.width;
private var box1_pos_y   : float = 0.080 * Screen.height;
private var box1_height  : float = 0.750 * Screen.height;  
private var box1_width   : float = 0.250 * Screen.width; 

private var button_width : float = 0.050 * Screen.width;
private var button_height :float = 0.035 * Screen.height;

private var box_left_side_pos_y : float = Screen.height * 0.150;
private var box_left_side_width : float = 0.02 * Screen.width;  

    
private var texture_pos_x: float = box1_pos_x + box_left_side_width;
//private var texture_pos_y: float = box1_pos_y + 5 * button_width;
private var texture_width : float = Screen.width * 0.030;
private var texture_height: float= (box1_height - button_height * 2) * 0.1;

private var waypoint_name_pos_x: float = texture_pos_x + texture_width;
private var waypoint_name_pos_width: float = box1_width - button_width;

var title_GUIStyle : GUIStyle;
var box_GUIStyle : GUIStyle;
var current_box_GUIStyle : GUIStyle;
var left_side_GUIStyle : GUIStyle;
var button_GUIStyle : GUIStyle;

function		OnGUI()
{
	if (showGUI)
	{
		exit_button_width = 0.012 * Screen.width;
		exit_button_height = 0.03 * Screen.height;

		box1_pos_x   = 0.075 * Screen.width;
		box1_pos_y   = 0.080 * Screen.height;
		box1_height  = 0.750 * Screen.height;  
		box1_width   = 0.250 * Screen.width; 

		button_width = 0.050 * Screen.width;
		button_height = 0.035 * Screen.height;

		box_left_side_pos_y = Screen.height * 0.150;
		box_left_side_width = 0.02 * Screen.width;  
   
		texture_pos_x= box1_pos_x + box_left_side_width;
		//texture_pos_y= box1_pos_y + 5 * button_width;
		texture_width = Screen.width * 0.030;
		texture_height = (box1_height - button_height * 2) * 0.1;

		waypoint_name_pos_x= texture_pos_x + texture_width;
		waypoint_name_pos_width= box1_width - button_width;
	
		title_GUIStyle.fontSize = ResizeFunctions.ResizeFont() + 7.2;
		box_GUIStyle.fontSize = ResizeFunctions.ResizeFont() + 2.2;
		current_box_GUIStyle.fontSize = ResizeFunctions.ResizeFont() + 2.2;
		button_GUIStyle.fontSize = ResizeFunctions.ResizeFont() + 2.2;
	
		//determiner les taille de tout les texte possible ici _
		if (GUI.Button(Rect(box1_pos_x + 5 * button_width - exit_button_width, box1_pos_y - button_height, exit_button_width,  button_height), "X"))
			showGUI = false;
			
		GUI.Button(Rect(box1_pos_x, box1_pos_y, button_width, button_height), "X");
		GUI.Button(Rect(box1_pos_x, box1_pos_y, button_width, button_height), "I");
		GUI.Button(Rect(box1_pos_x + 1 * button_width, box1_pos_y, button_width, button_height), "II");
		GUI.Button(Rect(box1_pos_x + 2 * button_width, box1_pos_y, button_width, button_height), "III");
		GUI.Button(Rect(box1_pos_x + 3 * button_width, box1_pos_y, button_width, button_height), "IV");
		GUI.Button(Rect(box1_pos_x + 4 * button_width, box1_pos_y, button_width, button_height), "V");
		GUI.Box   (Rect(box1_pos_x, Screen.height * 0.115, Screen.width * 0.25, button_height), "");
		GUI.Box   (Rect(box1_pos_x, Screen.height * 0.115, Screen.width * 0.25, button_height), "Choose Your Destination", title_GUIStyle);
		
		GUI.BeginGroup(Rect(0,0,0,0));
		GUI.EndGroup();
		
		GUI.Box   (Rect(box1_pos_x, box_left_side_pos_y, box_left_side_width, box1_height - button_height * 2), "", left_side_GUIStyle);
		
		GUI.BeginGroup(Rect(0,0,0,0));
		GUI.EndGroup();
		
		//Case  avec fleche a gauche du pays du waypoint
		for (var i : int = 0; i < numberOfWaypoint; i++)
		{
			GUI.Box(Rect(texture_pos_x, box_left_side_pos_y + texture_height * i, texture_width, texture_height), "");
			if (Waypoint[i].waypointActive)
			{
				 GUI.DrawTexture(Rect(texture_pos_x, box_left_side_pos_y + texture_height * i, texture_width, texture_height), waypointTexture);
				 if (detect_click_gui_box(waypoint_name_pos_x, box_left_side_pos_y + texture_height * i, waypoint_name_pos_width, texture_height))
			 	 {
			 	 	audio.PlayOneShot(waypointSound);
			 	 	transform.position = Waypoint[i].waypoint.transform.position;
			 	 	showGUI = false;
			 	 } 
			}
			else
				GUI.Box(Rect(texture_pos_x, box_left_side_pos_y + texture_height * i, texture_width, texture_height), "", button_GUIStyle);
		}
	  	
		 
	  	//GUI.BeginGroup(Rect(0,0,0,0));
		//GUI.EndGroup();
	 	//pays du waypoint
   		for (i = 0; i < numberOfWaypoint; i++)
   		{
   			 GUI.Box(Rect(waypoint_name_pos_x, box_left_side_pos_y + texture_height * (i % 10), waypoint_name_pos_width, texture_height), "");
   			 if (waypointName != Waypoint[i].waypointName)
   			 	GUI.Box(Rect(waypoint_name_pos_x, box_left_side_pos_y + texture_height * (i % 10), waypoint_name_pos_width, texture_height), Waypoint[i].waypointName, box_GUIStyle);
   			 else
   			 	GUI.Box(Rect(waypoint_name_pos_x, box_left_side_pos_y + texture_height * (i % 10), waypoint_name_pos_width, texture_height), Waypoint[i].waypointName, current_box_GUIStyle);
   		} 
   		
	  	GUI.BeginGroup(Rect(0,0,0,0));
		GUI.EndGroup();
	}

}

function	detect_click_gui_box(x1 : float, y1 : float, x2 : float, y2 : float)
{ 
	if (Input.GetMouseButtonDown(0))
	{
		var mousePos = Input.mousePosition;

		if (mousePos.x >= x1 && mousePos.x <= (x2 + x1) && mousePos.y <= Screen.height - y1 && mousePos.y >= (Screen.height - (y1 + y2)))
			return true;
	}
	
	return false; 
}