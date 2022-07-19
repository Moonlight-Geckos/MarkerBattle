using UnityEngine;

public class Radar : MonoBehaviour
{
    Stickman _stickman;
    private void Awake()
    {
        _stickman = GetComponentInParent<Stickman>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent.name != transform.parent.name)
        {
            _stickman.TargetDetected(other.GetComponentInParent<Targetable>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.name != transform.parent.name)
        {
            _stickman.TargetOut(other.GetComponentInParent<Targetable>());
        }
    }
}
