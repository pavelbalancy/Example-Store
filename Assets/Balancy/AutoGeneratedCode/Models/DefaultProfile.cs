using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class DefaultProfile : BaseModel
	{

		[JsonProperty]
		private string unnyIdResources;
		private InventoryConfig resources;
		[JsonProperty]
		private string unnyIdInventory;
		private InventoryConfig inventory;


		[JsonIgnore]
		public InventoryConfig Resources
		{
			get
			{
				if (resources == null)
					resources = DataEditor.GetModelByUnnyId<InventoryConfig>(unnyIdResources);
				return resources;
			}
		}

		[JsonIgnore]
		public InventoryConfig Inventory
		{
			get
			{
				if (inventory == null)
					inventory = DataEditor.GetModelByUnnyId<InventoryConfig>(unnyIdInventory);
				return inventory;
			}
		}

		[JsonProperty("startResources")]
		public readonly ItemWithAmount[] StartResources;

	}
#pragma warning restore 649
}