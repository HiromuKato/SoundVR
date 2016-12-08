using UnityEngine;
using System.Collections;

/// <summary>
/// 空間ボイスメモ
/// 空間内に自由にボイスメモを貼り付けます。
/// 左クリックしているあいだ録音します。(録音オブジェクトはクリックを開始した位置に生成されます)
/// 録音オブジェクトに視線が合うと音声を再生します。
/// </summary>
public class VoiceMemo : MonoBehaviour
{
    // マイクが接続されているかどうかのフラグ
    private bool micConnected = false;

    // 録音時の最小周波数
    private int minFreq;

    // 録音時の最大周波数
    private int maxFreq;

    // 録音オブジェクトのひな型
    public GameObject block;

    // 録音可能数
    const int kMaxObjNum = 5;

    // AudioSourceの配列
    private AudioSource[] audio_src = new AudioSource[kMaxObjNum];

    // 録音オブジェクトの配列
    private GameObject[] voice_cube = new GameObject[kMaxObjNum];

    // 何番目の録音かを保持する変数（from 0 to mkMaxObjNum - 1）
    private int voice_count = 0;

    //Use this for initialization  
    void Start()
    {
        // 録音オブジェクトを予め生成しておく
        for (int i = 0; i < kMaxObjNum; ++i)
        {
            voice_cube[i] = (GameObject)Instantiate(block, new Vector3(0, 0, 0), Quaternion.identity);
            audio_src[i] = voice_cube[i].GetComponent<AudioSource>();
        }

        // マイクが接続されているか確認
        if (Microphone.devices.Length <= 0)
        {
            Debug.LogWarning("Microphone not connected!");
        }
        else
        {
            // マイクが接続されているフラグを設定
            micConnected = true;

            // デフォルトマイクの周波数の範囲を取得する
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            //minFreq や maxFreq 引数で 0 の値が返されるとデバイスが任意の周波数をサポートすることを示す
            if (minFreq == 0 && maxFreq == 0)
            {
                // 録音サンプリングレートを48000 Hzに設定する
                maxFreq = 48000;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!micConnected)
        {
            // マイクが接続されていない場合はなにもしない
            return;
        }

        // マウスの左クリックを開始したとき
        if (Input.GetMouseButtonDown(0))
        {
            // 録音中でなければ処理を行う
            if (!Microphone.IsRecording(null))
            {
                //録音開始
                audio_src[voice_count].clip = Microphone.Start(null, true, 3, maxFreq);

                voice_cube[voice_count].GetComponent<AudioSource>().clip = audio_src[voice_count].clip;

                // カメラから距離10の位置に録音オブジェクト表示
                Transform camera = Camera.main.transform;
                Ray ray = new Ray(camera.position, camera.rotation * Vector3.forward);
                voice_cube[voice_count].transform.position = ray.GetPoint(10);

                // 録音中は赤くする
                voice_cube[voice_count].GetComponent<Renderer>().material.color = Color.red;
            }
        }
        // マウスの左クリックを終了したとき
        else if (Input.GetMouseButtonUp(0))
        {
            if (Microphone.IsRecording(null))
            {
                //録音中であれば録音停止
                Microphone.End(null);

                // カラーを白に戻す 
                voice_cube[voice_count].GetComponent<Renderer>().material.color = Color.white;

                // 録音オブジェクトのカウントをインクリメントする
                voice_count++;
                if (voice_count >= kMaxObjNum)
                {
                    voice_count = 0;
                }
            }
        }

        // 録音中でない場合、録音オブジェクトに視線があったら音声を再生する
        if (!Microphone.IsRecording(null))
        {
            Transform camera = Camera.main.transform;
            Ray ray;
            RaycastHit[] hits;
            GameObject hitObject;
            Debug.DrawRay(camera.position, camera.rotation * Vector3.forward * 100.0f);
            ray = new Ray(camera.position, camera.rotation * Vector3.forward);
            hits = Physics.RaycastAll(ray);

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                hitObject = hit.collider.gameObject;
                AudioSource source = hitObject.GetComponent<AudioSource>();
                if (source)
                {
                    if (!source.isPlaying)
                    {
                        source.Play();
                    }
                }
            }

            for (int i = 0; i < kMaxObjNum; ++i)
            {
                if (voice_cube[i].GetComponent<AudioSource>().isPlaying)
                {
                    voice_cube[i].GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    voice_cube[i].GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }
    }
}
