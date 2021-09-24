using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class InventoryConfig : BaseModel
	{



		[JsonProperty("displayName")]
		public readonly string DisplayName;

		[JsonProperty("autoResize")]
		public readonly bool AutoResize;

		[JsonProperty("minSlot")]
		public readonly int MinSlot;

		[JsonProperty("maxSlots")]
		public readonly int MaxSlots;

		[JsonProperty("maxItems")]
		public readonly int MaxItems;

	}
#pragma warning restore 649
}