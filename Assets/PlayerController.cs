using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions.Must;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] PhotonView PV;
    [SerializeField] AutoSoundPlayer ASP;
    [SerializeField] private GameObject cam;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private Camera camer;
    [SerializeField] private List<GameObject> FullBody;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioListener listener;
    [SerializeField] private CharacterController controller;
    [SerializeField] private WeaponInfo CurrentGun;
    [SerializeField] private WeaponInfo PrimaryGun;
    [SerializeField] private WeaponInfo SecondaryGun;
    [SerializeField] private WeaponInfo Melee;
    [SerializeField] float Speed = 5f;
    [SerializeField] float Sensitivity = 100f;
    [SerializeField] TwoBoneIKConstraint Right;
    [SerializeField] TwoBoneIKConstraint Left;
    [SerializeField] MultiAimConstraint GunAim;
    [SerializeField] RigBuilder rig;
    [SerializeField] float MaxStamina;
    [SerializeField] float StaminaDrain;
    [SerializeField] float StaminaGain;
    [SerializeField] float TimeBeforeStaminaRegen;
    [SerializeField] float SpeedGainOnRun = 6;
    [SerializeField] float JumpForce = 10;
    [SerializeField] float JumpCoolDown = 0.2f;
    [SerializeField] float StaminaOnJump = 20;
    public List<SkinnedMeshRenderer> SkinMeshes;
    Coroutine RecoilCoroutine;

    //float recoilDuration;
    //float recoildestination;
    protected bool reloading = false;
    bool needtojump = false;
    float Stamina;
    double LastSprintTime;
    float LastjumpCoolDown;
    float TimeBeforeNextShoot;
    bool Walking;
    bool Running;
    float Xaxis;
    float Yaxis;
    float MXaxis;
    float MYaxis;
    float angle = 0;
    Vector3 Gravity;
    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();
    //    if (!IsOwner) return;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    camer.enabled = true;
    //    listener.enabled = true;
    //    foreach (var item in FullBody)
    //    {
    //        item.layer = 2;
    //    }
    //    UpdateAmmo();
    //}
    private void Start()
    {
        Stamina = MaxStamina;
        if (PV.IsMine)
        {
            controller.enabled = true;
            controller.detectCollisions = true;
            Cursor.lockState = CursorLockMode.Locked;
            camer.enabled = true;
            listener.enabled = true;
            foreach (var item in FullBody)
            {
                item.layer = 2;
            }
            UpdateAmmo();
        }
        else
        {
            controller.detectCollisions = false;
        }
    }
    IEnumerator lerpRecoil(float duration)
    {
        float StartTime = 0;

        while (StartTime <= duration)
        {// this will be 0 at the beginning and 1 at the end.
            StartTime += Time.deltaTime;
            float step = Mathf.Clamp01(StartTime / duration);
            angle = Mathf.Lerp(angle, angle- CurrentGun.Recoil, step);
            yield return null;
        }

    }
    void ReloadEnd()
    {
        reloading = false;
        CurrentGun.Ammo = CurrentGun.MaxAmmo;
        UpdateAmmo();
    }
    void RemoveBullet()
    {
        if (CurrentGun.InfiniteAmmo) return;
        CurrentGun.Ammo--;
    }
    void UpdateAmmo()
    {
        LevelManager.Singletone.ammotext.enabled = !CurrentGun.InfiniteAmmo;
        LevelManager.Singletone.ammotext.text = $"Ammo: {CurrentGun.Ammo}";
    }
    void StartRecoil()
    {
        //recoilDuration = 0;
        //recoildestination = angle - CurrentGun.Recoil;
        if(RecoilCoroutine != null)
        {
            StopCoroutine(RecoilCoroutine);
        }
       RecoilCoroutine = StartCoroutine(lerpRecoil(CurrentGun.RecoilTime));
    }
    void EquipGun(int gun)
    {
        if(gun == 1)
        {
            CurrentGun = PrimaryGun;
        }else if(gun == 2)
        {
            CurrentGun = SecondaryGun;
        }else if(gun == 3)
        {
            CurrentGun = Melee;
        }
        if(PrimaryGun)
        PrimaryGun.gameObject.SetActive(false);
        if(SecondaryGun)
        SecondaryGun.gameObject.SetActive(false);
        if(Melee)
        Melee.gameObject.SetActive(false);
        CurrentGun.gameObject.SetActive(true);
        if (CurrentGun.NeedIK)
        {
            Right.data.target = CurrentGun.Handle;
            Left.data.target = CurrentGun.SecondHandle;
            rig.Build();
        }
        else
        {
            Right.data.target = null;
            Left.data.target = null;
            rig.Build();
        }
    }
    void UpdateStaminaBar()
    {
        LevelManager.Singletone.StaminaBar.fillAmount = Stamina / MaxStamina;
    }
    private void Update()
    {
        UpdateRun();
    }
    protected void UpdateRun()
    {
        if (!PV.IsMine) return;
        if(!reloading)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (CurrentGun == PrimaryGun || !PrimaryGun) return;
                animator.SetBool("Knife", false);
                CurrentGun = PrimaryGun;
                UpdateAmmo();
                this.photonView.RPC(nameof(GunSwitchClientRpc), RpcTarget.All, 1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (CurrentGun == SecondaryGun || !SecondaryGun) return;
                animator.SetBool("Knife", false);
                CurrentGun = SecondaryGun;
                UpdateAmmo();
                this.photonView.RPC(nameof(GunSwitchClientRpc), RpcTarget.All, 2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (CurrentGun == Melee || !Melee) return;
                animator.SetBool("Knife", true);
                CurrentGun = Melee;
                UpdateAmmo();
                this.photonView.RPC(nameof(GunSwitchClientRpc), RpcTarget.All, 3);
            }
        };
        MXaxis = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        MYaxis = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
        Xaxis = Input.GetAxisRaw("Horizontal");
        Yaxis = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.LeftShift) && !Running)
        {
            Running = true;
            Speed += SpeedGainOnRun;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && Running)
        {
            Running = false;
            Speed -= SpeedGainOnRun;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (controller.isGrounded)
            {
                if (Stamina > StaminaOnJump)
                {
                    Stamina -= StaminaOnJump;
                    LastSprintTime = PhotonNetwork.Time + TimeBeforeStaminaRegen;
                    if (LastjumpCoolDown < Time.time)
                    {
                        LastjumpCoolDown = Time.time + JumpCoolDown;
                        needtojump = true;
                        photonView.RPC(nameof(PlayTrigger), RpcTarget.All, "Jump");
                    }
                }
            }
        }
        if(transform.forward * Yaxis + transform.right * Xaxis == Vector3.zero)
        {
            Walking = false;
        }
        else
        {
            Walking = true;
        }
        if (Running)
        {
            if (Xaxis != 0 || Yaxis != 0)
            {
                LastSprintTime = PhotonNetwork.Time + TimeBeforeStaminaRegen;
                Stamina = Mathf.Clamp(Stamina - StaminaDrain * Time.deltaTime, 0, MaxStamina);
                if (Stamina <= 0)
                {
                    Running = false;
                    Speed -= SpeedGainOnRun;
                    animator.SetBool("Running", Running);
                }
                animator.SetBool("Running", Running);
            }
            else
            {
                if (LastSprintTime < PhotonNetwork.Time)
                {
                    Stamina = Mathf.Clamp(Stamina + StaminaGain * Time.deltaTime, 0, MaxStamina);
                }
                animator.SetBool("Running", false);
            }
            UpdateStaminaBar();
        }
        else
        {
            if(LastSprintTime < PhotonNetwork.Time)
            {
                Stamina = Mathf.Clamp(Stamina + StaminaGain * Time.deltaTime, 0, MaxStamina);
            }
            animator.SetBool("Running", Running);
            UpdateStaminaBar();
        }
        animator.SetBool("Walking", Walking);
        if (controller.isGrounded)
        {
            Gravity.y = -1;
        }
        else
        {
            Gravity.y -= 1 * Time.deltaTime;
        }
        if (needtojump)
        {
            needtojump = false;
            Gravity.y = JumpForce;
        }
        controller.Move(Gravity + (transform.forward * Yaxis + transform.right * Xaxis).normalized * Speed * Time.deltaTime);
        transform.Rotate(Vector3.up, MXaxis);
        angle -= MYaxis;

        //recoilDuration += Time.deltaTime;
        //if (recoilDuration < CurrentGun.RecoilTime)
        //{
        //    float norm = recoilDuration / CurrentGun.RecoilTime;
        //    angle = Mathf.Lerp(angle, angle - CurrentGun.Recoil, norm);
        //}
        if (CurrentGun.ShootValidation())
        {
            if (TimeBeforeNextShoot < Time.time && !reloading && CurrentGun.Ammo > 0)
            {
            TimeBeforeNextShoot = Time.time + CurrentGun.FireSpeed;
            RemoveBullet();
            UpdateAmmo();
            StartRecoil();
            this.photonView.RPC(nameof(GunShootClientRpc), RpcTarget.All);
            if (Physics.Raycast(camer.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, CurrentGun.Distance,~LevelManager.Singletone.BulletIgnore))
            {
                if(hit.collider.transform.root.TryGetComponent<Health>(out Health hp))
                {
                        if(hit.collider.name == "head")
                        {
                            hp.TakeDamage(CurrentGun.Damage * CurrentGun.HeadShotDamageMult);
                            print("HeadShot!");
                        }
                        else
                        {
                            hp.TakeDamage(CurrentGun.Damage);
                        }
                }
                if (hit.collider.CompareTag("Wall"))
                {
                    //BulletholeServerRpc(new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z), Quaternion.LookRotation(hit.normal));
                    //this.photonView.RPC(nameof(BulletholeClientRpc), RpcTarget.All, new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z), Quaternion.LookRotation(hit.normal));
                    BulletholeClientRpc(new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z), Quaternion.LookRotation(hit.normal));
                }
                print(hit.collider.name);
            }
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && CurrentGun.Reloadable && !reloading && !CurrentGun.InfiniteAmmo)
        {
            reloading = true;
            this.photonView.RPC(nameof(PlayTrigger), RpcTarget.All, CurrentGun.ReloadAnimation);
            Invoke(nameof(ReloadEnd), 1.7f);
        }
        angle = Mathf.Clamp(angle, -90, 90);
        cam.transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }
    private void FixedUpdate()
    {
        //if (!PV.IsMine) return;
    }
    private void LateUpdate()
    {
        cam.transform.position = cameraPos.position;
        //angle = Mathf.Clamp(angle, -90, 90);
        //cam.transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }

    //[ServerRpc(RequireOwnership = false)]
    //private void GunSwitchServerRpc(int gun)
    //{
    //    GunSwitchClientRpc(gun);
    //}
    [PunRPC]
    public void GunSwitchClientRpc(int gun)
    {
        EquipGun(gun);
    }
    //[ServerRpc(RequireOwnership = false)]
    //private void GunShootServerRpc()
    //{
    //    GunShootClientRpc();
    //}
    [PunRPC]
    public void GunShootClientRpc()
    {
        animator.SetTrigger(CurrentGun.AttackAnimation);
        CurrentGun.EmitShootParticle();
        ASP.ShootSoundAsync(CurrentGun.ShootClip, CurrentGun.MinRange, CurrentGun.MaxRange);
    }
    [PunRPC]
    public void PlayTrigger(string triggername)
    {
        animator.SetTrigger(triggername);
    }
    //[ServerRpc(RequireOwnership = false)]
    //private void BulletholeServerRpc(Vector3 pos, Quaternion rot)
    //{
    //    GameObject obj = Instantiate(LevelManager.Singletone.BulletHole, pos, rot);
    //    NetworkObject nobj = obj.GetComponent<NetworkObject>();
    //    nobj.Spawn();
    //}
    //[ClientRpc()]
    //[PunRPC]
    public void BulletholeClientRpc(Vector3 pos, Quaternion rot)
    {
        //GameObject obj = Instantiate(LevelManager.Singletone.BulletHole, new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z + -0.01f), Quaternion.LookRotation(hit.normal));
        //NetworkObject nobj = obj.GetComponent<NetworkObject>();
        //nobj.Spawn();
        GameObject obj = PhotonNetwork.Instantiate(LevelManager.Singletone.BulletHole.name, pos,rot);
    }
}
