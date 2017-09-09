/* Script Provided by Will Goldstone as part of Unity Game Development Essentials book assets */
/* Please Do Not Remove this comment - this script is for reference only */
private var nextUpdate : float = 0;
private var frames : float = 0;
private var fps : float = 0;
private var updatePeriod = 0.5;

function Update() {

	frames++;
	
	if (Time.time > nextUpdate){
		
		  fps = Mathf.Round(frames / updatePeriod);
				guiText.text = "Frames Per Second: " + fps;
                nextUpdate = Time.time + updatePeriod;
                frames = 0;		
	}
	
}

@script RequireComponent(GUIText)