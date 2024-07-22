using LoggerSystem.NetworkingLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LoggerRead.Windows
{
    /// <summary>
    /// Interaktionslogik für NetworkDownload.xaml
    /// </summary>
    public partial class NetworkDownload : Window
    {
        public NetworkDownload()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            //Try connect
            string Host = host.Text;
            TcpClient tcpClient;

            PacketV2 packetV2 = new PacketV2();

            packetV2.Command = PacketV2.Commands.GetXmlFiles;

            byte[] data = Serializer.ToByteArray(packetV2);


            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IPEndPoint.Parse(Host));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            var stream = tcpClient.GetStream();

            stream.Write(data, 0, data.Length);
            stream.Flush();

            //Rec
            MemoryStream memoryStream = new MemoryStream();

            Thread.Sleep(4000);


            byte[] buffer = new byte[512];

            stream.ReadTimeout = 1000;

            int numberOfBytesRead;
            do
            {
                numberOfBytesRead = 0;
                try
                {
                    numberOfBytesRead = stream.Read(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    break;
                }

                memoryStream.Write(buffer, 0, numberOfBytesRead);
            }
            while (stream.DataAvailable && numberOfBytesRead > 0);

            try
            {
                packetV2 = Serializer.ToPacketV2(memoryStream);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if(packetV2.Data == null)
            {
                MessageBox.Show("No data");
                return;
            }

            MainWindow.FilesVirtual.Clear();

            foreach (var file in packetV2.Data)
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IPEndPoint.Parse(Host));
                stream = tcpClient.GetStream();

                packetV2.Command = PacketV2.Commands.GetFileContent;
                packetV2.Data = new string[] { file };

                data = Serializer.ToByteArray(packetV2);

                stream.Write(data, 0, data.Length);
                stream.Flush();

                stream.ReadTimeout = 2500;
                memoryStream = new MemoryStream();
                //Read file
                do
                {
                    numberOfBytesRead = 0;
                    try
                    {
                        numberOfBytesRead = stream.Read(buffer, 0, buffer.Length);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        break ;
                    }

                    memoryStream.Write(buffer, 0, numberOfBytesRead);
                }
                while (stream.DataAvailable && stream.Socket.Available > 0 );

                try
                {
                    packetV2 = Serializer.ToPacketV2(memoryStream);
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    continue;
                }

                if(packetV2.Data == null)
                {
                    MessageBox.Show("Error no data");
                    continue;
                }

                MainWindow.FilesVirtual.Add(packetV2.Data.ToList());

            }
            this.Close();
        }
    }
}
