using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class game_controller : MonoBehaviour
{
    private static game_controller _instance;
    public static game_controller instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<game_controller>();
            return _instance;
        }
    }
    [System.Serializable]
    public class SlotSettingsClass
    {
        [System.Serializable]
        public class ItemClass
        {
            public float Value;
            public int GettingChance;
        }
        [System.Serializable]
        public class CombinationClass
        {
            public int[] numbers;
        }
        public string GameName;
        public player_controller.PlayerDataClass PlayerDefault;

        public int WinChance;

        public int SpinSpeed;
        public float SpinTime;
        public float SpinAcceleration;
        public float EndDelay;

        public List<int> BetsList;

        public List<ItemClass> Items;
        public List<CombinationClass> Combinations;
    }
    public SlotSettingsClass SlotSettings;
    public Sprite[] itemsList;
    [System.Serializable]
    public class SpinDataClass
    {
        [System.Serializable]
        public class ComboLineClass
        {
            public int ID;
            public int Length;
            public float winAmount;
        }
        public int[,] itemsData;
        public List<ComboLineClass> Combinations;
    }
    [System.Serializable]
    public class ReelClass
    {
        [System.Serializable]
        public class CellClass
        {
            public int SpriteID;
            public Image Image;
            public RectTransform transform;
        }
        public List<CellClass> Cells;
        public float CurrentSpeed;
        public float WantedSpeed;
        public bool isStop;
    }
    public List<ReelClass> SlotReels;
    public SpinDataClass CurrentSpin;
    public string DataUrl;

    public button_handler b_spin, b_bet;
    public Text GameName;
    public Image CellPrefab;
    public Transform SlotsHolder;
    public line[] line;

    private int HorizontalOffset = 300;
    private int VerticalOffset = 275;

    private bool canPlay = true;
    private bool isRun = false;
    private bool isStop = false;
    private bool isStopped = false;
    private float SpinTimer;
    public GameObject NetworkNeeded;
    public AudioSource audioComponent, reelAudio;
    public AudioClip SpinSound, ComboSound, ComboFailSound;


    private void Awake()
    {
        b_spin.OnClick += Spin;
        b_bet.OnClick += Bet;

        if(Application.internetReachability == NetworkReachability.NotReachable && (PlayerPrefs.GetString("GameSettings") == "" || PlayerPrefs.GetString("GameSettings") == null))
        {
            NetworkNeeded.SetActive(true);
            return;
        }

        UpdateSettings();
        BuildPaytable();
    }

    private void Update()
    {
        if (isRun)
        {
            for(int x = 0; x<SlotReels.Count; x++)
            {
                for (int i = 0; i < SlotReels[x].Cells.Count; i++)
                {
                    SlotReels[x].Cells[i].transform.anchoredPosition += new Vector2(0, -SlotReels[x].CurrentSpeed * Time.deltaTime);
                    if (SlotReels[x].Cells[i].transform.anchoredPosition.y <= -1375)
                        CellMoved(x, i);
                }


                if (SlotReels[x].isStop)
                    SlotReels[x].CurrentSpeed = SlotReels[x].Cells[1].transform.anchoredPosition.y * (SlotSettings.SpinSpeed/400);
                else
                    SlotReels[x].CurrentSpeed = Mathf.Lerp(SlotReels[x].CurrentSpeed, SlotReels[x].WantedSpeed, SlotSettings.SpinAcceleration * Time.deltaTime);


                if (canPlay)
                    return;
                SpinTimer += Time.deltaTime;

                if (SpinTimer >= SlotSettings.SpinTime+SlotSettings.EndDelay && !isStopped)
                {
                    isStopped = true;
                    Combination();
                }
                    if (!isStop)
                {
                    if (SpinTimer >= SlotSettings.SpinTime)
                    {
                        isStop = true;
                    }
                }

                
            }
        }
    }
    private void Combination()
    {
        reelAudio.Stop();
        if (CurrentSpin.Combinations.Count > 0)
            StartCoroutine(showCombo(1, 0));
        else
        {
            audioComponent.clip = ComboFailSound;
            audioComponent.loop = false;
            audioComponent.Play();
            End();
        }

    }
    IEnumerator showCombo(float time, int comboNum)
    {
        audioComponent.clip = ComboSound;
        audioComponent.loop = false;
        audioComponent.Play();

        int lineID = Random.Range(0, line.Length);

        line[lineID].gameObject.SetActive(true);

        List<Transform> points = new List<Transform>();
        for (int x = 4; x >= (5 - CurrentSpin.Combinations[comboNum].Length); x--)
            points.Add(SlotReels[x].Cells[SlotSettings.Combinations[CurrentSpin.Combinations[comboNum].ID].numbers[x]].Image.transform);
        line[lineID].SetPositions(points);

        player_controller.instance.AddWin(CurrentSpin.Combinations[comboNum].winAmount);
        player_controller.instance.Add(crypt.data.Encrypt("" + CurrentSpin.Combinations[comboNum].winAmount));

        yield return new WaitForSeconds(time);
        line[lineID].SetPositions(null);

        line[lineID].gameObject.SetActive(false);

        if (CurrentSpin.Combinations.Count - 1 > comboNum)
            StartCoroutine(showCombo(1, comboNum+1));
        else
            End();
    }
    private void End()
    {
        for (int x = 0; x < SlotReels.Count; x++)
        {
            SlotReels[x].WantedSpeed = 0;
            SlotReels[x].isStop = false;
        }
        player_controller.instance.RemoveWin();
        isRun = true;
        isStop = false;
        canPlay = true;
        isStopped = false;
        SpinTimer = 0;
        UpdateSettings();
    }
    public void UpdateSettings()
    {
        string json = "";
        try
        {
            json = new WebClient().DownloadString(DataUrl);
            SlotSettings = JsonUtility.FromJson<SlotSettingsClass>(json);
            string crypted = crypt.data.Encrypt(json);
            //Debug.Log(crypted);
            PlayerPrefs.SetString("GameSettings", crypted);
            PlayerPrefs.Save();
        }
        catch
        {
            string crypted = PlayerPrefs.GetString("GameSettings");
            //Debug.Log(crypted);
            SlotSettings = JsonUtility.FromJson<SlotSettingsClass>(crypt.data.Decrypt(crypted));
        }
        paytable_controller.instance.UpdatePaytable();
        GameName.text = SlotSettings.GameName;
    }

    private void Spin(button_handler button)
    {
        if (canPlay)
        {
            if (player_controller.instance.Reduce(crypt.data.Encrypt("" + SlotSettings.BetsList[player_controller.instance.bet])))
            {
                reelAudio.clip = SpinSound;
                reelAudio.loop = true;
                reelAudio.Play();
                CurrentSpin = GetSpinData();
                for (int x = 0; x < SlotReels.Count; x++)
                    SlotReels[x].WantedSpeed = SlotSettings.SpinSpeed;
                canPlay = false;
                isRun = true;
                SpinTimer = 0;
            }
        }
    }
    private void Bet(button_handler button)
    {
        player_controller.instance.bet++;
        if (player_controller.instance.bet == SlotSettings.BetsList.Count)
            player_controller.instance.bet = 0;
        SoftCount.instance.Soft(SlotSettings.BetsList[player_controller.instance.bet], player_controller.instance.BetText, "$", "");
    }

    private SpinDataClass GetSpinData()
    {
        SpinDataClass data = new SpinDataClass();
        data.Combinations = new List<SpinDataClass.ComboLineClass>();
        bool win = isWin();
        data.itemsData = new int[5, 3];

        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 3; y++)
                data.itemsData[x, y] = GetRandomID();

        if (win)
        {
            int[] comboLine = GetComboLine();

            for (int i = 0; i < comboLine.Length; i++)
            {
                int comboLength = GetComboLength();
                int winCell = GetRandomID();
                for (int x = 4; x >= (5 - comboLength); x--)
                    data.itemsData[x, SlotSettings.Combinations[comboLine[i]].numbers[x]] = winCell;
            }
        }

        for (int i = 0; i < SlotSettings.Combinations.Count; i++)
        {
            int id = data.itemsData[4, SlotSettings.Combinations[i].numbers[4]];
            SpinDataClass.ComboLineClass combo = new SpinDataClass.ComboLineClass();
            combo.ID = i;
            for (int x = 4; x >= 0; x--)
            {
               if(data.itemsData[x, SlotSettings.Combinations[i].numbers[x]] != id)
                    break;
                combo.Length = 5 - x;
                combo.winAmount = combo.Length * SlotSettings.Items[id].Value * (SlotSettings.BetsList[player_controller.instance.bet]/5);
            }
            if (combo.Length >= 3)
                data.Combinations.Add(combo);
        }

        return data;
    }
    private bool isWin()
    {
        int win = Random.Range(0, 101);
        if (win < SlotSettings.WinChance)
            return true;
        else
            return false;
    }
    private int GetComboLength()
    {
        return Random.Range(3, 6);
    }
    private int[] GetComboLine()
    {
        int amount = Random.Range(1, 5);
        int[] lines = new int[amount];
        for(int i = 0; i<lines.Length; i++)
        {
            lines[i] = Random.Range(0, SlotSettings.Combinations.Count);
        }
        return lines;
    }

    private void BuildPaytable()
    {
        SlotReels = new List<ReelClass>();
        for (int x = 0; x < 5; x++)
        {
            ReelClass reel = new ReelClass();
            reel.Cells = new List<ReelClass.CellClass>();
            for (int y = 0; y < 9; y++)
            {
                ReelClass.CellClass cell = new ReelClass.CellClass();
                cell.Image = SlotsHolder.GetChild((x*9)+y).GetComponent<Image>();
                cell.transform = cell.Image.GetComponent<RectTransform>();
                cell.SpriteID = GetRandomID();


                cell.Image.sprite = itemsList[cell.SpriteID];
                reel.Cells.Add(cell);

            }
            SlotReels.Add(reel);
        }
    }
    private int GetRandomID()
    {
        int totalChance = 0;
        for (int i = 0; i < SlotSettings.Items.Count; i++)
            totalChance += SlotSettings.Items[i].GettingChance;
        
        int winNum = Random.Range(0, totalChance);

        int curNum = 0;
        
        for (int i = 0; i < SlotSettings.Items.Count; i++)
        {
            curNum += SlotSettings.Items[i].GettingChance;
            if (curNum >= winNum)
                return i;
        }

            return 0;
    }
    private Vector2 GetWantedPosition(int x, int y)
    {
        Vector2 postion = new Vector2();
        postion.x = (x - (5 / 2)) * HorizontalOffset;
        postion.y = (y - (3 / 2)) * VerticalOffset;
        return postion;
    }
    private void CellMoved(int x, int y)
    {
        SlotReels[x].Cells[y].transform.anchoredPosition += new Vector2(0, SlotReels[x].Cells.Count * VerticalOffset);


        if (!isStop)
            SlotReels[x].Cells[y].SpriteID = GetRandomID();
        else
        {

            if (y == 8)
                SlotReels[x].isStop = true;
        }

        if (y == 0 || y == 1 || y == 2)
            SlotReels[x].Cells[y].SpriteID = CurrentSpin.itemsData[x,y];
        SlotReels[x].Cells[y].Image.sprite = itemsList[SlotReels[x].Cells[y].SpriteID];
        //
    }
}