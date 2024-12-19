using UnityEngine;
using UnityEngine.Networking;

public class ShareManager : MonoBehaviour
{
    private string botToken = "YOUR_BOT_TOKEN";
    private string chatId = ""; // 你可能需要动态获取这个值
    
    public void OnShareButtonClicked()
    {
        chatId = GameInstance.Instance.userInfo.telegramId;
        string link = "https://t.me/CapCapHubBot?start=share"; // 这是要分享的链接
        string shareText = $"Hey! Check out this cool app: {link}";
        
        Application.OpenURL($"https://t.me/share/url?url={link}&text={UnityWebRequest.EscapeURL(shareText)}");
    }
}
