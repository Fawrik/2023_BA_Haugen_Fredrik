using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileCollectionManager : MonoBehaviour
{
	public static ProfileCollectionManager Instance;

	public List<TempProfiles> tempProfiles = new List<TempProfiles>();

	string profilePath = "Profiles";

	//public void Awake()
	//{
	//	Instance = Instance ?? this;

	//	SetUpProfiles();
	//}

	//public void SetUpProfiles()
	//{
	//	var resProfiles = Resources.LoadAll<DialogueProfile>(profilePath);

	//	for (int i = 0; i < resProfiles.Length; i++)
	//	{
	//		tempProfiles.Add(new TempProfiles());

	//		tempProfiles[i].ActorId = resProfiles[i].actorID;
	//		tempProfiles[i].Title = resProfiles[i].title;
	//	}
	//}
}

[System.Serializable]
public class TempProfiles
{
	public string ActorId;
	public string Title;
}
