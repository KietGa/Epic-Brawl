using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AudioManager;
using Unity.Netcode;


public class Shin : PlayerSkill
{
    [SerializeField] private float superDuration = 5f;
    [SerializeField] private float superSpeedUp = 10f;
    [SerializeField] private float baseForce = 100f;
    [SerializeField] private int gadgetDamage = 3000;
    [SerializeField] private float gadgetRange = 70;

    public override void Attack()
    {
        for (int i = 0; i < hitPerAttack; i++)
        {
            RaycastHit hit;
            Vector3 sparyOffset = Random.insideUnitCircle * attackOffSet;
            sparyOffset.z = 0;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward + sparyOffset);

            if (Physics.Raycast(ray.origin, ray.direction, out hit, attackRange))
            {
                GameObject glow = Instantiate(hitVFX, hit.point, Quaternion.identity);

                if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<PlayerSetUp>().team != GetComponent<PlayerSetUp>().team)
                {
                    GMInstance.currentChargeSuper -= chargeSuperPerHit;
                    DamageServerRpc(attackDamage, hit.collider.GetComponent<PlayerSetUp>().id);
                }
            }
        }
    }

    public IEnumerator AttackDone()
    {
        yield return new WaitForSeconds(0.5f);
        isAttackFinish = true;
    }

    private void AttackAnim()
    {
        Deload(1);
        PlayAttackAudio();
        isAttackFinish = false;
        StartCoroutine(AttackDone());
        ResetHealServerRpc();
        anim.SetTrigger("shinAttack");
    }

    public override void Update()
    {
        if (currentMag < maxMag)
        {
            reloadTime -= Time.deltaTime;
            if (reloadTime < 0)
            {
                reloadTime = attackReload;
                Reload(1);
                AMInstance.PlayAudio(0, 0.6f);
            }
        }

        GMInstance.gadgetTime -= Time.deltaTime;
        GMInstance.gadgetCdImg.fillAmount = GMInstance.gadgetTime / cdGadget;

        if (GMInstance.gadgetTime <= 0)
        {
            if (isGadgetSound)
            {
                AudioManager.AMInstance.PlayAudio(6);
                isGadgetSound = false;
            }
            canGadget = true;
            GMInstance.gadgetTime = 0;
        }
        else
        {
            isGadgetSound = true;
        }

        GMInstance.superChargeImg.fillAmount = (float)GMInstance.currentChargeSuper / (float)chargeSuperRequire;
        if (GMInstance.currentChargeSuper <= 0)
        {
            if (isSuperSound)
            {
                AudioManager.AMInstance.PlayAudio(7);
                isSuperSound = false;
            }
            GMInstance.currentChargeSuper = 0;
            canSuper = true;
        }
        else
        {
            isSuperSound = true;
        }

        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<PlayerSetUp>().team != GetComponent<PlayerSetUp>().team)
            {
                GMInstance.dot.color = Color.red;
            }
            else
            {
                GMInstance.dot.color = Color.white;
            }
        }
        else if (Physics.Raycast(ray.origin, ray.direction, out hit, gadgetRange))
        {
            if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<PlayerSetUp>().team != GetComponent<PlayerSetUp>().team)
            {
                GMInstance.dot.color = Color.cyan;
            }
            else
            {
                GMInstance.dot.color = Color.white;
            }
        }
        else
        {
            GMInstance.dot.color = Color.white;
        }

        if (Input.GetMouseButtonDown(0) && currentMag > 0 && FinishAll())
        {
            AttackAnim();
        }

        if (Input.GetMouseButtonDown(1) && canSuper)
        {
            Super();
        }

        if (Input.GetKeyDown(KeyCode.Q) && canGadget && FinishAll())
        {
            Gadget();
        }
    }

    public override void Gadget()
    {
        base.Gadget();
        KnifeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void KnifeServerRpc(ServerRpcParams serverParam = default)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerSetUp>().id == (int)serverParam.Receive.SenderClientId)
            {
                Transform spawner = player.transform.GetChild(5);
                GameObject knife = Instantiate(Resources.Load<GameObject>("Knife"), spawner.position, Quaternion.Euler(player.transform.eulerAngles.x + 90, player.transform.eulerAngles.y, player.transform.eulerAngles.z));
                knife.GetComponent<NetworkObject>().Spawn();
                knife.GetComponent<Knife>().InitClientRpc((int)serverParam.Receive.SenderClientId, gadgetDamage, chargeSuperPerHit * 2);
                Rigidbody rb = knife.GetComponent<Rigidbody>();
                rb.AddForce(player.transform.forward * baseForce, ForceMode.Impulse);
                break;
            }
        }
    }

    public override void Super()
    {
        base.Super();
        GetComponent<PlayerMovement>().InscreseSpeedTemp(superSpeedUp, superDuration);
        InvisibilityServerRpc();
    }

    private IEnumerator Invisibility()
    {
        if (!transform.GetChild(1).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
            transform.GetChild(4).gameObject.SetActive(false);
            GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds(superDuration);
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(4).gameObject.SetActive(true);
            GetComponent<Renderer>().enabled = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InvisibilityServerRpc()
    {
        InvisibilityClientRpc();
    }

    [ClientRpc]
    private void InvisibilityClientRpc()
    {
        StartCoroutine(Invisibility());
    }
}
