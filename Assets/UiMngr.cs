using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UiMngr : MonoBehaviourPunCallbacks
{
    [SerializeField] Button hostbtn;
    [SerializeField] Button serverbtn;
    [SerializeField] Button clientbtn;
    [SerializeField] GameObject Player;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
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
    private void Start()
    {
        Debug.Log("Connecting");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to server");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined Jobby");
        PhotonNetwork.JoinOrCreateRoom("Test",null,null);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");
        PhotonNetwork.Instantiate(Player.name, Vector3.zero, Quaternion.identity);
    }
}
