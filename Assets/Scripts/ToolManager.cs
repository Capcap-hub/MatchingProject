using System.Collections;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    private int _selectToolId;
    public GameObject mergeEffectPrefab;
    public GameObject needleToolPrefab;
    
    private bool _isTooling; //正在执行道具
    
    public void SelectTool(int toolId)
    {
        _selectToolId = toolId;
        EventManager.Instance.Broadcast<bool>(GameEvents.SelectedToolEvent, _selectToolId >= 100);
    }

    public void Update()
    {
        if (_selectToolId < 100) return;

        if (Input.GetMouseButtonDown(0) &&
            !_isTooling)
        {
            _isTooling = true;
            if (_selectToolId == 100)
            {
                HandleNeedleTool();
            }
            else if (_selectToolId == 101)
            {
                HandleMixTool();
            }
        }
    }

    private void HandleNeedleTool()
    {
        int targetLayer = LayerMask.GetMask("Item");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, targetLayer);

        if (hit.collider != null &&
            hit.collider.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            NeedleTool needleTool = Instantiate(needleToolPrefab, hit.collider.gameObject.transform.position, needleToolPrefab.transform.rotation).GetComponent<NeedleTool>();
            
            needleTool.OnAnimationComplete += () =>
            {
                GameObject effect = Instantiate(mergeEffectPrefab, hit.collider.gameObject.transform.position, Quaternion.identity);
                // 销毁动画对象
                Destroy(effect, 1f);

                // 销毁点击的对象
                Destroy(hit.collider.gameObject);
        
                EventManager.Instance.Broadcast(GameEvents.FinishedToolUseEvent, _selectToolId);
                _isTooling = false;
            };
        }
        else
        {
            Debug.Log("没有点击到可交互物体");
            _isTooling = false;
        }
    }

    private void HandleMixTool()
    {
        int targetLayer = LayerMask.NameToLayer("Item");

        // 获取场景中所有活动的游戏对象
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // 检查物体是否属于目标层
            if (obj.layer == targetLayer)
            {
                if (obj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2D))
                {
                    float randomAngle = Random.Range(0f, 360f); // 在 0 到 360 之间生成一个随机角度（以度为单位）
                    Vector2 randomDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));

                    // 施加力
                    Vector2 randomForce = randomDirection * 40f;
                    rb2D.AddForce(randomForce, ForceMode2D.Impulse);
                }
            }
        }

        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.4f);
        EventManager.Instance.Broadcast(GameEvents.FinishedToolUseEvent, _selectToolId);
        _isTooling = false;
    }
}