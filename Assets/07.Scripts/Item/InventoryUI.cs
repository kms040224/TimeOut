using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    Inventory inven;

    public GameObject inventoryPanel;
    bool activeInventory = false;

    public Slot[] slots;
    public Transform slotHolder;
    private void Start()
    {
        inven = Inventory.instance;
        slots = slotHolder.GetComponentsInChildren<Slot>();
        inven.onSlotCountChange += SlotChange;
        inven.onChangeItem += RedrawSlotUI;
        inventoryPanel.SetActive(activeInventory);
        SlotChange(inven.SlotCnt);
    }

    private void SlotChange(int val)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Slotnum = i; // 슬롯 번호 재설정

            if (i < inven.SlotCnt)
                slots[i].GetComponent<Button>().interactable = true;
            else
                slots[i].GetComponent<Button>().interactable = false;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;

            if (inventoryPanel != null) // null 체크 추가
                inventoryPanel.SetActive(activeInventory);
        }
    }

    public void AddSlot()
    {
        inven.SlotCnt++;
    }

    void RedrawSlotUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].Slotnum = i;  // 슬롯 번호를 올바르게 설정
                slots[i].RemoveSlot();
            }
        }

        // 인벤토리 아이템 수 만큼 슬롯을 업데이트
        for (int i = 0; i < Mathf.Min(inven.items.Count, slots.Length); i++)
        {
            if (slots[i] != null)
            {
                slots[i].item = inven.items[i];
                slots[i].UpdateSlotUI();
            }
        }
    }
}