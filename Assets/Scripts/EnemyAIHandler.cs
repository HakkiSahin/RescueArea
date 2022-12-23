using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIHandler : MonoBehaviour
{
    public static EnemyAIHandler I;

    EnemyAI[] aiS;
    [HideInInspector] public Transform[] aiS_transform;
    [HideInInspector] public Dictionary<Transform, EnemyAI> dic = new Dictionary<Transform, EnemyAI>();

    void Start() {
	I = this;
	aiS = GameObject.FindObjectsOfType<EnemyAI>();
	aiS_transform = new Transform[aiS.Length];

	for(int i = 0; i < aiS.Length; i++) {
	    aiS_transform[i] = aiS[i].transform;
	    dic.Add(aiS[i].transform, aiS[i]);
	}
    }

    void Update() {
	    
    }

    public bool AllDeath()
    {
	    
	    int deathCount = 0;
	    for (int i = 0; i < aiS.Length; i++)
	    {
		    if (aiS[i].bDead) deathCount++;
	    }

	    return deathCount >= aiS.Length;

    }
}
