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
    public string[] options;
    public GameObject[] highlightnewObjs;
    public Transform questionPanel1,
                        questionPanel2,
                        questionPanel3;
    public GameObject displayOptionObj;
    Transform[] _quesitonPanels;
    void Start()
    {
        _quesitonPanels = new Transform[3];
        _quesitonPanels[0] = questionPanel1;
        _quesitonPanels[1] = questionPanel2;
        _quesitonPanels[2] = questionPanel3;

        OptionSpawnObjBounceEffect();
        ResetQuestionPanelPosition();
    }

    private void OnEnable() {
        ImageDragandDrop.onDrag += OnOptionDrag;
        ImageDragandDrop.onDragEnd += OnOptionObjDragEnd;
        ImageDropSlot.onDropInSlot += OnOptionObjectDroped;
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

    void SpawnQuestion(int spawnIndex = 0)
    {
        if(spawnIndex == _quesitonPanels.Length) return;
        // Debug.Log($"spawnIndex :: {spawnIndex}");

        Utilities.Instance.ANIM_Move(
            _quesitonPanels[spawnIndex], 
            _quesitonPanels[spawnIndex].position + (Vector3.down * 4), 
            callBack: () => {
                _quesitonPanels[spawnIndex].gameObject.AddComponent<HangingBoardUI>();
                SpawnRope(spawnIndex);
                SpawnQuestion(++spawnIndex);
            }
        );
    }

    void SpawnRope(int spawnParentIndex)
    {
        var spawnedRope = Instantiate(ropePrefabObj, _quesitonPanels[spawnParentIndex].parent);
        Utilities.Instance.ANIM_ShrinkOnPosition(spawnedRope.transform, new Vector3(1, 0, 1), 0f, callback: () => spawnedRope.SetActive(true));
        spawnedRope.transform.position = _quesitonPanels[spawnParentIndex].position;
        spawnedRope.transform.SetAsFirstSibling();
        Utilities.Instance.ANIM_ShowNormal(spawnedRope.transform, callback: () => {
            MoveHighLightner(spawnParentIndex, spawnedRope.transform);
        });
    }

    void MoveHighLightner(int parentIndex, Transform spawnPosition)
    {
        highlightnewObjs[parentIndex].transform.position = spawnPosition.position + (Vector3.down * 3f);
        // highlightnewObjs[parentIndex].SetActive(true);
    }

    void SpawnOption(int spawnIndex = 0)
    {
        if(spawnIndex == options.Length) { SpawnQuestion(); return; }

        var spawnedObj = Instantiate<Transform>(optionBoardPrefabObj, stickObj);
        spawnedObj.transform.position = objectSpawnPosition.position;
        spawnedObj.GetChild(0).GetComponent<TextMeshProUGUI>().text = options[spawnIndex];

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
        Destroy(dragObj);
        var spawnAnswerObj = Instantiate(displayOptionObj, dropSlotObj.transform.parent.GetChild(0));
        spawnAnswerObj.transform.position = dropSlotObj.transform.position + (Vector3.up * 0.45f);
        spawnAnswerObj.transform.SetAsFirstSibling();
        spawnAnswerObj.SetActive(true);
        dropSlotObj.SetActive(false);
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
