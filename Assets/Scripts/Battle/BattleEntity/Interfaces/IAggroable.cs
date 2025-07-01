using System;
using UnityEngine;

public interface IAggroable
{
    public BattleEntity aggoredTo { get; set; }
}