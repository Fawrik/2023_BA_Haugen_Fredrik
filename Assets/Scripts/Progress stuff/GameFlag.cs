using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameFlag 
{
	public string flagID;
	public int flagValue;

	public static bool operator == (GameFlag left, GameFlag right)
	{
		return left.flagID == right.flagID && left.flagValue == right.flagValue;
	}

	public static bool operator !=(GameFlag left, GameFlag right)
	{
		return !(left == right);
	}

	public override bool Equals(object obj)
	{
		if (obj is GameFlag i )
		{
			return i == this;
		}

		return false;
	}

	public override int GetHashCode()
	{
		return flagID.GetHashCode() ^ flagValue.GetHashCode();
	}

}


[CreateAssetMenu(menuName = "Other/GameFlag Collection", fileName = "New GameFlag List")]
public class GameFlagList: ScriptableObject
{
	public List<GameFlag> gameFlags = new List<GameFlag>();
}
