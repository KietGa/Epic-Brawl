using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;

    private void Awake()
    {
        playAgainButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        });
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        Hide();
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (clientId == NetworkManager.ServerClientId && !GameManager.GMInstance.isEnd)
            {
                Show();
            }
        }
        else
        {
            if (clientId == NetworkManager.ServerClientId)
            {
                Show();
            }
        }
    }

    private void Show()
    {
        Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}