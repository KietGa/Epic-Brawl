using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineLobbyManager : MonoBehaviour
{
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public static OnlineLobbyManager OLMInstace { get; private set; }

    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;
    public event EventHandler<OnLobbyListChangedArgs> OnLobbyListChanged;
    public class OnLobbyListChangedArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    private Lobby joinedLobby;
    private float heartbeatTimer;

    private void Awake()
    {
        OLMInstace = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private async void Init()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());
            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update()
    {
        HandleHeartBeat();
        HandleAutoResetRoomList();
    }

    private void HandleHeartBeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f)
            {
                float hearbeatTimerMax = 15f;
                heartbeatTimer = hearbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private void HandleAutoResetRoomList()
    {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn && SceneManager.GetActiveScene().name == "Lobby")
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f)
            {
                float hearbeatTimerMax = 3f;
                heartbeatTimer = hearbeatTimerMax;
                ListLobby();
            }
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    private void ShowMessage(string message)
    {
        messagePanel.SetActive(true);
        messageText.text = message;
    }

    public void CloseMessage()
    {
        messagePanel.SetActive(false);
    }

    private async Task<Allocation> AllocationRelay(int maxPlayer)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayer - 1);
            return allocation;
        }
        catch (Exception e)
        {
            print(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (Exception e)
        {
            print(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joincode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joincode);
            return joinAllocation;
        }
        catch (Exception e)
        {
            print(e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            int maxPlayer = PlayerPrefs.GetInt("nop", 10); 
            ShowMessage("Creating...");
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, new CreateLobbyOptions
            {
                IsPrivate = isPrivate
            });

            Allocation allocation = await AllocationRelay(maxPlayer);
            string relayJoinCode = await GetRelayJoinCode(allocation);
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("InLobby", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            ShowMessage("Failed to Create!");
            print(e);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            ShowMessage("Joining...");
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            ShowMessage("Failed to Join!");
            print(e);
        }
    }

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            ShowMessage("Joining...");
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            string relayCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            ShowMessage("Failed to Join!");
            print(e);
        }
    }

    public async void JoinWithId(string lobbyId)
    {
        try
        {
            ShowMessage("Joining...");
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string relayCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            ShowMessage("Failed to Join!");
            print(e);
        }
    }

    public async void DeleteLobby()
    {
        try
        {
            if (joinedLobby != null)
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
        }
        catch (Exception e)
        {
            print(e);
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private async void ListLobby()
    {
        
        QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
        {
            Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
        };
        QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
        OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedArgs
        {
            lobbyList = queryResponse.Results
        });
        

    }
}
