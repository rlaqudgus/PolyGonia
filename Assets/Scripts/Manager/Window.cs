using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Window : MonoBehaviour
{
    public GameObject firstSelectedOption;

    public void Open()
    {
        gameObject.SetActive(true);
        UIManager.Instance.windows.Add(this);
        
        EventSystem.current.SetSelectedGameObject(firstSelectedOption);

    }

    public void Close()
    {
        gameObject.SetActive(false);
        UIManager.Instance.windows.Remove(this);
        
        GameObject selected;
        int n_windows = UIManager.Instance.windows.Count;
        
        if (n_windows > 0) 
        {
            Window recentWindow = UIManager.Instance.windows[n_windows - 1];
            selected = recentWindow.firstSelectedOption;
        }
        else
        {
            selected = null;
        }
        
        EventSystem.current.SetSelectedGameObject(selected);

    }

    public void Reload()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedOption);
    }

}
