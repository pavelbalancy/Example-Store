using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Balancy.Data
{
#pragma warning disable 649

	public class Profile : BaseData
	{
		[JsonProperty("resources")]
		private Inventory resources;
		[JsonProperty("itemSlots"), JsonConverter(typeof(SmartListConverter<PurchaseInfo>))]
		private SmartList<PurchaseInfo> purchases;
		[JsonProperty("level")]
		private int level;
		
		[JsonIgnore]
		public Inventory Resources
		{
			get { return resources; }
			set { if (resources == value) return; resources = value; SetDirty(); }
		}
		
		[JsonIgnore]
		public SmartList<PurchaseInfo> Purchases { get { return purchases;} }
		
		[JsonIgnore]
		public int Level
		{
			get { return level; }
			set { if (level == value) return; level = value; SetDirty(); }
		}
		
		[OnDeserialized]
		internal new void OnDeserializedMethod(StreamingContext context) {
			if (resources == null) {
				resources = new Inventory();
				resources.OnDeserializedMethod(context);
			}
			resources.SubscribeForChanges(SetDirty);
			
			if (purchases == null)
				purchases = new SmartList<PurchaseInfo>();
			purchases.SubscribeForChanges(SetDirty);
		}

		public static Profile Instantiate()
		{
			Profile result = new Profile();
			result.OnDeserializedMethod(new StreamingContext());
			return result;
		}
	}
#pragma warning restore 649
}