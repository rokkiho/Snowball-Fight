using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	[SerializeField]
	string version = "0.0.1ad";

	GameObject overviewCamera;

	GameObject[] respawns;

	GameObject player;
	string playerName;

	bool offlineMode = false;

	bool gameStarted = false;
	bool respawnStarted = false;

	[SerializeField]
	float respawnDelay = 5f;

	float respawnTime = 0f;

	[SerializeField]
	GameObject RoomHUDPanel;
	[SerializeField]
	GameObject PlayerHUD;
	[SerializeField]
	GameObject CrosshairHit;
	[SerializeField]
	GameObject PlayerNameHUD;

	bool showCrosshairHit = false;
	[SerializeField]
	float crosshairHitDuration = 2f;
	float crosshairHitElapsedTime = 0f;

	int maxPlayersPerRoom = 20;

	// Use this for initialization
	void Start () {
		overviewCamera = GameObject.FindGameObjectWithTag ("OverviewCamera");

		respawns = GameObject.FindGameObjectsWithTag ("Respawn");

		//Connect ();
	}

	void Connect() {
		PlayerNameHUD.SetActive (false);

		if(offlineMode) {
			PhotonNetwork.offlineMode = true;
			OnJoinedLobby();
		}
		else {
			PhotonNetwork.ConnectUsingSettings (version);
		}
	}

	IEnumerator WaitToRespawn() {
		respawnStarted = true;
		respawnTime = respawnDelay;
		yield return new WaitForSeconds (respawnDelay);

		SpawnMyPlayer ();
		respawnStarted = false;
	}

	void OnJoinedLobby() {
		Debug.Log ("Joined Lobby");

		PhotonNetwork.JoinRandomRoom ();
	}

	void OnPhotonRandomJoinFailed() {
		Debug.Log ("Failed to join Lobby");

		RoomInfo[] availableRooms = PhotonNetwork.GetRoomList ();
		
		foreach(RoomInfo room in availableRooms) {
			if(room.playerCount < maxPlayersPerRoom)
				PhotonNetwork.JoinRoom(room.name);
		}
		
		RoomOptions roomOptions = new RoomOptions ();
		roomOptions.isVisible = true;
		roomOptions.maxPlayers = maxPlayersPerRoom;

		PhotonNetwork.JoinOrCreateRoom ("Room#" + Random.Range(0, 999999), roomOptions, TypedLobby.Default);
	}

	void OnJoinedRoom() {
		Debug.Log ("Joined Room");

		gameStarted = true;
	}

	void Update() {
		if(!gameStarted) {
			// If the game has not started (Joining room) yet, unlock the mouse and show the room HUD
			Screen.lockCursor = false;
			
			RoomHUDPanel.SetActive (true);
			PlayerHUD.SetActive (false);
			
			UILabel label = RoomHUDPanel.GetComponentInChildren<UILabel> ();
			
			label.text = PhotonNetwork.connectionStateDetailed.ToString ();
		}
		
		else if(player == null) {
			// If the player is null (not spawned yet or waiting for respawn), unlock the mouse and show the room HUD
			Screen.lockCursor = false;
			
			RoomHUDPanel.SetActive (true);
			PlayerHUD.SetActive (false);
			
			overviewCamera.SetActive(true);
			
			if(!respawnStarted)
				StartCoroutine("WaitToRespawn");
			else {
				UILabel label = RoomHUDPanel.GetComponentInChildren<UILabel> ();
				
				label.text = "Respawn Time: " + (int)respawnTime + " seconds remaining";
				
				respawnTime -= Time.deltaTime;
			}
		}
		else {
			// If the player is active, lock the cursor and show the player HUD
			Screen.lockCursor = true;
			
			RoomHUDPanel.SetActive (false);
			PlayerHUD.SetActive(true);
			
			UILabel label = PlayerHUD.GetComponentInChildren<UILabel>();
			label.text = player.GetComponent<NetworkHealth>().GetCurrHealth() + "";
			
			if(showCrosshairHit) {
				CrosshairHit.SetActive(true);
				crosshairHitElapsedTime += Time.deltaTime;
				
				if(crosshairHitElapsedTime >= crosshairHitDuration) {
					showCrosshairHit = false;
				}
			}
			else
				CrosshairHit.SetActive(false);
		}
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

		player.GetComponent<NetworkCharacter> ().SetName (playerName);
	}

	public void ShowCrosshairHit() {
		showCrosshairHit = true;
		crosshairHitElapsedTime = 0f;
	}

	public GameObject GetPlayer() {
		return player;
	}

	public void OnPlayerNameSubmit() {
		string name = PlayerNameHUD.transform.FindChild ("Window/NameInput/Label").GetComponent<UILabel> ().text;

		if(name != null && name.Length > 0) {
			playerName = name;
			Connect();
		}
	}
}
