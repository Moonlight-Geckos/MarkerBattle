using UnityEngine;

public abstract class Targetable : MonoBehaviour
{
    public abstract Player PlayerOwner
    {
        get;
    }
    public abstract void GetHit(Stickman stick);
}
