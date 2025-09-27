using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AudioManager;

public class Reo : PlayerSkill
{
    [SerializeField] private float superTime = 5f;
    [SerializeField] private int gadgetDamage = 3000;
    [SerializeField] private float gadgetRange = 10f;
    [SerializeField] private float superSpeedUp = 8f;

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

        if (Input.GetMouseButtonDown(0) && currentMag > 0 && isAttackFinish && isGadgetFinish)
        {
            Attack();
        }

        if (Input.GetMouseButtonDown(1) && canSuper && isSuperFinish)
        {
            Super();
        }

        if (Input.GetKeyDown(KeyCode.Q) && canGadget && isGadgetFinish)
        {
            Gadget();
        }
    }

    public override void Gadget()
    {
        base.Gadget();
        StartCoroutine(DoneGadget());
    }

    private IEnumerator DoneGadget()
    {
        anim.SetTrigger("gadget");
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(5).gameObject.SetActive(true);
        isGadgetFinish = false;
        yield return new WaitForSeconds(0.5f);
        isGadgetFinish = true;
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(5).gameObject.SetActive(false);
    }

    public void AttackGadget()
    {
        for (int i = 0; i < hitPerAttack; i++)
        {
            RaycastHit hit;
            Vector3 sparyOffset = Random.insideUnitCircle * attackOffSet;
            sparyOffset.z = 0;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward + sparyOffset);

            if (Physics.Raycast(ray.origin, ray.direction, out hit, gadgetRange))
            {
                GameObject glow = Instantiate(hitVFX, hit.point, Quaternion.identity);

                if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<PlayerSetUp>().team != GetComponent<PlayerSetUp>().team)
                {
                    GMInstance.currentChargeSuper -= chargeSuperPerHit * 2;
                    DamageServerRpc(gadgetDamage, hit.collider.GetComponent<PlayerSetUp>().id);
                }
            }
        }
    }

    public override void Super()
    {
        base.Super();
        GetComponent<PlayerMovement>().InscreseSpeedTemp(superSpeedUp, superTime);
        StartCoroutine(DecreaseSize(superTime));
    }

    public IEnumerator DecreaseSize(float time)
    {
        transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        yield return new WaitForSeconds(time);
        SetBaseScale();
    }

    public void SetBaseScale()
    {
        transform.localScale = new Vector3(3f, 3f, 3f);
    }
}
