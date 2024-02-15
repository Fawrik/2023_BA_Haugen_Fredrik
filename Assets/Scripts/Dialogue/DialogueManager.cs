using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager Instance;
	private void Awake()
	{
		if (Instance == null)
			Instance = this;


		player = GameObject.FindGameObjectWithTag("Player");
		nativeSizeHihglightBox = responseHighlightBox.GetComponent<RectTransform>();
		highlightPlacementRect = highlightPlacementSource.GetComponent<RectTransform>();
		highlightPlacementRectOriginPosition = highlightPlacementRect.anchoredPosition;
	}

	//Special characters in strings that affects how dialogue is read.
	private readonly List<char> punctuationCharacters = new List<char>
	{ '.', '!', '?' };
	private readonly List<char> vowels = new List<char>
	{ 'A', 'E', 'I', 'O', 'U', 'a', 'e', 'i', 'o', 'u' };

	int displayTextLength;

	[Header("General")]
	public bool inDialogueEvent = false;
	public float defaultDelay;
	float delay;
	public bool isCurrentlyTyping;
	public bool isCurrentlyAnimatedSpeaking;
	public bool inResponse = false;
	public Color defaultColorA;
	public Color defaultColorB;
	public GameObject DialogueColorA;
	public GameObject DialogueColorB;
	public GameObject ProfileColorB;
	public GameObject ProfileColorA;
	public AudioClip defaultActorBeep;
	private AudioClip actorBeep;
	public AudioClip defaultTextBeep;
	public AudioClip responseHoverBeep;
	public AudioClip responseSelectBeep;
	public AudioClip nextDialogue;
	public AudioClip switchActor;
	public AudioClip endDialogue;
	Vector2 defaultPitchRange = new Vector2(0.9f, 1.1f);
	Vector2 highPitchRange = new Vector2(0.9f, 1.3f);
	Vector2 lowPitchRange = new Vector2(0.8f, 0.6f);
	Vector2 veryLowPitchRange = new Vector2(0.6f, 0.4f);
	Vector2 pitchRange;
	public ResponseSelect responseSelect = ResponseSelect.None;
	bool chunkIsSilence;

	private GameObject player;
	DialogueEvent.DialogueChunk chunk;
	public TextMeshProUGUI dialogueTitle;
	public TextMeshProUGUI dialogueText;
	public Queue<DialogueEvent.DialogueChunk> dialogueChunks = new Queue<DialogueEvent.DialogueChunk>();
	public ActorBehaviourManager speakingActor;
	public ActorID currentActorID;
	public bool inTag;
	public bool stopTriggerBuffer;
	//Some other stuff
	private string completeText;
	public DialogueEvent dialogueEvent;

	[Header("Response")]
	#region ResponseJunk
	public GameObject responseText;
	public Transform responseTextSource;
	public GameObject responseHighlightBox;
	public GameObject responseHighlight;
	public List<GameObject> responseHighlights = new List<GameObject>();
	public Transform highlightPlacementSource;

	SpriteAnimation selectedResponseUpper;
	SpriteAnimation selectedResponseMiddle;
	SpriteAnimation selectedResponseDowner;
	public List<TextMeshProUGUI> responseTexts = new List<TextMeshProUGUI>();
	public List<DialogueResponse> shownResponses = new List<DialogueResponse>();
	public List<DialogueResponse> allResponses = new List<DialogueResponse>();
	public int currentResponseIndex;
	public DialogueResponse currentSelectedResponse;
	public DialogueResponse currentSelectedResponsePlacement;

	RectTransform highlightPlacementRect;
	Vector2 highlightPlacementRectOriginPosition;


	float highlightLength;
	RectTransform highlightBoxRect;
	RectTransform nativeSizeHihglightBox;
	//private float nativeSizeHihglightBox;
	#endregion

	[Header("Animation")]
	#region Animation Junk
	public Animator textBoxAnim;
	public Animator profileBoxAnim;
	public AnimationHolder responseBoxAnimHolder;
	public SpriteAnimator responseBoxSpriteAnim;
	public Animator responseBoxAnim;
	public Animator continueBopAnim;
	public Animator responseArrowsAnim;
	public Animator[] reponseHighlightsAnim;

	public bool isDialogueOpen = false;
	public bool isDialogueFromSouth = true;
	public bool isDialogueFromWest = true;
	#endregion


	
	

	public void Update()
	{
		textBoxAnim.SetBool("FromSouth", isDialogueFromSouth);
		responseBoxAnim.SetBool("fromSouth", isDialogueFromSouth);
		responseBoxAnim.SetBool("fromWest", isDialogueFromWest);

		if (inResponse)
		{
			Responding();
		}

		ConstantActorBehaviourUpdates();


		//StuckInAnElevatorFromSpace();
	}

	public IEnumerator EnqueueDialogue(DialogueEvent db)
	{
		player.GetComponent<PlayerController>().canMove = false;
		Instance.inDialogueEvent = true;
		dialogueEvent = db;
		dialogueChunks.Clear();
		textBoxAnim.SetBool("IsOpen", true);
		inDialogueEvent = true;

		

		foreach (DialogueEvent.DialogueChunk chunk in db.dialogueChunks)
		{
			dialogueChunks.Enqueue(chunk);
		}
		dialogueText.maxVisibleCharacters = 0;

		yield return new WaitForSeconds(0.2f);

		DequeueDialogue();
	}

	public void DequeueDialogue()
	{
		if (inDialogueEvent)
		{
			if (isCurrentlyTyping && !chunk.canTextBeSkipped)
			{
				return;
			}

			if (isCurrentlyTyping)
			{
				StopAllCoroutines();
				CompleteText();
				StartCoroutine(SpeakDelayShort());
				isCurrentlyTyping = false;
				return;
			}	

			if (dialogueChunks.Count == 0)
			{
				if (dialogueEvent.dialogueResponses.Count > 0)
				{
					SetUpResponse();
				}
				else
				{

					EndOfDialogue();
				}

				return;
			}


            if (chunk != null && !isCurrentlyTyping)
            {
                if (chunk.switchActor)
                {
                    AudioManager.Instance.PlayClip(switchActor);
                }
                else
                {
                    if (!inResponse && dialogueChunks.Peek() != chunk)
                    {
						AudioManager.Instance.PlayClip(nextDialogue);
					}
                }
            }


            chunk = dialogueChunks.Dequeue();

			GetTextDisplayLength();

			UpdateSpeakingActor();
			UpdateBoxColors(currentActorID);
			SingleActorBehaviourUpdates();
			UpdateFlags();
			//UpdateProfileTitles();
			FlagManager.Instance.ResetWestRoomFlags(chunk.resetEastRoomFlags);

			dialogueTitle.text = chunk.profile?.title;
			UpdateProfileBox();

			dialogueText.maxVisibleCharacters = 0;
			completeText = chunk.dialText;
			dialogueText.text = chunk.dialText;
			CheckIfChunkIsSilence();

			StartCoroutine(DisplayText(chunk));
			StartCoroutine(SpeakDelayLong());
		}


	}

	public enum TextAnimationEffect { None, Wave, Shake}

	public void GetTextDisplayLength()
	{
		bool isCounting = true;
		foreach (char c in chunk.dialText.ToCharArray())
		{
			if (c == '<')
				isCounting = false;

			if (isCounting)
				displayTextLength++;

			if (c == '>')
				isCounting = true;
		}
	}

	public void UpdateBoxColors(ActorID actorID)
    {
        if (actorID.isCharacter)
        {
			DialogueColorA.GetComponent<Image>().color = actorID.colorA;
			DialogueColorB.GetComponent<Image>().color = actorID.colorB;
			ProfileColorB.GetComponent<Image>().color = actorID.colorB;
			ProfileColorA.GetComponent<Image>().color = actorID.colorA;
		}
        else
        {
			ResetBoxColors();
        }
		
	}
	public void ResetBoxColors()
    {
		DialogueColorA.GetComponent<Image>().color = defaultColorA;
		DialogueColorB.GetComponent<Image>().color = defaultColorB;
		ProfileColorB.GetComponent<Image>().color = defaultColorB;
		ProfileColorA.GetComponent<Image>().color = defaultColorA;
	}

	IEnumerator DisplayText(DialogueEvent.DialogueChunk chunk)
	{
		continueBopAnim.SetInteger("State", 0);
		
		delay = defaultDelay;

        if (isDialogueOpen)
        {
			yield return new WaitForSeconds(.25f);
        }

		char[] fullTextArray = chunk.dialText.ToCharArray();


		
		isCurrentlyTyping = true;



		for (int i = 0; i < fullTextArray.Length; i++)
		{

			char c = fullTextArray[i];
			char nextC = 'c';

			if (i < fullTextArray.Length - 1)
			{
				nextC = fullTextArray[i + 1];
			}


			if (c == '<')
			{
				inTag = true;

				switch (nextC)
				{
					//case 'b': delay = defaultDelay; pitchRange = defaultPitchRange; break;
					case 'a': delay = 0.05f; pitchRange = lowPitchRange; break;
					case 'k': delay = 0.05f; pitchRange = veryLowPitchRange; break;
					case 'l': delay = 0.075f; break;
					case 'b': delay = 0.6f; pitchRange = lowPitchRange; break;
				}

			}


			if (dialogueText.maxVisibleCharacters < displayTextLength)
			{
				dialogueText.maxVisibleCharacters++;
				if (CheckVowels(c) && !CheckVowels(nextC))
				{
					PlayTextBeep();
				}
			}


			yield return new WaitForSeconds(delay);

			
			

			if (CheckPunctuation(c))
			{
				yield return new WaitForSeconds(0.2f);
			}
			else if (c == ',')
			{
				yield return new WaitForSeconds(0.1f);
			}

			if (c == '>')
			{
				inTag = false;
			}


		}


		yield return null;
		isCurrentlyTyping = false;
		continueBopAnim.SetInteger("State", 1);
		isDialogueOpen = true;
	}

	public void CheckIfChunkIsSilence()
	{
		int counter = 0;
		foreach (var c in dialogueText.text)
		{
			if (CheckPunctuation(c) || c == ' ')
				counter++;	
		}

		if (counter == dialogueText.text.Length)
			chunkIsSilence = true;
		else
			chunkIsSilence = false;
	}

	private void CompleteText()
	{
		//dialogueText.text = completeText;
		dialogueText.maxVisibleCharacters = displayTextLength;
		continueBopAnim.SetInteger("State", 1);
	}

	public virtual void EndOfDialogue()
	{
		dialogueEvent = null;
		Instance.inDialogueEvent = false;
		isCurrentlyAnimatedSpeaking = false;
		profileBoxAnim.SetInteger("ProfileVal", 0);
		textBoxAnim.SetBool("IsOpen", false);
		StartCoroutine(DialougeStopTriggerBuffer());
		EndGameCondition(chunk.quitGame);
		player.GetComponent<PlayerController>().canMove = true;
		AudioManager.Instance.PlayClip(endDialogue);
		continueBopAnim.SetInteger("State", 0);
		isDialogueOpen = false;
		ResetBoxColors();
	}

	private bool CheckPunctuation(char c)
	{
		if (punctuationCharacters.Contains(c))
		{
			return true;
		}
		else
			return false;
	}

	private bool CheckVowels(char c)
	{
		if (vowels.Contains(c))
		{
			return true;
		}
		else
			return false;
	}

	public void UpdateSpeakingActor()
	{
		
		if (speakingActor != null)
		{
			isCurrentlyAnimatedSpeaking = false;
			speakingActor.isTalking = false;
		}
		if (chunk.profile == null)
		{
			speakingActor = null;
			return;
		}
			
		ActorID[] actorCollection = ActorCollectionManager.Instance.actors;

		foreach (var actor in actorCollection)
		{
			if (actor.actorID == chunk.profile.actorID)
			{
				speakingActor = actor.GetComponent<ActorBehaviourManager>();
				currentActorID = actor.GetComponent<ActorID>();
			}
		}


		//GameObject g = FindObjectOfType<ActorID>().actorID 
	}

	public void PlayTextBeep()
	{
		if (chunk.profile == null)
		{
			AudioManager.Instance.audioSource.pitch = 1;
			AudioManager.Instance.PlayClip(defaultTextBeep);
		}
		else if(chunk.profile.beep == null)
		{
			actorBeep = defaultActorBeep;		
			AudioManager.Instance.audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
			
			AudioManager.Instance.PlayClip(actorBeep);
		}
		else
		{
			actorBeep = chunk.profile.beep;
            if (chunk.profile.variedPitch)
				AudioManager.Instance.audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
			else
				AudioManager.Instance.audioSource.pitch = 1;
			AudioManager.Instance.PlayClip(actorBeep);
		}
	}

	public void UpdateActorRotation(ActorRotation rot)
	{
		switch (rot)
		{
			case ActorRotation.Default:
				break;
			case ActorRotation.RotateDirection:
				speakingActor.RotateDirection(chunk.rotationDirection);
				break;
			case ActorRotation.RotateTowards:
				speakingActor.RotateTowardsActor(chunk.actorToLookAt);
				break;
			case ActorRotation.RotateAway:
				speakingActor.RotateAwayFromActor(chunk.actorToLookAt);
				break;
			default:
				break;
		}
	}

	

	public void UpdateBeepPitchRange(BeepPitch beepPitch)
	{
		switch (beepPitch)
		{
			case BeepPitch.Normal: pitchRange = defaultPitchRange; break;
			case BeepPitch.High: pitchRange = highPitchRange; break;
			case BeepPitch.Low: pitchRange = lowPitchRange; break;
			case BeepPitch.VeryLow: pitchRange = veryLowPitchRange; break;
		}
	}


	public void UpdateActorMood(string mood)
	{
		speakingActor.ChangeMood(mood);
	}

	public void  UpdateProfileBox()
	{
		if (dialogueTitle.text == null)
		{
			profileBoxAnim.SetInteger("ProfileVal", 0);
				return;
		}

		if (chunk.newProfileTitle)
			profileBoxAnim.SetTrigger("NewTitle");

		if (dialogueTitle.text.ToString() == "null")
			profileBoxAnim.SetInteger("ProfileVal", 0);

		else if (dialogueTitle.text.ToString() == "")
			profileBoxAnim.SetInteger("ProfileVal", 1);

		else
			profileBoxAnim.SetInteger("ProfileVal", 2);

	}


	public void SetUpResponse()
	{
		AudioManager.Instance.PlayClip(switchActor);

		int responseAmount = dialogueEvent.dialogueResponses.Count;
		foreach (var item in dialogueEvent.dialogueResponses)
		{
			allResponses.Add(item);
		}

		
		highlightBoxRect = nativeSizeHihglightBox;
		highlightPlacementRect.anchoredPosition = highlightPlacementRectOriginPosition;

		//Use to adjust highlights placement based on length between first and last highlight.
		Vector2 highlightPlacement = Vector2.zero;
		Vector2 highlightplacementOffset = new Vector2(0, -5); //The spacing between each new highlight object instantiated.
		Vector2 highlightBoxOddOffset = new Vector2(highlightPlacementRectOriginPosition.x, highlightPlacementRectOriginPosition.y - 0.5f); //The box's position if there is an odd amount of highlight objects.
		Vector2 highlightBoxEvenOffset = new Vector2(highlightPlacementRectOriginPosition.x, highlightPlacementRectOriginPosition.y); //The box's position if there is an even amount of highlight objects.

		float responseTextYOffset = 0;
		float responseTextInitialYpos = 0;
		

		if (responseAmount == 1)
		{
			selectedResponseUpper = responseBoxAnimHolder.otherAnimation[3].spriteAnimation;
			responseTextYOffset = 0;
			responseTextInitialYpos = 14;
		}
		else if (responseAmount == 2)
		{
			selectedResponseUpper = responseBoxAnimHolder.otherAnimation[2].spriteAnimation;
			selectedResponseDowner = responseBoxAnimHolder.otherAnimation[4].spriteAnimation;

			responseTextYOffset = 36;
			responseTextInitialYpos = 31;
		}
		else if (responseAmount > 2)
		{
			selectedResponseUpper = responseBoxAnimHolder.otherAnimation[1].spriteAnimation;
			selectedResponseMiddle = responseBoxAnimHolder.otherAnimation[3].spriteAnimation;
			selectedResponseDowner = responseBoxAnimHolder.otherAnimation[5].spriteAnimation;

			responseTextYOffset = 28; 
			responseTextInitialYpos = 41;
		}

		Vector2 responseTextPlacement = new Vector2(responseTextSource.GetComponent<RectTransform>().anchoredPosition.x +45, responseTextInitialYpos);

		for (int i = 0; i < responseAmount; i++)
		{
			if (i < 3)
			{
				//Response text stuff
				GameObject responseTextClone = Instantiate(responseText, responseTextPlacement, Quaternion.identity);
				responseTextPlacement += new Vector2(0, -responseTextYOffset);

				responseTextClone.transform.SetParent(responseTextSource, false);
				responseTexts.Add(responseTextClone.GetComponent<TextMeshProUGUI>());
				responseTexts[i].text = allResponses[i].responseText;

				shownResponses.Add(allResponses[i]);
			}

			//Highlight stuff
			GameObject highlightClone = Instantiate(responseHighlight, highlightPlacement, Quaternion.identity); //Instantiate highlight objects.
			highlightPlacement += highlightplacementOffset; // Add offset for next highlightObject.

			highlightClone.transform.SetParent(highlightPlacementSource, false); // Set parent to hihglightObject so it will be visible on canvas.
			responseHighlights.Add(highlightClone); // Add highlight to list of highlights.
		}

		
		//Use position of last item in highlight list to determine length from item 0 to item last.
		highlightLength = Mathf.Abs(responseHighlights[responseHighlights.Count - 1].GetComponent<RectTransform>().anchoredPosition.y); 

		//Adjust position of points by setting y-pos of hihglight parent to half of highlightLength. Scale highlightBox by adding scale.y + hihgLightlength
		highlightPlacementRect.anchoredPosition = new Vector2(highlightPlacementRect.anchoredPosition.x, highlightPlacementRect.anchoredPosition.y +Mathf.Floor(highlightLength/2));
		highlightBoxRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, highlightBoxRect.sizeDelta.y + highlightLength);

		//Determines y-pos of highlightBox based on the length being an even or odd number.
		highlightBoxRect.anchoredPosition = highlightLength % 2 == 0 ? highlightBoxEvenOffset : highlightBoxOddOffset;


		OpenResponseBox();
		
	}

	public void OpenResponseBox()
	{
		continueBopAnim.SetInteger("State", 2);
		responseSelect = ResponseSelect.None;
		currentSelectedResponse = null;
		responseBoxSpriteAnim.Animation = responseBoxAnimHolder.otherAnimation[0].spriteAnimation;
		ResponseBoxSpriteAnimationChange(responseBoxAnimHolder.otherAnimation[0].spriteAnimation);
		currentResponseIndex = 0;

		PlayerMovementToggler.Instance.TogglePlayerMovement(false);
		dialogueText.color = Color.grey;
		StartCoroutine("FinishOpeningResponseBox");
	}

	IEnumerator FinishOpeningResponseBox()
	{
		responseBoxAnim.SetBool("isActive", true);
		responseArrowsAnim.SetFloat("PointVal", 1);
		yield return new WaitUntil(() => responseBoxAnim.transform.GetChild(0).gameObject.activeSelf == true);
		yield return new WaitForEndOfFrame();
		inResponse = true;
		ChangeResponseSelection(0);

		if (dialogueTitle.text != null)
			profileBoxAnim.SetInteger("ProfileVal", 1);
		
	}

	IEnumerator DialougeStopTriggerBuffer()
	{
		stopTriggerBuffer = true;

		yield return new WaitForSeconds(.2f);

		stopTriggerBuffer = false;
	}

	public void Responding()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			ChangeResponseSelection(1);
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			ChangeResponseSelection(-1);
		}
		else if (Input.GetKeyDown(KeyCode.Z) && responseSelect != ResponseSelect.None)
		{
			SelectResponse();
		}

	}

	public void ChangeResponseSelection(int increment)
	{
		//The selection is on top / bottom of the list. Selection therefore stays the same.
		if (currentSelectedResponse == allResponses[0] && increment == -1 ||
				currentSelectedResponse == allResponses[allResponses.Count - 1] && increment == 1)
		{
		
			return;
		}
		//The selection is at the top/bottom of the shown selections while there are more responses beyond. The list of shown responses needs to update.
		else if (increment == -1 && currentSelectedResponse == shownResponses[0] || increment == 1 && currentSelectedResponse == shownResponses[shownResponses.Count - 1])
		{
		
			for (int i = 0; i < shownResponses.Count; i++)
			{
				shownResponses[i] = allResponses[shownResponses[i].index + increment];
				responseTexts[i].text = shownResponses[i].responseText;
			}

			currentResponseIndex += increment;
			currentSelectedResponse = allResponses[currentResponseIndex];
		}
		//The current selection is updated without the need to update shown responses
		else
		{

			currentResponseIndex = increment == 1 && responseSelect == ResponseSelect.None ? 0 : currentResponseIndex + increment;

			currentSelectedResponse = allResponses[currentResponseIndex];

			if (responseSelect != ResponseSelect.None)
			{
				if (currentSelectedResponse == shownResponses[0])
				{
					responseSelect = ResponseSelect.Upper;
				}
				else if (currentSelectedResponse == shownResponses[shownResponses.Count - 1])
				{
					responseSelect = ResponseSelect.Downer;
				}
				else
				{
					responseSelect = ResponseSelect.Middle;
				}
			}
			else if (responseSelect == ResponseSelect.None)
			{
				responseSelect = increment == 1 ? ResponseSelect.Upper : ResponseSelect.None;

			}
			

			switch (responseSelect)
			{
				case ResponseSelect.None:
					responseBoxSpriteAnim.Animation = responseBoxAnimHolder.otherAnimation[0].spriteAnimation;
					ResponseBoxSpriteAnimationChange(responseBoxAnimHolder.otherAnimation[0].spriteAnimation);
					
					responseArrowsAnim.SetFloat("PointVal", 1);
					break;
				case ResponseSelect.Upper:
					responseBoxSpriteAnim.Animation = selectedResponseUpper;
					ResponseBoxSpriteAnimationChange(selectedResponseUpper);
					
					break;
				case ResponseSelect.Middle:
					responseBoxSpriteAnim.Animation = selectedResponseMiddle;
					ResponseBoxSpriteAnimationChange(selectedResponseMiddle);

					break;
				case ResponseSelect.Downer:
					responseBoxSpriteAnim.Animation = selectedResponseDowner;
					ResponseBoxSpriteAnimationChange(selectedResponseDowner);
					break;
				case ResponseSelect.NoPointers:
					responseArrowsAnim.SetFloat("PointVal", 1);
					break;
				default:
					break;
			}

			for (int i = 0; i < responseTexts.Count; i++)
			{
				if (responseTexts[i].text == currentSelectedResponse.responseText)
				{
					responseTexts[i].color = Color.white;
				}
				else
				{
					responseTexts[i].color = Color.grey;
				}
			}

		}

		if (responseSelect != ResponseSelect.None)
		{
			AudioManager.Instance.PlayClip(responseHoverBeep);
			if (currentSelectedResponse == allResponses[0])
			{

				if (allResponses.Count > 1)
					responseArrowsAnim.SetFloat("PointVal", 2);
				else
					responseArrowsAnim.SetFloat("PointVal", 0);

			}
			else if (currentSelectedResponse == allResponses[allResponses.Count - 1])
			{
				responseArrowsAnim.SetFloat("PointVal", 4);
			}
			else
			{
				responseArrowsAnim.SetFloat("PointVal", 3);
			}

			for (int i = 0; i < allResponses.Count; i++)
			{
				if (i == currentResponseIndex)
				{
					responseHighlights[i].GetComponent<Animator>().SetBool("isActive", true);
				}
				else
				{
					responseHighlights[i].GetComponent<Animator>().SetBool("isActive", false);
				}
			}
		}
		else if (responseSelect == ResponseSelect.None)
		{
			foreach (var item in responseTexts)
			{
				item.color = Color.grey;
			}
			currentSelectedResponse = null;
		}



		//print("OK end");
	}

	public void SelectResponse()
	{
		inResponse = false;
		PlayerMovementToggler.Instance.TogglePlayerMovement(true);

		responseTexts.ForEach(item => Destroy(item.gameObject));
		responseHighlights.ForEach(item => Destroy(item.gameObject));

		//foreach (var item in responseTexts)
		//{
		//	Destroy(item.gameObject);
		//}
		//foreach (var item in responseHighlights)
		//{
		//	Destroy(item.gameObject);
		//}

		allResponses.Clear();
		shownResponses.Clear();
		responseTexts.Clear();
		responseHighlights.Clear();

		highlightBoxRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, highlightBoxRect.sizeDelta.y - highlightLength);
		responseBoxAnim.SetBool("isActive", false);

		Instance.inDialogueEvent = false;
		dialogueEvent = null;
		AudioManager.Instance.PlayClip(responseSelectBeep);

		StartCoroutine("WaitAframeForNewDialogue");
		StartCoroutine(DialougeStopTriggerBuffer());
		//dialogueText.text = "";

	}

	IEnumerator WaitAframeForNewDialogue()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		StartCoroutine(EnqueueDialogue(currentSelectedResponse.newDialogue));
		//EnqueueDialogue(currentSelectedResponse.newDialogue);
		dialogueText.color = Color.white;
	}

	public void UpdateFlags()
	{
		foreach (var dialogueFlag in chunk.gameFlagsToUpdate)
		{
			foreach (var managerFlag in FlagManager.Instance.flags)
			{
				if (dialogueFlag.flagID == managerFlag.flagID)
				{
					managerFlag.flagValue = dialogueFlag.flagValue;
				}
			}
		}
	}

	//public void UpdateProfileTitles()
	//{
	//	foreach (var profileTitle in chunk.dialogueProfilesToUpdate)
	//	{
	//		foreach (var profile in ProfileCollectionManager.Instance.tempProfiles)
	//		{
	//			//if (profileTitle.profileID == profile.actorID)
	//			//{
	//			//	profile.title = profileTitle.newTitle;
	//			//}
	//		}
	//	}
	//}


	public void ConstantActorBehaviourUpdates()
	{
		if (speakingActor!= null)
		{
			speakingActor.isTalking = isCurrentlyAnimatedSpeaking;
		}

	
	}

	public void SingleActorBehaviourUpdates()
	{

		if (speakingActor != null)
		{
			UpdateActorRotation(chunk.actorRotation);
			UpdateActorMood(chunk.actorMood);
			UpdateBeepPitchRange(chunk.beepPitch);

			

			
		}

	}

	public void EndGameCondition(bool endGame)
	{
		if (endGame)
		{
			Application.Quit();
		}
	}


	//Note to self, unity animator sin exit time / transition duration e faktisk useful sometimes.

	IEnumerator SpeakDelayLong()
	{
		isCurrentlyAnimatedSpeaking = !chunkIsSilence;
		yield return new WaitForSeconds(2);
		yield return new WaitUntil(() => isCurrentlyTyping == false);
		isCurrentlyAnimatedSpeaking = false;
		yield return null;
	}

	IEnumerator SpeakDelayShort()
	{
		isCurrentlyAnimatedSpeaking = !chunkIsSilence;
		yield return new WaitForSeconds(1);
		yield return new WaitUntil(() => isCurrentlyTyping == false);
		isCurrentlyAnimatedSpeaking = false;
	}



	public void StuckInAnElevatorFromSpace()
	{
		if (dialogueTitle.text.ToString() == "null")
		{
			profileBoxAnim.SetInteger("ProfileVal", 0);
			print("Null");
		}
		else if (dialogueTitle.text.ToString() == "")
		{
			profileBoxAnim.SetInteger("ProfileVal", 1);
			print("hidden");
		}
		else 
		{
			profileBoxAnim.SetInteger("ProfileVal", 2);
			print("visible");
		}

	}

	
	public void ResponseBoxSpriteAnimationChange(SpriteAnimation animation)
	{
		responseBoxSpriteAnim.gameObject.GetComponent<Image>().sprite = animation.Frames[0].Sprite;
	}

	
}

public enum ResponseSelect { None, Upper, Middle, Downer, NoPointers}