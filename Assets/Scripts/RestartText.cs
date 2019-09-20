using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartText : MonoBehaviour {

    GameManager gm;
    public Text restartText;
    public Text restartScoreText;

    // Use this for initialization
    void Start () {
        gm = GameManager.Instance;
        restartText = GetComponent<Text>();
        restartText.enabled = false;
        restartScoreText.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (gm.player != null && gm.player.health <= 0)
        {
            restartScoreText.text = "You made it to " + gm.level + " - " + gm.room;
            restartScoreText.enabled = true;
            restartText.enabled = true; 
        }
	}
}
