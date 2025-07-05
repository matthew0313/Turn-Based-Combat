using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;

public interface IAlimentTextDisplayed
{
    public string alimentText { get; }
    public event Action onAlimentTextChange;
}