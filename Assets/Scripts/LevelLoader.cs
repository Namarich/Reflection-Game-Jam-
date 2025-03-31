using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    public float waitTime = 1f;

    // Update is called once per frame



    public void LoadNextLevel(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }


    IEnumerator LoadLevel(string sceneName)
    {
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
        LoadNextLevel("Menu");
    }
}
