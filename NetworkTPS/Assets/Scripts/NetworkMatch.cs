using UnityEngine;
using ExitGames.Client.Photon;

public enum MatchType {
	None = 0,
	DeathMatch = 1,
	TeamDeathMatch = 2,
	MeltTheCastle = 3,
	CarryTheSnow = 4,
	RandomMatch = 5
}

public enum TeamType {
	None = 0,
	Team1 = 1,
	Team2 = 2
}

internal abstract class Match {
	// Respawn properties
	protected bool respawnStarted;
	protected float respawnTime;
	protected float respawnDelay;
	protected GameObject[] respawns;
	protected GameObject overviewCamera;

	// Player properties
	protected GameObject player;
	protected TeamType teamType = TeamType.None;

	// Match properties
	protected static bool friendlyFireEnabled = false;
	//protected static Hashtable matchProperties = new Hashtable();

	// Methods	
	public abstract void InitMatch ();
	public abstract void UpdateMatch ();
	public abstract void SpawnMyPlayer ();
	public abstract void ChangeTeam (TeamType team);
	public static bool IsFriendlyFireEnabled() {
		return friendlyFireEnabled;
	}
	public void OnPlayerJoin () {
		Hashtable setPlayerTeam = new Hashtable () {{"Team", teamType}};
		PhotonNetwork.player.SetCustomProperties (setPlayerTeam);

		Hashtable setPlayerKills = new Hashtable () {{"Kills", 0}};
		PhotonNetwork.player.SetCustomProperties (setPlayerKills);

		Hashtable setPlayerDeaths = new Hashtable () {{"Deaths", 0}};
		PhotonNetwork.player.SetCustomProperties (setPlayerDeaths);

		Hashtable setPlayerAssists = new Hashtable () {{"Assists", 0}};
		PhotonNetwork.player.SetCustomProperties (setPlayerAssists);
	}
	public static void OnAKillsB(PhotonPlayer a, PhotonPlayer b, PhotonPlayer[] assists = null) {
		int aKills = (int)a.customProperties ["Kills"];
		aKills++;

		Hashtable setPlayerKills = new Hashtable () {{"Kills", aKills}};
		a.SetCustomProperties (setPlayerKills);

		int bDeaths = (int)b.customProperties ["Deaths"];
		bDeaths++;

		Hashtable setPlayerDeaths = new Hashtable () {{"Deaths", bDeaths}};
		b.SetCustomProperties (setPlayerDeaths);

		if(assists != null)
			foreach(PhotonPlayer player in assists) {
				int pAssists = (int)player.customProperties ["Assists"];
				pAssists++;

				Hashtable setPlayerAssists = new Hashtable() {{"Assists", pAssists}};
				player.SetCustomProperties(setPlayerAssists);
			}

		//DebugScore ();
	}
	static void DebugScore() {
		PhotonPlayer[] players = PhotonNetwork.playerList;

		foreach(PhotonPlayer player in players) {
			Debug.Log ("==" + player.name + "==");
			Debug.Log ("K: " + player.customProperties["Kills"] + ", D: " + player.customProperties["Deaths"] + ", A: " + player.customProperties["Assists"]);
		}
	}
}

internal class DeathMatch : Match {
	public override void InitMatch() {
		overviewCamera = GameObject.FindGameObjectWithTag ("OverviewCamera");
		respawns = GameObject.FindGameObjectsWithTag ("Respawn");
		respawnDelay = 5f;
		teamType = TeamType.None;

		if(PhotonNetwork.isMasterClient) {
			Hashtable matchProperties = new Hashtable();
			matchProperties.Add ("gameType", "deathmatch");

			string[] customPropertiesForLobby = new string[1];
			customPropertiesForLobby[0] = "gameType";

			PhotonNetwork.room.SetCustomProperties (matchProperties);
			PhotonNetwork.room.SetPropertiesListedInLobby (customPropertiesForLobby);
		}

		OnPlayerJoin ();
	}

	public override void UpdateMatch() {
		if(player == null) {
			// If the player is null (not spawned yet or waiting for respawn), unlock the mouse and show the room HUD
			Screen.lockCursor = false;
			
			PlayerHUD.HideHP();
			
			HUDController.ShowRoomHUD();
			
			overviewCamera.SetActive(true);
			
			if(!respawnStarted) {
				respawnStarted = true;
				respawnTime = respawnDelay;
			}
			else {
				GameObject.Find("RoomHUD").
						   transform.FindChild("Message").
						   GetComponent<UILabel>().text = "Respawn Time: " + (int)respawnTime + " seconds remaining";

				if(respawnTime <= 0) {
					respawnStarted = false;
					SpawnMyPlayer();
				}
				else
					respawnTime -= Time.deltaTime;
			}
		}
		
		else {
			// If the player is active, lock the cursor and show the player HUD
			Screen.lockCursor = true;
			
			HUDController.ShowPlayerHUD();
			
			PlayerHUD.ShowHP();
		}
	}

	public override void SpawnMyPlayer() {
		if(respawns == null) {
			Debug.LogError("No respawn points");
			return;
		}
		
		overviewCamera.SetActive (false);
		
		Transform spawnSpot = respawns[Random.Range (0, respawns.Length)].transform;
		
		player = PhotonNetwork.Instantiate ("Player", spawnSpot.position, spawnSpot.transform.localRotation, 0);
		
		MouseLook[] mouseLooks = player.GetComponentsInChildren<MouseLook>();
		foreach(MouseLook script in mouseLooks)
			script.enabled = true;
		
		FireProjectile[] fireProjectiles = player.GetComponentsInChildren<FireProjectile> ();
		foreach(FireProjectile script in fireProjectiles)
			script.enabled = true;
		
		player.transform.FindChild ("Head/Main Camera").gameObject.SetActive (true);
		
		player.GetComponent<NetworkCharacter> ().SetName (PhotonNetwork.player.name);
		player.GetComponent<NetworkCharacter> ().SetTeam (teamType);
	}

	public override void ChangeTeam (TeamType team) {
		// Do nothing since this is a deathmatch
	}
}

public class NetworkMatch : Photon.MonoBehaviour {
	MatchType matchType = MatchType.None;
	Match match;

	void InitMatch(MatchType matchType) {
		switch(matchType) {
		case MatchType.DeathMatch:
			this.matchType = matchType;
			match = new DeathMatch();
			match.InitMatch();
			break;
		case MatchType.RandomMatch:
			int type = Random.Range (1, 4);
			InitMatch ((MatchType)type);
			break;
		default:
			break;
		}
	}

	public void CreateNewMatch(MatchType matchType) {
		InitMatch (matchType);
	}

	public void ReceiveMatchInfo() {
		photonView.RPC ("SendMatchInfo", PhotonTargets.All);
		InitMatch (matchType);
	}

	public static bool IsFriendlyFireEnabled() {
		return Match.IsFriendlyFireEnabled ();
	}

	void Update() {
		if(match != null)
			match.UpdateMatch();
	}

	public static void AKillsB(PhotonPlayer a, PhotonPlayer b, PhotonPlayer[] assists = null) {
		Match.OnAKillsB (a, b, assists);
	}

	[RPC]
	void SendMatchInfo() {
		if(PhotonNetwork.isMasterClient)
			photonView.RPC ("GetMatchInfo", PhotonTargets.All, (int)matchType);
	}

	[RPC]
	void GetMatchInfo(int matchType) {
		if(this.matchType == MatchType.None || this.matchType != (MatchType)matchType)
			InitMatch ((MatchType)matchType);
	}

	public static int GetLocalPlayerKills() {
		return (int)PhotonNetwork.player.customProperties["Kills"];
	}

	public static int GetLocalPlayerDeaths() {
		return (int)PhotonNetwork.player.customProperties["Deaths"];
	}

	public static int GetLocalPlayerAssists() {
		return (int)PhotonNetwork.player.customProperties["Assists"];
	}
}
