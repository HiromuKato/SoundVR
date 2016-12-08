using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// 視線シンセ
/// 見る位置によって動的に生成する音（サイン波）の周波数が変わります
/// </summary>
public class SisenSynth : MonoBehaviour
{
    // 視線ヒットの対象となるGameObject
    public GameObject plane;

    // 周波数を表示するテキスト
    public Text infoText;

    // -1 < sin < 1 なので音量をgain倍する
    public double gain = 0.05;

    // 周波数
    private double frequency = 440;

    // 1サンプルにおける変化量
    private double unit;

    // 位相
    private double phase = 0.0;

    // サンプリングレート
    const double sample_rate = 48000;

    // 円周率
    const double PI = System.Math.PI;

    // ステート管理用
    private enum PlayState
    {
        Stop,
        SineWave,
    }
    private PlayState playState = PlayState.Stop;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        playState = PlayState.Stop;

        Transform camera = Camera.main.transform;
        Ray ray;
        RaycastHit[] hits;
        GameObject hitObject;
        ray = new Ray(camera.position, camera.rotation * Vector3.forward);
        hits = Physics.RaycastAll(ray);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            hitObject = hit.collider.gameObject;
            if (hitObject == plane)
            {
                //Debug.Log("HIT_X:" + hit.point.x); // from -5 to 5
                float pos = hit.point.x + 5;         // from 0 to 10
                frequency = 200 * pos;               // from 0 to 2000
                infoText.text = frequency.ToString("F1") + " Hz";

                playState = PlayState.SineWave;
            }
        }
        Debug.DrawRay(camera.position, camera.rotation * Vector3.forward * 100.0f);
    }

    /// <summary>
    /// サイン波を生成します
    /// </summary>
    /// <param name="data">音声データ</param>
    /// <param name="channels">チャンネル</param>
    void SineWave(float[] data, int channels)
    {
        unit = frequency * 2 * PI / sample_rate;
        for (var i = 0; i < data.Length; i = i + channels)
        {
            phase += unit;

            data[i] = (float)(gain * Math.Sin(phase));
            if (channels == 2)
                data[i + 1] = data[i];
            if (phase > 2 * Math.PI)
                phase = 0;
        }
    }

    /// <summary>
    /// 音声データにフィルターをかけます
    /// </summary>
    /// <param name="data">音声データ</param>
    /// <param name="channels">チャンネル</param>
    void OnAudioFilterRead(float[] data, int channels)
    {
        switch (playState)
        {
            case PlayState.SineWave:
                SineWave(data, channels);
                break;
        }
    }
}