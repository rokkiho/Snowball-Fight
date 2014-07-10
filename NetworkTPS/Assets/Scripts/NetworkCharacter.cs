using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;

[RequireComponent(typeof(PhotonView), typeof(CharacterController))]
public class NetworkCharacter : Photon.MonoBehaviour {
	// Player properties
	string playerName;
	Transform head;
	[SerializeField]
	float maxHealth = 100;
	float currHealth;

	// Player movement
	[SerializeField]
	float movementSpeed = 6;
	[SerializeField]
	float jumpSpeed = 8;
	CharacterController controller;
	float gravity = Physics.gravity.y;
	Vector3 moveDirection = Vector3.zero;

	// Network properties
	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	Quaternion realHeadRotation = Quaternion.identity;

	// Use this for initialization
	void Start () {
		head = transform.FindChild ("Head");
		controller = GetComponent<CharacterController> ();
		currHealth = maxHealth;

		realPosition = transform.position;
		realRotation = transform.rotation;
		realHeadRotation = head.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<PhotonView>().isMine) {
			HandleMovement();
			HPHUD.UpdateHealthInfo (currHealth, maxHealth);
		}
		else {
			transform.Find("Name").GetComponent<TextMesh>().text = playerName;
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.2f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.2f);
			head.rotation = Quaternion.Lerp(head.rotation, realHeadRotation, 0.2f);
		}
	}

	void HandleMovement() {
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

	public void SetName(string name) {
		playerName = name;
	}

	public float GetCurrHealth() {
		return currHealth;
	}
	
	public float GetMaxHealth() {
		return maxHealth;
	}
	
	public void ApplyDamage(float amount) {
		currHealth -= amount;
		
		if(currHealth <= 0) {
			currHealth = 0;
			Die();
		}
	}
	
	void Die() {
		if(photonView.instantiationId == 0)
			Destroy (gameObject);
		else if(PhotonNetwork.isMasterClient)
			PhotonNetwork.Destroy(gameObject);
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if(head == null) 
			return;

		if(stream.isWriting) {
			// This is OURS

			stream.SendNext(playerName);
			stream.SendNext(currHealth);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(head.rotation);
		}
		else {
			// This is others'
			
			playerName = (string)stream.ReceiveNext();
			currHealth = (float)stream.ReceiveNext();
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
			realHeadRotation = (Quaternion)stream.ReceiveNext();
		}
	}
}
