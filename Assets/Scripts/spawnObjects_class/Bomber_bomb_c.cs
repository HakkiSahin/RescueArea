using System.Collections;
using System.Collections.Generic;
using stats;
using UnityEngine;
using DG.Tweening;

public class Bomber_bomb_c : MonoBehaviour
{
    float speed = 25f;
    bool bCanGo = false;
    Vector3 ToGo;
    float attackToGive;
    MeshRenderer mr;
    public bool bStuns;

    public float Range;
    private float timer;
    private float timerLimit = 2f;

    void Start() {
	mr = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    void Update() {
	if(!mr.enabled) return;

	if (bCanGo)
	{
		transform.GetChild(2).gameObject.SetActive(true);
		transform.position = Vector3.MoveTowards(transform.position, ToGo+Vector3.up, Time.deltaTime * speed);
		
	}
		if(Vector3.Distance(transform.position, ToGo+Vector3.up) < 0.1f) {
			Collider[] coll = Physics.OverlapSphere(transform.position, Range); 

			for(int i = 0; i < coll.Length; i++) {
			EnemyAI trytoget;
				if (coll[i].TryGetComponent<EnemyAI>(out trytoget)) {
				trytoget.bStun = bStuns;
				}
			}

			mr.enabled = false;
			transform.GetChild(1).gameObject.SetActive(true);
			Destroy(gameObject, 1f);
		}
		
    }

    public void Set(Vector3 toGo, bool bStun) {
	ToGo = toGo;
	bStuns = bStun;
	Invoke("SetReal", 0.3f);
	
    }

    public void SetReal()
    {
	    transform.SetParent(null);
	    transform.DOJump(ToGo + Vector3.up, 5f, 1, 0.5f);
	    bCanGo = true;  
    }
    private void OnTriggerStay(Collider other)
    {
	    if (other.CompareTag("wallparent"))
	    {
		    other.transform.GetChild(0).gameObject.SetActive(false);
		    other.transform.GetChild(1).gameObject.SetActive(true);
	    }

	    if (other.CompareTag("wall"))
	    {
		    other.GetComponent<Rigidbody>().isKinematic = false;
		    other.isTrigger = false;
	    }
    }
}
