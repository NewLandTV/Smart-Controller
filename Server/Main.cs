Server server = new Server();

while (server.IsRunning)
{
    if (!server.Connected)
    {
        server.AcceptClient();
    }
    else
    {
        server.ReceiveDataFromClient();
    }

    Thread.Sleep(1);
}