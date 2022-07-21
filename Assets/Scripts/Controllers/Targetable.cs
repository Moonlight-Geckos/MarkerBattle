using UnityEngine;

public abstract class Targetable : MonoBehaviour
{
    protected int _uniqueNum;
    public abstract Player PlayerOwner
    {
        get;
    }
    public abstract void GetHit(Stickman stick);
    public override int GetHashCode()
    {
        return _uniqueNum;
    }
    public override bool Equals(object other)
    {
        return other.GetHashCode() == GetHashCode();
    }
    private void OnEnable()
    {
        _uniqueNum = DataHolder.UniqueNum;
    }
}
