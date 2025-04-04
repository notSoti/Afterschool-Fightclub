using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;


public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject numberone;
    public GameObject numbertwo;
    public GameObject numberthree;
    float time = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void timesetter() {
        time = 0;
    }
    private void Update() {

        if (time >= 0)
        {
            time += Time.deltaTime;
        }

        if (Math.Floor(time) == 0) {
            numberthree.SetActive(true);
        }
        if (Math.Floor(time) == 1) {
            numberthree.SetActive(false);
            numbertwo.SetActive(true);
        }
        if (Math.Floor(time) == 2) {
            numbertwo.SetActive(false);
            numberone.SetActive(true);
        }
        if (Math.Floor(time) == 3) {
            numberone.SetActive(false);
            time = -1;
            // SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
        


    }


}
