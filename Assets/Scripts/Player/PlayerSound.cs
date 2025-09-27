using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSound : NetworkBehaviour
{
    [SerializeField] private float footstepTimerMax = 0.3f;
    [SerializeField] private AudioClip[] moveClips;
    private float footstepTimer;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GetComponent<PlayerMovement>().isWalking)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer < 0f)
            {
                footstepTimer = footstepTimerMax;
                FootstepServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void FootstepServerRpc()
    {
        FootstepClientRpc();
    }

    [ClientRpc]
    private void FootstepClientRpc()
    {
        source.PlayOneShot(moveClips[Random.Range(0, moveClips.Length)], 1f);
    }
}
