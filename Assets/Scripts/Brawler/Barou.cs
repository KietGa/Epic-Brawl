using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AudioManager;
using Unity.Netcode;


public class Barou : PlayerSkill
{
    [SerializeField] private float baseForce = 10f;
    [SerializeField] private float explodeAttackRange = 15f;
    [SerializeField] private float explodeSuperRange = 25f;
    [SerializeField] private float explodeTime = 3f;
    [SerializeField] private int gadgetHeal = 1500;
    private float addForce = 0f;

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
        else if (Physics.Raycast(ray.origin, ray.direction, out hit, superRange))
        {
            if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<PlayerSetUp>().team != GetComponent<PlayerSetUp>().team)
            {
                GMInstance.dot.color = Color.yellow;
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

        if (Input.GetMouseButton(0) && currentMag > 0)
        {
            if (FinishAll())
            {
                isAttackFinish = false;
                transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (!isAttackFinish && addForce <= 3f)
            {
                addForce += Time.deltaTime;
            }
        }

        if (Input.GetMouseButtonUp(0) && currentMag > 0 && !isAttackFinish)
        {
            Attack();
        }

        if (Input.GetMouseButton(1) && canSuper)
        {
            if (FinishAll())
            {
                isSuperFinish = false;
                transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (!isSuperFinish && addForce <= 5f)
            {
                addForce += Time.deltaTime;
            }
        }

        if (Input.GetMouseButtonUp(1) && canSuper && !isSuperFinish)
        {
            Super();
        }

        if (Input.GetKeyDown(KeyCode.Q) && canGadget)
        {
            Gadget();
        }
    }

    public override void Attack()
    {
        Deload(1);
        isAttackFinish = true;
        PlayAttackAudio(0.7f);
        transform.GetChild(1).gameObject.SetActive(true);
        AttackServerRpc(addForce);
        addForce = 0;
        ResetHealServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackServerRpc(float addForce, ServerRpcParams serverParam = default)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerSetUp>().id == (int)serverParam.Receive.SenderClientId)
            {
                Transform spawner = player.transform.GetChild(5);
                GameObject grenade = Instantiate(Resources.Load<GameObject>("Grenade"), spawner.position, player.transform.rotation);
                grenade.GetComponent<NetworkObject>().Spawn();
                grenade.GetComponent<Grenade>().InitClientRpc((int)serverParam.Receive.SenderClientId, attackDamage, explodeAttackRange, explodeTime, chargeSuperPerHit, 0);
                Rigidbody rb = grenade.GetComponent<Rigidbody>();
                rb.AddForce(player.transform.forward * baseForce * addForce, ForceMode.Impulse);
                break;
            }
        }
    }

    public override void Gadget()
    {
        base.Gadget();
        HealServerRpc(gadgetHeal, (int)OwnerClientId);
    }

    public override void Super()
    {
        base.Super();
        isSuperFinish = true;
        transform.GetChild(1).gameObject.SetActive(true);
        SuperServerRpc(addForce);
        addForce = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SuperServerRpc(float addForce, ServerRpcParams serverParam = default)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerSetUp>().id == (int)serverParam.Receive.SenderClientId)
            {
                Transform spawner = player.transform.GetChild(5);
                GameObject grenade = Instantiate(Resources.Load<GameObject>("Grenade"), spawner.position, player.transform.rotation);
                grenade.GetComponent<NetworkObject>().Spawn();
                grenade.GetComponent<Grenade>().InitClientRpc((int)serverParam.Receive.SenderClientId, superDamage, explodeSuperRange, explodeTime, chargeSuperPerHit, 1);
                Rigidbody rb = grenade.GetComponent<Rigidbody>();
                rb.AddForce(player.transform.forward * baseForce * addForce, ForceMode.Impulse);
                break;
            }
        }
    }
}
