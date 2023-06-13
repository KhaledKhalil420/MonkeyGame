using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class lobbyPlayerObject : MonoBehaviour
{
    public string name;

    public TMP_Text nameText;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = name;
    }
}
