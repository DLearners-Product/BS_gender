using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thumbnail5Controller : MonoBehaviour
{
    [SerializeField] List<CattleData> maleCattleData;
    [SerializeField] List<CattleData> femaleCattleData;
    [SerializeField] List<GameObject> forestEnvironmentObjs;
    [SerializeField] Transform animalDisplayPosition;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

#region ANIMATION

    void ShowFarm(Transform animalObj)
    {
        // Utilities.Instance.ANIM_Move(sourceObj, endPosition.position);
    }

#endregion

    [Serializable]
    class CattleData
    {
        public Sprite cattleSprite;
        public string cattleType;
    }
}

