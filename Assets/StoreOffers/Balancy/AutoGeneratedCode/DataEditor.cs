using Balancy.Models;
using System.Collections.Generic;

namespace Balancy
{
#pragma warning disable 649

	public partial class DataEditor
	{


		public static List<ItemModel> ItemModels { get; private set; }
		public static List<InventoryConfig> InventoryConfigs { get; private set; }
		public static List<StoreOffer> StoreOffers { get; private set; }
		public static DefaultProfile DefaultProfile { get; private set; }

		static partial void PrepareGeneratedData() {
			var itemModelWrapper = ParseDictionary<ItemModel>();
			if (itemModelWrapper == null || itemModelWrapper.List == null)
				ItemModels = new List<ItemModel>(0);
			else {
				ItemModels = new List<ItemModel>(itemModelWrapper.List.Length);
				foreach (var child in itemModelWrapper.List)
					ItemModels.Add(child);
			}

			ParseDictionary<ConditionPurchasesCount>();

			var inventoryConfigWrapper = ParseDictionary<InventoryConfig>();
			if (inventoryConfigWrapper == null || inventoryConfigWrapper.List == null)
				InventoryConfigs = new List<InventoryConfig>(0);
			else {
				InventoryConfigs = new List<InventoryConfig>(inventoryConfigWrapper.List.Length);
				foreach (var child in inventoryConfigWrapper.List)
					InventoryConfigs.Add(child);
			}

			var storeOfferWrapper = ParseDictionary<StoreOffer>();
			if (storeOfferWrapper == null || storeOfferWrapper.List == null)
				StoreOffers = new List<StoreOffer>(0);
			else {
				StoreOffers = new List<StoreOffer>(storeOfferWrapper.List.Length);
				foreach (var child in storeOfferWrapper.List)
					StoreOffers.Add(child);
			}


			var defaultProfileWrapper = ParseDictionary<DefaultProfile>();
			if (defaultProfileWrapper != null && defaultProfileWrapper.List != null && defaultProfileWrapper.List.Length > 0 && defaultProfileWrapper.Config != null)
			{
				for (int i = 0; i < defaultProfileWrapper.List.Length; i++)
				{
					if (defaultProfileWrapper.List[i].UnnyId == defaultProfileWrapper.Config.Selected)
					{
						DefaultProfile = defaultProfileWrapper.List[i];
						break;
					}
				}
			}
			else
				DefaultProfile = null;

			ParseDictionary<ConditionBase>();

			ParseDictionary<ConditionPlayerLevel>();

		}
	}
#pragma warning restore 649
}