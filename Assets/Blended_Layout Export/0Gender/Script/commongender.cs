using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class commongender : MonoBehaviour
{

    public GameObject[] GA_Objects;
    public int I_count;
    public GameObject G_final;
    public Transform startPosition,
                    displayPosition,
                    endPosition;
    public DotNavigation dotNavigation;
    public TextMeshProUGUI displayText;

    [Header("Animation")]
    public Transform boardObj;
    public Transform nxtBtn, backBtn;

    private void Awake() {
        dotNavigation.maxCount = GA_Objects.Length;
        nxtBtn.gameObject.SetActive(false);
        backBtn.gameObject.SetActive(false);
    }

    void Start()
    {
        G_final.SetActive(false);
        I_count = 0;
        for (int i = 0; i < GA_Objects.Length; i++)
        {
            AllignToPosition(GA_Objects[i].transform, startPosition, 0f);
            GA_Objects[i].SetActive(true);
        }

        ShowBoard();
        // showobject(displayPosition);
    }

    public void Showobject(Transform targetPosition, Action callBackFunc = null)
    {
        Utilities.Instance.ANIM_Move(GA_Objects[I_count].transform, targetPosition.position, callBack: ()=>{
            callBackFunc();
        });
    }

    public void BUT_next()
    {
        if(I_count<GA_Objects.Length-1)
        {
            Showobject(endPosition);
            I_count++;
            Showobject(displayPosition, () => {
                displayText.text = GA_Objects[I_count].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
            });
            dotNavigation.OnClickNextButton();
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
            Showobject(startPosition);
            I_count--;
            Showobject(displayPosition, () => {
                displayText.text = GA_Objects[I_count].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
            });
            dotNavigation.OnClickBackButton();
        }
    }

    void AllignToPosition(Transform sourceObject, Transform targetObject, float moveTime)
    {
        Utilities.Instance.ANIM_Move(sourceObject, targetObject.position, moveTime);
    }

#region ANIMATION

    void ShowBoard()
    {
        Utilities.Instance.ANIM_ShrinkOnPosition(boardObj, new Vector3(1, 0, 1), 0f);
        Utilities.Instance.ANIM_BounceEffect(boardObj, callback: MoveNextBackButton);
    }

    void MoveNextBackButton()
    {
        nxtBtn.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);
        Utilities.Instance.ANIM_Move(nxtBtn, (nxtBtn.position + (Vector3.right * 1.5f)));
        Utilities.Instance.ANIM_Move(backBtn, (backBtn.position + (Vector3.left * 1.5f)), callBack: () => {
            Showobject(displayPosition, () => {
                displayText.text = GA_Objects[I_count].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
            });
        });
    }

#endregion
}
