using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class empty : MonoBehaviour
{
    private void Start()
    {
        if(PlayerPrefs.GetInt("level_real") ==0) PlayerPrefs.SetInt("level_real",1);
        SceneManager.LoadScene(PlayerPrefs.GetInt("level_real"));
        Debug.Log(123);
    }
}
