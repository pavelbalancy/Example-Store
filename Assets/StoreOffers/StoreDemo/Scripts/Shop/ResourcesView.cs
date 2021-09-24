using Balancy.Data;
using Balancy.Models;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesView : MonoBehaviour
{
    [SerializeField]
    private Text _text;
    
    [SerializeField]
    private SlotType _slotType;
    
    [SerializeField]
    private GameObject _animation;
    
    private int _currentValue;
    
    private void Awake()
    {
        GlobalEvents.ProfileInitializedEvent += OnProfileInitialized;
        GlobalEvents.ItemInSlotUpdatedEvent += OnItemInSlotUpdated;
    }
    
    private void OnDestroy()
    {
        GlobalEvents.ProfileInitializedEvent -= OnProfileInitialized;
        GlobalEvents.ItemInSlotUpdatedEvent -= OnItemInSlotUpdated;
    }

    private void OnProfileInitialized(Profile profile)
    {
        RefreshValue(profile.Resources.GetItemsCount(_slotType), false);
    }

    private void OnItemInSlotUpdated(ItemInSlot itemInSlot)
    {
        if (itemInSlot.AnyItem() && (itemInSlot.Item.SlotMask & _slotType) != 0)
        {
            var increased = _currentValue < itemInSlot.Count;
            RefreshValue(itemInSlot.Count, increased);
        }
    }

    private void RefreshValue(int value, bool animated)
    {
        _currentValue = value;
        _text.text = value.ToString();
        if (animated)
            PlayAnimation();
    }

    private void PlayAnimation()
    {
        _animation.SetActive(false);
        _animation.SetActive(true);
    }
}
