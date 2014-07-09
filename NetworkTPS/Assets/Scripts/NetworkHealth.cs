using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class NetworkHealth : Photon.MonoBehaviour {

	[SerializeField]
	float maxHealth = 100;

	float currHealth;

	// Use this for initialization
	void Start () {
		currHealth = maxHealth;
	}

	public float GetCurrHealth() {
		return currHealth;
	}

	public float GetMaxHealth() {
		return maxHealth;
	}

	void Update() {
		HPHUD.UpdateHealthInfo (currHealth, maxHealth);
	}

	[RPC]
	public void ApplyDamage(float amount) {
		currHealth -= amount;

		if(currHealth <= 0) {
			currHealth = 0;
			Die();
		}
	}

	void Die() {
		if(photonView.instantiationId == 0)
			Destroy (gameObject);
		else if(photonView.isMine)
			PhotonNetwork.Destroy(gameObject);
	}
}
