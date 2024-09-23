using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

//Adindan gorsendiyi kimi nadir mellim UDP SERVER :)
await UpUDPServer();
async Task UpUDPServer()
{
    var server = new UdpClient(27001);
    var remoteEP = new IPEndPoint(IPAddress.Loopback, 0);
    server.Connect(remoteEP);
    string command;

    var client = new UdpClient();
    var clientEP = new IPEndPoint(IPAddress.Loopback, 27002);

    while (true)
    {
        var bytes = await server.ReceiveAsync();
        command = Encoding.Default.GetString(bytes.Buffer);
        if (command != null)
        {
            switch (command)
            {
                case "Screenshot":
                    byte[] sendBytes = CaptureScreen();
                    List<byte[]> arrayChunks = sendBytes.Chunk(111).ToList();

                    for (int i = 0; i < arrayChunks.Count; i++)
                    {
                        var chunk = arrayChunks[i];
                        await client.SendAsync(chunk, chunk.Length, clientEP);
                    }
                    var loadEndedMessage = Encoding.Default.GetBytes("Transmission Complete");
                    client.Send(loadEndedMessage, clientEP);
                    break;
            }
        }
    }
}

//Screen eden code hissesi
byte[] CaptureScreen()
{

    Bitmap bm;
    using (MemoryStream ms = new MemoryStream())
    {
        using (bm = new Bitmap(1500, 800))
        {
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.CopyFromScreen(0, 0, 0, 0, bm.Size);
            }
            bm.Save(ms, ImageFormat.Png);
        }
        return ms.ToArray();
    }
}

