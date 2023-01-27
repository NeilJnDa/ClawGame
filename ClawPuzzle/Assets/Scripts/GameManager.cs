using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }
    private static GameManager _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    [Button("Complete Current Level")]
    private void CompleteCurrentLevel()
    {
        CompleteLevel();
    }
    public List<string> levelList = new List<string>();
    public int currentLevelIndex = 0;
    public Level currentLevel;

    public TMP_Text displayText;
    public GameObject endText;

    [Header("Audio")]
    public List<AudioClip> successSounds = new List<AudioClip>();


    // Start is called before the first frame update
    void Start()
    {
        displayText.text = currentLevel.displayName;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CompleteLevel()
    {
        StartCoroutine("CompleteLevelExecutor");
    }
    IEnumerator CompleteLevelExecutor()
    {
        InputManager.Instance.EnableInput(false);
        if(successSounds.Count > 0)
            AudioSource.PlayClipAtPoint(successSounds[Random.Range(0, successSounds.Count)], Vector3.zero);

        var playerRB = FindObjectOfType<Player>().transform.GetComponent<Rigidbody>();
        playerRB.useGravity = true;
        playerRB.isKinematic = false;
        yield return new WaitForEndOfFrame();

        playerRB.AddForce(Vector3.down / 10f);

        yield return new WaitForSeconds(1f);
        displayText.text = "";

        currentLevel.transform.DOLocalMoveX(-15f, 1f);
        yield return new WaitForSeconds(1f);
        currentLevelIndex++;
        if(currentLevelIndex < levelList.Count)
        {
            currentLevel.levelName = levelList[currentLevelIndex];
            currentLevel.Initialize();
            TurnManager.Instance.Initialize();
            currentLevel.transform.position = Vector3.right * 15f;
            currentLevel.transform.DOMove(Vector3.zero, 1f);
            yield return new WaitForSeconds(1f);
            displayText.text = currentLevel.displayName;
            InputManager.Instance.EnableInput(true);
        }
        else {
            displayText.text = "";
            endText.SetActive(true);
        }

    }
}
