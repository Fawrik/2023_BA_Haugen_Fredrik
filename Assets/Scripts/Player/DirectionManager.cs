using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionManager : MonoBehaviour
{
	private SpriteAnimator anim;
	private SpriteRenderer rend;
	public PlayerController player;

	private Vector2 positionLastFrame;

	[Header("Directions")]
	public Direction facingDirection;

	[Header("Animations")]
	public bool EnableAnimator = true;
	public DirectionalSpriteAnimation Animation;

    void Start()
    {
		anim = GetComponent<SpriteAnimator>();
		rend = GetComponent<SpriteRenderer>();
		player = GetComponent<PlayerController>();

		positionLastFrame = transform.position;
	}

	
    void Update()
    {
		if (player.isMoving)
		{
			if (player.movement.y < 0 && player.movement.x == 0)
				facingDirection = Direction.South;

			else if (player.movement.x < 0 && player.movement.y == 0)
				facingDirection = Direction.West;

			else if (player.movement.x > 0 && player.movement.y == 0)
				facingDirection = Direction.East;

			else if (player.movement.y > 0 && player.movement.x == 0)
				facingDirection = Direction.North;

			else if (player.movement.y < 0 && player.movement.x < 0)
				facingDirection = Direction.SouthWest;

			else if (player.movement.x < 0 && player.movement.y > 0)
				facingDirection = Direction.NorthWest;

			else if (player.movement.y > 0 && player.movement.x > 0)
				facingDirection = Direction.NorthEast;

			else if (player.movement.y < 0 && player.movement.x > 0)
				facingDirection = Direction.SouthEast;
		}
		

		if (EnableAnimator && Animation != null)
		{
			switch (facingDirection)
			{
				case Direction.South:
					anim.Animation = Animation.South;
					player.dirVector = Vector2.down;
					break;
				case Direction.East:
					anim.Animation = Animation.East ? Animation.East : Animation.South;
					player.dirVector = Vector2.right;
					break;
				case Direction.West:
					anim.Animation = Animation.West ? Animation.West : Animation.South;
					player.dirVector = Vector2.left;
					break;
				case Direction.North:
					anim.Animation = Animation.North ? Animation.North : Animation.South;
					player.dirVector = Vector2.up;
					break;
				case Direction.SouthWest:
					anim.Animation = Animation.SouthWest ? Animation.SouthWest : Animation.South;
					player.dirVector = new Vector2(-1, -1);
					break;
				case Direction.NorthWest:
					anim.Animation = Animation.NorthWest ? Animation.NorthWest : Animation.South;
					player.dirVector = new Vector2(-1, 1);
					break;
				case Direction.NorthEast:
					anim.Animation = Animation.NorthEast ? Animation.NorthEast : Animation.South;
					player.dirVector = new Vector2(1, 1);
					break;
				case Direction.SouthEast:
					anim.Animation = Animation.SouthEast ? Animation.SouthEast : Animation.South;
					player.dirVector = new Vector2(1, -1);
					break;
				default:
					break;
			}
		}
		positionLastFrame = transform.position;
	}
}

public enum Direction { South, SouthWest, West, NorthWest, North, NorthEast, East, SouthEast}