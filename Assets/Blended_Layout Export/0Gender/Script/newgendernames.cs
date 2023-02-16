using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class newgendernames : MonoBehaviour
{
    public GameObject[] GA_Questions;
    public int I_count, I_Qcount;
    public GameObject G_final;
    // Start is called before the first frame update
    void Start()
    {
        I_Qcount = 0;
        G_final.SetActive(false);
        showquestion();
    }

    public void showquestion()
    {
        for(int i=0;i<GA_Questions.Length;i++)
        {
            GA_Questions[i].SetActive(false);
        }
        I_count = -1;
        GA_Questions[I_Qcount].SetActive(true);
        BUT_Next();
    }
    
    public void BUT_Next()
    {
        I_count++;
        if(I_count<=2)
        {
           // GA_Questions[I_Qcount].SetActive(true);
            for (int i = 0; i < GA_Questions[I_Qcount].transform.childCount; i++)
            {
                GA_Questions[I_Qcount].transform.GetChild(i).gameObject.SetActive(false);
            }
            GA_Questions[I_Qcount].transform.GetChild(I_count).gameObject.SetActive(true);
        }
        else
        {
            I_Qcount++;
            if(I_Qcount<GA_Questions.Length)
            {
                showquestion();
            }
            else
            {
                G_final.SetActive(true);
            }
           
        }
        
    }
}
