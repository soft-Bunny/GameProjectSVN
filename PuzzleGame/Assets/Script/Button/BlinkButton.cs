using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkButton : MonoBehaviour {

    public GameObject StartButton;

    public static BlinkButton instance;

    void Awake()
    {
        if (BlinkButton.instance == null)
            BlinkButton.instance = this;
    }

    // Use this for initialization
    void Start () {
        StartButton.SetActive(true);
        StartCoroutine(ShowReady());
	}

    IEnumerator ShowReady()
    {
       while(true)
        {
            StartButton.SetActive(true);
            yield return new WaitForSeconds(1.2f);
            StartButton.SetActive(false);
            yield return new WaitForSeconds(1.2f);
        }
    }
	
	// Update is called once per frame
	void Update () {
 
    }
}
