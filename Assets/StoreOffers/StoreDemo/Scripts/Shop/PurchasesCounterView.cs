using Balancy.Data;
using Balancy.Models;
using UnityEngine;
using UnityEngine.UI;

public class PurchasesCounterView : MonoBehaviour
{
    [SerializeField]
    private Text _text;
    
    private void Awake()
    {
        GlobalEvents.ProfileInitializedEvent += OnProfileInitialized;
        GlobalEvents.OfferPurchasedEvent += OnOfferPurchased;
    }
    
    private void OnDestroy()
    {
        GlobalEvents.ProfileInitializedEvent -= OnProfileInitialized;
        GlobalEvents.OfferPurchasedEvent -= OnOfferPurchased;
    }

    private void OnProfileInitialized(Profile profile)
    {
        Refresh(profile);
    }

    private void OnOfferPurchased(StoreOffer storeOffer)
    {
        Refresh(GameProgress.GetCurrentProgress());
    }

    private void Refresh(Profile profile)
    {
        _text.text = profile.Statistics.Purchases.Count.ToString();
    }
}
