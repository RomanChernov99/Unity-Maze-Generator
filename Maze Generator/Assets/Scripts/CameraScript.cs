using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	private float ZoomSensibility = 10f;
	private float MoveSensibility = 3f;

	private float minFov = 5f;
	private float maxFov = 90f;

    void Update()
    {
		//Get our mouse Input
		Vector3 mouseMovement = new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y"));

		//Get our camera Field of View
		float fov = Camera.main.fieldOfView;

		//Set this vector 3 to be equal tou our camera position
		Vector3 cameraPos = transform.position;

		//Set our Field of View regarding our Mouse Scrollwheel Input
		fov -= Input.GetAxis("Mouse ScrollWheel") * ZoomSensibility;

		//Restrict our Field of view between a minimum of 15 and a maximum of 90
		fov = Mathf.Clamp(fov, minFov, maxFov);

		//Set our Camera Field of View to be equal to our fov variable
		Camera.main.fieldOfView = fov;

		//if we are pressing the mouse wheel down, we can move our camera according to our mouse movement
		if (Input.GetMouseButton(2))
		{
			cameraPos -= mouseMovement * MoveSensibility;
			transform.position = cameraPos;
		}
    }
}
