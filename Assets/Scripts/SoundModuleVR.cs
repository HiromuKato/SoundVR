using UnityEngine;
using System.Collections;

/// <summary>
/// 視線がヒットしたオブジェクトに追加されているオーディオモジュール
/// (サンプルとしてAudioEchoFilter, AudioDistortionFilterのみ対応)を取得し、
/// 本スクリプトがアタッチされたオブジェクトにコピーします
/// </summary>
public class SoundModuleVR : MonoBehaviour
{
    // Echoモジュール
    public GameObject module0;

    // Distortionモジュール
    public GameObject module1;

    // Module0にレイがヒットしているかどうかのフラグ
    private bool ishitModule0 = false;

    // Module1にレイがヒットしているかどうかのフラグ
    private bool ishitModule1 = false;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ishitModule0 = false;
        ishitModule1 = false;

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
            if (hitObject == module0)
            {
                ishitModule0 = true;
            }
            else if (hitObject == module1)
            {
                ishitModule1 = true;
            }
        }
        ChangeModule();
    }

    /// <summary>
    /// ヒットしているオブジェクトのモジュール情報を音源にコピーします。
    /// ヒットしていない場合はモジュール情報を音源から削除します。
    /// </summary>
    void ChangeModule()
    {
        // 任意のモジュールに対応できるようにしたほうがよいですが、
        // Module0:Echo, Module1:Distortionの決め打ち実装になっています
        if (ishitModule0)
        {
            AudioEchoFilter fil_src = GameObject.Find("Module0").GetComponent<AudioEchoFilter>();
            if (fil_src)
            {
                if (gameObject.GetComponent<AudioEchoFilter>() == null)
                {
                    AudioEchoFilter fil_dst = gameObject.AddComponent<AudioEchoFilter>();
                    fil_dst.delay = fil_src.delay;
                    fil_dst.decayRatio = fil_src.decayRatio;
                    fil_dst.wetMix = fil_src.wetMix;
                    fil_dst.dryMix = fil_src.wetMix;
                    fil_src.gameObject.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
        else
        {
            GameObject obj_src = GameObject.Find("Module0");
            obj_src.GetComponent<Renderer>().material.color = Color.blue;

            AudioEchoFilter fil_dst = gameObject.GetComponent<AudioEchoFilter>();
            if (fil_dst != null)
            {
                Destroy(fil_dst);
            }
        }

        if (ishitModule1)
        {
            AudioDistortionFilter fil_src = GameObject.Find("Module1").GetComponent<AudioDistortionFilter>();
            if (fil_src)
            {
                if (gameObject.GetComponent<AudioDistortionFilter>() == null)
                {
                    AudioDistortionFilter fil_dst = gameObject.AddComponent<AudioDistortionFilter>();
                    fil_dst.distortionLevel = fil_src.distortionLevel;
                    fil_src.gameObject.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
        else
        {
            GameObject obj_dst = GameObject.Find("Module1");
            obj_dst.GetComponent<Renderer>().material.color = Color.blue;
            AudioDistortionFilter fil_dst = gameObject.GetComponent<AudioDistortionFilter>();
            if (fil_dst != null)
            {
                Destroy(fil_dst);
            }
        }
    }
}
