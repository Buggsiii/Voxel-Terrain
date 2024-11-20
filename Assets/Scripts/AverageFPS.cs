using System;
using UnityEngine;

public class AverageFPS : MonoBehaviour
{
    private readonly float[] _fpsBuffer = new float[120];
    private float _averageFPS = 0;
    private bool _isStarted = false;
    private bool _isBufferFull = false;

    private void Update()
    {
        if (!_isStarted)
            return;

        if (_isBufferFull)
        {
            _isStarted = false;
            LogAverageFPS();
            return;
        }

        float fps = 1f / Time.deltaTime;
        AddFPSToBuffer(fps);

        float sum = 0;
        int i;
        for (i = 0; i < _fpsBuffer.Length; i++)
        {
            if (_fpsBuffer[i] == 0)
                break;
            sum += _fpsBuffer[i];
        }

        _averageFPS = sum / i;

        if (!IsBufferFull())
            return;

        _isBufferFull = true;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"Average FPS: {_averageFPS}");

        if (GUI.Button(new Rect(10, 40, 200, 40), "Log Average FPS"))
            StartLoggingFPS();
    }

    private void AddFPSToBuffer(float fps)
    {
        for (int i = _fpsBuffer.Length - 1; i > 0; i--)
            _fpsBuffer[i] = _fpsBuffer[i - 1];

        _fpsBuffer[0] = fps;
    }

    public void StartLoggingFPS()
    {
        _isStarted = true;
        _isBufferFull = false;
        Array.Fill(_fpsBuffer, 0);
    }

    private bool IsBufferFull() => Array.TrueForAll(_fpsBuffer, fps => fps != 0);

    private void LogAverageFPS() => Debug.Log($"Average FPS: {_averageFPS}");
}
