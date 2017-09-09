#pragma strict

var goldAmount : int;
var playerGoldCircleGameObject : GameObject;
var textGoldGameObject : GameObject = null;
var	scrollingActive : boolean = false;
var scrollingSpeed : float = 0.5;
var tmpTimeToScroll : float = 0;
var timeToScroll : float = 6;

var goldSound : AudioClip;

static var timeToDestroy : float = 50;

var renderers : Component[];

function Update()
{
	timeToDestroy -= Time.deltaTime;
	
	if (textGoldGameObject != null)
	{
		scrollingActive = true;
		textGoldGameObject.GetComponent(TextMesh).text = goldAmount + "";
		textGoldGameObject.GetComponent(TextMesh).fontSize = 20;
		Destroy(gameObject.GetComponent(TextMesh));
		textGoldGameObject.transform.position.y += scrollingSpeed * Time.deltaTime;
		if (scrollingActive)
		{
			tmpTimeToScroll += Time.deltaTime;
			if (tmpTimeToScroll > timeToScroll)
			{
				textGoldGameObject.GetComponent(TextMesh).text = null;
				tmpTimeToScroll = 0;
				scrollingActive = false;
				Destroy(gameObject);
			}
		}
	}
	
	if (timeToDestroy < tmpTimeToScroll)
	{
		tmpTimeToScroll = timeToDestroy - 0.05;
		if (timeToDestroy < 0.3 && textGoldGameObject)
		{
			textGoldGameObject.GetComponent(TextMesh).text = null;
			tmpTimeToScroll = 0;
			scrollingActive = false;
			Destroy(gameObject);
		}
	}
}

function	OnTriggerEnter(collision : Collider)
{
	if (collision.gameObject ==  playerGoldCircleGameObject)
	{
		gameObject.AddComponent(AudioSource);
		audio.PlayOneShot(goldSound);
		playerGoldCircleGameObject.transform.parent.GetComponent(PlayerGoldItem).gold += goldAmount;
		print("gold amount : " + playerGoldCircleGameObject.transform.parent.GetComponent(PlayerGoldItem).gold);
		textGoldGameObject = Instantiate(GameObject.Find("Gold Scrolling Text"), gameObject.transform.position, gameObject.transform.rotation);

    	renderers = gameObject.GetComponentsInChildren(Renderer);
 		
    	for (var rend : Renderer in renderers)
   	 	{
    		Destroy(rend.gameObject);
    		//rend.renderer.enabled = false;
    	}
    }

}
