#pragma strict

var playerCollider : GameObject;

var waypointTime : float = 3.00;
var numberWaypoint : int = 10;
private var waypointActive : boolean[];

function		Start()
{
	waypointActive = new boolean[numberWaypoint];
	for (var i : int = 0; i < numberWaypoint; i++)
		waypointActive[i] = false;
}

private var i : int = 0;

function		Update()
{
	
	if (playerCollider && playerCollider.GetComponent(PlayerWaypoint).showGUI == true)
	{
		for (i = 0; i < numberWaypoint; i++)
		{
			if (waypointActive[i] == true)
			{
				if (Vector3.Distance(transform.position , playerCollider.transform.position) > 6)
				{
		 			playerCollider.GetComponent(PlayerWaypoint).showGUI = false;
		 			waypointActive[i] = false;
		 		}
			}
		}
	}
}

function		OnTriggerEnter(other : Collider)
{
	if (other.transform.CompareTag("Player"))
	{
		playerCollider = other.gameObject;
		playerCollider.GetComponent(PlayerWaypoint).showGUI = true;
		
		for (var i : int = 0; i < playerCollider.GetComponent(PlayerWaypoint).numberOfWaypoint; i++)
		{
			if (gameObject.name == "Waypoint" + (i + 1))
			{
				playerCollider.GetComponent(PlayerWaypoint).Waypoint[i].waypointActive = true;
				playerCollider.GetComponent(PlayerWaypoint).waypointName = playerCollider.GetComponent(PlayerWaypoint).Waypoint[i].waypointName;
				waypointActive[i] = true;
			}
		}
	}
} 	
