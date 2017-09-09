var level : int = Random.Range(1, 10 + 1);
var xpAmount : int = 150;
var xpPercent : float = 1.00;

function	InitWithLevel()
{
	var		tmpLevel : int = level; 
	var 	dizaine : int = tmpLevel * 0.1 - (tmpLevel * 0.1 % 1);
	var		tmpDizaine : int = 1;
	var		minLevel : int = 100;
	var 	maxLevel : int = 110;
	//level of drop vari aussi
	//if (tmpLevel < 10)
	//{
		xpPercent 																				*= (1 + (0.2  *  tmpLevel % 10));
		gameObject.GetComponent(EnemyCharacteristic).healthPercent 								*= (1 + (0.3  *  tmpLevel % 10));
		gameObject.GetComponent(EnemyCharacteristic).dmgPercent									*= (1 + (0.2  *  tmpLevel % 10));
		gameObject.GetComponent(EnemyCharacteristic).attackSpeedPercent							/= (1 + (0.05 *  tmpLevel % 10));
		gameObject.GetComponent(EnemyCharacteristic).fastMovement 								+=  1         *  tmpLevel % 10;
	//}  
	
	while (dizaine >= 1)
	{
		if (tmpLevel - tmpDizaine * 10 >= 10)
		{
		 	xpPercent 																		*= (1 + (0.1 * 10));
			gameObject.GetComponent(EnemyCharacteristic).healthPercent 						*= (1 + (0.1 * 10));
			gameObject.GetComponent(EnemyCharacteristic).dmgPercent							*= (1 + (0.02 * 10));
			gameObject.GetComponent(EnemyCharacteristic).attackSpeedPercentfastMovement 	/= (1 + (0.005 * 10));
			gameObject.GetComponent(EnemyCharacteristic).fastMovement 						*= (1 + (0.02 * 10));
		} 
	/*	else
		{
		 	xpPercent 									*= (1 + (0.1 * tmpLevel - tmpDizaine * 10));
			healthPercent 								*= (1 + (0.1 * tmpLevel - tmpDizaine * 10));
			dmgPercent									*= (1 + (0.02 * tmpLevel - tmpDizaine * 10));
			yAttackSpeed								/= (1 + (0.005 * tmpLevel - tmpDizaine * 10));
			gameObject.GetComponent(FollowMe).moveSpeed *= (1 + (0.02 * tmpLevel - tmpDizaine * 10));		
		} */
		dizaine--;
	}
}