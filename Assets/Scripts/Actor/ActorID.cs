using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ActorID : MonoBehaviour
{
	public bool isDialougeTrigger = true;
	
	[Header("ID info")]
	public string actorID;

	[ShowIf("isDialougeTrigger", true)]
	public string subID;


	[ShowIf("isDialougeTrigger", true)]
	[Header("Direct info")]
	public bool isDirectDialogue;

	[ShowIf("isDirectDialogue", true)]
	public DialogueEvent directDialogue;

	public Color colorA;
	public Color colorB;

	public bool isCharacter = true;
}
