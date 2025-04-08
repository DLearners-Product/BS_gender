using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Thumbnail5Controller : MonoBehaviour
{
    [SerializeField] List<CattleData> maleCattleData;
    [SerializeField] List<CattleData> femaleCattleData;
    [SerializeField] List<EnvironmentData> forestEnvironmentObjs;
    [SerializeField] List<EnvironmentData> pondEnvironmentObjs;
    [SerializeField] List<EnvironmentData> farmEnvironmentObjs;
    [SerializeField] List<EnvironmentData> coopEnvironmentObjs;

    [SerializeField] Transform animalDisplayPosition;
    [SerializeField] Transform bridDisplayPosition;
    [SerializeField] Button nextBTN, backBTN;
    [SerializeField] Transform displayPanel;
    [SerializeField] TextMeshProUGUI displayText;
    [SerializeField] string[] displayPanels;
    [SerializeField] AudioClip[] genderPanelAudioClip;
    [SerializeField] Transform textDisplayPanel;
    [SerializeField] GameObject activityCompleted;
    List<string> birds = new List<string>(){"drake", "duck"};
    int prevIndex = 0, currentIndex = 0;
    List<EnvironmentData> currentEnv;
    List<CattleData> currentCattleData;
    Transform displayPanelOrgPos;

    void Start()
    {
        currentCattleData = maleCattleData;
        PlayGenderTypeAudio(genderPanelAudioClip[0]);
        displayPanelOrgPos = textDisplayPanel.transform;
        LowerDisplayPanel(0f);
        backBTN.interactable = false;
        nextBTN.interactable = false;
    }

    void PlayGenderTypeAudio(AudioClip playClip)
    {
        AudioManager.PlayAudio(playClip);
        Invoke(nameof(ShowContent), playClip.length);
    }

    void ShowContent()
    {
        string cattleType = currentCattleData[currentIndex].cattleType;

        switch (cattleType)
        {
            case "farm":
                currentEnv = farmEnvironmentObjs;
                break;
            case "coop":
                currentEnv = coopEnvironmentObjs;
                break;
            case "forest":
                currentEnv = forestEnvironmentObjs;
                break;
            case "pond":
                currentEnv = pondEnvironmentObjs;
                break;
        }
        ShowEnvironment(currentEnv);
    }

    public void OnNextButtonClick()
    {
        prevIndex = currentIndex;
        currentIndex++;
        LowerDisplayPanel(0.5f);
        nextBTN.interactable = false;
        backBTN.interactable = false;
        RemoveCurrentEnv(currentEnv, currentEnv.Count - 1);
    }

    public void OnBackButtonClick()
    {
        prevIndex = currentIndex;
        currentIndex--;
        LowerDisplayPanel(0.5f);
        nextBTN.interactable = false;
        backBTN.interactable = false;
        RemoveCurrentEnv(currentEnv, currentEnv.Count - 1);
    }

    public void OnSpeakerBTNClick()
    {
        AudioManager.PlayAudio(currentCattleData[currentIndex].cattleName);
    }

    void ChangeAnimal()
    {
        // ++currentIndex;
        if(currentIndex == maleCattleData.Count && currentCattleData == femaleCattleData)
        {
            activityCompleted.SetActive(true);
        } else if (currentIndex == maleCattleData.Count) {
            currentIndex = 0;
            currentCattleData = femaleCattleData;
            ShrinkPanelAndExpand(displayPanels[1], genderPanelAudioClip[1]);
        } else if(currentIndex < 0) {
            currentIndex = maleCattleData.Count - 1;
            currentCattleData = maleCattleData;
            ShrinkPanelAndExpand(displayPanels[0], genderPanelAudioClip[0]);
        } else
            ShowContent();
    }


#region ANIMATION

    void ShowEnvironment(List<EnvironmentData> envData, int startIndex = 0)
    {
        if(envData.Count == startIndex) return;

        envData[startIndex].MoveToDestination(() => {
            ShowEnvironment(envData, ++startIndex);
            Debug.Log($"startIndex :: {startIndex}");
            if(startIndex == 1) ShowAnimal();
        });
    }

    void RemoveCurrentEnv(List<EnvironmentData> envData, int startIndex = 0)
    {
        if(startIndex < 0) return;

        envData[startIndex].RevertToOriginalPos(() => {
            RemoveCurrentEnv(envData, --startIndex);
            Debug.Log($"startIndex :: {startIndex}");
            if(startIndex < 0) RemoveAnimal();
        });
    }

    void ShowAnimal()
    {
        if(birds.Contains(currentCattleData[currentIndex].cattleObject.name.ToLower()))
            currentCattleData[currentIndex].MoveTo(bridDisplayPosition, () => {
                backBTN.interactable = true;
                nextBTN.interactable = true;

                textDisplayPanel.GetComponentInChildren<TextMeshProUGUI>().text = currentCattleData[currentIndex].cattleObject.name;

                RiseDisplayPanel(0.5f);
            });
        else
            currentCattleData[currentIndex].MoveTo(animalDisplayPosition, () => {
                nextBTN.interactable = true;
                if(currentIndex != 0 || currentCattleData.Equals(femaleCattleData)) backBTN.interactable = true;

                textDisplayPanel.GetComponentInChildren<TextMeshProUGUI>().text = currentCattleData[currentIndex].cattleObject.name;

                RiseDisplayPanel(0.5f);
            });
    }

    void LowerDisplayPanel(float moveTime) => Utilities.Instance.ANIM_Move(textDisplayPanel, displayPanelOrgPos.position + Vector3.down * 2f, moveTime);
    void RiseDisplayPanel(float moveTime) => Utilities.Instance.ANIM_Move(textDisplayPanel, displayPanelOrgPos.position + Vector3.up * 2f, moveTime);
    void RemoveAnimal() => currentCattleData[prevIndex].ResetPosition(ChangeAnimal);

    void ShrinkPanelAndExpand(string displayContent, AudioClip genderAudioClip)
    {
        Utilities.Instance.ANIM_ScaleEffect(displayPanel, new Vector3(0, 1, 1), () => {
            displayText.text = displayContent;
            Utilities.Instance.ANIM_ScaleEffect(displayPanel, Vector3.one);
            PlayGenderTypeAudio(genderAudioClip);
        });
    }

#endregion

    [Serializable]
    class CattleData
    {
        public GameObject cattleObject;
        public string cattleType;
        public AudioClip cattleName;
        Vector3 originalPosition;

        public void MoveTo(Transform targetPos, Action callBack = null)
        {
            originalPosition = cattleObject.transform.position;
            Utilities.Instance.ANIM_Move(cattleObject.transform, targetPos.position, callBack : ()=>{
                AudioManager.PlayAudio(cattleName);
                callBack?.Invoke();
            });
        }

        public void ResetPosition(Action callBack = null)
        {
            Utilities.Instance.ANIM_Move(cattleObject.transform, originalPosition, callBack: ()=>{
                callBack?.Invoke();
            });
        }
    }

    [Serializable]
    class EnvironmentData
    {
        public GameObject sourceObj;
        public Transform endPoint;
        Vector3 originalPosition;

        public void MoveToDestination(Action callback = null)
        {
            originalPosition = sourceObj.transform.position;
            Utilities.Instance.ANIM_Move(sourceObj.transform, endPoint.position, callBack: () => { callback?.Invoke(); });
        }

        public void RevertToOriginalPos(Action callback = null)
        {
            Utilities.Instance.ANIM_Move(sourceObj.transform, originalPosition, callBack: () => { callback?.Invoke(); });
        }
    }
}

