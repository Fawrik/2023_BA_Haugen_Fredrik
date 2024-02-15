using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorCollectionManager : MonoBehaviour
{
	public static ActorCollectionManager Instance;

	public ActorID[] actors;

	public void Awake()
	{
		Instance = Instance ?? this;

		actors = FindObjectsOfType<ActorID>();

	}
}

