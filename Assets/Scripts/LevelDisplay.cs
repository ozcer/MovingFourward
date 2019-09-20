using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    public Text levelText;
    GameManager gm;
    // Use this for initialization
    void Start()
    {
        gm = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        int level = gm.level;
        int room = gm.room;

        levelText.text = level +" - "+room;
    }
}
