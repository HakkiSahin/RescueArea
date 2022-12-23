using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    private Transform menus;
    public Animator[] anim;
    public static GameHandler i;
    public TMP_Text Level;
    public Animator swipeTutorial;


    private void Start()
    {
        i = this;
        menus = GameObject.FindGameObjectWithTag("menus").transform;
        Array.Resize(ref anim, menus.childCount);
        for (var i = 0; i < menus.childCount; i++) anim[i] = menus.GetChild(i).GetComponent<Animator>();

        Level.text = "LVL" + " " + (PlayerPrefs.GetInt("level") + 1);
    }


    public void Update()
    {
        ScreenShot();
        if(EnemyAIHandler.I.AllDeath()) EndGameEnable();
        if (TouchHandler.I.chrcDeath())
        {
            TryAgainEnable();
        }
    }

    

    private bool fooprot;

    public void StartMenu_disable()
    {
        //anim[0].SetBool("start", false);

        if (!fooprot)
            //Elephant.LevelStarted(PlayerPrefs.GetInt("level") + 1);
            fooprot = true;

        swipeTutorial.SetBool("done", true);
    }

    public void EndGameEnable()
    {
        anim[1].SetBool("start", true);
    }

    public void TryAgainEnable()
    {
        anim[2].SetBool("start", true);
    }

    public void tryAgainButton()
    {
        var current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(current);
    }

    private int currentSS = 0;

    private void ScreenShot()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ScreenCapture.CaptureScreenshot("game" + PlayerPrefs.GetInt("s") + ".png");
            PlayerPrefs.SetInt("s", PlayerPrefs.GetInt("s") + 1);
            Time.timeScale = 0.001f;
            Debug.Log("Selection");
        }

        if (Input.GetKeyDown(KeyCode.A)) Time.timeScale = 1;
    }

    public void endGameButton()
    {
        var current = SceneManager.GetActiveScene().buildIndex + 1;
        //Elephant.LevelCompleted(PlayerPrefs.GetInt("level") + 1);
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        Debug.Log(PlayerPrefs.GetInt("level"));
        if (current < SceneManager.sceneCountInBuildSettings)
        {
            PlayerPrefs.SetInt("level_real", current);
        }
        else
        {
            current = 1;
            PlayerPrefs.SetInt("level_real", 1);
        }

        SceneManager.LoadScene(current);
    }
}