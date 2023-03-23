using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class dragmain : MonoBehaviour
{
    public static dragmain OBJ_dragmain;
    public GameObject[] GA_Questions;
    public int I_Qcount;
    public GameObject G_answeright;
    public Text TXT_Qtext;
    public string[] STR_Questions;
    public GameObject G_weighter, G_final;
    public AudioSource AS_crt, AS_wrg;

   
    public void Start()
    {
        OBJ_dragmain = this;
        I_Qcount = -1;
        G_final.SetActive(false);
        ScoreManager.instance.InstantiateScore(GA_Questions.Length);
        showquestion();
    }
    public void showquestion()
    {
        I_Qcount++;
        G_weighter.GetComponent<Animator>().SetInteger("Cond", 0);
        if (I_Qcount<GA_Questions.Length)
        {
            for (int i = 0; i < GA_Questions.Length; i++)
            {
                GA_Questions[i].SetActive(false);
            }
            GA_Questions[I_Qcount].SetActive(true);
            GA_Questions[I_Qcount].transform.GetChild(0).gameObject.SetActive(false);
            G_answeright.GetComponent<Image>().enabled = false;
            G_answeright.transform.GetChild(0).GetComponent<Text>().text = null;
            TXT_Qtext.text = STR_Questions[I_Qcount];
        }
        else
        {
            G_final.SetActive(true);
        }
        
    }

    public void BUT_next()
    {
        showquestion();
    }
    public void THI_correct()
    {
        ScoreManager.instance.RightAnswer(I_Qcount, 1, GA_Questions[I_Qcount].transform.GetChild(1).transform.GetChild(0).GetComponentInChildren<Image>().name);
        G_weighter.GetComponent<Animator>().SetInteger("Cond", 1);
        GA_Questions[I_Qcount].transform.GetChild(0).gameObject.SetActive(true);
        AS_crt.Play();
       
    }
    public void THI_wrg()
    {
        ScoreManager.instance.WrongAnswer(I_Qcount);
        AS_wrg.Play();
    }
    
   
   

    
}
