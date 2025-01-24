using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilScripts : MonoBehaviour
{
    public List<AudioClip> clips;

    public void PlaySound(AudioSource source)
    {
        int randomNumber = Random.Range(0, 3);

        source.clip = clips[randomNumber];
        source.Play();

    }
}
