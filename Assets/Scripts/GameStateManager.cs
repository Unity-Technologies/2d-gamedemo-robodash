using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {

    public GameObject playerPrefab;
    public Vector2 playerRespawnPoint;

    float levelStartedAt;

    enum GameState { NotStarted, Playing, Done }
    GameState gameState = GameState.NotStarted;

    public void PlayerDied() {
        StartCoroutine(RespawnPlayer());
    }

    IEnumerator RespawnPlayer() {
        yield return new WaitForSeconds(.5f);
        MakePlayer();
    }

    void MakePlayer() {
        var startPoint = FindObjectOfType<LevelStart>();
        if (startPoint != null) {
            playerRespawnPoint = startPoint.transform.position;
            startPoint.LevelStarted();
        }
        Instantiate(playerPrefab, playerRespawnPoint, playerPrefab.transform.rotation);
    }

    public void StartGame() {
        gameState = GameState.Playing;
        MakePlayer();
        levelStartedAt = Time.time;
    }

    public void GoalReached(LevelGoal goal) {
        if (gameState != GameState.Playing) return;
        gameState = GameState.Done;
        float totalTime = Time.time - levelStartedAt;
        FindObjectOfType<UIAnimation>().LevelDone(totalTime);
        FindObjectOfType<Hero>().LevelComplete();
    }
}
