using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPlayerTemplate : MonoBehaviour
{
    [SerializeField] private Image teamColor;
    [SerializeField] private TextMeshProUGUI playerNameText;

    public void SetPlayer(string playerName)
    {
        playerNameText.text = playerName;
        teamColor.sprite = DataManager.DMInstance.brawlerIcons[0];
    }

    public void SetBrawl(int brawl)
    {
        teamColor.sprite = DataManager.DMInstance.brawlerIcons[brawl];
    }
}
