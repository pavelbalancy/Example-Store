using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ConditionPurchasesCount : ConditionBase
	{



		[JsonProperty("minPurchases")]
		public readonly int MinPurchases;

	}
#pragma warning restore 649
}