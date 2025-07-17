using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    [Header("Buttons")]
    public Button m_PlayButton;
    public int lvlToChange = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void Awake()
    {
        m_PlayButton.onClick.AddListener(ButtonClicked);
    }
    // Update is called once per frame
    void Update()
    {
      
    }

    void ButtonClicked()
    {
        Debug.Log("it has begun");
        SceneManager.LoadScene(lvlToChange);
    }
    
  
    
}
