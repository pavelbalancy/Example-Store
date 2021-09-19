using System.Runtime.Serialization;
using Balancy;
using Balancy.Data;
using Balancy.Models;
using Newtonsoft.Json;

public class PurchaseInfo : BaseData
{
    [JsonProperty("unnyIdOffer")]
    private string unnyIdOffer;
    private StoreOffer offer;
    [JsonProperty("time")]
    private int time;
    
    [JsonIgnore]
    public StoreOffer Offer
    {
        get
        {
            if (offer == null)
                offer = DataEditor.GetModelByUnnyId<StoreOffer>(unnyIdOffer);
            return offer;
        }
        set
        {
            if (offer == value)
                return;
            offer = value;
            unnyIdOffer = offer == null ? "0" : offer.UnnyId;
            SetDirty();
        }
    }
    
    [JsonIgnore]
    public int Time
    {
        get { return time; }
        set { if (time == value) return; time = value; SetDirty(); }
    }
    
    [OnDeserialized]
    internal new void OnDeserializedMethod(StreamingContext context) {

    }

    public static PurchaseInfo Instantiate()
    {
        PurchaseInfo result = new PurchaseInfo();
        result.OnDeserializedMethod(new StreamingContext());
        return result;
    }
}
