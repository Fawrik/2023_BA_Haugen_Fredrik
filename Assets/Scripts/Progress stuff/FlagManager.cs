using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FlagManager : MonoBehaviour
{
	public static FlagManager Instance;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;

	}

	public List<GameFlag> flags;



	public void SetUpFlags()
	{
		//GameFlagList gameFlags = Resources.Load<GameFlag>()
	}

	public void UpdateFlag(string ID, int value) // value as either incremental value or override value? hmmmmm
	{

	}

	public void ResetAllFLags()
	{
		foreach (var flag in flags)
		{
			flag.flagValue = 0;
		}
	}

	public void ResetWestRoomFlags(bool reset)
	{
		if (reset == true)
		{
			flags[0].flagValue = 0;
			flags[1].flagValue = 0;
			flags[2].flagValue = 0;
		}
		
	}
}
