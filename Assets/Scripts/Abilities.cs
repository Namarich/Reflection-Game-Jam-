using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class Abilities : MonoBehaviour
{
    private enum Function {PlayerSpeedUp, ProjectileSpeedUp, ProjectileDamageUp, PlayerHealthUp, ShotSpeedUp, ProjectileSpeedReduction};

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
        player.shotSpeed = RoundUpTheFloat(RoundUpTheFloat(player.shotSpeed) * (2-effect));
    }

    public void ProjectileSpeedReduction()
    {
        player.projectileSpeedReduction = RoundUpTheFloat(player.projectileSpeedReduction * effect);
    }


    public void UpdateAbilities()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        switch (function)
        {
            case Function.PlayerSpeedUp:
                abilityName = "Player SpeedUp";
                effect = 1;
                effectSign = "+";
                isPercentageBased = false;
                initialValue = player.moveSpeed;
                break;

            case Function.ProjectileSpeedUp:
                abilityName = "Projectile SpeedUp";
                effect = 1.25f;
                effectSign = "*";
                isPercentageBased = true;
                initialValue = player.projectileSpeed;
                break;

            case Function.ProjectileDamageUp:
                abilityName = "Projectile DamageUp";
                effect = 1.2f;
                effectSign = "*";
                isPercentageBased = true;
                initialValue = player.projectileDamage;
                break;

            case Function.PlayerHealthUp:
                abilityName = "Player HealthUp";
                effect = 30;
                effectSign = "+";
                isPercentageBased = false;
                initialValue = player.maxHealth;
                break;

            case Function.ShotSpeedUp:
                abilityName = "Shot SpeedUp";
                effect = 1.15f;
                effectSign = "/";
                isPercentageBased = true;
                initialValue = player.shotSpeed;
                break;

            case Function.ProjectileSpeedReduction:
                abilityName = "Projectile Speed ReductionDown";
                effect = 1.3f;
                effectSign = "*";
                isPercentageBased = true;
                initialValue = player.projectileSpeedReduction;
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

            case Function.ProjectileSpeedReduction:
                ProjectileSpeedReduction();
                break;
        }
        waveMan.NextWave();
    }

    public void TakeRandomEnum()
    {
        function = (Function)Random.Range(0, Function.GetValues(typeof(Function)).Length);
        while (waveMan.selectedAbilities.Contains(function.ToString()))
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
            effectText.text = effectSign + effect.ToString();
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
        foreach (StatUI a in stats)
        {
            if (a.value == initialValue)
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
    }

    public void Unhover()
    {
        anim.Play("ButtonBack");
        foreach (StatUI a in stats)
        {

            a.DisableAddedValueText();

        }
    }

    public float RoundUpTheFloat(float a)
    {

        //return Mathf.Round(a * 100.0f) * 0.01f;
        return (float)System.Math.Round(a, 2);
    }
}
