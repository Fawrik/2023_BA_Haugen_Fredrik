using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBehaviourManager : MonoBehaviour
{
	public bool isTalking;
	public bool isMoving;

	public string actorMood;
	public ActorMovement actorMovement;
	public ActorRotation actorRotation;
	public Direction facingDirection;
	public AnimState actionState;

	public AnimationHolder animationHolder;
	private SpriteAnimator anim;
	private SpriteRenderer rend;

	private Vector2 positionLastFrame;



	[Header("Animations")]
	public bool EnableAnimator = true;
	public DirectionalSpriteAnimation Animation;



	void Start()
    {
		anim = GetComponent<SpriteAnimator>();
		rend = GetComponent<SpriteRenderer>();

		positionLastFrame = transform.position;
	}

    void Update()
    {
		AnimationUpdate();
		AnimationLogic();
	}

	public void AnimationUpdate()
	{
		if (!isMoving && !isTalking)
			actionState = AnimState.Idle;

		else if (isMoving && !isTalking)
			actionState = AnimState.Walking;

		else if (!isMoving && isTalking)
			actionState = AnimState.Talking;

		else if (isMoving && isTalking)
			actionState = AnimState.WalkAndTalk;
	}

	public void AnimationLogic()
	{
		if (actorMood == "" || actorMood == "0")
		{
			switch (actionState)
			{
				case AnimState.Idle:
					Animation = animationHolder.directionalAnimations[0];
					break;
				case AnimState.Talking:
					Animation = animationHolder.directionalAnimations[1];
					break;
				case AnimState.Walking:
					Animation = animationHolder.directionalAnimations[2];
					break;
				case AnimState.WalkAndTalk:
					Animation = animationHolder.directionalAnimations[3];
					break;
				default:
					break;
			}

			
		}
		else
		{
            //animationHolder.

            foreach (var item in animationHolder.behavioralAnimationCollection)
            {
                if (item.Mood == actorMood)
                {
					switch (actionState)
					{
						case AnimState.Idle:
							Animation = item.actions.idle;
							break;
						case AnimState.Talking:
							Animation = item.actions.talk;
							break;
						case AnimState.Walking:
							Animation = item.actions.walk;
							break;
						case AnimState.WalkAndTalk:
							Animation = item.actions.walkAndTalk;
							break;
						default:
							break;
					}
				}
            }

			


			//switch (actorMood)
			//{
			//	case ActorMood.Default:

			//		break;
			//	case ActorMood.Shocked:
			//		anim.Animation = isTalking
			//			? animationHolder.otherAnimation[1].spriteAnimation : animationHolder.otherAnimation[0].spriteAnimation;
			//		break;
			//	case ActorMood.Giggle:
			//		anim.Animation = isTalking
			//			? animationHolder.otherAnimation[3].spriteAnimation : animationHolder.otherAnimation[2].spriteAnimation;
			//		break;
			//	case ActorMood.Sad:
			//		anim.Animation = isTalking
			//			? animationHolder.otherAnimation[5].spriteAnimation : animationHolder.otherAnimation[4].spriteAnimation;
			//		break;
			//	case ActorMood.Angry:
			//		anim.Animation = isTalking
			//			? animationHolder.otherAnimation[7].spriteAnimation : animationHolder.otherAnimation[6].spriteAnimation;
			//		break;
			//	case ActorMood.Annoyed:
			//		anim.Animation = isTalking
			//			? animationHolder.otherAnimation[9].spriteAnimation : animationHolder.otherAnimation[8].spriteAnimation;
			//		break;
			//	case ActorMood.Impish:
			//		anim.Animation = isTalking
			//			? animationHolder.otherAnimation[11].spriteAnimation : animationHolder.otherAnimation[10].spriteAnimation;
			//		break;
			//	case ActorMood.Laugh:
			//		anim.Animation = isTalking
			//			? animationHolder.otherAnimation[13].spriteAnimation : animationHolder.otherAnimation[12].spriteAnimation;
			//		break;
			//	default:
			//		break;
			//}
		}


		if (EnableAnimator && Animation != null)
		{
			switch (facingDirection)
			{
				case Direction.South:
					anim.Animation = Animation.South;
					break;
				case Direction.East:
					anim.Animation = Animation.East ? Animation.East : Animation.SouthEast;
					break;
				case Direction.West:
					anim.Animation = Animation.West ? Animation.West : Animation.SouthEast;
					break;
				case Direction.North:
					anim.Animation = Animation.North ? Animation.North : Animation.SouthEast;
					break;
				case Direction.SouthEast:
					anim.Animation = Animation.SouthEast ? Animation.SouthEast : Animation.SouthEast;
					break;
				case Direction.SouthWest:
					anim.Animation = Animation.SouthWest ? Animation.SouthWest : Animation.SouthEast;
					break;
				case Direction.NorthWest:
					anim.Animation = Animation.NorthWest ? Animation.NorthWest : Animation.SouthEast;
					break;
				case Direction.NorthEast:
					anim.Animation = Animation.NorthEast ? Animation.NorthEast : Animation.SouthEast;
					break;
				default:
					break;
			}
		}

	}

	void FigureThisLater()
	{
		if (transform.position.y < positionLastFrame.y)
			facingDirection = Direction.South;

		else if (transform.position.y > positionLastFrame.y)
			facingDirection = Direction.North;

		else if (transform.position.x > positionLastFrame.x)
			facingDirection = Direction.East;

		else if (transform.position.x < positionLastFrame.x)
			facingDirection = Direction.West;

		if (EnableAnimator && Animation != null)
		{
			switch (facingDirection)
			{
				case Direction.South:
					anim.Animation = Animation.South;
					break;
				case Direction.East:
					anim.Animation = Animation.East ? Animation.East : Animation.South;
					break;
				case Direction.West:
					anim.Animation = Animation.West ? Animation.West : Animation.South;
					break;
				case Direction.North:
					anim.Animation = Animation.North ? Animation.North : Animation.South;
					break;
				default:
					break;
			}
		}
		positionLastFrame = transform.position;
	}

	void UpdateRotation(ActorRotation rot)
	{

	}

	public void RotateTowardsActor(string actorID)
	{
		Vector2 heading = Vector2.zero;
		float distance = 0;
		Vector2 direction = Vector2.zero;

		if (actorID == "")
		{
			heading = PlayerController.Instance.transform.position - transform.position;
		}
		else
		{
			ActorID[] actorCollection = ActorCollectionManager.Instance.actors;

			foreach (var actor in actorCollection)
			{
				if (actor.actorID == actorID)
				{
					heading = actor.transform.position - transform.position;
				}
			}
		}

		distance = heading.magnitude;
		direction = heading / distance;
		
		// what the fuck
		Vector2 roundedDirection = direction;

		//OG
		//Vector2 roundedDirection = direction.Round(1);

		if (Mathf.Abs(roundedDirection.x) > Mathf.Abs(roundedDirection.y))
		{
			if (roundedDirection.x > 0)
			{
				facingDirection = Direction.East;
			}
			else if (roundedDirection.x < 0)
			{
				facingDirection = Direction.West;
			}
		}
		else if (Mathf.Abs(roundedDirection.x) < Mathf.Abs(roundedDirection.y))
		{
			if (roundedDirection.y > 0)
			{
				facingDirection = Direction.NorthEast;
                if (roundedDirection.x  !=  0)
                {
					if (roundedDirection.x > 0)
					{
						facingDirection = Direction.NorthEast;
					}
					else if (roundedDirection.x < 0)
					{
						facingDirection = Direction.NorthWest;
					}
				}
			}
			else if (roundedDirection.y < 0)
			{
				facingDirection = Direction.SouthEast;

				if (roundedDirection.x != 0)
				{
					if (roundedDirection.x > 0)
					{
						facingDirection = Direction.SouthEast;
					}
					else if (roundedDirection.x < 0)
					{
						facingDirection = Direction.SouthWest;
					}
				}
			}
		}
	}

	public void RotateAwayFromActor(string actorID)
	{
		Vector2 heading = Vector2.zero;
		float distance = 0;
		Vector2 direction = Vector2.zero;

		if (actorID == "")
		{
			heading = PlayerController.Instance.transform.position - transform.position;
		}
		else
		{
			ActorID[] actorCollection = ActorCollectionManager.Instance.actors;

			foreach (var actor in actorCollection)
			{
				if (actor.actorID == actorID)
				{
					heading = actor.transform.position - transform.position;
				}
			}
		}

		distance = heading.magnitude;
		direction = heading / distance;

		// what the fuck
		Vector2 roundedDirection = direction;



		if (Mathf.Abs(roundedDirection.x) > Mathf.Abs(roundedDirection.y))
		{
			if (roundedDirection.x > 0)
			{
				facingDirection = Direction.West;
			}
			else if (roundedDirection.x < 0)
			{
				facingDirection = Direction.East;
			}
		}
		else if (Mathf.Abs(roundedDirection.x) < Mathf.Abs(roundedDirection.y))
		{
			if (roundedDirection.y > 0)
			{
				facingDirection = Direction.SouthEast;
				if (roundedDirection.x != 0)
				{
					if (roundedDirection.x > 0)
					{
						facingDirection = Direction.SouthWest;
					}
					else if (roundedDirection.x < 0)
					{
						facingDirection = Direction.SouthEast;
					}
				}
			}
			else if (roundedDirection.y < 0)
			{
				facingDirection = Direction.East;

				if (roundedDirection.x != 0)
				{
					if (roundedDirection.x > 0)
					{
						facingDirection = Direction.West;
					}
					else if (roundedDirection.x < 0)
					{
						facingDirection = Direction.East;
					}
				}
			}
		}
	}
	public void RotateDirection(Direction direction)
	{
		facingDirection = direction;
	}

	void UpdateMovement(ActorMovement move)
	{

	}

	public void ChangeMood(string mood)
	{
		actorMood = mood;
	}

	

	void OverrideAnimation(SpriteAnimation animation)
	{

	}


}


public enum AnimState { Idle, Walking, Talking, WalkAndTalk }
public enum ActorMood { Default, Giggle, Shocked, Sad, Angry, Annoyed, Impish, Laugh }
public enum ActorRotation { Default, RotateDirection, RotateTowards, RotateAway }
public enum ActorMovement { Default, Still, MoveByDirstance, MoveBetweenDirstances, MoveToPoint, MoveBetweenPoints}
public enum BeepPitch { Normal, High, Low, VeryLow}

[System.Serializable]
public class MoveDirstance
{
	public Direction direction;

	public float distance;
}