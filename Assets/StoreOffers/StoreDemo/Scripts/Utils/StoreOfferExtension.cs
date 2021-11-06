using Balancy;
using Balancy.Data;
using Balancy.Models;

public static class StoreOfferExtension
{
    public static bool IsInApp(this StoreOffer storeOffer)
    {
        return !string.IsNullOrEmpty(storeOffer.InAppId);
    }

    public static void Purchase(this StoreOffer storeOffer)
    {
        if (storeOffer.CanPurchase())
        {
            storeOffer.TakeResources();
            storeOffer.GiveItems();
            storeOffer.SavePurchase();
            GlobalEvents.InvokeOfferPurchased(storeOffer);
        }
    }

    private static void SavePurchase(this StoreOffer storeOffer)
    {
        var profile = GameProgress.GetCurrentProgress();
        var info = PurchaseInfo.Instantiate();
        info.Offer = storeOffer;
        info.Time = (int) UnnyTime.GetServerTime();
        profile.Statistics.Purchases.Add(info);
    }

    private static void GiveItems(this StoreOffer storeOffer)
    {
        var profile = GameProgress.GetCurrentProgress();
        foreach (var itemWithAmount in storeOffer.Items)
        {
            var count = itemWithAmount.Count;
            profile.Resources.AddItem(itemWithAmount.Item, ref count);
        }
    }

    private static void TakeResources(this StoreOffer storeOffer)
    {
        if (!storeOffer.IsInApp())
        {
            var profile = GameProgress.GetCurrentProgress();

            var amount = storeOffer.Price.Count;
            profile.Resources.TakeItems(storeOffer.Price.Item, ref amount);
        }
    }

    public static bool CanPurchase(this StoreOffer storeOffer)
    {
        if (storeOffer.IsInApp())
            return true;
        
        var profile = GameProgress.GetCurrentProgress();
        var haveItems = profile.Resources.GetItemsCount(storeOffer.Price.Item);
        return haveItems >= storeOffer.Price.Count;
    }
}
