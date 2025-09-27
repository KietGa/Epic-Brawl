using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AudioManager;


public class Tusk : PlayerSkill
{
    [SerializeField] private float superSpeed = 10f;
    [SerializeField] private int superAttackBoost = 500;
    [SerializeField] private float superDuration = 5f; 
    private bool isRight = true;

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
        yield return new WaitForSeconds(0.8f);
        isAttackFinish = true;
    }

    private void AttackAnim()
    {
        Deload(1);
        ResetHealServerRpc();
        PlayAttackAudio();
        isRight = !isRight;
        isAttackFinish = false;
        StartCoroutine(AttackDone()); 

        if (isRight)
        {
            anim.SetTrigger("tuskRightAttack");
        }
        else
        {
            anim.SetTrigger("tuskLeftAttack");
        }
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

        if (Input.GetMouseButtonDown(0) && currentMag > 0 && FinishAll())
        {
            AttackAnim();
        }

        if (Input.GetMouseButtonDown(1) && canSuper)
        {
            Super();
        }

        if (Input.GetKeyDown(KeyCode.Q) && canGadget)
        {
            Gadget();
        }
    }

    public override void Gadget()
    {
        base.Gadget();
        GetComponent<PlayerMovement>().InscreseSpeedTemp(superSpeed, superDuration);
        GetComponent<PlayerSkill>().InscreaseDamageTemp(superAttackBoost, superDuration);
    }

    public override void Super()
    {
        base.Super();
        HealServerRpc(GetComponent<PlayerHealth>().maxHealth / 2, (int)OwnerClientId);
    }
}
