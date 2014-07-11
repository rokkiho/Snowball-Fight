using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	int lastProjectileID = 0;

	List<NetworkProjectile> projectiles = new List<NetworkProjectile> ();

	// Update is called once per frame
	void Update () {
		if(photonView.isMine) {
			if(Input.GetAxis ("Fire1") > 0f) {
				if(Time.realtimeSinceStartup - fireTime >= fireRate) {
					lastProjectileID++;
					photonView.RPC ("OnShoot", PhotonTargets.All, new object[]{transform.position, lastProjectileID});
				}
			}
		}
	}
	
	void CreateProjectile(Vector3 position, double createTime, int projectileID) {
		fireTime = Time.realtimeSinceStartup;

		foreach(Transform firePoint in firePoints) {
			GameObject obj = ((GameObject)Instantiate(missile.gameObject, firePoint.position, Quaternion.identity));

			NetworkProjectile projectile = obj.GetComponent<NetworkProjectile>();

			projectile.SetCreationTime(createTime);
			projectile.SetProjectileID(projectileID);
			projectile.SetOwner (GetComponent<NetworkCharacter>());

			projectiles.Add (projectile);

			obj.GetComponent<Rigidbody>().AddForce(firePoint.forward * firePower, ForceMode.Impulse);
		}
	}

	public void SendProjectileHit(int projectileID) {
		photonView.RPC ("OnProjectileHit", PhotonTargets.Others, new object[]{projectileID});
	}

	[RPC]
	void OnShoot(Vector3 position, int projectileID, PhotonMessageInfo info) {
		double timestamp = PhotonNetwork.time;

		if(info != null) {
			timestamp = info.timestamp;
		}

		CreateProjectile (position, timestamp, projectileID);
	}

	[RPC]
	void OnProjectileHit(int projectileID) {
		projectiles.RemoveAll (item => item == null);

		NetworkProjectile projectile = projectiles.Find (item => item.GetProjectileID () == projectileID);

		if(projectile != null) {
			projectile.OnProjectileHit();
			projectiles.Remove(projectile);
		}
	}
}
