using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class EventChecker : MonoBehaviour
{
	[SerializeField]
	Player player;
	Text text;

	void Start ()
	{
		text = GetComponent<Text> ();
		player.Stand += playerStand;
		player.Move += playerMove;
		player.Crouch += playerCrouch;
		player.Jump += playerJump;
		player.Hurt += playerHurt;
		player.Death += playerDeath;
	}

	private void playerDeath ()
	{
		text.text = " Death ";
	}

	private void playerHurt ()
	{
		text.text = " Hurt ";
	}

	private void playerJump ()
	{
		Debug.Log ("Jump");
		text.text = " Jump ";
	}

	private void playerCrouch ()
	{
		Debug.Log ("Crouch");
		text.text = " Crouch ";
	}

	private void playerMove ()
	{
		text.text = " Move ";
	}

	private void playerStand ()
	{
		text.text = " Stand ";
	}
}
