using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 空間イコライザー
/// 視線がヒットしたオブジェクトに応じて聞いている音楽の聞こえる周波数が変わります
/// </summary>
[RequireComponent (typeof (AudioSource))]
public class SpacialEqualizer : MonoBehaviour
{
    // Cubeオブジェクトの配列
    public GameObject[] cube;

    // フィルタの数値を表示するテキスト
    public Text infoText;

    // オブジェクトの数（本サンプルでは８バンドに分割している）
    private const int kMaxCubeNum = 8;
    
    // CubeオブジェクトにAddされているオーディオソースの配列
    private AudioSource[] audio_src;

    // Use this for initialization
    void Start()
    {
        audio_src = new AudioSource[kMaxCubeNum];
        for (int i = 0; i < kMaxCubeNum; ++i)
        {
            audio_src[i] = cube[i].GetComponent<AudioSource>();
            audio_src[i].volume = 0.01f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < kMaxCubeNum; i++)
        {
            if(cube[i].gameObject.GetComponent<Renderer>().material.color != Color.white)
            {
                cube[i].gameObject.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        Transform camera = Camera.main.transform;
        Ray ray;
        RaycastHit[] hits;
        GameObject hitObject;
        ray = new Ray(camera.position, camera.rotation * Vector3.forward);
        hits = Physics.RaycastAll(ray);

        Debug.DrawRay(camera.position, camera.rotation * Vector3.forward * 100.0f);

        if (hits.Length == 0)
        {
            infoText.text = "";

            // ヒットしているオブジェクトがない場合はすべての音量を小さくする
            for (int i = 0; i < kMaxCubeNum; ++i)
            {
                audio_src[i].volume = 0.01f;
            }
            return;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            hitObject = hit.collider.gameObject;

            for (int j = 0; j < kMaxCubeNum; j++)
            {
                audio_src[j].volume = 0.0f;
                if (hitObject == cube[j])
                {
                    // ヒットしたオブジェクトのカラーと音量を変える
                    cube[j].gameObject.GetComponent<Renderer>().material.color = Color.green;
                    audio_src[j].volume = 1.0f;

                    // フィルターの範囲表示
                    AudioLowPassFilter lpf = cube[j].GetComponent<AudioLowPassFilter>();
                    AudioHighPassFilter hpf = cube[j].GetComponent<AudioHighPassFilter>();
                    infoText.text = "HPF:" + hpf.cutoffFrequency.ToString("F0") + "\nLPF:" + lpf.cutoffFrequency.ToString("F0");
                }
            }
        }
    }

}
