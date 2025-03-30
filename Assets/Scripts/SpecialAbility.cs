using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAbility : MonoBehaviour
{
    public GameObject panel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePanel()
    {
        panel.SetActive(true);
    }

    public void DisablePanel()
    {
        panel.SetActive(false);
    }
}
