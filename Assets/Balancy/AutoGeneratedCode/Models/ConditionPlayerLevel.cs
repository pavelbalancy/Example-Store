using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ConditionPlayerLevel : ConditionBase
	{



		[JsonProperty("level")]
		public readonly int Level;

	}
#pragma warning restore 649
}