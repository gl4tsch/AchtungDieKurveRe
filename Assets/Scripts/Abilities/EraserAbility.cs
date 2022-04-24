using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserAbility : BaseAbility
{
    float eraseLength = 0.13f;
    int erasePixelLength => (int)(eraseLength * Settings.Instance.ArenaWidth.Value);

    public EraserAbility(Snake snake) : base(snake) { }

    protected override void Perform()
    {
        var arenaWidth = Settings.Instance.ArenaWidth.Value;
        var erase = new Snake.LineDrawData();
        erase.UVPosA = mySnake.Position / arenaWidth;
        erase.UVPosB = erase.UVPosA + mySnake.Direction.normalized * eraseLength;
        erase.thickness = mySnake.Thickness / arenaWidth;
        erase.color = new Color(0, 0, 0, 0);
        erase.clipCircle = 1;
        mySnake.InjectLineDrawData(new List<Snake.LineDrawData> { erase });
    }
}
