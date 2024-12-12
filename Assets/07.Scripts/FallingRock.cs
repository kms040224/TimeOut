using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    public float damage = 20f; // ������ ������ ������
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ������Ʈ�� �����ɴϴ�.
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered with: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Vector3 hitDirection = other.transform.position - transform.position;
                hitDirection.Normalize();
                player.TakeDamage(hitDirection, damage);
            }
        }
    }
}
