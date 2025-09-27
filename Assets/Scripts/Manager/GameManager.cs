using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager GMInstance { get; private set; }
    public Image dot;
    public GameObject[] mags;
    public Image gadgetCdImg;
    public Image superChargeImg;
    public Image healthBar;
    public Transform[] brawlers;
    public Transform[] spawners;
    public GameObject lowhpObject;
    public GameObject deathPanel;
    public GameObject inviciblePanel;
    public TextMeshProUGUI redTeamKillsText;
    public Image redTeamKillsImage;
    public TextMeshProUGUI blueTeamKillsText;
    public Image blueTeamKillsImage;
    private int killsRequire;
    [SerializeField] private GameObject endGamePanel;
    public bool isEnd;
    public GameObject teamKillAno;
    public Image teamKillerImage;
    public Image teamDeathImage;
    public TextMeshProUGUI killerName;
    public TextMeshProUGUI deathName;
    public NetworkVariable<int> redTeamKills = new NetworkVariable<int>(0);
    public NetworkVariable<int> blueTeamKills = new NetworkVariable<int>(0);
    public bool isStarted;
    public float gadgetTime;
    public int currentChargeSuper;
    public NetworkVariable<float> matchTime = new NetworkVariable<float>(100);
    [SerializeField] private TextMeshProUGUI matchTimeText;
    [SerializeField] private GameObject[] maps;
    [SerializeField] private Material[] skyBoxs;
    [SerializeField] private float rotateSpeed = 1.2f;

    private void Awake()
    {
        GMInstance = this;
        isStarted = true;
    }

    public override void OnNetworkSpawn()
    {
        switch (DataManager.DMInstance.gameMode.Value)
        {
            case 0:
                killsRequire = 20;
                break;

            case 1:
                killsRequire = 15; 
                break;

            case 2:
                killsRequire = 10;
                break;

            case 3:
                killsRequire = 5;
                break;

            case 4:
                killsRequire = 1;
                break;
        }

        redTeamKillsText.text = redTeamKills.Value + "/" + killsRequire;
        redTeamKillsImage.fillAmount = 0;
        blueTeamKillsText.text = blueTeamKills.Value + "/" + killsRequire;
        blueTeamKillsImage.fillAmount = 0;
        redTeamKills.OnValueChanged += RedTeamKills_OnValueChanged;
        blueTeamKills.OnValueChanged += BlueTeamKills_OnValueChanged;
        matchTime.OnValueChanged += MatchTime_OnValueChanged;

        if (IsServer)
        {
            MapClientRpc(PlayerPrefs.GetInt("map"));
            matchTime.Value = PlayerPrefs.GetFloat("matchTime", 300f);
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    [ClientRpc]
    private void MapClientRpc(int index)
    {
        if (index == 3)
        {
            RenderSettings.skybox = skyBoxs[0];
        }
        maps[index].SetActive(true);
    }

    private void Update()
    {
        if (IsServer && !isEnd) 
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
            matchTime.Value -= Time.deltaTime;
            if (matchTime.Value <= 0 && !isEnd)
            {
                matchTime.Value = 0;
                isEnd = true;

                if (redTeamKills.Value > blueTeamKills.Value)
                {
                    EndGameServerRpc(false);
                }
                else if (redTeamKills.Value < blueTeamKills.Value)
                {
                    EndGameServerRpc(true);
                }
                else
                {
                    DrawClientRpc();
                }
            }
        }
    }

    private void MatchTime_OnValueChanged(float previousValue, float newValue)
    {
        matchTimeText.text = Mathf.CeilToInt(matchTime.Value) + "";
    }

    private void BlueTeamKills_OnValueChanged(int previousValue, int newValue)
    {
        blueTeamKillsText.text = blueTeamKills.Value + "/" + killsRequire;
        blueTeamKillsImage.fillAmount = (float)blueTeamKills.Value / (float)killsRequire;
    }

    private void RedTeamKills_OnValueChanged(int previousValue, int newValue)
    {
        redTeamKillsText.text = redTeamKills.Value + "/" + killsRequire;
        redTeamKillsImage.fillAmount = (float)redTeamKills.Value / (float)killsRequire;
    }

    public void Respawn()
    {
        StartCoroutine(RespawnDelay());
    }

    private IEnumerator RespawnDelay()
    {
        deathPanel.SetActive(false);
        PointServerRpc();
        yield return new WaitForSeconds(3f);
        RespawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PointServerRpc(ServerRpcParams srp = default)
    {
        int clientId = (int)srp.Receive.SenderClientId;

        if (DataManager.DMInstance.teamList[(int)clientId] == 1)
        {
            redTeamKills.Value += 1;
            if (redTeamKills.Value == killsRequire && !isEnd)
            {
                isEnd = true;
                EndGameServerRpc(false);
            }
        }
        else
        {
            blueTeamKills.Value += 1;
            if (blueTeamKills.Value == killsRequire && !isEnd)
            {
                isEnd = true;
                EndGameServerRpc(true);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RespawnServerRpc(ServerRpcParams srp = default)
    {
        int clientId = (int)srp.Receive.SenderClientId;
        int ran = Random.Range(0, 5);

        if (DataManager.DMInstance.teamList[(int)clientId] == 1)
        {
            ran = Random.Range(5, 10);
        }

        Transform player = Instantiate(brawlers[DataManager.DMInstance.brawlList[clientId]], spawners[ran].position, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject((ulong)clientId, true);
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        int redPos = 0;
        int bluePos = 5;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (DataManager.DMInstance.teamList[(int)clientId] == 0)
            {
                Transform player = Instantiate(brawlers[DataManager.DMInstance.brawlList[(int)clientId]], spawners[redPos++].position, Quaternion.identity);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            }
            else
            {
                Transform player = Instantiate(brawlers[DataManager.DMInstance.brawlList[(int)clientId]], spawners[bluePos++].position, Quaternion.identity);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndGameServerRpc(bool isRealerWin)
    {
        EndGameClientRpc(isRealerWin);
    }

    [ClientRpc]
    private void DrawClientRpc()
    {
        Cursor.lockState = CursorLockMode.None;
        endGamePanel.SetActive(true);
        AudioManager.AMInstance.PlayAudio(4);
        endGamePanel.GetComponent<Image>().color = new Color(255, 255, 255, 100f / 255f);
        endGamePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Draw!";
        endGamePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 1);
    }

    [ClientRpc]
    private void EndGameClientRpc(bool isBlueTeamWin)
    {
        Cursor.lockState = CursorLockMode.None;
        endGamePanel.SetActive(true);

        if (isBlueTeamWin)
        {
            AudioManager.AMInstance.PlayAudio(2);
            endGamePanel.GetComponent<Image>().color = new Color(255, 255, 255, 100f / 255f);
            endGamePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Blue Team Win!";
            endGamePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0, 200, 255, 1);
        }
        else
        {
            AudioManager.AMInstance.PlayAudio(3);
            endGamePanel.GetComponent<Image>().color = new Color(255, 0, 0, 100f / 255f);
            endGamePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Red Team Win!";
            endGamePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
        }
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public IEnumerator KillAno(int sender, int death, int team)
    {
        AudioManager.AMInstance.PlayPitchAudio(5, 0.6f);
        GameManager.GMInstance.teamKillAno.SetActive(true);
        if (team == 0)
        {
            GameManager.GMInstance.teamKillAno.GetComponent<Image>().color = Color.cyan;
        }
        else
        {
            GameManager.GMInstance.teamKillAno.GetComponent<Image>().color = Color.red;
        }
        GameManager.GMInstance.teamKillerImage.sprite = DataManager.DMInstance.brawlerIcons[DataManager.DMInstance.brawlList[sender]];
        GameManager.GMInstance.teamDeathImage.sprite = DataManager.DMInstance.brawlerIcons[DataManager.DMInstance.brawlList[death]];
        GameManager.GMInstance.killerName.text = DataManager.DMInstance.nameList[sender].ToString();
        GameManager.GMInstance.deathName.text = DataManager.DMInstance.nameList[death].ToString();

        yield return new WaitForSeconds(3f);
        GameManager.GMInstance.teamKillAno.SetActive(false);
    }

    [ClientRpc]
    public void DeathAnoClientRpc(int sender, int death)
    {
        StartCoroutine(GameManager.GMInstance.KillAno(sender, death, DataManager.DMInstance.teamList[death]));
    }
}
