using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class HoverAudio : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    public AudioSource audioSource;
    public AudioClip clip;
    public GameObject spawnedAuidoManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.localScale = new Vector3(1.1f, 1.1f, 0);

        if(audioSource == null)  {spawnedAuidoManager = AudioManager.PlayAudio(clip); return;}

        audioSource.clip = clip;
        audioSource?.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.transform.localScale = new Vector3(1, 1, 0);
        if(spawnedAuidoManager != null)
        {
            Destroy(spawnedAuidoManager);
            return;
        }
        audioSource?.Stop();
    }
}
