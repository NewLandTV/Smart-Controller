using NetworkModule;
using UnityEngine;

public class MouseButton : KeyButton
{
    // Datas
    [SerializeField]
    private ushort downButton;
    [SerializeField]
    private ushort upButton;

    private void Awake()
    {
        InitializeOriginColor();
    }

    protected override void DownEvent()
    {
        NetworkManager.instance.SendDataToServer(new Packet(Packet.CMD_C2S_MOUSE_EVENT, $"{downButton}"));
    }

    protected override void UpEvent()
    {
        NetworkManager.instance.SendDataToServer(new Packet(Packet.CMD_C2S_MOUSE_EVENT, $"{upButton}"));
    }
}
