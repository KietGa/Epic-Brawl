using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static GameManager;


public class Grenade : NetworkBehaviour
{
    public GameObject[] explodeFX;
    public AudioClip clip;
    public int damage {  get; set; }
    public float explodeRange {  get; set; } //20
    public float explodeTime { get; set; } //3
    private float timer;
    public int id;
    public int chargeSuperPerHit { get; set; }
    public int fxIndex { get; set; }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > explodeTime)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(explodeFX[fxIndex], transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explodeRange);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                if (IsServer)
                {
                    print("---");
                    print(collider.GetComponent<PlayerSetUp>().id);
                    print(id);
                    DamageClientRpc(damage, collider.GetComponent<PlayerSetUp>().id, id);
                    ChargeClientRpc(collider.GetComponent<PlayerSetUp>().id, id);
                }
            }
        }

        PlayAudioServerRpc();
        DestroyServerRpc();
    }

    [ClientRpc]
    private void ChargeClientRpc(int target, int id)
    {
        print((int)NetworkManager.LocalClientId);
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

    [ServerRpc(RequireOwnership = false)]
    public void PlayAudioServerRpc()
    {
        PlayAudioClientRpc();
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
    public void InitClientRpc(int id, int damage, float explodeRange, float explodeTime, int chargeSuperPerHit, int fxIndex)
    {
        this.id = id;
        this.damage = damage / 2;
        this.explodeRange = explodeRange;
        this.explodeTime = explodeTime;
        this.chargeSuperPerHit = chargeSuperPerHit / 2;
        this.fxIndex = fxIndex;
    }
}
