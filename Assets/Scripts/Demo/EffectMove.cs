using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 特效移动脚本
/// </summary>
public class EffectMove : MonoBehaviour
{
    public float moveSpeed = 100;//世界坐标和屏幕坐标是100:1
    float timeVal;
    int randomYPos;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-transform.right * Time.deltaTime* moveSpeed);
        timeVal += Time.deltaTime;
        if (timeVal >= 1)
        {
            timeVal = 0;
            randomYPos = Random.Range(-1, 2);
        }
        else
        {
            transform.Translate(transform.up * randomYPos * Time.deltaTime);
        }
    }
}
