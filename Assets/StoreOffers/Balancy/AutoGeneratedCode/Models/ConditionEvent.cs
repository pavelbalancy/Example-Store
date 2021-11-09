using UnityEngine;
using Newtonsoft.Json;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ConditionEvent : ConditionBase
	{

		[JsonProperty]
		private string unnyIdGameEvent;
		private GameEvent gameEvent;


		[JsonIgnore]
		public GameEvent GameEvent
		{
			get
			{
				if (gameEvent == null)
					gameEvent = DataEditor.GetModelByUnnyId<GameEvent>(unnyIdGameEvent);
				return gameEvent;
			}
		}

	}
#pragma warning restore 649
}