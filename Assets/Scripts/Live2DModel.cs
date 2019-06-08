using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;
using live2d.framework;
public class Live2DModel : MonoBehaviour
{
    //2进制json缓存
    public TextAsset modleFile;
    //加载的缓存
    Live2DModelUnity live2DModel;
    //加载贴图缓存
    public Texture2D[] tex;
    //动作对象管理
    public TextAsset[] motionFiles;
    //1,2 Idel 3.左摆头 4.右摆头微笑 5前弯腰较生气 6.不开心闭眼 7.8不开心睁眼
    //9放点 10卖萌 11哭 12掐腰生气 13恶心不舒服 14摇来摇去

    //加入了动画优先级设置
    //动作未进行的状态 优先级为0
    ///待机动作发生时 优先级为1
    //其他动作进行时 优先级为2
    //无视优先级 强制发生的动作 优先级为3
    L2DMotionManager l2DMotionManager = new L2DMotionManager();

    Live2DMotion[] motions;
    //4X4矩阵 live2D画布
    Matrix4x4 live2DCanvasPos;
    //动作管理队列
    MotionQueueManager motionQueueManager = new MotionQueueManager();
    //同时做第二种动作管理
    MotionQueueManager motionQueueManagerA = new MotionQueueManager();
    //层级
    public int motionIndex;
    public float weight;

    //自动眨眼
    EyeBlinkMotion eyeBlinkMotion = new EyeBlinkMotion();
    //鼠标拖拽引起动作变化
    L2DTargetPoint drag = new L2DTargetPoint();

    //套用物理运算的设定
    PhysicsHair physicsHairSideLeft=new PhysicsHair();//两边头发
    PhysicsHair physicsHairSideRight = new PhysicsHair();
    PhysicsHair physicsHairBackLeft = new PhysicsHair();//后方头发
    PhysicsHair physicsHairBackRight = new PhysicsHair();

    //表情 一直切换里面有一个层级会一直加 到后面就表情不会变化了
    public TextAsset[] expressionFiles;
    L2DExpressionMotion[] expressions;
    MotionQueueManager expressionMotionQueueManager=new MotionQueueManager();
    // Start is called before the first frame update
    void Start()
    {
        Live2D.init();//Live2D的初始化

        //Live2D.dispose();//Live2D的释放

        //读取模型

        //moc读取
        //Application.dataPath 就是Assets文件夹下目录
        //Live2DModelUnity.loadModel(Application.dataPath+ "/Resources/Epsilon/runtime/Epsilon.moc");

        //json读取 和之前替换模型一样操作 moc复制出来一个后缀加.bytes
        //modleFile = Resources.Load<TextAsset>("Epsilon/runtime/Epsilon.moc");
        live2DModel = Live2DModelUnity.loadModel(modleFile.bytes);

        //与贴图建立关联

        //加载所有贴图
        //tex = Resources.LoadAll<Texture2D>("Epsilon/runtime/Epsilon.1024");
        for (int i = 0; i < tex.Length; i++)
        {
            live2DModel.setTexture(i, tex[i]);
        }

        //制定显示位置和尺寸 参数分别为左右下上 近视口距离(离相机近的距离)50 远视口距离(离相机远的距离)-50 （官方建议参数）
        //正交矩阵与相关Api设置显示图像
        float modelWidth = live2DModel.getCanvasWidth();
        float modelHeight = live2DModel.getCanvasHeight();

        live2DCanvasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, 50, -50);

        //播放动作
        //实例化动作对象
        //live2DMotionIdle = Live2DMotion.loadMotion(Application.dataPath+"");
        //TextAsset mtnFile = Resources.Load<TextAsset>();
        //live2DMotionIdle = Live2DMotion.loadMotion(mtnFile.bytes);

        motions = new Live2DMotion[motionFiles.Length];
        for (int i = 0; i < motions.Length; i++)
        {
            motions[i] = Live2DMotion.loadMotion(motionFiles[i].bytes);
        }

        //设置某一个动画的一些属性
        //重复播放是否淡入 不带很僵硬
        motions[0].setLoopFadeIn(false);
        //设置淡入淡出时间 参数毫秒
        motions[0].setFadeOut(1000);
        motions[0].setFadeIn(1000);

        ////动画是否循环
        //motions[0].setLoop(true);

        //motions[5].setLoop(true);
        ////第二个参数播放完是否删除
        //motionQueueManager.startMotion(motions[0]);
        //motionQueueManagerA.startMotion(motions[5]);

        
        #region  左右两侧头发的摇摆
        //头发摇摆左
        //套用物理运算 长度影响摇摆周期(短慢长快) 
        //空气阻力0-1也影响速度 默认0.5
        //质量
        physicsHairSideLeft.setup(0.2f,0.5f,0.14f);

        //设置输入参数
        //设置哪一个部分变动时进行哪一种物理运算
        PhysicsHair.Src srcXLeft = PhysicsHair.Src.SRC_TO_X;//横向摇摆
        //第三个参数 头发影响度 变动时头发受到0.005影响速度 json表推荐 第四个权重
        physicsHairSideLeft.addSrcParam(srcXLeft, "PARAM_ANGLE_X",0.005f,1);
        //设置输出表现
        PhysicsHair.Target targetLeft = PhysicsHair.Target.TARGET_FROM_ANGLE;//根据角度表现`angleV会很快到极限值然后停留一段

        physicsHairSideLeft.addTargetParam(targetLeft, "PARAM_HAIR_SIDE_L", 0.005f, 1);

        //头发摇摆右
        //套用物理运算 长度影响摇摆周期(短慢长快) 
        //空气阻力0-1也影响速度 默认0.5
        //质量
        physicsHairSideRight.setup(0.2f, 0.5f, 0.14f);

        //设置输入参数
        //设置哪一个部分变动时进行哪一种物理运算
        PhysicsHair.Src srcXRight = PhysicsHair.Src.SRC_TO_X;//横向摇摆
        //第三个参数 头发影响度 变动时头发受到0.005影响速度 json表推荐 第四个权重
        physicsHairSideRight.addSrcParam(srcXRight, "PARAM_ANGLE_X", 0.005f, 1);
        //设置输出表现
        PhysicsHair.Target targetRight = PhysicsHair.Target.TARGET_FROM_ANGLE;//根据角度表现`

        physicsHairSideRight.addTargetParam(targetRight, "PARAM_HAIR_SIDE_R", 0.005f, 1);
        #endregion

        #region 前后头发摇摆
        //左边
        physicsHairBackLeft.setup(0.24f, 0.5f, 0.18f);
        PhysicsHair.Src srcXBackLeft = PhysicsHair.Src.SRC_TO_X;
        PhysicsHair.Src srcZBackLeft = PhysicsHair.Src.SRC_TO_G_ANGLE;
        physicsHairBackLeft.addSrcParam(srcXBackLeft, "PARAM_ANGLE_X",0.005f,1);
        physicsHairBackLeft.addSrcParam(srcZBackLeft, "PARAM_ANGLE_Z", 0.8f, 1);

        PhysicsHair.Target targetBackLeft = PhysicsHair.Target.TARGET_FROM_ANGLE;
        physicsHairBackLeft.addTargetParam(targetBackLeft,"PARAM_HAIR_BACK_L",0.005f,1);

        //右边
        physicsHairBackRight.setup(0.24f, 0.5f, 0.18f);
        PhysicsHair.Src srcXBackRight = PhysicsHair.Src.SRC_TO_X;
        PhysicsHair.Src srcZBackRight = PhysicsHair.Src.SRC_TO_G_ANGLE;
        physicsHairBackRight.addSrcParam(srcXBackRight, "PARAM_ANGLE_X", 0.005f, 1);
        physicsHairBackRight.addSrcParam(srcZBackRight, "PARAM_ANGLE_Z", 0.8f, 1);

        PhysicsHair.Target targetBackRight = PhysicsHair.Target.TARGET_FROM_ANGLE;
        physicsHairBackRight.addTargetParam(targetBackRight, "PARAM_HAIR_BACK_R", 0.005f, 1);
        #endregion

        ////加载表情数据
        expressions = new L2DExpressionMotion[expressionFiles.Length];
        for (int i = 0; i < expressions.Length; i++)
        {
            expressions[i] = L2DExpressionMotion.loadJson(expressionFiles[i].bytes);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //设置矩阵 当前坐标转换为世界坐标 矩阵相乘
        live2DModel.setMatrix(transform.localToWorldMatrix * live2DCanvasPos);
        //启动api自带的update
        live2DModel.update();
        ////启动动画管理 告诉他启动哪一个角色的动画
        //motionQueueManager.updateParam(live2DModel);
        ////多个动作同时播放会卡一下最好不要设置相同参数 一般模型师都K好整个动作
        //motionQueueManagerA.updateParam(live2DModel);

        expressionMotionQueueManager.updateParam(live2DModel);
        ////控制动画切换动作
        if (Input.GetKeyDown(KeyCode.M))
        {
            motionIndex++;
            if (motionIndex >= expressions.Length)
            {
                motionIndex = 0;
            }
            expressionMotionQueueManager.startMotion(expressions[motionIndex]);
        }

        
        ////判断待机动作 当前动作进行完毕 true
        //if (l2DMotionManager.isFinished())
        //{
        //    StartMotion(0, 1);
        //}
        // if (Input.GetKeyDown(KeyCode.M))
        //{
        //    StartMotion(14, 2);
        //}
        ////启动l2DMotionManager里的Update
        //l2DMotionManager.updateParam(live2DModel);

        //设置参数 参数id(就live2d编辑器的那个) value就是该参数值 weight权重 影响度
        //live2DModel.setParamFloat("PARAM_ANGLE_X",30,weight);

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    //往之前度数加上10度
        //    live2DModel.addToParamFloat("PARAM_ANGLE_X", 10);
        //}
        ////之前倍数乘上这个数值 也就是正值会向右边扭头 负值控制向左扭头 就可以控制数值了
        //live2DModel.multParamFloat("PARAM_ANGLE_X", 10);
        ////也可以通过获取索引设置参数
        //int paramAngleX;
        //paramAngleX = live2DModel.getParamIndex("PARAM_ANGLE_X");
        //live2DModel.setParamFloat(paramAngleX,30);

        ////参数的保存与回复
        //live2DModel.setParamFloat("PARAM_ANGLE_X", 30, weight);
        ////保存与回复的参数是整个模型的所有参数 并不是之前同方法里的设置的几个参数
        //live2DModel.saveParam();
        //live2DModel.loadParam();

        ////设置某一部分的透明度
        //live2DModel.setPartsOpacity("PARTS_01_CLOTHES",0);
        //眨眼
        eyeBlinkMotion.setParam(live2DModel);
        //后面时间按创建时间计算 类似于计时器
        long time = UtSystem.getUserTimeMSec();//获取执行时间
        physicsHairSideLeft.update(live2DModel, time);
        physicsHairSideRight.update(live2DModel, time);
        physicsHairBackLeft.update(live2DModel, time);
        physicsHairBackRight.update(live2DModel, time);
        //模型跟随鼠标转向与看向
        //参数及时更新，考虑加速度等自然因素 计算坐标 逐渐更新
        drag.update();
        //获取鼠标位置
        Vector3 pos = Input.mousePosition;
        if (Input.GetMouseButton(0))
        {
            //转化坐标
            //得到Live2D鼠标监测点的比例值-1到1（对应Live2D拖拽
            //管理坐标系，或者叫影响度）
            //然后我们通过这个值去设置我们的参数 比如旋转30*当前的到的值
            //然后按这个值所带来的影响度去影响我们的动作
            //从而达到看向某一点的位置
            drag.Set(pos.x/Screen.width*2-1, pos.y / Screen.height * 2 - 1);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            drag.Set(0, 0);
        }

      
        //模型转向
        if (drag.getX()!=0)
        {
            live2DModel.setParamFloat("PARAM_ANGLE_X",30*drag.getX());
            live2DModel.setParamFloat("PARAM_ANGLE_Y", 30 * drag.getY());
            live2DModel.setParamFloat("PARAM_BODY_ANGLE_X", 10 * drag.getX());
            //官方给出如果取-drag.getX() 人物就会一直看你 就是屏幕中间
            live2DModel.setParamFloat("PARAM_EYE_BALL_X",drag.getX());
            live2DModel.setParamFloat("PARAM_EYE_BALL_Y", drag.getY());
        }
    }
    //主摄像头或迷你地图相机正在观看。
    private void OnRenderObject()
    {
        live2DModel.draw();//绘制
    }

    void StartMotion(int motionIndex, int priority)
    {
        //当前优先级 如果当前优先级比较大就不播放了
        if (l2DMotionManager.getCurrentPriority() >= priority)
            return;

        l2DMotionManager.startMotion(motions[motionIndex]);
    }
}
