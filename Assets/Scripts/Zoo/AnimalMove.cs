using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMove : MonoBehaviour
{
    public float speed = 5f;            // �������� �������� ������
    public float rotationSpeed = 50f;  // �������� �������� �� �����
    public float circleRadius = 5f;    // ������ �����
    public float returnSpeed = 5f;     // �������� ����������� � ��������� �����

    private Vector3 startPosition;     // ��������� �������
    private bool movingForward = true;
    private bool rotating = false;
    private bool returning = false;

    private float circleAngle;         // ���� �������� ������� �� �����
    private Vector3 circleCenter;      // ����� ����������

    void Start()
    {
        // ���������� ��������� ������� �������
        startPosition = transform.position;
    }

    void Update()
    {
        if (movingForward)
        {
            // �������� ������
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // ������� ��� �������� � �������� �� �����
            if (Vector3.Distance(transform.position, startPosition) >= circleRadius)
            {
                movingForward = false;
                rotating = true;

                // ��������� ����� ���������� � ��������� ����
                circleCenter = startPosition;
                Vector3 offset = transform.position - circleCenter;
                circleAngle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg; // ��������� ����
            }
        }
        else if (rotating)
        {
            // ������� �������� �� �����
            float previousAngle = circleAngle;
            circleAngle += rotationSpeed * Time.deltaTime; // ����������� ����

            float previousX = Mathf.Cos(previousAngle * Mathf.Deg2Rad) * circleRadius;
            float previousZ = Mathf.Sin(previousAngle * Mathf.Deg2Rad) * circleRadius;

            float x = Mathf.Cos(circleAngle * Mathf.Deg2Rad) * circleRadius;
            float z = Mathf.Sin(circleAngle * Mathf.Deg2Rad) * circleRadius;

            Vector3 previousPosition = circleCenter + new Vector3(previousX, 0, previousZ);
            Vector3 currentPosition = circleCenter + new Vector3(x, 0, z);

            transform.position = currentPosition;

            // ������������ ������ � ����������� ��������
            Vector3 direction = (currentPosition - previousPosition).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // ���� ������ ���� ��������
            if (circleAngle >= 360f)
            {
                circleAngle = 0f;
                rotating = false;
                returning = true;
            }
        }
        else if (returning)
        {
            // ����������� � ��������� �����
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);

            // ���� ������ ������ ��������� �����, ��������� ����
            if (Vector3.Distance(transform.position, startPosition) < 0.1f)
            {
                returning = false;
                movingForward = true;
            }
        }
    }
}
