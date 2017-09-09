#pragma strict

static var itemFind : float = 1.00;
static var itemRarity : float = 1.00;
static var goldFindQuantity : float = 8.00;
static var goldFindAmount : float = 2;
static var gold : long = 0;

var goldCircleGameObject : GameObject; //= GameObject.Find("Gold Circle Detection");//GetComponent(ParentChildFunction).GetChildrenGameObject(gameObject, "Gold Circle Detection"); // get a children with his name
var itemCircleGameObject : GameObject; //= GameObject.Find("Item Circle Detection");//GetComponent(ParentChildFunction).GetChildrenGameObject(gameObject, "Item Circle Detection"); // get a children with his name
private var tmpCirclePickingRange : float = 0.0f;
static var goldPickingRange : float = 10.0f;
static var	itemPickingRange : float = 7.0f;
private var tmpGoldPickingRange : float = 0.0f;
private var	tmpItemPickingRange : float = 0.0f;
static var goldPickingScale : float = 0.1f;
static var itemPickingScale : float = 0.1f;

function Start () 
{
	goldCircleGameObject = GameObject.Find("Gold Circle Detection");
	itemCircleGameObject = GameObject.Find("Item Circle Detection");
}

function Update () 
{
	GoldItemCalcul(goldCircleGameObject, goldPickingScale, goldPickingRange, tmpGoldPickingRange);
	GoldItemCalcul(itemCircleGameObject, itemPickingScale, itemPickingRange, tmpItemPickingRange);
}

function	GoldItemCalcul(circleGameObject : GameObject, pickingScale : float, pickingRange : float, tmpPickingRange)
{
	if (tmpPickingRange != pickingRange)
	{
		circleGameObject.transform.localScale = Vector3(pickingRange * pickingScale, pickingRange * pickingScale, pickingRange * pickingScale);
		tmpCirclePickingRange = pickingRange;	
	}
}