using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIViewManager : MonoBehaviour
{
    [SerializeField] private List<UIView> uiViews = new List<UIView>();

    public static UIViewManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIViewManager>();
            }

            return _instance;
        }
    }
    
    private static UIViewManager _instance;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
    }

    public void EnableUIView(GameStates identifier)
    {
        foreach (var uiView in uiViews)
        {
            if (uiView.identifier.Equals(identifier))
            {
                uiView.EnableView();
                break;
            }
        }
    }
    
    public void EnableUIViewExclusive(GameStates identifier)
    {
        foreach (var uiView in uiViews)
        {
            if (uiView.identifier.Equals(identifier))
            {
                uiView.EnableView();
            }
            else
            {
                uiView.DisableView();
            }
        }
    }

    public void DisableUIView(GameStates identifier)
    {
        foreach (var uiView in uiViews)
        {
            if (uiView.identifier.Equals(identifier))
            {
                uiView.DisableView();
            }
        }
    }
}
