using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour
{
    public int id;
    public int team;
    public TextMeshPro nameText;

    public override void OnNetworkSpawn()
    {
        id = (int)OwnerClientId;
        team = DataManager.DMInstance.teamList[id];
        nameText = transform.GetChild(4).GetComponent<TextMeshPro>();
        nameText.text = DataManager.DMInstance.nameList[id].ToString();
        if (team == 0)
        {
            nameText.color = Color.red;
        }
        else
        {
            nameText.color = Color.cyan;
        }

        if (IsOwner)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
            GetComponent<AudioListener>().enabled = true;
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<PlayerView>().enabled = true;
            GetComponent<PlayerSkill>().enabled = true;        
            GetComponent<PlayerDeath>().enabled = true;
        }
    }
}
