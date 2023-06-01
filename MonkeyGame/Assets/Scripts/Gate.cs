using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, ITriggerable
{
    private bool IsOpen = false;
    [SerializeField] private float LerpSpeed;
    [SerializeField] private Vector2 Offset;
    private Vector2 StartingPosition;
    private Vector2 NewOffset;

    private void Start()
    {
        StartingPosition = transform.position;
        NewOffset = StartingPosition + Offset;  
    }

    public void Trigger()
    {
        IsOpen = !IsOpen;
    }


    private void FixedUpdate()
    {
        if(IsOpen)
        {
            transform.position = Vector2.Lerp(transform.position, NewOffset, LerpSpeed * Time.fixedDeltaTime);
        }

        if(!IsOpen)
        {
            transform.position = Vector2.Lerp(transform.position, StartingPosition, LerpSpeed * Time.fixedDeltaTime);
        }
    }
}
