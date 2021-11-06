using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Balancy.Data
{
#pragma warning disable 649

	public class Profile : ParentBaseData
	{
		[JsonProperty("resources")]
		private Inventory resources;
		[JsonProperty("statistics")]
		private Statistics statistics;
		
		[JsonIgnore]
		public Inventory Resources
		{
			get { return resources; }
			set { if (resources == value) return; resources = value; SetDirty(); }
		}
		
		[JsonIgnore]
		public Statistics Statistics
		{
			get { return statistics; }
			set { if (statistics == value) return; statistics = value; SetDirty(); }
		}
		
		[OnDeserialized]
		internal new void OnDeserializedMethod(StreamingContext context) {
			if (resources == null) {
				resources = new Inventory();
				resources.OnDeserializedMethod(context);
			}
			resources.SubscribeForChanges(SetDirty);
			
			if (statistics == null) {
				statistics = new Statistics();
				statistics.OnDeserializedMethod(context);
			}
			statistics.SubscribeForChanges(SetDirty);
		}

		public static Profile Instantiate()
		{
			Profile result = new Profile();
			result.OnDeserializedMethod(new StreamingContext());
			return result;
		}
	}
#pragma warning restore 649
}