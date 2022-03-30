using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserAbility : BaseAbility
{
    int erasePixelLength = 200;

    public EraserAbility(Snake snake) : base(snake) { }

    protected override void Perform()
    {
        var arenaWidth = Settings.Instance.ArenaWidth;
        var erase = new Snake.LineDrawData();
        erase.UVPosA = mySnake.Position / arenaWidth;
        erase.UVPosB = erase.UVPosA + mySnake.Direction.normalized * ((float)erasePixelLength / (float)arenaWidth);
        erase.thickness = mySnake.Thickness;
        erase.color = new Color(0, 0, 0, 0);
        mySnake.InjectLineDrawData(new List<Snake.LineDrawData> { erase });
    }
}
