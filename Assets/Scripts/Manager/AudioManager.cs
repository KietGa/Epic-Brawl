using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager AMInstance {  get; private set; }
    public AudioSource[] sources;
    public AudioClip[] clips;

    private void Awake()
    {
        AMInstance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayAudioServerRpc(int index, float volume = 1f)
    {
        PlayAudioClientRpc(index, volume);
    }

    [ClientRpc]
    private void PlayAudioClientRpc(int index, float volume = 1f)
    {
        PlayAudio(index, volume);
    }

    public void PlayAudio(int index, float volume = 1f)
    {
        sources[0].PlayOneShot(clips[index], volume);
    }

    public void PlayPitchAudio(int index, float volume = 1f)
    {
        sources[1].pitch = Random.Range(0.8f, 1.2f);
        sources[1].PlayOneShot(clips[index], volume);
    }
}
