using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButtonHandler : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.Instance.TriggerJump();
    }
}
