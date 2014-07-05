using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class NetworkCharacter : MonoBehaviour {

	Vector3 position = Vector3.zero;
	Quaternion rotation = Quaternion.identity;

	Transform head;
	Quaternion headRotation = Quaternion.identity;

	// Use this for initialization
	void Start () {
		position = transform.position;
		rotation = transform.rotation;

		head = transform.FindChild ("Head");
		headRotation = head.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<PhotonView>().isMine) {

		}
		else {
			transform.position = Vector3.Lerp(transform.position, position, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.1f);
			head.rotation = Quaternion.Lerp(head.rotation, headRotation, 0.1f);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if(stream.isWriting) {
			// This is OURS
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);

			if(head != null)
				stream.SendNext (head.rotation);
		}
		else {
			// This is others'
			position = (Vector3)stream.ReceiveNext();
			rotation = (Quaternion)stream.ReceiveNext();

			headRotation = (Quaternion)stream.ReceiveNext();
		}
	}
}
