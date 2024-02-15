using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Sprite Based Animation/Sprite Amimation Holder", fileName = "New Animation Holder")]
public class AnimationHolder : ScriptableObject
{
	//public List<SpriteAnimation> animCollection = new List<SpriteAnimation>();
	[BoxGroup("Basic Info")]
	[LabelWidth(100)]
	public string ActorID;

	[BoxGroup("Basic Info")]
	[LabelWidth(100)]
	[TextArea]
	public string Description;

	public List<DirectionalSpriteAnimation> directionalAnimations = new List<DirectionalSpriteAnimation>();

	//[TableList]
	public List<BehavioralAnimation> behavioralAnimationCollection = new List<BehavioralAnimation>();

	//[TableList]
	public List<animCollection> otherAnimation = new List<animCollection>();

	

	//public void Sprite(string animName, SpriteAnimator anim)
	//{
	//	for (int i = 0; i < animCollection.Count; i++)
	//	{
	//		if (animCollection[i].name == animName)
	//		{
	//			anim.Animation = animCollection[i];
	//		}
	//	}
	//}

}

[System.Serializable]
public class animCollection
{
	public string groupName;
	public SpriteAnimation spriteAnimation;
	//public List<SpriteAnimation> animationCollection = new List<SpriteAnimation>();
}

[System.Serializable]
public class BehavioralAnimation
{
	public string Mood;
	public AnimationAction actions;
	
}

[System.Serializable]
public class AnimationAction
{
	public DirectionalSpriteAnimation idle;

	public DirectionalSpriteAnimation talk;

	public DirectionalSpriteAnimation walk;

	public DirectionalSpriteAnimation walkAndTalk;
}
