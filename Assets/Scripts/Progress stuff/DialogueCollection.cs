using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCollection : MonoBehaviour
{
	public static DialogueCollection Instance;


	public List<DialogueEvent> dialogueCollection = new List<DialogueEvent>();

	public List<DialogueProfile> profiles = new List<DialogueProfile>();


	string dialogueAllDataPath = "Dialogue/";
	string dialougeFinalBuildDataPath = "Dialogue/";

	string profilesAllDataPath;
	string profilesFinalBuildDataPath;




	private void Awake()
	{
		if (Instance == null)
			Instance = this;

		SetUpDialogue();
	}


	public void SetUpDialogue()
	{
		var dials = Resources.LoadAll<DialogueEvent>(dialogueAllDataPath);

		foreach (var item in dials)
		{
			dialogueCollection.Add(item);
		}
	}

}
