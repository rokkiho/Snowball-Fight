using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class TPSCharacter : MonoBehaviour {

	[SerializeField]
	float movementSpeed = 6;
	[SerializeField]
	float jumpSpeed = 8;

	CharacterController controller;

	float gravity = Physics.gravity.y;

	Vector3 moveDirection = Vector3.zero;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(controller.isGrounded) {
			moveDirection = new Vector3(Input.GetAxis ("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= movementSpeed;

			if(Input.GetButton ("Jump")) {
				moveDirection.y = jumpSpeed;
			}
		}

		// Apply gravity
		moveDirection.y += gravity * Time.deltaTime;

		// Move the controller
		controller.Move (moveDirection * Time.deltaTime);
	}
}
