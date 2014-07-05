using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	[SerializeField]
	string version = "0.0.1ad";

	GameObject overviewCamera;

	GameObject[] respawns;

	GameObject player;

	bool offlineMode = false;

	bool gameStarted = false;
	bool respawnStarted = false;

	[SerializeField]
	float respawnDelay = 5f;

	float respawnTime = 0f;

	// Use this for initialization
	void Start () {
		overviewCamera = GameObject.FindGameObjectWithTag ("OverviewCamera");

		respawns = GameObject.FindGameObjectsWithTag ("Respawn");

		Connect ();
	}

	void Update() {
		if(!gameStarted) return;

		if(player == null) {
			overviewCamera.SetActive(true);

			if(!respawnStarted)
				StartCoroutine("WaitToRespawn");
			else
				respawnTime -= Time.deltaTime;
		}
	}

	IEnumerator WaitToRespawn() {
		respawnStarted = true;
		respawnTime = respawnDelay;
		yield return new WaitForSeconds (respawnDelay);

		SpawnMyPlayer ();
		respawnStarted = false;
	}

	void Connect() {
		if(offlineMode) {
			PhotonNetwork.offlineMode = true;
			OnJoinedLobby();
		}
		else {
			PhotonNetwork.ConnectUsingSettings (version);
		}
	}

	void OnGUI() {
		GUILayout.BeginVertical ();
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());

		if(respawnStarted) {
			GUILayout.Label ("Waiting to respawn");
			GUILayout.Label (respawnTime + " seconds remaining");
		}

		if(player != null)
			GUILayout.Label (player.GetComponent<NetworkHealth>().GetCurrHealth() + "/" + player.GetComponent<NetworkHealth>().GetMaxHealth());

		GUILayout.EndVertical ();
	}

	void OnJoinedLobby() {
		Debug.Log ("Joined Lobby");
		PhotonNetwork.JoinRandomRoom ();
	}

	void OnPhotonRandomJoinFailed() {
		Debug.Log ("Failed to join Lobby");
		PhotonNetwork.CreateRoom (null);
	}

	void OnJoinedRoom() {
		Debug.Log ("Joined Room");

		gameStarted = true;
		//SpawnMyPlayer ();
	}

	void SpawnMyPlayer() {
		if(respawns == null) {
			Debug.LogError("No respawn points");
			return;
		}

		overviewCamera.SetActive (false);

		GameObject spawnSpot = respawns[Random.Range (0, respawns.Length)];

		player = PhotonNetwork.Instantiate ("Player", spawnSpot.transform.position, spawnSpot.transform.localRotation, 0);

		player.GetComponent<TPSCharacter> ().enabled = true;

		MouseLook[] mouseLooks = player.GetComponentsInChildren<MouseLook>();
		foreach(MouseLook script in mouseLooks)
			script.enabled = true;

		FireProjectile[] fireProjectiles = player.GetComponentsInChildren<FireProjectile> ();
		foreach(FireProjectile script in fireProjectiles)
			script.enabled = true;

		player.transform.FindChild ("Head/Main Camera").gameObject.SetActive (true);

		player.GetComponent<NetworkHealth> ().enabled = true;
	}
}
