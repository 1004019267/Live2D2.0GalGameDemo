using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// 开始游戏的按钮
/// </summary>
public class LoadGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(LoadGameScence);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadGameScence()
    {
        SceneManager.LoadScene(1);
    }
}
