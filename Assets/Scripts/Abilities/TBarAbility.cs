using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBarAbility : BaseAbility
{
    float tLength = 0.15f;

    public TBarAbility(Snake snake) : base(snake) { }

    protected override void Perform()
    {
        var arenaWidth = Settings.Instance.ArenaWidth.Value;
        var tbar = new Snake.LineDrawData();
        var UVAnchorPos = (mySnake.Position - mySnake.Direction * mySnake.Thickness) / arenaWidth;
        tbar.UVPosA = UVAnchorPos + tLength / 2 * Vector2.Perpendicular(mySnake.Direction);
        tbar.UVPosB = UVAnchorPos + tLength / 2 * -Vector2.Perpendicular(mySnake.Direction);
        tbar.thickness = mySnake.Thickness;
        tbar.color = mySnake.Color;

        mySnake.InjectLineDrawData(new List<Snake.LineDrawData> { tbar });
    }
}
