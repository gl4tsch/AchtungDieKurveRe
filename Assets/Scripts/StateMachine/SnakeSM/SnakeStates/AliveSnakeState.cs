using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Snake
{
    private class AliveSnakeState : BaseState
    {
        int ArenaWidth => Settings.Instance.ArenaWidth.Value;
        int ArenaHeight => Settings.Instance.ArenaHeight.Value;

        // Gap
        Stack<LineDrawData> gapSegmentBuffer = new Stack<LineDrawData>();
        float distSinceLastGap = 0;

        public override void OnEnter()
        {
            gapSegmentBuffer.Clear();
            Context.injectionDrawBuffer.Clear();

            // Pos
            Context.Position = new Vector2(UnityEngine.Random.Range(0 + Context.Thickness * 2, ArenaWidth - Context.Thickness * 2), UnityEngine.Random.Range(0 + Context.Thickness * 2, ArenaHeight - Context.Thickness * 2));
            // don't spawn too close to another snake
            float tooClose = 0.2f * ArenaWidth;
            int maxTries = 10;
            int currentTry = 0;
            while (currentTry < maxTries)
            {
                float minDist = Mathf.Infinity;
                foreach (var snake in AliveSnakes)
                {
                    float dist = Vector2.Distance(Context.Position, snake.Position);
                    minDist = Mathf.Min(dist, minDist);
                }
                if (minDist > tooClose)
                {
                    break;
                }
                Context.Position = new Vector2(UnityEngine.Random.Range(0 + Context.Thickness * 2, ArenaWidth - Context.Thickness * 2), UnityEngine.Random.Range(0 + Context.Thickness * 2, ArenaHeight - Context.Thickness * 2));
                currentTry++;
            }

            // Dir
            //Direction = UnityEngine.Random.insideUnitCircle.normalized;
            Vector2 center = new Vector2(ArenaWidth / 2, ArenaHeight / 2);
            Context.Direction = (center - Context.Position).normalized;

            Context.Ability.SetUses(Context.Score.Place);

            Context.LeftAction.Enable();
            Context.RightAction.Enable();
            Context.FireAction.Enable();

            AliveSnakes.Add(Context);
            Debug.Log(Context.Color + " alive!");
        }

        public override void OnUpdate()
        {
            UpdatePosition();
            UpdateGap();
        }

        void UpdatePosition()
        {
            Context.prevPos = Context.Position;
            float degrees = Context.TurnRate * Context.turnSign * Time.deltaTime;
            Context.Direction = Quaternion.Euler(0, 0, degrees) * Context.Direction;
            Context.Position += Context.Direction * Context.Speed * Time.deltaTime;
        }

        void UpdateGap()
        {
            distSinceLastGap += Vector2.Distance(Context.prevPos, Context.Position);

            if (distSinceLastGap > Context.GapFrequency)
            {
                // add to gap buffer
                var arenaWidth = Settings.Instance.ArenaWidth.Value;

                var prevUVPos = Context.prevPos / arenaWidth; //(Position - Direction * Settings.Instance.SnakeGapWidth) / arenaWidth;
                var newUVPos = Context.Position / arenaWidth;

                var gapSegment = new Snake.LineDrawData();
                gapSegment.thickness = (Context.Thickness + 4f) / arenaWidth;
                gapSegment.color = new Vector4(0, 0, 0, 0);
                gapSegment.clipCircle = 1; // clip gap around start

                // check if data can be combined
                // TODO: unspaghetti
                if (gapSegmentBuffer.Count > 0)
                {
                    var lastSegment = gapSegmentBuffer.Peek();

                    if (Vector2.Angle(lastSegment.UVPosB - lastSegment.UVPosA, newUVPos - prevUVPos) < 0.0001)
                    {
                        gapSegmentBuffer.Pop();

                        gapSegment.UVPosA = lastSegment.UVPosA;
                        gapSegment.UVPosB = newUVPos;
                    }
                    else
                    {
                        gapSegment.UVPosA = prevUVPos;
                        gapSegment.UVPosB = newUVPos;
                    }
                }
                else
                {
                    // first gap segment
                    gapSegment.UVPosA = prevUVPos;
                    gapSegment.UVPosB = newUVPos;
                }

                gapSegmentBuffer.Push(gapSegment);
            }

            // Gap end
            if (distSinceLastGap > Context.GapFrequency + Context.GapWidth)
            {
                // clip end of gap
                var lastSegment = gapSegmentBuffer.Pop();
                lastSegment.clipCircle = lastSegment.clipCircle == 1 ? 3 : 2;
                gapSegmentBuffer.Push(lastSegment);

                List<LineDrawData> gapData = new List<LineDrawData>(gapSegmentBuffer);
                Context.InjectLineDrawData(gapData);

                gapSegmentBuffer.Clear();
                distSinceLastGap -= Context.GapFrequency + Context.GapWidth;
            }
        }

        public override void OnExit()
        {
            Context.Score.IncreaseScore(AllSnakes.Count - AliveSnakes.Count);
            AliveSnakes.Remove(Context);
            Debug.Log(Context.Color + " ded!");
        }
    }
}