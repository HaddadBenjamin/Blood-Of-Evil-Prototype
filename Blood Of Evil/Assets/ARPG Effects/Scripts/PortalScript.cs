using UnityEngine;
using System.Collections;

public class PortalScript : MonoBehaviour {

	public GameObject portalOpen;
	public GameObject portalIdle;
	public GameObject portalClose;

	void Start ()
	{
		OpenPortal();
	}


	public void OpenPortal()
	{
		StartCoroutine("PortalLoop");
	}
	

	IEnumerator PortalLoop()
	{
		GameObject portalOpener = (GameObject) Instantiate(portalOpen, transform.position, transform.rotation);
		yield return new WaitForSeconds(0.8f);
		GameObject portalIdler = (GameObject) Instantiate(portalIdle, transform.position, transform.rotation);

		yield return new WaitForSeconds(2f);
		Destroy (portalIdler);
		Destroy (portalOpener);
		GameObject portalCloser = (GameObject) Instantiate(portalClose, transform.position, transform.rotation);

		yield return new WaitForSeconds(1f);
		Destroy (portalCloser);
		OpenPortal();
	}

//	IENumerator ClosePortal()
//	{
//		yield return new WaitForSeconds(2f);
//		Destroy (portalIdler);
//		GameObject portalClose = (GameObject) Instantiate(portalClose, transform.position, transform.rotation);
//	}
}