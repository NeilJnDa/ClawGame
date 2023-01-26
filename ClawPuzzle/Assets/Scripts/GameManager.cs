using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public List<string> levelList = new List<string>();
    public int currentLevelIndex = 0;
    public Level currentLevel;
    // Start is called before the first frame update
    void Start()
    {
        
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
        var playerRB = FindObjectOfType<Player>().transform.GetComponent<Rigidbody>();
        playerRB.useGravity = true;
        playerRB.isKinematic = false;
        playerRB.AddForce(Vector3.down / 10f);

        yield return new WaitForSeconds(2f);
        currentLevel.transform.DOLocalMoveX(-10f, 2f);
        yield return new WaitForSeconds(2f);
        currentLevelIndex++;
        if(currentLevelIndex < levelList.Count)
        {
            currentLevel.levelName = levelList[currentLevelIndex];
            currentLevel.Initialize();
            TurnManager.Instance.Initialize();
            currentLevel.transform.position = Vector3.right * 10f;
        }
        currentLevel.transform.DOMove(Vector3.zero, 2f);
        yield return new WaitForSeconds(2f);
    }
}
