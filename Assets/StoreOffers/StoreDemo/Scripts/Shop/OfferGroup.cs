using Balancy.Models;
using UnityEngine;

public class OfferGroup : MonoBehaviour
{
    [SerializeField]
    private OfferView[] offers;

    private int nextViewIndex = 0; 

    public void AddOffer(StoreOffer offer)
    {
        if (nextViewIndex >= offers.Length)
        {
            Debug.LogError("Too many offers in one group, skipping " + offer.Name);
        }
        else
        {
            offers[nextViewIndex++].SetOffer(offer);
        }
    }
}
