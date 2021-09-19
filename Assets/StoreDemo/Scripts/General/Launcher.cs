using Balancy;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField]
    private string apiGameId;
    [SerializeField]
    private string publicKey;

    private void Start()
    {
        Main.Init(new AppConfig
        {
            ApiGameId = apiGameId,
            PublicKey = publicKey,
            Environment = Constants.Environment.Development,
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
}
