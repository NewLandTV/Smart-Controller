using System.Runtime.InteropServices;

internal static class Mouse
{
    // Enums
    public enum Event
    {
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        RightDown = 0x0008,
        RightUp = 0x0010
    }

    public enum Direction
    {
        Up,
        Left,
        Down,
        Right
    }

    // Structs
    public struct Point
    {
        public int x;
        public int y;
    }

    // Functions
    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point lpPoint);

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int x, int y);

    public static void ButtonEvent(Event mouseEvent)
    {
        mouse_event((uint)mouseEvent, 0, 0, 0, 0);
    }

    public static void MoveTo(Direction direction)
    {
        GetCursorPos(out Point point);

        switch (direction)
        {
            case Direction.Up:
                if (point.y - 1 < 0)
                {
                    return;
                }

                SetCursorPos(point.x, point.y - 1);

                break;
            case Direction.Left:
                if (point.x - 1 < 0)
                {
                    return;
                }

                SetCursorPos(point.x - 1, point.y);

                break;
            case Direction.Down:
                if (point.y + 1 > 1080)
                {
                    return;
                }

                SetCursorPos(point.x, point.y + 1);

                break;
            case Direction.Right:
                if (point.x + 1 > 1920)
                {
                    return;
                }

                SetCursorPos(point.x + 1, point.y);

                break;
        }
    }
}
