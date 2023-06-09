using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenTrap : MonoBehaviour
{
    [SerializeField] private GameObject Spike;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other)
        {
            Spike.gameObject.SetActive(true);
        }
    }
}
