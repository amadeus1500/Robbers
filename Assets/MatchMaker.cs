using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MatchMaker : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform Content;
    [SerializeField] MRoom RM;
    [SerializeField] TMP_InputField RoomNameField;
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private Dictionary<string, GameObject> AddedRooms = new Dictionary<string, GameObject>();
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }
    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.InRoom) return;
        RoomOptions info  = new RoomOptions();
        PhotonNetwork.CreateRoom(RoomNameField.text, info);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Room Got created: " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
    }
    public void ForListChange(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
        AddedRooms.Clear();
        foreach (var item in cachedRoomList)
        {
            MRoom room = Instantiate(RM, Content);
            room.roomname_ = item.Value.Name;
            room.gameObject.GetComponent<Button>().onClick.AddListener(() => {
                PhotonNetwork.JoinRoom(item.Value.Name);
            });
            AddedRooms[room.gameObject.name] = room.gameObject;
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ForListChange(roomList);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined The Room");
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined The Lobby");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to master: " + PhotonNetwork.CloudRegion);
        Debug.Log("Connecting to Lobby...");
        PhotonNetwork.JoinLobby();
    }
}
