using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class FireProjectile : MonoBehaviour {
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
		fireTime += Time.deltaTime;

		if(Input.GetAxis ("Fire1") > 0f) {
			if(fireTime >= fireRate) {
				GetComponent<PhotonView>().RPC ("CreateProjectile", PhotonTargets.All);

				fireTime = 0;
			}
		}
	}

	[RPC]
	void CreateProjectile() {
		if(GetComponent<PhotonView>().isMine)
			foreach(Transform firePoint in firePoints) {
				Rigidbody obj = PhotonNetwork.Instantiate(missile.gameObject.name, firePoint.position, Quaternion.identity, 0).GetComponent<Rigidbody>();
				obj.useGravity = true;
				obj.AddForce(firePoint.forward * firePower, ForceMode.Impulse);
				obj.gameObject.GetComponent<SphereCollider>().enabled = true;
			}
	}
}
