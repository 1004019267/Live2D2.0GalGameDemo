using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;
public class Live2DSimpleModol : MonoBehaviour
{
    public TextAsset modelFile;
    public Texture2D texture;
    public TextAsset idleMotionFile;
    public GameObject girl;

    Live2DModelUnity live2DModel;
    Matrix4x4 live2DCanvansPos;

    Live2DMotion live2DmotionIdle;
    MotionQueueManager motionQueueManager = new MotionQueueManager();

    EyeBlinkMotion eyeBlinkMotion = new EyeBlinkMotion();

    public float moveSpeed;

    Vector3 initPos;

    int hitCount;
    //判断Boss是否被打败
    public bool isDefeat;
    // Start is called before the first frame update
    void Start()
    {
        Live2D.init();
        live2DModel = Live2DModelUnity.loadModel(modelFile.bytes);
        live2DModel.setTexture(0, texture);
        float modelWidth = live2DModel.getCanvasWidth();
        live2DCanvansPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, -50, 50);

        live2DmotionIdle = Live2DMotion.loadMotion(idleMotionFile.bytes);

        live2DmotionIdle.setLoop(true);

        motionQueueManager.startMotion(live2DmotionIdle);
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        live2DModel.setMatrix(transform.localToWorldMatrix * live2DCanvansPos);
        motionQueueManager.updateParam(live2DModel);
        eyeBlinkMotion.setParam(live2DModel);

        live2DModel.update();
        if (GameManager.Instance.gameOver)
        {
            return;
        }
        //判断Boss是否追上girl
        if ((girl.transform.position.x - transform.position.x) < 3)
        {
            GameManager.Instance.gameOver = true;
        }

        if (isDefeat)
        {
            transform.position = Vector3.Lerp(transform.position, initPos, 0.2f);
        }
        else
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }

    private void OnRenderObject()
    {
        live2DModel.draw();
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.gameOver)
        {
            return;
        }
        if (hitCount >= 20)
        {
            isDefeat = true;
            GameManager.Instance.DefeatBadBoy();
        }
        else
        {
            hitCount++;
        }
    }
}
