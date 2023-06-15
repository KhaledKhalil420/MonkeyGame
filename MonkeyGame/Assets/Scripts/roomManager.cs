using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode;
using System.IO;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class roomManager : MonoBehaviour
{
    [SerializeField] TMP_Text playersInGame; 
    [SerializeField] float maxStartTime = 5;
    [SerializeField] float minPlayersCount = 2;
    float StartTime;

    [SerializeField] levelButton levelButton;
    [SerializeField] Transform buttonsHolder;

    [SerializeField] Transform playerHolder;
    [SerializeField] lobbyPlayerObject player;

    [SerializeField] Animator anim;

    [SerializeField] bool developerMode;

    [SerializeField] TMP_InputField codeField;
    
    

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
        checkIfConnected();
    }

    bool connected;
    void checkIfConnected()
    {
        connected = NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost;

        if(connected)
        {
            anim.SetBool("Connected", true);
            codeField.text = lobbyManager.Instance.currentLobby.LobbyCode;
        }
    }

    void hostManangment()
    {
        if(!NetworkManager.Singleton.IsClient)
        {
            //transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        //transform.GetChild(0).gameObject.SetActive(true);

        //gameStatsText.gameObject.SetActive(true);

        if(!NetworkManager.Singleton.IsHost) return;
        
        if(NetworkManager.Singleton.ConnectedClients.Count < minPlayersCount)
        {
            playersInGame.text = "players : " + NetworkManager.Singleton.ConnectedClients.Count + " / " + maxStartTime;
        }
        if(NetworkManager.Singleton.ConnectedClients.Count >= minPlayersCount)
        {
            //StartTime -= Time.deltaTime;
            //gameStatsText.text = "Starting in " + StartTime.ToString("0");

            if(!NetworkManager.Singleton.IsHost) return;
            if(developerMode)
            loadLevelButtons();
            else
            loadPlayers();
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

    bool loadedPlayers;
    void loadPlayers()
    {
        anim.SetTrigger("startGame");
        if(loadedPlayers) return;

        buttonsHolder.gameObject.SetActive(true);

        foreach(Player lobbyPlayer in lobbyManager.Instance.currentLobby.Players)
        {
            lobbyPlayerObject p = Instantiate(player, playerHolder);
            p.name = lobbyPlayer.Data["playerName"].Value;            
        }

        loadedPlayers = true;
    }


    public void startGame()
    {
        lobbyManager.Instance.loadNextRound();
    }
}
