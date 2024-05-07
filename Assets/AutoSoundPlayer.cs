using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSoundPlayer : MonoBehaviourPun
{
    [SerializeField] AudioSource footstepSource;
    [SerializeField] AudioSource ShootSource;
    [SerializeField] AudioSource ReloadSource;

    [SerializeField] AudioClip FootstepSound;
    [SerializeField] AudioClip jumpSound;

    [SerializeField] AudioClip MagRemoveSound;
    [SerializeField] AudioClip MagInputSound;
    [SerializeField] CharacterController contrl;
    public void PlayFootstepAsync()
    {
        if (photonView.IsMine)
        {
            if (contrl.isGrounded)
            {
                footstepSource.PlayOneShot(FootstepSound);
            }
        }
        else
        {
            footstepSource.PlayOneShot(FootstepSound);
        }
    }
   public void PlayJumpAsync()
    {
        footstepSource.PlayOneShot(jumpSound);
    }
    public void PlayMagRemove()
    {
        ReloadSource.PlayOneShot(MagRemoveSound);
    }
    public void PlayMagInput()
    {
        ReloadSource.PlayOneShot(MagInputSound);
    }
    public void ShootSoundAsync(AudioClip sound, float MinRange, float MaxRange)
    {
        ShootSource.maxDistance = MaxRange;
        ShootSource.minDistance = MinRange;
        ShootSource.PlayOneShot(sound);
    }
}
