using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    bool bDisabled = false;
    bool bExploded = false;
    public float radius;
    public float damage;

    void Start() {
    }

    void Update() {
    }

    void DoDisable() {
	Debug.Log(transform.name + " is disabled");
	bDisabled = true;
    }

    void DoExplode() {
	SwatMove swatMove;
	

	Collider[] coll = Physics.OverlapSphere(transform.position, radius);

	string debug_str = "";
	for(int i = 0; i < coll.Length; i++) {
	    if(coll[i].TryGetComponent(out swatMove)) {
		swatMove.attack.prop.HP -= damage;
		debug_str += (swatMove.transform.name +" Got damage from "+ transform.name +" Damage: "+ damage + "\n");
	    }
	}

	Debug.Log(debug_str);
	Debug.Log(transform.name + " is exploded");
	bExploded = true;
    }

    public void OnTriggerStay(Collider other) {
	if(other.CompareTag("ToCarry")) {
	    if(bDisabled || bExploded) return;

	    SwatMove swatMove = other.GetComponent<SwatMove>();

	    if(swatMove._class == stats._Class.AntiMiner && !bDisabled) {
		    transform.GetChild(2).gameObject.SetActive(true);
			DoDisable();
	    } else {
		    transform.GetChild(1).gameObject.SetActive(true);
		    
			DoExplode();
			swatMove.EndSwat();
		
	    }
	    Destroy(gameObject, 0.5f);
	    //gameObject.SetActive(false);
	}
    }
}
