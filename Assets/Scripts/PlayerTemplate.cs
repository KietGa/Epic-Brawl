using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTemplate : MonoBehaviour
{
    [SerializeField] private Image teamColor;
    [SerializeField] private TextMeshProUGUI playerNameText;

    public void SetPlayer(string playerName)
    {
        playerNameText.text = playerName;
        teamColor.color = new Color(1, 0, 0, (float)155/(float)255);
    }

    public void SetTeam(int team)
    {
        if (team == 0)
        {
            teamColor.color = new Color(1, 0, 0, (float)155 / (float)255);
        }
        else if (team == 1)
        {
            teamColor.color = new Color(0, 1, 1, (float)155 / (float)255);
        }
        else if (team == 2)
        {
            teamColor.color = new Color((float)50/255, (float)50 / 255, (float)50 / 255, (float)155 / (float)255);
        }
    }
}
