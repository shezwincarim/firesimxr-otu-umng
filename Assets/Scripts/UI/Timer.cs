using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class Timer : MonoBehaviour
{
    [Header("Timer Elements")]
    public TextMeshProUGUI timerText, gradeText, completedTimeText;
    public TextMeshProUGUI taskText; 
    public float timeRemaining, modeTimer;
    private bool timerIsRunning = false;
    private float totalTime;
    private EmergencyDialer phone;
    private FireAlarm alarm;
    private FireSpreadManager fireInstance;

    [Header("Ui Elements")]
    public GameObject phoneObject;
    public GameObject fire;
    public GameObject fireText;
    public GameObject finishedText;
    public GameObject timesUpText;
    public Image task1, task2, task3, task4;
    public Color doneColor, failedColor;
    public GameObject tasksAccomplished, leftRay, rigthRay;

    private string[] tasks = { "Identify Fire", "Trigger Alarm", "Call Emergency", "Extinguish Fire" };
    private int currentTaskIndex = 0;

    private void Start()
    {
        timerIsRunning = true;
        totalTime = timeRemaining;
        alarm = GameObject.FindWithTag("FireAlarm")?.GetComponent<FireAlarm>();
        phone = phoneObject.GetComponent<EmergencyDialer>();
        UpdateTaskText();
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (alarm.isCompleted && FireSpreadManager.Instance.allFiresOut && phone.isCompleted && FireSpreadManager.Instance.isIdentified)
            {
                CompleteAllTasks();
                return;
            }

            if (FireSpreadManager.Instance.allFiresOut)
            {
                CompleteAllTasks();
                return;
            }

            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
                UpdateTaskProgress();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                CheckTasksCompletion();
                tasksAccomplished.SetActive(true);
                timesUpText.SetActive(true);
                leftRay.SetActive(true);
                rigthRay.SetActive(true);
                float finishedTime = totalTime - timeRemaining;
                DisplayCompletedTime(finishedTime);
                gradeText.text = CalculateGrade();
            }
        }
    }

    void UpdateTaskProgress()
    {
        if (currentTaskIndex == 0 && FireSpreadManager.Instance.isIdentified) currentTaskIndex = 1;
        if (currentTaskIndex == 1 && alarm.isCompleted) currentTaskIndex = 2;
        if (currentTaskIndex == 2 && phone.isCompleted) currentTaskIndex = 3;
        if (currentTaskIndex == 3 && FireSpreadManager.Instance.allFiresOut) currentTaskIndex = 4;

        UpdateTaskText();
    }

    void UpdateTaskText()
    {
        if (taskText != null && currentTaskIndex < tasks.Length)
        {
            taskText.text = tasks[currentTaskIndex];
        }
    }

    void CompleteAllTasks()
    {
        SetTaskColor(doneColor);
        tasksAccomplished.SetActive(true);
        finishedText.SetActive(true);
        leftRay.SetActive(true);
        rigthRay.SetActive(true);
        timerIsRunning = false;
        gradeText.text = CalculateGrade();
        float finishedTime = totalTime - timeRemaining;
        DisplayCompletedTime(finishedTime);
    }

    void DisplayCompletedTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        completedTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private string CalculateGrade()
    {
        int tasksCompleted = 0;
        if (FireSpreadManager.Instance.isIdentified) tasksCompleted++;
        if (alarm.isCompleted) tasksCompleted++;
        if (phone.isCompleted) tasksCompleted++;
        if (FireSpreadManager.Instance.allFiresOut) tasksCompleted++;

        float timeScore = timeRemaining / modeTimer;
        float gradeScore = (tasksCompleted / 4.0f) * 0.7f + timeScore * 0.3f;

        if (gradeScore >= 0.9) return "A+";
        if (gradeScore >= 0.8) return "A";
        if (gradeScore >= 0.7) return "B+";
        if (gradeScore >= 0.6) return "B";
        if (gradeScore >= 0.5) return "C+";
        if (gradeScore >= 0.4) return "C";
        if (gradeScore >= 0.3) return "D+";

        return "D-";
    }

    private void SetTaskColor(Color color)
    {
        task1.color = alarm.isCompleted ? doneColor : failedColor;
        task2.color = phone.isCompleted ? doneColor : failedColor;
        task3.color = FireSpreadManager.Instance.isIdentified ? doneColor : failedColor;
        task4.color = FireSpreadManager.Instance.allFiresOut ? doneColor : failedColor;
    }

    private void CheckTasksCompletion()
    {
        SetTaskColor(failedColor);
        if (alarm.isCompleted) task1.color = doneColor;
        if (phone.isCompleted) task2.color = doneColor;
        if (FireSpreadManager.Instance.isIdentified) task3.color = doneColor;
        if (FireSpreadManager.Instance.allFiresOut) task4.color = doneColor;
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
