using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] float timeFreezeLerp = .1f;
    [SerializeField] float slowTimeScale = .1f;
    bool gamePaused = false;
    bool gameSlowed = false;
    bool timeScaleNormal = true;
    float normalTimeScale = 1;
    float normalFixedDeltaTime = 0.02f;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if(Time.timeScale == 1)
        {
            timeScaleNormal = true;
        }
        else
        {
            timeScaleNormal = false;
        }

        if (gamePaused)
        {
            StopTime();
        }
        else if (gameSlowed)
        {
            SlowTime();
        }
        else if(!timeScaleNormal)
        {
            ResumeTime();
        }
        //Debug.Log($"Fixed Delta Time = {Time.fixedDeltaTime}");
    }
    public void PauseGame()
    {
        gamePaused = true;
    }
    public void SlowGame()
    {
        gameSlowed = true;
    }
    public void ResumeGame()
    {
        gamePaused = false;
        gameSlowed = false;
    }
    private void StopTime()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, 0, timeFreezeLerp);
        Time.fixedDeltaTime = normalFixedDeltaTime * Time.timeScale;
    }
    private void SlowTime()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, slowTimeScale, timeFreezeLerp);
        Time.fixedDeltaTime = normalFixedDeltaTime * Time.timeScale;
    }
    private void ResumeTime()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, normalTimeScale, timeFreezeLerp);
        Time.fixedDeltaTime = normalFixedDeltaTime * Time.timeScale;
    }
}
