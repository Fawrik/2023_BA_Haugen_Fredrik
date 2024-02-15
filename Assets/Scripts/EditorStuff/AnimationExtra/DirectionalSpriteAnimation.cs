using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(menuName = "Sprite Based Animation/Directional Animation", fileName = "New Directional Animation")]
public class DirectionalSpriteAnimation : ScriptableObject
{
	public string tag;
	public SpriteAnimation South;
	public SpriteAnimation SouthWest;
	public SpriteAnimation West;
	public SpriteAnimation NorthWest;
	public SpriteAnimation North;
	public SpriteAnimation NorthEast;
	public SpriteAnimation East;
	public SpriteAnimation SouthEast;
}

