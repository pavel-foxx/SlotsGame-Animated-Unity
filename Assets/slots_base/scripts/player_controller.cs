using System;
using UnityEngine;
using UnityEngine.UI;

public class player_controller : MonoBehaviour
{
    private static player_controller _instance;
    public static player_controller instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<player_controller>();
            return _instance;
        }
    }
    [System.Serializable]
    public class PlayerDataClass
    {
        public float balance;
    }

    private PlayerDataClass PlayerData;
    public Text BalanceText, WinText, BetText;
    public float win;
        public int bet;

    private void Awake()
    {

    }

    private void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && (PlayerPrefs.GetString("PlayerData") == "" || PlayerPrefs.GetString("PlayerData") == null))
        {
            return;
        }
        LoadData();

        SoftCount.instance.Soft(PlayerData.balance, BalanceText, "$", "");
        SoftCount.instance.Soft(win, WinText, "$", "");
        SoftCount.instance.Soft(game_controller.instance.SlotSettings.BetsList[bet], BetText, "$", "");
    }

    public bool Reduce(string crypted)
    {
        float count;
        if (!float.TryParse(crypt.data.Decrypt(crypted), out count))
            return false;

        if (count > PlayerData.balance)
            return false;
        PlayerData.balance -= count;
        SoftCount.instance.Soft(PlayerData.balance, BalanceText, "$", "");

        SaveData();
        return true;
    }

    public void Add(string crypted)
    {
        float count;
        if (!float.TryParse(crypt.data.Decrypt(crypted), out count))
            return;

        PlayerData.balance += count;
        SoftCount.instance.Soft(PlayerData.balance, BalanceText, "$", "");

        SaveData();
    }

    public void AddWin(float count)
    {
        win += count;
        WinText.text = "$" + win;
    }
    public void RemoveWin()
    {
        win = 0;
        WinText.text = "$0";
    }

    private void GetDefaultData()
    {
        PlayerData = game_controller.instance.SlotSettings.PlayerDefault;
        string json = JsonUtility.ToJson(PlayerData);
        PlayerPrefs.SetString("PlayerData", crypt.data.Encrypt(json));
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(PlayerData);
        PlayerPrefs.SetString("PlayerData", crypt.data.Encrypt(json));
    }

    private void LoadData()
    {
        string protect = PlayerPrefs.GetString("PlayerData", "");
        if (protect == "" || protect == null)
        {
            Debug.Log("get def");
            GetDefaultData();
        }
        else
        {
            string json = crypt.data.Decrypt(protect);
            if (json != null)
                PlayerData = JsonUtility.FromJson<PlayerDataClass>(json);
            else
                GetDefaultData();
        }
    }
}
