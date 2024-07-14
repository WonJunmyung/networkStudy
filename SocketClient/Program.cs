using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace silly
{
    class Client
    {
        static int port = 5517;
        static Socket? socketClient;
        static string? clientInfo;
        static void Main(string[] args)
        {
            // 로컬 호스트의 이름
            string host = Dns.GetHostName();
            //string host = DisplayLocalHostName();
            if (host == "")
            {
                return;
            }
            // Dns 서버에서 호스트 이름 또는 IP주소와 연결된 IP 주소를 가져옴
            IPHostEntry entry = Dns.GetHostEntry(host);
            foreach (IPAddress address in entry.AddressList)
            {
                Console.WriteLine(address);
            }
            IPAddress iPAddress = entry.AddressList[0];
            // EndPoint 설정
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);

            // 소켓 생성
            socketClient = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                // 서버에 접속
                socketClient.Connect(iPEndPoint);
                clientInfo = socketClient!.LocalEndPoint!.ToString();
                Console.WriteLine("Connected to Server : " + clientInfo);
                

                #region stream
                NetworkStream networkStream = new NetworkStream(socketClient);
                StreamReader streamReader = new StreamReader(networkStream, Encoding.GetEncoding("utf-8"));
                StreamWriter streamWriter = new StreamWriter(networkStream, Encoding.GetEncoding("utf-8"));

                // 데이터를 받을 리시브를 생성해주자
                Thread receiveThread = new Thread(ReceiveData);
                // streamReader 정보를 가져가기 위해 인자로 삽입
                receiveThread.Start(streamReader);

                // 콘솔에서 키 입력
                ConsoleKeyInfo keyInfo;
                string? receiveData;
                string? sendData = "";
                do
                {
                    // 입력 키
                    keyInfo = Console.ReadKey();
                    sendData += keyInfo.KeyChar;
                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        streamWriter.WriteLine(sendData);
                        streamWriter.Flush();
                        sendData = null;
                        Console.WriteLine();
                    }

                }
                while (keyInfo.Key != ConsoleKey.Escape);
                
                #endregion


                #region send byte
                /***************************** send byte *************************/
                //// 서버에 보낼 텍스트
                //string sendText = "Send Data!";
                //// 서버에 보낼 텍스트 byte 형태로 변환
                //byte[] sendBytes = Encoding.UTF8.GetBytes(sendText);
                //// 서버에게 데이터 전송
                //socketClient.Send(sendBytes);

                //// 데이터를 받을 버퍼 생성
                //byte[] buffer = new byte[socketClient.ReceiveBufferSize];
                //// 데이터를 수신
                //int byteRead = socketClient.Receive(buffer);
                //// 받은 데이터를 UTF8 형식으로 변환
                //string receivedText = Encoding.UTF8.GetString(buffer, 0, byteRead);
                //Console.WriteLine("receivedText : " + receivedText);
                /******************************************************************/
                #endregion
                // 연결 종료 (송수신 모두 사용하지 않도록 설정)
                socketClient.Shutdown(SocketShutdown.Both);
                socketClient.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private static void ReceiveData(object? obj)
        {
            string? receiveData;
            while (true)
            {
                try
                {
                    receiveData = ((StreamReader)obj!).ReadLine();
                    if (receiveData == null)
                    {
                        break;
                    }
                    else
                    {
                        if (receiveData != "")
                        {
                            // 내 아이피가 아니라면
                            if (!receiveData.Contains(clientInfo!))
                            {
                                Console.WriteLine(receiveData);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("서버와의 접속이 종료되었습니다.");
                    socketClient!.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                    Environment.Exit(0);
                }
            }
        }
    }
}