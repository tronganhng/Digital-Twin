using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TabButton : MonoBehaviour
{
    [SerializeField] private string tabButtonName;
    [SerializeField] protected TabContent content;
    [SerializeField] private RectTransform selectedRect;
    [SerializeField] private RectTransform unSelectedRect;
    [SerializeField] private List<TMP_Text> buttonTitles;

    public Action onClick;

    public virtual void Init()
    {
        foreach (var title in buttonTitles)
        {
            title.text = tabButtonName;
        }
    }

    public void OnClick()
    {
        onClick?.Invoke();
    }

    public virtual void SetSelected(bool isSelected, bool isInstant = false)
    {
        content.SetActive(isSelected);

        if (isInstant)
        {
            selectedRect.gameObject.SetActive(isSelected);
            unSelectedRect.gameObject.SetActive(!isSelected);
        }
        else
        {
            SetAnim(isSelected);
        }
    }

    public void SetEnable(bool isActive)
    {
        gameObject.SetActive(isActive);
        content.gameObject.SetActive(isActive);
    }

    private void SetAnim(bool isSelected)
    {
        if (isSelected)
        {
            selectedRect.gameObject.SetActive(true);
            unSelectedRect.gameObject.SetActive(false);
            selectedRect.DOKill();
            selectedRect.localScale = new Vector3(1, 0, 1);
            selectedRect.DOScaleY(1, 0.35f).SetEase(Ease.OutBack);
        }
        else
        {
            unSelectedRect.gameObject.SetActive(true);
            selectedRect.gameObject.SetActive(false);
        }
    }
}