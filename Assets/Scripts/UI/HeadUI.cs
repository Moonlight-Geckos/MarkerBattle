using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadUI : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private float fadeSpeed = 2;

    [SerializeField]
    private GameObject blackFadedBackground;

    [SerializeField]
    private GameObject startGamePanel;

    [SerializeField]
    private GameObject inGamePanel;

    [SerializeField]
    private GameObject endGamePanel;

    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private GameObject shopPanel;

    [SerializeField]
    private GameObject handAnimator;

    #endregion

    private GameObject currentActivePanel;
    private CanvasGroup blackBackCG;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
                currentActivePanel = child.gameObject;
        }

        blackBackCG = blackFadedBackground.GetComponent<CanvasGroup>();
        blackBackCG.alpha = 0;

        EventsPool.GameFinishedEvent.AddListener((bool w) => FinishGame(w));
    }
    public void StartGame()
    {
        EventsPool.GameStartedEvent.Invoke();
        FadeToPanel(inGamePanel, 0);
    }
    public void Next()
    {
        int ind = SceneManager.GetActiveScene().buildIndex;
        if (ind < SceneManager.sceneCountInBuildSettings - 1)
            SceneManager.LoadScene(ind + 1);
        else
            SceneManager.LoadScene(0);
    }
    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void PauseUnpause()
    {
        if (Time.timeScale > 0)
        {
            ShowPanel(pausePanel, 1);
            Time.timeScale = 0;
        }
        else
        {
            ShowPanel(inGamePanel, 0);
            Time.timeScale = 1;
        }

    }
    public void Shop()
    {
        if(currentActivePanel != shopPanel)
            FadeToPanel(shopPanel, 1);
        else
            FadeToPanel(startGamePanel, 0);

    }
    public void Exit()
    {
        Application.Quit();
    }
    private void FadeToPanel(GameObject panel, float wait = 0, int blackBack = -1)
    {
        CanvasGroup panelCG = panel.GetComponent<CanvasGroup>();
        CanvasGroup curCG = currentActivePanel.GetComponent<CanvasGroup>();
        panelCG.alpha = 0;
        panel.SetActive(true);
        IEnumerator fade()
        {
            yield return new WaitForSeconds(wait);
            while(panelCG.alpha < 1)
            {
                panelCG.alpha += Time.deltaTime * fadeSpeed;
                curCG.alpha -= Time.deltaTime * fadeSpeed;
                if (blackBack == 1)
                {
                    blackBackCG.alpha -= Time.deltaTime * fadeSpeed;
                }
                else if(blackBack == 0)
                {
                    blackBackCG.alpha += Time.deltaTime * fadeSpeed;
                }
                yield return new WaitForEndOfFrame();  
            }
            currentActivePanel.SetActive(false);
            curCG.alpha = 0;
            currentActivePanel = panel;
        }
        StartCoroutine(fade());
    }
    private void ShowPanel(GameObject panel, int blackBack = -1)
    {
        currentActivePanel.SetActive(false);
        panel.SetActive(true);
        currentActivePanel = panel;

        if (blackBack == 0)
        {
            blackBackCG.alpha = 0;
        }
        else if (blackBack == 1)
        {
            blackBackCG.alpha = 1;
        }
    }
    private void FinishGame(bool win)
    {
        EndGame end = endGamePanel.GetComponent<EndGame>();
        if (win)
            end.Win();
        else
            end.Lose();
        FadeToPanel(endGamePanel, 1.5f, 0);
    }
    private void Update()
    {
        handAnimator.transform.position = Input.mousePosition;
    }
}