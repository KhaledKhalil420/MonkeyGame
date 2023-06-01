using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode;

public class roomManager : MonoBehaviour
{

    public TMP_Text gameStatsText;

    public TMP_Text timerText; 
    [SerializeField] float maxStartTime = 5;
    [SerializeField] float minPlayersCount = 2;
    float StartTime;


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
            StartTime -= Time.deltaTime;
            gameStatsText.text = "Starting in " + StartTime.ToString("0");

            if(!NetworkManager.Singleton.IsHost) return;
            if(StartTime <= 0)
            startGame();
        }
    }


    void startGame()
    {
        lobbyManager.Instance.UpdateJoinedLobby();
        NetworkManager.Singleton.SceneManager.LoadScene("Khaled", LoadSceneMode.Single);
    }

}
