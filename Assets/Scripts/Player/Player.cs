using System.Collections.Generic;
using UnityEngine;

namespace Player_Field
{
    [RequireComponent(typeof(Player_Movements), typeof(Player_Interactions))]
    public class Player : MonoBehaviour
    {
        [SerializeField] Animator m_anim;
        public Animator anim => m_anim;


        public Player_Movements movements;
        public Player_Interactions interactions;
        private void Awake()
        {
            movements = GetComponent<Player_Movements>();
            interactions = GetComponent<Player_Interactions>();
        }
    }
}