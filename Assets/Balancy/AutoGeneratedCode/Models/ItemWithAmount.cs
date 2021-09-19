using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ItemWithAmount : BaseModel
	{

		[JsonProperty]
		private string unnyIdItem;
		private ItemModel item;


		[JsonIgnore]
		public ItemModel Item
		{
			get
			{
				if (item == null)
					item = DataEditor.GetModelByUnnyId<ItemModel>(unnyIdItem);
				return item;
			}
		}

		[JsonProperty("count")]
		public readonly int Count;

	}
#pragma warning restore 649
}