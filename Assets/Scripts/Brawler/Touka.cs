using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static GameManager;

public class Touka : PlayerSkill
{
    [SerializeField] private int gadgetHeal = 2000;
    [SerializeField] private int superHeal = 2000;

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

            if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<PlayerSetUp>().team == GetComponent<PlayerSetUp>().team)
            {
                GMInstance.currentChargeSuper -= chargeSuperPerHit;
                HealServerRpc(gadgetHeal, hit.collider.GetComponent<PlayerSetUp>().id);
            }
        }
    }

    public override void Super()
    {
        base.Super();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerSetUp>().team == GetComponent<PlayerSetUp>().team)
            {
                HealServerRpc(superHeal, player.GetComponent<PlayerSetUp>().id);
            }
        }
    }
}
