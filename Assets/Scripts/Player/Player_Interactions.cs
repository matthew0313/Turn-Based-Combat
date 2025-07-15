using System.Collections.Generic;
using UnityEngine;
using MEC;
using Player_Battle;
using System;
using AnimatorCutscene;

namespace Player_Field
{
    public class Player_Interactions : MonoBehaviour, ICutsceneCallbackReceiver
    {
        Player origin;

        [SerializeField] float m_interactRange = 3.0f;
        public float interactRange => m_interactRange;

        readonly List<Interaction> interactions = new();
        Interaction selected;

        public event Action<Interaction> onInteractionSelect;

        bool canInteract = true;
        public ControlMachine<bool> canInteractControl { get; private set; }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
        private void Awake()
        {
            origin = GetComponent<Player>();
            canInteractControl = new(value => canInteract = value, true);
        }
        private void Update()
        {
            if (!canInteract) return;
            interactions.Sort((a, b) =>
            {
                return Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position));
            });
            if (interactions.Count > 0)
            {
                if (selected != interactions[0]) Select(interactions[0]);
            }
            else if (selected != null) Deselect();

            if(selected != null && InputManager.Instance.Interact.triggered)
            {
                selected.OnInteract();
            }
        }
        void Select(Interaction interaction)
        {
            if (selected != null) selected.OnDeselect();
            selected = interactions[0];
            selected.OnSelect();

            onInteractionSelect?.Invoke(interaction);
        }
        void Deselect()
        {
            if(selected != null) selected.OnDeselect();
            selected = null;

            onInteractionSelect?.Invoke(null);
        }
        public void AddInteraction(Interaction interaction)
        {
            interactions.Add(interaction);
        }
        public void RemoveInteraction(Interaction interaction)
        {
            interactions.Remove(interaction);
        }
        private void OnDisable()
        {
            if (selected != null) Deselect();
        }

        readonly ControlElement<bool> cutsceneCanInteractDisable = new(false, 9999, true);
        public void OnCutsceneStart()
        {
            Deselect();
            canInteractControl.AddControl(cutsceneCanInteractDisable);
        }

        public void OnCutsceneEnd()
        {
            canInteractControl.RemoveControl(cutsceneCanInteractDisable);
        }
    }
}