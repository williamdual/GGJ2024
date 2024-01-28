using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private Dictionary<String, AudioClip> soundLibrary;
    // Start is called before the first frame update
    void Start()
    {
        soundLibrary = new Dictionary<string, AudioClip>();
        

        AudioClip[] audioArr = Resources.LoadAll<AudioClip>("Sounds");
        for(int i = 0; i < audioArr.Length; i++){
            soundLibrary.Add(audioArr[i].name, audioArr[i]);
        }

    }

    public void PlaySound(String key){
        AudioSource.PlayClipAtPoint(soundLibrary[key], Vector3.zero);
    }
}
