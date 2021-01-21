using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
	public GameObject explosion;

	private float time;


	private void Update()
	{
		//if our bullet doesn't hit anything we want to destroy it after some time has passed
		time += 0.01f;

		if (time >= 5f)
		{
			Destroy(gameObject);
		}
	}

	//When the bullet collides
	private void OnCollisionEnter(Collision collision)
	{
		//if it collides with an enemy
		if (collision.gameObject.tag == "Enemy")
		{
			//instantiate explosion effect
			Instantiate(explosion, collision.gameObject.transform.position, Quaternion.identity);

			//destroy the enemy
			Destroy(collision.gameObject);

			//destroy the bullet
			Destroy(gameObject);
		}

		//if it collides with the walls
		if (collision.gameObject.tag == "Walls")
		{
			//instantiate explosion effect
			Instantiate(explosion, collision.gameObject.transform.position, Quaternion.identity);

			//destroy the bullet
			Destroy(gameObject);
		}
	}
}
