using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode;
using System.IO;

public class roomManager : MonoBehaviour
{

    public TMP_Text gameStatsText;

    public TMP_Text timerText; 
    [SerializeField] float maxStartTime = 5;
    [SerializeField] float minPlayersCount = 2;
    float StartTime;

    public levelButton levelButton;
    public Transform buttonsHolder;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        StartTime = maxStartTime;
    }


    // Update is called once per frame
    void Update()
    {
        hostManangment();
    }

    void hostManangment()
    {
        if(!NetworkManager.Singleton.IsClient)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        transform.GetChild(0).gameObject.SetActive(true);

        gameStatsText.gameObject.SetActive(true);

        if(!NetworkManager.Singleton.IsHost) return;
        
        if(NetworkManager.Singleton.ConnectedClients.Count < minPlayersCount)
        {
            gameStatsText.text = "players : " + NetworkManager.Singleton.ConnectedClients.Count + " / 2";
        }
        if(NetworkManager.Singleton.ConnectedClients.Count >= minPlayersCount)
        {
            //StartTime -= Time.deltaTime;
            //gameStatsText.text = "Starting in " + StartTime.ToString("0");

            if(!NetworkManager.Singleton.IsHost) return;
            //if(StartTime <= 0)
            loadLevelButtons();
        }
    }

    bool loadedLevels;
    void loadLevelButtons()
    {
        if(loadedLevels) return;

        buttonsHolder.gameObject.SetActive(true);

        DirectoryInfo dir = new DirectoryInfo("Assets/Levels");
        FileInfo[] info = dir.GetFiles("*.unity");
        

        foreach(FileInfo scene in info)
        {
            levelButton button = Instantiate(levelButton, buttonsHolder);
            button.name = Path.GetFileNameWithoutExtension(scene.Name);
        }

        loadedLevels = true;
    }

    void startGame()
    {
        lobbyManager.Instance.UpdateJoinedLobby();
        //NetworkManager.Singleton.SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }

}
