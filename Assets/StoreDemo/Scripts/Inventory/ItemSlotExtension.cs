using Balancy.Models;

namespace Balancy.Data
{
    public static class ItemSlotExtension
    {
        public static ItemSlot Create(SlotType type = SlotType.Default)
        {
            var itemSlot = ItemSlot.Instantiate();
            itemSlot.Type = type;
            return itemSlot;
        }
        
        public static ItemSlot Create(ItemWithAmount itemWithAmount, SlotType type = SlotType.Default)
        {
            var itemSlot = ItemSlot.Instantiate();
            itemSlot.Type = type;
            itemSlot.ItemInSlot.PutNewItemInSlot(itemWithAmount.Item, itemWithAmount.Count);
            return itemSlot;
        }

        public static InventoryError MoveItemToAnotherSlot(this ItemSlot slotFrom, ItemSlot slotTo)
        {
            var result = slotFrom.CanMoveItemToAnotherSlot(slotTo);
            if (result == InventoryError.None)
            {
                slotFrom.ItemInSlot.MoveItemToAnotherSlot(slotTo.ItemInSlot);
            }

            return result;
        }

        public static InventoryError CanMoveItemToAnotherSlot(this ItemSlot slotFrom, ItemSlot slotTo)
        {
            if (!slotFrom.AnyItem())
                return InventoryError.NoItem;

            if (slotFrom.Type != slotTo.Type)
            {
                if (!slotTo.CanAcceptItem(slotFrom.ItemInSlot.Item))
                    return InventoryError.CantPutItemIntoThisSlot;

                if (slotTo.AnyItem() && !slotFrom.CanAcceptItem(slotTo.ItemInSlot.Item))
                    return InventoryError.CantPutItemIntoThisSlot;
            }

            return InventoryError.None;
        }

        public static bool CanMergeItem(this ItemSlot slotFrom, ItemSlot slotTo)
        {
            if (slotFrom?.ItemInSlot?.Item == null || slotTo?.ItemInSlot?.Item == null) return false;

            var result = slotFrom.CanMoveItemToAnotherSlot(slotTo);
            if (result != InventoryError.None) return false;

            return slotFrom.ItemInSlot.CanMergeSlots(slotTo.ItemInSlot.Item);
        }

        public static bool AnyItem(this ItemSlot slot)
        {
            if (slot?.ItemInSlot == null) return false;
            return slot.ItemInSlot.AnyItem();
        }

        public static bool IsEmpty(this ItemSlot slot) => !slot.AnyItem();

        public static bool IsFull(this ItemSlot slot)
        {
            return slot.ItemInSlot.Count >= slot.ItemInSlot.Item.MaxStack;
        }

        public static int IncreaseAmountOfItemsInSlot(this ItemSlot slot, int increaseAmount)
        {
            return slot.ItemInSlot.IncreaseAmountOfItemsInSlot(increaseAmount);
        }

        public static int GetAvailableSlotsForItem(this ItemSlot slot)
        {
            return slot.ItemInSlot.GetAvailableSlotsForItem();
        }

        public static bool CanAcceptItem(this ItemSlot slot, ItemModel item)
        {
            if (item == null) return false;

            return ((int) slot.Type & (int)item.SlotMask) != 0;
        }
    }
}