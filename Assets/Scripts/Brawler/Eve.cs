using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AudioManager;


public class Eve : PlayerSkill
{
    [SerializeField] private float superSpeedBoost = 8;
    [SerializeField] private float superDuration = 5;
    [SerializeField] private float gadgetDistance = 12;

    private bool isForward;

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.W))
        {
            isForward = true;
        }
        else if (Input.GetKeyDown(KeyCode.S)) 
        {
            isForward = false;
        }
    }

    public override void Attack()
    {
        base.Attack();
    }

    public override void Gadget()
    {
        base.Gadget();
        GetComponent<PlayerMovement>().InscreseSpeedTemp(superSpeedBoost, superDuration);
    }

    public override void Super()
    {
        base.Super();
        if (isForward)
        {
            transform.Translate(transform.InverseTransformDirection(transform.forward) * gadgetDistance);
        }
        else
        {
            transform.Translate(transform.InverseTransformDirection(transform.forward) * -gadgetDistance);
        }

        float y = Mathf.Clamp(transform.position.y, 0f, 5f);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
