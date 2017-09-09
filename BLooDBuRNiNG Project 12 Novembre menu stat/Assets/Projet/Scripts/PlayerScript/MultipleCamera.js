static var startCamera : int;
var camera1 : Camera; 
var camera2 : Camera; 
var camera3 : Camera;
var camera4 : Camera;

static var moduloTab : int;
//Les 4 caméra par défaut, vous pouvez en ajouter ou en supprimer selon vos besoins.

function Start () 
{ 
	startCamera = 1;
	moduloTab = 1;
//   camera1.enabled = true; //On active la première caméra uniquement
//   camera2.enabled = false; 
//   camera3.enabled = false;
//   camera4.enabled = false;
//   startCamera = 1;//variable qui change en fonction de la caméra active
} 
function Update () 
{
	Handle_Event();
	if (ChatSystem.selectTextfield == true)
	{
		if (startCamera == 1)
	    { 
	     	camera1.enabled = true; 		
		 	camera2.enabled = false; 
		   	camera3.enabled = false;
	    	camera4.enabled = false;
	    } 
	   	else if (startCamera == 2) //si on appuie sur "c" et que la caméra active est la 1
	   	{ 
	   		camera1.enabled = false; 
	   		camera2.enabled = true; 
	   	   	camera3.enabled = false;
	      	camera4.enabled = false;
	   	} 
	  	else if (startCamera == 3)
	  	{ 
	      	camera1.enabled = false; 
	      	camera2.enabled = false; 
	      	camera3.enabled = true;
	      	camera4.enabled = false;
		} 
		else if (startCamera == 4)
	    { 
	      	camera1.enabled = false; 
	       	camera2.enabled = false; 
	       	camera3.enabled = false;
	       	camera4.enabled = true;
	    } 
	}
}

function	Handle_Event()
{
	if (Input.GetKeyDown ("v"))
	{
		startCamera++;
		if (startCamera == 5)
			startCamera = 1;
	}
		
	if (Input.GetKeyDown ("tab"))
	   	moduloTab++;
	if (moduloTab == 10)
		moduloTab = 0;
}