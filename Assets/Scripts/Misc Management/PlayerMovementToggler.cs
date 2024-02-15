using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementToggler : MonoBehaviour
{
	public static PlayerMovementToggler Instance;
	public Animator letterBoxAnim;
	PlayerController playerController;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;

		playerController = FindObjectOfType<PlayerController>();
	}

	public void TogglePlayerMovement(bool canMove)
	{
		letterBoxAnim.SetBool("canMove", canMove);
		playerController.canMove = canMove;
	}

}
