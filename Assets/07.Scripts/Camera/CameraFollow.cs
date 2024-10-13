using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // ī�޶� ����ٴ� ĳ����
    public Vector3 offset;        // ī�޶�� ĳ���� ������ �Ÿ�
    public float smoothSpeed = 0.125f;  // ī�޶��� �������� �ε巴�� �ϱ� ���� �ӵ�
    public float rotationAngle = 45f;   // ī�޶��� ���ͺ� ȸ�� ����
    public float height = 5f;     // ī�޶� ĳ���� ���� �󸶳� ���� ��ġ����

    public float zoomSpeed = 2f;  // ����/�ƿ� �ӵ�
    public float minZoom = 2f;    // ���� ����
    public float maxZoom = 10f;   // �ܾƿ� ����

    private float currentZoom = 5f; // ���� �� ��

    void LateUpdate()
    {
        if (target == null) return;

        // ���콺 �ٷ� ����/�ƿ� �� ����
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom); // �� ���� ����

        // ĳ������ ��ġ�� ���� ī�޶��� ���ο� ��ġ ���
        Vector3 desiredPosition = target.position + Quaternion.Euler(0, rotationAngle, 0) * offset * currentZoom;
        desiredPosition.y += height;  // ī�޶� ���� ����

        // �ε巴�� ī�޶� �̵�
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // ī�޶� �׻� ĳ���͸� �ٶ󺸵��� ����
        transform.LookAt(target);
    }
}
