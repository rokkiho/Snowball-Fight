using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerHUD : MonoBehaviour {

	// crosshairhit
	static float crossHairHitDisplayTime = 1f;
	static float crossHairHitDisplayElapsedTime = 0f;
	[SerializeField]
	GameObject crossHairHitHUDObj;
	static bool shouldShowCrossHairHit = false;

	// hp
	[SerializeField]
	GameObject hpHUDObj;
	static bool shouldShowHp;

	public static void ShowCrossHairHit() {
		shouldShowCrossHairHit = true;
		crossHairHitDisplayElapsedTime = 0f;
	}

	public static void ShowHP() {
		shouldShowHp = true;
	}

	public static void HideHP() {
		shouldShowHp = false;
	}

	void Update() {
		if(shouldShowCrossHairHit) {
			crossHairHitHUDObj.SetActive(true);
			crossHairHitDisplayElapsedTime += Time.deltaTime;

			if(crossHairHitDisplayElapsedTime >= crossHairHitDisplayTime)
				shouldShowCrossHairHit = false;
		}
		else {
			crossHairHitHUDObj.SetActive(false);
		}

		if(shouldShowHp) {
			hpHUDObj.SetActive(true);
		}
		else {
			hpHUDObj.SetActive(false);
		}
	}
}
