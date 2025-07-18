using System.Collections.Generic;
using UnityEngine;
using MEC;
using Player_Battle;
using AnimatorCutscene;

namespace Player_Field
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player_Movements : MonoBehaviour, ISavable, ICutsceneCallbackReceiver
    {
        Player origin;
        Rigidbody2D rb;

        [SerializeField] Transform flipPart;
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
        void UpdateMovement()
        {
            if (!canMove)
            {
                rb.linearVelocity = Vector2.zero; return;
            }
            Vector2 move = InputManager.Instance.Move.ReadValue<Vector2>();
            if (move.x > 0) flipPart.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            if (move.x < 0) flipPart.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            origin.anim.SetBool(isMovingID, move.x != 0 || move.y != 0);
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

        readonly ControlElement<bool> cutsceneCanMoveDisable = new(false, 9999, true);
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