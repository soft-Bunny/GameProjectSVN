using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redeal : MonoBehaviour {

    public GameObject RedealButton;

    public AudioClip clickSound; 
    AudioSource myAudio;



    void Start () {
        myAudio = GetComponent<AudioSource>();
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnclickRetry()
    {
        ClickSound();
  
        RedealButton.transform.localScale = Vector2.one * 1.1f;
        Invoke("IwannaSleep", 0.3f);

    }

    void ClickSound()
    {
        myAudio.PlayOneShot(clickSound);
    }

    void ClickEffect()
    {
    }

    void IwannaSleep()
    {
        RedealButton.transform.localScale = Vector2.one * 1.0f;
    }
}
