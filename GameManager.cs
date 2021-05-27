using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject[] targets;
    public GameObject target;
    public GameObject player;
    public GameObject camera;
    public GameObject menuUI;
    public bool playerIsFound = false;//to inform all other bot that player needs to be killed
    private GameObject[] spawnPoints;
    private bool gameRunning = false;
    public int numberOfEnemies = 10;
    public GameObject numberOfEnemiesGO;

    public GameObject gameOverText;

    public long score=0;

    private void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("spawnPoint"); // GetComponentsInChildren<Transform>();

        Time.timeScale = 0.0f;
        //ui display

    }
    public void resumeGame()
    {
        Time.timeScale = 1.0f;
        gameRunning = true;
        menuUI.SetActive(false);
    }
    public void pauseGame()
    {
        Time.timeScale = 0.0f;
        gameRunning = false;
        Cursor.lockState=CursorLockMode.None;
        menuUI.SetActive(true);
    }

    private void FixedUpdate()
    {
        checkOutField();
        targets = GameObject.FindGameObjectsWithTag("target");
        try
        {
            numberOfEnemies = Int16.Parse(numberOfEnemiesGO.GetComponent<Text>().text);
        }catch(Exception e)
        {
            //Debug.LogError(e);
        }
        if(targets.Length <= numberOfEnemies)
        {
            Instantiate(target, spawnPoints[(int)(UnityEngine.Random.value*spawnPoints.Length)].transform.position, Quaternion.Euler(new Vector3(0, UnityEngine.Random.value * 360, 0)));
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            if (gameRunning)
            {
                pauseGame();
                //showMenu(true);
            }
            else
            {
                resumeGame();
                //showMenu(false);
            }
            
        }
            
        //locking cursor
        if (Input.GetKey(KeyCode.O)){
            Cursor.lockState = Cursor.lockState==CursorLockMode.None?CursorLockMode.Locked:CursorLockMode.None;
            Debug.Log(Cursor.lockState);
        }

    }
    void showMenu(bool show)
    {
        menuUI.SetActive(show);
    }
    private void Update()
    {
        if (player == null || player.activeSelf == false)
        {
            camera.SetActive(true);
        }
    }
    void checkOutField()
    {
        if (player.transform.position.y < -1.0)
        {
            Debug.LogError("lower!");
            player.GetComponent<Transform>().SetPositionAndRotation(new Vector3(UnityEngine.Random.value * 50 - 25, 1, -25 + UnityEngine.Random.value * 50), Quaternion.Euler(new Vector3(0, 0, 0))); ;
        }
    }

    public void incScore(int i)
    {
        score += i;
    }
    public void gameOver()
    {
        pauseGame();
        gameOverText.SetActive(true);
        gameOverText.GetComponent<Text>().text = "You score is  " + score + "!\nCongratulations!";

    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
