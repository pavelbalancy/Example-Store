using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ItemModel : BaseModel
	{



		[JsonProperty("name")]
		public readonly string Name;

		[JsonProperty("description")]
		public readonly string Description;

		[JsonProperty("maxStack")]
		public readonly int MaxStack;

		[JsonProperty("icon")]
		public readonly UnnyAsset Icon;

		[JsonProperty("slotMask")]
		public readonly SlotType SlotMask;

	}
#pragma warning restore 649
}