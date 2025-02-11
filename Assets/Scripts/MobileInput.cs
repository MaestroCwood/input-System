using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;

    // Ќастройки игрока(надо сделать менюшку дл€ выбора чувствительности)
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveInputDeadZone;

    // ќбнаружение нажатий
    private int leftFingerId, rightFingerId;
    private float halfScreenWidth;

    //  амера
    private Vector2 lookInput;
    private float cameraPitch;

    // ѕередвижение
    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;

    void Start()
    {
        // id = -1 означает что нажатие не найдено т.е. его нет
        leftFingerId = -1;
        rightFingerId = -1;

        // расчет разделительной 'черты' экрана
        halfScreenWidth = Screen.width / 2;

        // расчет погрешности
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
    }

    // Update is called once per frame
    void Update()
    {
        // Handles input
        GetTouchInput();


        if (rightFingerId != -1)
        {
            // если нашлось нажатие с правой части экрана запускаем расчет поворота игрока
            Debug.Log("Rotating");
            LookAround();
        }

        if (leftFingerId != -1)
        {
            // если нашлось нажатие с левой части экрана заупскаем расчет передвижени€ игрока
            Debug.Log("Moving");
            Move();
        }
    }

    void GetTouchInput()
    {
        // иттерируем каждое нажатие
        for (int i = 0; i < Input.touchCount; i++)
        {

            Touch t = Input.GetTouch(i);

            // провер€ем в какой фазе находитс€ нажатие
            switch (t.phase)
            {
                case TouchPhase.Began:

                    if (t.position.x < halfScreenWidth && leftFingerId == -1)
                    {
                        // заносим id касани€ в leftFingerId
                        leftFingerId = t.fingerId;

                        // заносим позицию касани€ в moveTouchStartPosition
                        moveTouchStartPosition = t.position;
                    }
                    else if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        // заносим id касани€ в rightFingerId
                        rightFingerId = t.fingerId;
                    }

                    break;
                case TouchPhase.Ended:// два case подр€д обозначают or. “о что под ними выполнитс€ при одном из этих условий(или двух сразу)
                case TouchPhase.Canceled:

                    if (t.fingerId == leftFingerId)
                    {
                        // прекращаем следить за левым нажатием
                        leftFingerId = -1;
                        Debug.Log("Ћевый палец отпущен");
                    }
                    else if (t.fingerId == rightFingerId)
                    {
                        // прекращаем следить за правым нажатием
                        rightFingerId = -1;
                        Debug.Log("ѕравый палец отпущен");
                    }

                    break;
                case TouchPhase.Moved:

                    
                    if (t.fingerId == rightFingerId)
                    {
                        // –асчитываем данные дл€ поворота игрока
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    }
                    else if (t.fingerId == leftFingerId)
                    {

                        // –асчитываем передвижение игрокка
                        moveInput = t.position - moveTouchStartPosition;
                    }

                    break;
                case TouchPhase.Stationary:
                    // ≈сли палец неподвижен или передвижение меньше погрешности, то устанавливаем lookInput нулевым
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

        // ¬ертикальное вращение
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // √оризонтальное вращение
        transform.Rotate(transform.up, lookInput.x);
    }

    void Move()
    {

        // Ќе вращаем если движение меньше или равно погрешности
        if (moveInput.sqrMagnitude <= moveInputDeadZone) return;

        // умножаем нормализованое направление на скорость передвижени€
        Vector2 movementDirection = moveInput.normalized * moveSpeed * Time.deltaTime;
        // ƒвигаем игрока относительно направлени€ взгл€да
        characterController.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);
    }
}
