using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    //ссылки
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;

    // Ќастройки игрока
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveInputDeadZone;

    // ќбнаружение косаний
    private int leftFingerId, rightFingerId;
    private float halfScreenWidth;

    //  амера
    private Vector2 lookInput;
    private float cameraPitch;

    // ѕередвижение игрока
    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;

    void Start()
    {
        // id = -1 означает что касани€ нет
        leftFingerId = -1;
        rightFingerId = -1;

        // расчитываетс€ один раз
        halfScreenWidth = Screen.width / 2;

        // расчет погрешности
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
    }

    void Update()
    {
        
        GetTouchInput();


        if (rightFingerId != -1)
        {
            // Ќачинаем отслеживать движение правого пальца если задетектили его
            Debug.Log("Rotating");
            LookAround();
        }

        if (leftFingerId != -1)
        {
            // Ќачинаем отслеживать движение левого пальца если задетектили его
            Debug.Log("Moving");
            Move();
        }
    }

    void GetTouchInput()
    {
        // ѕроходимс€ по всем касани€м
        for (int i = 0; i < Input.touchCount; i++)
        {

            Touch t = Input.GetTouch(i);

            // ѕровер€ем в каком состо€нии находитс€ косание
            switch (t.phase)
            {
                case TouchPhase.Began:

                    if (t.position.x < halfScreenWidth && leftFingerId == -1)
                    {
                        // Ќачинаем отслеживать левый палец если он только нажалс€
                        leftFingerId = t.fingerId;

                        // ”станавливаем начальную позицию дл€ пальца
                        moveTouchStartPosition = t.position;
                    }
                    else if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        // Ќачинаем отслеживать правый палец если он только нажалс€
                        rightFingerId = t.fingerId;
                    }

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:

                    if (t.fingerId == leftFingerId)
                    {
                        // ѕрекращаем отслеживать левый палец
                        leftFingerId = -1;
                        Debug.Log("Stopped tracking left finger");
                    }
                    else if (t.fingerId == rightFingerId)
                    {
                        // прекращаем отслеживать правый палец
                        rightFingerId = -1;
                        Debug.Log("Stopped tracking right finger");
                    }

                    break;
                case TouchPhase.Moved:

                    // ѕолучение данных дл€ поворота
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    }
                    else if (t.fingerId == leftFingerId)
                    {

                        // –асчет позиции от изначального места нажати€
                        moveInput = t.position - moveTouchStartPosition;
                    }

                    break;
                case TouchPhase.Stationary:
                    // ≈сли палец неподвижен то устанавливаем lookInput равным нулю 
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }

    void LookAround()
    {

        // ѕоворот по вертикали
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // ѕоворот по горизонтали
        transform.Rotate(transform.up, lookInput.x);
    }

    void Move()
    {

        // ≈сли движение меньше погрешности то ничего не делаем
        if (moveInput.sqrMagnitude <= moveInputDeadZone) return;

        // ”множаем полученное направление на скорсть передвижени€
        Vector2 movementDirection = moveInput.normalized * moveSpeed * Time.deltaTime;
        // ѕередвигаем персонажа
        characterController.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);
    }

}