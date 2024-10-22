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
            Debug.Log("Slot number: " + Slotnum); // ���� ���� ��ȣ ���

            // ���� ���� ��ȣ�� ����Ͽ� �κ��丮���� ������ ����
            bool isUse = item.Use();
            if (isUse)
            {
                // �ε����� ����Ͽ� ������ ����
                Inventory.instance.RemoveItem(Slotnum);
                Debug.Log("Removing item from slot: " + Slotnum);
            }
        }
    }
}
