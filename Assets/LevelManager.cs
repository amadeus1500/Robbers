using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum GameProgress
{
    Intermission,
    MidGame,
    End
}


public class LevelManager : MonoBehaviourPunCallbacks
{
public static LevelManager Singletone;
    public GameObject BulletHole;
    public LayerMask BulletIgnore;
    public TextMeshProUGUI ammotext;
    public TextMeshProUGUI HpText;
    public TextMeshProUGUI InterText;
    public Image Hpbar;
    public GameObject PlayerPrefab;
    public Health Clienthealth;
    public List<Spectation> AvSpectators = new List<Spectation>();
    //ExitGames.Client.Photon.Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
    bool coroutineStarted = false;
    public List<Player> InGamePlayers = new List<Player>();
    private GameProgress Proggress = GameProgress.Intermission;
    double TimeLeft;
    bool CanStart()
    {
        bool ret = false;
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            ret = true;
        }
        return ret;
    }

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
    IEnumerator InterMission()
    {
        if (CanStart())
        {
            double timLeft = TimeLeft - PhotonNetwork.Time;
            this.photonView.RPC(nameof(UpdateText), RpcTarget.All, $"{Mathf.Round((float)timLeft)}");
            if (timLeft <= 0)
            {
                Proggress = GameProgress.MidGame;
                TimeLeft = PhotonNetwork.Time + 60;
                //roomProperties["GameStarted"] = true;
                this.photonView.RPC(nameof(UpdateText), RpcTarget.All, "Match Starts");
                Player[] players = PhotonNetwork.PlayerList;
                foreach (var plr in players)
                {
                    photonView.RPC(nameof(Spawn), plr);
                    AddPlr(plr);
                };
                Debug.Log(players);
            }
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            //roomProperties["TimeLeft"] = PhotonNetwork.Time + 10;
            TimeLeft = PhotonNetwork.Time + 10;
            this.photonView.RPC(nameof(UpdateText), RpcTarget.All, $"Not Enough Players");
            yield return new WaitForSeconds(0.1f);
        }
        //SetHashes();
        coroutineStarted = false;
    }
    IEnumerator MidGame()
    {
        if (CanStart())
        {
            double timLeft = TimeLeft - PhotonNetwork.Time;
            this.photonView.RPC(nameof(UpdateText), RpcTarget.All, $"{Mathf.Round((float)timLeft)}");
            if (timLeft <= 0)
            {
                Proggress = GameProgress.End;
            }
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            Proggress = GameProgress.End;
            yield return new WaitForSeconds(0.1f);
        }
        coroutineStarted = false;
    }
    //public override void OnMasterClientSwitched(Player newMasterClient)
    //{
    //    SetHashes();
    //    GameStarted = (bool)PhotonNetwork.CurrentRoom.CustomProperties["GameStarted"];
    //}
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (coroutineStarted) return;
            if (Proggress == GameProgress.MidGame)
            {
                StartCoroutine(MidGame());
            }
            else if(Proggress == GameProgress.Intermission)
            {
                coroutineStarted = true;
                StartCoroutine(InterMission());
            }else if(Proggress == GameProgress.End)
            {
                photonView.RPC(nameof(KillAll), RpcTarget.All);
                Proggress = GameProgress.Intermission;
                TimeLeft = PhotonNetwork.Time + 10;
                InGamePlayers.Clear();
            }
        }
    }
    public void AddPlr(Player gameplayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        InGamePlayers.Add(gameplayer);
        Debug.Log($"Players In Game {InGamePlayers.Count}");
    }
    //void SetHashes()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    //    }
    //}
    //private void Start()
    //{        //roomProperties["TimeLeft"] = PhotonNetwork.Time + 10;
    //    //roomProperties["GameStarted"] = false;
    //    //SetHashes();
    //}
    [PunRPC]
    public void Spawn()
    {
        PhotonNetwork.Instantiate(PlayerPrefab.name, Vector3.zero, Quaternion.identity);
    }
    [PunRPC]
    public void KillAll()
    {
        PhotonNetwork.Destroy(Clienthealth.gameObject);
        AvSpectators.Clear();
    }
    [PunRPC]
    public void UpdateText(string text)
    {
        InterText.text = text;
    }
    //[PunRPC]
    //public void UpdateGame(double time, bool game)
    //{
    //    print("UpdateGameMethodStarted");
    //    TimeLeft = time;
    //    GameStarted = game;
    //}
    [PunRPC]
    public void Died(int actornumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Player plr = PhotonNetwork.CurrentRoom.GetPlayer(actornumber);
        if (plr != null)
        {
            InGamePlayers.Remove(plr);
            if (InGamePlayers.Count <= 1)
            {
                Proggress = GameProgress.End;
            }
        }
        print($"Actor Num:{actornumber}  plr: {plr}");
        foreach (var item in InGamePlayers)
        {
            print($"player id:{item.UserId} Player actorNumber: {item.ActorNumber} Player Nickname: {item.NickName}");
        }
    }
}
