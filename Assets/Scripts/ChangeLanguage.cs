using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeLanguage : MonoBehaviour
{
    public string englishText;
    public string russianText;

    public TMP_Text text;

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "english")
        {
            text.text = englishText;
        }
        else if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "russian")
        {
            text.text = russianText;
        }
    }
}
