using UnityEngine;
using System.Collections;

public enum MatchType {
	None = 0,
	DeathMatch = 1,
	TeamDeathMatch = 2,
}

internal abstract class Match {
	protected bool respawnStarted;
	protected float respawnTime;

	protected float respawnDelay;

	protected GameObject overviewCamera;

	protected GameObject player;

	protected GameObject[] respawns;

	public abstract void InitMatch ();
	public abstract void UpdateMatch ();

	public abstract void SpawnMyPlayer ();
}

internal class DeathMatch : Match {
	public override void InitMatch() {
		overviewCamera = GameObject.FindGameObjectWithTag ("OverviewCamera");
		respawns = GameObject.FindGameObjectsWithTag ("Respawn");
		respawnDelay = 5f;
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
		
		player.GetComponent<NetworkCharacter> ().SetName (NetworkManager.GetPlayerName());
	}
}

public class NetworkMatch : Photon.MonoBehaviour {
	MatchType matchType = MatchType.None;
	Match match;

	void InitMatch(MatchType matchType) {
		match = null;

		switch(matchType) {
		case MatchType.DeathMatch:
			this.matchType = matchType;
			match = new DeathMatch();
			match.InitMatch();
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

	void Update() {
		if(match != null)
			match.UpdateMatch();
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
}
