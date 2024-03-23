using NetworkModule;
using System.Collections;
using UnityEngine;

public class MouseMoveButton : KeyButton
{
    // Enums
    public enum Direction
    {
        Up,
        Left,
        Down,
        Right
    }

    // Datas
    [SerializeField]
    private Direction direction;

    // Flags
    private bool onPressed;

    private void Awake()
    {
        InitializeOriginColor();
    }

    private IEnumerator Start()
    {
        while (true)
        {
            if (onPressed)
            {
                NetworkManager.instance.SendDataToServer(new Packet(Packet.CMD_C2S_MOUSE_MOVE, $"{(int)direction}"));
            }

            yield return null;
        }
    }

    protected override void DownEvent()
    {
        onPressed = true;
    }

    protected override void UpEvent()
    {
        onPressed = false;
    }
}
