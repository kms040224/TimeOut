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
            slots[i].Slotnum = i; // ���� ��ȣ �缳��

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

            if (inventoryPanel != null) // null üũ �߰�
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
                slots[i].Slotnum = i;  // ���� ��ȣ�� �ùٸ��� ����
                slots[i].RemoveSlot();
            }
        }

        // �κ��丮 ������ �� ��ŭ ������ ������Ʈ
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