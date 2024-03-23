namespace NetworkModule
{
    public class Packet
    {
        // Constants
        public const ushort MAX_SIZE = 1024;

        // Commands, C2S = Client to Server
        public const ushort CMD_C2S_OPEN = 0;
        public const ushort CMD_C2S_KEY_EVENT = 1;
        public const ushort CMD_C2S_MOUSE_EVENT = 2;
        public const ushort CMD_C2S_MOUSE_MOVE = 3;

        public const ushort CMD_INVALID = ushort.MaxValue;

        // Datas
        private ushort command;
        public ushort Command
        {
            get => command;
            set => command = value;
        }
        private string message;
        public string Message
        {
            get => message;
            set => message = value;
        }

        // Constructors
        public Packet()
        {
            command = CMD_INVALID;
            message = string.Empty;
        }

        public Packet(ushort command) : this()
        {
            this.command = command;
        }

        public Packet(ushort command, string message) : this(command)
        {
            this.message = message;
        }
    }
}