using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class InputPanel : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public static UnityEvent<Vector2> OnSwipe = new UnityEvent<Vector2>();
    private Vector2 _mouseStartPos = Vector2.zero;
    private void Awake()
    {
        OnSwipe.RemoveAllListeners();
    }
    public void OnBeginDrag(PointerEventData eventData) => _mouseStartPos = eventData.position;
    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - _mouseStartPos;
        bool xMoreThanY = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
        direction = new Vector2(xMoreThanY ? direction.x / Mathf.Abs(direction.x) : 0, !xMoreThanY ? direction.y / Mathf.Abs(direction.y) : 0);
        OnSwipe.Invoke(direction);
    }
}
