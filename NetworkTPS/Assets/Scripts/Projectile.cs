using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class Projectile : Photon.MonoBehaviour {

	[SerializeField]
	float lifeTime = 2f;

	float elapsedTime = 0.0f;

	[SerializeField]
	float damage = 10;

	[SerializeField]
	Transform explosionEffect;

	// Update is called once per frame
	void Update () {
		if(GetComponent<PhotonView>().isMine) {
			elapsedTime += Time.deltaTime;
			
			if(elapsedTime >= lifeTime) {
				photonView.RPC ("DestroyProjectile", PhotonTargets.All, transform.position);
			}
		}
	}

	void OnCollisionEnter(Collision col) {
		if(GetComponent<PhotonView>().isMine) {
			if(col.gameObject.GetComponentInChildren<NetworkHealth>() != null) {
				PlayerHUD.ShowCrossHairHit();
				PhotonView targetPv = col.gameObject.GetComponent<PhotonView>();
				targetPv.RPC("ApplyDamage", PhotonTargets.All, damage);
			}

			photonView.RPC ("DestroyProjectile", PhotonTargets.All, transform.position);
		}
	}

	[RPC]
	void DestroyProjectile(Vector3 position) {
		Instantiate (explosionEffect, position, Quaternion.identity);
		if(photonView.instantiationId == 0)
			Destroy (gameObject);
		else if(photonView.isMine) {
			PhotonNetwork.Destroy (gameObject);
		}
	}
}
