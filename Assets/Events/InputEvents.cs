using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEvents
{
    public event Action OnSubmitPressed;

    public void SubmitPressed()
    {
        if (OnSubmitPressed != null)
        {
            OnSubmitPressed();
        }
    }   
}
