using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using static GameManager;

public class Knife : NetworkBehaviour
{
    public AudioClip clip;
    public int damage {  get; set; }
    public int id;
    public int chargeSuperPerHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsServer)
            {
                DamageClientRpc(damage, other.GetComponent<PlayerSetUp>().id, id);
                ChargeClientRpc(other.GetComponent<PlayerSetUp>().id, id);
                PlayAudioClientRpc();
            }
        }

        DestroyServerRpc();
    }

    [ClientRpc]
    private void ChargeClientRpc(int target, int id)
    {
        if (id == (int)NetworkManager.LocalClientId && DataManager.DMInstance.teamList[target] != DataManager.DMInstance.teamList[id])
        {
            GMInstance.currentChargeSuper -= chargeSuperPerHit;
        }
    }

    [ClientRpc]
    private void DamageClientRpc(int damage, int target, int sender)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerSetUp>().id == target)
            {
                if (DataManager.DMInstance.teamList[sender] != DataManager.DMInstance.teamList[target])
                {
                    player.GetComponent<PlayerHealth>().LostHP(damage, sender);
                    break;
                }
            }
        }
    }

    [ClientRpc]
    protected void PlayAudioClientRpc()
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyServerRpc()
    {
        Destroy(gameObject);
        GetComponent<NetworkObject>().Despawn();
    }

    [ClientRpc]
    public void InitClientRpc(int id, int damage, int chargeSuperPerHit)
    {
        this.id = id;
        this.damage = damage;
        this.chargeSuperPerHit = chargeSuperPerHit;
    }
}
