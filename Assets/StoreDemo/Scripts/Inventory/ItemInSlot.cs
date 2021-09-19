using UnityEngine;
using Newtonsoft.Json;
using Balancy.Models;
using System.Runtime.Serialization;

namespace Balancy.Data
{
#pragma warning disable 649

	public class ItemInSlot : BaseData
	{

		[JsonProperty("count")]
		private int count;
		[JsonProperty("unnyIdItem")]
		private string unnyIdItem;
		private ItemModel item;


		[JsonIgnore]
		public int Count
		{
			get { return count; }
			set { if (count == value) return; count = value; SetDirty(); }
		}

		[JsonIgnore]
		public ItemModel Item
		{
			get
			{
				if (item == null)
					item = DataEditor.GetModelByUnnyId<ItemModel>(unnyIdItem);
				return item;
			}
			set
			{
				if (item == value)
					return;
				item = value;
				unnyIdItem = item == null ? "0" : item.UnnyId;
				SetDirty();
			}
		
		}

		[OnDeserialized]
		internal new void OnDeserializedMethod(StreamingContext context) {
		}

		public static ItemInSlot Instantiate()
		{
			ItemInSlot result = new ItemInSlot();
			result.OnDeserializedMethod(new StreamingContext());
			return result;
		}
	}
#pragma warning restore 649
}