using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Remover : MonoBehaviour
{
    [SerializeField] float LifeTime;
    private void Start()
    {
        Destroy(gameObject, LifeTime);
    }
}
