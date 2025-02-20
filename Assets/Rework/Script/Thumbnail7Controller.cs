using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Thumbnail7Controller : MonoBehaviour
{
    public Transform maleOptionSpawnPoint,
                    femaleOptionSpawnPoint;
    public GameObject optionDisplayObj;
    public Sprite[] maleSprites,
                    femaleSprites;
    Transform[] _maleoptionSpawnPoints,
                _femaleoptionSpawnPoints;

    void Start()
    {
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

    void SpawnOptions(Transform parentObj, Sprite[] spawnSprites, Transform[] spawnPositions)
    {
        int _childCount = parentObj.childCount;
        for (int i = 0; i < _childCount; i++)
        {
            var spawnedObj = Instantiate(optionDisplayObj, parentObj);
            spawnedObj.transform.position = spawnPositions[i].position;
            spawnedObj.GetComponent<FloatingObject>().enabled = true;
            spawnedObj.transform.GetChild(0).GetComponent<Image>().sprite = spawnSprites[i];
        }
    }

    void Update()
    {
        
    }
}
