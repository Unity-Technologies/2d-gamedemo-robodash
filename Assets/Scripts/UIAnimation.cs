using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIAnimation : MonoBehaviour {

    Animator animator;
    public Text timeLabel;
    enum UIState { Start, Game, GameOver }
    UIState uiState = UIState.Start;

    void Start () {
        animator = GetComponent<Animator>();
    }
    
    void Update() {

        if (uiState != UIState.Game) {
            if (Input.GetButton("Jump")) {
                if (uiState == UIState.GameOver) {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                } else {
                    uiState = UIState.Game;
                    FindObjectOfType<GameStateManager>().StartGame();
                    animator.SetTrigger("start_game");
                }
            }

        }
        
    }
    
    public void LevelDone(float timeInSeconds) {

        System.TimeSpan ts = new System.TimeSpan(0, 0, 0, 0, Mathf.RoundToInt(timeInSeconds * 1000f));
        string s = "" + ts.Seconds + "." + ts.Milliseconds;
        if (ts.Minutes > 0) s = s.Insert(0, ts.Minutes + ":");
        timeLabel.text = s;
        animator.SetTrigger("game_over");
        uiState = UIState.GameOver;
    }

}
