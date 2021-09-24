using UnityEngine;
using Newtonsoft.Json;
using Balancy.Models;
using System.Runtime.Serialization;

namespace Balancy.Data
{
#pragma warning disable 649

	public class ItemSlot : BaseData
	{

		[JsonProperty("type")]
		private SlotType type;
		[JsonProperty("itemInSlot")]
		private ItemInSlot itemInSlot;


		[JsonIgnore]
		public SlotType Type
		{
			get { return type; }
			set { if (type == value) return; type = value; SetDirty(); }
		}

		[JsonIgnore]
		public ItemInSlot ItemInSlot
		{
			get { return itemInSlot; }
			set { if (itemInSlot == value) return; itemInSlot = value; SetDirty(); }
		}

		[OnDeserialized]
		internal new void OnDeserializedMethod(StreamingContext context) {
			if (itemInSlot == null) {
				itemInSlot = new ItemInSlot();
				itemInSlot.OnDeserializedMethod(context);
			}
			itemInSlot.SubscribeForChanges(SetDirty);
		}

		public static ItemSlot Instantiate()
		{
			ItemSlot result = new ItemSlot();
			result.OnDeserializedMethod(new StreamingContext());
			return result;
		}
	}
#pragma warning restore 649
}