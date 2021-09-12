//Attach this script to the GameObject you would like to detect dragging on
//Attach an Event Trigger component to the GameObject (Click the Add Component button and go to Event>Event Trigger)
//Make sure the Camera you are using has a Physics Raycaster (Click the Add Component button and go to Event>Physics Raycaster) so it can detect clicks on GameObjects.

using UnityEngine;
using UnityEngine.EventSystems;

public class LevelDragger : MonoBehaviour
{
    void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { OnDragDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    public void OnDragDelegate(PointerEventData data)
    {
        Debug.Log("Dragging");
        Vector2 pos = GetComponent<RectTransform>().position;
        GetComponent<RectTransform>().position = new Vector2(pos.x + data.scrollDelta.x, pos.y);
    }
}