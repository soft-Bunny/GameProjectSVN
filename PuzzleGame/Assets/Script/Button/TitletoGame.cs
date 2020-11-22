using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitletoGame : MonoBehaviour {

    //private GUIStyle guiStyle = new GUIStyle();

    public AudioClip clickSound; // Audioclip이라는 데이터타입에 변수 생성
    AudioSource myAudio; // 컴퍼넌트에서 AudioSource가져오기

    // Use this for initialization
    void Start () {
        myAudio = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            ClickSound();
          
        }

    }

    void ClickSound()
    {
        myAudio.PlayOneShot(clickSound);
    }

    public void SceneChange()
    {
        Invoke("RealSceneChange", 1.9f);
    }

    public void RealSceneChange()
    {
        SceneManager.LoadScene("GameScene");

    }


}
