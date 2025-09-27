using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AudioManager;


public class Bang : PlayerSkill
{
    [SerializeField] private float hitPerAttackFloat = 1.25f;
    [SerializeField] private float hitPerSuperFloat = 2.2f;

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
            Attack();
        }

        if (Input.GetMouseButtonDown(1) && canSuper && FinishAll())
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
        ResetHealServerRpc();
        isAttackFinish = false;
        Deload(1);

        for (float i = 0; i <= hitPerAttackFloat; i += 0.2f)
        {
            StartCoroutine(FullAttack(i));
        }
    }

    private IEnumerator FullAttack(float time)
    {
        yield return new WaitForSeconds(time);
        OneAttack();
        if (time == hitPerAttackFloat)
        {
            isAttackFinish = true;
        }
    }

    private void OneAttack()
    {
        PlayPitchAudioServerRpc(0, 0.5f);
        anim.SetTrigger("attack");

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

    public override void Gadget()
    {
        base.Gadget();
        Reload(2);
    }

    public override void Super()
    {
        isSuperFinish = false;
        base.Super();

        for (float i = 0; i <= hitPerSuperFloat+0.01f; i += 0.15f)
        {
            i = i * 100 / 100;
            StartCoroutine(FullSuper((float)i+0.01f));
        }
    }

    private IEnumerator FullSuper(float time)
    {
        yield return new WaitForSeconds(time);
        OneSuper();
        if (time >= hitPerSuperFloat)
        {
            isSuperFinish = true;
        }
    }

    private void OneSuper()
    {
        PlayPitchAudioServerRpc(2, 0.5f);
        anim.SetTrigger("attack");

        RaycastHit hit;
        Vector3 sparyOffset = Random.insideUnitCircle * attackOffSet;
        sparyOffset.z = 0;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward + sparyOffset);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, superRange))
        {
            GameObject glow = Instantiate(hitVFX, hit.point, Quaternion.identity);

            if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<PlayerSetUp>().team != GetComponent<PlayerSetUp>().team)
            {
                GMInstance.currentChargeSuper -= chargeSuperPerHit / 2;
                DamageServerRpc(superDamage, hit.collider.GetComponent<PlayerSetUp>().id);
            }
        }
    }
}
