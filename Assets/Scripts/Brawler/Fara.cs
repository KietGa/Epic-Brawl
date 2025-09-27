using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Fara : PlayerSkill
{
    [SerializeField] private int gadgetDamage = 1500;
    [SerializeField] private int gadgetHeal = 1500;
    [SerializeField] private int superHeal = 1500;

    public override void Attack()
    {
        base.Attack();
    }

    public override void Gadget()
    {
        base.Gadget();
        anim.SetTrigger("attack");
        ResetHealServerRpc();

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
                DamageServerRpc(gadgetDamage, hit.collider.GetComponent<PlayerSetUp>().id);
                HealServerRpc(gadgetHeal, (int)OwnerClientId);
            }
        }
    }

    public override void Super()
    {
        base.Super();
        anim.SetTrigger("attack");
        ResetHealServerRpc();

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
                DamageServerRpc(superDamage, hit.collider.GetComponent<PlayerSetUp>().id);
                HealServerRpc(superHeal, (int)OwnerClientId);
            }
        }
    }
}
