using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Slot : MonoBehaviour, IPointerUpHandler
{
    public int Slotnum;
    public Item item;
    public Image itemIcon;

    public void UpdateSlotUI()
    {
        itemIcon.sprite = item.itemImage;
        itemIcon.gameObject.SetActive(true);
    }
    public void RemoveSlot()
    {
        item = null;
        itemIcon.gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (item != null)
        {
            Debug.Log("Slot number: " + Slotnum); // 현재 슬롯 번호 출력

            // 현재 슬롯 번호를 사용하여 인벤토리에서 아이템 제거
            bool isUse = item.Use();
            if (isUse)
            {
                // 인덱스를 사용하여 아이템 제거
                Inventory.instance.RemoveItem(Slotnum);
                Debug.Log("Removing item from slot: " + Slotnum);
            }
        }
    }
}
