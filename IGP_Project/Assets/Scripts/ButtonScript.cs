using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ButtonScript : NetworkBehaviour
{
    [Networked] private bool isPressed { get; set; }

    private Animator buttonAnimator;

    // Start is called before the first frame update
    void Start()
    {
        buttonAnimator = GetComponentInChildren<Animator>();
    }

    public override void Spawned()
    {
        base.Spawned();

        isPressed = false;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (isPressed)
        {
            buttonAnimator.SetBool("IsPressed", true);
        }
    }

    public void SetIsPressed()
    {
        isPressed = true;
    }

    public bool GetIsPressed()
    {
        return isPressed;
    }
}
