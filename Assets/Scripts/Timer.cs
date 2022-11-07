using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timer;
    public TextMeshProUGUI timeText;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        SetTimeText((int)timer);
    }
    public void SetTimeText(int time)
    {
        int minute = time / 60;
        int seconds = time % 60;
        timeText.text = minute.ToString("00") + ":" + seconds.ToString("00");
    }
}
