using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class Menu : MonoBehaviour
{
    CanvasGroup cg;
    [SerializeField] Ease curve = Ease.OutCubic;
    [SerializeField] Vector2 startPosition;
    [SerializeField] Vector2 finalPosition;
    Vector2 origin;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)startPosition);
        Gizmos.DrawSphere(transform.position + (Vector3)startPosition, 5);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 5);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)finalPosition);
        Gizmos.DrawSphere(transform.position + (Vector3)finalPosition, 5);
    }

    void Start()
    {
        // position alignement
        origin = transform.position;
        startPosition += origin;
        finalPosition += origin;

        cg = GetComponent<CanvasGroup>();
        MenuManager.current.onSetMenu += OnSetMenu; // Subscribe to event
    }

    void OnSetMenu(string name)
    {
        // Set alpha to 1 when menu is opened and disable raycast blocking when closed
        bool itsMe = name == gameObject.name;
        cg.DOKill(true);
        cg.blocksRaycasts = itsMe;
        if (itsMe)
        {
            cg.DOFade(1, 0.4f).SetEase(curve);
            transform.position = startPosition;
            transform.DOMove(origin, 0.6f).SetEase(curve);
        }
        else
        {
            cg.DOFade(0, 0.4f).SetEase(curve);
            transform.position = origin;
            transform.DOMove(finalPosition, 0.6f).SetEase(curve);
        }
            
    }
}
