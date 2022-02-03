using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ConditionDateRange : ConditionBase
	{



		[JsonProperty("startDate")]
		public readonly string StartDate;

		[JsonProperty("finishDate")]
		public readonly string FinishDate;

	}
#pragma warning restore 649
}