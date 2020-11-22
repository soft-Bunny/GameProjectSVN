using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    GameObject battleTrigger;
    GameObject endbattleTrigger;

    GameObject endgameObject;

    public GameObject resultCanvas;



    public bool playeronBattle = false;

    void Awake()
    {
        resultCanvas.SetActive(false);

        battleTrigger = GameObject.FindGameObjectWithTag("Battle");
        endbattleTrigger = GameObject.FindGameObjectWithTag("EndBattle");
        endgameObject = GameObject.FindGameObjectWithTag("EndGame");

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject ==  battleTrigger)
        {
            playeronBattle = true;
            Debug.Log("Battle Mode");
        }

        if (other.gameObject == endbattleTrigger)
        {
            playeronBattle = false;
            Debug.Log("Battle Close");
        }

        if (other.gameObject == endgameObject)
        {
            resultCanvas.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
