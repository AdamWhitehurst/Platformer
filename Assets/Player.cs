using UnityEngine;
using System.Collections;

public delegate void PlayerEventHandler ();
public class Player : MonoBehaviour
{
	public int playerHealth;
	public int extraJumps = 1;

	public float minSwipeDistY;
	public float minSwipeDistX;
	public float maxSwipeTime;

	public float moveForce = 20f;
	public float swipeForce = 20f;
	public float jumpForce = 200f;

	[Range (0.01f, 1.00f)]
	public float swipePower = 1f;
	public float maxVelocity;
	public float turnAroundLagTime = 0.2f;
	public float bounciness = 2f;

	private Rigidbody2D rigidBody;
	private Animator animator;
	private Vector2 swipeStartPos;
	private float swipeStartTime;
	private float swipeTurnAroundLagTime;
	private bool jumpingLastFrame;
	private int currentJumpAmount;

	public event PlayerEventHandler
		Stand,
		Move,
		Jump,
		Crouch,
		Hurt,
		Death;

	void Awake ()
	{
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
        animator = GetComponent<Animator> ();
	}

	void Update ()
	{
#if UNITY_ANDROID
		if (Input.touchCount > 0)
		{

			Touch touch = Input.touches [0];

			switch (touch.phase)
			{

				case TouchPhase.Began:
					swipeStartPos = touch.position;
					swipeStartTime = Time.timeSinceLevelLoad;
					swipeTurnAroundLagTime = turnAroundLagTime + Time.timeSinceLevelLoad;
					OnMove ();
					break;

				case TouchPhase.Stationary:
				case TouchPhase.Moved:

					float distHorizontal = (new Vector3 (touch.position.x, 0, 0) - new Vector3 (swipeStartPos.x, 0, 0)).magnitude;
					float phaseAmount = Mathf.Sign (touch.position.x - swipeStartPos.x);

					if (distHorizontal > minSwipeDistX)
					{
						if (phaseAmount > 0) //right
						{

							if (Time.timeSinceLevelLoad > swipeTurnAroundLagTime)
							{
								gameObject.transform.rotation = new Quaternion (gameObject.transform.rotation.x, 0f, gameObject.transform.rotation.z, 0f);
							}
							Vector2 vel = rigidBody.velocity;
							vel.x = moveForce * 2f;
							rigidBody.velocity = vel;
						}

						else if (phaseAmount < 0) //left
						{
							if (Time.timeSinceLevelLoad > swipeTurnAroundLagTime)
							{
								gameObject.transform.rotation = new Quaternion (gameObject.transform.rotation.x, 180f, gameObject.transform.rotation.z, 0f);
							}
							Vector2 vel = rigidBody.velocity;
							vel.x = -moveForce * 2f;
							rigidBody.velocity = vel;
						}
					}
					OnMove ();
					break;

				case TouchPhase.Ended:

					float swipeDistVertical = (new Vector3 (0, touch.position.y, 0) - new Vector3 (0, swipeStartPos.y, 0)).magnitude;

					if (swipeDistVertical > minSwipeDistY && Time.timeSinceLevelLoad <= maxSwipeTime + swipeStartTime)

					{

						float swipeValue = Mathf.Sign (touch.position.y - swipeStartPos.y);

						if (swipeValue > 0) //up swipe
						{
							if (currentJumpAmount <= extraJumps)
							{
								rigidBody.AddForce (new Vector2 (0f, jumpForce), ForceMode2D.Impulse);
								currentJumpAmount++;
								OnJump ();
							}
						}

						else if (swipeValue < 0) //Down Swipe
						{
							//Crouch
							OnCrouch ();
						}
					}

					float swipeDistHorizontal = (new Vector3 (touch.position.x, 0, 0) - new Vector3 (swipeStartPos.x, 0, 0)).magnitude;

					if (swipeDistHorizontal > minSwipeDistX && Time.timeSinceLevelLoad <= maxSwipeTime + swipeStartTime)
					{

						float swipeAmount = Mathf.Sign (touch.position.x - swipeStartPos.x);

						if (swipeAmount > 0) //right swipe
						{
							//gameObject.transform.rotation = new Quaternion ( gameObject.transform.rotation.x , 0f , gameObject.transform.rotation.z , 0f );
							rigidBody.AddForce (new Vector2 ((swipeDistHorizontal * swipePower) * swipeForce, 0f));
						}

						else if (swipeAmount < 0) //left swipe
						{
							//gameObject.transform.rotation = new Quaternion ( gameObject.transform.rotation.x , 180f , gameObject.transform.rotation.z , 0f );
							rigidBody.AddForce (new Vector2 ((-swipeDistHorizontal * swipePower) * swipeForce, 0f));
						}
					}
					break;
			}
		}
#endif

		if (rigidBody.velocity.y != 0)
		{
			animator.SetBool ("isJumping", true);
			jumpingLastFrame = true;
		}

		else if (jumpingLastFrame)
		{
			jumpingLastFrame = false;
			currentJumpAmount = 0;
			animator.SetTrigger ("land");
			OnCrouch ();
		}

		if (rigidBody.velocity.x != 0 || rigidBody.velocity.y != 0)
		{
			animator.SetBool ("isMoving", true);
			OnMove ();
		}

		else
		{
			animator.SetBool ("isMoving", false);
			OnStand ();
		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.gameObject.tag == "Hitbox")
		{
			col.gameObject.GetComponentInParent<Rigidbody2D> ().velocity = (-col.gameObject.GetComponentInParent<Rigidbody2D> ().velocity / 3) * bounciness;
			OnHurt ();
		}
	}

	#region Event Methods
	void OnStand ()
	{
		if (Stand != null)
		{
			Stand ();
		}
	}
	void OnMove ()
	{
		if (Move != null)
		{
			Move ();
		}
	}
	void OnJump ()
	{
		if (Jump != null)
		{
			Jump ();
		}
	}
	void OnCrouch ()
	{
		if (Crouch != null)
		{
			Crouch ();
		}
	}
	void OnHurt ()
	{
		playerHealth--;
		if (Hurt != null)
		{
			Hurt ();
		}

		if (playerHealth <= 0)
		{
			OnDeath ();
			gameObject.SetActive (false);
		}
	}
	void OnDeath ()
	{
		if (Death != null)
		{
			Death ();
		}
	}
	#endregion
}

