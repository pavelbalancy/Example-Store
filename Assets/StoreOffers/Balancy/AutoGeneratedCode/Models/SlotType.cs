using System;

namespace Balancy.Models
{
	[Flags]
	public enum SlotType
	{
		Default = 1,
		Gems = 2,
		Gold = 4,
	}
}