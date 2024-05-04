using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviourPunCallbacks
{
public static LevelManager Singletone;
    public GameObject BulletHole;
    public LayerMask BulletIgnore;
    public TextMeshProUGUI ammotext;
    public TextMeshProUGUI HpText;
    public Image Hpbar;
    public GameObject PlayerPrefab;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (Singletone == null)
        {
            Singletone = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //   GameObject rob = PhotonNetwork.Instantiate("Robber2", Vector3.zero, Quaternion.identity);
    //    rob.GetComponent<PhotonView>().TransferOwnership(newPlayer);
    //}
    private void Start()
    {
        this.photonView.RPC(nameof(Spawn), RpcTarget.AllBufferedViaServer);
    }
    [PunRPC]
    public void Spawn()
    {
        PhotonNetwork.Instantiate(PlayerPrefab.name, Vector3.zero, Quaternion.identity);
    }
}
