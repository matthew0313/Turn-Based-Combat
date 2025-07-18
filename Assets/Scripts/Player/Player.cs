using System.Collections.Generic;
using UnityEngine;

namespace Player_Field
{
    [RequireComponent(typeof(Player_Movements), typeof(Player_Interactions))]
    public class Player : MonoBehaviour
    {
        [SerializeField] Animator m_anim;
        public Animator anim => m_anim;


        Player_Movements m_movements;
        public Player_Movements movements
        {
            get
            {
                if(m_movements == null) m_movements = GetComponent<Player_Movements>();
                return m_movements;
            }
        }
        Player_Interactions m_interactions;
        public Player_Interactions interactions
        {
            get
            {
                if(m_interactions == null) m_interactions = GetComponent<Player_Interactions>();
                return m_interactions;
            }
        }
    }
}