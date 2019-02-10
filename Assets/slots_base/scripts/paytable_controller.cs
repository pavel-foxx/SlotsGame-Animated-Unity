using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class paytable_controller : MonoBehaviour {
    private static paytable_controller _instance;
    public static paytable_controller instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<paytable_controller>();
            return _instance;
        }
    }
    public Text[] PayInfo;
    public button_handler OpenPaytable, ClosePaytable;
    public GameObject PaytableObject;

    private void Awake()
    {
        OpenPaytable.OnClick += Open;
        ClosePaytable.OnClick += Close;
    }

    public void UpdatePaytable()
    {
        for (int i = 0; i < PayInfo.Length; i++)
            PayInfo[i].text = "= " + game_controller.instance.SlotSettings.Items[i].Value / 5 + "\n x Bet";
    }

    private void Open(button_handler button)
    {
        game_controller.instance.UpdateSettings();
        UpdatePaytable();
        PaytableObject.SetActive(true);
    }
    private void Close(button_handler button)
    {
        PaytableObject.SetActive(false);
    }
}
