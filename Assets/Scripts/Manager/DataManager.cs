using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : NetworkBehaviour
{
    public static DataManager DMInstance { get; private set; }
    public NetworkList<int> brawlList;
    public NetworkList<FixedString128Bytes> nameList;
    public NetworkList<int> teamList;
    public NetworkVariable<int> teamMode = new NetworkVariable<int>(0);
    public NetworkVariable<int> gameMode = new NetworkVariable<int>(0);
    public NetworkVariable<int> maxPlayers = new NetworkVariable<int>(0);
    public NetworkVariable<int> map = new NetworkVariable<int>(0);
    public NetworkVariable<float> matchTime = new NetworkVariable<float>(0);
    public Sprite[] brawlerIcons;
    private string ano;


    private void Awake()
    {
        DMInstance = this;
        brawlList = new NetworkList<int>();
        nameList = new NetworkList<FixedString128Bytes>();
        teamList = new NetworkList<int>();
    }

    private void Start()
    {
        DontDestroyOnLoad(this);

        if (IsServer)
        {
            teamMode.Value = PlayerPrefs.GetInt("teamMode", 0);
            gameMode.Value = PlayerPrefs.GetInt("gameMode", 0);
            maxPlayers.Value = PlayerPrefs.GetInt("nop", 0);
            map.Value = PlayerPrefs.GetInt("map", 0);
            matchTime.Value = PlayerPrefs.GetFloat("matchTime", 0);
        }

        if (teamMode.Value == 1)
        {
            InLobbyManager.ILMInstance.pickTeamObject.SetActive(false);
        }

        switch (teamMode.Value)
        {
            case 0:
                ano += "Teammate Choosen";
                break;

            case 1:
                ano += "Teammate Random";
                break;
        }

        switch (gameMode.Value)
        {
            case 0:
                ano += " - Death Match 20\n";
                break;

            case 1:
                ano += " - Death Match 15\n";
                break;

            case 2:
                ano += " - Death Match 10\n";
                break;

            case 3:
                ano += " - Death Match 5\n";
                break;

            case 4:
                ano += " - Death Match 1\n";
                break;
        }

        ano += maxPlayers.Value + "P - ";
        ano += matchTime.Value + "s - ";

        switch (map.Value)
        {
            case 0:
                ano += "Green Field";
                break;

            case 1:
                ano += "Five Lands";
                break;

            case 2:
                ano += "Zero Block";
                break;
        }

        InLobbyManager.ILMInstance.anoText.text = ano;
        string playerName = PlayerPrefs.GetString("playerName", "Player" + Random.Range(100, 1000));
        print(playerName);
        ListAddServerRpc(playerName);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ListAddServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        nameList.Add(playerName);
        brawlList.Add(0);
        if (teamMode.Value == 0)
        {
            teamList.Add(0);
        }
        else
        {
            teamList.Add(2);
        }
        ListAddClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ListRemoveServerRpc(ServerRpcParams serverRpcParams = default)
    {
        print(serverRpcParams.Receive.SenderClientId);
        int id = (int)serverRpcParams.Receive.SenderClientId;
        nameList[id] = "NULL1211";
        ListAddClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ListResetServerRpc()
    {
        ListAddClientRpc();
    }

    [ClientRpc]
    private void ListAddClientRpc()
    {
        if (InLobbyManager.ILMInstance.playerContainer != null)
        {
            foreach (Transform child in InLobbyManager.ILMInstance.playerContainer)
            {
                if (child == InLobbyManager.ILMInstance.playerTemplate) continue;
                Destroy(child.gameObject);
            }

            for (int i = 0; i < nameList.Count; i++)
            {
                if (nameList[i] != "NULL1211")
                {
                    Transform lobbyTransform = Instantiate(InLobbyManager.ILMInstance.playerTemplate, InLobbyManager.ILMInstance.playerContainer);
                    lobbyTransform.gameObject.SetActive(true);
                    lobbyTransform.GetComponent<PlayerTemplate>().SetPlayer(nameList[i].ToString());
                    lobbyTransform.GetComponent<PlayerTemplate>().SetTeam(teamList[i]);
                }
            }
        }
    }

    public void SetTeam(int team)
    {
        SetTeamServerRpc(team);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RandomTeamServerRpc()
    {
        for (int i = 0; i < teamList.Count; i++)
        {
            teamList[i] = 0;
        }

        List<int> tempTeamList = new List<int>();
        int total = teamList.Count / 2;

        if (teamList.Count % 2 != 0)
        {
            int ran = Random.Range(1, 3);
            if (ran == 1)
            {
                total++;
            }
        }

        for (int i = 0; i < total; i++)
        {
            int ran = Random.Range(0, teamList.Count);

            foreach (int j in tempTeamList)
            {
                while (j == ran)
                {
                    ran = Random.Range(0, teamList.Count);
                }
            }

            tempTeamList.Add(ran);
        }

        for (int i = 0; i < tempTeamList.Count; i++)
        {
            teamList[tempTeamList[i]] = 1;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetBrawlServerRpc(int brawlId, ServerRpcParams serverRpcParams = default)
    {
        brawlList[(int)serverRpcParams.Receive.SenderClientId] = brawlId;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetTeamServerRpc(int team, ServerRpcParams serverRpcParams = default)
    {
        teamList[(int)serverRpcParams.Receive.SenderClientId] = team;
    }
}
