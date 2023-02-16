using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public GameObject[] GA_Objects;
    public GameObject G_mainmenu,G_back;

    private void Start()
    {
        Back_menu();
    }
    public void BUT_select(int index)
    {
        G_mainmenu.SetActive(false);
        G_back.SetActive(true);
        for (int i=0;i<GA_Objects.Length;i++)
        {
            GA_Objects[i].SetActive(false);
        }
        GA_Objects[index].SetActive(true);
    }
    public void Back_menu()
    {
        Debug.Log("back");
        G_back.SetActive(false);
        for (int i = 0; i < GA_Objects.Length; i++)
        {
            GA_Objects[i].SetActive(false);
        }
        G_mainmenu.SetActive(true);
    }

}
