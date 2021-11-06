using System;
using Balancy.Data;
using Balancy.Models;
using UnityEngine;

public class ConditionChecker
{
    private static ConditionChecker _instance;
    public static ConditionChecker Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ConditionChecker();
            return _instance;
        }
    }
    
    private event Action _conditionChangedCallback;
    private Coroutine _coroutine;

    public ConditionChecker()
    {
        _instance = this;
        SubscribeForChanges();
    }
    
    public bool AreAllConditionsComplete(ConditionBase[] conditions)
    {
        foreach (var condition in conditions)
        {
            if (!IsConditionComplete(condition))
                return false;
        }

        return true;
    }

    private bool IsConditionComplete(ConditionBase condition)
    {
        var profile = GameProgress.GetCurrentProgress();;
        switch (condition)
        {
            case ConditionPlayerLevel _conditionPlayerLevel:
            {
                if ((profile.Statistics.Level >= _conditionPlayerLevel.Level) == condition.Inverse)
                    return false;
                break;
            }
            case ConditionPurchasesCount _conditionPurchasesCount:
            {
                if ((profile.Statistics.Purchases.Count >= _conditionPurchasesCount.MinPurchases) == condition.Inverse)
                    return false;
                break;
            }
        }
        
        return true;
    }

    private void SubscribeForChanges()
    {
        GlobalEvents.OfferPurchasedEvent += OnOfferPurchased;
        GlobalEvents.PlayerLevelChangedEvent += OnPlayerLevelChanged;
    }

    private void OnOfferPurchased(StoreOffer storeOffer)
    {
        ConditionChanged();
    }

    private void OnPlayerLevelChanged(Profile profile)
    {
        ConditionChanged();
    }
    
    private void ConditionChanged()
    {
        if (_coroutine != null)
            return;

        _coroutine = Coroutines.WaitOneFrame(() =>
        {
            _coroutine = null;
            _conditionChangedCallback?.Invoke();
        });
    }
    
    public void SubscribeToConditionChange(Action callback)
    {
        _conditionChangedCallback += callback;
    }

    public void UnsubscribeFromConditionChange(Action callback)
    {
        _conditionChangedCallback -= callback;
    }
}
