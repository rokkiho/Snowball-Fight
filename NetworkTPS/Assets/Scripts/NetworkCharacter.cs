using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class NetworkCharacter : MonoBehaviour {

	Vector3 position = Vector3.zero;
	Quaternion rotation = Quaternion.identity;

	Transform head;
	Quaternion headRotation = Quaternion.identity;

	string playerName;

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
			transform.Find("Name").GetComponent<TextMesh>().text = name;
			transform.position = Vector3.Lerp(transform.position, position, 0.2f);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.2f);
			head.rotation = Quaternion.Lerp(head.rotation, headRotation, 0.2f);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if(stream.isWriting) {
			// This is OURS
			stream.SendNext(playerName);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);

			if(head != null)
				stream.SendNext (head.rotation);
		}
		else {
			// This is others'
			playerName = (string)stream.ReceiveNext();
			position = (Vector3)stream.ReceiveNext();
			rotation = (Quaternion)stream.ReceiveNext();

			headRotation = (Quaternion)stream.ReceiveNext();
		}
	}

	public void SetName(string name) {
		playerName = name;
	}
}
