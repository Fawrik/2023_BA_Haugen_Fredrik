using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TogglePixelGrid : MonoBehaviour
{
	public GameObject PixelGrid;
	public GameObject PinkGrid;


    void Start()
    {
		PixelGrid.GetComponent<Image>().enabled = false;
		PinkGrid.GetComponent<SpriteRenderer>().enabled = false;
	}

    void Update()
    {
		ToggleGrids();
    }


	void ToggleGrids()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			PixelGrid.GetComponent<Image>().enabled = !PixelGrid.GetComponent<Image>().enabled;
			PinkGrid.GetComponent<SpriteRenderer>().enabled = !PinkGrid.GetComponent<SpriteRenderer>().enabled;
		}

		//if (Input.GetKeyDown(KeyCode.Tab))
		//{
		//	DialogueManager.Instance.isDialogueFromSouth = !DialogueManager.Instance.isDialogueFromSouth;
		//}


	}
}
