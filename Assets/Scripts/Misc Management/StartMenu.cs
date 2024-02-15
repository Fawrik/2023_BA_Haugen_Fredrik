using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

	public AudioSource source;
	public AudioClip click;
	public GameObject audiDestroyPlz;
    
    void Start()
    {
        
    }

   
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Z))
		{

			StartCoroutine("StartGame");
			
		}
    }

	IEnumerator StartGame()
	{
		source.PlayOneShot(click);
		yield return new WaitForSeconds(.02f);
		SceneManager.LoadScene(1);
	}
}
