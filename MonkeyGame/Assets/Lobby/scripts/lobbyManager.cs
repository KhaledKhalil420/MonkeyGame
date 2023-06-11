using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using System.IO;

public class lobbyManager : MonoBehaviour
{
    public TMP_InputField nameFieald;

    public Lobby currentLobby;
    Lobby currJoinedLobby;
    private float hearbeatTimer;

    public Transform roomsHolder;
    public GameObject roomObject;

    private List<GameObject> roomsList = new List<GameObject>();
    
    public static string playerName;

    public static lobbyManager Instance;
    
    List<GameObject> players = new List<GameObject>();

    float GetLobbyTime = 0;

    

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


    bool offline;


    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(NetworkManager.Singleton.IsHost)
            HandleLobbyHeartBeat();

        if(GetLobbyTime > 0)
        {
            GetLobbyTime -= Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.R)) pickRandomLevels();
    }

    private async void HandleLobbyHeartBeat()
    {
        if(currentLobby != null)
        {
            hearbeatTimer -= Time.deltaTime;
            if(hearbeatTimer < 0)
            {
                float hearbeatMaxTimer = 15;
                hearbeatTimer = hearbeatMaxTimer;

               await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
        }
    } 

    public async void HostAsync()
    {
        try
        {
            string lobbyName = "A lobby";
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions();
            UpdatePlayerOptions playerOptions = new UpdatePlayerOptions();
            options.IsPrivate = false;

            
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

            string RelayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "RelayCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member, value: RelayCode
                    )
                }
            };

            playerOptions.Data = new Dictionary<string, PlayerDataObject>()
            {
                {
                    "playerName", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member, value: playerName)
                },
                {
                    "playerId", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member, value: "0")
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            string playerId = AuthenticationService.Instance.PlayerId;
            lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, playerOptions);

            currentLobby = lobby;

            NetworkManager.Singleton.StartHost();
            Debug.Log("Creadet lobby " + lobby.Name + RelayCode);
            playerOptions.Data["playerId"].Value = NetworkManager.Singleton.LocalClientId.ToString();

            
            
        }
        catch
        {

        }
    }

    public async void JoinLobby(Lobby lobby)
    {
        try
        {
            UpdatePlayerOptions playerOptions = new UpdatePlayerOptions();
            playerOptions.Data = new Dictionary<string, PlayerDataObject>()
            {
                {
                    "playerName", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member, value: playerName)
                },
                {
                    "playerId", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member, value: "0")
                }
            };
            

            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string playerId = AuthenticationService.Instance.PlayerId;
            joinedLobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, playerOptions);
            string JoinCode = joinedLobby.Data["RelayCode"].Value;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(JoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );
            currentLobby = joinedLobby;
            NetworkManager.Singleton.StartClient();
            playerOptions.Data["playerId"].Value = NetworkManager.Singleton.LocalClientId.ToString();
            Debug.Log(joinedLobby.Name + JoinCode);

        }
        catch
        {

        }
    }


    public async void joinRandomRoom()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            if(queryResponse.Results.Count > 0)
            {
                UpdatePlayerOptions playerOptions = new UpdatePlayerOptions();
                playerOptions.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "playerName", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member, value: playerName)
                    },
                    {
                        "playerId", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member, value: "0")
                    }
                };

                Lobby joinedLobby = await Lobbies.Instance.QuickJoinLobbyAsync();
                string playerId = AuthenticationService.Instance.PlayerId;
                joinedLobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, playerOptions);
                string JoinCode = joinedLobby.Data["RelayCode"].Value;
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(JoinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                );
                currentLobby = joinedLobby;
                NetworkManager.Singleton.StartClient();
                Debug.Log(joinedLobby.Name + JoinCode);
                playerOptions.Data["playerId"].Value = NetworkManager.Singleton.LocalClientId.ToString();

            }
            else
            {
                HostAsync();
            }

        }
        catch (Exception e) 
        {
            Debug.Log(e);
        }
    }

    public async void ListLobby()
    {
        try
        {
        foreach(GameObject room in roomsList)
        {
            Destroy(room);
        }

        roomsList.Clear();

        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
        

        Debug.Log("Lobbies found: " + queryResponse.Results.Count);
        foreach(Lobby lobby in queryResponse.Results)
        {
            GameObject room = Instantiate(roomObject, roomsHolder);
            //room.GetComponent<RoomObject>().lobby = lobby;
            roomsList.Add(room);
        }
        }
        catch
        {
            
        }
    }

    public Lobby GetJoinedLobby()
    {
        UpdateJoinedLobby();
        return currJoinedLobby;
    }

    public int GetPlayersCount()
    {
       UpdateJoinedLobby();
       return currJoinedLobby.Players.Count;
    }

    public async void UpdateJoinedLobby()
    {
        try
        {
            if(GetLobbyTime <= 0)
            {
                if(currentLobby == null) return;
                currJoinedLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id.ToString());
                GetLobbyTime = 5;
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void spawnNetWorkObject(NetworkObject objectToSpawm)
    {
        objectToSpawm.Spawn();
    }

    public async void Authenticate()
    {
        if(nameFieald.text != null)
        {
            playerName = nameFieald.text;

            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(playerName);

            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Sighn in" + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            nameFieald.transform.parent.gameObject.SetActive(false);
        }
    }

    #region gameRounds

    int curRound;


    int [] numbers = new int[3];
    void pickRandomLevels()
    {
       //having an int Array of 3 members
       //using a string variable to keep record of the used numbers
       string usedNumbers ="-";

       //cycle to get 10 random numbers
       for (int i = 0 ; i<3; i++)
       {
        //get a random number between 0 - 120, 0 and 120 included on the random numbers
        int randomNumber = UnityEngine.Random.Range(0,scenesFiles().Length);
        //Checking if the random number you get is unique, if you already have it in the string means that this random number is repeated, the "-X-" is to make the difference between the numbers, like -1- from -11-, if you dont have the "-X-" the 1 is in 21 and would be a "repeated" number
        while(usedNumbers.Contains("-"+randomNumber.ToString()+"-"))
        {
             //if a number is repeated, then get a new random number
             randomNumber = UnityEngine.Random.Range(0,scenesFiles().Length);
        }
        usedNumbers+= randomNumber.ToString()+"-";
        numbers[i] = randomNumber;
        }

        foreach(int i in numbers)
        {
            Debug.Log(i);
        }
    }

    public void loadNextRound()
    {
        pickRandomLevels();

        string sceneName = Path.GetFileNameWithoutExtension(scenesFiles()[numbers[curRound]].Name);
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    FileInfo[] scenesFiles()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Levels");
        return dir.GetFiles("*.unity");
    }
        
    #endregion
}
