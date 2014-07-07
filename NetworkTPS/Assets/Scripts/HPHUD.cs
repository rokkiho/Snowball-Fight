using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel), typeof(TweenColor), typeof(TweenScale))]
public class HPHUD : MonoBehaviour {

	TweenColor tweenColor;
	TweenScale tweenScale;

	float alarmThreshold = 0.3f;

	static float maxHealthVal;
	static float currHealthVal;

	// Use this for initialization
	void Start () {
		tweenColor = GetComponent<TweenColor> ();
		tweenScale = GetComponent<TweenScale> ();
	}

	public static void UpdateHealthInfo(float curr, float max) {
		currHealthVal = curr;
		maxHealthVal = max;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<UILabel> ().text = (int)currHealthVal + "";

		if((currHealthVal / maxHealthVal) <= alarmThreshold) {
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
