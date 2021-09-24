using System;
using System.Collections.Generic;
using System.Linq;
using Balancy.Models;
using UnityEngine;

namespace Balancy.Data
{
    public enum InventoryError
    {
        None,
        InventoryIsFull,
        NoItem,
        CantPutItemIntoThisSlot,
    }

    public static class InventoryExtension
    {
        public static Balancy.Data.Inventory Create(InventoryConfig config)
        {
            var inventory = Balancy.Data.Inventory.Instantiate();
            inventory.Config = config;
            inventory.ValidateAndFix();
            return inventory;
        }
        
        public static void ValidateAndFix(this Balancy.Data.Inventory inventory)
        {
            var inventorySize = Mathf.Clamp(inventory.ItemSlots.Count, inventory.Config.MinSlot, inventory.Config.MaxSlots);
            if (inventorySize != inventory.ItemSlots.Count)
                inventory.ChangeSlotsCountWithRearrangement(inventorySize);
        }
        
        public static InventoryError ForceAddItem(this Balancy.Data.Inventory inventory, ItemModel item, ref int amountToPut)
        {
            inventory.AddItem(item, ref amountToPut);
            while (amountToPut > 0)
            {
                var wasCount = amountToPut;
                
                inventory.Expand();
                inventory.AddItem(item, ref amountToPut);
                
                if (wasCount == amountToPut)
                    return InventoryError.CantPutItemIntoThisSlot;
            }

            return InventoryError.None;
        }
        
        public static InventoryError AddItem(this Balancy.Data.Inventory inventory, ItemModel item, ref int amountToPut)
        {
            inventory.AddItemOnlyMerge(item, ref amountToPut);
            return inventory.AddItemWithoutMerge(item, ref amountToPut);
        }
        
        public static void AddItemOnlyMerge(this Balancy.Data.Inventory inventory, ItemModel item, ref int amountToPut)
        {
            inventory.TryToMergeItemWithOtherSlots(item, ref amountToPut);
        }

        public static InventoryError AddItemWithoutMerge(this Balancy.Data.Inventory inventory, ItemModel item, ref int amountToPut)
        {
            return amountToPut <= 0 ? InventoryError.None : inventory.PlaceItemIntoTheFirstAvailableSlot(item, ref amountToPut);
        }
        
        public static int GetAvailableSlotsForItem(this Balancy.Data.Inventory inventory, ItemModel item)
        {
            int count = 0;
            if (item.MaxStack > 1)
                count += inventory.GetAvailableSlotsForMerge(item);

            count += inventory.GetAvailableEmptySlotsCount(item) * item.MaxStack;
            return count;
        }

        public static InventoryError PlaceItemIntoTheFirstAvailableSlot(this Balancy.Data.Inventory inventory, ItemModel item, ref int count)
        {
            foreach (var slot in inventory.ItemSlots)
            {
                if (slot.AnyItem()) continue;

                if (!slot.CanAcceptItem(item)) continue;
                
                var itemToPut = Mathf.Min(count, item.MaxStack);
                slot.ItemInSlot.PutNewItemInSlot(item, itemToPut);
                count -= itemToPut;
                if (count == 0)
                    return InventoryError.None;
            }

            return InventoryError.InventoryIsFull;
        }

        public static ItemSlot GetAvailableSlotOfType(this Balancy.Data.Inventory inventory, ItemModel item, bool includeNonEmpty = false)
        {
            foreach (var slot in inventory.ItemSlots)
            {
                if (!includeNonEmpty && slot.AnyItem()) continue;
                if (slot.CanAcceptItem(item)) return slot;
            }

            return null;
        }
        
        public static ItemSlot GetSlotOfType(this Balancy.Data.Inventory inventory, ItemModel item)
        {
            foreach (var slot in inventory.ItemSlots)
            {
                if (slot.CanAcceptItem(item)) return slot;
            }

            return null;
        }

        public static bool CanAcceptItem(this Balancy.Data.Inventory inventory, ItemModel item)
        {
            foreach (var slot in inventory.ItemSlots)
            {
                if (slot.CanAcceptItem(item))
                    return true;
            }

            return false;
        }

        public static int GetTakenSlotsCount(this Balancy.Data.Inventory inventory)
        {
            int count = 0;
            foreach (var slot in inventory.ItemSlots)
            {
                if (slot.AnyItem())
                    count++;
            }

            return count;
        }

        public static void ChangeSlotsCountWithRearrangement(this Balancy.Data.Inventory inventory, int newSlotsCount)
        {
            if (newSlotsCount >= inventory.ItemSlots.Count)
                inventory.Expand(newSlotsCount - inventory.ItemSlots.Count);
            else
            {
                List<ItemInSlot> itemsToRearrange = new List<ItemInSlot>();
                for (int i = newSlotsCount; i < inventory.ItemSlots.Count; i++)
                {
                    if (inventory.ItemSlots[i].AnyItem())
                        itemsToRearrange.Add(inventory.ItemSlots[i].ItemInSlot);
                }

                inventory.ItemSlots.RemoveRange(newSlotsCount, inventory.ItemSlots.Count - newSlotsCount);

                foreach (var itemInSlot in itemsToRearrange)
                {
                    int count = itemInSlot.Count;
                    inventory.AddItem(itemInSlot.Item, ref count);
                }
            }
        }

        public static bool Contains(this Balancy.Data.Inventory inventory, ItemSlot itemSlot)
        {
            foreach (var slot in inventory.ItemSlots)
                if (itemSlot == slot)
                    return true;
            return false;
        }
        
        public static bool Contains(this Balancy.Data.Inventory inventory, ItemInSlot itemInSlot)
        {
            foreach (var slot in inventory.ItemSlots)
                if (itemInSlot == slot.ItemInSlot)
                    return true;
            return false;
        }
        
        public static bool Contains(this Balancy.Data.Inventory inventory, ItemModel itemModel)
        {
            foreach (var slot in inventory.ItemSlots)
                if (itemModel == slot.ItemInSlot.Item)
                    return true;
            return false;
        }

        public static int GetItemsCount(this Balancy.Data.Inventory inventory, ItemModel item = null)
        {
            int count = 0;
            foreach (var slot in inventory.ItemSlots)
                if ((item == null || item == slot.ItemInSlot.Item) && slot.AnyItem())
                    count += slot.ItemInSlot.Count;

            return count;
        }
        
        public static int GetItemsCount(this Balancy.Data.Inventory inventory, SlotType slotType)
        {
            int count = 0;
            foreach (var slot in inventory.ItemSlots)
                if ((slot.Type & slotType) != 0 && slot.AnyItem())
                    count += slot.ItemInSlot.Count;
            return count;
        }

        public static ItemSlot GetFirstSlot(this Balancy.Data.Inventory inventory, SlotType slotType)
        {
            foreach (var slot in inventory.ItemSlots)
                if ((slot.Type & slotType) != 0)
                    return slot;
            return null;
        }
        
        public static int GetFilledSlotsCount(this Balancy.Data.Inventory inventory)
        {
            int count = 0;
            foreach (var slot in inventory.ItemSlots)
                if (slot.AnyItem())
                    count++;

            return count;
        }
        
        public static int GetItemsCount(this Balancy.Data.Inventory inventory, string itemUnnyId)
        {
            int count = 0;
            foreach (var slot in inventory.ItemSlots)
                if (itemUnnyId == slot.ItemInSlot.Item.UnnyId && slot.AnyItem())
                    count += slot.ItemInSlot.Count;

            return count;
        }

        public static bool IsEmpty(this Balancy.Data.Inventory inventory)
        {
            return inventory.GetItemsCount() == 0;
        }

        public static void TakeAllItems(this Balancy.Data.Inventory inventory, ItemModel item)
        {
            foreach (var slot in inventory.ItemSlots)
            { 
                if (item == slot.ItemInSlot.Item)
                    slot.ItemInSlot.DecreaseAmountOfItemsInSlot(slot.ItemInSlot.Count);
            }
        }
        
        public static void TakeItems(this Balancy.Data.Inventory inventory, ItemModel item, ref int amountToTake)
        {
            var itemSlots = new List<ItemSlot>();
            foreach (var slot in inventory.ItemSlots)
            { 
                if (item == slot.ItemInSlot.Item)
                {
                    itemSlots.Add(slot);
                }
            }

            itemSlots = itemSlots.OrderBy(s => s.ItemInSlot.Count).ToList();
            
            foreach (var slot in itemSlots)
            { 
                if (amountToTake <= slot.ItemInSlot.Count)
                {
                    slot.ItemInSlot.DecreaseAmountOfItemsInSlot(amountToTake);
                    amountToTake = 0;
                    break;
                }

                amountToTake -= slot.ItemInSlot.Count;
                slot.ItemInSlot.RemoveItemFromSlot();
            }
        }

        #region service methods

        private static void TryToMergeItemWithOtherSlots(this Balancy.Data.Inventory inventory, ItemModel item, ref int count)
        {
            foreach (var slot in inventory.ItemSlots)
            {
                if (slot.ItemInSlot.Item != item) continue;

                count -= slot.IncreaseAmountOfItemsInSlot(count);
                if (count == 0)
                    break;
            }
        }

        public static void Expand(this Balancy.Data.Inventory inventory, int slotsCount = 1)
        {
            for (int i = 0; i < slotsCount; i++)
                inventory.ItemSlots.Add(ItemSlotExtension.Create());
        }

        private static int GetAvailableSlotsForMerge(this Balancy.Data.Inventory inventory, ItemModel item)
        {
            int count = 0;
            
            foreach (var slot in inventory.ItemSlots)
            {
                if (slot.ItemInSlot.Item == item)
                    count += slot.GetAvailableSlotsForItem();
            }

            return count;
        }
        
        public static int GetEmptySlotsCount(this Balancy.Data.Inventory inventory)
        {
            int count = 0;
            
            foreach (var slot in inventory.ItemSlots)
            {
                if (!slot.AnyItem())
                    count++;
            }

            return count;
        }
        
        public static int GetAvailableEmptySlotsCount(this Balancy.Data.Inventory inventory, ItemModel itemModel)
        {
            int count = 0;
            
            foreach (var slot in inventory.ItemSlots)
            {
                if (!slot.AnyItem() && slot.CanAcceptItem(itemModel))
                    count++;
            }

            return count;
        }

        #endregion
    }
}