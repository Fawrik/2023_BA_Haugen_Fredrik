using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOverride : MonoBehaviour
{

	public CamManager cam;

	private void Awake()
	{
		cam = FindObjectOfType<CamManager>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		cam.inFixedCamArea = true;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		cam.inFixedCamArea = false;
	}
}
