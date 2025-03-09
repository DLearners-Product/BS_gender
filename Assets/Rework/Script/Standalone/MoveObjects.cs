using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveObjects : MonoBehaviour
{
    [SerializeField] Transform endPosition;
    [SerializeField] Vector3 endPoint;
    [SerializeField] float movementSpeed;
    [SerializeField] UnityEvent onComplete;

    delegate void UpdateDel();
    UpdateDel fixedUpDel;
    Vector3 _originalPosition, _endPosition;

    [ContextMenu("Capture Position")]
    private void CapturePosition()
    {
        endPoint = transform.position;
    }

    void Start()
    {
        _originalPosition = transform.position;
    }

    private void OnEnable() {
        _endPosition = (endPosition == null) ? endPoint : endPosition.position;
        fixedUpDel += MoveObjectToEndPosition;
    }

    private void OnDisable() {
        fixedUpDel -= MoveObjectToEndPosition;
    }

    void FixedUpdate()
    {
        fixedUpDel?.Invoke();
    }

    void MoveObjectToEndPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, _endPosition, movementSpeed * Time.deltaTime);

        Debug.Log($"Distance : {Vector3.Distance(_endPosition, transform.position)}", gameObject);

        if(Vector3.Distance(_endPosition, transform.position) <= 0.09f) { 
            Debug.Log("came to if condition"); 
            fixedUpDel -= MoveObjectToEndPosition; 
            onComplete?.Invoke();
        }
    }

    public void ReturnToOriginalPosition()
    {
        _endPosition = _originalPosition;
        fixedUpDel += MoveObjectToEndPosition;
    }
}
