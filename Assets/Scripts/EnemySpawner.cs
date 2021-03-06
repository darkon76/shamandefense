﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {

    [System.Serializable]
    class EnemyProbability
    {
        public GameObject enemyPrefab;
        public int probabilityWeight;
    }

    public void AddPath(BezierSpline path)
    {
        paths.Add(path);
    }

    public void ResetPaths()
    {
        paths.RemoveRange(1, paths.Count - 1);
    }

    [SerializeField] EnemyProbability[] enemyProbability;
    [SerializeField] List<BezierSpline> paths;
    [SerializeField]    float timer = .1f;
    [SerializeField]    Transform exitLocation;

    PoolDictionary enemyPool = new PoolDictionary();

    [SerializeField]
    Probability<GameObject> probability = new Probability<GameObject>();

    private float previousTime, waitTime;
    [SerializeField] float maxWaitTime = 7;
    [SerializeField] float minWaitTime = 0.5f;

    public void SetWaitTimes(float maxTime, float minTime)
    {
        maxWaitTime = maxTime;
        minWaitTime = minTime;
    }

    void OnEnable()
    {
        EnemyAI.EventOnStop += OnStop;
    }

    void OnDisable()
    {
        EnemyAI.EventOnStop -= OnStop;
    }

    void OnStop()
    {
        CancelInvoke("Spawn");
    }

	// Use this for initialization
	void Start () {
        enemyPool.targetParent = transform;
        foreach(EnemyProbability prob in enemyProbability)
        {
            probability.Add(prob.enemyPrefab, prob.probabilityWeight);
        }
        
        //InvokeRepeating("Spawn", .01f, timer);

        previousTime = Time.time;
        waitTime = maxWaitTime;
        StartCoroutine(OnSpawn());
    }

    IEnumerator OnSpawn()
    {
        yield return new WaitForSeconds(1.0f);
       
        EnemyAI ai = Instantiate<GameObject>(enemyProbability[0].enemyPrefab).GetComponent<EnemyAI>();
        int value = Random.Range(0, paths.Count);
        ai.spline = paths[value];
        ai.exitLocation = exitLocation;
        ai.transform.position = transform.position;
        ai.gameObject.SetActive(true);

    }


    bool reducingWait = true;
    void Update()
    {
        if (Time.time > previousTime + waitTime)
        {
            previousTime = Time.time;
            Spawn();

            if (waitTime < minWaitTime)
            {
                reducingWait = false;
            }
            if (waitTime > maxWaitTime)
            {
                reducingWait = true;
            }

            waitTime = waitTime * (minWaitTime + (reducingWait ? 0 : 1)); //waitTime = waitTime + (0.5f * (reducingWait ? -1 : 1));
        }
    }

    void Spawn()
    {
        EnemyAI ai = enemyPool.Get(probability.Get()).GetComponent<EnemyAI>();
        int value = Random.Range(0, paths.Count);
        ai.spline = paths[value];
        ai.exitLocation = exitLocation;
        ai.transform.position = transform.position;
        ai.gameObject.SetActive(true);

    }
}
