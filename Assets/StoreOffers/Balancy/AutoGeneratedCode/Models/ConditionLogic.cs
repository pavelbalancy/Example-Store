using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ConditionLogic : ConditionBase
	{

		[JsonProperty]
		private string[] unnyIdConditions;
		private ConditionBase[] conditions;


		[JsonIgnore]
		public ConditionBase[] Conditions
		{
			get
			{
				if (conditions == null) {
					if (unnyIdConditions == null)
						return conditions = new ConditionBase[0];
					conditions = new ConditionBase[unnyIdConditions.Length];
					for (int i = 0;i < unnyIdConditions.Length;i++)
						conditions[i] = DataEditor.GetModelByUnnyId<ConditionBase>(unnyIdConditions[i]);
				}
				return conditions;
			}
		}

	}
#pragma warning restore 649
}