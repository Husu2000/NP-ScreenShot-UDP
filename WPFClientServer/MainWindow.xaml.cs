using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace WPFClientServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UdpClient UdpServer;
        UdpClient UdpClient;
        IPEndPoint UdpServerEP;
        IPEndPoint UdpClientEP;



        public MainWindow()
        {
            try
            {

                UdpServer = new UdpClient(27002);
                UdpServerEP = new IPEndPoint(IPAddress.Any, 0);
                UdpClient = new UdpClient();
                UdpClientEP = new IPEndPoint(IPAddress.Loopback, 27001);
                InitializeComponent();
                UpUDPServer_Button_Click(new object(), new RoutedEventArgs());
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


        List<byte> recievedByteList = new();
        async Task UpUDPServerAsync() => await Task.Run(() => UpUDPServer());
        async void UpUDPServer()
        {

            try
            {
                while (true)
                {
                    var recievedImageByte = UdpServer.Receive(ref UdpServerEP);
                    if (Encoding.Default.GetString(recievedImageByte) == "Transmission Complete")
                    {
                        var tempList = recievedByteList;
                        Dispatcher.Invoke(() =>
                        {
                            Bitmap bm2 = ByteArrayToBitmapImage(tempList.ToArray());
                            BitmapImage bmi = ConvertBitmapToBitmapImage(bm2);
                            ScreenShoot_Image.Source = bmi;
                        });
                    }
                    else
                    {
                        recievedByteList.AddRange(recievedImageByte);

                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "NOOB");
            }
        }



        async Task SendDataToConsoleServerAsync() => await Task.Run(SendDataToConsoleServer);
        void SendDataToConsoleServer()
        {
            recievedByteList = new();
            var messageBytes = Encoding.Default.GetBytes("Screenshot");
            UdpClient.Send(messageBytes, messageBytes.Length, UdpClientEP);
        }
        Bitmap ByteArrayToBitmapImage(byte[] byteArray)
        {

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                Bitmap bitmap = new Bitmap(stream);
                return bitmap;
            }
        }

        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SendDataToConsoleServer();


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        async private void UpUDPServer_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await UpUDPServerAsync();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;

            }

        }


    }
}