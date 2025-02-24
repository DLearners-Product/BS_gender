using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Thumbnail7Controller : MonoBehaviour
{
    public Transform maleOptionSpawnPoint,
                    femaleOptionSpawnPoint;
    public GameObject optionDisplayObj;
    public Sprite[] maleSprites,
                    femaleSprites;

    public GameObject seesawObj;
    public Image maledisplayObj, femaledisplayObj;
    public List<MaleFemalePair> matchAnswers;

    Transform[] _maleoptionSpawnPoints,
                _femaleoptionSpawnPoints;
    bool _leanOnRight = true,
            _maleSelected = false,
            _femaleSelected = false;
    Transform _selectedMaleObj,
                _selectedFemaleObj;
    Transform[] _maleSpawnedOptions,
                _femaleSpawnedOptions;
    Vector3 _selectedMaleOrgPos,
                _selectedFemaleOrgPos;

    void Start()
    {
        _maleSpawnedOptions = new Transform[maleSprites.Length];
        _femaleSpawnedOptions = new Transform[femaleSprites.Length];

        GetChildObjs(maleOptionSpawnPoint, ref _maleoptionSpawnPoints);
        GetChildObjs(femaleOptionSpawnPoint, ref _femaleoptionSpawnPoints);
        SpawnOptions(maleOptionSpawnPoint, maleSprites, _maleoptionSpawnPoints);
        SpawnOptions(femaleOptionSpawnPoint, femaleSprites, _femaleoptionSpawnPoints);
    }

    void GetChildObjs(Transform parentObj, ref Transform[] traformArr)
    {
        int childCount = parentObj.childCount;
        traformArr = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            traformArr[i] = parentObj.GetChild(i);
        }
    }

    void SpawnOptions(Transform parentObj, Sprite[] spawnSprites, Transform[] spawnPositions, int index = 0)
    {
        if(index == spawnSprites.Length) return;

        int _childCount = parentObj.childCount;
        // for (int i = 0; i < _childCount; i++)
        // {
        var spawnedObj = Instantiate(optionDisplayObj, parentObj);
        spawnedObj.transform.position = Vector3.zero;
        spawnedObj.transform.GetChild(0).GetComponent<Image>().sprite = spawnSprites[index];
        spawnedObj.GetComponent<Button>().onClick.AddListener(OnOptionClicked);
        Utilities.Instance.ANIM_Move(spawnedObj.transform, spawnPositions[index].position, callBack: () => {
            spawnedObj.GetComponent<FloatingObject>().enabled = true; 
            SpawnOptions(parentObj, spawnSprites, spawnPositions, ++index);
        });
        // }
    }

    void AssignImageDisplay(Image sourceObj, Image destinationObj)
    {
        destinationObj.sprite = sourceObj.sprite;
    }

    MaleFemalePair GetPair(string malePairName)
    {
        int _count = matchAnswers.Count;
        for (int i = 0; i < _count; i++)
        {
            if(malePairName.Contains(matchAnswers[i].maleAnimal))
            {
                return matchAnswers[i];
            }
        }
        return null;
    }

    void EvaluateAnswer()
    {
        if(!_maleSelected || !_femaleSelected) return;

        var malePairName = maledisplayObj.sprite.name;
        var feMalePairName = femaledisplayObj.sprite.name;

        var pairObj = GetPair(malePairName);

        if(feMalePairName.Contains(pairObj.femalePairAnimal))
        {
            
        }else{ ReleaseSelectedObjs(); }
    }

    void ReleaseSelectedObjs()
    {
        _selectedMaleObj.gameObject.SetActive(true);
        _selectedFemaleObj.gameObject.SetActive(true);

        maledisplayObj.gameObject.SetActive(false);
        femaledisplayObj.gameObject.SetActive(false);

        _maleSelected = false;
        _femaleSelected = false;

        Utilities.Instance.ANIM_Move(_selectedMaleObj, _selectedMaleOrgPos);
        Utilities.Instance.ANIM_Move(_selectedFemaleObj, _selectedFemaleOrgPos);
    }

    void PlaySeeSaw()
    {
        if(_leanOnRight){
            Utilities.Instance.ANIM_RotateObj(seesawObj.transform, new Vector3(0, 0, -10));
            _leanOnRight = false;
        }else{
            Utilities.Instance.ANIM_RotateObj(seesawObj.transform, new Vector3(0, 0, 10));
            _leanOnRight = true;
        }
    }

    bool IsMaleObject(string matchSTR)
    {
        string pattern = @"\bMale\w";
        Regex rg = new Regex(pattern);
        return rg.Match(matchSTR).Success;
    }

#region OnButtonClickListener
    public void OnOptionClicked()
    {
        var clickedObj = EventSystem.current.currentSelectedGameObject.transform;
        clickedObj.GetComponent<FloatingObject>().enabled = false;
        Image destinationObj = null;

        var parentName = clickedObj.transform.parent.name;

        if(IsMaleObject(parentName))
        {
            _selectedMaleOrgPos = clickedObj.position;
            _selectedMaleObj = clickedObj;
            _maleSelected = true;
            destinationObj = maledisplayObj;
        }else{
            _selectedFemaleOrgPos = clickedObj.position;
            _selectedFemaleObj = clickedObj;
            _femaleSelected = true;
            destinationObj = femaledisplayObj;
        }

        Utilities.Instance.ANIM_Move(clickedObj, destinationObj.transform.position, callBack: () => {
            AssignImageDisplay(clickedObj.transform.GetChild(0).GetComponent<Image>(), destinationObj);
            destinationObj.gameObject.SetActive(true);
            clickedObj.gameObject.SetActive(false);
            Invoke(nameof(EvaluateAnswer), 0.5f);
        });
    }
#endregion

[Serializable]
public class MaleFemalePair
{
    public string maleAnimal;
    public string femalePairAnimal;
}
}
