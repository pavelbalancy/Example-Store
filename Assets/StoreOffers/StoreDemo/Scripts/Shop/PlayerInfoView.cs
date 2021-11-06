using Balancy.Data;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoView : MonoBehaviour
{
    [SerializeField]
    private Text _text;
    
    [SerializeField]
    private MyButton _incLevel;
    
    [SerializeField]
    private MyButton _decLevel;
    
    private void Awake()
    {
        GlobalEvents.ProfileInitializedEvent += OnProfileInitialized;
        GlobalEvents.PlayerLevelChangedEvent += OnPlayerLevelChanged;
        
        _incLevel.onClick.AddListener(IncLevelClicked);
        _decLevel.onClick.AddListener(DecLevelClicked);
    }

    private void IncLevelClicked()
    {
        GameProgress.IncreaseLevel();
    }

    private void DecLevelClicked()
    {
        GameProgress.DecreaseLevel();
    }

    private void OnDestroy()
    {
        GlobalEvents.ProfileInitializedEvent -= OnProfileInitialized;
        GlobalEvents.PlayerLevelChangedEvent -= OnPlayerLevelChanged;
    }

    private void OnProfileInitialized(Profile profile)
    {
        Refresh(profile);
    }

    private void OnPlayerLevelChanged(Profile profile)
    {
        Refresh(profile);
    }

    private void Refresh(Profile profile)
    {
        _text.text = profile.Statistics.Level.ToString();
    }
}
