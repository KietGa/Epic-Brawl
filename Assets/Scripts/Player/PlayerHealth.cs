using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static AudioManager;
using static GameManager;


public class PlayerHealth : NetworkBehaviour
{
    public int maxHealth = 6000;
    public int currentHealth;
    public float invicibleTime = 3f;
    public float currentTime;
    private float waitTime = 5f;
    private float healTime = 1f;
    private bool isInvicible;

    private void Start()
    {
        currentHealth = maxHealth;
        GMInstance.healthBar.fillAmount = 1;
        StartCoroutine(Invicible());
    }

    private IEnumerator Invicible()
    {
        isInvicible = true;
        yield return new WaitForSeconds(invicibleTime);
        isInvicible = false;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentHealth < maxHealth)
        {
            if (currentTime >= waitTime + healTime)
            {
                currentTime = waitTime;
                HealHP(maxHealth / 10);
                AMInstance.PlayAudio(1, 0.8f);
            }
        }
    }

    public void LostHP(int hp, int sender)
    {
        if (!isInvicible)
        {
            currentTime = 0;
            currentHealth -= hp;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            if (currentHealth <= 0)
            {
                int death = GetComponent<PlayerSetUp>().id;
                GetComponent<PlayerDeath>().DeathAnoServerRpc(sender, death);
            }
            transform.GetChild(3).GetChild(1).GetComponent<Image>().fillAmount = (float)currentHealth / (float)maxHealth;
            if (IsOwner)
            {
                GMInstance.healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
            }
        }
    }

    public void HealHP(int hp)
    {
        currentHealth += hp;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        transform.GetChild(3).GetChild(1).GetComponent<Image>().fillAmount = (float)currentHealth / (float)maxHealth;
        if (IsOwner)
        {
            GMInstance.healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
        }
    }
}
