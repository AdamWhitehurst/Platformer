using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RotationalPlayer : MonoBehaviour
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
	private SpriteRenderer spriteRenderer;
	private float lastTouchRadius;
	private Vector2 lastTouchPos;
	private float touchStartTime;
	private bool jumpingLastFrame;
	private int currentJumpAmount;
	private Quaternion originalRotation;
	private float startAngle;

	public event PlayerEventHandler
		Stand,
		Move,
		Jump,
		Crouch,
		Hurt,
		Death;

	void Awake ()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
	}

	public void Start ()
	{
		originalRotation = this.transform.rotation;

	}
	public void Update ()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch (0);
			switch (touch.phase)
			{

				case (TouchPhase.Began):
					{
#if UNITY_IPHONE || UNITY_ANDROID
						originalRotation = rigidBody.transform.rotation;
						Vector3 screenPos = Camera.main.WorldToScreenPoint (transform.position);
						Vector3 vector = (Vector3)Input.GetTouch (0).position - screenPos;
						startAngle = Mathf.Atan2 (vector.y, vector.x) * Mathf.Rad2Deg;
						lastTouchPos = touch.position;
#else
						// Untested mouse control input, copy/pasted from touch with mouse substituted...
						 originalRotation = this.transform.rotation;
						 Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
						 Vector3 vector = Input.mousePosition - screenPos;
						 startAngle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
#endif
						break;
					}
				case (TouchPhase.Moved):
					{
#if UNITY_IPHONE || UNITY_ANDROID
						// So lost here...
						//Vector3 screenPos = Camera.main.WorldToScreenPoint (transform.position);
						//Vector3 vector = (Vector3)touch.position - screenPos;
						//float angle = Mathf.Atan2 (vector.y, vector.x) * Mathf.Rad2Deg;
						//Quaternion newRotation = Quaternion.AngleAxis (angle - startAngle, this.transform.forward);
						//newRotation.y = 0;
						//newRotation.eulerAngles = new Vector3 (0, 0, newRotation.eulerAngles.z);

						//float angle = Mathf.Atan2 (Vector2.Distance (lastTouchPos, touch.position), Vector2.Distance (transform.position, touch.position)) * Mathf.Rad2Deg;
						//rigidBody.rotation += angle / 10;
						//lastTouchPos = touch.position;
						//rigidBody.AddTorque (Vector2.Distance (lastTouchPos, touch.position), ForceMode2D.Force);
#else
						// Untested mouse control input, copy/pasted from touch with mouse substituted...
						 Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
						 Vector3 vector = Input.mousePosition - screenPos;
						 float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
						 Quaternion newRotation = Quaternion.AngleAxis(angle - startAngle , this.transform.forward);
						 newRotation.y = 0; //see comment from above 
						 newRotation.eulerAngles = new Vector3(0,0,newRotation.eulerAngles.z);
						 this.transform.rotation = originalRotation *  newRotation;   
#endif
						break;
					}
			}
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

