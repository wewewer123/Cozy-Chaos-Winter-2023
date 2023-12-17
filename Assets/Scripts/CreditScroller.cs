using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScroller : MonoBehaviour
{
    [SerializeField] private float scrollLength;
    [SerializeField] private float scrollTime;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    void Start()
    {
        _rectTransform.DOAnchorPosY(scrollLength, scrollTime).onComplete = LoadMainMenu;
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
