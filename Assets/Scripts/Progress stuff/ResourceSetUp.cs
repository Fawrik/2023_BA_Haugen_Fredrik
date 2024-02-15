using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSetUp : MonoBehaviour
{

	string flagsDataPath;
	string dialogueAllDataPath;
	string dialougeFinalBuildDataPath;
	string profilesAllDataPath;
	string profilesFinalBuildDataPath;

	public void SetUpFlags()
	{
		GameFlagList gameFlags = Resources.Load<GameFlagList>("");
	}

}
