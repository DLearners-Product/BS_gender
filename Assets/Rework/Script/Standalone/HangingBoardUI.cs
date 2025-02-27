using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HangingBoardUI : MonoBehaviour
{
    private void Start() {
        transform.DORotate(new Vector3(0, 0, 5f), 1f)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }
}
