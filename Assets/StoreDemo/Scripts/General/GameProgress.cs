using Balancy;
using Balancy.Data;
using UnityEngine;

public static class GameProgress
{
    private const string PROFILE_KEY = "ProfileKey";

    private static Profile _currentProgress;
    
    public static void LoadProfile()
    {
        Storage.LoadSmartObject<Profile>(PROFILE_KEY, responseData =>
        {
            Debug.Log("LoadProfile response = " + responseData.Success);
            if (responseData.Success)
            {
                var profile = _currentProgress = responseData.Data;
                profile.ValidateAndFix();
                Debug.Log("Gems Count => " + profile.GetGems());
                Debug.Log("Gold Count => " + profile.GetGold());
                GlobalEvents.InvokeProfileInitialized(profile);
            } else
                Debug.LogError("Error: " + responseData.Error.Message);
        });
    }

    public static Profile GetCurrentProgress()
    {
        return _currentProgress;
    }

    public static void IncreaseLevel()
    {
        _currentProgress.Level++;
        GlobalEvents.InvokePlayerLevelChanged(_currentProgress);
    }

    public static void DecreaseLevel()
    {
        if (_currentProgress.Level <= 1)
            return;
        
        _currentProgress.Level--;
        GlobalEvents.InvokePlayerLevelChanged(_currentProgress);
    }

    public static void ResetProgress()
    {
        _currentProgress.Level = 1;
        _currentProgress.Purchases.Clear();
        _currentProgress.Resources.ItemSlots.Clear();
        _currentProgress.Resources.Config = null;
        _currentProgress.ValidateAndFix();
        GlobalEvents.InvokeProfileInitialized(_currentProgress);
    }
}