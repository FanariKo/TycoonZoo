using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMove : MonoBehaviour
{
    public float speed = 5f;            // Скорость движения вперед
    public float rotationSpeed = 50f;  // Скорость поворота по кругу
    public float circleRadius = 5f;    // Радиус круга
    public float returnSpeed = 5f;     // Скорость возвращения в начальную точку

    private Vector3 startPosition;     // Начальная позиция
    private bool movingForward = true;
    private bool rotating = false;
    private bool returning = false;

    private float circleAngle;         // Угол поворота объекта по кругу
    private Vector3 circleCenter;      // Центр окружности

    void Start()
    {
        // Запоминаем начальную позицию объекта
        startPosition = transform.position;
    }

    void Update()
    {
        if (movingForward)
        {
            // Движение вперед
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Условие для перехода к движению по кругу
            if (Vector3.Distance(transform.position, startPosition) >= circleRadius)
            {
                movingForward = false;
                rotating = true;

                // Вычисляем центр окружности и начальный угол
                circleCenter = startPosition;
                Vector3 offset = transform.position - circleCenter;
                circleAngle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg; // Начальный угол
            }
        }
        else if (rotating)
        {
            // Плавное движение по кругу
            float previousAngle = circleAngle;
            circleAngle += rotationSpeed * Time.deltaTime; // Увеличиваем угол

            float previousX = Mathf.Cos(previousAngle * Mathf.Deg2Rad) * circleRadius;
            float previousZ = Mathf.Sin(previousAngle * Mathf.Deg2Rad) * circleRadius;

            float x = Mathf.Cos(circleAngle * Mathf.Deg2Rad) * circleRadius;
            float z = Mathf.Sin(circleAngle * Mathf.Deg2Rad) * circleRadius;

            Vector3 previousPosition = circleCenter + new Vector3(previousX, 0, previousZ);
            Vector3 currentPosition = circleCenter + new Vector3(x, 0, z);

            transform.position = currentPosition;

            // Поворачиваем объект в направлении движения
            Vector3 direction = (currentPosition - previousPosition).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // Если полный круг завершен
            if (circleAngle >= 360f)
            {
                circleAngle = 0f;
                rotating = false;
                returning = true;
            }
        }
        else if (returning)
        {
            // Возвращение в начальную точку
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);

            // Если объект достиг начальной точки, повторяем цикл
            if (Vector3.Distance(transform.position, startPosition) < 0.1f)
            {
                returning = false;
                movingForward = true;
            }
        }
    }
}
