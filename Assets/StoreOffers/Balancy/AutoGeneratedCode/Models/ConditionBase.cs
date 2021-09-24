using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ConditionBase : BaseModel
	{



		[JsonProperty("inverse")]
		public readonly bool Inverse;

	}
#pragma warning restore 649
}