using System;
using Balancy;
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
    
    public bool IsConditionsComplete(ConditionLogic condition)
    {
        return IsConditionComplete(condition);
    }

    private bool IsConditionComplete(ConditionBase condition)
    {
        var profile = GameProgress.GetCurrentProgress();;
        switch (condition)
        {
            case ConditionEvent _conditionEvent:
            {
                //TODO you need to implement Events logic using the data from DataEditor.GameEvents and check running events here
                return false;
                break;
            }
            case ConditionDateRange _conditionDateRange:
            {
                var dateNow = DateTime.Now;
                var startDate = DateTime.Parse(_conditionDateRange.StartDate);
                var finishDate = DateTime.Parse(_conditionDateRange.FinishDate);
                if (dateNow < startDate || dateNow > finishDate)
                    return false;
                break;
            }
            case ConditionAnd _conditionAnd:
            {
                foreach (var cond in _conditionAnd.Conditions)
                {
                    if (!IsConditionComplete(cond))
                        return false;
                }
                break;
            }
            case ConditionOr _conditionOr:
            {
                var anyConditionComplete = false;
                foreach (var cond in _conditionOr.Conditions)
                {
                    if (IsConditionComplete(cond))
                    {
                        anyConditionComplete = true;
                        break;
                    }
                }

                if (!anyConditionComplete)
                    return false;
                break;
            }
            case ConditionPlayerLevel _conditionPlayerLevel:
            {
                if (profile.Statistics.Level < _conditionPlayerLevel.Level)
                    return false;
                break;
            }
            case ConditionPurchasesCount _conditionPurchasesCount:
            {
                if (profile.Statistics.Purchases.Count < _conditionPurchasesCount.MinPurchases)
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
