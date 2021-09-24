using UnityEngine;
using Newtonsoft.Json;
using Balancy.Models;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Balancy.Data
{
#pragma warning disable 649

	public class Inventory : BaseData
	{

		[JsonProperty("itemSlots"), JsonConverter(typeof(SmartListConverter<ItemSlot>))]
		private SmartList<ItemSlot> itemSlots;
		
		[JsonProperty("unnyIdConfig")]
		private string unnyIdConfig;
		private InventoryConfig config;


		[JsonIgnore]
		public InventoryConfig Config
		{
			get
			{
				if (config == null)
					config = DataEditor.GetModelByUnnyId<InventoryConfig>(unnyIdConfig);
				return config;
			}
			set
			{
				if (config == value)
					return;
				config = value;
				unnyIdConfig = config == null ? "0" : config.UnnyId;
				SetDirty();
			}
		}


		[JsonIgnore]
		public SmartList<ItemSlot> ItemSlots { get { return itemSlots;} }

		[OnDeserialized]
		internal new void OnDeserializedMethod(StreamingContext context) {
			if (itemSlots == null)
				itemSlots = new SmartList<ItemSlot>();
			itemSlots.SubscribeForChanges(SetDirty);
		}

		public static Inventory Instantiate()
		{
			Inventory result = new Inventory();
			result.OnDeserializedMethod(new StreamingContext());
			return result;
		}
	}
#pragma warning restore 649
}