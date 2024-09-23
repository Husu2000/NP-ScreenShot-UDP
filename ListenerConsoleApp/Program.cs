using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


UpServer();
async void UpServer()
{
    var server = new UdpClient(27001);

    var remoteEP = new IPEndPoint(IPAddress.Any, 0);
    string command;


    while (true)
    {
        var bytes = server.Receive(ref remoteEP);
        command = Encoding.Default.GetString(bytes);
        if (command != null)
        {
            switch (command)
            {
                case "Screenshot":
                    var client = new UdpClient();
                    var clientEP = new IPEndPoint(IPAddress.Loopback, 27002);
                    byte[] sendBytes = CaptureScreen();
                    List<byte[]> arrayChunks = SplitArray(sendBytes, 3);
                    var capturedBitmap = CaptureScreenReturnBitmap();
                    var jsonString = JsonSerializer.Serialize(capturedBitmap);
                    var jsonStringBytes = Encoding.Default.GetBytes(jsonString);
                    client.Send(jsonStringBytes, jsonStringBytes.Length, clientEP);
                    //for (int i = 0; i < arrayChunks.Count; i++)
                    //{
                    //    var chunk = arrayChunks[i];
                    //    //client.Send(chunk, chunk.Length, clientEP);
                    //   await client.SendAsync(chunk, chunk.Length, clientEP);
                    //}
                    var loadEndedMessage = Encoding.Default.GetBytes("Transmission Complete");
                    client.Send(loadEndedMessage, clientEP);
                    break;
            }
        }
    }
}
async Task<byte[]> CaptureScreenshotAsync() => await Task.Run(CaptureScreen);
byte[] GetBitmapBytes(Bitmap bitmap)
{
    ImageConverter ic = new ImageConverter();
    return (byte[])ic.ConvertTo(bitmap, typeof(byte[]));
}
static List<T[]> SplitArray<T>(T[] array, int chunkSize)
{
    List<T[]> chunks = new List<T[]>();

    for (int i = 0; i < array.Length; i += chunkSize)
    {
        chunks.Add(array.Skip(i).Take(chunkSize).ToArray());
    }

    return chunks;
}

byte[] CaptureScreen()
{
    Bitmap bm;
    using (MemoryStream ms = new MemoryStream())
    {
        using (bm = new Bitmap(1600, 720))
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
Bitmap CaptureScreenReturnBitmap()
{
    Bitmap bm;

    using (bm = new Bitmap(1600, 720))
    {
        using (Graphics g = Graphics.FromImage(bm))
        {
            g.CopyFromScreen(0, 0, 0, 0, bm.Size);
        }
    }
    return bm;
}
