import Debug;
private var log:Array = new Array();
var maxLogMessages:int = 200;
var visible:boolean = true;
var stringToEdit : String = "";
static var selectTextfield : boolean = true;
private var dt = Date();

function	Start()
{
	Input.eatKeyPressOnTextFieldFocus = false;
	log.Add("Welcome on Blood of Evil Chat");
	printGUIStyle.normal.textColor = Vector4(0, 1, 0.0, .7);
}

function	print(string:String)
{
	log.push(string);
	if(log.length > maxLogMessages)
	log.RemoveAt(0);
}
	
private var scrollPos:Vector2 = Vector2(0, 0);
private var lastLogLen:int = 0;
var printGUIStyle:GUIStyle;
var maxLogLabelHeight:float = 100.0f;
	
function	OnGUI()
{
	if(visible)
	{
		GUI.SetNextControlName ("chatWindow");
		stringToEdit = GUI.TextField (Rect (600.0, Screen.height - 54, 700, 20), stringToEdit, 100);//longueur de la boite ou on ecrit
		if (!selectTextfield) 
			GUI.FocusControl ("chatWindow");   	// permet d'avoir l'attention  sur le gui du nom chatWindow 
		else
			GUI.FocusControl (""); 				// permet d'avoir l'attention sur aucun gui en gros sa quitte le chat
		var logBoxWidth:float = 180.0;
		var logBoxHeights:float[] = new float[log.length];
		// calculate full height of scrollview
		var totalHeight:float = 0.0;
		var i:int = 0;
		for(var string:String in log)
		{
			var logBoxHeight = Mathf.Min(maxLogLabelHeight, printGUIStyle.CalcHeight(GUIContent(string), logBoxWidth));
			logBoxHeights[i++] = logBoxHeight;
			totalHeight += logBoxHeight+10.0;
		}
		var innerScrollHeight:float = totalHeight;
		// if there's a new message, automatically scroll to bottom
		if(lastLogLen != log.length)
		{
			scrollPos = Vector2(0.0, innerScrollHeight);
			lastLogLen = log.length;
		}
		scrollPos = GUI.BeginScrollView(Rect(600.0, Screen.height-150.0-50.0, 700, 150), scrollPos, Rect(0.0, 0.0, 180, innerScrollHeight)); // taille de ce qui apparait à l'écran
		var currY:float = 0.0;
		i = 0;
		for(var string:String in log)
		{
			logBoxHeight = logBoxHeights[i++];
			GUI.Label(Rect(10, currY, logBoxWidth, logBoxHeight), string, printGUIStyle);
			currY += logBoxHeight+10.0;
		}
		GUI.EndScrollView();
	}
}

function Update () 
{
    var day = dt.Now.Day;
    var month = dt.Now.Month;
    var year = dt.Now.Year;
    var hours = dt.Now.Hour;
    var minutes = dt.Now.Minute;
    var seconds = dt.Now.Second;
	if(Input.GetKeyDown("return")) 
	{
		selectTextfield = !selectTextfield;
		if(stringToEdit != "") 
		{
			log.Add("[" + hours + ":" + minutes + ":" + seconds + "] " + PlayerCharacteristic.playerName + ": " + stringToEdit );
			stringToEdit = "";
		}
	}
}

//if (GUI.Button(Rect(Screen.width / 2,Screen.height / 2 - 40, 80, 20), "Tchat here biatch"))// faire un bouton qui enveloppe le chat