using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode;

public class levelButton : MonoBehaviour
{
    public string name;
    [SerializeField] TMP_Text nameText;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = name;
    }

    public void SelectLevel()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(name, LoadSceneMode.Single);
    }
}
