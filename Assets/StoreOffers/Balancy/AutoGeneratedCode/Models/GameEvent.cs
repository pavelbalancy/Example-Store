using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class GameEvent : BaseModel
	{

		[JsonProperty]
		private string unnyIdCondition;
		private ConditionLogic condition;


		[JsonIgnore]
		public ConditionLogic Condition
		{
			get
			{
				if (condition == null)
					condition = DataEditor.GetModelByUnnyId<ConditionLogic>(unnyIdCondition);
				return condition;
			}
		}

		[JsonProperty("name")]
		public readonly string Name;

	}
#pragma warning restore 649
}