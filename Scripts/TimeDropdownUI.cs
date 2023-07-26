using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class TimeDropdownUI : MonoBehaviour
{
    private TMP_Dropdown _dropdown;
    private ImageDetails _imageDetails;
    private List<TMP_Dropdown.OptionData> _optionDatas;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        _imageDetails = FindObjectOfType<ImageDetails>();
        
        _dropdown.options.Clear();
        PopulateTimes();

        _dropdown.onValueChanged.AddListener(OnTimeSelected);
    }

    private void OnDestroy()
    {
        _dropdown.onValueChanged.RemoveListener(OnTimeSelected);
    }

    private void OnTimeSelected(int index)
    {
        _imageDetails.SetTime(_optionDatas[index].text);
    }

    private void PopulateTimes()
    {
        _optionDatas = new List<TMP_Dropdown.OptionData>();

        var currentIndex = 0;
        var nowDate = DateTime.Now;
        var selectedIndex = -1;

        var roundedToNearest = RoundUp(nowDate, TimeSpan.FromMinutes(15));

        for (var hour = 0; hour < 24; hour++)
        {
            for (var minute = 0; minute < 60; minute += 15)
            {
                var time = $"{hour:00}:{minute:00}";
                _optionDatas.Add(new TMP_Dropdown.OptionData(time));

                var comparableTime = new TimeSpan(hour, minute, 0);
                
                if (TimeSpan.Compare(roundedToNearest.TimeOfDay, comparableTime) == 0)
                {
                    // check estimated minute
                    selectedIndex = currentIndex;
                }
                
                currentIndex++;
            }
        }

        _dropdown.AddOptions(_optionDatas);

        if (selectedIndex == -1) return;
        
        _dropdown.value = selectedIndex;
        _imageDetails.SetTime(_optionDatas[selectedIndex].text);
    }
    
    private static DateTime RoundUp(DateTime dt, TimeSpan d)
    {
        var modTicks = dt.Ticks % d.Ticks;
        var delta = modTicks != 0 ? d.Ticks - modTicks : 0;
        return new DateTime(dt.Ticks + delta, dt.Kind);
    }
}