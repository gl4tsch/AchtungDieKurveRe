using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaGPU : MonoBehaviour
{
    [SerializeField] ComputeShader cs;
    [SerializeField] RawImage image;

    int pixelWidth => Settings.Instance.ArenaWidth;
    int pixelHeight => Settings.Instance.ArenaHeight;

    public event System.Action<Snake, Snake> CollisionEvent;

    RenderTexture renderTex;
    ComputeBuffer snakeBuffer;

    private void OnEnable()
    {
        snakeBuffer = new ComputeBuffer(Snake.Snakes.Count, sizeof(float) * 13);
    }

    private void OnDisable()
    {
        snakeBuffer.Release();
        snakeBuffer = null;
    }

    private IEnumerator Start()
    {
        PrepareRenderTex();

        yield return new WaitForSeconds(1);

        RandomStartPositions();
    }

    private void PrepareRenderTex()
    {
        // create render texture
        if (renderTex != null) renderTex.Release();
        renderTex = new RenderTexture(pixelWidth, pixelHeight, 0);
        renderTex.enableRandomWrite = true;
        //renderTex.format = RenderTextureFormat.ARGB32;
        renderTex.Create();
        image.texture = renderTex;

        // prepare compute shader
        cs.SetTexture(0, "Arena", renderTex);
        cs.SetTexture(1, "Arena", renderTex);
        cs.SetInt("_Width", pixelWidth);
        cs.SetInt("_Height", pixelHeight);
        cs.SetInt("_SnakeCount", Snake.Snakes.Count);

        // fill arena border
        cs.Dispatch(1, pixelWidth / 8, pixelHeight / 8, 1);
    }

    private void Update()
    {
        if (Snake.Snakes.Count == 0) return;

        Snake.SnakeDrawData[] snakesData = new Snake.SnakeDrawData[Snake.Snakes.Count];

        for (int i = 0; i < Snake.Snakes.Count; i++)
        {
            var snake = Snake.Snakes[i];
            snake.UpdatePosition();
            var data = snake.GetDrawData();
            snakesData[i] = data;
        }

        DrawSnakes(snakesData);
        ReadCollisions(ref snakesData);
    }

    void DrawSnakes(Snake.SnakeDrawData[] data)
    {
        // TODO: try to dispatch separately for every segment
        snakeBuffer.SetData(data);
        cs.SetBuffer(0, "_Snakes", snakeBuffer);
        cs.Dispatch(0, pixelWidth / 8, pixelHeight / 8, 1);
    }

    void ReadCollisions(ref Snake.SnakeDrawData[] data)
    {
        snakeBuffer.GetData(data);
        foreach (var snake in data)
        {
            if (snake.collision.w > 0)
            {
                Debug.Log(snake.color + " collided with " + snake.collision);
                Snake.Snakes.Remove(GetSnakeByColor((Color)snake.color));
            }
        }
    }

    void RandomStartPositions()
    {
        foreach (var snake in Snake.Snakes)
        {
            int travelInASec = (int)Settings.Instance.SnakeSpeed;
            snake.Spawn(pixelWidth, pixelHeight, travelInASec);
        }
    }

    Snake GetSnakeByColor(Color col)
    {
        return Snake.Snakes.Find(s => s.Color.Equals(col));
    }
}
