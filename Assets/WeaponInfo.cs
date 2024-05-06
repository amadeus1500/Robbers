using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum FireMode
{
    Auto,
    Semi
};
public class WeaponInfo : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles;
    [SerializeField] FireMode fireMode;
    public Transform Handle;
    public Transform SecondHandle;
    public float FireSpeed;
    public float Damage;
    public string AttackAnimation = "Shoot";
    public float Recoil;
    public float RecoilTime;
    public float Distance = 150;
    public bool NeedIK = true;
    public string ReloadAnimation = "Reload";
    public bool Reloadable = true;
    public bool InfiniteAmmo = false;
    public int Ammo = 30;
    public int MaxAmmo = 30;
    public float HeadShotDamageMult = 1.5f;
    public AudioClip ShootClip;
    public float MinRange = 5;
    public float MaxRange = 20;
    public bool ShootValidation()
    {
        bool shot = false;
        if(Input.GetMouseButton(0) && fireMode == FireMode.Auto)
            shot = true;
        else if(Input.GetMouseButtonDown(0) && fireMode == FireMode.Semi)
            shot = true;

        return shot;
    }
    //IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    //{
    //    float time = 0;
    //    Vector3 startPosition = transform.position;

    //    while (time < duration)
    //    {
    //        transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
    //        time += Time.deltaTime;
    //        yield return null;
    //    }
    //    transform.position = targetPosition;
    //    time = 0;
    //    startPosition = transform.position;

    //    while (time < duration)
    //    {
    //        transform.position = Vector3.Lerp(startPosition, StartTransform.position, time / duration);
    //        time += Time.deltaTime;
    //        yield return null;
    //    }
    //    transform.position = StartTransform.position;
    //}
    public void EmitShootParticle()
    {
        foreach (var particle in particles)
        {
            particle.Play();
        }
    }
}
