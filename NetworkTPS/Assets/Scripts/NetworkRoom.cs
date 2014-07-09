using UnityEngine;
using System.Collections.Generic;

public class NetworkRoom : MonoBehaviour {

	static NetworkMatch match;

	void Start() {
		match = transform.FindChild ("NetworkMatch").GetComponent<NetworkMatch>();
	}

	public static void StartDeathMatch() {
		match.CreateNewMatch (MatchType.DeathMatch);
	}

	public static void ReceiveMatchInfo() {
		match.ReceiveMatchInfo ();
	}
}
