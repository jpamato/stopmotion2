using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeaserMode : MonoBehaviour
{
    public List<GameObject> escenas;
    public int count;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject e in escenas)
            e.SetActive(false);

        escenas[0].SetActive(true);
        count = 0;
    }

    private void OnEnable() {
        Next();
    }

    void Next() {
        escenas[count].SetActive(false);
        count++;
        if (count > escenas.Count - 1)
            count = 0;
        escenas[count].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
