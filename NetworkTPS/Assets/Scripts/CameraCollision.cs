using UnityEngine;
using System.Collections;

public class CameraCollision : MonoBehaviour {

	float minDistance = 1.0f;
	float maxDistance = 4.0f;
	float smooth = 10f;

	Vector3 dollyDir;
	float distance;

	void Start () {
		dollyDir = transform.localPosition.normalized;
		distance = transform.localPosition.magnitude;
	}

	void Update () {
		Vector3 desiredCamPos = transform.parent.parent.TransformPoint (dollyDir * maxDistance);

		RaycastHit hit;
		if(Physics.Linecast (transform.parent.parent.position, desiredCamPos, out hit)) {
			distance = Mathf.Clamp (hit.distance, minDistance, maxDistance);
		}
		else {
			distance = maxDistance;
		}

		transform.localPosition = Vector3.Lerp (transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
	}
}
