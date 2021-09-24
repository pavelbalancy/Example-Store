using UnityEngine;

public class GameLoader : MonoBehaviour
{
    private void Awake()
    {
        GlobalEvents.BalancyInitializedEvent += OnBalancyInitialized;
    }

    private void OnBalancyInitialized()
    {
        GlobalEvents.BalancyInitializedEvent -= OnBalancyInitialized;
        GameProgress.LoadProfile();
    }
}
