using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverride : MonoBehaviour
{

	public bool isDialogueFromSouth;
	public bool isDialogueFromWest;

	public bool inUIOverride;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		DialogueManager.Instance.isDialogueFromSouth = isDialogueFromSouth;
		DialogueManager.Instance.isDialogueFromWest = isDialogueFromWest;
		PlayerController.Instance.InUiOverride = true;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		DialogueManager.Instance.isDialogueFromSouth = true;
		DialogueManager.Instance.isDialogueFromWest = true;
		PlayerController.Instance.InUiOverride = false;
	}

}
