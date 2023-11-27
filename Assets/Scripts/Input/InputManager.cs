using UnityEngine;
using UnityEngine.Events;
public class InputManager : MonoBehaviour
{
    public static UnityEvent<Vector2> MoveInput = new UnityEvent<Vector2>();
    private void Awake()
    { 
        InputPanel.OnSwipe.AddListener(OnSwipe);
    }
    private void OnSwipe(Vector2 direction)
    {
        MoveInput.Invoke(direction);
    }
}
