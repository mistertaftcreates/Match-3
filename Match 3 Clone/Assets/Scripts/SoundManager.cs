using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioSource[] destroyNoise;

    public void PlayRandomDestroyNoise()
	{
		//Choose a random number
		int clipToPlay = Random.Range(0, destroyNoise.Length);
		//play that clip
		destroyNoise[clipToPlay].Play();
	}
}
