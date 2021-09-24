using Balancy.Data;
using Balancy.Models;

public static class GlobalEvents
{
    public delegate void BalancyInitializedDelegate();
    public delegate void ProfileInitializedDelegate(Profile profile);
    public delegate void ItemInSlotUpdatedDelegate(ItemInSlot itemInSlot);
    public delegate void OfferPurchasedDelegate(StoreOffer storeOffer);
    public delegate void PlayerLevelChangedDelegate(Profile profile);

    public static event BalancyInitializedDelegate BalancyInitializedEvent;
    public static event ProfileInitializedDelegate ProfileInitializedEvent;
    public static event ItemInSlotUpdatedDelegate ItemInSlotUpdatedEvent;
    public static event OfferPurchasedDelegate OfferPurchasedEvent;
    public static event PlayerLevelChangedDelegate PlayerLevelChangedEvent;

    public static void InvokeBalancyInitialized() => BalancyInitializedEvent?.Invoke();
    public static void InvokeItemInSlotUpdated(ItemInSlot itemInSlot) => ItemInSlotUpdatedEvent?.Invoke(itemInSlot);
    public static void InvokeProfileInitialized(Profile profile) => ProfileInitializedEvent?.Invoke(profile);
    public static void InvokeOfferPurchased(StoreOffer storeOffer) => OfferPurchasedEvent?.Invoke(storeOffer);
    public static void InvokePlayerLevelChanged(Profile profile) => PlayerLevelChangedEvent?.Invoke(profile);
}
