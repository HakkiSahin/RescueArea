using System;
using System.Collections;
using System.Collections.Generic;
using stats;
using UnityEngine;

public class Enemy_bullet_c : MonoBehaviour
{
    private bool prot = false;
    MeshRenderer mr;
    bool bCanGo = false;
    Vector3 ToGo;
    private SwatMove sw;
    void Start() {
        mr = transform.GetChild(0).GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        if(!mr.enabled) return;
        if (bCanGo)
        {
            transform.GetChild(2).gameObject.SetActive(true);
            transform.position = Vector3.Lerp(transform.position, new Vector3(ToGo.x, ToGo.y+1f, ToGo.z), 25f * Time.deltaTime);
        }
        if (Vector3.Distance(transform.position, new Vector3(ToGo.x, ToGo.y+1f, ToGo.z)) < 0.1f && !prot)
        {
            if(!sw.attack.prop.bDeath)sw.attack.GetDamage(transform);
            transform.GetChild(1).gameObject.SetActive(true);
            mr.enabled = false;
            Destroy(gameObject, 1f);
            prot = true;

        }
    }

    public void Set(Vector3 toGo, Transform chrc = null)
    {
        ToGo = toGo;
        if (chrc != null)
        {
            sw = chrc.GetComponent<SwatMove>();
            bCanGo = true;
        }
        
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
