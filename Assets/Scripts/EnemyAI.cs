using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using stats;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform toAttack;
    public bool bDead;
    public static EnemyAI I;

    public float Health;
    Animator anim;
    public GameObject spawnedOne;
    private GameObject spawnedOne_copy;
    public float timer;
    private float timerlimit = 3.5f;
    public float stuntimer;
    private float stunlimit = 4f;
    public bool bCanGo = false;
    private bool prot = false;
    public bool bStun = false;

    void Start() {
	anim = GetComponent<Animator>();
	Health = 100f;
	I = this;
    }

    void Update() {
		if(!bDead) FindClosest();
		
		if(toAttack != null) {
			Vector3 attackPoint = new Vector3(toAttack.position.x, transform.position.y, toAttack.position.z) - transform.position;
			transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(attackPoint),
				Time.deltaTime * 5f);
			
		}

		if (bStun && !bDead)
		{
			anim.CrossFade("Spin In Place", 0f, 0);
			stuntimer += Time.deltaTime;
			if (stuntimer >= stunlimit)
			{
				bStun = false;
				stuntimer = 0;
			}
			
		}

		if (!bStun && !bDead && !bCanGo)
		{
			anim.CrossFade("RifleIdle", 0f, 0);
		}

		if (TouchHandler.I.AllEnded()) prot = true;
		if(prot && !bStun) timer += Time.deltaTime;
		if (timer >= timerlimit)
		{
			bCanGo = true;
		
			if (!bDead && toAttack != GameObject.FindGameObjectWithTag("FarestObj").transform )
			{
				spawnedOne_copy = Instantiate(spawnedOne, new Vector3(transform.position.x, transform.position.y+1f,transform.position.z), Quaternion.identity);
				anim.CrossFade("Firing Rifle", 0f, 0);
				if(spawnedOne_copy != null) spawnedOne_copy.GetComponent<Enemy_bullet_c>().Set(toAttack.position, toAttack);
				spawnedOne_copy = null;
			}
			timer = 0;
		
		}
		

		if(Health <= 0f) {
			bDead = true;
	    
		}

		if(bDead) anim.CrossFade("Death", 0f, 0);
    }

    public void GetDamage(float attackDamage) {
		Health -= attackDamage;
    }

    public void FindClosest()
    {
	    
	    toAttack = GameObject.FindGameObjectWithTag("FarestObj").transform;

	    for (int i = 0; i < TouchHandler.I.ToCarry.Length; i++)
	    {
		    float old = Vector3.Distance(transform.position,
			    new Vector3(toAttack.position.x, transform.position.y, toAttack.position.z));
		    float current = Vector3.Distance(transform.position,
			    new Vector3(TouchHandler.I.ToCarry[i].transform.position.x, transform.position.y,
				    TouchHandler.I.ToCarry[i].transform.position.z));
		    if (current < old && TouchHandler.I.ToCarry[i].GetComponent<SwatMove>().attack.prop.HP > 0)
		    {
			    toAttack = TouchHandler.I.ToCarry[i].transform;
		    }
		    
	    }

	    

    }
    


}
