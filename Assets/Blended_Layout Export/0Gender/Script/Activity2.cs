using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Activity2 : MonoBehaviour
{
    public GameObject[] GA_Questions;
    public int I_Qcount;
    public GameObject G_final;
    //public Sprite SPR_green, SPR_red,SPR_normal;
    GameObject dummy;
    bool B_Canclick;
    // Start is called before the first frame update
    void Start()
    {
        I_Qcount = -1;
        showquestion();
    }

    public void showquestion()
    {
        I_Qcount++;
        B_Canclick = true;
        if (I_Qcount < GA_Questions.Length)
        {
            for (int i = 0; i < GA_Questions.Length; i++)
            {
                GA_Questions[i].SetActive(false);
            }
            GA_Questions[I_Qcount].SetActive(true);
            //GA_Questions[I_Qcount].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            G_final.SetActive(true);
        }

    }
    public void BUT_clicking()
    {
        if(B_Canclick)
        {
            dummy = EventSystem.current.currentSelectedGameObject;
            B_Canclick = false;
            if (dummy.tag == "answer")
            {
                dummy.GetComponent<Image>().color = Color.green;
                //GA_Questions[I_Qcount].transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<Image>().color = Color.green;
                //GA_Questions[I_Qcount].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                dummy.GetComponent<Image>().color = Color.red;
               //GA_Questions[I_Qcount].transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<Image>().color = Color.red;
                Invoke("THI_normal", 1f);
            }
        }
       

    }
    public void THI_normal()
    {
        dummy.GetComponent<Image>().color = Color.white;
       // GA_Questions[I_Qcount].transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<Image>().color = Color.white;
        B_Canclick = true;
    }

    public void BUT_next()
    {
        showquestion();
    }
}
