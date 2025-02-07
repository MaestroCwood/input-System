using System;
using UnityEngine;
using YG;

namespace Services
{
    public class InputService : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private YGTouchscreen ygTouchscreen;
        
        public static InputService Instance;
        public bool IsMobile { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsAttacking { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            Initialize();
        }

        private void Update()
        {
            Debug.Log(YandexGame.EnvironmentData.deviceType);
        }

        private void Initialize()
        {
            IsMobile = YandexGame.EnvironmentData.deviceType == "mobile";

            if (IsMobile)
            {
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
                return;
            }
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false; 
        }

        public Vector2 GetMovementAxisRaw()
        {
            Vector2 moveDir;
        
            if (IsMobile)
            {
                moveDir = new Vector2(SimpleInput.GetAxisRaw("Horizontal"),
                    SimpleInput.GetAxisRaw("Vertical"));
            
                return moveDir;
            }
            moveDir = new Vector2(Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"));
            
            return moveDir;
        }
        public Vector2 GetLookAxisRaw()
        {
            Vector2 moveDir;


            if (IsMobile)
            {
                moveDir = ygTouchscreen.GetTouchscreenInput();
                Debug.Log(moveDir);
                return moveDir;   
            }

            moveDir = new Vector2(Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y"));
            
            return moveDir;
        }
        public bool GetJumpButton()
        {
            if (IsMobile)
                return SimpleInput.GetButtonDown("Jump");
            
            return Input.GetKeyDown(KeyCode.Space);
        }
        public bool GetActionButton()
        {
            if (IsMobile)
            {
                var isJump = SimpleInput.GetButtonDown("Attack");
                return isJump;
            }

            return Input.GetMouseButtonDown(0);
        }
    }
}
