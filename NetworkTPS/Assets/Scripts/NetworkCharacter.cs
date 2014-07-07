using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;

[System.Serializable]
internal class NetworkCharPacket {
	public Vector3 position;
	public Quaternion rotation;
	public Quaternion headRotation;

	public NetworkCharPacket() {
		position = Vector3.zero;
		rotation = Quaternion.identity;
		headRotation = Quaternion.identity;
	}
}

[RequireComponent(typeof(PhotonView))]
public class NetworkCharacter : Photon.MonoBehaviour {
	string playerName;
	Transform head;

	NetworkCharPacket myInfo = new NetworkCharPacket();
	NetworkCharPacket receivedInfo = new NetworkCharPacket();

	static bool registeredType = false;

	// Use this for initialization
	void Start () {
		head = transform.FindChild ("Head");

		myInfo.position = transform.position;
		myInfo.rotation = transform.rotation;
		myInfo.headRotation = head.rotation;

		if(!registeredType) {
			PhotonPeer.RegisterType(typeof(NetworkCharPacket), (byte)'N', SerializeNetworkCharPacket, DeserializeNetworkCharPacket);
			registeredType = true;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<PhotonView>().isMine) {

		}
		else {
			transform.Find("Name").GetComponent<TextMesh>().text = playerName;
			transform.position = Vector3.Lerp(transform.position, receivedInfo.position, 0.2f);
			transform.rotation = Quaternion.Lerp(transform.rotation, receivedInfo.rotation, 0.2f);
			head.rotation = Quaternion.Lerp(head.rotation, receivedInfo.headRotation, 0.2f);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if(stream.isWriting) {
			// This is OURS
			myInfo.position = transform.position;
			myInfo.rotation = transform.rotation;

			if(head != null)
				myInfo.headRotation = head.rotation;

			stream.SendNext(playerName);
			stream.SendNext(SerializeNetworkCharPacket(myInfo));
		}
		else {
			// This is others'

			playerName = (string)stream.ReceiveNext();
			receivedInfo = (NetworkCharPacket) DeserializeNetworkCharPacket ((byte[])stream.ReceiveNext());
		}
	}

	public void SetName(string name) {
		playerName = name;
	}

	static byte[] SerializeNetworkCharPacket(object customobject) {
		NetworkCharPacket packet = (NetworkCharPacket)customobject;

		byte[] bytes = new byte[3 * 4 + 4 * 4 + 4 * 4];
		int index = 0;
		Protocol.Serialize (packet.position.x, bytes, ref index);
		Protocol.Serialize (packet.position.y, bytes, ref index);
		Protocol.Serialize (packet.position.z, bytes, ref index);

		Protocol.Serialize (packet.rotation.x, bytes, ref index);
		Protocol.Serialize (packet.rotation.y, bytes, ref index);
		Protocol.Serialize (packet.rotation.z, bytes, ref index);
		Protocol.Serialize (packet.rotation.w, bytes, ref index);

		Protocol.Serialize (packet.headRotation.x, bytes, ref index);
		Protocol.Serialize (packet.headRotation.y, bytes, ref index);
		Protocol.Serialize (packet.headRotation.z, bytes, ref index);
		Protocol.Serialize (packet.headRotation.w, bytes, ref index);

		return bytes;
	}

	static object DeserializeNetworkCharPacket(byte[] bytes) {
		NetworkCharPacket packet = new NetworkCharPacket ();
		int index = 0;
		Protocol.Deserialize (out packet.position.x, bytes, ref index);
		Protocol.Deserialize (out packet.position.y, bytes, ref index);
		Protocol.Deserialize (out packet.position.z, bytes, ref index);

		Protocol.Deserialize (out packet.rotation.x, bytes, ref index);
		Protocol.Deserialize (out packet.rotation.y, bytes, ref index);
		Protocol.Deserialize (out packet.rotation.z, bytes, ref index);
		Protocol.Deserialize (out packet.rotation.w, bytes, ref index);

		Protocol.Deserialize (out packet.headRotation.x, bytes, ref index);
		Protocol.Deserialize (out packet.headRotation.y, bytes, ref index);
		Protocol.Deserialize (out packet.headRotation.z, bytes, ref index);
		Protocol.Deserialize (out packet.headRotation.w, bytes, ref index);

		return packet;
	}
}
