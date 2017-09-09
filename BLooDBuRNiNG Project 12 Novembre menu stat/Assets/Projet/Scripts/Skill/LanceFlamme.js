 #pragma strict
 /* Script Provided by Will Goldstone as part of Unity Game Development Essentials book assets */
/* Please Do Not Remove this comment - this script is for reference only */
static var canThrow : boolean = true;
var throwSound : AudioClip;
var coconutObject : Rigidbody;
var throwForce : float;
var itemposition : Vector3;
var vitesseSort : float = 0.5;
var misterFloat : float = 0.0;
var misterDestroy : float = 0.0;

itemposition = transform.position;
itemposition.x += 0.1;
itemposition.y -= .8;
function Update () {
	
	if(Input.GetKeyDown("p")  && canThrow){
			audio.PlayOneShot(throwSound);
			var newCoconut : Rigidbody = Instantiate(coconutObject, transform.position, transform.rotation);
				//transform.position.y += 1;
				newCoconut.name = "coconut";
				newCoconut.rigidbody.velocity = transform.TransformDirection(Vector3(0,0, throwForce));
				Physics.IgnoreCollision(transform.root.collider, newCoconut.collider, true);
		canThrow = false;
		if(canThrow == false){
			misterFloat += Time.deltaTime;
			misterDestroy += Time.deltaTime;
		}		
		if(misterFloat > vitesseSort){
	    	canThrow = true;
	    	misterFloat = 0.0;
	    }
	    if(misterDestroy >= 3){
        	Destroy(gameObject);   
        	misterDestroy = 0.0;
        }
	}
}

@script RequireComponent(AudioSource)
