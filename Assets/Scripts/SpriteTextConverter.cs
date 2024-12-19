using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpriteTextConverter : MonoBehaviour
{
    [SerializeField] public TMP_Text scoreText; // 绑定的TMP文本
    private string _spriteSheetName = "ScoreFont"; // TMP图集中精灵的名称

    /// <summary>
    /// 设置显示的内容，自动转换为富文本格式
    /// </summary>
    /// <param name="inputText">输入的普通字符文本</param>
    public void SetText(string inputText)
    {
        // 将字符转换为富文本格式
        string convertedText = ConvertToRichText(inputText);

        // 设置TMP文本
        scoreText.text = convertedText;
    }

    /// <summary>
    /// 将输入的普通文本转换为TMP富文本
    /// </summary>
    /// <param name="input">输入的普通文本</param>
    /// <returns>富文本格式字符串</returns>
    private string ConvertToRichText(string input)
    {
        List<string> convertedSprites = new List<string>();

        foreach (char c in input)
        {
            // 假设每个字符都需要对应一个精灵
            convertedSprites.Add($"<sprite=\"{_spriteSheetName}\" name=\"{c}\">");
        }

        // 将所有富文本字符拼接成一个字符串
        return string.Join("", convertedSprites);
    }
}