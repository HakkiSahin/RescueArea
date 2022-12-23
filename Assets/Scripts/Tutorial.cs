using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public Transform fingers;
    public Transform chrc;
    public Transform finger;
    
	

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("RealLevel") == 0) fingers.gameObject.SetActive(true);
		
        if (fingers.gameObject.activeSelf)
        {
            if (Input.GetMouseButton(0))
            {
                
                Vector3 goMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                goMousePos.x -= 0.5f;
                goMousePos.z -= 0.5f;
                //if (Physics.Raycast(chrc.position, Vector3.up, out hit) && fingers.GetChild(0).gameObject.activeSelf)
                if(TouchHandler.I.selectedCount > 0)
                {
                    
                    
                    fingers.GetChild(0).gameObject.SetActive(false);
                    fingers.GetChild(1).gameObject.SetActive(true);
                    

                }

                
                

            }
            if (Input.GetMouseButtonUp(0) && fingers.GetChild(1).gameObject.activeSelf)
            {
                fingers.GetChild(1).gameObject.SetActive(false);
                finger.gameObject.SetActive(true);
            }
            
            
        }
		
    }
}
