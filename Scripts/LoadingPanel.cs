using System;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    public static Action LoadingStartEvent = delegate { };
    public static Action LoadingEndEvent = delegate { };

    private bool _isVisible;

    private void OnEnable()
    {
        LoadingStartEvent += Show;
        LoadingEndEvent += Hide;
    }

    private void OnDisable()
    {
        LoadingStartEvent -= Show;
        LoadingEndEvent -= Hide;
    }

    private void Show()
    {
        if (_isVisible) return;
        
        _isVisible = true;
        _panel.SetActive(true);
    }

    private void Hide()
    {
        if (!_isVisible) return;
        
        _isVisible = false;
        _panel.SetActive(false);
    }
}