using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    private Animator Animator;
    private bool restartSuggested = false;

    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        Animator = gameObject.GetComponent<Animator>();
        Animator.Play("EndingCutscene");
    }


    public void RestartLvl()
    {
        Animator.Play("EndingCutscene2");
        restartSuggested = true;

    }
    
    // Update is called once per frame
    void Update()
    {
        if (restartSuggested && timer > 5f)
        {
                SceneManager.LoadScene(0);
        }

        timer += Time.deltaTime;
    }
}
