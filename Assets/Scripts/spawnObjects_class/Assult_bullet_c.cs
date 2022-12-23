using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assult_bullet_c : MonoBehaviour
{
    float speed = 50f;
    bool bCanGo = false;
    Vector3 ToGo;
    float attackToGive;
    MeshRenderer mr;

    public float Range;
    private EnemyAI ai;

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
            ai.GetDamage(attackToGive);
            mr.enabled = false;
            transform.GetChild(1).gameObject.SetActive(true);
            Destroy(gameObject, 1f);
        }
    }

    public void Set(Vector3 toGo, float AttackToGive, Transform enemy = null) {
        ToGo = toGo;
        transform.SetParent(null);
        attackToGive = AttackToGive;
        if(enemy != null) ai = enemy.GetComponent<EnemyAI>();
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
