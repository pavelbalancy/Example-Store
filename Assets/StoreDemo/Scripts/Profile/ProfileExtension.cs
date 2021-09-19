using Balancy.Models;

namespace Balancy.Data
{
    public static class ProfileExtension
    {
        public static int GetGems(this Profile profile) => profile.Resources.GetItemsCount(SlotType.Gems);
        public static int GetGold(this Profile profile) => profile.Resources.GetItemsCount(SlotType.Gold);

        public static Profile Create()
        {
            var newProfile = Profile.Instantiate();
            newProfile.ValidateAndFix();
            return newProfile;
        }
        
        public static void ValidateAndFix(this Profile profile)
        {
            profile.CheckResources();
        }

        private static void CheckResources(this Profile profile)
        {
            var resources = profile.Resources;
            if (resources.Config == null)
            {
                var defaultProfile = DataEditor.DefaultProfile;
                resources.Config = defaultProfile.Resources;
                
                resources.ItemSlots.Clear();
                foreach (var startResource in defaultProfile.StartResources)
                    resources.ItemSlots.Add(ItemSlotExtension.Create(startResource, startResource.Item.SlotMask));
            }
            
            resources.ValidateAndFix();
        }
    }
}
