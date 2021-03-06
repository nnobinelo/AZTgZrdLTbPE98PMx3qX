﻿using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public GameObject spawnArea;
    RectTransform spawnAreaRect;
    Vector3[] spawnAreaCorners = new Vector3[4];
    CircleCollider2D collider; //used to account for the offset needed for the radius of the target
    public GameObject TargetPrefab;
    BallController ballC;
    LevelGenerator LG;
    GameManager game;
    float randomSize;
    Vector3 defaultTargetSize = new Vector3(0.23f, 0.23f, 1);
    Vector3 troubleshootingSize = new Vector3(0.7f, 0.7f, 1);
    public float travelSpeed;
    float codeTravelSpeed;
    float tempSpeed;
    Transform nextLvl;
    public float growShrinkSpeed;
    Vector3[] nextObstaclePath;
    float targetRadius = 2.53f; //based on 2d circle collider radius
    float targetSpawnOffset = 0.667f;
    static CryptoRandom rng = new CryptoRandom();
    List<Target> targets;
    GameObject targetP;
    Target targetComponent;
    int nextLvlNum;

    public static TargetController Instance;

    public delegate void TargetCDelegate(float spd);
    public static event TargetCDelegate ChangeSpeedImmediately;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        codeTravelSpeed = travelSpeed;

        collider = TargetPrefab.GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        LG = LevelGenerator.Instance;
        game = GameManager.Instance;
        ballC = BallController.Instance;

        spawnAreaRect = spawnArea.transform as RectTransform;
        spawnAreaRect.GetWorldCorners(spawnAreaCorners);

        targets = new List<Target>();

        SpawnTargets(3, Color.yellow);
    }

    public void SpawnTargets(int size, Color color)
    {
        int i = targets.Count;
        while (targets.Count < size)
        {
            targetP = Instantiate(TargetPrefab);
            targetP.SetActive(false);
            targetP.name = "target" + i;
            i++;

            targetComponent = targetP.GetComponent<Target>();
            targetComponent.SetColor(color);

            targets.Add(targetComponent);
        }
    }

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
        LevelGenerator.NextLvlGenerated += NextLvlGenerated;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
        LevelGenerator.NextLvlGenerated -= NextLvlGenerated;
    }

    public void IncreaseTravelSpeed(float speed) // initial travel speed started at 2
    {
        codeTravelSpeed = speed;
    }

    public void IncreaseTravelSpeedImmediately(float speed) // initial travel speed started at 2
    {
        try
        {
            ChangeSpeedImmediately(speed);
        }
        catch (System.NullReferenceException) { }
    }

    void NextLvlGenerated()
    {
        nextLvl = LG.GetNextLvl;
        nextObstaclePath = LG.GetNextObstaclePath;
        tempSpeed = codeTravelSpeed;
        nextLvlNum = LG.GetNextLvlNumber;

        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].InUse)
            {
                if (nextLvlNum == 1 || nextLvlNum == 2)
                {
                    if (nextObstaclePath != null)
                    {
                        targets[i].gameObject.SetActive(true);
                        targets[i].Spawn(nextLvl, Vector2.zero, defaultTargetSize, true, false, nextObstaclePath, tempSpeed);
                    }
                    else
                    {
                        targets[i].gameObject.SetActive(true);
                        targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, false, false);
                    }
                }

                if (nextLvlNum == 3)
                {
                    int randomNumber = rng.Next(1,12);
                    if (randomNumber % 2 != 0) //if this if statement is true then the target will growShrink
                    {
                        if (nextObstaclePath != null)
                        {
                            targets[i].gameObject.SetActive(true);
                            targets[i].Spawn(nextLvl, Vector2.zero, defaultTargetSize, true, true, nextObstaclePath, tempSpeed, growShrinkSpeed);
                        }
                        else
                        {
                            targets[i].gameObject.SetActive(true);
                            targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, false, true, nextObstaclePath, tempSpeed, growShrinkSpeed);
                        }
                    }
                    else
                    {
                        if (nextObstaclePath != null)
                        {
                            targets[i].gameObject.SetActive(true);
                            targets[i].Spawn(nextLvl, Vector2.zero, defaultTargetSize, true, false, nextObstaclePath, tempSpeed);
                        }
                        else
                        {
                            targets[i].gameObject.SetActive(true);
                            targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, false, false, nextObstaclePath);
                        }
                    }
                }

                if (nextLvlNum >= 4)
                {
                    if (nextObstaclePath != null)
                    {
                        targets[i].gameObject.SetActive(true);
                        targets[i].Spawn(nextLvl, Vector2.zero, defaultTargetSize, true, true, nextObstaclePath, tempSpeed, growShrinkSpeed);
                    }
                    else
                    {
                        targets[i].gameObject.SetActive(true);
                        targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, false, true, nextObstaclePath, tempSpeed, growShrinkSpeed);
                    }
                }
                break;
            }
        }
    }

    public void SpawnTarget(Transform obTransform, bool travel, bool growShrink, Vector3[] obPath = null) // used for other game modes mainly, if not using obstacles just set obTransform to enviroment transform
    {
        tempSpeed = codeTravelSpeed;

        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].InUse)
            {
                targets[i].gameObject.SetActive(true);
                targets[i].Spawn(obTransform, RandomPos(), defaultTargetSize, travel, growShrink, obPath, tempSpeed, growShrinkSpeed);
                break;
            }
        }
    }

    public void ResetTargets()
    {
        codeTravelSpeed = travelSpeed;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].isActiveAndEnabled)
            {
                targets[i].StopUsing();
            }
        }
    }

    void GameStarted()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].InUse)
            {
                targets[i].gameObject.SetActive(true);
                targets[i].Spawn(LG.GetCurrentLvl, RandomPos(), defaultTargetSize, false, false);
                break;
            }
        }
    }

    public Vector2 RandomPos()
    {
        return new Vector2(RandomFloat(spawnAreaCorners[0].x + (targetSpawnOffset), spawnAreaCorners[3].x - (targetSpawnOffset)), RandomFloat(spawnAreaCorners[0].y + (targetSpawnOffset), spawnAreaCorners[2].y - (targetSpawnOffset)));
    }

    public float GetSpawnAreaXPositions(bool leftEdge) //returns right edge if this argument is false
    {
        if (leftEdge)
            return spawnAreaCorners[0].x;
        else
            return spawnAreaCorners[3].x;
    }

    float RandomFloat(double min, double max)
    {
        return (float)(min + rng.NextDouble() * (max - min));
    }

    public Vector3 GetCurrentTargetPos(int targetIndex)
    {
        return targets[targetIndex].transform.position;
    }

    public float getTravelSpeed(int targetIndex)
    {
        return targets[targetIndex].TravelSpeed;
    }

    public bool IsMoving(int targetIndex)
    {
        return targets[targetIndex].Moving;
    }

    public void ShrinkTarget(int targetIndex)
    {
        targets[targetIndex].Shrink();
    }

    public void ShrinkTarget2(int targetIndex) //used for other game modes to make transitions less weird
    {
        targets[targetIndex].Shrink2();
    }

    public void SetTargetColor(Color col)
    {
        for(int i = 0; i < targets.Count; i++)
        {
            targets[i].SetColor(col);
        }
    }
}
