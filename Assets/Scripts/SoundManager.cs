using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource audi;
    public AudioSource mainSongSource;

    [System.Serializable]
    public class Sound
    {
        public AudioClip clip;
        public string name;
    }


    public List<Sound> sounds;

    public void PlaySound(string name)
    {
        int i = 0;
        AudioClip b = sounds[i].clip;
        while (sounds[i].name != name)
        {
            i += 1;
        }
        b = sounds[i].clip;

        audi.clip = b;
        audi.Play();
    }
}
