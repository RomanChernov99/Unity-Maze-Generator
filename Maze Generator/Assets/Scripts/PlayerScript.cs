using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerScript : MonoBehaviour
{
	public GameObject Trail;

	public GameObject bullet;
	private float bulletSpeed = 100f;

	private float Speed = 15f;
	private NavMeshAgent navAgent;
	private CharacterController characterController;

	public bool isDead = false;

	private void Start()
	{
		//Instantiate our trial renderer as a child of our player
		GameObject trail = Instantiate(Trail, transform.position, Quaternion.identity, transform);

		//Get our nav mesh agent
		navAgent = GetComponent<NavMeshAgent>();

		//Get our character controller component
		characterController = GetComponent<CharacterController>();
	}
	void Update()
    {
		//Get our Player Input
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		//Turn the Input into a Vector3
		Vector3 direction = new Vector3(horizontal, 0f, vertical);

		//If we do want to move
		if (direction.magnitude > 0.1f)
		{
			//We stop the NavMeshAgent
			navAgent.isStopped = true;

			//And move our character according to our Input
			characterController.Move(direction * Speed * Time.deltaTime);
		}


		//When we press the left mouse button
		if (Input.GetMouseButtonDown(0))
		{
			//return a ray going from our camera to our mouse position
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			//If we hit something then we set our Nav Mesh Agent's destination to be the point where we hit something
			if (Physics.Raycast(ray, out hit))
			{
				navAgent.isStopped = false;
				navAgent.SetDestination(hit.point);
			}
		}

		//If we press the right mouse button
		if (Input.GetMouseButtonDown(1))
		{
			//Instantiate a bullet at our player's position
			Instantiate(bullet, transform.position, Quaternion.identity);

			//get the bullet's rigid body component
			Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();

			//shoot the bullet in the direction that we are moving
			bulletRB.velocity = direction * bulletSpeed;
		}
    }

	//this function is called when an enemy collides with our player
	public void PlayerIsDead()
	{
		//player dies
		isDead = true;
		Debug.Log("You have died, try again");
	}


}
