using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTrigger : MonoBehaviour
{
    private LayerMask _itemLayer; // 用于检测特定层
    private Dictionary<GameObject, Coroutine> _activeTimers = new Dictionary<GameObject, Coroutine>(); // 记录物体和计时器

    private void Start()
    {
        _itemLayer = LayerMask.GetMask("Item");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是目标 Layer 的物体
        if (((1 << other.gameObject.layer) & _itemLayer) != 0 &&
            other.gameObject.GetComponent<ItemMovement>().hasTouched)
        {
            // 如果该物体没有在计时，开始计时
            if (!_activeTimers.ContainsKey(other.gameObject))
            {
                Coroutine timer = StartCoroutine(TimerCoroutine(other.gameObject, 3f));
                _activeTimers.Add(other.gameObject, timer);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查是否是目标 Layer 的物体
        if (((1 << other.gameObject.layer) & _itemLayer) != 0 &&
            other.gameObject.GetComponent<ItemMovement>().hasTouched)
        {
            // 如果物体离开，取消对应的计时器
            if (_activeTimers.ContainsKey(other.gameObject))
            {
                StopCoroutine(_activeTimers[other.gameObject]);
                _activeTimers.Remove(other.gameObject);
            }
        }
    }

    private IEnumerator TimerCoroutine(GameObject item, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // 如果计时完成，调用目标方法
        if (item != null)
        {
            TriggerMethod(item);
            
            // 停止所有计时器，清空状态（只需要一个物体触发方法）
            StopAllTimers();
        }
    }

    private void TriggerMethod(GameObject item)
    {
        EventManager.Instance.Broadcast(GameEvents.GameOverEvent);
    }

    private void StopAllTimers()
    {
        // 停止所有计时器
        foreach (var timer in _activeTimers.Values)
        {
            StopCoroutine(timer);
        }
        _activeTimers.Clear(); // 清空字典
    }
}