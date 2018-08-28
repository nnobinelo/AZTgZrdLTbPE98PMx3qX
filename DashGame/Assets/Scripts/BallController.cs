﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public float absorbSpeed;
    public Vector2 startPos;
    public float initialSpeed;
    public float boostSpeed;
    public float CameraShakeIntensity;
    public float CameraShakeDuration;
    public GameObject[] ballPrefabs;

    TargetController target;
    GameObject ballSpawner;
    LevelGenerator LG;
    GameManager game;

    float startSpeed;
    Animator spawnerAnimator;
    Vector2 RandomXPos;
    string targetHit; //Name of the target that was hit;
    Camera mainCam;
    readonly Vector3 originalCamPos = new Vector3(0, 0, -50);     
    public CanvasGroup whiteFlashCG;
    Image whiteFlashCGPanel;
    bool flash = false;
    bool fade2Black = false;
    bool fadeBack = false;
    bool pauseAllCoroutines = false;
    int selectedBallIndex;
    Ball[] balls;
    float tempSpeed;

    public static BallController Instance;

    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
        ballSpawner = GameObject.Find("BallSpawner");
        spawnerAnimator = ballSpawner.GetComponent<Animator>();
        startSpeed = initialSpeed;

        balls = new Ball[ballPrefabs.Length];

        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            GameObject ballPref = Instantiate(ballPrefabs[i]);
            balls[i] = ballPref.GetComponent<Ball>();
            balls[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        //whenever you are retrieving a singleton of another class make sure it is after the singleton is creaeted in that class
        //So pretty much always create a singleton in awake and then retrieve it in start
        target = TargetController.Instance;
        game = GameManager.Instance;
        LG = LevelGenerator.Instance;

        whiteFlashCGPanel = whiteFlashCG.GetComponentInChildren<Image>();

        selectedBallIndex = ZPlayerPrefs.GetInt("ballInUse");
    }

    public int Link2BallItem(string name)
    {
        for (int i = 0; i < balls.Length; i++)
        {
            if ((name.Substring(0, name.Length - 1) + "(Clone)").Equals(balls[i].name))
            {
                return i;
            }
        }
        return 0;
    }

    public void IncreaseDropSpeed(float speed) //original speed is 3
    {
        startSpeed = speed;
    }

    public void SetBall(int index)
    {
        balls[selectedBallIndex].gameObject.SetActive(false);

        selectedBallIndex = index;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            pauseAllCoroutines = true;
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            pauseAllCoroutines = true;
            ZPlayerPrefs.SetInt("ballInUse", selectedBallIndex);
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    private void OnApplicationQuit()
    {
        ZPlayerPrefs.SetInt("ballInUse", selectedBallIndex);
    }

    private void OnEnable()
    { 
        GameManager.GameStarted += GameStarted;
        LevelGenerator.TransitionDone += TransitionDone;
        GameManager.Revive += Revive;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
        LevelGenerator.TransitionDone -= TransitionDone;
        GameManager.Revive -= Revive;
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(1);
        while (pauseAllCoroutines || game.Paused)
        {
            yield return null;
        }

    }

    private void Update()
    {
        if (flash)
        {
            whiteFlashCG.alpha -= Time.deltaTime * 3;
            if (whiteFlashCG.alpha <= 0)
            {
                whiteFlashCG.alpha = 0;
                flash = false;
            }
        }

        if (fade2Black)
        {
            if (!fadeBack)
            {
                whiteFlashCG.alpha += Time.deltaTime * 4;
                if (whiteFlashCG.alpha >= 1)
                {
                    whiteFlashCG.alpha = 1;
                    LG.GoBack2StartLvl(); //sent to levelgenerator
                    target.ResetTargets(); //sent to targetcontroller
                    balls[selectedBallIndex].gameObject.SetActive(false);
                    startSpeed = initialSpeed;
                    fadeBack = true;
                }
            }
            else
            {
                whiteFlashCG.alpha -= Time.deltaTime * 4;
                if (whiteFlashCG.alpha <= 0)
                {
                    whiteFlashCG.alpha = 0;
                    fade2Black = false;
                    fadeBack = false;
                }
            }
        }

    }

    public void FlashWhite()
    {
        whiteFlashCGPanel.color = Color.white;
        flash = true;
        whiteFlashCG.alpha = 1;
    }

    public void Fade2Black() //comes from gamemanager
    {
        whiteFlashCGPanel.color = Color.black;
        fade2Black = true;
        whiteFlashCG.alpha = 0;
    }

    void GameStarted()
    {
        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(startSpeed, boostSpeed, absorbSpeed, startPos, Quaternion.Euler(0, 0, 0), true);
        tempSpeed = startSpeed;

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = startPos;
    }

    void TransitionDone()
    {
        RandomXPos = new Vector2(target.RandomSpawnAreaXRange, startPos.y);

        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(startSpeed, boostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);
        tempSpeed = startSpeed;

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = RandomXPos;
    }

    void Revive()
    {
        RandomXPos = new Vector2(target.RandomSpawnAreaXRange, startPos.y);

        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(tempSpeed, boostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = RandomXPos;
    }

    public void SetTargetHit(string target)
    {
        targetHit = target;
    }

    public string GetTargetHit
    {
        get
        {
            return targetHit;
        }
    }

    public void CameraShake()
    {
        StartCoroutine(ShakeCamera());
    }

    public IEnumerator ShakeCamera()
    {
        float elapsedTime = 0;

        while (elapsedTime < CameraShakeDuration)
        {
            elapsedTime += Time.deltaTime;

            float percentComplete = elapsedTime / CameraShakeDuration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= Mathf.PerlinNoise(x, y) * CameraShakeIntensity * damper;
            y *= Mathf.PerlinNoise(x, y) * CameraShakeIntensity * damper;

            mainCam.transform.localPosition = new Vector3(x, y, originalCamPos.z);

            while (pauseAllCoroutines || game.Paused)
            {
                yield return null;
            }

            yield return null;
        }

        mainCam.transform.position = originalCamPos;
    }

}
