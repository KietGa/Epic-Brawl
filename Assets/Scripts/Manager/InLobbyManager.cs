using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InLobbyManager : NetworkBehaviour
{
    public static InLobbyManager ILMInstance {  get; private set; }
    public Transform playerContainer;
    public Transform playerTemplate;
    public TextMeshProUGUI anoText;
    [SerializeField] private GameObject startBtn;
    public GameObject pickTeamObject;
    [SerializeField] private float refreshTime = 1f;
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyCode;
    private float currentTime;

    private void Awake()
    {
        ILMInstance = this;
    }

    private void Start()
    {
        playerTemplate.gameObject.SetActive(false);

        Lobby lobby = OnlineLobbyManager.OLMInstace.GetLobby();
        lobbyName.text = "Lobby Name: " + lobby.Name;
        lobbyCode.text = "Lobby Code: " + lobby.LobbyCode;

        if (IsServer)
        {
            startBtn.SetActive(true);
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > refreshTime)
        {
            currentTime = 0;
            DataManager.DMInstance.ListResetServerRpc();
        }
    }

    public void StartGame()
    {
        if (IsServer)
        {
            if (DataManager.DMInstance.teamMode.Value == 1)
            {
                DataManager.DMInstance.RandomTeamServerRpc();
            }
            OnlineLobbyManager.OLMInstace.DeleteLobby();
            NetworkManager.Singleton.SceneManager.LoadScene("Waiting", LoadSceneMode.Single);
        }
    }

    public void QuitRoom()
    {
        if (OnlineLobbyManager.OLMInstace.IsLobbyHost())
        {
            OnlineLobbyManager.OLMInstace.DeleteLobby();
        }
        else
        {
            DataManager.DMInstance.ListRemoveServerRpc();
            OnlineLobbyManager.OLMInstace.LeaveLobby();
        }

        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }
}
