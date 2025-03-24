using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Thumbnail8Controller : MonoBehaviour
{
    public Transform stickObj;
    public Transform optionBoardPrefabObj;
    public GameObject ropePrefabObj;
    public Transform optionPlacementPosition;
    public Transform objectSpawnPosition;
    public GameObject optionDropHighlightner;
    public GameObject[] highlightnewObjs;
    public Transform questionPanel1,
                        questionPanel2,
                        questionPanel3;
    public GameObject displayOptionObj;
    public GenderDataStructure[] optionData;
    public GameObject counterObj;
    public TextMeshProUGUI textScoreBoard;
    public GameObject activityCompleted;
    public AudioClip wrongSFX;
    Transform[] _quesitonPanels;
    int answeredCount = 0;

    void Start()
    {
        _quesitonPanels = new Transform[3];
        _quesitonPanels[0] = questionPanel1;
        _quesitonPanels[1] = questionPanel2;
        _quesitonPanels[2] = questionPanel3;

        OptionSpawnObjBounceEffect();
        ResetQuestionPanelPosition();
        UpdateScoreBoard();
    }

    private void OnEnable() {
        ImageDragandDrop.onDrag += OnOptionDrag;
        ImageDragandDrop.onDragEnd += OnOptionObjDragEnd;
        ImageDropSlot.onDropInSlot += OnOptionObjectDroped;
    }

    private void OnDisable() {
        ImageDragandDrop.onDrag -= OnOptionDrag;
        ImageDragandDrop.onDragEnd -= OnOptionObjDragEnd;
        ImageDropSlot.onDropInSlot -= OnOptionObjectDroped;
    }

#region ANIMATION_METHODS

    void OptionSpawnObjBounceEffect()
    {
        Utilities.Instance.ANIM_BounceEffect(stickObj.transform, callback: () => { SpawnOption(); });
    }

    void ResetQuestionPanelPosition()
    {
        for (int i = 0; i < _quesitonPanels.Length; i++)
        {
            Utilities.Instance.ANIM_Move(_quesitonPanels[i], _quesitonPanels[i].position + (Vector3.up * 5), 0f);
        }
    }

    void SpawnCounter()
    {
        Utilities.Instance.ANIM_Move(counterObj.transform, counterObj.transform.position + (Vector3.up * 2f));
    }

    void SpawnQuestion(int spawnIndex = 0)
    {
        if(spawnIndex == _quesitonPanels.Length) { SpawnCounter(); return; }

        Utilities.Instance.ANIM_Move(
            _quesitonPanels[spawnIndex], 
            _quesitonPanels[spawnIndex].position + (Vector3.down * 4), 
            callBack: () => {
                _quesitonPanels[spawnIndex].gameObject.AddComponent<HangingBoardUI>();
                SpawnRope(spawnIndex, _quesitonPanels[spawnIndex].GetChild(0).position + (Vector3.up * 0.55f));
                SpawnQuestion(++spawnIndex);
            }
        );
    }

    void SpawnRope(int spawnParentIndex, Vector3 palacementPosition)
    {
        var spawnedRope = Instantiate(ropePrefabObj, _quesitonPanels[spawnParentIndex].parent);
        Utilities.Instance.ANIM_ShrinkOnPosition(spawnedRope.transform, new Vector3(1, 0, 1), 0f, callback: () => spawnedRope.SetActive(true));
        spawnedRope.transform.position = palacementPosition;
        spawnedRope.transform.SetAsFirstSibling();
        Utilities.Instance.ANIM_ShowNormal(spawnedRope.transform, callback: () => {
            // spawnedRope.AddComponent<HangingBoardUI>();
            MoveHighLightner(spawnParentIndex, spawnedRope.transform);
        });
    }

    void MoveHighLightner(int parentIndex, Transform spawnPosition)
    {
        highlightnewObjs[parentIndex].transform.position = spawnPosition.position + (Vector3.down * 2.2f);
    }

    void SpawnOption(int spawnIndex = 0)
    {
        if(spawnIndex == optionData.Length) { SpawnQuestion(); return; }

        var spawnedObj = Instantiate<Transform>(optionBoardPrefabObj, stickObj);
        spawnedObj.transform.position = objectSpawnPosition.position;
        spawnedObj.GetChild(0).GetComponent<TextMeshProUGUI>().text = optionData[spawnIndex].genderName;

        Utilities.Instance.ANIM_Move(spawnedObj, optionPlacementPosition.position + (Vector3.down * spawnIndex), callBack: () => { 
            SpawnOption(++spawnIndex);
            spawnedObj.GetComponent<ImageDragandDrop>().ResetParentOriginalPosition(); 
        });
    }
#endregion

    public void OnOptionDrag(GameObject draggingObj)
    {
        float[] distances = new float[_quesitonPanels.Length];
        int minDistanceIndex = -1;
        float smallestDis = 0f;
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = Vector3.Distance(draggingObj.transform.position, highlightnewObjs[i].transform.position);
            if(i == 0)
            {
                smallestDis = distances[i];
                minDistanceIndex = i;
            }else if(distances[i] < smallestDis) {
                smallestDis = distances[i];
                minDistanceIndex = i;
            }
        }
        EnableHighlightner(minDistanceIndex);
    }

    void OnOptionObjDragEnd(GameObject optionObj)
    {
        DiableAllHighlightner();
    }

    void OnOptionObjectDroped(GameObject dragObj, GameObject dropSlotObj)
    {
        string droppedGenderName = dragObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        var droppedGender = GetGenderData(droppedGenderName);
        var dropSlotIndex = GetSpawnIndex(dropSlotObj.transform.parent.name);

        if(!EvaluateAnswer(droppedGender, dropSlotIndex)) { AudioManager.PlayAudio(wrongSFX); return; }

        Destroy(dragObj);
        AudioManager.PlayAudio(droppedGender.genderNameClip);

        answeredCount++;
        UpdateScoreBoard();
        if (answeredCount == optionData.Length)
        {
            activityCompleted.SetActive(true);
            return;
        }

        var spawnAnswerObj = Instantiate(displayOptionObj, dropSlotObj.transform.parent.GetChild(0));
        spawnAnswerObj.transform.position = dropSlotObj.transform.position + (Vector3.up * 0.255f);
        spawnAnswerObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = droppedGenderName;
        spawnAnswerObj.transform.SetAsFirstSibling();
        spawnAnswerObj.SetActive(true);
        spawnAnswerObj.transform.parent.gameObject.AddComponent<HangingBoardUI>();

        dropSlotObj.SetActive(false);
        SpawnRope(GetSpawnIndex(dropSlotObj.transform.parent.name), (dropSlotObj.transform.parent.GetChild(0).GetChild(0).position  + (Vector3.up * 0.55f)));
    }

    void EnableActivityCompleted() => activityCompleted.SetActive(true);

    bool EvaluateAnswer(GenderDataStructure gender, int droppedObjIndx)
    {
        return ((int)gender.gender) == droppedObjIndx;
    }

    GenderDataStructure GetGenderData(string genderName)
    {
        for (int i = 0; i < optionData.Length; i++)
        {
            if (optionData[i].genderName.Equals(genderName))
            {
                return optionData[i];
            }
        }
        return null;
    }

    void UpdateScoreBoard()
    {
        textScoreBoard.text = $"{answeredCount} / {optionData.Length}";
    }

    int GetSpawnIndex(string parentObjName)
    {
        if(parentObjName.Contains("Q1")) return 0;
        else if(parentObjName.Contains("Q2")) return 1;
        else if(parentObjName.Contains("Q3")) return 2;
        return -1;
    }

    void DiableAllHighlightner()
    {
        for (int i = 0; i < highlightnewObjs.Length; i++)
        {
            highlightnewObjs[i].SetActive(false);
        }
    }

    void EnableHighlightner(int enablerIndex)
    {
        DiableAllHighlightner();

        highlightnewObjs[enablerIndex].SetActive(true);
    }

}

[System.Serializable]
public class GenderDataStructure
{
    public string genderName;
    public Genders gender;
    public Sprite genderSprite;
    public AudioClip genderNameClip;
}

public enum Genders
{
    Masculine,
    Feminine,
    Common
}