using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    private const int reviveId=1;
    public GameObject reviveAdGameObject;
    public float durationOfReviveAd;
    public float startOfReviveAd;

    public void ReviveAdButton()
    {
        YGAdsProvider.ShowRewardedAd(reviveId);
        reviveAdGameObject.SetActive(false);
    }

    public void RewardForAd(int id)
    {
        if (id != reviveId)
        {
            return;
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().hasRevived = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().currentHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().maxHealth;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().FillTheHealthBar();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Reset();
    }

    private void Update()
    {
        if (reviveAdGameObject.activeInHierarchy && startOfReviveAd+durationOfReviveAd <= Time.time)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Reset();
        }
    }
}
