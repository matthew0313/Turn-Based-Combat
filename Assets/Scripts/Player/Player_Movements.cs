using System.Collections.Generic;
using UnityEngine;
using AnimatorCutscene;

namespace Player_Field
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player_Movements : MonoBehaviour, ISavable, ICutsceneCallbackReceiver
    {
        Player origin;
        Rigidbody2D rb;

        [SerializeField] float moveSpeed = 5.0f;

        bool canMove = true;
        public ControlMachine<bool> canMoveControl { get; private set; }
        private void Awake()
        {
            origin = GetComponent<Player>();
            rb = GetComponent<Rigidbody2D>();
            canMoveControl = new(value => canMove = value, true);
        }
        private void Update()
        {
            UpdateMovement();
        }
        readonly int isMovingID = Animator.StringToHash("IsMoving");
        readonly int directionID = Animator.StringToHash("Direction");
        void UpdateMovement()
        {
            if (!canMove || StageManager.Instance.UI.isTabOpen)
            {
                rb.linearVelocity = Vector2.zero; return;
            }
            Vector2 move = InputManager.Instance.Move.ReadValue<Vector2>();
            if(Mathf.Abs(move.x) >= Mathf.Abs(move.y))
            {
                if (move.x > 0) origin.anim.SetInteger(directionID, 0);
                if (move.x < 0) origin.anim.SetInteger(directionID, 2);
            }
            else
            {
                if (move.y > 0) origin.anim.SetInteger(directionID, 1);
                if (move.y < 0) origin.anim.SetInteger(directionID, 3);
            }
            origin.anim.SetBool(isMovingID, move.magnitude > 0.05f);
            rb.linearVelocity = move * moveSpeed;
        }
        private void OnEnable()
        {
            rb.simulated = true;
        }
        private void OnDisable()
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
            origin.anim.SetBool(isMovingID, false);
        }

        public void Save(SaveData data)
        {
            data.playerPos = transform.position;
        }

        public void Load(SaveData data)
        {
            transform.position = data.playerPos;
        }

        readonly ControlElement<bool> cutsceneCanMoveDisable = new(() => false, 9999, true);
        public void OnCutsceneStart()
        {
            canMoveControl.AddControl(cutsceneCanMoveDisable);
        }

        public void OnCutsceneEnd()
        {
            canMoveControl.RemoveControl(cutsceneCanMoveDisable);
        }
    }
}