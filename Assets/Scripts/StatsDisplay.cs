using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    public GridObject host;
    public Text expText;

    // Use this for initialization
    void Start()
    {
        expText = GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        host = GameManager.Instance.player;
        if (host == null)
        {
            expText.text = "Exp: " + "no host";
        }
        else
        {
            expText.text = "Attack: " + host.attack;
        }
    }
}
