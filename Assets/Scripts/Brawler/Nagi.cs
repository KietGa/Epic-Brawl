using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Nagi : PlayerSkill
{
    [SerializeField] private int gadgetDamage = 2000;

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
            }
        }
    }

    public override void Super()
    {
        GMInstance.currentChargeSuper = chargeSuperRequire;
        canSuper = false;
        ResetHealServerRpc();
        AudioManager.AMInstance.PlayAudioServerRpc(8);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerSetUp>().team != GetComponent<PlayerSetUp>().team)
            {
                DamageServerRpc(superDamage, player.GetComponent<PlayerSetUp>().id);
            }
        }
    }
}
