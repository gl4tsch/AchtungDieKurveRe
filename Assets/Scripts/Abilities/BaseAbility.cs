using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility
{
    protected Snake mySnake;
    int uses = 999;
    public int Uses => uses;

    public Action<int> OnUsesChanged;

    public BaseAbility(Snake snake)
    {
        mySnake = snake;
    }

    public void SetUses(int uses)
    {
        this.uses = uses;
        OnUsesChanged?.Invoke(uses);
    }

    public void Activate()
    {
        if (uses > 0)
        {
            Perform();
            uses--;
            OnUsesChanged?.Invoke(Uses);
        }
    }

    protected virtual void Perform()
    {
    }
}