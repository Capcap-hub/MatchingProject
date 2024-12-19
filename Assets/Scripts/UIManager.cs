using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private SpriteTextConverter _spriteTextConverter;

    private int _totalScore;

    public GameObject gameOverBG;

    public TextMeshProUGUI gameOverScoreText;
    
    // Start is called before the first frame update
    void Start()
    {
        _spriteTextConverter = gameObject.GetComponent<SpriteTextConverter>();
        _spriteTextConverter.SetText("0");
        
        EventManager.Instance.AddHandler<int>(GameEvents.UpdateScoreEvent, HandleUpdateScore);
        EventManager.Instance.AddHandler(GameEvents.GameOverEvent, HandleGameOver);
    }

    private void HandleUpdateScore(int score)
    {
        _totalScore += score;
        _spriteTextConverter.SetText(_totalScore.ToString());
    }

    private void HandleGameOver()
    {
        NetworkManager.Instance.updateScore(_totalScore);
        gameOverBG.SetActive(true);
        gameOverScoreText.text = _spriteTextConverter.scoreText.text;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
