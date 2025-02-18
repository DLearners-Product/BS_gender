using System.Collections;
using System.Collections.Generic;
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

    [Header("Animation")]
    public Transform boardObj;

    private void Awake() {
        dotNavigation.maxCount = GA_Objects.Length;
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

    public void showobject(Transform targetPosition)
    {
        Utilities.Instance.ANIM_Move(GA_Objects[I_count].transform, targetPosition.position);
    }

    public void BUT_next()
    {
        if(I_count<GA_Objects.Length-1)
        {
            showobject(endPosition);
            I_count++;
            showobject(displayPosition);
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
            showobject(startPosition);
            I_count--;
            showobject(displayPosition);
            dotNavigation.OnClickBackButton();
        }
    }

    void AllignToPosition(Transform sourceObject, Transform targetObject, float moveTime)
    {
        Utilities.Instance.ANIM_Move(sourceObject, targetObject.position, moveTime);
    }

#region ANIMATION

    public void ShowBoard()
    {
        Utilities.Instance.ANIM_ShrinkOnPosition(boardObj, new Vector3(1, 0, 1), 0f);
        Utilities.Instance.ANIM_JumpWithEffect(boardObj);
    }

#endregion
}
