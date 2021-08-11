using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	
	public ObjPool bullutepool;
	public Transform gunPos;

	int mask;

	// Use this for initialization
	void Start () {
		mask = 1 << (LayerMask.NameToLayer("Collision"));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{

			onFire();

		}
	}
	public void onFire()
	{




		//第三人称摄像机代瞄准



		Camera onCma = gameObject.GetComponent<Camera>();
		Ray dir = new Ray(gameObject.transform.position, gameObject.transform.forward);
		RaycastHit downrcHit;
		Vector3 lookPos;
		if (Physics.Raycast(dir, out downrcHit, 200f, mask))
		{
			lookPos = downrcHit.point;
		}
		else
		{


			Vector3 cmaPos = gameObject.transform.position;
			lookPos = cmaPos + dir.direction.normalized * 200f;

		}

		GameObject blt = bullutepool.getObj("Bullute");

		//第三人称摄像机代瞄准
		blt.transform.position = gunPos.position;
		blt.transform.LookAt(lookPos);
		
		blt.GetComponent<Bullute>().startMove();
		//blt.GetComponent<Rigidbody>().AddForce(blt.transform.forward * mySpeedCon.NowSpeed, ForceMode.VelocityChange);
	}
}
