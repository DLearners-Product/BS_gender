using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotNavigation : MonoBehaviour
{

    #region user input variables
    
    [Header("SETTINGS------------------------------------------------------------------------------------------------")]
    //user configuration
    public int maxCount;
    public bool remainFilled;
    public bool hasEnd;

    #endregion


    # region unity variables

    [Header("BUTTON------------------------------------------------------------------------------------------------")]
    //button
    [SerializeField] private Button btn_Next;
    [SerializeField] private Button btn_Back;


    [Header("GAME OBJECT------------------------------------------------------------------------------------------------")]
    //game object
    [SerializeField] private GameObject go_DotPrefab;
    [SerializeField] private GameObject go_BlackScreen;


    [Header("TRANSFORM ------------------------------------------------------------------------------------------------")]
    //game object
    [SerializeField] private Transform go_DotsContainer;

    #endregion


    #region local variables
    private int currentCount;
    private List<GameObject> go_InnerDots;

    #endregion


    void Start()
    {
        currentCount = 0;

        go_InnerDots = new List<GameObject>();

        for (int i = 0; i < maxCount; i++)
        {
            var dot = Instantiate(go_DotPrefab, new Vector3(0, 0, 0), Quaternion.identity, go_DotsContainer);

            if (i == 0) dot.transform.GetChild(0).gameObject.SetActive(true);       //condition to make first dot enabled
            else dot.transform.GetChild(0).gameObject.SetActive(false);             //condition to make other than first dot disabled

            go_InnerDots.Add(dot.transform.GetChild(0).gameObject);
        }
    }


    public void OnClickNextButton()
    {
        if (!remainFilled) go_InnerDots[currentCount].SetActive(false);         //condition to keep the fill remain during navigation

        currentCount++;                                                         //incrementing count

        if (currentCount >= maxCount)                                           //condition to make black screen visible
        {
            go_BlackScreen.SetActive(true);
            return;
        }

        if (go_InnerDots[currentCount].activeSelf) go_InnerDots[currentCount].GetComponent<Animator>().SetTrigger("active");
        else go_InnerDots[currentCount].SetActive(true);

        if (!hasEnd)
            if (currentCount == maxCount - 1) btn_Next.interactable = false;    //if hasEnd is disabled, disabling next when reaching last dot

        if (currentCount == 1) btn_Back.interactable = true;                    //enabling back when you can go behind
    }


    public void OnClickBackButton()
    {
        if (!remainFilled) go_InnerDots[currentCount].SetActive(false);     //condition to keep the fill remain during navigation

        currentCount--;                                                     //decrementing count

        if (go_InnerDots[currentCount].activeSelf) go_InnerDots[currentCount].GetComponent<Animator>().SetTrigger("active");
        else go_InnerDots[currentCount].SetActive(true);

        if (currentCount == 0) btn_Back.interactable = false;               //disabling back when reaching first dot
        if (currentCount == maxCount - 2) btn_Next.interactable = true;     //enabling next when you can go forward
    }

}
