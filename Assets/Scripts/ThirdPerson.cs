using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPerson : MonoBehaviour
{
	#region Instance Variables
	public Camera playerCamera;
	public CinemachineFreeLook orbitCamera;

	public GameObject model;
	private Rigidbody rb;

	private float walkSpeed = 3f;
	public bool isWalking = false;

	private float sprintSpeed = 8f;

	Vector3 velocityChange;

	private float jumpPower = 8f;
	private bool isGrounded = false;
	private bool jumpLanded = true;
	public bool isJumping = false;

	public AudioSource jumpStart;
	public List<AudioClip> grassJS;
	public AudioSource jumpLand;
	public List<AudioClip> grassJL;
	
	public Animator playerAnim;
	private float smoothBlend = 0.1f;

	private float groundDistance;
	#endregion

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();

		playerCamera.fieldOfView = 90f;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		RotateToCamera();

		CheckGround();

		Jump();

		Attack();
	}

	private void FixedUpdate()
	{
		MovePlayer();
	}

	private void MovePlayer()
	{
		Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		Vector2 moveDirection = new Vector2(targetVelocity.x, targetVelocity.z);

		targetVelocity.Normalize();

		isWalking = targetVelocity.magnitude > 0;
		bool isSprinting = Input.GetKey(KeyCode.LeftShift);

		playerAnim.SetFloat("x", moveDirection.x * (isSprinting ? 1f : 0.5f), smoothBlend, Time.deltaTime);
		playerAnim.SetFloat("y", moveDirection.y * (isSprinting ? 1f : 0.5f), smoothBlend, Time.deltaTime);
		playerAnim.SetBool("Walking", isWalking);

		float moveSpeed = isSprinting ? sprintSpeed : walkSpeed;

		targetVelocity = transform.TransformDirection(targetVelocity) * moveSpeed;

		Vector3 velocity = rb.velocity;
		velocityChange = targetVelocity - velocity;
		velocityChange.y = 0;

		rb.AddForce(velocityChange, ForceMode.VelocityChange);
	}

	private void RotateToCamera()
	{
		bool turnBody = isWalking && !Mathf.Approximately(transform.eulerAngles.y, orbitCamera.m_XAxis.Value);
		rb.freezeRotation = !turnBody;

		if(turnBody)
		{
			Quaternion desiredRotation = Quaternion.Euler(0f, orbitCamera.m_XAxis.Value, 0f);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, 15f);
		}

		transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
		model.transform.localPosition = new Vector3(0f, model.transform.localPosition.y, 0f);
	}

	private void CheckGround()
	{
		Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
		Vector3 direction = transform.TransformDirection(Vector3.down);
		float distance = .70f;

		isGrounded = Physics.Raycast(origin, direction, out RaycastHit hit, distance);

		Ray ground = new Ray(transform.position, -Vector3.up);

		if(Physics.Raycast(ground, out hit))
		{
			if(hit.collider.CompareTag("Ground"))
			{
				groundDistance = hit.distance;
			}
		}

		if(groundDistance > 1.1f && groundDistance < 1.2f && !isJumping)
		{
			Vector3 snapToGround = new Vector3(0f, 0.025f, 0f);
			transform.position -= snapToGround;
		}
	}

	private void Jump()
	{
		playerAnim.SetBool("Jumping", !isGrounded);

		if(groundDistance >= 1.2f)
		{
			jumpLanded = false;
		}

		if(!jumpLanded && isGrounded)
		{
			JumpAudio(jumpLand, grassJS);

			jumpLanded = true;
			isJumping = false;
		}

		if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
		{
			JumpAudio(jumpStart, grassJS);

			rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
			isJumping = true;
		}
	}

	private void JumpAudio(AudioSource source, List<AudioClip> clips)
	{
		AudioClip clip = clips[Random.Range(0, clips.Count)];
		source.clip = clip;

		source.Play();
	}

	private void Attack()
	{
		bool isAttacking = playerAnim.IsInTransition(1) || playerAnim.GetCurrentAnimatorStateInfo(1).IsName("UpperBody.Attack01");

		if(!isAttacking && Input.GetMouseButtonDown(0) && isGrounded)
		{
			playerAnim.SetTrigger("Attack");
		}
	}
}