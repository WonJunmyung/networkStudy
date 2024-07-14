using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Xsl;

namespace Silly
{
    /// <summary>
    /// 데이터를 주고 받을 클래스 생성
    /// </summary>
    public class ServerEventArgs : EventArgs
    {
        // 주고 받을 데이터
        public string text;
        // 생성시 데이터를 변수에 저장
        public ServerEventArgs(string text)
        {
            this.text = text;
        }
    }
    /// <summary>
    /// 서버에서 주고 받을 이벤트 핸들러 생성
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="serverEventArg"></param>
    public delegate void ServerEventHandler(object sender, ServerEventArgs serverEventArg);

    
    /// <summary>
    /// 클라이언트 핸들
    /// </summary>
    public class ClientHandle
    {
        // 클라이언트 소켓
        private Socket socketClient;
        public string? clientInfo;
        // 네트워크 엑세스를 위한 데이터 기본 스트림
        private NetworkStream? networkStream = null;
        // 특정 인코딩의 바이트 스트림에서 읽어오기
        private StreamReader? streamReader = null;
        // 특정 인코딩의 바이트 스트림에서 쓰기
        private StreamWriter? streamWriter = null;

        MultiThreadServer Server;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="client"></param>
        public ClientHandle(Socket client, MultiThreadServer server)
        {
            Server = server;
            // 클라이언트 소켓에 담기
            this.socketClient = client;
            // 기본 스트림을 클라이언트 소켓에 할당
            networkStream = new NetworkStream(client);
            // utf-8 인코딩을 사용하는 읽기 네트워크 스트림
            streamReader = new StreamReader(networkStream!, Encoding.GetEncoding("utf-8"));
            // utf-8 인코딩을 사용하는 쓰기 네트워크 스트림
            streamWriter = new StreamWriter(networkStream!, Encoding.GetEncoding("utf-8"));
            // 쓰기 네트워크 스트림을 자동으로 플러시(데이터를 목적지로 전송하고 버퍼를 비움) 한다.
            // defualt값은 true, false로 해놓을 경우 버퍼가 넘치면 예외를 발생시킨다.
            streamWriter.AutoFlush = true;
            
        }


        /// <summary>
        /// 클라이언트가 취할 행동
        /// </summary>
        public void Run()
        {
            string? receiveData;

            while (true)
            {

                // 스트림에서 한줄을 읽고 데이터를 문자열로 반환한다.
                try
                {
                    receiveData = streamReader!.ReadLine();
                    if (receiveData == null)
                    {
                        break;
                    }
                    else
                    {
                        if (receiveData != "")
                        {
                            Console.WriteLine(receiveData);
                        }
                    }
                    // 각 쓰레드에 전송을 해주자
                    WriteClientData(receiveData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
            }
            // 연결 해제
            Disconnect();
        }

        /// <summary>
        /// 연결 해제
        /// </summary>
        public void Disconnect()
        {
            Console.WriteLine("Disconnect");
            // 소켓 및 스트림 닫기
            streamReader?.Close();
            streamWriter?.Close();
            networkStream?.Close();
            socketClient.Close();
        }

        /// <summary>
        /// 받은 메세지를 다시 전송하는 함수
        /// </summary>
        /// <param name="Message"></param>
        public void WriteClientData(string Message)
        {
            if (Message != null && Message != "")
            {
                // 재전송
                streamWriter?.WriteLine(clientInfo + ":" + Message);
                Server.ServerToClient(clientInfo + ":" + Message);
            }
        }

        public void ServerToClient(string Message)
        {
            
            streamWriter?.WriteLine(Message);
            Console.WriteLine("보낸 데이터 : " + Message);
        }


    }

    public class MultiThreadServer
    {
        // 클라이언트 연결 수신
        private TcpListener? server = null;
        // 클라이언트 접속 이벤트
        public event ServerEventHandler? clientConnected = null;
        // 서버 종료 이벤트
        public event ServerEventHandler? serverClosed = null;
        // 클라이언트 핸들 리스트
        public List<ClientHandle> clientHandles = new List<ClientHandle>();
        // ip 주소
        public string? ipAddress = null;
        // 포트
        static int port = 5517;

        /// <summary>
        /// 생성자
        /// </summary>
        public MultiThreadServer()
        {
            SetServer();
        }

        public void Start()
        {
            if(server != null)
            {
                // 서버 시작
                server.Start();

                Thread connect = new Thread(ClientConnect);
                // 스레드 시작
                connect.Start();
            }
        }

        public void Stop()
        {
            server?.Stop();
        }

        /// <summary>
        /// 서버 설정
        /// </summary>
        void SetServer()
        {
            // 로컬 호스트의 이름
            string host = Dns.GetHostName();
            //string host = DisplayLocalHostName();
            if (host == "")
            {
                return;
            }
            // Dns 서버에서 호스트 이름 또는 IP주소와 연결된
            // IP 주소를 가져옴
            IPHostEntry entry = Dns.GetHostEntry(host);
            foreach (IPAddress address in entry.AddressList)
            {
                Console.WriteLine(address);
            }
            IPAddress iPAddress = entry.AddressList[0];
            // EndPoint 설정
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);
            // TCP 네트워크 클라이언트 연결을 수신
            server = new TcpListener(iPEndPoint);
        }

        


        /// <summary>
        /// 스레드에서 사용할 클라이언트 연결
        /// </summary>
        private void ClientConnect()
        {
            while (true)
            {
                try
                {
                    // 클라이언트 접속 대기
                    Socket? client = server?.AcceptSocket();
                    
                    if (client != null)
                    {
                        // 접속된 클라이언트 IP 확인
                        string? clientIP = client.RemoteEndPoint!.ToString();
                        // 연결된 클라이언트가 있다면
                        // 클라이언트 연결 이벤트 실행
                        clientConnected?.Invoke(this, new ServerEventArgs(clientIP!));
                        ClientHandle clientHandle = new ClientHandle(client, this);
                        clientHandle.clientInfo = clientIP!;
                        clientHandles.Add(clientHandle);
                        // 접속된 클라이언트의 객체를 생성 Thread를 생성하여 동작 수행
                        Thread connectedClient = new Thread(new ThreadStart(clientHandle.Run));
                        // 스레드 시작
                        connectedClient.Start();
                    }
                }
                catch (Exception ex)
                {
                    if(serverClosed != null)
                    {
                        serverClosed(this, new ServerEventArgs(ex.ToString()));
                    }
                }
            }
        }

        public void ServerToClient(string Message)
        {
            for (int i = 0; i < clientHandles.Count; i++)
            {
                clientHandles[i].ServerToClient(Message);
            }
        }
    }

    /// <summary>
    /// 동작 클래스
    /// </summary>
    class StartClass()
    {
        // MultiThreadServer class 할당
        private static MultiThreadServer server = new MultiThreadServer();
        // 메인 함수
        static void Main(string[] args)
        {
            // 클라이언트 접속 이벤트
            server.clientConnected += Server_clientConnected;
            // 콘솔에서 키 입력
            ConsoleKeyInfo keyInfo;
            do
            {
                // 입력 키
                keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.S:
                        Console.WriteLine("Start Server");
                        server.Start();
                        break;
                    case ConsoleKey.Q:
                        Console.WriteLine("End Server");
                        server.Stop();
                        break;
                }
            }
            while (keyInfo.Key != ConsoleKey.Escape);
            
        }

        /// <summary>
        /// 서버에 클라이언트가 접속 시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="serverEventArg"></param>
        private static void Server_clientConnected(object sender, ServerEventArgs serverEventArg)
        {
            // 클라이언트가 연결되면 콘솔에 메세지
            Console.WriteLine(serverEventArg.text);
        }

        

    }
}

    