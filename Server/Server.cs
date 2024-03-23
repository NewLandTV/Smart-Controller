using NetworkModule;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal sealed class Server
{
    // Constants
    private readonly string SMART_CONTROLLER_SERVER_FOLDER_PATH;
    private readonly string LOG_FILE_PATH;

    // Sockets
    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private Socket client;

    // Datas
    private byte[] receiveBuffer;

    // Flags
    private bool isRunning;
    public bool IsRunning => isRunning;

    private bool connected;
    public bool Connected => connected;

    // Enums
    public enum LogType
    {
        Log,
        Warning,
        Error
    }

    // Constructors
    public Server()
    {
        try
        {
            Console.Title = "Smart Controller Server";

            // Initialize constants
            SMART_CONTROLLER_SERVER_FOLDER_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SmartControllerServer");
            LOG_FILE_PATH = Path.Combine(SMART_CONTROLLER_SERVER_FOLDER_PATH, "Log.txt");

            // Delete log file
            if (File.Exists(LOG_FILE_PATH))
            {
                File.Delete(LOG_FILE_PATH);
            }

            // Initialize socket and listen
            socket.Bind(new IPEndPoint(IPAddress.Any, Config.SERVER_PORT));
            socket.Listen();

            isRunning = true;

            Log(LogType.Log, "Started server!");
        }
        catch (Exception ex)
        {
            Log(LogType.Error, ex.Message);
            Shutdown();
        }
    }

    // Functions
    public void AcceptClient()
    {
        if (!isRunning || connected)
        {
            return;
        }

        try
        {
            Log(LogType.Log, "Waiting for client connection...");

            socket.BeginAccept(AcceptCallback, null).AsyncWaitHandle.WaitOne();
        }
        catch (Exception ex)
        {
            Log(LogType.Error, ex.Message);
        }
    }

    private void AcceptCallback(IAsyncResult result)
    {
        if (!isRunning || connected)
        {
            return;
        }

        try
        {
            client = socket.EndAccept(result);
            connected = true;

            Log(LogType.Log, "Client connected.");
        }
        catch (Exception ex)
        {
            Log(LogType.Error, ex.Message);
        }
    }

    public void ReceiveDataFromClient()
    {
        if (!isRunning || !connected)
        {
            return;
        }

        try
        {
            receiveBuffer = new byte[Packet.MAX_SIZE];

            client.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, null).AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(10000), false);
        }
        catch (Exception ex)
        {
            Log(LogType.Error, ex.Message);
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        if (!isRunning || !connected)
        {
            return;
        }

        try
        {
            // Receive data
            int receiveBytes = client.EndReceive(result);

            // Disconnect
            if (receiveBytes == 0)
            {
                ClientDisconnect();

                return;
            }

            string[] split = Encoding.ASCII.GetString(receiveBuffer).Split('|', 2);

            ExecutePacket(new Packet(ushort.TryParse(split[0], out ushort command) ? command : Packet.CMD_INVALID, split[1]));
        }
        catch (Exception ex) when (ex is SocketException)
        {
            ClientDisconnect();
        }
        catch (Exception ex)
        {
            Log(LogType.Error, ex.Message);
        }
    }

    /// <summary>
    /// It is executed according to the command type.
    /// </summary>
    /// <param name="command">The type of command.</param>
    private void ExecutePacket(Packet packet)
    {
        if (!isRunning)
        {
            return;
        }

        switch (packet.Command)
        {
            case Packet.CMD_C2S_OPEN:
                if (!ushort.TryParse(packet.Message, out ushort action))
                {
                    return;
                }

                switch (action)
                {
                    case 0:
                        Log(LogType.Log, "Open JkhTV Youtube");

                        Process.Start(new ProcessStartInfo("https://www.youtube.com/@NewLand2019-JkhTV") { UseShellExecute = true });

                        break;
                    case 1:
                        Log(LogType.Log, "Open Google");

                        Process.Start(new ProcessStartInfo("https://www.google.com") { UseShellExecute = true });

                        break;
                    case 2:
                        Log(LogType.Log, "Open NewLand Naver Cafe");

                        Process.Start(new ProcessStartInfo("https://cafe.naver.com/2019newland") { UseShellExecute = true });

                        break;
                    case 3:
                        Log(LogType.Log, "Open NewLand TV Github");

                        Process.Start(new ProcessStartInfo("https://github.com/NewLandTV") { UseShellExecute = true });

                        break;
                }

                break;
            case Packet.CMD_C2S_KEY_EVENT:
                {
                    string[] split = packet.Message.Split('|');

                    if (!Enum.TryParse(split[0], out Keyboard.KeyCode keyCode) || !ushort.TryParse(split[1], out ushort eventType))
                    {
                        return;
                    }

                    switch (eventType)
                    {
                        case 0:
                            Log(LogType.Log, $"{Enum.GetName(keyCode)} Key Down");

                            Keyboard.KeyDown(keyCode);

                            break;
                        case 1:
                            Log(LogType.Log, $"{Enum.GetName(keyCode)} Key Up");

                            Keyboard.KeyUp(keyCode);

                            break;
                    }

                    break;
                }
            case Packet.CMD_C2S_MOUSE_EVENT:
                if (!Enum.TryParse(packet.Message, out Mouse.Event mouseEvent))
                {
                    return;
                }

                Log(LogType.Log, $"{Enum.GetName(mouseEvent)} Mouse Event");

                Mouse.ButtonEvent(mouseEvent);

                break;
            case Packet.CMD_C2S_MOUSE_MOVE:
                if (!Enum.TryParse(packet.Message, out Mouse.Direction direction))
                {
                    return;
                }

                Log(LogType.Log, $"Move {Enum.GetName(direction)} Mouse Cursor");

                Mouse.MoveTo(direction);

                break;
        }
    }

    /// <summary>
    /// Output message to log file and to the screen.
    /// </summary>
    /// <param name="logType">The type of log to output.</param>
    /// <param name="message">The log message to output.</param>
    public void Log(LogType logType, string message)
    {
        if (!isRunning)
        {
            return;
        }

        FileStream? fileStream = null;

        try
        {
            // Create folder or file if they don't exist
            if (!Directory.Exists(SMART_CONTROLLER_SERVER_FOLDER_PATH))
            {
                Directory.CreateDirectory(SMART_CONTROLLER_SERVER_FOLDER_PATH);
            }

            string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{logType}] {message}";
            byte[] lineBytes = Encoding.UTF8.GetBytes($"{line}\n");

            Console.WriteLine(line);

            // Write line to file
            fileStream = new FileStream(LOG_FILE_PATH, FileMode.Append, FileAccess.Write);

            fileStream.Write(lineBytes, 0, lineBytes.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            fileStream?.Dispose();
        }
    }

    private void ClientDisconnect()
    {
        if (!connected)
        {
            return;
        }

        connected = false;

        client.Close();

        Log(LogType.Log, "Client disconnected.");
    }

    public void Shutdown()
    {
        if (!isRunning)
        {
            return;
        }

        ClientDisconnect();

        isRunning = false;

        socket.Close();

        Log(LogType.Log, "Server is shutdown!");
    }
}
