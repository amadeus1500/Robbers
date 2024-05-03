using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
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
    public MeshRenderer[] meshes;
    public SkinnedMeshRenderer[] skinnedmesher;
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
        contrl.enabled = false;
        CharControler.enabled = false;
        animator.enabled = false;
        Invoke(nameof(Respawn), 5f);
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
