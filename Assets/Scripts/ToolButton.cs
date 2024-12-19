using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour
{
    public int toolId;
    private ToolManager _toolManager;
    public Sprite selectedSprite;    // 选中状态的图片
    public Sprite unselectedSprite;  // 未选中状态的图片
    public ParticleSystem _particleEffect; // 粒子效果
    public Text countText;

    private Image buttonImage;
    private bool isSelected = false; // 当前状态
    public int count;
    
    //Highlight
    public GameObject fullScreenOverlay; // 全屏的黑色蒙版
    private Dictionary<GameObject, int> originalSortingOrders = new Dictionary<GameObject, int>(); // 存储物体的原始层级

    void Start()
    {
        _toolManager = GameObject.Find("GameManager").GetComponent<ToolManager>();
        buttonImage = GetComponent<Image>();
        EventManager.Instance.AddHandler<int>(GameEvents.FinishedToolUseEvent, HandleFinishedToolUse);
        EventManager.Instance.AddHandler<int, int>(GameEvents.UpdateToolItemCountEvent, HandleUpdateToolItemCount);

        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(ToggleSelection);
        
        // 初始化未选中状态
        buttonImage.sprite = unselectedSprite;
        
        // 确保粒子系统未激活
        if (_particleEffect)
        {
            _particleEffect.Stop();
            _particleEffect.Clear();
        }
         
    }

    public void ToggleSelection()
    {
        if (count <= 0)
        {
            return;
        }

        isSelected = !isSelected;

        // 切换图片
        buttonImage.sprite = isSelected ? selectedSprite : unselectedSprite;

        // 播放粒子效果
        if (isSelected && _particleEffect)
        {
            _toolManager.SelectTool(toolId);
            _particleEffect.Play();
            ShowHighlight();
        }
        else if (_particleEffect)
        {
            _toolManager.SelectTool(0);
            _particleEffect.Stop();
            _particleEffect.Clear();
            HideHighlight();
        }
    }
    // 请求成功的回调
    private void OnRequestSuccess(string responseData)
    {
        Debug.Log("请求成功! 服务器响应数据: " + responseData);
    }

    // 请求失败的回调
    private void OnRequestError(string error)
    {
        Debug.LogError("请求失败! 错误信息: " + error);
    }
    private void HandleFinishedToolUse(int selectToolId)
    {
        if (selectToolId == toolId)
        {
            ToggleSelection();
        }
    }
    
    //Highlight
    public void ShowHighlight()
    {
        int targetLayer = LayerMask.NameToLayer("Item");
        List<GameObject> objectsToHighlight = new List<GameObject>();
        // 获取场景中所有活动的游戏对象
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == targetLayer)
            {
                objectsToHighlight.Add(obj);
            }
        }
        
        // 激活全屏蒙版
        if (fullScreenOverlay != null)
        {
            fullScreenOverlay.SetActive(true);
            fullScreenOverlay.transform.SetAsLastSibling();
        }

        // 遍历需要高亮的物体
        foreach (GameObject obj in objectsToHighlight)
        {
            // 检查物体是否有 SpriteRenderer
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // 存储物体的原始 order in layer
                if (!originalSortingOrders.ContainsKey(obj))
                {
                    originalSortingOrders[obj] = spriteRenderer.sortingOrder;
                }
                // 将物体的 order in layer 提升到一个较高的值
                spriteRenderer.sortingLayerName = "Highlight"; 
                spriteRenderer.sortingOrder = 2; // 保证比普通物体高
            }
        }
        
        gameObject.transform.SetAsLastSibling();
        if (toolId == 101)
        {
            EventManager.Instance.Broadcast(GameEvents.ShowGuideEvent, true);
        }
    }

    private void HideHighlight()
    {
        if (fullScreenOverlay != null)
        {
            fullScreenOverlay.SetActive(false);
        }
    }
    
    private void HandleUpdateToolItemCount(int itemId, int count)
    {
        if (toolId == itemId)
        {
            this.count = count;
            countText.text = "X" + this.count.ToString();   
        }
    }
}

