using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using Photon.Realtime;
using System;
using UnityEngine.UI;

namespace Silly
{
    public class GUINetwork : MonoBehaviourPunCallbacks
    {
        public GUISkin skin;
        public bool toggleText;
        public int toolbarInt = 0;
        public int selGridInt = 0;
        public float hSliderValue = 0.0f;
        public float hSbarValue;
        public string netWorkStatus = "netWorkStatus";
        public float guiWidth = 1000f;
        public float guiHeight = 500.0f;

        public Transform[] wayPoint;
        

        //private string networkStatus = "netWorkStatus";

        private string[] ServerStatus = new string[] {
            "ServerStatus",
            "Connected",
            "JoinLobby",
            "Disconnected",
            "CreateRoom",
            "FailedCreateRoom",
            "JoinRoom",
            "LeaveRobby",
            "LeaveRoom",
        };
        
        private enum EnumStatus
        {
            ServerStatus,
            Connected,
            JoinLobby,
            Disconnected,
            CreateRoom,
            FailedCreateRoom,
            JoinRoom,
            LeaveRobby,
            LeaveRoom,
        }

        private EnumStatus statusNum = EnumStatus.ServerStatus;

        List<string> player = new List<string>();
        



        bool isOpen = false;

        private readonly string version = "1.0f";
        // ��ҹ��� ������
        private string userId = "user1";
        private string roomName = "room0";
        private string chatData = "";
        private string stringToEdit = "";

        RoomOptions roomOptions = new RoomOptions();
        
        private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();


        public void Awake()
        {
            SetPhotonInfo();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGUI()
        {
            GUI.skin = skin;

            if (isOpen)
            {
                GUI.BeginGroup(new Rect((Screen.width - guiWidth) / 2, (Screen.height - guiHeight) / 2, guiWidth, guiHeight));
                GUI.Box(new Rect(0, 0, guiWidth, guiHeight), "Network GUI");
                

                if(GUI.Button(new Rect(20, 70, 220, 40), "��������")){
                    SetPhotonInfo();
                }
                userId = GUI.TextField(new Rect(20, 120, 220, 40), userId);

                GUI.Button(new Rect((guiWidth - 240), 70, 220, 40), netWorkStatus);
                roomName = GUI.TextField(new Rect((guiWidth - 240), 120, 220, 40), roomName);
                
                if (GUI.Button(new Rect(25, 180, 110, 30), "��������"))
                {
                    PhotonNetwork.ConnectUsingSettings();
                }
                if(GUI.Button(new Rect(140, 180, 110, 30), "������������"))
                {
                    PhotonNetwork.Disconnect();
                }
                if (GUI.Button(new Rect(255, 180, 110, 30), "�κ�����"))
                {
                    PhotonNetwork.JoinLobby();
                }


                if(GUI.Button(new Rect(370, 180, 110, 30), "�κ���������"))
                {
                    PhotonNetwork.LeaveLobby();
                }

                if(GUI.Button(new Rect(25, 230, 110, 30), "�游���"))
                {
                    if (roomName != null)
                    {
                        PhotonNetwork.CreateRoom(roomName, roomOptions);
                    }
                }
                if(GUI.Button(new Rect(140, 230, 110, 30), "������"))
                {
                    PhotonNetwork.JoinRoom(roomName);
                }

                if(GUI.Button(new Rect(255, 230, 110, 30), "��������/����"))
                {
                    PhotonNetwork.JoinRandomRoom();
                }
                if(GUI.Button(new Rect(370, 230, 110, 30), "�涰����"))
                {
                    PhotonNetwork.LeaveRoom();
                }

                if (GUI.Button(new Rect(guiWidth - 50, 10, 40, 40), "X"))
                {
                    isOpen = false;
                }

                if (GUI.Button(new Rect(25, 270, 110, 30), "�븮��Ʈ"))
                {
                    //PhotonNetwork.GetCustomRoomList(null, LobbyType.Default);
                }

                /************* Chatting ���� ****************************/
                chatData = GUI.TextField(new Rect(140, 270, 225, 30), chatData);

                if (GUI.Button(new Rect(370, 270, 110, 30), "������"))
                {
                    stringToEdit += chatData;
                    SendButtonOnClicked();
                    //PhotonNetwork.GetCustomRoomList(null, LobbyType.Default);
                }
                stringToEdit = GUI.TextField(new Rect(25, 320, guiWidth - 50, 40), stringToEdit);
                
                GUI.EndGroup();
            }
            else
            {
                if (GUI.Button(new Rect(Screen.width - 80, 40, 40, 40), "�޴�"))
                {
                    isOpen = true;
                }
            }
        }

        /// <summary>
        /// ���� ���� ���� ����
        /// </summary>
        void SetPhotonInfo()
        {
            // ���� ���� �����鿡�� �ڵ� �� �ε�
            PhotonNetwork.AutomaticallySyncScene = true;
            // ���� ���� ���� ���
            PhotonNetwork.GameVersion = version;
            // ���̵� �Ҵ�
            PhotonNetwork.NickName = userId;
            // ��� Ƚ�� ����. Default �ʴ� 30ȸ
            // PhotonNetwork.SendRate = 30;

            // �� �ɼ�
            // �ִ� ������ ��. ���� ���� �ִ� �����ڰ� 20��
            roomOptions.MaxPlayers = 2;
            // ���� ������� ������
            roomOptions.IsOpen = true;
            // �κ񿡼� �� ��� ����
            roomOptions.IsVisible = true;
        }

        #region �ݹ� �Լ� ����

        #region �Ϸ� �Լ�
        /// <summary>
        /// ���� ���� ���� �� �ݹ�
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("�����Ϳ� ����");
            statusNum = EnumStatus.Connected;
            netWorkStatus = ServerStatus[(int)statusNum];
            //PhotonNetwork.JoinLobby();
        }
        /// <summary>
        /// ���� ���� ���� �� �ݹ�
        /// </summary>
        /// <param name="cause"></param>
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.Log(cause);
            statusNum = EnumStatus.Disconnected;
            Debug.Log(statusNum);
            cachedRoomList.Clear();
            netWorkStatus = ServerStatus[(int)statusNum];
        }
        /// <summary>
        /// ���� ���� �κ� ���� �� �ݹ�
        /// </summary>
        public override void OnJoinedLobby()
        {
            Debug.Log("�κ� ���� �Ϸ�");
            statusNum = EnumStatus.JoinLobby;
            netWorkStatus = ServerStatus[(int)statusNum];
            // �κ� ���ӽ� �� ����Ʈ Ŭ����
            cachedRoomList.Clear();
        }
        /// <summary>
        /// ���� ���� �κ� ���� ���� �� �ݹ�
        /// </summary>
        public override void OnLeftLobby()
        {
            Debug.Log("�κ� ���� ����");
            cachedRoomList.Clear();
            statusNum = EnumStatus.LeaveRobby;
            netWorkStatus = ServerStatus[(int)statusNum];
        }
        /// <summary>
        /// ���� ���� �� ���� ���� �� �ݹ�
        /// </summary>
        public override void OnLeftRoom()
        {
            Debug.Log("�κ� ���� ����");
            statusNum = EnumStatus.LeaveRoom;
            netWorkStatus = ServerStatus[(int)statusNum];
        }
        /// <summary>
        /// �� ���� �� �ݹ�
        /// </summary>
        public override void OnCreatedRoom()
        {
            Debug.Log("���� �����Ǿ����ϴ�.");
            Debug.Log("������ �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
            statusNum = EnumStatus.CreateRoom;
            netWorkStatus = ServerStatus[(int)statusNum];
        }
        /// <summary>
        /// �� ���� �� �ݹ�
        /// </summary>
        public override void OnJoinedRoom()
        {
            Debug.Log("���� ���ӵǾ����ϴ�.");
            statusNum = EnumStatus.JoinRoom;
            netWorkStatus = ServerStatus[(int)statusNum];

            GameObject obj = PhotonNetwork.Instantiate("Player", wayPoint[PhotonNetwork.CurrentRoom.PlayerCount - 1].position, Quaternion.identity);
            if (obj.GetPhotonView().IsMine)
            {
                obj.tag = "Player";
            }
            
        }


        #endregion

        #region �����Լ�
        /// <summary>
        /// ���� �κ� ���� �� ���� �� ���� ���� �ݹ� => ���ο� ���� ����
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("���� ��ġ�� ���� : retunCode : " + returnCode + ", message : " + message);
            PhotonNetwork.CreateRoom(null, roomOptions);
        }
        /// <summary>
        /// �� ���� ����
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("�� ���� ���� : retunCode : " + returnCode + ", message : " + message);
            statusNum = EnumStatus.FailedCreateRoom;
            netWorkStatus= ServerStatus[(int)statusNum];
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.Log("�� ���� : " + newPlayer);
            player.Add(newPlayer.UserId);
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log("�� ���� ���� : " +  otherPlayer);
            player.Remove(otherPlayer.UserId);
        }

        #endregion

        // ���� �Լ�



        #endregion

        /// <summary>
        /// �븮��Ʈ ���
        /// </summary>
        /// <param name="roomList"></param>
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
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
        }

        /// <summary>
        /// �� ����Ʈ ������Ʈ(�κ� ���ӽ� �ڵ����� ȣ���)
        /// </summary>
        /// <param name="roomList"></param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            UpdateCachedRoomList (roomList);
            foreach(RoomInfo roomInfo in roomList)
            {
                Debug.Log("Room Name : " + roomInfo.Name + " , IsOpen : " + roomInfo.IsOpen + ", Player Count : " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers);
            }
        }






        public void SendButtonOnClicked()
        {
            photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, chatData);
            //ReceiveMsg(msg);
            
        }

        [PunRPC]
        public void ReceiveMsg(string msg)
        {
            stringToEdit += msg;
        }




    }
}
