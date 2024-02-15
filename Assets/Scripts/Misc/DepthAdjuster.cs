using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthAdjuster : MonoBehaviour
{
	SpriteRenderer rend;

	private void Awake()
	{
		rend = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		rend.sortingOrder = (int)Camera.main.WorldToScreenPoint(transform.position).y * -1;

	}
}
