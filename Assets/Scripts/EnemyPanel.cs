using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPanel : MonoBehaviour
{
    public Animator anim;
    public GameObject descriptionPanel;

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
