using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffect : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if(GetComponent<ParticleSystem>().isStopped) {
			Destroy (gameObject);
		}
	}
}
