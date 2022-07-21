using System;
using UnityEngine;

[Serializable]
public class Player
{
    public Color mainColor;
    public int number;
    public float damage = 1;
    public float attackCooldown;
    public KdTree<Targetable> _targetsTree;
}
