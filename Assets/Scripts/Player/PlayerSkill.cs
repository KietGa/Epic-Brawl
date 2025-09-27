using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using static AudioManager;
using Unity.Netcode;

public class PlayerSkill : NetworkBehaviour
{
    public int baseDamage = 1000;
    public int attackDamage = 1000;
    public float attackReload = 3f;
    public float attackDelay = 0.3f;
    public float attackRange = 30f;
    public float attackOffSet = 0f;
    public int maxMag = 3;
    public int superDamage = 600;
    public float superRange = 60f;
    public int hitPerAttack = 1;
    public int hitPerSuper = 10;
    public int chargeSuperPerHit = 10;
    public int chargeSuperRequire = 100;
    public float cdGadget = 10f;
    public GameObject hitVFX;
    public AudioClip[] clips;
    protected Camera cam;
    protected float reloadTime;
    protected int currentMag;
    protected bool canGadget;
    protected bool canSuper;
    protected bool isAttackFinish;
    protected bool isSuperFinish;
    protected bool isGadgetFinish;
    protected bool isGadgetSound;
    protected bool isSuperSound;
    protected Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        cam = transform.GetChild(2).GetComponent<Camera>();
        if (GMInstance.isStarted)
        {
            GMInstance.currentChargeSuper = chargeSuperRequire;
            GMInstance.gadgetTime = cdGadget;
            GMInstance.isStarted = false;
        }
        reloadTime = attackReload;
        currentMag = maxMag;
        canSuper = false;
        canGadget = false;
        isAttackFinish = true;
        isSuperFinish = true;
        isGadgetFinish = true;
        GMInstance.mags[0].SetActive(true);
        GMInstance.mags[1].SetActive(true);
        GMInstance.mags[2].SetActive(true);
    }

    public virtual void Update()
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

        if (Input.GetMouseButtonDown(0) && currentMag > 0 && isAttackFinish)
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

    public bool FinishAll()
    {
        return isAttackFinish && isSuperFinish && isGadgetFinish;
    }

    public virtual void Super()
    {
        GMInstance.currentChargeSuper = chargeSuperRequire;
        canSuper = false;
        ResetHealServerRpc();
        PlaySuperAudio();
    }

    public virtual void Gadget()
    {
        GMInstance.gadgetTime = cdGadget;
        canGadget = false;
        ResetHealServerRpc();
        PlayGadgetAudio();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetHealServerRpc()
    {
        ResetHealClientRpc();
    }

    [ClientRpc]
    private void ResetHealClientRpc()
    {
        GetComponent<PlayerHealth>().currentTime = 0;
    }

    public virtual void Attack()
    {
        StartCoroutine(AttackDelay());
        Deload(1);
        PlayAttackAudio(0.5f);
        anim.SetTrigger("attack");
        ResetHealServerRpc();

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

    protected IEnumerator AttackDelay()
    {
        isAttackFinish = false;
        yield return new WaitForSeconds(attackDelay);
        isAttackFinish = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DamageServerRpc(int damage, int target, ServerRpcParams srp = default)
    {
        DamageClientRpc(damage, target, (int)srp.Receive.SenderClientId);
    }

    [ClientRpc]
    private void DamageClientRpc(int damage, int target, int sender)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) 
        {
            if (player.GetComponent<PlayerSetUp>().id == target)
            {
                player.GetComponent<PlayerHealth>().LostHP(damage, sender);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealServerRpc(int heal, int target)
    {
        HealClientRpc(heal, target);
    }

    [ClientRpc]
    private void HealClientRpc(int heal, int target)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerSetUp>().id == target)
            {
                player.GetComponent<PlayerHealth>().HealHP(heal);
                break;
            }
        }
    }


    public void Reload(int mag)
    {
        for (int i = 0; i < mag; i++)
        {
            currentMag++;
            currentMag = Mathf.Clamp(currentMag, 0, maxMag);
            GMInstance.mags[currentMag - 1].SetActive(true);
        }
    }

    public void Deload(int mag)
    {
        for (int i = 0; i < mag; i++)
        {
            currentMag--;
            currentMag = Mathf.Clamp(currentMag, 0, maxMag);
            GMInstance.mags[currentMag].SetActive(false);
        }
    }

    public void InscreaseDamageTemp(int damage, float time)
    {
        StartCoroutine(InscreseDamage(damage, time));
    }

    public IEnumerator InscreseDamage(int damage, float time)
    {
        attackDamage += damage;
        yield return new WaitForSeconds(time);
        SetBaseDamage();
    }

    public void SetBaseDamage()
    {
        attackDamage = baseDamage;
    }

    public void Charge()
    {
        GMInstance.currentChargeSuper -= chargeSuperPerHit;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayAudioServerRpc(int index, float volume = 1f)
    {
        PlayAudioClientRpc(index, volume);
    }

    [ClientRpc]
    protected void PlayAudioClientRpc(int index, float volume = 1f)
    {
        GetComponent<AudioSource>().pitch = 1f;
        GetComponent<AudioSource>().PlayOneShot(clips[index], volume);
    }

    public void PlaySuperAudio(float volume = 1f)
    {
        PlayAudioServerRpc(2, volume);
    }

    public void PlayGadgetAudio(float volume = 1f)
    {
        PlayAudioServerRpc(1, volume);
    }

    public void PlayAttackAudio(float volume = 1f)
    {
        PlayAudioServerRpc(0, volume);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayPitchAudioServerRpc(int index, float volume = 1f)
    {
        PlayPitchAudioClientRpc(index, volume);
    }

    [ClientRpc]
    protected void PlayPitchAudioClientRpc(int index, float volume = 1f)
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
        GetComponent<AudioSource>().PlayOneShot(clips[index], volume);
    }
}
