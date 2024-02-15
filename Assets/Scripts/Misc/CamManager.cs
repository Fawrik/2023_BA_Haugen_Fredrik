using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{
	public Transform target;
	public Vector3 offset;

	public Transform fixedCamArea;

	public bool inFixedCamArea;



    void Start()
    {
	
	}

    

	private void LateUpdate()
	{
		if (!inFixedCamArea)
		{
			transform.position = target.position + offset;
		}
		else
		{
			transform.position = fixedCamArea.position;
		}
		
	}


}


public enum CameraState { followPlayer, LockInPosition, LockAtPoint }