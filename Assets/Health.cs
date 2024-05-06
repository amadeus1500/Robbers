using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviourPun
{
    public float health;
    public float Maxhealth;
    bool isAlive = true;
    [SerializeField] PlayerController contrl;
    [SerializeField] PhotonView PV;
    [SerializeField] CharacterController CharControler;
    [SerializeField] Animator animator;
    [SerializeField] Vector3 SpawnPoint;
    [SerializeField] Spectation spectator;
    [SerializeField] List<Rigidbody> rigibodies;
    public MeshRenderer[] meshes;
    public SkinnedMeshRenderer[] skinnedmesher;
    private void Start()
    {
        if (!PV.IsMine) return;
        LevelManager.Singletone.Clienthealth = this;
    }
    public void Respawn()
    {
        health = 100;
        animator.enabled = true;
        transform.position = SpawnPoint;
        CharControler.enabled = true;
        contrl.enabled = true;
        isAlive = true;
        if(PV.IsMine)
         UpdateHpBar();
    }
    public void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        health = 0;
        foreach (var item in rigibodies)
        {
            item.isKinematic = false;
        }
        contrl.enabled = false;
        CharControler.enabled = false;
        animator.enabled = false;
        LevelManager.Singletone.Died(photonView.Owner.ActorNumber);
        if (PV.IsMine)
        {
            spectator.enabled = true;
        }
        spectator.Died = true;
        if (PV.IsMine)
        {
            spectator.Switch(1);
        }
    }
    float OneToZero(float value, float maxvaule)
    {
        return value / maxvaule;
    }
    private void UpdateHpBar()
    {
        LevelManager.Singletone.HpText.text = $"{health}";
        LevelManager.Singletone.Hpbar.fillAmount = OneToZero(health, Maxhealth);
    }
    public void TakeDamage(float damage)
    {
        this.PV.RPC(nameof(DamageClientRpc), RpcTarget.All, damage);
    }
    [PunRPC]
   public void DamageClientRpc(float dm)
    {
        health -= dm;
        if (health <= 0)
        {
            Die();
        }
        if (PV.IsMine)
        {
            UpdateHpBar();
        }
    }
}
