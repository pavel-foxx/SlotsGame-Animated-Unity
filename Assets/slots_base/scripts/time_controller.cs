using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class time_controller : MonoBehaviour
{
    public Text d_time;
    private void Start()
    {
        StartCoroutine(check());
    }
    IEnumerator check()
    {
        DateTime date = DateTime.Now;
        Console.WriteLine(date.ToString("d"), CultureInfo.InstalledUICulture);

        d_time.text = date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
        yield return new WaitForSeconds(1);
        StartCoroutine(check());
    }
}
