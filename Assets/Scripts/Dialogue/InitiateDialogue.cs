using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InitiateDialogue : MonoBehaviour
{
	public static InitiateDialogue Instance;
	public FlagManager flagManager;

	private void Awake()
	{
		Instance = Instance ?? this;

		flagManager = FindObjectOfType<FlagManager>();
	}

	public void InitiateDialogueByID(string actorID, string subID)
	{
		// Get dialogueTrigger's two potential IDs + relevant flags. Search through dialogue dump collection.
		//Find Dialogue that matches by all flags, actorID and subID.


		DialogueEvent[] dialoguesIDFiltered = DialogueCollection.Instance.dialogueCollection
			.Where(_dialogue => _dialogue.actorID == actorID && _dialogue.subID == subID)
			.ToArray();

		DialogueEvent dialogueToRun = dialoguesIDFiltered
			.Single(_dialogue => _dialogue.gameFlags
				.All(i => flagManager.flags.Contains(i))
			);

		StartCoroutine(DialogueManager.Instance.EnqueueDialogue(dialogueToRun));
		//DialogueManager.Instance.EnqueueDialogue(dialogueToRun);


		//if (dialoguesIDFiltered.Length == 1)
		//{
		//	DialogueManager.Instance.EnqueueDialogue(dialoguesIDFiltered[0]);
		//}
		//else
		//{
		//	int flagAmount = dialoguesIDFiltered.Length;
		//	int matchingFlags = 0;

		//	if (dialoguesIDFiltered[0].gameFlags[0].flagID == flagManager.flags[0].flagID)
		//	{

		//	}

		//	for (int i = 0; i < dialoguesIDFiltered.Length; i++)
		//	{
		//		for (int ii = 0; ii < dialoguesIDFiltered[i].gameFlags.Count; ii++)
		//		{
		//			for (int iii = 0; iii < flagManager.flags.Count - 1; iii++)
		//			{
		//				if (dialoguesIDFiltered[i].gameFlags[ii] == flagManager.flags[iii])
		//				{
		//					matchingFlags++;

		//					if (matchingFlags == dialoguesIDFiltered.Length)
		//					{
		//						DialogueManager.Instance.EnqueueDialogue(dialoguesIDFiltered[i]);
								
		//					}
		//				}
		//			}
		//		}

			
		//	}

		//	if (matchingFlags != dialoguesIDFiltered.Length)
		//	{
		//		Debug.LogError("Error: ambigious dialogue initiating. Adjust flags/IDs.");
		//	}
		//}
			
			
			//&& _dialogue.gameFlags.ForEach(flag => FlagManager.Instance.flags.ForEach(_flag => flag == _flag)));
			
		
		
		//&& _dialogue.gameFlags.ForEach(flag => FlagManager.Instance.flags.ForEach(_flag => )));
	}

	public void InitiateDialogueDirectly(DialogueEvent dialogue)
	{
		StartCoroutine(DialogueManager.Instance.EnqueueDialogue(dialogue));
		//DialogueManager.Instance.EnqueueDialogue(dialogue);
	}
}
