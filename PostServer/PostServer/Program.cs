using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace PostServer
{
    class Program
    {
        static TcpListener listener;
        const int port = 12345;

        static List<Socket> sockets = new List<Socket>();

        static void Main(string[] args)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);

                listener.Start();

                Thread thread = new Thread(TakeClient);
                thread.Start();
            }
            catch (Exception)
            {

            }
        }

        static void GetPostInfo(object obj)
        {
            byte[] recBuf = new byte[4 * 1024];
            Socket socket = (Socket)obj;
            string textWithNames = "";            
            while (true)
            {                
                int resSize = socket.Receive(recBuf);               
                    string json = Encoding.UTF8.GetString(recBuf, 0, resSize);
                    PostInfo postInfo = JsonConvert.DeserializeObject<PostInfo>(json);

                    if (postInfo.Text == null)
                    {
                        using (var context = new PostContext())
                        {
                            context.PostInfos.Add(postInfo);
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        using (var context = new PostContext())
                        {
                            foreach (var i in context.PostInfos)
                            {
                                if (i.PostIndex == postInfo.Text)
                                {
                                    textWithNames += i.NamePlace + "\n";
                                }
                            }
                        }
                        recBuf = Encoding.UTF8.GetBytes(textWithNames);
                        socket.Send(recBuf, textWithNames.Length, SocketFlags.None);
                    }                
                Thread.Sleep(10);
            }
        }

        static void TakeClient(object obj)
        {
            while (true)
            {
                Socket socket = listener.AcceptSocket();
                sockets.Add(socket);
                ThreadPool.QueueUserWorkItem(GetPostInfo, socket);
            }
        }

    }
}
