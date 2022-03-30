using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility
{
    protected Snake mySnake;
    public int Uses = 999;

    public BaseAbility(Snake snake)
    {
        mySnake = snake;
    }

    public void Activate()
    {
        if (Uses > 0)
        {
            Uses--;
            Perform();
        }
    }

    protected virtual void Perform()
    {
    }
}