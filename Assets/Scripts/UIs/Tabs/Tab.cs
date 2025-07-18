using System.Collections.Generic;
using UnityEngine;

public abstract class Tab : MonoBehaviour
{
    public bool isOpen { get; private set; } = false;
    public virtual void Open()
    {
        isOpen = true;
    }
    public virtual void Close()
    {
        isOpen = false;
    } 
}