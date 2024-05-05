using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectation : MonoBehaviourPun
{
    [SerializeField] Health health;
    [SerializeField] GameObject Camera;
    private Spectation SelectedSpection;
    public bool Died = false;
    private int index = 0;
    bool runned = false;
    private void Start()
    {
        if(!runned)
        {
            LevelManager.Singletone.AvSpectators.Add(this);
            runned = true;
            enabled = false;
        };
    }
    public void Switch(int Difference)
    {
        if (!photonView.IsMine) return;
        index += Difference;
        if (index >= LevelManager.Singletone.AvSpectators.Count)
        {
            index = 0;
        }else if(index < 0)
        {
            index = LevelManager.Singletone.AvSpectators.Count -1;
        }
        if (!LevelManager.Singletone.AvSpectators[index].Died)
        {
            SelectedSpection = LevelManager.Singletone.AvSpectators[index];
        }
        else
        {
            Switch(Difference);
        }
    }
    private void Update()
    {
        if (!photonView.IsMine) return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Switch(-1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Switch(1);
        }
    }
    private void LateUpdate()
    {
        if (!photonView.IsMine) return;
        if (SelectedSpection != null)
        {
            Camera.transform.position = SelectedSpection.gameObject.transform.position;
            Camera.transform.rotation = SelectedSpection.gameObject.transform.rotation;
        }
    }
}
