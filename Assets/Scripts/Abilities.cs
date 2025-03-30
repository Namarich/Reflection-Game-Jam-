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

    public TMP_Text nameText;
    public TMP_Text effectText;

    public WaveManager waveMan;


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
        player.projectileSpeed *= effect;
    }

    public void ProjectileDamageUp()
    {
        player.projectileDamage *= effect;
    }

    public void PlayerHealthUp()
    {
        player.maxHealth += effect;
        player.Heal(player.maxHealth);
    }

    public void ShotSpeedUp()
    {
        player.shotSpeed /= effect;
    }

    public void ProjectileSpeedReduction()
    {
        player.projectileSpeedReduction *= effect;
    }


    public void UpdateAbilities()
    {
        switch (function)
        {
            case Function.PlayerSpeedUp:
                abilityName = "Player SpeedUp";
                effect = 1;
                effectSign = "+";
                isPercentageBased = false;
                break;

            case Function.ProjectileSpeedUp:
                abilityName = "Projectile SpeedUp";
                effect = 1.25f;
                effectSign = "*";
                isPercentageBased = true;
                break;

            case Function.ProjectileDamageUp:
                abilityName = "Projectile DamageUp";
                effect = 1.2f;
                effectSign = "*";
                isPercentageBased = true;
                break;

            case Function.PlayerHealthUp:
                abilityName = "Player HealthUp";
                effect = 30;
                effectSign = "+";
                isPercentageBased = false;
                break;

            case Function.ShotSpeedUp:
                abilityName = "Shot SpeedUp";
                effect = 1.1f;
                effectSign = "/";
                isPercentageBased = true;
                break;

            case Function.ProjectileSpeedReduction:
                abilityName = "Projectile SpeedReductionDown";
                effect = 1.3f;
                effectSign = "/";
                isPercentageBased = true;
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
        if (waveMan.selectedAbilities.Contains(function.ToString()))
        {
            waveMan.selectedAbilities.Remove(function.ToString());
        }
        
    }
}
