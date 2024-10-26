using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMovement : MonoBehaviour
{
	private Rigidbody rb;

	public Camera playerCamera;

	public float fov = 90f;
	public bool cameraCanMove = true;
	public float mouseSensitivity = 2f;
	public float maxLookAngle = 100f;

	public bool lockCursor = true;

	private float yaw = 0.0f;
	private float pitch = 0.0f;

	public bool playerCanMove = true;
	public float walkSpeed = 5f;
	public float maxVelocityChange = 10f;

	private bool isWalking = false;

	public KeyCode sprintKey = KeyCode.LeftShift;
	public float sprintSpeed = 7f;
	private bool isSprinting = false;

	public KeyCode jumpKey = KeyCode.Space;
	public float jumpPower = 5f;

	private bool isGrounded = false;

	private bool canAttack = true;

	public Animator playerAnim;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();

		playerCamera.fieldOfView = fov;
	}

	void Start()
    {
		if(lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	void Update()
    {
		if(cameraCanMove)
		{
			float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
			float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

			yaw += mouseX;
			pitch -= mouseY;
			pitch = Mathf.Clamp(pitch, -90f, 75f);

			transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
			playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
		}

		if(Input.GetKeyDown(jumpKey) && isGrounded)
		{
			Jump();
		}

		CheckGround();

		if(canAttack && Input.GetMouseButtonDown(0))
		{
			playerAnim.SetBool("LAttacking", true);
			float randAttack = Random.Range(0f, 1f);
			playerAnim.SetFloat("LeftOrRight", randAttack);
			canAttack = false;
		}
		else
		{
			playerAnim.SetBool("LAttacking", false);
			canAttack = true;
		}
	}

	private void FixedUpdate()
	{
		if(playerCanMove)
		{
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

			if(targetVelocity.x != 0 || targetVelocity.z != 0 && isGrounded)
			{
				playerAnim.SetBool("Walking", true);
				isWalking = true;
			}
			else
			{
				playerAnim.SetBool("Walking", false);
				isWalking = false;
			}

			if(Input.GetKey(sprintKey))
			{
				targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

				Vector3 velocity = rb.velocity;
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;

				if(velocityChange.x != 0 || velocityChange.z != 0)
				{
					playerAnim.SetBool("Running", true);
					isSprinting = true;
				}

				rb.AddForce(velocityChange, ForceMode.VelocityChange);
			}
			else
			{
				playerAnim.SetBool("Running", false);
				isSprinting = false;

				targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

				Vector3 velocity = rb.velocity;
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;

				rb.AddForce(velocityChange, ForceMode.VelocityChange);
			}
		}
	}

	private void CheckGround()
	{
		Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
		Vector3 direction = transform.TransformDirection(Vector3.down);
		float distance = .75f;

		if(Physics.Raycast(origin, direction, out RaycastHit hit, distance))
		{
			Debug.DrawRay(origin, direction * distance, Color.red);
			playerAnim.SetBool("Jumping", false);
			isGrounded = true;
			playerCanMove = true;
		}
		else
		{
			playerAnim.SetBool("Jumping", true);
			isGrounded = false;
			playerCanMove = false;
		}
	}

	private void Jump()
	{
		if(isGrounded)
		{
			playerAnim.SetBool("Jumping", true);
			rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
			isGrounded = false;
		}
	}
}
