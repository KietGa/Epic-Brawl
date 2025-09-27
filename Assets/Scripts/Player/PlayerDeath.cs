using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeath : NetworkBehaviour
{
    private bool isDeath;

    private void Start()
    {
        StartCoroutine(Invicible());
    }

    private IEnumerator Invicible()
    {
        GameManager.GMInstance.inviciblePanel.SetActive(true);
        yield return new WaitForSeconds(GetComponent<PlayerHealth>().invicibleTime);
        GameManager.GMInstance.inviciblePanel.SetActive(false);
    }

    void Update()
    {
        if (GetComponent<PlayerHealth>().currentHealth <= GetComponent<PlayerHealth>().maxHealth / 4 && GetComponent<PlayerHealth>().currentHealth > 0)
        {
            GameManager.GMInstance.lowhpObject.SetActive(true);
            GameManager.GMInstance.lowhpObject.GetComponent<Image>().color = new Color(1, 0, 0, (float)60 / (float)255);
        }
        else if (GetComponent<PlayerHealth>().currentHealth <= GetComponent<PlayerHealth>().maxHealth * 5 / 10 && GetComponent<PlayerHealth>().currentHealth > 0)
        {
            GameManager.GMInstance.lowhpObject.SetActive(true);
            GameManager.GMInstance.lowhpObject.GetComponent<Image>().color = new Color(1, 0, 0, (float)30 / (float)255);
        }
        else
        {
            GameManager.GMInstance.lowhpObject.SetActive(false);
        }

        if (GetComponent<PlayerHealth>().currentHealth <= 0 && !isDeath)
        {
            isDeath = true;
            Death();
        }
    }

    private void Death()
    {
        GameManager.GMInstance.Respawn();
        DestroyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeathAnoServerRpc(int sender, int death)
    {
        GameManager.GMInstance.DeathAnoClientRpc(sender, death);
    }


    [ServerRpc(RequireOwnership = false)]

    private void DestroyServerRpc()
    {
        Destroy(gameObject);
        GetComponent<NetworkObject>().Despawn();
    }
}
