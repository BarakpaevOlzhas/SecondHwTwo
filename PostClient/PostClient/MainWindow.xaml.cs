using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PostClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Socket socket;
        const int port = 12345;

        public MainWindow()
        {
            InitializeComponent();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),port);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(iPEndPoint);            

            Thread thread = new Thread(TakeMessages);
            thread.Start();
        }

        private void TakeMessages(object obj)
        {
            byte[] recBuf = new byte[4 * 1024];
            while (true)
            {
                try
                {
                    int recSize = socket.Receive(recBuf);

                    string text = Encoding.UTF8.GetString(recBuf,0,recSize);

                    Dispatcher.Invoke(() =>
                    {
                        textBlockNamePlace.Text = text;
                    });
                }
                catch (Exception)
                {

                }
            }            
        }

        private void ButtonClickFind(object sender, RoutedEventArgs e)
        {
            PostInfo postInfo = new PostInfo {                
                Text = textBoxPostIndexFind.Text
            };

            string json = JsonConvert.SerializeObject(postInfo);

            socket.Send(Encoding.UTF8.GetBytes(json));
        }

        private void ButtonClickAdd(object sender, RoutedEventArgs e)
        {
            PostInfo postInfo = new PostInfo
            {                
                NamePlace = textBoxNamePlaceGet.Text,
                PostIndex = textBoxPostIndexGet.Text
            };
            
            string json = JsonConvert.SerializeObject(postInfo);

            socket.Send(Encoding.UTF8.GetBytes(json));
        }
    }
}
