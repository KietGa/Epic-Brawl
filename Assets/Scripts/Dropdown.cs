using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropdown : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetInt("teamMode", 0);
        PlayerPrefs.SetInt("gameMode", 2);
        PlayerPrefs.SetInt("nop", 6);
        PlayerPrefs.SetFloat("matchTime", 300);
        PlayerPrefs.SetInt("map", 0);
    }

    public void TeamMode(int val)
    {
        if (val == 0)
        {
            PlayerPrefs.SetInt("teamMode", 0);
        }
        else if (val == 1) 
        {
            PlayerPrefs.SetInt("teamMode", 1);
        }
    }

    public void GameMode(int val)
    {
        if (val == 0)
        {
            PlayerPrefs.SetInt("gameMode", 0);
        }
        else if (val == 1)
        {
            PlayerPrefs.SetInt("gameMode", 1);
        }
        else if (val == 2)
        {
            PlayerPrefs.SetInt("gameMode", 2);
        }
        else if (val == 3)
        {
            PlayerPrefs.SetInt("gameMode", 3);
        }
        else if (val == 4)
        {
            PlayerPrefs.SetInt("gameMode", 4);
        }
    }

    public void NumberOfPlayers(int val)
    {
        if (val == 0)
        {
            PlayerPrefs.SetInt("nop", 10);
        }
        else if (val == 1)
        {
            PlayerPrefs.SetInt("nop", 8);
        }
        else if (val == 2)
        {
            PlayerPrefs.SetInt("nop", 6);
        }
        else if (val == 3)
        {
            PlayerPrefs.SetInt("nop", 4);
        }
        else if (val == 4)
        {
            PlayerPrefs.SetInt("nop", 2);
        }
    }

    public void MatchTime(int val)
    {
        if (val == 0)
        {
            PlayerPrefs.SetFloat("matchTime", 420);
        }
        else if (val == 1)
        {
            PlayerPrefs.SetFloat("matchTime", 360);
        }
        else if (val == 2)
        {
            PlayerPrefs.SetFloat("matchTime", 300);
        }
        else if (val == 3)
        {
            PlayerPrefs.SetFloat("matchTime", 240);
        }
        else if (val == 4)
        {
            PlayerPrefs.SetFloat("matchTime", 180);
        }
    }

    public void Maps(int val)
    {
        if (val == 0)
        {
            PlayerPrefs.SetInt("map", 0);
        }
        else if (val == 1)
        {
            PlayerPrefs.SetInt("map", 1);
        }
        else if (val == 2)
        {
            PlayerPrefs.SetInt("map", 2);
        }
        else if (val == 3)
        {
            PlayerPrefs.SetInt("map", 3);
        }
    }
}
