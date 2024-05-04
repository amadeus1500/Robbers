using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UiMngr : MonoBehaviourPunCallbacks
{
    [SerializeField] MRoom RM;
    [SerializeField] TMP_InputField InputField;
    [SerializeField] GameObject Content;
    List<RoomInfo> RoomList;
    private void Start()
    {
        CreateRoom();
    }
    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinLobby();
        Debug.Log("Connected");
    }
    //public override void OnJoinedLobby()
    //{
    //    base.OnJoinedLobby();
    //    Debug.Log("Joined Jobby");
    //    CreateRoom();
    //}
    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.CreateRoom(InputField.text, new RoomOptions() { MaxPlayers = 4, IsVisible = true, IsOpen = true });
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting");
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        //foreach (var room in roomList)
        //{
        //    if (room.RemovedFromList)
        //    {
        //        MadeRooms.Remove(room);
        //    }
        //    else
        //    {
        //        MadeRooms.Add(room);
        //    }
        //}
        //foreach (var room in MadeRooms)
        //{
        //    MRoom rm = Instantiate(RM, Content.transform);
        //    rm.roomname_ = room.Name;
        //}
        if (p_list == null) return;
        base.OnRoomListUpdate(p_list);

        Transform listcontent = Content.transform;
        Debug.Log("P_list count: " + p_list.Count);
        //RoomList = new List<RoomInfo>();
        foreach (var oneroomdata in p_list)
        {
            if (oneroomdata.RemovedFromList == true || oneroomdata.IsOpen == false)
            {
                RoomList.Remove(oneroomdata);
                continue;
            }
            RoomList.Add(oneroomdata);
        }
        Debug.Log("roomList count: " + RoomList.Count);
        //clearRoomList();//clears the old list in display
        foreach (var room in RoomList)
        {
            Debug.LogWarning("creating new room panel");
            MRoom newRoomPanel = Instantiate(RM, listcontent) as MRoom;//creates new room data panel
            newRoomPanel.roomname_ = room.Name;
            //newRoomPanel.transform.Find("players in room").GetComponent<Text>().text = room.PlayerCount + "/" + room.MaxPlayers;//sets no. of players in room currently
            //newRoomPanel.transform.Find("join room Button").GetComponent<Button>().onClick.AddListener(delegate { joinRoom(newRoomPanel.transform); });//allows the button to be used to connect
        }

    }
    //public override void OnCreateRoomFailed(short returnCode, string message)
    //{
    //    Debug.Log(returnCode);
    //    Debug.Log(message);
    //}
    //public override void OnCreatedRoom()
    //{
    //    base.OnCreatedRoom();
    //    Debug.Log("Room Got Created");
    //}
    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();
    //    Debug.Log("Joined Room");
    //    PhotonNetwork.Instantiate(Player.name, Vector3.zero, Quaternion.identity);
    //}
}
