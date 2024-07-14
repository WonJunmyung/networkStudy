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
        private string userId = "�÷��̾�1";

        private void Awake()
        {
            DontDestroyOnLoad(this);
            // ���� ���� �����鿡�� �ڵ� �� �ε�
            PhotonNetwork.AutomaticallySyncScene = true;
            // ���� ���� ���� ���
            PhotonNetwork.GameVersion = version;
            // ���̵� �Ҵ�
            PhotonNetwork.NickName = userId;
            // ��� Ƚ�� ����. Default �ʴ� 30ȸ
            Debug.Log(PhotonNetwork.SendRate);
            // ���� ����
            //PhotonNetwork.ConnectUsingSettings();

        }

        // ���� ���� ���� �� �ݹ�
        public override void OnConnectedToMaster()
        {
            Debug.Log("�����Ϳ� ����");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("�κ� ���� �Ϸ�");
            // ���� ��ġ
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("���� ��ġ�� ���� : retunCode : " + returnCode + ", message : " + message);

            RoomOptions roomOptions = new RoomOptions();
            // �ִ� ������ ��. ���� ���� �ִ� �����ڰ� 20��
            roomOptions.MaxPlayers = 20;
            // ���� ������� ������
            roomOptions.IsOpen = true;
            // �κ񿡼� �� ��� ����
            roomOptions.IsVisible = true;

            PhotonNetwork.CreateRoom("Room0", roomOptions);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("���� �����Ǿ����ϴ�.");
            Debug.Log("������ �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("�� ���� : " + PhotonNetwork.InRoom);
            Debug.Log("���� ���� �÷��̼� �� : " + PhotonNetwork.CurrentRoom.PlayerCount);

            foreach(var player in PhotonNetwork.CurrentRoom.Players)
            {
                Debug.Log("���ӵ� �÷��̾� �г��� : " + player.Value.NickName + " , �г��� ������ȣ : " + player.Value.ActorNumber);
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
