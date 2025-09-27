using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AudioManager;

public class Bling : PlayerSkill
{
    [SerializeField] private float gadgetDistance = 15;

    public override void Gadget()
    {
        base.Gadget();
        transform.Translate(transform.InverseTransformDirection(transform.forward) * -gadgetDistance);
        float y = Mathf.Clamp(transform.position.y, 0f, 5f);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public override void Super()
    {
        base.Super();

        for (int i = 0; i < hitPerSuper; i++)
        {
            RaycastHit hit;
            Vector3 sparyOffset = Random.insideUnitCircle * attackOffSet;
            sparyOffset.z = 0;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward + sparyOffset);

            if (Physics.Raycast(ray.origin, ray.direction, out hit, superRange))
            {
                GameObject glow = Instantiate(hitVFX, hit.point, Quaternion.identity);

                if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<PlayerSetUp>().team != GetComponent<PlayerSetUp>().team)
                {
                    GMInstance.currentChargeSuper -= chargeSuperPerHit * 2;
                    DamageServerRpc(superDamage, hit.collider.GetComponent<PlayerSetUp>().id);
                }
            }
        }
    }
}
