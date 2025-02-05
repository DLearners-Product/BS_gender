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
    [SerializeField] Button bakcBTN;
    [SerializeField] Transform displayPanel;
    [SerializeField] TextMeshProUGUI displayText;
    [SerializeField] string[] displayPanels;
    List<string> birds = new List<string>(){"drake", "duck"};
    int contentIndex = 0;
    List<EnvironmentData> currentEnv;
    List<CattleData> currentCattleData;

    void Start()
    {
        currentCattleData = maleCattleData;
        ShowContent();
    }

    void ShowContent()
    {
        string cattleType = currentCattleData[contentIndex].cattleType;

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

    public void OnBackButtonClick()
    {
        bakcBTN.interactable = false;
        RemoveCurrentEnv(currentEnv, currentEnv.Count - 1);
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
        if(birds.Contains(currentCattleData[contentIndex].cattleObject.name.ToLower()))
            currentCattleData[contentIndex].MoveTo(bridDisplayPosition, () => {bakcBTN.interactable = true;});
        else
            currentCattleData[contentIndex].MoveTo(animalDisplayPosition, () => {bakcBTN.interactable = true;});
    }

    void RemoveAnimal()
    {
        currentCattleData[contentIndex].ResetPosition(()=>{ ++contentIndex; ShowContent(); });
    }

    void ShrinkPanelAndExpand()
    {
        Utilities.Instance.ANIM_ScaleEffect(displayPanel, new Vector3(0, 1, 1), () => {
            displayText.text = displayPanels[1];
            Utilities.Instance.ANIM_ScaleEffect(displayPanel, Vector3.one);
        });
    }

#endregion

    [Serializable]
    class CattleData
    {
        public Sprite cattleSprite;
        public GameObject cattleObject;
        public string cattleType;
        Vector3 originalPosition;

        public void MoveTo(Transform targetPos, Action callBack = null)
        {
            originalPosition = cattleObject.transform.position;
            Utilities.Instance.ANIM_Move(cattleObject.transform, targetPos.position, callBack : ()=>{callBack();});
        }

        public void ResetPosition(Action callBack = null)
        {
            Utilities.Instance.ANIM_Move(cattleObject.transform, originalPosition, callBack: ()=>{
                callBack();
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
            Utilities.Instance.ANIM_Move(sourceObj.transform, endPoint.position, callBack: () => { callback(); });
        }

        public void RevertToOriginalPos(Action callback = null)
        {
            Utilities.Instance.ANIM_Move(sourceObj.transform, originalPosition, callBack: () => { callback(); });
        }
    }
}

