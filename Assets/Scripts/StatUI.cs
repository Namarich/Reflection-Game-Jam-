using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatUI : MonoBehaviour
{

    private Player player;

    public enum Stats {damage, health, shotSpeed, projectileSpeed, playerSpeed, projectileSizeUp};

    public Stats stat;

    public string description;
    public string russianDescription;

    public TMP_Text descriptionText;
    public TMP_Text valueText;

    public float value;

    public GameObject descriptionPanel;
    public WaveManager waveMan;

    public TMP_Text addedValue;
    public GameObject addedValueText;



    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        if (waveMan.IsSelectionScreen)
        {
            FindTheValue();
        }
    }

    public void FindTheValue()
    {
        switch (stat)
        {
            case Stats.damage:
                value = player.projectileDamage;
                break;
            case Stats.health:
                value = player.maxHealth;
                break;
            case Stats.shotSpeed:
                value = player.shotSpeed;
                break;
            case Stats.projectileSpeed:
                value = player.projectileSpeed;
                break;
            case Stats.playerSpeed:
                value = player.moveSpeed;
                break;
            case Stats.projectileSizeUp:
                value = player.projectileSize;
                break;
        }
        //DisableAddedValueText();
        UpdateTheUI();
    }

    public void UpdateTheUI()
    {
        valueText.text = value.ToString();
        if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "english")
        {
            descriptionText.text = description;
        }
        else if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "russian")
        {
            descriptionText.text = russianDescription;
        }
    }

    public void Hover()
    {
        descriptionPanel.SetActive(true);
    }

    public void Unhover()
    {
        descriptionPanel.SetActive(false);
    }


    public void UpdateAddedValue(string b)
    {
        addedValueText.SetActive(true);
        addedValue.text = b;
    }

    public void DisableAddedValueText()
    {
        addedValueText.SetActive(false);
    }
}
