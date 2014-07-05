using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class Projectile : MonoBehaviour {

	[SerializeField]
	float lifeTime = 2f;

	float elapsedTime = 0.0f;

	[SerializeField]
	float damage = 10;

	GameObject ownerObject;

	// Update is called once per frame
	void Update () {
		if(GetComponent<PhotonView>().isMine) {
			elapsedTime += Time.deltaTime;
			
			if(elapsedTime >= lifeTime) {
				GetComponent<PhotonView>().RPC ("DestroyProjectile", PhotonTargets.All);
			}
		}
	}

	public void SetOwnerObject(GameObject go) {
		ownerObject = go;
	}

	void OnCollisionEnter(Collision col) {
		if(GetComponent<PhotonView>().isMine) {
			if(col.gameObject.GetComponentInChildren<NetworkHealth>() != null) {
				//int ownerID = ownerObject.GetComponent<PhotonView>().ownerId;
				//int targetID = col.gameObject.GetComponent<PhotonView>().ownerId;
				//Debug.Log (ownerID + " hit " + targetID);
				col.gameObject.GetComponent<PhotonView>().RPC("ApplyDamage", PhotonTargets.All, damage);
			}

			GetComponent<PhotonView>().RPC ("DestroyProjectile", PhotonTargets.All);
		}
	}

	[RPC]
	void DestroyProjectile() {
		if(GetComponent<PhotonView>().isMine)
			PhotonNetwork.Destroy (gameObject);
	}
}
