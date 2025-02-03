using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Thumbnail4Controller : MonoBehaviour
{
    public GameObject[] panel1Contents;
    public GameObject[] panel2Contents;
    public GameObject panel1, 
                        panel2;
    public AudioClip[] contentClips;
    public AnimationClip panel1ChangeAnimClip, pane2ChangeAnimClip;
    public Button backBTN;
    int currentContentIndex = 0;

    void Start()
    {
        SpawnContent();
    }

    void SpawnContent()
    {
        backBTN.interactable = true;
        var spawnedPanel1 = Instantiate(panel1Contents[currentContentIndex], panel1.transform.GetChild(0));
        var spawnedPanel2 = Instantiate(panel2Contents[currentContentIndex], panel2.transform.GetChild(0));

        spawnedPanel1.GetComponent<Button>().onClick.AddListener(OnContentImageClicked);
        spawnedPanel2.GetComponent<Button>().onClick.AddListener(OnContentImageClicked);

        panel1.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = panel1Contents[currentContentIndex].name;
        panel2.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = panel2Contents[currentContentIndex].name;

        panel1.GetComponent<Animator>().Play("panel1_in");
        panel2.GetComponent<Animator>().Play("panel2_in");
        currentContentIndex++;
    }

    public void OnNextBTNClicked()
    {
        backBTN.interactable = false;
        panel1.GetComponent<Animator>().Play("panel1_change");
        panel2.GetComponent<Animator>().Play("panel2_change");
        Invoke(nameof(RemoveOLDSprite), panel1ChangeAnimClip.length);
    }

    public void OnContentNamePanelClicked()
    {
        Debug.Log("Button Clicked....");
        var selectedObject = EventSystem.current.currentSelectedGameObject;
        string selectedSTR = selectedObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        foreach (var clip in contentClips)
        {
            if(clip.name.ToLower().Contains(selectedSTR.ToLower()))
            {
                AudioManager.PlayAudio(clip);
                break;
            }
        }
    }

    public void OnContentImageClicked()
    {
        var selectedObject = EventSystem.current.currentSelectedGameObject;
        selectedObject.GetComponent<AudioSource>().Play();
    }

    void RemoveOLDSprite()
    {
        Destroy(panel1.transform.GetChild(0).GetChild(1).gameObject);
        Destroy(panel2.transform.GetChild(0).GetChild(1).gameObject);
        SpawnContent();
    }
}
