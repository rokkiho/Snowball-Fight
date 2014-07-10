using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class NetworkProjectile : Photon.MonoBehaviour {

	[SerializeField]
	float lifeTime = 2f;
	
	float elapsedTime = 0.0f;
	
	[SerializeField]
	float damage = 10;
	
	[SerializeField]
	Transform explosionEffect;

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	void Start() {
		realPosition = transform.position;
		realRotation = transform.rotation;
	}

	// Update is called once per frame
	void Update () {
		if(PhotonNetwork.isMasterClient) {
			elapsedTime += Time.deltaTime;
			
			if(elapsedTime >= lifeTime) {
				photonView.RPC ("DestroyProjectile", PhotonTargets.All);
			}
		}

		if(photonView.isMine) {
			
		}
		else {
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.2f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.2f);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if(stream.isWriting) {
			// This is OURS'
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else {
			// This is others'
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
		}
	}

	void OnCollisionEnter(Collision col) {
		NetworkCharacter character = col.gameObject.GetComponentInChildren<NetworkCharacter>();
		if(character != null) {
			PlayerHUD.ShowCrossHairHit();
			character.ApplyDamage(damage);
		}
		
		photonView.RPC ("DestroyProjectile", PhotonTargets.All);
	}

	[RPC]
	void DestroyProjectile() {
		Instantiate (explosionEffect, transform.position, Quaternion.identity);
		if(photonView.instantiationId == 0)
			Destroy (gameObject);
		else if(PhotonNetwork.isMasterClient) {
			PhotonNetwork.Destroy (gameObject);
		}
	}
}
