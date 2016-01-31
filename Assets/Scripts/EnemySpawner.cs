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

    [SerializeField] EnemyProbability[] enemyProbability;
    [SerializeField] List<BezierSpline> paths;
    [SerializeField]    float timer = .1f;
    [SerializeField]    Transform exitLocation;

    PoolDictionary enemyPool = new PoolDictionary();

    [SerializeField]
    Probability<GameObject> probability = new Probability<GameObject>();

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
        
        InvokeRepeating("Spawn", .01f, timer);

    }

    void Update()
    {

    }

    void Spawn()
    {
        EnemyAI ai = enemyPool.Get(probability.Get()).GetComponent<EnemyAI>();
        int value = Random.Range(0, paths.Count);
        ai.spline = paths[value];
        ai.exitLocation = transform;
        ai.transform.position = transform.position;
        ai.gameObject.SetActive(true);

    }
}
