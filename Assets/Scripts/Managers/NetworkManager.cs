using NetworkModule;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    // Sockets
    private Socket socket;

    // Flags
    public bool IsConnected => socket == null ? false : socket.Connected;

    // Functions
    private void Awake()
    {
        instance = this;
    }

    public void Connect(string ip)
    {
        if (IsConnected)
        {
            return;
        }

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        socket.BeginConnect(IPAddress.Parse(ip), Config.SERVER_PORT, ConnectCallback, null).AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(10000), false);
    }

    private void ConnectCallback(IAsyncResult result)
    {
        socket.EndConnect(result);
    }

    public void SendDataToServer(Packet packet)
    {
        if (!IsConnected)
        {
            return;
        }

        byte[] data = Encoding.ASCII.GetBytes($"{packet.Command}|{packet.Message}");

        socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, null);
    }

    private void SendCallback(IAsyncResult result)
    {
        if (!IsConnected)
        {
            return;
        }

        socket.EndSend(result);
    }

    public void Disconnect()
    {
        if (!IsConnected)
        {
            return;
        }

        socket.Close();

        socket = null;
    }
}
