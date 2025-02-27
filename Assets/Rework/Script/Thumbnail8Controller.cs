using System.Collections;
using System.Collections.Generic;
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

        Utilities.Instance.ANIM_Move(
            _quesitonPanels[spawnIndex], 
            _quesitonPanels[spawnIndex].position + (Vector3.down * 4), 
            callBack: () => {
                _quesitonPanels[spawnIndex].gameObject.AddComponent<HangingBoardUI>();
                _quesitonPanels[spawnIndex].GetComponent<ImageDragandDrop>().ResetParentOriginalPosition();
                SpawnRope(spawnIndex);
                SpawnQuestion(++spawnIndex);
            }
        );
    }

    void SpawnRope(int spawnParentIndex)
    {
        var spawnedRope = Instantiate(ropePrefabObj, _quesitonPanels[spawnParentIndex].parent);
        Utilities.Instance.ANIM_ShrinkOnPosition(spawnedRope.transform, new Vector3(1, 0, 1), 0f);
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
        Utilities.Instance.ANIM_Move(spawnedObj, optionPlacementPosition.position + (Vector3.down * spawnIndex), callBack: () => { SpawnOption(++spawnIndex); });
    }
    
#endregion
}
