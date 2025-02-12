using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class commongender : MonoBehaviour
{

    public GameObject[] GA_Objects;
    public int I_count;
    public GameObject G_final;

    void Start()
    {
        G_final.SetActive(false);
        I_count = 0;
        showobject();
    }

    public void showobject()
    {
        for(int i=0;i<GA_Objects.Length;i++)
        {
            GA_Objects[i].SetActive(false);
        }
        GA_Objects[I_count].SetActive(true);
    }
    public void BUT_next()
    {
        if(I_count<GA_Objects.Length-1)
        {
            I_count++;
            showobject();
        }
        else
        {
            G_final.SetActive(true);
        }
    }
    public void BUT_back()
    {
        if (I_count >0)
        {
            I_count--;
            showobject();
        }
    }
}
