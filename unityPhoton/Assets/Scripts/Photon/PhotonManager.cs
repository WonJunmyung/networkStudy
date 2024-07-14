using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


namespace Silly
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        private readonly string version = "1.0f";
        private string userId = "플레이어1";

        private void Awake()
        {
            DontDestroyOnLoad(this);
            // 같은 룸의 유저들에게 자동 씬 로딩
            PhotonNetwork.AutomaticallySyncScene = true;
            // 같은 버전 접속 허용
            PhotonNetwork.GameVersion = version;
            // 아이디 할당
            PhotonNetwork.NickName = userId;
            // 통신 횟수 설정. Default 초당 30회
            Debug.Log(PhotonNetwork.SendRate);
            // 서버 접속
            //PhotonNetwork.ConnectUsingSettings();

        }

        // 포톤 서버 접속 후 콜백
        public override void OnConnectedToMaster()
        {
            Debug.Log("마스터에 접속");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("로비에 접속 완료");
            // 랜덤 매치
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("랜덤 매치에 실패 : retunCode : " + returnCode + ", message : " + message);

            RoomOptions roomOptions = new RoomOptions();
            // 최대 접속자 수. 무료 버전 최대 접속자가 20명
            roomOptions.MaxPlayers = 20;
            // 룸을 열어놓을 것인지
            roomOptions.IsOpen = true;
            // 로비에서 룸 목록 노출
            roomOptions.IsVisible = true;

            PhotonNetwork.CreateRoom("Room0", roomOptions);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("룸이 생성되었습니다.");
            Debug.Log("생성된 룸 이름 : " + PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("룸 입장 : " + PhotonNetwork.InRoom);
            Debug.Log("현재 룸의 플레이서 수 : " + PhotonNetwork.CurrentRoom.PlayerCount);

            foreach(var player in PhotonNetwork.CurrentRoom.Players)
            {
                Debug.Log("접속된 플레이어 닉네임 : " + player.Value.NickName + " , 닉네임 고유번호 : " + player.Value.ActorNumber);
            }

            //Transform[] points = GameObject.Find("SpawnPointerGroup").GetComponentsInChildren<Transform>();
            //int idx = Random.Range(1, points.Length);

            //PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation);
        }

        


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
