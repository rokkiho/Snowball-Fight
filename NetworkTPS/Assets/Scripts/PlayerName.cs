using UnityEngine;
using System.Collections;

public class PlayerName : MonoBehaviour {

	Camera cameraToLookAt;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		cameraToLookAt = Camera.main;

		if(cameraToLookAt == null) return;

		Vector3 v = cameraToLookAt.transform.position - transform.position;
		
		v.x = v.z = 0.0f;
		
		transform.LookAt( cameraToLookAt.transform.position - v ); 
		
		transform.Rotate(0,180,0);
	}
}
