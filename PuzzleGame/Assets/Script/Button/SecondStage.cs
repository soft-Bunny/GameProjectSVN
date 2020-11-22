using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecondStage : MonoBehaviour
{ 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
  

    }

    public void SceneChange()
    {
        Invoke("SecondSceneChange", 1.9f);
    }

    public void SecondSceneChange()
    {
        SceneManager.LoadScene("GameScene_02");
    }
}
