using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinDisplay : MonoBehaviour
{
    public Player host;
    public Text coinText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        host = GameManager.Instance.player;

        if (host == null)
        {
            coinText.text = "COINS: " + "no host";
        }
        else
        {
            coinText.text = "COINS: " + host.coins;
        }
    }
}
