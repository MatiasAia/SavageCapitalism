using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class CharacterControls : MonoBehaviour {

	public Material[] materials;
	public float speed = 10.0f;
	public float airVelocity = 8f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public float jumpHeight = 2.0f;
	public float maxFallSpeed = 20.0f;
	public float rotateSpeed = 25f; //Speed the player rotate
	private Vector3 moveDir;
	private Vector2 movementInput;
	public GameObject cam;
	private Rigidbody rb;

	private float distToGround;

	private bool canMove = true; //If player is not hitted
	private bool isStuned = false;
	private bool wasStuned = false; //If player was stunned before get stunned another time
	private float pushForce;
	private Vector3 pushDir;

	public bool[] checkPoints = new bool[16];
	private bool slide = false;

	public int money = 1000;

	void  Start (){
		// get the distance to ground
		distToGround = GetComponent<Collider>().bounds.extents.y;
	}
	
	bool IsGrounded (){
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}
	
	void Awake () {
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		rb.useGravity = false;
		GetComponent<MeshRenderer>().material = materials[GetComponent<PlayerInput>().user.id];
		//checkPoint = transform.position;
	}

	

	void FixedUpdate () {
		if (canMove)
		{
			if (moveDir.x != 0 || moveDir.z != 0)
			{
				Vector3 targetDir = moveDir; //Direction of the character

				targetDir.y = 0;
				if (targetDir == Vector3.zero)
					targetDir = transform.forward;
				Quaternion tr = Quaternion.LookRotation(targetDir); //Rotation of the character to where it moves
				Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed); //Rotate the character little by little
				transform.rotation = targetRotation;
			}

			if (IsGrounded())
			{
			 // Calculate how fast we should be moving
				Vector3 targetVelocity = moveDir;
				targetVelocity *= speed;

				// Apply a force that attempts to reach our target velocity
				Vector3 velocity = rb.velocity;
				if (targetVelocity.magnitude < velocity.magnitude) //If I'm slowing down the character
				{
					targetVelocity = velocity;
					rb.velocity /= 1.1f;
				}
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;
				if (!slide)
				{
					if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
						rb.AddForce(velocityChange, ForceMode.VelocityChange);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
					//Debug.Log(rb.velocity.magnitude);
				}

				// Jump
				if (IsGrounded() && Input.GetButton("Jump"))
				{
					rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
				}
			}
			else
			{
				if (!slide)
				{
					Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity, rb.velocity.y, moveDir.z * airVelocity);
					Vector3 velocity = rb.velocity;
					Vector3 velocityChange = (targetVelocity - velocity);
					velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
					velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
					rb.AddForce(velocityChange, ForceMode.VelocityChange);
					if (velocity.y < -maxFallSpeed)
						rb.velocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
				}
			}
		}
		else
		{
			rb.velocity = pushDir * pushForce;
		}
		// We apply gravity manually for more tuning control
		rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
	}

	private void Update()
	{
		moveDir = new Vector3(movementInput.x, 0, movementInput.y);

		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
		{
			if (hit.transform.tag == "Slide")
			{
				slide = true;
			}
			else
			{
				slide = false;
			}
		}
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	public void HitPlayer(Vector3 velocityF, float time)
	{
		rb.velocity = velocityF;

		pushForce = velocityF.magnitude;
		pushDir = Vector3.Normalize(velocityF);
		StartCoroutine(Decrease(velocityF.magnitude, time));
	}

	public void LoadCheckPoint()
	{
		Respawn(true);
	}

	public void Respawn(bool restMoney)
	{
		Vector3 positionToRespawn = new Vector3();
		//CarManager isOwner = new CarManager();
		for (int i = 0; i < checkPoints.Length; i++)
		{
			Debug.Log(i + " " + checkPoints[i]);
			if (!checkPoints[i])
				if (i == 0)
				{
					positionToRespawn = CheckPointManager.control.GetCheckpointPosition(checkPoints.Length - 1);
					break;
				}
				else
				{
					positionToRespawn = CheckPointManager.control.GetCheckpointPosition(i - 1);
					break;
				}
		}

		if (restMoney)
		{
			//if (isOwner)
			//{
			//	isOwner.money += 200;
			//}

			money -= 200;
		}

		//carController.StopCar();
		Debug.Log(positionToRespawn);
		transform.position = positionToRespawn + new Vector3(0, 1, 0);
	}

	public void ReceiveID(int ID)
	{
		//Debug.Log("ID: " + ID);
		if (ID == 0)
		{
			checkPoints[ID] = true;
			return;
		}
		Debug.Log(checkPoints[ID - 1]);
		checkPoints[ID] = checkPoints[ID - 1];
		CheckFinishLap();

	}

	public void CheckFinishLap()
	{
		bool fullLap = true;

		for (int i = 0; i < checkPoints.Length; i++)
		{
			fullLap = fullLap && checkPoints[i];
		}

		if (fullLap)
		{
			money += CheckPointManager.control.CarCompleteLap();
			for (int i = 0; i < checkPoints.Length; i++)
			{
				checkPoints[i] = false;
			}
		}
	}

	private IEnumerator Decrease(float value, float duration)
	{
		if (isStuned)
			wasStuned = true;
		isStuned = true;
		canMove = false;

		float delta = 0;
		delta = value / duration;

		for (float t = 0; t < duration; t += Time.deltaTime)
		{
			yield return null;
			if (!slide) //Reduce the force if the ground isnt slide
			{
				pushForce = pushForce - Time.deltaTime * delta;
				pushForce = pushForce < 0 ? 0 : pushForce;
				//Debug.Log(pushForce);
			}
			rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0)); //Add gravity
		}

		if (wasStuned)
		{
			wasStuned = false;
		}
		else
		{
			isStuned = false;
			canMove = true;
		}
	}

	public void OnMove(InputAction.CallbackContext ctx)
	{
		movementInput = ctx.ReadValue<Vector2>();
	}
}
