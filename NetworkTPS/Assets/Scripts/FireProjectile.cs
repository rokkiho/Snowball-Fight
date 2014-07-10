using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class FireProjectile : Photon.MonoBehaviour {
	[SerializeField]
	Rigidbody missile;

	[SerializeField]
	float firePower = 10;

	[SerializeField]
	float fireRate = 0.5f;

	float fireTime = 0.0f;

	[SerializeField]
	Transform[] firePoints;

	// Update is called once per frame
	void Update () {
		if(photonView.isMine) {
			fireTime += Time.deltaTime;

			if(Input.GetAxis ("Fire1") > 0f) {
				if(fireTime >= fireRate) {
					photonView.RPC ("CreateProjectile", PhotonTargets.MasterClient);
				}
			}
		}
	}

	[RPC]
	void CreateProjectile() {
		foreach(Transform firePoint in firePoints) {
			Rigidbody obj = PhotonNetwork.Instantiate(missile.gameObject.name, firePoint.position, Quaternion.identity, 0).GetComponent<Rigidbody>();
			obj.gameObject.GetComponent<SphereCollider>().enabled = true;
			obj.useGravity = true;
			obj.AddForce(firePoint.forward * firePower, ForceMode.Impulse);
		}
		
		fireTime = 0;
	}
}
