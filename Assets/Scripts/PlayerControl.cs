using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public Player player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (player.health > 0 && player.inControl)
        {
            if (Input.GetKeyDown("w"))
            {
                player.InteractInDirection(Constant.QuadDir.Up);
            }
            else if (Input.GetKeyDown("a"))
            {
                player.InteractInDirection(Constant.QuadDir.Left);
            }
            else if (Input.GetKeyDown("s"))
            {
                player.InteractInDirection(Constant.QuadDir.Down);
            }
            else if (Input.GetKeyDown("d"))
            {
                player.InteractInDirection(Constant.QuadDir.Right);
            }
            else if (Input.GetKeyDown("space"))
            {

            }
        }
        if (Input.GetKeyDown("r"))
        {
            GameManager.Instance.RestartScene();
        }
    }

}
