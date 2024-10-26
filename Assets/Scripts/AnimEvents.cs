using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
	public ThirdPerson player;
	private AudioSource footstep;
	private AudioSource attack;

	public List<AudioClip> grassFS;
	public List<AudioClip> punch;

	void Awake()
    {
		footstep = player.GetComponents<AudioSource>()[2];
		attack = player.GetComponents<AudioSource>()[3];
    }

	private void PlayFootstep()
	{
		if(!player.isWalking || player.isJumping)
		{
			footstep.Stop();
			return;
		}

		if(!footstep.isPlaying)
		{
			AudioClip clip = grassFS[Random.Range(0, grassFS.Count)];
			footstep.clip = clip;

			footstep.Play();
		}
	}

	private void PlayPunch()
	{
		AudioClip clip = punch[Random.Range(0, punch.Count)];
		attack.clip = clip;

		attack.Play();
	}
}
