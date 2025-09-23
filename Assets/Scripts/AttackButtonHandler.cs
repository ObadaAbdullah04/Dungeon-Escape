using UnityEngine;
using UnityEngine.EventSystems;

public class AttackButtonHandler : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.Instance.TriggeAttack();
    }
}
