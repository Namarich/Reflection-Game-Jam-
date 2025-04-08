using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class Abilities : MonoBehaviour
{
    private enum Function { PlayerSpeedUp, ProjectileSpeedUp, ProjectileDamageUp, PlayerHealthUp, ShotSpeedUp, ProjectileSizeUp, ExtraBullet, Acceleration, ExplosiveImpact};

    private Function function;

    private Player player;

    private string abilityName;
    private string effectSign;
    private float effect;
    private bool isPercentageBased;

    private float initialValue;

    public TMP_Text nameText;
    public TMP_Text effectText;

    public WaveManager waveMan;

    public Animator anim;

    public List<StatUI> stats;

    public List<GameObject> specialAbilities;

    private string rarity;
    public List<Color> colors;
    public TMP_Text rarityText;

    private void Start()
    {
        foreach (GameObject a in specialAbilities)
        {
            a.SetActive(false);
        }
    }

    private void Update()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

    public void PlayerSpeedUp()
    {
        player.moveSpeed += effect;
        
    }

    public void ProjectileSpeedUp()
    {
        player.projectileSpeed = RoundUpTheFloat(effect * player.projectileSpeed);

    }

    public void ProjectileDamageUp()
    {
        player.projectileDamage = RoundUpTheFloat(effect * player.projectileDamage);

    }

    public void PlayerHealthUp()
    {
        player.maxHealth += effect;
        player.Heal(player.maxHealth);

    }

    public void ShotSpeedUp()
    {
        player.shotSpeed = RoundUpTheFloat(RoundUpTheFloat(player.shotSpeed) * (2 - effect));

    }

    public void ProjectileSizeUp()
    {
        //player.projectileSpeedReduction = RoundUpTheFloat(player.projectileSpeedReduction * effect);
        player.projectileSize *= effect;

    }

    public void ExtraBullet()
    {
        player.isExtraBullet = true;
        waveMan.cannotUseAbilities.Add(function.ToString());
        specialAbilities[0].SetActive(true);
    }

    public void Acceleration()
    {
        player.isDoublingSpeedBullet = true;
        waveMan.cannotUseAbilities.Add(function.ToString());
        specialAbilities[1].SetActive(true);
    }

    public void ExplosiveImpact()
    {
        player.isExplosiveImpact = true;
        waveMan.cannotUseAbilities.Add(function.ToString());
        specialAbilities[3].SetActive(true);
    }


    public void UpdateAbilities()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        switch (function)
        {
            case Function.PlayerSpeedUp:
                abilityName = "Player SpeedUp";
                effect = 2;
                effectSign = "+";
                isPercentageBased = false;
                initialValue = player.moveSpeed;
                rarity = "Common";
                break;

            case Function.ProjectileSpeedUp:
                abilityName = "Projectile SpeedUp";
                effect = 1.4f;
                effectSign = "*";
                isPercentageBased = true;
                initialValue = player.projectileSpeed;
                rarity = "Rare";
                break;

            case Function.ProjectileDamageUp:
                abilityName = "Projectile DamageUp";
                effect = 1.35f;
                effectSign = "*";
                isPercentageBased = true;
                initialValue = player.projectileDamage;
                rarity = "Common";
                break;

            case Function.PlayerHealthUp:
                abilityName = "Player HealthUp";
                effect = 50;
                effectSign = "+";
                isPercentageBased = false;
                initialValue = player.maxHealth;
                rarity = "Common";
                break;

            case Function.ShotSpeedUp:
                abilityName = "Shot SpeedUp";
                effect = 1.15f;
                effectSign = "/";
                isPercentageBased = true;
                initialValue = player.shotSpeed;
                rarity = "Rare";
                break;

            case Function.ProjectileSizeUp:
                abilityName = "Projectile SizeUp";
                effect = 1.15f;
                effectSign = "*";
                isPercentageBased = true;
                initialValue = player.projectileSize;
                rarity = "Rare";
                break;

            case Function.ExtraBullet:
                abilityName = "Extra Bullet";
                effect = 0;
                effectSign = "+ 1 bullet";
                isPercentageBased = false;
                initialValue = 0;
                rarity = "Epic";
                break;

            case Function.Acceleration:
                abilityName = "Acceleration";
                effect = 0;
                effectSign = "Every 2 reflections double the move speed of a bullet";
                isPercentageBased = false;
                initialValue = 0;
                rarity = "Rare";
                break;

            case Function.ExplosiveImpact:
                abilityName = "Explosive Impact";
                effect = 0;
                effectSign = "If 2 bullets collide, they produce an explosion, which deals 2x damage";
                isPercentageBased = false;
                initialValue = 0;
                rarity = "Epic";
                break;
        }
    }

    public void Activate()
    {
        switch (function)
        {
            case Function.PlayerSpeedUp:
                PlayerSpeedUp();
                break;

            case Function.ProjectileSpeedUp:
                ProjectileSpeedUp();
                break;

            case Function.ProjectileDamageUp:
                ProjectileDamageUp();
                break;

            case Function.PlayerHealthUp:
                PlayerHealthUp();
                break;

            case Function.ShotSpeedUp:
                ShotSpeedUp();
                break;

            case Function.ProjectileSizeUp:
                ProjectileSizeUp();
                break;

            case Function.ExtraBullet:
                ExtraBullet();
                break;

            case Function.Acceleration:
                Acceleration();
                break;


            case Function.ExplosiveImpact:
                ExplosiveImpact();
                break;
        }
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("press");
        if (!waveMan.enemyInfoScreen.activeSelf)
        {
            waveMan.EnemyInfoScreen();
        }
        
    }

    public void TakeRandomEnum()
    {
        function = (Function)Random.Range(0, Function.GetValues(typeof(Function)).Length);
        while (waveMan.selectedAbilities.Contains(function.ToString()) || waveMan.cannotUseAbilities.Contains(function.ToString()))
        {
            function = (Function)Random.Range(0, Function.GetValues(typeof(Function)).Length);
        }
        waveMan.selectedAbilities.Add(function.ToString());
        UpdateAbilities();
        nameText.text = abilityName;
        if (isPercentageBased)
        {
            if (effectSign == "/")
            {
                effectText.text = "-" + ((effect - 1) * 100).ToString() + "%";
            }
            else
            {
                effectText.text = "+" + ((effect - 1) * 100).ToString() + "%";
            }

        }
        else
        {
            if (effect != 0)
            {
                effectText.text = effectSign + effect.ToString();
            }
            else
            {
                effectText.text = effectSign;
            }

        }

        rarityText.text = rarity;
        if (rarity == "Common")
        {
            rarityText.color = colors[0];
        }
        else if (rarity == "Rare")
        {
            rarityText.color = colors[1];
            if (Random.Range(0,1) > 0.7f)
            {
                TakeRandomEnum();
            }
        }
        else if (rarity == "Epic")
        {
            rarityText.color = colors[2];
            if (Random.Range(0, 1) > 0.5f)
            {
                TakeRandomEnum();
            }
        }
        else if (rarity == "Legendary")
        {
            rarityText.color = colors[3];
            if (Random.Range(0, 1) > 0.3f)
            {
                TakeRandomEnum();
            }
        }

        
    }

        
        
        
    

    public void RemoveMyselfFromList()
    {
        if (function.ToString() != "")
        {
            if (waveMan.selectedAbilities.Contains(function.ToString()))
            {
                waveMan.selectedAbilities.Remove(function.ToString());
            }
        }
    }



    public void Hover()
    {
        anim.Play("Button");
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("select");
        foreach (StatUI a in stats)
        {
            if (a.value == initialValue && initialValue != 0)
            {
                if (!isPercentageBased)
                {
                    a.UpdateAddedValue(effectSign+effect.ToString());
                    break;
                }
                else
                {
                    if (effectSign == "/")
                    {
                        a.UpdateAddedValue($"-{RoundUpTheFloat(((effect - 1) * initialValue))}");
                    }
                    else
                    {
                        a.UpdateAddedValue($"+{RoundUpTheFloat(((effect - 1) * initialValue))}");
                    }
                    break;
                    
                }
            }
        }
        if (waveMan.isTutorial)
        {
            waveMan.tutorialPanel.SetActive(false);
        }
    }

    public void Unhover()
    {
        anim.Play("ButtonBack");
        foreach (StatUI a in stats)
        {

            a.DisableAddedValueText();

        }

        if (waveMan.isTutorial)
        {
            waveMan.tutorialPanel.SetActive(true);
        }
    }

    public float RoundUpTheFloat(float a)
    {

        //return Mathf.Round(a * 100.0f) * 0.01f;
        return (float)System.Math.Round(a, 2);
    }
}
