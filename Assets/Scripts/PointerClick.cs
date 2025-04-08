using UnityEngine;
using Valve.VR.Extras;
using UnityEngine.EventSystems;

public class PointerClick : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;

    void OnEnable()
    {
        if (laserPointer != null)
        {
            laserPointer.PointerClick += OnPointerClick;
        }
    }
    void OnDisable()
    {
        if (laserPointer != null)
        {
            laserPointer.PointerClick -= OnPointerClick;
        }
    }
    private void OnPointerClick(object sender, PointerEventArgs e)
    {
        Debug.Log("Laser Click: " + e.target.name);

        ExecuteEvents.Execute<IPointerClickHandler>(
            target: e.target.gameObject,
            eventData: new PointerEventData(EventSystem.current),
            functor: ExecuteEvents.pointerClickHandler
        );
    }
}