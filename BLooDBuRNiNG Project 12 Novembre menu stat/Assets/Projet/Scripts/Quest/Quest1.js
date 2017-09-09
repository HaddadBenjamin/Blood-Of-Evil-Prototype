private var hit : RaycastHit;
var vitesseAffichage = 0.07;
private var boolDisplayText = false;
private var boolCompleteQuest1 = false;
private var boolToOpen = false;
private var boolCanOpenShop = false;
private var boolCanCount : boolean = false;
static var quest1 : boolean = false;
private var myText : String;
var myGuiText : GUIText;
private var skeltonsBonesObject : GameObject;
private var timeCount : float = 0.0;

static var questItemPercent : float = 0.0;
static var pos : Vector3;
static var rot : Quaternion;
static var boolActiveInstantiateSkeltonBones : boolean = false; 

var questCompleteSound : AudioClip;

function Start () 
{
	myText = "Allez voir l'homme du feu, il semblerait qu'il\nest une quète à vous proposer";
	TypeText();
	skeltonsBonesObject = gameObject.Find("Os de Squelette");
}

function	Update()
{
	if (quest1 == false)
	{
		if (Input.GetMouseButtonDown(0) && collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), hit, Mathf.Infinity) && boolDisplayText == false)
		{
			boolDisplayText = true;
			gameObject.Find("Spawn Point").GetComponent(SpawmingSkeltons).numberSpawn = 8;
			var fleche : Transform = ParentChildFunction.FindChildTransform(gameObject.Find("Barbarian").transform, "Fleche");
			fleche.active = false;
		}
		if (boolDisplayText == true && PlayerScore.score < 100)
		{
			myGuiText.text = "";
			myText = "Guerrier, j'ai besoin de vous !" + "\n" + "Allez me chercher 100 os de squelettes";
			boolDisplayText = false;
			TypeText();
		}
		if (PlayerScore.score >= 100 && ShopSystem.boolCanOpen == false && boolToOpen == false)
		{
			myGuiText.text = "";
			myText = "Retournez voir l'homme du feu afin de compléter votre quète";
					boolCompleteQuest1 = true;
			boolToOpen = true;
			TypeText();
		}
		if (Input.GetMouseButtonDown(0) && collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), hit, Mathf.Infinity) && boolCompleteQuest1 == true)
		{
			myGuiText.text = "";
			myText = "Merci Guerrier, je vous serez infiniment reconnaissant" + "\n" + "prenez ces quelques pièces d'or et cette épée d'acier" + "\n" + "Vos dommages ont augmentés de 30";
			PlayerDamage.minDmg += 30;
			PlayerDamage.maxDmg += 30;
			boolDisplayText = false;//this.shopSytem..
			boolCompleteQuest1 = false;
			boolCanOpenShop = true;
			boolCanCount = true;
			quest1 = true;
			gameObject.Find("Spawn Point").GetComponent(SpawmingSkeltons).numberSpawn = 0;
			//Quest2.quest2 = true;
			TypeText();
			audio.PlayOneShot(questCompleteSound);
		}
		if (boolCanOpenShop == true && Input.GetMouseButtonDown(0) && collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), hit, Mathf.Infinity))
			ShopSystem.boolCanOpen = true;
	}
	if (boolCanCount == true)
	{
		timeCount += Time.deltaTime;
		if (timeCount > 17)
		{
			myGuiText.text = "";
			boolCancount = false;
		}
	}
	if (boolActiveInstantiateSkeltonBones == true)
		InstantiateSkeltonBones();
		
}
 
function	TypeText () 
{
	for (var letter in myText.ToCharArray()) 
	{
		myGuiText.text += letter;
		yield WaitForSeconds (vitesseAffichage);
	}              
}

function	InstantiateSkeltonBones()
{
	var numberOfBonesDrop : int = 0;
	var randomPosX : float = 0;
	var randomPosZ : float = 0;
	numberOfBonesDrop *= PlayerGoldItem.itemFind;
	for (numberOfBonesDrop = 0; numberOfBonesDrop < questItemPercent; numberOfBonesDrop++)
	{
		randomPosX = Random.Range(pos.x - 0.5, pos.x + 0.5);
		randomPosZ = Random.Range(pos.z - 0.5, pos.z + 0.5);
		skeltonsBonesObject.GetComponent(SkeltonBoneScript).playerItemCircleGameObject = GameObject.Find("Item Circle Detection"); 		
		Instantiate(skeltonsBonesObject, Vector3(randomPosX, pos.y, randomPosZ), rot);
	}
	boolActiveInstantiateSkeltonBones = false;
}