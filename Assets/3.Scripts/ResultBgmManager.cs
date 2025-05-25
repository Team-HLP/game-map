using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultBgmManager : MonoBehaviour
{
    private AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        if (audio != null) audio.Play();
    }

    void Destroy()
    {
        if (audio != null) audio.Stop();
    }
}
