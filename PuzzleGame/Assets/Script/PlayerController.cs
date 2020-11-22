using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    private Transform tr;
    private Animator anim;

    private int plattack = 0;

    public SliderMgr slidemgr;

    public int hskill;

    public int finishedAttack = 0;

    //private int hashRun = Animator.StringToHash("IsRun");
    //private int hashAttack = Animator.StringToHash("IsAttack");


    // 이동 속도 변수
    public float moveSpeed = 10.0f;

    // 회전 속도 변수
    public float rotSpeed = 80.0f;


    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
    }

    void FixedUpdate ()
    {

     


    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log(this.hskill);
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        // 전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        // 이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준좌표)
        tr.Translate(moveDir.normalized * moveSpeed * v * Time.deltaTime, Space.Self);

        // 회전축 * 속도 * Time.deltaTime * 변위값
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * h);

        AnimationUpdate();

    }


    void AnimationUpdate()
    {
        if (h == 0 && v == 0)
        {
            anim.SetBool("IsRun", false);
        }
        else
        {
            anim.SetBool("IsRun", true);
        }

        if (this.slidemgr.Hskill >= 1)
        {
            anim.SetBool("IsAttack", true);
            finishedAttack = 1;
        }
        else
        {
            anim.SetBool("IsAttack", false);
            finishedAttack = 0;
        }

    }
}