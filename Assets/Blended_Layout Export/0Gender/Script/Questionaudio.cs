using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Questionaudio : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    AudioSource audioSource;
    public AudioClip[] clip;
    public int Index;
    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }
    public void Update()
    {
        Index = dragmain.OBJ_dragmain.I_Qcount;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.localScale = new Vector3(1.1f, 1.1f, 0);
        audioSource.clip = clip[Index];
        audioSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.transform.localScale = new Vector3(1, 1, 0);
        audioSource.Stop();
    }
}
