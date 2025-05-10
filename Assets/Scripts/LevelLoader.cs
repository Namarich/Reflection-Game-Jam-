using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    public float waitTime = 1f;

    public Animator buttonAnim;

    public string language = "english";

    // Update is called once per frame

    public void Start()
    {
        if (PlayerPrefs.GetString("language") != "english" && PlayerPrefs.GetString("language") != "russian")
        {
            PlayerPrefs.SetString("language", "english");
        }
        else
        {
            language = PlayerPrefs.GetString("language");
        }
        
    }

    public void LoadNextLevel(string sceneName)
    {
        //StartCoroutine(LoadLevel(sceneName));
        SceneManager.LoadScene(sceneName);
    }


    IEnumerator LoadLevel(string sceneName)
    {
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("press");
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(sceneName);
    }

    public void ToTheGame()
    {
        LoadNextLevel("SampleScene");
    }

    public void ToTheMenu()
    {
        YGAdsProvider.TryShowFullscreenAd(0.7f);
        LoadNextLevel("Menu");
    }

    public void QuitTheGame()
    {
        Application.Quit();
    }

    public void Hover()
    {
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("select");
        buttonAnim.Play("Button");
    }

    public void Unhover()
    {
        buttonAnim.Play("ButtonBack");
    }

    public void ChangeToRussian()
    {
        language = "russian";
        PlayerPrefs.SetString("language", language);
    }

    public void ChangeToEnglish()
    {
        language = "english";
        PlayerPrefs.SetString("language", language);
    }
}
