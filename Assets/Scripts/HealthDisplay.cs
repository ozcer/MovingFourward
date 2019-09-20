using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {
    public GridObject host;
    public Text healthText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        host = GameManager.Instance.player;

        if (host == null)
        {
            healthText.text = "Health: " + "no host";
        }
        else
        {
            healthText.text = "Health: " + host.health+"/"+host.maxHealth;
        }
    }
}
