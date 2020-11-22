using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{

    public GameObject Button1;

    public AudioClip clickSound; // Audioclip이라는 데이터타입에 변수 생성
    AudioSource myAudio; // 컴퍼넌트에서 AudioSource가져오기

    // Use this for initialization
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnclickRetry()
    {
        ClickSound();
        Button1.transform.localScale = Vector2.one * 1.1f;
        Invoke("SceneChange", 0.5f);

    }

    void ClickSound()
    {
        myAudio.PlayOneShot(clickSound);
    }

    public void SceneChange()
    {
        Application.Quit();
    }

}
