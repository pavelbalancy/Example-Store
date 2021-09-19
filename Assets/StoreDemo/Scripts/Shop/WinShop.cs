using System.Collections.Generic;
using Balancy;
using Balancy.Data;
using Balancy.Models;
using UnityEngine;

public class WinShop : MonoBehaviour
{
    private const int OFFERS_PER_PAGE = 5;
    private const int GROUPS_PER_PAGE = 3;
    
    [SerializeField]
    private GameObject prefabSmallSoft;
    [SerializeField]
    private GameObject prefabBigSoft;
    [SerializeField]
    private GameObject prefabEmpty;
    
    [SerializeField]
    private Transform content;

    private List<StoreOffer> _lastCreatedOffers;
    private ConditionChecker _conditionChecker;

    private void Awake()
    {
        _conditionChecker = ConditionChecker.Instance;
        _conditionChecker.SubscribeToConditionChange(Refresh);
        GlobalEvents.ProfileInitializedEvent += OnProfileInitialized;
    }

    private void OnDestroy()
    {
        _conditionChecker.UnsubscribeFromConditionChange(Refresh);
        GlobalEvents.ProfileInitializedEvent -= OnProfileInitialized;
    }

    private void OnProfileInitialized(Profile profile)
    {
        Refresh();
    }

    private void Refresh()
    {
        var availableOffers = GetAvailableOffers();
        
        if (DidOffersChanged(availableOffers))
            CreateViews(availableOffers);
    }

    private List<StoreOffer> GetAvailableOffers()
    {
        var list = new List<StoreOffer>();
        foreach (var offer in DataEditor.StoreOffers)
        {
            if (_conditionChecker.AreAllConditionsComplete(offer.Conditions))
                list.Add(offer);
        }

        list.Sort((p1, p2) => p1.Order.CompareTo(p2.Order));
        return list;
    }

    private void CreateViews(List<StoreOffer> offers)
    {
        _lastCreatedOffers = offers;
        content.RemoveChildren();
        
        OfferGroup _group = null;
        int lastGroup = -1;
        int lastPage = 0;
        
        for (int i = 0; i < offers.Count; i++)
        {
            var offer = offers[i];
            var order = offer.Order;
            var pageNum = (order - 1) / OFFERS_PER_PAGE;
            var group = GetOfferGroupNum(order, out var isBig);

            if (lastPage != pageNum)
            {
                lastPage = pageNum;
                CreateEmptyObject();
            }
            
            if (lastGroup != group)
            {
                lastGroup = group;
                _group = CreateNewGroup(isBig);
            }

            _group.AddOffer(offer);
        }
    }
    
    private void CreateEmptyObject()
    {
        Instantiate(prefabEmpty, content);
    }

    private OfferGroup CreateNewGroup(bool isBig)
    {
        var prefab = GetPrefab(isBig);
        var go = Instantiate(prefab, content);
        return go.GetComponent<OfferGroup>();
    }

    private GameObject GetPrefab(bool isBig)
    {
        return isBig ? prefabBigSoft : prefabSmallSoft;
    }

    private int GetOfferGroupNum(int order, out bool isBig)
    {
        var pageNum = (order - 1) / OFFERS_PER_PAGE;
        var orderInGroup = order - pageNum * OFFERS_PER_PAGE;
        int groupNum = 0;
        isBig = false;
        switch (orderInGroup)
        {
            case 1:
            case 2:
                groupNum = 0;
                break;
            case 3:
                groupNum = 1;
                isBig = true;
                break;
            case 4:
            case 5:
                groupNum = 2;
                break;
        }

        return groupNum + pageNum * GROUPS_PER_PAGE;
    }

    private bool DidOffersChanged(List<StoreOffer> newOffers)
    {
        if (_lastCreatedOffers == null)
            return true;

        if (_lastCreatedOffers.Count != newOffers.Count)
            return true;

        for (int i = 0; i < newOffers.Count; i++)
        {
            if (_lastCreatedOffers[i] != newOffers[i])
                return true;
        }

        return false;
    }
}
