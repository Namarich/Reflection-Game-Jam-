using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public static class YGAdsProvider
{
    public static void TryShowFullscreenAd(float probability)
    {
        float a = Random.Range(0f, 1f);
        if (a <= probability)
        {
            YandexGame.FullscreenShow();
        }
        else
        {
            return;
        }
        
    }

    public static void ShowRewardedAd(int id)
    {
        YandexGame.RewVideoShow(id);
    }
}
