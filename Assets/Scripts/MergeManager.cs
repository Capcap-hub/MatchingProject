using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    private float _moveDuration = 0.1f; // 位移动画持续时间
    private Spawner _spawner;
    private int[] _scores;
    private int _baseScore = 1;
    private float _growthRate = 2.5f;
    
    public GameObject mergeEffectPrefab;

    private void Start()
    {
        _spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        _scores = GenerateScores(10, _baseScore, _growthRate);

        // 打印所有分数以验证
        for (int i = 0; i < _scores.Length; i++)
        {
            // Debug.Log($"Level {i + 1} Score: {_scores[i]}");
        }   
    }

    public void MergeItems(List<GameObject> items)
    {
        if (items == null || items.Count < 2)
        {
            Debug.LogWarning("Not enough items to merge.");
            return;
        }

        // 计算中心点
        Vector2 centerPosition = CalculateCenter(items);

        // 开始位移动画
        StartCoroutine(MoveToCenterAndMerge(items, centerPosition));
    }

    // 计算物体的中心点
    private Vector2 CalculateCenter(List<GameObject> items)
    {
        Vector2 sum = Vector2.zero;
        foreach (GameObject item in items)
        {
            sum += (Vector2)item.transform.position;
        }
        return sum / items.Count;
    }

    private IEnumerator MoveToCenterAndMerge(List<GameObject> items, Vector2 centerPosition)
    {
        float elapsedTime = 0f;

        // 记录每个物体的初始位置
        Dictionary<GameObject, Vector2> startPositions = new Dictionary<GameObject, Vector2>();
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                startPositions[item] = item.transform.position;
            }
        }

        // 位移动画
        while (elapsedTime < _moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _moveDuration;

            // 使用缓动函数调整 t，模拟加速
            float easedT = Mathf.SmoothStep(0, 1, t);

            foreach (GameObject item in items)
            {
                if (item != null)
                {
                    item.transform.position = Vector2.Lerp(startPositions[item], centerPosition, easedT);
                }
            }

            yield return null; // 等待下一帧
        }

        // 确保所有物体到达最终位置
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                item.transform.position = centerPosition;
            }
        }

        int itemId = 0;
        ItemHandler itemHandler = items.First().GetComponent<ItemHandler>();
        itemId = itemHandler.itemId + 1;

        // 销毁原物体
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }

        GameObject prefab = _spawner.itemPrefabs[itemId];
        if (prefab != null) // 生成了新物体，计算分数
        {
            // 播放合成特效
            PlayMergeEffect(centerPosition);
            GameObject obj = Instantiate(prefab, centerPosition, Quaternion.identity);
            obj.layer = LayerMask.NameToLayer("Item");
            // 弹跳动画
            StartCoroutine(PlayBounceAnimation(obj));

            ItemMovement itemMovement = obj.GetComponent<ItemMovement>();
            itemMovement.IsMergeSpawnerItem(true);
            itemMovement.hasTouched = true;
            EventManager.Instance.Broadcast(GameEvents.UpdateScoreEvent, _scores[itemId]);
            EventManager.Instance.Broadcast(GameEvents.MatchedSoundEvent);
        }
    }

    private IEnumerator PlayBounceAnimation(GameObject obj)
    {
        // 检查物体是否已销毁
        if (obj == null) yield break;

        Vector3 originalScale = obj.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f; // 放大20%

        float bounceDuration = 0.2f; // 弹跳动画持续时间
        float elapsedTime = 0f;

        // 放大阶段
        while (elapsedTime < bounceDuration)
        {
            if (obj == null) yield break; // 检查物体是否已销毁
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / bounceDuration;
            obj.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsedTime = 0f;

        // 缩小回原始大小阶段
        while (elapsedTime < bounceDuration)
        {
            if (obj == null) yield break; // 检查物体是否已销毁
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / bounceDuration;
            obj.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        // 确保回到原始大小
        if (obj != null)
        {
            obj.transform.localScale = originalScale;
        }
    }


    private int[] GenerateScores(int levels, int baseScore, float growthRate)
    {
        int[] scoreArray = new int[levels];

        for (int i = 0; i < levels; i++)
        {
            // 使用指数增长公式生成分数
            scoreArray[i] = Mathf.RoundToInt(baseScore * Mathf.Pow(growthRate, i));
        }

        return scoreArray;
    }
    
    private void PlayMergeEffect(Vector2 position)
    {
        if (mergeEffectPrefab != null)
        {
            GameObject effect = Instantiate(mergeEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 1f); // 确保特效在2秒后自动销毁
        }
    }
}
