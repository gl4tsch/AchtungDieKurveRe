using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Snake;

public class ArenaGPU : MonoBehaviour
{
    [SerializeField] ComputeShader cs;
    [SerializeField] RawImage image;
    [SerializeField] int pixelWidth, pixelHeight;

    public event System.Action<Snake, Snake> CollisionEvent;

    RenderTexture renderTex;
    List<Snake> snakes = new List<Snake>();
    ComputeBuffer snakeBuffer;

    private void OnEnable()
    {
        snakeBuffer = new ComputeBuffer(3, sizeof(float) * 9);
    }

    private void OnDisable()
    {
        snakeBuffer.Release();
        snakeBuffer = null;
    }

    private void Start()
    {
        renderTex = new RenderTexture(pixelWidth, pixelHeight, 0);
        renderTex.enableRandomWrite = true;
        renderTex.Create();
        cs.SetTexture(0, "Arena", renderTex);
        cs.SetInt("_Width", pixelWidth);
        cs.SetInt("_Height", pixelHeight);
        cs.SetInt("_SnakeCount", 3);
        image.texture = renderTex;

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
        if (snakes.Count == 0) return;

        SnakeData[] snakesData = new SnakeData[snakes.Count];

        for (int i = 0; i < snakes.Count; i++)
        {
            var snake = snakes[i];
            var prevPos = snake.Position / pixelWidth;
            snake.UpdatePosition(-1);
            var newPos = snake.Position / pixelWidth;
            snakesData[i] = new SnakeData();
            snakesData[i].prevPos = prevPos;
            snakesData[i].newPos = newPos;
            snakesData[i].thickness = snake.Thickness / pixelWidth;
            snakesData[i].color = new Vector4(snake.Color.r, snake.Color.g, snake.Color.b, snake.Color.a);
        }

        DrawSnakes(snakesData);
    }

    void DrawSnakes(SnakeData[] data)
    {
        snakeBuffer.SetData(data);
        cs.SetBuffer(0, "_Snakes", snakeBuffer);
        cs.Dispatch(0, pixelWidth / 8, pixelHeight / 8, 1);
    }

    void RandomStartPositions()
    {
        foreach (var snake in snakes)
        {
            snake.Position = new Vector2(Random.Range(0, pixelWidth), Random.Range(0, pixelHeight));
            snake.Direction = Random.insideUnitCircle.normalized;
        }
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

    void ClearTexture()
    {
        // TODO
    }
}
