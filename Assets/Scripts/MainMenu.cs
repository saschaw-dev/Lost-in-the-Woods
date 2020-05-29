using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    GameObject info;
    GameObject backButton;
    GameObject infoButton;
    GameObject startButton;
    GameObject quitButton;

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Use this for initialization
    void Start () {
        info = GameObject.Find("Infobild");
        startButton = GameObject.Find("Start");
        infoButton = GameObject.Find("Info");
        quitButton = GameObject.Find("Beenden");
        backButton = GameObject.Find("Zurück");

        info.SetActive(false);
        backButton.SetActive(false);
	}
	

    public void StartGame()
    {
        SceneManager.LoadScene("LS_Scene");

    }

    public void ShowInfo()
    {
        info.SetActive(true);
        backButton.SetActive(true);
        startButton.SetActive(false);
        infoButton.SetActive(false);
        quitButton.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void Back()
    {
        info.SetActive(false);
        backButton.SetActive(false);
        startButton.SetActive(true);
        infoButton.SetActive(true);
        quitButton.SetActive(true);
    }

    public void BackToMainMenu()
    {
        //hier müssten eig noch vorher Daten gespeichert werden die sonst
        //beim Laden der neuen Szene verloren gehen!!!
        SceneManager.LoadScene("mainmenu");
    }
}
