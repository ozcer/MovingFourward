using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Collectible {

    public Rigidbody rb;

	// Use this for initialization
	protected void Start () {
        base.Start();
        transform.rotation = Quaternion.Euler(90, 45, 0);
        rb.AddForce(0, 200, 0);
        //StartCoroutine(DelayedPickUp());
    }
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
}
    IEnumerator DelayedPickUp()
    {
        yield return new WaitForSeconds(.5f);
        GetPickedUp(GameManager.Instance.player);
    }
    public override void GetPickedUp(Player player)
    {
        player.coins += 1;
        base.GetPickedUp(player);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }
}
