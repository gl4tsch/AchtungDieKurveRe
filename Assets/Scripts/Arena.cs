using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arena : MonoBehaviour
{
    [SerializeField] ComputeShader cs;
    [SerializeField] RawImage image;

    int pixelWidth => Settings.Instance.ArenaWidth.Value;
    int pixelHeight => Settings.Instance.ArenaHeight.Value;

    RenderTexture renderTex;
    ComputeBuffer snakeBuffer;
    ComputeBuffer lineBuffer;

    bool gameRunning = false;

    Queue<Snake.LineDrawData> lineDrawDataBuffer = new Queue<Snake.LineDrawData>();
    int maxAdditionalLinesDrawnEachFramePerSnake = 3;

    private void OnEnable()
    {
        snakeBuffer = new ComputeBuffer(Snake.AllSnakes.Count, sizeof(float) * 9 + sizeof(int));
        lineBuffer = new ComputeBuffer(Snake.AllSnakes.Count * maxAdditionalLinesDrawnEachFramePerSnake, sizeof(float) * 9 + sizeof(int));
    }

    private void OnDisable()
    {
        snakeBuffer.Release();
        snakeBuffer = null;
        lineBuffer.Release();
        lineBuffer = null;
    }

    public void ResetArena()
    {
        PrepareRenderTex();
        foreach(var snake in Snake.AllSnakes)
        {
            snake.Reset();
        }
        StartRound();
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
        cs.SetInt("_SnakeCount", Snake.AllSnakes.Count);

        ClearArenaTex();
    }

    private void ClearArenaTex()
    {
        // fill arena border
        cs.Dispatch(1, pixelWidth / 8, pixelHeight / 8, 1);
    }

    private void Update()
    {
        if (!gameRunning) return;

        if (Snake.AliveSnakes.Count <= 1)
        {
            EndRound();
            StartCoroutine(StartRoundAfterInput());
            return;
        }

        List<Snake.SnakeDrawData> snakesDrawData = new List<Snake.SnakeDrawData>();
        List<Snake.LineDrawData> lineDrawData = new List<Snake.LineDrawData>();

        foreach (var snake in Snake.AliveSnakes)
        {
            snake.Update();
            var snakeData = snake.GetSnakeDrawData();
            snakesDrawData.Add(snakeData);

            // fill line buffer
            var lineData = snake.GetLineDrawData();
            foreach(var ld in lineData)
            {
                lineDrawDataBuffer.Enqueue(ld);
            }

            // drain line buffer
            for (int i = 0; i < maxAdditionalLinesDrawnEachFramePerSnake; i++)
            {
                if (lineDrawDataBuffer.Count <= 0) break;

                lineDrawData.Add(lineDrawDataBuffer.Dequeue());
            }
        }

        var snakesDrawDataArray = snakesDrawData.ToArray();
        var lineDrawDataArray = lineDrawData.ToArray();

        Draw(snakesDrawDataArray, lineDrawDataArray); // dispatch compute shader

        ProcessCollisions(ref snakesDrawDataArray);
    }

    void Draw(Snake.SnakeDrawData[] snakeData, Snake.LineDrawData[] lineData)
    {
        snakeBuffer.SetData(snakeData);
        lineBuffer.SetData(lineData);

        cs.SetBuffer(0, "_Snakes", snakeBuffer);
        cs.SetBuffer(0, "_Lines", lineBuffer);

        cs.SetInt("_LineCount", lineData.Length);

        cs.Dispatch(0, pixelWidth / 8, pixelHeight / 8, 1);
    }

    void ProcessCollisions(ref Snake.SnakeDrawData[] data)
    {
        snakeBuffer.GetData(data);
        foreach (var snakeData in data)
        {
            if(snakeData.collision == 1)
            {
                var snake = GetSnakeByColor(snakeData.color);
                snake.Kill();
            }
        }
    }

    IEnumerator StartRoundAfterInput()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        StartRound();
    }

    void StartRound()
    {
        ClearArenaTex();
        lineDrawDataBuffer.Clear();

        foreach (var snake in Snake.AllSnakes)
        {
            snake.Spawn();
        }

        gameRunning = true;
    }

    public void EndRound()
    {
        gameRunning = false;

        for(int i = Snake.AliveSnakes.Count - 1; i >= 0; i--)
        {
            Snake.AliveSnakes[i].Kill();
        }
    }

    Snake GetSnakeByColor(Color col)
    {
        return Snake.AllSnakes.Find(s => s.Color.Equals(col));
    }
}
