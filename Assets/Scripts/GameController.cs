using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject Button;
    [SerializeField]
    private GameObject winPanel, losePanel, gamePanel;
    private GameObject btn;
    [SerializeField]
    private Button replay, retry;
    private List<Button> btnList = new List<Button>();
    [SerializeField]
    private Sprite backgroundImg;
    private List<Sprite> GameSprite = new List<Sprite>();
    private List<int> idOfSprite = new List<int>();
    private Sprite[] SourceSprites;
    private bool firstGuess, secondGuess;
    private string firstName, secondName;
    private int firstIndex, secondIndex;
    private int totalGuess, correctGuess,incorrectGuess, maxGuess = 5;
    [SerializeField]
    private AudioClip backgroundSound, winSound, loseSound;
    [SerializeField]
    private AudioClip trueAnswer, wrongAnswer;
    private AudioSource audioSource;
    private AudioSource effectSource;
    [SerializeField]
    private Text guesses;
    // Start is called before the first frame update
    void Start()
    {
        guesses.text = "Guesses: " + (maxGuess - incorrectGuess).ToString();
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        gamePanel.SetActive(true);
        audioSource = gameObject.AddComponent<AudioSource>();
        effectSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundSound;
        audioSource.Play();
        audioSource.playOnAwake = true;
        audioSource.loop = true;
        GetButton();
        totalGuess = btnList.Count / 2;
        AddListener();
        AddSprites();
        Shuffle(GameSprite);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GetButton()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Button");
        for (int i = 0; i < obj.Length; i++)
        {
            btnList.Add(obj[i].GetComponent<Button>());
            btnList[i].image.sprite = backgroundImg;
        }
        
    }
    void AddListener()
    {
        foreach(Button btn in btnList)
        {
            btn.onClick.AddListener(() => PickPuzzle());
        }
        replay.onClick.AddListener(() => RestartGame());
        retry.onClick.AddListener(() => RestartGame());
    }
    void PickPuzzle()
    {
        int index = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        string name = GameSprite[index].name;
        if(!firstGuess)
        {
            firstIndex = index;
            firstGuess = true;
            firstName = name;
            btnList[firstIndex].image.sprite = GameSprite[firstIndex];
        }
        else if(!secondGuess && index != firstIndex)
        {
            secondGuess = true;
            secondName = name;
            secondIndex = index;
            btnList[secondIndex].image.sprite = GameSprite[secondIndex];
            StartCoroutine(CheckPuzzleMatched());
        }
    }
    IEnumerator CheckPuzzleMatched()
    {
        if (firstName == secondName && firstIndex != secondIndex)
        {
            effectSource.clip = trueAnswer;
            effectSource.Play();
            correctGuess++;
            btnList[firstIndex].interactable = false;
            btnList[secondIndex].interactable = false;
            yield return new WaitForSeconds(0.5f);
            btnList[firstIndex].image.color = new Color(0, 0, 0, 0);
            btnList[secondIndex].image.color = new Color(0, 0, 0, 0);
        }
        else
        {
            effectSource.clip = wrongAnswer;
            effectSource.Play();
            incorrectGuess++;
            yield return new WaitForSeconds(0.5f);
            btnList[firstIndex].image.sprite = backgroundImg;
            btnList[secondIndex].image.sprite = backgroundImg;
        }
        guesses.text = "Guesses: " + (maxGuess - incorrectGuess).ToString();
        firstGuess = false;
        secondGuess = false;
        CheckIfFinish();
    }
    void CheckIfFinish()
    {
        if(correctGuess == totalGuess)
        {
            audioSource.clip = winSound;
            audioSource.loop = false;
            gamePanel.SetActive(false);
            winPanel.SetActive(true);
            audioSource.Play();
        }
        if (incorrectGuess == maxGuess)
        {
            audioSource.clip = loseSound;
            audioSource.loop = false;
            gamePanel.SetActive(false);
            losePanel.SetActive(true);
            audioSource.Play();
        }
    }
    void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    void Awake()
    {
        SourceSprites = Resources.LoadAll<Sprite>("Sprites/GameImage");
        for (int i = 0; i < 8; i++)
        {
            btn = Instantiate(Button);
            btn.name = "" + i;
            btn.transform.SetParent(gamePanel.transform, false);
        }
    }
    void AddSprites()
    {
        int size = btnList.Count;
        int index = Random.Range(0, SourceSprites.Length);
        for (int i = 0; i < size/2; i++)
        {
            while(idOfSprite.Contains(index))
            {
                index = Random.Range(0, SourceSprites.Length);
            }
            idOfSprite.Add(index);
            GameSprite.Add(SourceSprites[index]);
            GameSprite.Add(SourceSprites[index]);
        }
    }
    void Shuffle(List<Sprite> list)
    {
        Sprite temp;
        for(int i=0;i<list.Count;i++)
        {
            temp = list[i];
            int ran = Random.Range(i, list.Count);
            list[i] = list[ran];
            list[ran] = temp;
        }
    }
}
