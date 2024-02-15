using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Dialogue Event", menuName = "Dialogue/Event")]
public class DialogueEvent : ScriptableObject
{
	#region Dialogue event specific classes
	[System.Serializable]
	public class DialogueChunk
	{
		public DialogueProfile profile;     //Sepparate calss that contains all personal info for the dialogue speaker. e.g. Name of speaker, voice of speaker, object of speaker etc.

		[TextArea(4, 8)]
		public string dialText;

		public bool updateBehaviour = false;
		public bool otherUpdates = false;

		[Header("Behaviour")]
		[ShowIf("updateBehaviour", true)]
		public string actorMood = "";

		[ShowIf("updateBehaviour", true)]
		public ActorRotation actorRotation;

		[ShowIf("actorRotation", ActorRotation.RotateDirection)]
		[Indent] public Direction rotationDirection;

		[ShowIf("actorRotation", ActorRotation.RotateTowards)]
		[Indent] public string actorToLookAt;

		[ShowIf("actorRotation", ActorRotation.RotateAway)]
		[Indent] public string actorToAvoid;

		[ShowIf("updateBehaviour", true)]
		public ActorMovement actorMovement;

		[ShowIf("actorMovement", ActorMovement.MoveByDirstance)]
		[Indent] public MoveDirstance moveDirstance;

		[ShowIf("actorMovement", ActorMovement.MoveBetweenDirstances)]
		[Indent] public List<MoveDirstance> moveDirstances = new List<MoveDirstance>();

		[ShowIf("actorMovement", ActorMovement.MoveToPoint)]
		[Indent] public string movePointID;

		[ShowIf("actorMovement", ActorMovement.MoveBetweenPoints)]
		[Indent] public List<string> movePointIDs = new List<string>();

		[ShowIf("updateBehaviour", true)]
		public BeepPitch beepPitch = BeepPitch.Normal;

		[Header("Other")]
		[ShowIf("otherUpdates", true)]
		public bool canPlayerMove = true;

		[ShowIf("otherUpdates", true)]
		public bool canTextBeSkipped = true;

		[ShowIf("otherUpdates", true)]
		public float continueTime;

		[ShowIf("otherUpdates", true)]
		public CameraState cameraState;

		[ShowIf("otherUpdates", true)]
		public GameFlag[] gameFlagsToUpdate;

		[ShowIf("otherUpdates", true)]
		public ProfileUpdate[] dialogueProfilesToUpdate;

		[ShowIf("otherUpdates", true)]
		public bool resetEastRoomFlags = false;

		[ShowIf("otherUpdates", true)]
		public bool quitGame = false;

		public bool newProfileTitle;
		public bool switchActor = false;
	}


	#endregion

	/// <summary>
	/// Dialogue Event
	/// </summary>

	[Header("Identification")]
	public string actorID;
	public string subID;
	[TextArea(3, 6)]
	public string description;
	public List<GameFlag> gameFlags = new List<GameFlag>();

	[Header("Inser dialogue information here")]
	public DialogueChunk[] dialogueChunks;

	[Header("Inser dialogue information here")]
	public List<DialogueResponse> dialogueResponses = new List<DialogueResponse>();
	//public DialogueResponse[] dialogueResponses;

	public void UpdateFlags()
	{

	}

	public void UpdateProfile()
	{

	}

}




[System.Serializable]
public class DialogueResponse
{
	public int index;

	[TextArea(4, 8)]
	public string responseText;

	public DialogueEvent newDialogue;
}

[System.Serializable]
public class ProfileUpdate
{
	public string profileID;
	public string newTitle;
}