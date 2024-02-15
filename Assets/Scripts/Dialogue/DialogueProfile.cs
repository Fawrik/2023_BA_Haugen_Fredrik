using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Profile", menuName = "Dialogue/Profile")]
public class DialogueProfile : ScriptableObject
{
	public string actorID;

	public string title;

	public AudioClip beep;

	public bool variedPitch = true;

}
