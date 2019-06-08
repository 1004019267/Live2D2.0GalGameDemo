using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 特效产生器
/// </summary>
public class EffectSpawn : MonoBehaviour
{
    public GameObject[] effectGos;
    public Transform canvans;
    // Start is called before the first frame update
    void Start()
    {
        //延迟调用 方法 延迟几秒调用 几秒重复一次
        InvokeRepeating("CreateEffectGo",0,2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateEffectGo()
    {
        int randomIndex = Random.Range(0, effectGos.Length);
        //随机旋转角度
        transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(0,45)));
        GameObject effectGo = Instantiate(effectGos[randomIndex],transform.position,transform.rotation);
        effectGo.transform.SetParent(canvans);
    }
}
