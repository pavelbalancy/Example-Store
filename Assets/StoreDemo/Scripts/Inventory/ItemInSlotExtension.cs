using System;
using Balancy.Models;

namespace Balancy.Data
{
    public static class ItemInSlotExtension
    {
        public static void MoveItemToAnotherSlot(this ItemInSlot slotFrom, ItemInSlot slotTo)
        {
            if (!slotFrom.AnyItem())
                return;

            if (!slotTo.AnyItem())
            {
                slotTo.PutNewItemInSlot(slotFrom.Item, slotFrom.Count);
                slotFrom.RemoveItemFromSlot();
            }
            else
            {
                if (CanMergeSlots(slotFrom.Item, slotTo.Item))
                    MergeItems(slotFrom, slotTo);
                else
                    ExchangeItemsInSlots(slotFrom, slotTo);
            }
        }

        public static void PutNewItemInSlot(this ItemInSlot slot, ItemModel item, int amount)
        {
            slot.Item = item;
            slot.Count = amount;
            slot.ItemInSlotWasUpdated();
        }

        public static bool AnyItem(this ItemInSlot slot)
        {
            return slot.Item != null && slot.Count > 0;
        }

        public static int IncreaseAmountOfItemsInSlot(this ItemInSlot slot, int increaseAmount)
        {
            if (slot.Item.MaxStack < slot.Count + increaseAmount)
            {
                increaseAmount = slot.Item.MaxStack - slot.Count;
            }

            if (increaseAmount != 0)
            {
                slot.Count += increaseAmount;
                slot.ItemInSlotWasUpdated();
            }

            return increaseAmount;
        }

        public static int GetAvailableSlotsForItem(this ItemInSlot slot)
        {
            return (slot.Item?.MaxStack ?? 0) - slot.Count;
        }
        
        public static void DecreaseAmountOfItemsInSlot(this ItemInSlot slot, int decreaseAmount)
        {
            slot.Count = Math.Max(0, slot.Count - decreaseAmount);
            if (!slot.AnyItem())
                slot.Item = null;
            slot.ItemInSlotWasUpdated();
        }

        public static void RemoveItemFromSlot(this ItemInSlot slot)
        {
            slot.Item = null;
            slot.Count = 0;
            slot.ItemInSlotWasUpdated();
        }

        public static ItemInSlot Clone(this ItemInSlot slot) => new ItemInSlot {Item = slot.Item, Count = slot.Count};

        public static bool CanMergeSlots(this ItemInSlot slot, ItemModel item2) => CanMergeSlots(slot.Item, item2);

        private static bool CanMergeSlots(ItemModel item1, ItemModel item2)
        {
            return item1 == item2 && item1 != null;
        }

        private static void MergeItems(ItemInSlot slotFrom, ItemInSlot slotTo)
        {
            int amountToMerge = slotTo.IncreaseAmountOfItemsInSlot(slotFrom.Count);
            if (amountToMerge > 0)
                slotFrom.DecreaseAmountOfItemsInSlot(amountToMerge);
        }

        private static void ExchangeItemsInSlots(ItemInSlot slot1, ItemInSlot slot2)
        {
            ItemModel item1 = slot1.Item;
            int count1 = slot1.Count;
            slot1.Item = slot2.Item;
            slot1.Count = slot2.Count;
            slot2.Item = item1;
            slot2.Count = count1;
            slot1.ItemInSlotWasUpdated();
            slot2.ItemInSlotWasUpdated();
        }

        private static void ItemInSlotWasUpdated(this ItemInSlot slot)
        {
            GlobalEvents.InvokeItemInSlotUpdated(slot);
        }
    }
}