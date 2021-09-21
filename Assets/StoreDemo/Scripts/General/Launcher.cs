using System.Collections.Generic;
using Balancy;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    private const string GAME_ID = "game_id";
    private const string PUBLIC_KEY = "public_key";
    
    [SerializeField]
    private string apiGameId;
    [SerializeField]
    private string publicKey;

    private Dictionary<string, string> _urlParams;

    private void Start()
    {
        _urlParams = ParseUrl();
        
        Main.Init(new AppConfig
        {
            ApiGameId = GetGameId(),
            PublicKey = GetPublicKey(),
            Environment = Constants.Environment.Development,
            Platform = Constants.Platform.AndroidGooglePlay,
            OnReadyCallback = response =>
            {
                Debug.Log("Balancy Init  " + response.Success);
                if (response.Success)
                    GlobalEvents.InvokeBalancyInitialized();
                else
                    Controller.PrintAllErrors();
            }
        });
    }

    private string GetGameId()
    {
        if (HasBothGameIDAndPublicKey())
            return _urlParams[GAME_ID];

        return apiGameId;
    }
    
    private string GetPublicKey()
    {
        if (HasBothGameIDAndPublicKey())
            return _urlParams[PUBLIC_KEY];

        return publicKey;
    }

    private bool HasBothGameIDAndPublicKey()
    {
        return _urlParams.ContainsKey(GAME_ID) && _urlParams.ContainsKey(PUBLIC_KEY);
    }

    private Dictionary<string, string> ParseUrl()
    {
        var paramsDict = new Dictionary<string, string>();

        var url = Application.absoluteURL;
        var splitUrl = url.Split('?');
        if (splitUrl.Length == 2)
        {
            var prms = splitUrl[1];
            var splitParams = prms.Split('&');
            foreach (var kvp in splitParams)
            {
                var splitKvp = kvp.Split('=');
                if (splitKvp.Length == 2)
                {
                    if (!paramsDict.ContainsKey(splitKvp[0]))
                        paramsDict.Add(splitKvp[0], splitKvp[1]);
                }
            }
        }

        return paramsDict;
    }
}
