using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullute : MonoBehaviour,PoolObj {

	ObjPool belongpool;

	ObjPool PoolObj.BelongPool {
		get
		{
			return belongpool;
		}
		set
		{
			belongpool = value;
		}

	}

	public GameObject Obj
	{
		get
		{
			return gameObject;
		}
	}

	string objname;
	public string ObjName
	{
		get
		{
			return objname;
		}

		set
		{
			objname = value;
		}
	}

	// Use this for initialization
	//public void startfly()
	//{
	//	GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 10, ForceMode.Impulse);
	//	Invoke("hangUpBullute", 3);
	//}
	//public void setbelongpoool(bltpool s)
	//{
	//	belongpool = s;
	//}
	//public void hangUpBullute()
	//{
	//	//gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * -10, ForceMode.Impulse);
	//	gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
	//	belongpool.backtoPool(gameObject);


	//}

	// Update is called once per frame


	public void reSetObj()
	{
		gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
		gameObject.GetComponent<TrailRenderer>().Clear();
	}

	public void startMove()
	{
		gameObject.SetActive(true);
		GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 10, ForceMode.VelocityChange);
		Invoke("hangUp", 10);
	}

	public void hangUp()
	{
		CancelInvoke("hangUp");
		belongpool.backtoPool(this);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Bullute"))
		{
			return;
		}
			hangUp();

	}
	private void OnTriggerStay(Collider other)
	{
		if (!gameObject.activeSelf)
		{
			return;
		}
		if (other.gameObject.CompareTag("Bullute"))
		{
			return;
		}
		hangUp();


	}
	private void OnTriggerExit(Collider other)
	{
		if (!gameObject.activeSelf)
		{
			return;
		}
		if (other.gameObject.CompareTag("Bullute"))
		{
			return;
		}
		hangUp();


	}

}
