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
        // 대소문자 구분함
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
                

                if(GUI.Button(new Rect(20, 70, 220, 40), "서버셋팅")){
                    SetPhotonInfo();
                }
                userId = GUI.TextField(new Rect(20, 120, 220, 40), userId);

                GUI.Button(new Rect((guiWidth - 240), 70, 220, 40), netWorkStatus);
                roomName = GUI.TextField(new Rect((guiWidth - 240), 120, 220, 40), roomName);
                
                if (GUI.Button(new Rect(25, 180, 110, 30), "서버접속"))
                {
                    PhotonNetwork.ConnectUsingSettings();
                }
                if(GUI.Button(new Rect(140, 180, 110, 30), "서버접속해제"))
                {
                    PhotonNetwork.Disconnect();
                }
                if (GUI.Button(new Rect(255, 180, 110, 30), "로비접속"))
                {
                    PhotonNetwork.JoinLobby();
                }


                if(GUI.Button(new Rect(370, 180, 110, 30), "로비접속해제"))
                {
                    PhotonNetwork.LeaveLobby();
                }

                if(GUI.Button(new Rect(25, 230, 110, 30), "방만들기"))
                {
                    if (roomName != null)
                    {
                        PhotonNetwork.CreateRoom(roomName, roomOptions);
                    }
                }
                if(GUI.Button(new Rect(140, 230, 110, 30), "방참가"))
                {
                    PhotonNetwork.JoinRoom(roomName);
                }

                if(GUI.Button(new Rect(255, 230, 110, 30), "랜덤참가/새방"))
                {
                    PhotonNetwork.JoinRandomRoom();
                }
                if(GUI.Button(new Rect(370, 230, 110, 30), "방떠나기"))
                {
                    PhotonNetwork.LeaveRoom();
                }

                if (GUI.Button(new Rect(guiWidth - 50, 10, 40, 40), "X"))
                {
                    isOpen = false;
                }

                if (GUI.Button(new Rect(25, 270, 110, 30), "룸리스트"))
                {
                    //PhotonNetwork.GetCustomRoomList(null, LobbyType.Default);
                }

                /************* Chatting 관련 ****************************/
                chatData = GUI.TextField(new Rect(140, 270, 225, 30), chatData);

                if (GUI.Button(new Rect(370, 270, 110, 30), "보내기"))
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
                if (GUI.Button(new Rect(Screen.width - 80, 40, 40, 40), "메뉴"))
                {
                    isOpen = true;
                }
            }
        }

        /// <summary>
        /// 포톤 접속 정보 세팅
        /// </summary>
        void SetPhotonInfo()
        {
            // 같은 룸의 유저들에게 자동 씬 로딩
            PhotonNetwork.AutomaticallySyncScene = true;
            // 같은 버전 접속 허용
            PhotonNetwork.GameVersion = version;
            // 아이디 할당
            PhotonNetwork.NickName = userId;
            // 통신 횟수 설정. Default 초당 30회
            // PhotonNetwork.SendRate = 30;

            // 룸 옵션
            // 최대 접속자 수. 무료 버전 최대 접속자가 20명
            roomOptions.MaxPlayers = 2;
            // 룸을 열어놓을 것인지
            roomOptions.IsOpen = true;
            // 로비에서 룸 목록 노출
            roomOptions.IsVisible = true;
        }

        #region 콜백 함수 모음

        #region 완료 함수
        /// <summary>
        /// 포톤 서버 접속 후 콜백
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("마스터에 접속");
            statusNum = EnumStatus.Connected;
            netWorkStatus = ServerStatus[(int)statusNum];
            //PhotonNetwork.JoinLobby();
        }
        /// <summary>
        /// 포톤 서버 해제 후 콜백
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
        /// 포톤 서버 로비 접속 후 콜백
        /// </summary>
        public override void OnJoinedLobby()
        {
            Debug.Log("로비에 접속 완료");
            statusNum = EnumStatus.JoinLobby;
            netWorkStatus = ServerStatus[(int)statusNum];
            // 로비 접속시 룸 리스트 클리어
            cachedRoomList.Clear();
        }
        /// <summary>
        /// 포톤 서버 로비 접속 해제 후 콜백
        /// </summary>
        public override void OnLeftLobby()
        {
            Debug.Log("로비에 접속 해제");
            cachedRoomList.Clear();
            statusNum = EnumStatus.LeaveRobby;
            netWorkStatus = ServerStatus[(int)statusNum];
        }
        /// <summary>
        /// 포톤 서버 룸 접속 해제 후 콜백
        /// </summary>
        public override void OnLeftRoom()
        {
            Debug.Log("로비에 접속 해제");
            statusNum = EnumStatus.LeaveRoom;
            netWorkStatus = ServerStatus[(int)statusNum];
        }
        /// <summary>
        /// 방 생성 후 콜백
        /// </summary>
        public override void OnCreatedRoom()
        {
            Debug.Log("룸이 생성되었습니다.");
            Debug.Log("생성된 룸 이름 : " + PhotonNetwork.CurrentRoom.Name);
            statusNum = EnumStatus.CreateRoom;
            netWorkStatus = ServerStatus[(int)statusNum];
        }
        /// <summary>
        /// 방 접속 후 콜백
        /// </summary>
        public override void OnJoinedRoom()
        {
            Debug.Log("룸이 접속되었습니다.");
            statusNum = EnumStatus.JoinRoom;
            netWorkStatus = ServerStatus[(int)statusNum];

            GameObject obj = PhotonNetwork.Instantiate("Player", wayPoint[PhotonNetwork.CurrentRoom.PlayerCount - 1].position, Quaternion.identity);
            if (obj.GetPhotonView().IsMine)
            {
                obj.tag = "Player";
            }
            
        }


        #endregion

        #region 실패함수
        /// <summary>
        /// 포톤 로비 접속 후 랜덤 방 접속 실패 콜백 => 새로운 방을 생성
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("랜덤 매치에 실패 : retunCode : " + returnCode + ", message : " + message);
            PhotonNetwork.CreateRoom(null, roomOptions);
        }
        /// <summary>
        /// 룸 생성 실패
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("룸 생성 실패 : retunCode : " + returnCode + ", message : " + message);
            statusNum = EnumStatus.FailedCreateRoom;
            netWorkStatus= ServerStatus[(int)statusNum];
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.Log("룸 접속 : " + newPlayer);
            player.Add(newPlayer.UserId);
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log("룸 접속 해제 : " +  otherPlayer);
            player.Remove(otherPlayer.UserId);
        }

        #endregion

        // 실패 함수



        #endregion

        /// <summary>
        /// 룸리스트 담기
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
        /// 룸 리스트 업데이트(로비에 접속시 자동으로 호출됨)
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
