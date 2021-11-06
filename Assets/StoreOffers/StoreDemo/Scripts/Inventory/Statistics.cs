using UnityEngine;
using Newtonsoft.Json;
using Balancy.Models;
using System.Runtime.Serialization;

namespace Balancy.Data
{
#pragma warning disable 649

	public class Statistics : BaseData
	{
		[JsonProperty("purchases"), JsonConverter(typeof(SmartListConverter<PurchaseInfo>))]
		private SmartList<PurchaseInfo> purchases;
		[JsonProperty("level")]
		private int level;
		
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
						
			if (purchases == null)
				purchases = new SmartList<PurchaseInfo>();
			purchases.SubscribeForChanges(SetDirty);

		}

		public static Statistics Instantiate()
		{
			Statistics result = new Statistics();
			result.OnDeserializedMethod(new StreamingContext());
			return result;
		}
	}
#pragma warning restore 649
}