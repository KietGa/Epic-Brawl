using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_InputField roomCodeInput;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;
    private string playerName;

    private void Awake()
    {
        lobbyTemplate.gameObject.SetActive(false);
        playerName = PlayerPrefs.GetString("playerName", "Player" + Random.Range(100, 1000));
    }

    private void Start()
    {
        playerNameInput.text = playerName;
        playerNameInput.onValueChanged.AddListener((string newName) =>
        {
            SetPlayerName(newName);
        });

        OnlineLobbyManager.OLMInstace.OnLobbyListChanged += OLMInstace_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void OLMInstace_OnLobbyListChanged(object sender, OnlineLobbyManager.OnLobbyListChangedArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void OnDestroy()
    {
        OnlineLobbyManager.OLMInstace.OnLobbyListChanged -= OLMInstace_OnLobbyListChanged;
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyTemplate>().SetLobby(lobby);
        }
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        PlayerPrefs.SetString("playerName", playerName);
    }

    public void CreateRoomPublic()
    {
        OnlineLobbyManager.OLMInstace.CreateLobby(roomNameInput.text, false);
    }

    public void CreateRoomPrivate()
    {
        OnlineLobbyManager.OLMInstace.CreateLobby(roomNameInput.text, true);
    }

    public void CreateRoom()
    {
        lobbyPanel.SetActive(false);
        createRoomPanel.SetActive(true);
    }

    public void Back()
    {
        lobbyPanel.SetActive(true);
        createRoomPanel.SetActive(false);
    }

    public void QuickJoin()
    {
        OnlineLobbyManager.OLMInstace.QuickJoin();
    }

    public void JoinWithCode()
    {
        OnlineLobbyManager.OLMInstace.JoinWithCode(roomCodeInput.text);
    }

    public void Menu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }
}
