using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public GameObject G_mainmenu,
                        G_back;
    public Transform[] spawnPoints;
    public GameObject[] GA_genderMenuPanels;
    public GameObject[] GA_genderObjs;
    public GameObject[] GA_masculineAssets;
    public GameObject[] GA_feminineAssets;
    public GameObject[] GA_commonAssets;
    Vector3[] mainMenuObjOrgPos;
    int currentSelectedGender = -1;

    private void Start()
    {
        mainMenuObjOrgPos = new Vector3[GA_genderMenuPanels.Length];

        for (int i = 0; i < GA_genderMenuPanels.Length; i++)
        {
            mainMenuObjOrgPos[i] = GA_genderMenuPanels[i].transform.position;
        }
    }

    public void BUT_select(int index)
    {
        G_mainmenu.SetActive(false);
        G_back.SetActive(true);
        for (int i=0;i<GA_genderObjs.Length;i++)
        {
            GA_genderObjs[i].SetActive(false);
        }
        GA_genderObjs[index].SetActive(true);
    }

    public void Back_menu()
    {
        G_back.SetActive(false);
        for (int i = 0; i < GA_genderObjs.Length; i++)
        {
            GA_genderObjs[i].SetActive(false);
        }
        G_mainmenu.SetActive(true);
    }

#region LISTENERS

    public void OnGenderPanelClicked(int genderIndex)
    {
        switch (genderIndex)
        {
            case 1:
                GA_genderMenuPanels[1].SetActive(false);
                GA_genderMenuPanels[2].SetActive(false);
                GA_genderMenuPanels[0].transform.SetAsLastSibling();
                currentSelectedGender = 0;
                MoveAnndScaleDownTo(GA_masculineAssets, GA_genderMenuPanels[1].transform);
                break;
            case 2:
                GA_genderMenuPanels[0].SetActive(false);
                GA_genderMenuPanels[2].SetActive(false);
                GA_genderMenuPanels[1].transform.SetAsLastSibling();
                currentSelectedGender = 1;
                MoveAnndScaleDownTo(GA_feminineAssets, GA_genderMenuPanels[1].transform);
                break;
            case 3:
                GA_genderMenuPanels[0].SetActive(false);
                GA_genderMenuPanels[1].SetActive(false);
                GA_genderMenuPanels[2].transform.SetAsLastSibling();
                currentSelectedGender = 2;
                MoveAnndScaleDownTo(GA_commonAssets, GA_genderMenuPanels[1].transform);
                break;
            default:
                break;
        }
    }

    public void OnBackButtonClicked()
    {
        switch(currentSelectedGender)
        {
            case 0:
                ResetFlotObjPos(GA_masculineAssets, 0, callBack: ResetBackToOriginalMainMenu);
                break;
            case 1:
                ResetFlotObjPos(GA_feminineAssets, 0, callBack: ResetBackToOriginalMainMenu);
                break;
            case 2:
                ResetFlotObjPos(GA_commonAssets, 0, callBack: ResetBackToOriginalMainMenu);
                break;
        }
    }

#endregion

#region ANIMATION_EFFECTS

    void MoveAnndScaleDownTo(GameObject[] genderChildObj, Transform endPosObj)
    {
        ResetFlotObjPos(genderChildObj, 0, 0f, 0f);

        Transform sourceObj = GA_genderMenuPanels[currentSelectedGender].transform;
        Transform panelObj = G_mainmenu.transform.GetChild(G_mainmenu.transform.childCount - 2);

        panelObj.gameObject.SetActive(true);

        Utilities.Instance.ANIM_ImageFade(panelObj.GetComponent<Image>(), 0.5f);
        Utilities.Instance.ANIM_MoveWithScaleUp(sourceObj, endPosObj.position, () => {
            G_mainmenu.SetActive(false);
            GA_genderObjs[currentSelectedGender].SetActive(true);
            ReleaseChildObj(genderChildObj, 0);
        });
    }

    void ResetFlotObjPos(GameObject[] genderObjs, int objIndex, float moveTime=0.5f, float scaleTime=0.5f, Action callBack=null)
    {
        if(genderObjs.Length == objIndex) return;

        genderObjs[objIndex].GetComponent<FloatingObject>().enabled = false;

        if(callBack != null && objIndex == (genderObjs.Length - 1)){
            Utilities.Instance.ANIM_MoveWithScaleDown(genderObjs[objIndex].transform, Vector3.zero, moveTime, scaleTime, () => {
                callBack();
            });
            return;
        }

        Utilities.Instance.ANIM_MoveWithScaleDown(genderObjs[objIndex].transform, Vector3.zero, moveTime, scaleTime, () => {
            ResetFlotObjPos(genderObjs, ++objIndex, moveTime, scaleTime, callBack);
        });
    }

    void ReleaseChildObj(GameObject[] childObjects, int childIndex)
    {
        if(childIndex == childObjects.Length) { G_back.SetActive(true); return; }

        Utilities.Instance.ANIM_MoveWithScaleUp(childObjects[childIndex].transform, spawnPoints[childIndex].position, () => {
            childObjects[childIndex].GetComponent<FloatingObject>().enabled = true;
            ReleaseChildObj(childObjects, ++childIndex);
        });
    }

    void ResetBackToOriginalMainMenu()
    {
        G_mainmenu.SetActive(true);
        GA_genderObjs[currentSelectedGender].SetActive(false);

        Transform panelObj = G_mainmenu.transform.GetChild(G_mainmenu.transform.childCount - 2);
        Utilities.Instance.ANIM_ImageFade(panelObj.GetComponent<Image>(), 0f);

        Utilities.Instance.ANIM_Move(GA_genderMenuPanels[currentSelectedGender].transform, mainMenuObjOrgPos[currentSelectedGender], callBack: () => {
            GA_genderMenuPanels[currentSelectedGender].transform.SetSiblingIndex(currentSelectedGender);
            EnableAllMenuObj();
            currentSelectedGender = -1;
            panelObj.gameObject.SetActive(false);
            G_back.SetActive(false);
        });

        Utilities.Instance.ScaleObject(GA_genderMenuPanels[currentSelectedGender].transform, 1.25f, 0.5f);
    }

    void EnableAllMenuObj()
    {
        for (int i = 0; i < GA_genderMenuPanels.Length; i++)
        {
            GA_genderMenuPanels[i].SetActive(true);
        }
    }

#endregion
}
