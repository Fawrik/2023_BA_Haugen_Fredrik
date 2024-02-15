using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourManager : MonoBehaviour
{
	public string actorID;
	public Rotation rot;
	public Movement move;
	public Animation anim;

	public enum Rotation { Normal, TowardsPlayer, Fixed}
	public enum Movement { Still, MoveTo, MoveDirection}
	public enum Animation { Default, Override}

	//public void 

}
