using UnityEngine;
using UnityEngine.EventSystems;

public class TapPanel : MonoBehaviour
{

    private CircleSpawner _spawnerInstance;
    private Vector3 _position;
    private void Awake()
    {
        var eventTrigger = GetComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(GrowCircle);
        eventTrigger.triggers.Add(entry);


        EventTrigger.Entry _stopGrowEntry = new EventTrigger.Entry();
        _stopGrowEntry.eventID = EventTriggerType.PointerUp;
        _stopGrowEntry.callback.AddListener(StopGrow);
        eventTrigger.triggers.Add(_stopGrowEntry);
    }
    private void GrowCircle(BaseEventData data)
    {
        if(_spawnerInstance == null)
        {
            _spawnerInstance = gameObject.AddComponent<CircleSpawner>();
        }
        PointerEventData p_data = (PointerEventData)data;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(p_data.position), out hit, 100))
        {
            if (hit.collider.gameObject.layer == StaticValues.CircleLayer)
                return;
            _position = hit.point;
            _position.y = -0.08f;
            _spawnerInstance.GrowCircle(0, _position);
        }
    }
    private void StopGrow(BaseEventData data)
    {
        if (_spawnerInstance == null)
            return;
        _spawnerInstance.StopGrowing();
    }
}
