using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TweenColor), typeof(TweenScale))]
public class HPHUDDetector : MonoBehaviour {

	TweenColor tweenColor;
	TweenScale tweenScale;

	[SerializeField]
	NetworkManager networkManager;

	[SerializeField]
	float alarmThreshold = 0.3f;

	// Use this for initialization
	void Start () {
		tweenColor = GetComponent<TweenColor> ();
		tweenScale = GetComponent<TweenScale> ();
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = networkManager.GetPlayer ();
		if(player != null) {
			float currHP = player.GetComponent<NetworkHealth>().GetCurrHealth();
			float maxHP = player.GetComponent<NetworkHealth>().GetMaxHealth();

			if((currHP / maxHP) <= alarmThreshold) {
				tweenColor.enabled = true;
				tweenScale.enabled = true;
			}
			else {
				tweenColor.Sample(0, false);
				tweenColor.enabled = false;
				tweenScale.Sample(0, false);
				tweenScale.enabled = false;
			}
		}
	}
}
