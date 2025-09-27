using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameText;

    public void SetLobby(Lobby lobby)
    {
        roomNameText.text = lobby.Name;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnlineLobbyManager.OLMInstace.JoinWithId(lobby.Id);
        });
    }
}
