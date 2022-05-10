using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAbility : BaseAbility
{
    float jumpLength = 0.06f;
    int jumpPixelLength => (int)(jumpLength * Settings.Instance.ArenaWidth.Value);

    public JumpAbility(Snake snake) : base(snake)
    {
    }

    protected override void Perform()
    {
        var prevPos = mySnake.Position;
        var newPos = prevPos + mySnake.Direction * jumpPixelLength;
        mySnake.Teleport(newPos);

        var arenaWidth = Settings.Instance.ArenaWidth.Value;
        var noCollisionFill = new Snake.LineDrawData();
        noCollisionFill.UVPosA = prevPos / arenaWidth;
        noCollisionFill.UVPosB = newPos / arenaWidth;
        noCollisionFill.thickness = mySnake.Thickness / arenaWidth;
        noCollisionFill.color = mySnake.Color;
        noCollisionFill.clipCircle = 1;
        mySnake.InjectLineDrawData(new List<Snake.LineDrawData> { noCollisionFill });
    }
}
