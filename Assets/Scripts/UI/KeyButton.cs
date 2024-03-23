using NetworkModule;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Datas
    [SerializeField]
    protected int keyCode;

    private Color originColor;
    private Color pressedColor;

    // Components
    private Image image;

    private void Awake()
    {
        InitializeOriginColor();
    }

    protected void InitializeOriginColor()
    {
        // Initialize variables
        originColor = Color.white;
        pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);

        // Get components
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    // Event systems
    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = pressedColor;

        DownEvent();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.color = originColor;

        UpEvent();
    }

    protected virtual void DownEvent()
    {
        NetworkManager.instance.SendDataToServer(new Packet(Packet.CMD_C2S_KEY_EVENT, $"{keyCode}|0"));
    }

    protected virtual void UpEvent()
    {
        NetworkManager.instance.SendDataToServer(new Packet(Packet.CMD_C2S_KEY_EVENT, $"{keyCode}|1"));
    }
}
