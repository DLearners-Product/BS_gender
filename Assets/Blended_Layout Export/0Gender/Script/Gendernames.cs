using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gendernames : MonoBehaviour
{
    public Sprite[] SPR_Images;
    public GameObject G_Image;
    // Start is called before the first frame update
    void Start()
    {
        G_Image.SetActive(false);
    }

    // Update is called once per frame
    public void BUT_click(int index)
    {
        G_Image.SetActive(true);
        G_Image.GetComponent<Image>().sprite = SPR_Images[index];
        G_Image.GetComponent<Image>().preserveAspect = true;
    }
}
