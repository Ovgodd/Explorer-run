using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
	// Config
	[SerializeField] float runSpeed = 5f;
	[SerializeField] float jumpSpeed = 5f;
	[SerializeField] float climbSpeed = 5f;
	[SerializeField] float slideSpeed = 5f;
	[SerializeField] Vector2 deathKick = new Vector2(2f, 2f);
	//[SerializeField] float slideSpeed = 5f;
	// State
	private bool slide = true;

	// Cached component references
	Rigidbody2D myRigidBody;
	Animator myAnimator;  
	CapsuleCollider2D myBodyCollider2D;
	BoxCollider2D myFeet;
	float gravityScaleAtStart;
	bool isAlive = true;

	// Message then methods
	void Start()
	{
		myRigidBody = GetComponent<Rigidbody2D>();
		myAnimator = GetComponent<Animator>();
		myBodyCollider2D = GetComponent<CapsuleCollider2D>();
		myFeet = GetComponent<BoxCollider2D>();
		gravityScaleAtStart = myRigidBody.gravityScale;
		
	}

	// Update is called once per frame
	void Update()
	{
		if (!isAlive) { return; }
		Run();
		ClimbLadder();
		FlipSprite();
		Jump();
		Die();


		//Slide();

}

	private void Run()
	{

		{
			float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is betweeen -1 to +1
			Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
			myRigidBody.velocity = playerVelocity;
			

			bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
			myAnimator.SetBool("Running", playerHasHorizontalSpeed);
			Debug.Log("run");
		}
		

	}
	private void ClimbLadder()
	{

		
		if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing"))) 
		{
			myAnimator.SetBool("Climbing", false);
			myRigidBody.gravityScale = gravityScaleAtStart;
			return; 
		}
		
		float controlClimb = CrossPlatformInputManager.GetAxis("Vertical");
		Vector2 playerVelocityClimb = new Vector2(myRigidBody.velocity.x, controlClimb * climbSpeed);
		myRigidBody.velocity = playerVelocityClimb;
		myRigidBody.gravityScale = 0f;


		bool playerHasVertivalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
		myAnimator.SetBool("Climbing", playerHasVertivalSpeed);
		if (CrossPlatformInputManager.GetButtonDown("Jump"))
		{
			Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
			myRigidBody.velocity += jumpVelocityToAdd;
		}
	}

	private void Jump()
	{

		if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
		{
			myAnimator.SetBool("Jumping",false);
		}
		if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
		{
			return;
		}


		if (CrossPlatformInputManager.GetButtonDown("Jump"))
		{
			myAnimator.SetBool("Jumping",true);
			Debug.Log("Jump");
			Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
			myRigidBody.velocity += jumpVelocityToAdd;
		}
		
	}
	private void Die()
	{
		if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Trap")))

		{
			isAlive = false;
			myAnimator.SetTrigger("Dying");
		}
	}


	 /*private void Slide()
	{
		if (CrossPlatformInputManager.GetButtonDown("Slide"))
		{

			if (slide && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Explorer_Slide")) 
				
			{
				Vector2 slideVelocityToAdd = new Vector2(runSpeed * slideSpeed * 5f, 0f);
				myRigidBody.velocity += slideVelocityToAdd;
				myAnimator.SetBool("Sliding", true);
			}
			else if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sliding"))
			{
				myAnimator.SetBool("Sliding", false);
			}
		}
	}*/

	private void FlipSprite()
	{
		bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
		if (playerHasHorizontalSpeed)
		{
			transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);

		}
	}

}