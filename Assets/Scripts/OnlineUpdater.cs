using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineUpdater : MonoBehaviour
{
    // Datas
    [SerializeField]
    private string versionUrl;
    [SerializeField]
    private string downloadUrl;

    // Functions
    public void CheckLatestVersion(Action<bool> callback)
    {
        //StartCoroutine(CheckLatestVersionCoroutine(callback));
    }

    private IEnumerator CheckLatestVersionCoroutine(Action<bool> callback)
    {
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(versionUrl))
        {
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result != UnityWebRequest.Result.Success || !unityWebRequest.downloadHandler.text.Equals(Application.version))
            {
                callback(false);

                yield break;
            }

            callback(true);
        }
    }

    public void DownloadLatestVersion()
    {
        /*WebClient webClient = new WebClient();

        webClient.DownloadFile(downloadUrl, Path.Combine(Application.persistentDataPath, "Latest.apk"));*/
    }
}
