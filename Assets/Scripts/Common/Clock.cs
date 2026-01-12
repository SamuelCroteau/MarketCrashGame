using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public int maxHoursInCycle = 5;
    public float hourInterval = 60f;

    [HideInInspector]
    public int currentHour = 0;
    [HideInInspector]
    public float currentTime = 0f;
    [HideInInspector]
    public int currentCycle = 0;
    [HideInInspector]
    public bool shouldAdvanceTime = true;

    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }

    private void Update()
    {
        if (!shouldAdvanceTime)
            return;
        currentTime += Time.deltaTime;

        if (currentTime >= hourInterval)
            UpdateHours();
        if (currentHour >= maxHoursInCycle)
        {
            currentCycle++;
            currentHour = 0;
        }

        Finder.LevelManager.inGameUI.UpdateClock((currentCycle * maxHoursInCycle) + currentHour, currentTime == 0f ? 0 : (int)((currentTime / hourInterval) * 60));
    }

    public void UpdateHours()
    {
        currentHour++;
        currentTime = 0f;
        levelManager.ComputeHourlyChanges();
    }
}