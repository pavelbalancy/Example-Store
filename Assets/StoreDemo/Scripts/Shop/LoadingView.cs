using Balancy.Data;
using UnityEngine;

public class LoadingView : MonoBehaviour
{
    private void Awake()
    {
        GlobalEvents.ProfileInitializedEvent += OnProfileInitialized;
    }

    private void OnProfileInitialized(Profile profile)
    {
        GlobalEvents.ProfileInitializedEvent -= OnProfileInitialized;
        gameObject.SetActive(false);
    }
}
