using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyPanel : MonoBehaviour
{
    public Animator anim;
    public GameObject descriptionPanel;

    public TMP_Text text;
    public string englishDescription;
    public string russianDescription;

    public void Update()
    {
        if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "english")
        {
            text.text = englishDescription;
        }
        else if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "russian")
        {
            text.text = russianDescription;
        }
    }

    public void Hover()
    {
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("select");
        anim.Play("Button");
        descriptionPanel.SetActive(true);
    }

    public void UnHover()
    {
        anim.Play("ButtonBack");
        descriptionPanel.SetActive(false);
    }
}
