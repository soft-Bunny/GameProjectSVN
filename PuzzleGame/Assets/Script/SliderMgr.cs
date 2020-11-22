using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMgr : MonoBehaviour
{
    public Slider miniheart;
    public Slider minidiamond;
    public Slider minispade;
    public Slider miniclober;

    public Text heartnum;
    public Text diamondnum;
    public Text spadenum;
    public Text clobernum;

    public int Hscore = 0;
    public int Dscore = 0;
    public int Sscore = 0;
    public int Cscore = 0;

    public int Hskill = 0;
    public int Dskill = 0;
    public int Sskill = 0;
    public int Cskill = 0;

    PlayerController playercontroller;

    //public struct skill
    //{
    //    public int Hskill;
    //    public int Dskill;
    //    public int Sskill;
    //    public int Cskill;

    //    public skill (int Hskill, int Dskill, int Sskill, int Cskill)
    //    {       
    //        this.Hskill = 0;
    //        this.Dskill = 0;
    //        this.Sskill = 0;
    //        this.Cskill = 0;

    //    }
    //}

    //public skill addskill;

    public int Maxscore = 30;

    BlockControl blockcontrol;

    // Start is called before the first frame update
    void Start()
    {
    }
 
    // Update is called once per frame
    void Update()
    {
        //Hscore = this.blockcontrol.counting.heart;
        //Dscore = this.blockcontrol.counting.diamond;
        //Sscore = this.blockcontrol.counting.spade;
        //Cscore = this.blockcontrol.counting.clober;

        heartnum.text = Hscore.ToString();
        diamondnum.text = Dscore.ToString();
        spadenum.text = Sscore.ToString();
        clobernum.text = Cscore.ToString();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Hscore += 5;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Dscore += 5;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Sscore += 5;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Cscore += 5;
        }
        Slide();
        Skill();

        //if (this.Hskill >= 1)
        //{
        //    anim.SetBool("IsAttack", true);

        //}
        //else
        //{
        //    anim.SetBool("IsAttack", false);

        //}


        FinishedSkill();
    }

    public void Slide()
    {
        miniheart.value = Mathf.Lerp(miniheart.value, (float)Hscore / (float)Maxscore, Time.deltaTime * 10);
        minidiamond.value = Mathf.Lerp(minidiamond.value, (float)Dscore / (float)Maxscore, Time.deltaTime * 10);
        minispade.value = Mathf.Lerp(minispade.value, (float)Sscore / (float)Maxscore, Time.deltaTime * 10);
        miniclober.value = Mathf.Lerp(miniclober.value, (float)Cscore / (float)Maxscore, Time.deltaTime * 10);
    }

    public void Skill()
    {
        if (Hscore >= 30)
        {
            Hscore = 0;
            //this.addskill.Hskill++;
            Hskill++;
        }
        if (Dscore >= 30)
        {
            Dscore = 0;
            //this.addskill.Dskill++;
            Dskill++;
        }
        if (Sscore >= 30)
        {
            Sscore = 0;
            //this.addskill.Sskill++;
            Sskill++;

        }
        if (Cscore >= 30)
        {
            Cscore = 0;
            //this.addskill.Cskill++;
            Cskill++;
        }
    }

    public void FinishedSkill()
    {
        if (this.playercontroller.finishedAttack == 1)
        {
            Hskill = 0;
            Dskill = 0;
            Sskill = 0;
            Cskill = 0;
        }

    }

    public void Count0(int i)
    {
       i = 0;
    }

}
