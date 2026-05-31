using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AudioClip hoverSound;
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySfx(hoverSound);
        transform.DOScale(Vector3.one * 1.2f, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.5f);
    }
}
