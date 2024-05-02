using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UiMngr : MonoBehaviour
{
    [SerializeField] Button hostbtn;
    [SerializeField] Button serverbtn;
    [SerializeField] Button clientbtn;
    private void Awake()
    {
        hostbtn.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartHost();
        });
        serverbtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
        clientbtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }
}
