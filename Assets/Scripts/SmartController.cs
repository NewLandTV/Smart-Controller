using NetworkModule;
using System.Diagnostics;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SmartController : MonoBehaviour
{
    // UI
    [SerializeField]
    private TMP_InputField ipInputField;
    [SerializeField]
    private TextMeshProUGUI versionText;

    // Other components
    [SerializeField]
    private OnlineUpdater onlineUpdater;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        versionText.text = $"v{Application.version}";

        onlineUpdater.CheckLatestVersion((isLatestVersion) =>
        {
            if (isLatestVersion)
            {
                return;
            }

            onlineUpdater.DownloadLatestVersion();
        });
    }

    private void OnApplicationQuit()
    {
        NetworkManager.instance.Disconnect();
    }

    // Button click events
    public void OnConnectButtonClick()
    {
        if (ipInputField.text.Length <= 0)
        {
            return;
        }

        NetworkManager.instance.Connect(ipInputField.text);
    }

    public void OnDisconnectButtonClick()
    {
        NetworkManager.instance.Disconnect();
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Process.GetCurrentProcess().Kill();
#endif
    }

    public void OnOpenButtonClick(int index)
    {
        NetworkManager.instance.SendDataToServer(new Packet(Packet.CMD_C2S_OPEN, $"{index}"));
    }
}
