using UnityEngine;
using System.Collections;

public class NetworkProjectile : MonoBehaviour {

	[SerializeField]
	float lifeTime = 2f;
	
	float elapsedTime = 0.0f;
	
	[SerializeField]
	float damage = 10;

	[SerializeField]
	Transform explosionEffect;

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	NetworkCharacter owner;
	int projectileID;
	double creationTime;

	void Start() {
		realPosition = transform.position;
		realRotation = transform.rotation;
	}

	// Update is called once per frame
	void Update () {
		float timePassed = (float)(PhotonNetwork.time - creationTime);
		
		if(timePassed > lifeTime) {
			OnProjectileHit();
		}
	}

	public void SetCreationTime(double time) {
		creationTime = time;
	}

	public void SetOwner(NetworkCharacter owner) {
		this.owner = owner;
	}

	public void SetProjectileID(int id) {
		projectileID = id;
	}

	public int GetProjectileID() {
		return projectileID;
	}

	public void OnProjectileHit() {
		Instantiate (explosionEffect, transform.position, Quaternion.identity);
		Destroy (gameObject);
	}

	void OnCollisionEnter(Collision col) {
		NetworkCharacter character = col.gameObject.GetComponent<NetworkCharacter>();
		if(character != null && !character.GetNetworkView().isMine) {
			PlayerHUD.ShowCrossHairHit();
			character.GetNetworkView().RPC ("ApplyDamage", PhotonTargets.All, damage);
		}
		OnProjectileHit();
	}
}
