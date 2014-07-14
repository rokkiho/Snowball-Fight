using UnityEngine;
using System.Collections;

public class KDAHUD : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		GetComponent<UILabel> ().text = NetworkMatch.GetLocalPlayerKills () + " / " + NetworkMatch.GetLocalPlayerDeaths () + " / " + NetworkMatch.GetLocalPlayerAssists ();
	}
}
