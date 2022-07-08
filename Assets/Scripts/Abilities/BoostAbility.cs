using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class BoostAbility : BaseAbility
{
    float boostDuration = 2000f; //ms
    float boostSpeedModifier = 2f;
    float boostTurnRateModifier = 2f;

    Timer boostTimer;

    public BoostAbility(Snake snake) : base(snake)
    {
        boostTimer = new Timer(boostDuration);
        boostTimer.AutoReset = false;
        boostTimer.Elapsed += new ElapsedEventHandler(OnBoostEnd);
    }

    protected override void Perform()
    {
        Debug.Log("Mopsgeschwindigkeit!");
        mySnake.SpeedModifier = boostSpeedModifier;
        mySnake.TurnRateModifier = boostTurnRateModifier;
        boostTimer.Stop();
        boostTimer.Start();
    }

    void OnBoostEnd(object sender, ElapsedEventArgs e)
    {
        Debug.Log("Boost End");
        mySnake.SpeedModifier = 1;
        mySnake.TurnRateModifier = 1;
    }

    public override void Stop()
    {
        base.Stop();
        boostTimer.Stop();
        OnBoostEnd(null, null);
    }
}
