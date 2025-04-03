using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public GameObject soundObject;
    public List<GameObject> soundObjects;

    //public AudioSource audi;
    public AudioSource mainSongSource;

    [System.Serializable]
    public class Sound
    {
        public AudioClip clip;
        public string name;
    }

    private void Update()
    {
        CheckAllAudioObjects();
    }


    public List<Sound> sounds;

    public void PlaySound(string name)
    {
        GameObject audi = GetFreeAudioObject();
        if (audi == null)
        {
            CreateNewSoundObject();
            audi = GetFreeAudioObject();
        }
        int i = 0;
        AudioClip b = sounds[i].clip;
        while (sounds[i].name != name)
        {
            i += 1;
        }
        b = sounds[i].clip;

        audi.SetActive(true);
        audi.GetComponent<AudioSource>().clip = b;
        audi.GetComponent<AudioSource>().Play();
    }

    public GameObject GetFreeAudioObject()
    {
        foreach(GameObject a in soundObjects)
        {
            if (!a.activeInHierarchy)
            {
                return a;
            }
        }
        return null;
    }

    public void CreateNewSoundObject()
    {
        GameObject a = Instantiate(soundObject, transform.position,Quaternion.identity);
        a.transform.parent = gameObject.transform;
        soundObjects.Add(a);
        a.SetActive(false);
    }

    public void CheckAllAudioObjects()
    {
        foreach (GameObject a in soundObjects)
        {
            if (!a.GetComponent<AudioSource>().isPlaying)
            {
                a.SetActive(false);
            }
        }
    }

    public void ChangeSoundFXVolume(float _volume)
    {
        foreach (GameObject a in soundObjects)
        {
            a.GetComponent<AudioSource>().volume = _volume;
        }
    }
}
