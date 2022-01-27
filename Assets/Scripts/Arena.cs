using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arena : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] int pixelWidth, pixelHeight;
    [SerializeField] int snakeThickness;

    public event System.Action<Snake, Snake> CollisionEvent;

    Texture2D tex;
    Color32[] pixels;
    List<Snake> snakes = new List<Snake>();

    private IEnumerator Start()
    {
        tex = new Texture2D(pixelWidth, pixelHeight);
        pixels = new Color32[pixelWidth * pixelHeight];
        ClearTexture();
        image.texture = tex;

        yield return new WaitForSeconds(1);

        snakes = new List<Snake>
        {
            new Snake(Color.red),
            new Snake(Color.green),
            new Snake(Color.blue)
        };

        RandomStartPositions();
    }

    private void Update()
    {
        foreach(var snake in snakes)
        {
            var lastPos = snake.Position;
            snake.UpdatePosition(-1);
            var newPos = snake.Position;
            DrawLine(lastPos, newPos, snake.Thickness, snake.Color);
        }

        ApplyPixels();
    }

    void RandomStartPositions()
    {
        foreach(var snake in snakes)
        {
            snake.Position = new Vector2(Random.Range(0, pixelWidth), Random.Range(0, pixelHeight));
            snake.Direction = Random.insideUnitCircle.normalized;
        }
    }

    void DrawLine(Vector2 start, Vector3 end, float thickness, Color32 color)
    {
        int minX = (int)Mathf.Max(0, (Mathf.Min(start.x, end.x) - thickness));
        int maxX = (int)Mathf.Min(pixelWidth, (Mathf.Max(start.x, end.x) + thickness));
        int minY = (int)Mathf.Max(0, (Mathf.Min(start.y, end.y) - thickness));
        int maxY = (int)Mathf.Min(pixelHeight, (Mathf.Max(start.y, end.y) + thickness));

        for (int x = minX; x < maxX; x++)
        {
            for(int y = minY; y < maxY; y++)
            {
                if(DistToLine(new Vector2(x, y), start, end) <= thickness)
                {
                    int idx = CoordinatesToIndex(x, y);
                    if (pixels[idx].a > 0)
                    {
                        //// collision
                        //Debug.Log(color + " collided with " + pixels[idx]);
                        //Snake snakeA = GetSnakeByColor(color);
                        //Snake snakeB = GetSnakeByColor(pixels[idx]);
                        //CollisionEvent?.Invoke(snakeA, snakeB);
                        //snakes.Remove(snakeA);
                    }
                    else
                    {
                        pixels[idx] = color;
                    }
                }
            }
        }
    }

    float DistToLine(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 pa = point - a, ba = b - a;
        float h = Mathf.Max(0, Mathf.Min(1, Vector2.Dot(pa, ba) / Vector2.Dot(ba, ba)));
        Vector2 d = pa - ba * h;
        return Vector2.Dot(d, d);
    }

    Vector2Int IndexToCoordinates(int index)
    {
        return new Vector2Int(index % pixelHeight, index / pixelWidth);
    }

    int CoordinatesToIndex(int x, int y)
    {
        return x + pixelWidth * y;
    }

    Snake GetSnakeByColor(Color32 col)
    {
        return snakes.Find(s => s.Color.Equals(col));
    }

    void ApplyPixels()
    {
        tex.SetPixels32(pixels);
        tex.Apply();
    }

    void ClearTexture()
    {
        for(int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color32(0, 0, 0, 0);
        }
        tex.SetPixels32(pixels);
        tex.Apply();
    }
}
