using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance;

	private DirectionManager _directionManager;

	#region Public Variables
	public float movementSpeed;
	public Transform movePoint;
	public bool isMoving;
	public bool isTalking;
	public bool canMove = true;
	public bool canRotate = true;
	public DialogueEvent QuitDialogue;
	public Direction direction;
	public AnimState animState;
	public Vector2 movement;
	public Vector2 dirVector;
	public bool InUiOverride;
	#endregion

	#region References
	Rigidbody2D rb;
	public AnimationHolder animationHolder;
	public Animator resetText;
	#endregion

	ActorID trigger;
	RaycastHit2D interactionDetect;
	Ray2D interactionSeeker;
	public bool canInteract;
	public int nullDialogue;


	#region Methods
	void Awake()
	{
		Instance = Instance ?? this;
		GetReferencesAwake();
	}

 
    void Update()
    {

		MovementInput();

		Animations();
		LogicUpdates();

		ResetAllDialogue();
		QuitGame();
		ProgressDialogue();
		CheckIfInteractable();
		TriggerDialogue();

	}

	private void FixedUpdate()
	{
		MovementPhysics();
	}
	#endregion


	#region Functions
	void GetReferencesAwake()
	{
		rb = GetComponent<Rigidbody2D>();
		_directionManager = GetComponent<DirectionManager>();
		//animationHolder = GetComponentInChildren<AnimationHolder>();
	}


	void MovementInput()
	{
		if (canMove)
		{
			movement.y = Input.GetAxisRaw("Vertical");
			movement.x = Input.GetAxisRaw("Horizontal");
		}
		else
		{
			movement = Vector2.zero;
		}

		//Color green = Color.green;
		//Debug.DrawRay(transform.position, dirVector, green);
	}

	void MovementPhysics()
	{
        if (canMove)
        {
			rb.MovePosition(rb.position + movement.normalized * movementSpeed * Time.deltaTime);
        }
	}

	void CheckIfInteractable()
	{
		int layerMask = 1 << 8;
		Vector3 raycastOrigin = transform.position + new Vector3(0, -.25f, 0);
		interactionDetect = Physics2D.Raycast(raycastOrigin, dirVector, 1.75f, layerMask);

        if (!DialogueManager.Instance.inDialogueEvent)
        {

            if (interactionDetect.collider != null)
            {
                if (interactionDetect.collider.GetComponent<ActorID>().isDialougeTrigger)
                {
                    trigger = interactionDetect.collider.GetComponent<ActorID>();
                    canInteract = true;
                    Debug.DrawRay(raycastOrigin, dirVector * interactionDetect.distance, Color.yellow);
					//Debug.DrawLine(transform.position, dirVector * interactionDetect.distance * 0.5f, Color.blue);
					DialogueManager.Instance.UpdateBoxColors(trigger);
                }
                else
                {
                    trigger = null;
                    canInteract = false;
                    Debug.DrawRay(raycastOrigin, dirVector * 1.5f, Color.black);
					DialogueManager.Instance.ResetBoxColors();

				}
            }
            else
            {
                trigger = null;
                canInteract = false;
                Debug.DrawRay(raycastOrigin, dirVector * 1.5f, Color.black);
				DialogueManager.Instance.ResetBoxColors();
			}
        }
        else
            canInteract = false;

    }

    void TriggerDialogue()
	{
        if (Input.GetKeyDown(KeyCode.Z) && !DialogueManager.Instance.inDialogueEvent && !DialogueManager.Instance.inResponse
            && !DialogueManager.Instance.stopTriggerBuffer && canInteract)
        {

            //DialogueManager.Instance.isDialogueFromSouth = isDialogueFromSouth;
            //DialogueManager.Instance.isDialogueFromWest = isDialogueFromWest;

            if (!InUiOverride)
            {
				if (_directionManager.facingDirection == Direction.South || _directionManager.facingDirection == Direction.SouthEast
			   || _directionManager.facingDirection == Direction.SouthWest)
				{
					DialogueManager.Instance.isDialogueFromSouth = false;
					//DialogueManager.Instance.isDialogueFromWest = false;
				}
				else
                {
					DialogueManager.Instance.isDialogueFromSouth = true;
					//DialogueManager.Instance.isDialogueFromWest = true;
				}
			}
           

            if (trigger.isDirectDialogue)
                InitiateDialogue.Instance.InitiateDialogueDirectly(trigger.directDialogue);
            else
                InitiateDialogue.Instance.InitiateDialogueByID(trigger.actorID, trigger.subID);
        }
    }

	void ResetAllDialogue()
	{
        if (Input.GetKeyDown(KeyCode.Tab) && !DialogueManager.Instance.inDialogueEvent && !DialogueManager.Instance.inResponse)
        {
            FlagManager.Instance.ResetAllFLags();
            resetText.SetTrigger("Activate");
        }
    }

	void QuitGame()
	{
        if (Input.GetKeyDown(KeyCode.Escape) && !DialogueManager.Instance.inDialogueEvent && !DialogueManager.Instance.inResponse)
        {
            InitiateDialogue.Instance.InitiateDialogueDirectly(QuitDialogue);
        }
    }

	/// <summary>
	/// Everything related to the player's animations.
	/// </summary>

	void Animations()
	{
		AnimationUpdate();
		AnimationLogic();
	}

	public void LogicUpdates()
	{

		isMoving = movement == new Vector2(0, 0) ? isMoving = false : isMoving = true;

		//if (movement != new Vector2(0, 0))
		//	isMoving = true;
		//else
		//	isMoving = false;

	}

	public void ProgressDialogue()
	{
        //if (Input.GetKeyDown(KeyCode.Space) && !DialogueManager.Instance.inDialogueEvent)
        //    DialogueManager.Instance.EnqueueDialogue(testDialogue);

       if (Input.GetKeyDown(KeyCode.Z) && DialogueManager.Instance.inDialogueEvent && DialogueManager.Instance.inResponse == false)
            DialogueManager.Instance.DequeueDialogue();
    }

	public void AnimationUpdate()
	{
		if (!isMoving && !isTalking)
			animState = AnimState.Idle;

		else if (isMoving && !isTalking)
			animState = AnimState.Walking;

		else if (!isMoving && isTalking)
			animState = AnimState.Talking;

		else if (isMoving && isTalking)
			animState = AnimState.WalkAndTalk;
	}

	public void AnimationLogic()
	{
		switch (animState)
		{
			case AnimState.Idle:
				_directionManager.Animation = animationHolder.directionalAnimations[0];
				break;
			case AnimState.Walking:
				_directionManager.Animation = animationHolder.directionalAnimations[1];
				break;
			case AnimState.Talking:
				_directionManager.Animation = animationHolder.directionalAnimations[2];
				break;
			case AnimState.WalkAndTalk:
				_directionManager.Animation = animationHolder.directionalAnimations[3];
				break;
			default:
				break;
		}
	}

	

	#endregion
}



