using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ConditionGemsCount : ConditionBase
	{



		[JsonProperty("gems")]
		public readonly int Gems;

	}
#pragma warning restore 649
}