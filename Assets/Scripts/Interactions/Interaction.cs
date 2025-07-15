using System.Collections.Generic;
using UnityEngine;
using Player_Field;

public abstract class Interaction : MonoBehaviour
{
    public abstract string interactText { get; }
    public virtual Vector2 interactPosition => transform.position;
    Player m_player;
    protected Player player
    {
        get
        {
            if (m_player == null) m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            return m_player;
        }
    }
    bool added = false;
    protected virtual bool canInteract => gameObject.activeInHierarchy && enabled;
    protected void Addinteraction()
    {
        if (added) return;

        player.interactions.AddInteraction(this);
        added = true;
    }
    protected void RemoveInteraction()
    {
        if (!added) return;

        player.interactions.RemoveInteraction(this);
        added = false;
    }
    protected virtual void Update()
    {
        if (!canInteract)
        {
            RemoveInteraction(); return;
        }

        float dist = Vector2.Distance(interactPosition, player.transform.position);
        Vector2 dir = ((Vector2)player.transform.position - interactPosition).normalized;
        if (dist <= player.interactions.interactRange && !Physics2D.Raycast(interactPosition, dir, dist, LayerMask.GetMask("Map")))
        {
            Addinteraction();
        }
        else RemoveInteraction();
    }
    public abstract void OnInteract();
    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }
}