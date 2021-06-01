using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class JoinRoomMenu : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject roomDisplayPrefab;
    [SerializeField] Transform content;

    List<RoomDisplay> roomDisplayList = new List<RoomDisplay>();


    public override void OnDisable()
    {
        for (int i = 0; i < roomDisplayList.Count; i++)
        {
            Destroy(roomDisplayList[i].gameObject);
        }
        roomDisplayList.Clear();
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo roomInfo in roomList)
        {
            int index = roomDisplayList.FindIndex(x => x.RoomInfo.Name == roomInfo.Name);
            if (roomInfo.RemovedFromList)
            {
                if(index != -1)
                {
                    Destroy(roomDisplayList[index].gameObject);
                    roomDisplayList.RemoveAt(index);
                }
            }
            else if (index == -1)
            {
                RoomDisplay roomDisplay = Instantiate(roomDisplayPrefab, content).GetComponent<RoomDisplay>();
                roomDisplay.SetRoomInfo(roomInfo);
                roomDisplayList.Add(roomDisplay);
            }
        }
    }

}
