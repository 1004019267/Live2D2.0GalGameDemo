using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using live2d;
public class GameManager : MonoBehaviour
{
    //单例
    static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    //游戏有关的判断
    public bool gameOver;
    public Live2DSimpleModol badBoyScript;
    public GameObject badBoyTalkLine;
    public GameObject gameOverBtns;

    //玩家属性
    public int gold;
    public int favor;
    public int haveDays;

    public Text goldText;
    public Text favorText;
    public Text dateText;

    public LAppModelProxy lAppModelProxy;

    public GameObject actionBtns;

    public GameObject talkLine;

    public Text talkLineText;
    //天黑天亮属性
    public Image mask;
    public bool toAnotherDay;
    public bool toBeDay;//即将天亮
    float timeVal;

    //打工

    public GameObject workBtns;
    public Sprite[] workSprites;
    public Image workImage;
    public GameObject workUI;

    //聊天
    public GameObject ChatUI;


    //约会
    public SpriteRenderer bgImage;
    public Sprite[] dateSprites;

    //其他
    public GameObject clickEffect;
    public Canvas canvans;
    public Texture2D girlNewClothes;

    //音乐播放
    AudioSource audioSource;
    public AudioClip[] audioClips;

    private void Awake()
    {
        _instance = this;
        gold = favor = 0;
        haveDays = 20;
        UpdateUI();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop=true;
        audioSource.clip = audioClips[0];
        audioSource.Play();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //产生鼠标点击特效
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvans.transform as RectTransform, Input.mousePosition, canvans.worldCamera, out mousePos);
            GameObject go = Instantiate(clickEffect);
            go.transform.SetParent(canvans.transform);
            go.transform.localPosition = mousePos;
        }
        //游戏结束逻辑
        if (gameOver)
        {
            talkLine.SetActive(true);
            gameOverBtns.SetActive(true);
            actionBtns.SetActive(false);

            if (favor >= 1400)
            {
                talkLineText.text = "你追到了属于你的傲娇女神B̆̈ĕ̈ h̆̈ă̈p̆̈p̆̈y̆̈";
            }
            else if (haveDays != 0 && favor < 1500)
            {
                talkLineText.text = "你没能在Boss欺负她时保护她 最后他们决裂了";
            }
            else
            {
                talkLineText.text = "你没能在她离开前获取她的芳心";
            }
        }
        //是否过度到另外一天
        if (toAnotherDay)
        {
            if (toBeDay)
            {
                timeVal += Time.deltaTime;
                //天亮的方法
                if (timeVal >= 2)
                {
                    timeVal = 0;
                    ToDay();
                }
            }
            else
            {
                //天黑方法
                ToDark();
            }


        }
    }
    //即将天黑
    public void ToBeDark()
    {
        toAnotherDay = true;
    }
    //天黑方法
    void ToDark()
    {
        //差值当前值 目标值 时间
        mask.color += new Color(0, 0, 0, Mathf.Lerp(0, 1, 0.1f));
        if (mask.color.a >= 0.8f)
        {
            mask.color = new Color(0, 0, 0, 1);
            toBeDay = true;
            ResetUI();
            UpdateUI();
        }
    }
    //天亮
    void ToDay()
    {
        mask.color -= new Color(0, 0, 0, Mathf.Lerp(1, 0, 0.1f));
        if (mask.color.a <= 0.2f)
        {
            mask.color = new Color(0, 0, 0, 0);
            toAnotherDay = false;
            toBeDay = false;
            if (haveDays!=5)
            {
                audioSource.clip = audioClips[0];
                audioSource.Play();
            }
        }
    }
    /// <summary>
    /// 打工
    /// </summary>
    public void ClickWorkBtn()
    {
        actionBtns.SetActive(false);
        workBtns.SetActive(true);
        //隐藏人物
        lAppModelProxy.SetVisible(false);
        PlayBtnSound();
        audioSource.clip = audioClips[2];
        audioSource.Play();
    }

    public void GetMoney(int workIndex)
    {
        audioSource.PlayOneShot(audioClips[6]);
        workBtns.SetActive(false);
        ChangeGold((4 - workIndex) * 20);
        workImage.overrideSprite = workSprites[workIndex];
        workUI.SetActive(true);
        talkLine.SetActive(true);
        talkLineText.text = "一顿辛劳后获得" + ((4 - workIndex) * 20).ToString() + "金币";
    }
    /// <summary>
    /// 聊天
    /// </summary>
    public void ClickChat()
    {
        actionBtns.SetActive(false);
        ChatUI.SetActive(true);
        audioSource.clip = audioClips[1];
        audioSource.Play();
        if (favor >= 100)
        {
            lAppModelProxy.GetModel().StartMotion("tap_body", 1, 2);
        }
        else
        {
            //参数 当前组的名字 组里面第几个  优先级
            lAppModelProxy.GetModel().StartMotion("tap_body", 0, 2);
        }
    }

    public void GetFavor(int chatIndex)
    {
        ChatUI.SetActive(false);
        talkLine.SetActive(true);
        switch (chatIndex)
        {
            case 0:
                if (favor > 20)
                {
                    ChangeFavor(8);
                    talkLineText.text = "谢谢你，你也很帅";
                    lAppModelProxy.GetModel().SetExpression("f02");
                    audioSource.PlayOneShot(audioClips[7]);
                }
                else
                {
                    ChangeFavor(2);
                    talkLineText.text = "嗯";
                    lAppModelProxy.GetModel().SetExpression("f08");
                }
                break;
            case 1:
                if (favor > 60)
                {
                    ChangeFavor(15);
                    talkLineText.text = "emmmmmm 啊~不好意思，谢谢你";
                    lAppModelProxy.GetModel().SetExpression("f07");
                    audioSource.PlayOneShot(audioClips[7]);
                }
                else
                {
                    ChangeFavor(-20);
                    talkLineText.text = "去死！臭虫";
                    lAppModelProxy.GetModel().SetExpression("f04");
                }
                break;
            case 2:
                if (favor > 100)
                {
                    ChangeFavor(35);
                    talkLineText.text = "那我们一起去玩吧，我也等了你好久";
                    lAppModelProxy.GetModel().SetExpression("f05");
                    audioSource.PlayOneShot(audioClips[7]);
                }
                else
                {
                    ChangeFavor(-40);
                    talkLineText.text = "贱民！你配和我说话吗！";
                    lAppModelProxy.GetModel().SetExpression("f03");
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 约会
    /// </summary>
    public void ClickDateBtn()
    {
        actionBtns.SetActive(false);
        talkLine.SetActive(true);
        int randomNum = Random.Range(1, 4);
        bool hasEnoughGold = false;
        bgImage.sprite = dateSprites[randomNum];
        switch (randomNum)
        {
            case 1:
                if (gold >= 50)
                {
                    ChangeGold(-50);
                    ChangeFavor(100);
                    talkLineText.text = "学校门口好热闹啊！ 真开心";
                    hasEnoughGold = true;
                }
                else
                {
                    talkLineText.text = "穷鬼!!!气死我啦 出门不带钱";
                    ChangeFavor(-60);
                }
                break;
            case 2:
                if (gold >= 150)
                {
                    ChangeGold(-150);
                    ChangeFavor(200);
                    talkLineText.text = "这里的饭真的好好吃啊 我下次还想来";
                    hasEnoughGold = true;
                }
                else
                {
                    talkLineText.text = "真寒酸 我请你去死算了";
                    ChangeFavor(-150);
                }
                break;
            case 3:
                if (gold >= 250)
                {
                    ChangeGold(-300);
                    ChangeFavor(350);
                    talkLineText.text = "礼物好漂亮！谢谢你 下次我也买给你";
                    hasEnoughGold = true;
                }
                else
                {
                    talkLineText.text = "哎 一个玩具都买不起，你拿什么追我？";
                    ChangeFavor(-250);
                }
                break;
            default:
                break;
        }
        if (hasEnoughGold)
        {
            lAppModelProxy.GetModel().StartMotion("pinch_in", 0, 2);
        }
        else
        {
            lAppModelProxy.GetModel().StartMotion("flick_head", 0, 2);
        }
        audioSource.clip = audioClips[3];
        audioSource.Play();
    }
    /// <summary>
    /// 表白
    /// </summary>
    public void ClickLoveBtn()
    {
        actionBtns.SetActive(false);
        talkLine.SetActive(true);
        audioSource.clip = audioClips[4];
        audioSource.Play();
        if (favor >= 1200)
        {
            talkLineText.text = "其实我也喜欢你很久了" + "/n" + "真好呢自己喜欢的人也喜欢自己" + "/n"
                + "今后多多指教";
            lAppModelProxy.GetModel().StartMotion("pinch_out", 0, 2);
            lAppModelProxy.GetModel().SetExpression("f07");
            gameOver = true;
        }
        else
        {
            talkLineText.text = "啊~吓我一跳 对不起，你是个好人 最近不怎么想谈恋爱";
            lAppModelProxy.GetModel().StartMotion("shake", 0, 2);
            lAppModelProxy.GetModel().SetExpression("f04");
        }
        PlayBtnSound();
    }
    //更新玩家UI显示
    void UpdateUI()
    {
        goldText.text = gold.ToString();
        favorText.text = favor.ToString();
        dateText.text = haveDays.ToString();
    }
    //改变金币
    public void ChangeGold(int goldValue)
    {
        gold += goldValue;
        if (gold <= 0)
        {
            gold = 0;
        }
        UpdateUI();
    }
    //改变好感度
    public void ChangeFavor(int favorValue)
    {
        favor += favorValue;
        if (favor <= 0)
        {
            favor = 0;
        }
        UpdateUI();
    }

    //重置所有UI
    void ResetUI()
    {
        workUI.SetActive(false);
        talkLine.SetActive(false);
        actionBtns.SetActive(true);
        lAppModelProxy.SetVisible(true);
        lAppModelProxy.GetModel().SetExpression("f01");
        bgImage.sprite = dateSprites[0];
        haveDays--;
        if (haveDays == 5)
        {
            CreateBadBoy();
        }
        else if (haveDays == 10)
        {
            //换装
            Live2DModelUnity live2DModelUnity = lAppModelProxy.GetModel().GetLive2DModelUnity();
            live2DModelUnity.setTexture(2, girlNewClothes);
        }
        else if (haveDays == 0)
        {
            gameOver = true;
        }
    }

    //产生Boss的方法
    void CreateBadBoy()
    {
        lAppModelProxy.isRunnigAway = true;
        badBoyScript.gameObject.SetActive(true);
        lAppModelProxy.GetModel().SetExpression("f04");
        actionBtns.SetActive(false);

        badBoyTalkLine.SetActive(true);
        audioSource.clip = audioClips[5];
        audioSource.Play();
    }

    public void CloseBadBoyTalkLine()
    {
        badBoyTalkLine.SetActive(false);
    }

    public void DefeatBadBoy()
    {
        lAppModelProxy.GetModel().StartMotion("shake", 0, 2);
        talkLine.SetActive(true);
        talkLineText.text = "将才吓死我了 谢谢你大笨蛋";
        ChangeFavor(300);
        badBoyScript.gameObject.SetActive(false);
    }

    public void LoadScence(int scenceNumber)
    {
        SceneManager.LoadScene(scenceNumber);
    }

    public void PlayBtnSound()
    {
        audioSource.PlayOneShot(audioClips[8]);
    }
}
