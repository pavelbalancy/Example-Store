using Balancy;
using Balancy.Addressables;
using Balancy.Data;
using Balancy.Models;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MyButton))]
public class OfferView : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    
    [SerializeField]
    private Image productIcon;
    [SerializeField]
    private Text productCount;
    
    [SerializeField]
    private Image priceIcon;
    [SerializeField]
    private Text priceCount;

    private MyButton _myButton;
    private StoreOffer _offer;

    private void Awake()
    {
        _myButton = GetComponent<MyButton>();
        _myButton.onClick.AddListener(OnClicked);

        GlobalEvents.ItemInSlotUpdatedEvent += OnItemInSlotUpdated;
    }

    private void OnDestroy()
    {
        GlobalEvents.ItemInSlotUpdatedEvent -= OnItemInSlotUpdated;
    }

    private void OnItemInSlotUpdated(ItemInSlot itemInSlot)
    {
        CheckAvailability();
    }

    private void OnClicked()
    {
        _offer.Purchase();
    }

    public void SetOffer(StoreOffer offer)
    {
        _offer = offer;
        SetMainIcon(offer);
        SetPrice(offer);
        SetValue(offer);
        CheckAvailability();
    }

    private void SetMainIcon(StoreOffer offer)
    {
        SetSprite(icon, offer.Icon, true);
    }

    private void SetPrice(StoreOffer offer)
    {
        if (offer.IsInApp())
        {
            priceIcon.gameObject.SetActive(false);
            priceCount.text = "$ 99.99";
        }
        else
        {
            var priceItem = offer.Price?.Item;
            if (priceItem != null)
            {
                SetSprite(priceIcon, priceItem.Icon);
                priceCount.text = offer.Price.Count.ToString();
            }
            else
            {
                Debug.LogError("Wrong Price in Offer " + offer.Name);
                priceIcon.gameObject.SetActive(false);
                priceCount.gameObject.SetActive(false);
            }
        }
    }

    private void SetValue(StoreOffer offer)
    {
        if (offer.IsInApp())
        {
            if (offer.Items.Length > 0)
            {
                var valueItem = offer.Items[0]?.Item;
                if (valueItem != null)
                {
                    SetSprite(productIcon, valueItem.Icon);
                    productCount.text = offer.Items[0].Count.ToString();
                    return;
                }
            }
            Debug.LogError("Wrong Value in Offer " + offer.Name);
            priceIcon.gameObject.SetActive(false);
            priceCount.gameObject.SetActive(false);
        }
        else
        {
            productIcon.gameObject.SetActive(false);
            productCount.text = offer.Name;
        }
    }

    private void SetSprite(Image image, UnnyAsset asset, bool useNativeSize = false)
    {
        AssetsLoader.GetSprite(asset, sprite =>
        {
            image.sprite = sprite;
            if (useNativeSize)
                image.SetNativeSize();
        });
    }

    private void CheckAvailability()
    {
        _myButton.interactable = _offer.CanPurchase();
    }
}
