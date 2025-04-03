using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public float musicVolume = 0.3f;
    public float soundFXVolume = 1f;
    public bool isCursorVisibleAlways = true;

    public GameObject settingsPanel;

    public Sprite playSprite;
    public Sprite pauseSprite;

    public Slider musicVolumeSlider;
    public Slider soundFXSlider;
    public Toggle cursorToggle;

    public Image settingsImage;


    void Update()
    {
        if (settingsPanel.activeInHierarchy)
        {
            GiveOutAllOfTheValues();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !settingsPanel.activeInHierarchy)
        {
            ShowSettings();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && settingsPanel.activeInHierarchy)
        {
            HideSettings();
        }
    }

    public void ButtonPress()
    {
        if (settingsPanel.activeInHierarchy)
        {
            HideSettings();
        }
        else if (!settingsPanel.activeInHierarchy)
        {
            ShowSettings();
        }
    }

    public void ShowSettings()
    {
        Cursor.visible = true;
        settingsPanel.SetActive(true);
        SetTheValues();
        settingsImage.sprite = playSprite;
        Time.timeScale = 0;
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("press");
    }

    public void HideSettings()
    {
        if (!isCursorVisibleAlways)
        {
            Cursor.visible = false;
        }
        settingsPanel.SetActive(false);
        settingsImage.sprite = pauseSprite;
        Time.timeScale = 1;
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("press");
    }


    public void SetTheValues()
    {
        musicVolumeSlider.value = musicVolume;
        soundFXSlider.value = soundFXVolume;
        cursorToggle.isOn = isCursorVisibleAlways;
    }

    public void GiveOutAllOfTheValues()
    {
        GameObject.FindGameObjectWithTag("WaveManager").GetComponent<WaveManager>().isCursorVisibleAlways = isCursorVisibleAlways;
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().ChangeSoundFXVolume(soundFXVolume);
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().mainSongSource.volume = musicVolume;
        musicVolume = musicVolumeSlider.value;
        soundFXVolume = soundFXSlider.value;
        isCursorVisibleAlways = cursorToggle.isOn;
    }
}
