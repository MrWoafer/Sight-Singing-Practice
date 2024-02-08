using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;

public enum TickType
{
    none,
    tick,
    cross
}

public class StaveGUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float playScaleBPM = 140f;

    [Header("Managers")]
    [SerializeField]
    private Games gamesManager;
    [SerializeField]
    private AudioManager audio;

    [Header("References")]
    [SerializeField]
    private TMP_Dropdown clefDropdown;
    [SerializeField]
    private TMP_Dropdown keyDropdown;
    [SerializeField]
    private TMP_Dropdown modeDropdown;
    [SerializeField]
    private Button keyRandomiseButton;
    [SerializeField]
    private Button modeRandomiseButton;
    [SerializeField]
    private TMP_Dropdown timeDropdown;
    [SerializeField]
    private TMP_Dropdown gamesDropdown;
    [SerializeField]
    private Slider bpmSlider;
    [SerializeField]
    private TextMeshProUGUI bpmSliderText;
    [SerializeField]
    private TextMeshProUGUI typingArea;
    [SerializeField]
    private GameObject tickImg;
    [SerializeField]
    private GameObject crossImg;
    [SerializeField]
    private Stave stave;
    [SerializeField]
    private GameObject nameKeySignaturesOptions;
    [SerializeField]
    private TMP_Dropdown nameKeySignaturesModeDropdown;
    [SerializeField]
    private GameObject nameIntervalsOptions;
    [SerializeField]
    private GameObject singIntervalsOptions;
    [SerializeField]
    private TMP_Dropdown singIntervalsDirectionDropdown;
    [SerializeField]
    private GameObject singMelodyOptions;
    [SerializeField]
    private TMP_Dropdown singMelodyDifficultyDropdown;
    [SerializeField]
    private TMP_Dropdown singMelodyRhythmDifficultyDropdown;

    private NoteName[] keys = MusicTheory.circleOfKeys;
    private Clef[] clefs = new Clef[] { Clef.treble, Clef.alto, Clef.bass };
    private TimeSignature[] times = new TimeSignature[] { TimeSignature._4_4, TimeSignature._3_4, TimeSignature._2_4, TimeSignature._5_4, TimeSignature._6_8, TimeSignature._9_8,
                                                          TimeSignature._12_8 };

    private Game[] games = new Game[] { Game.none, Game.nameKeySignatures, Game.nameNotes, Game.nameScaleDegrees, Game.plotScaleDegrees, Game.nameIntervals, Game.singIntervals,
                                        Game.singMelody, Game.transcribeMelody };

    private Mode[] modes = new Mode[] { Mode.major, Mode.minor };

    private float[] bpmOptions = new float[] { 40f, 60f, 80f, 100f, 120f, 140f, 160f, 180f };

    // Start is called before the first frame update
    void Start()
    {
        List<string> keyList = new List<string>();
        foreach(NoteName key in keys)
        {
            keyList.Add(key + " / " + new Key(key, Mode.major).relativeMinor.tonic + "m");
        }
        keyDropdown.ClearOptions();
        keyDropdown.AddOptions(keyList);
        keyDropdown.value = Array.IndexOf(keys, stave.key.tonic);

        List<string> modeList = new List<string>();
        foreach (Mode mode in modes)
        {
            modeList.Add(Functions.ToUpperFirstLetter(mode.ToString()));
        }
        modeDropdown.ClearOptions();
        modeDropdown.AddOptions(modeList);
        modeDropdown.value = Array.IndexOf(modes, stave.key.mode);

        List<string> clefList = new List<string>();
        foreach (Clef clef in clefs)
        {
            clefList.Add(Functions.ToUpperFirstLetter(clef.ToString()));
        }
        clefDropdown.ClearOptions();
        clefDropdown.AddOptions(clefList);
        clefDropdown.value = Array.IndexOf(clefs, stave.clef);

        List<string> timeList = new List<string>();
        foreach (TimeSignature time in times)
        {
            timeList.Add(time.ToString());
        }
        timeDropdown.ClearOptions();
        timeDropdown.AddOptions(timeList);
        timeDropdown.value = Array.IndexOf(times, stave.time);

        List<string> gameList = new List<string>();
        foreach (Game game in games)
        {
            gameList.Add(gamesManager.GetGameString(game));
        }
        gamesDropdown.ClearOptions();
        gamesDropdown.AddOptions(gameList);
        gamesDropdown.value = 0;

        bpmSlider.maxValue = bpmOptions.Length - 1;
        bpmSlider.value = Array.IndexOf(bpmOptions, gamesManager.playbackBPM);

        HideOptionBoxes();
        SetTypingText("");
        SetTick(TickType.none);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectKey(int value)
    {
        stave.SetKey(new Key(keys[value], Mode.major));
    }
    public void RandomiseKey()
    {
        stave.SetKey(new Key(keys[UnityEngine.Random.Range(0, keys.Length)], Mode.major));
    }
    public void SetKeyDropdown(Key key)
    {
        keyDropdown.SetValueWithoutNotify(Array.IndexOf(keys, key.relativeMajor.tonic));
    }

    public void SelectMode(int value)
    {
        stave.SetKey(stave.key.Relative(modes[value]));
    }
    public void RandomiseMode()
    {
        modeDropdown.value = UnityEngine.Random.Range(0, modes.Length);
        SelectMode(modeDropdown.value);
    }
    public void SetModeDropdown(Mode mode)
    {
        modeDropdown.SetValueWithoutNotify(Array.IndexOf(modes, mode));
    }

    public void SelectClef(int value)
    {
        stave.SetClef(clefs[value]);
    }
    public void RandomiseClef()
    {
        stave.SetClef(clefs[UnityEngine.Random.Range(0, clefs.Length)]);
    }
    public void SetClefDropdown(Clef clef)
    {
        clefDropdown.SetValueWithoutNotify(Array.IndexOf(clefs, clef));
    }

    public void SelectTime(int value)
    {
        stave.SetTime(times[value]);
    }
    public void RandomiseTime()
    {
        stave.SetTime(times[UnityEngine.Random.Range(0, times.Length)]);
    }
    public void SetTimeDropdown(TimeSignature time)
    {
        timeDropdown.SetValueWithoutNotify(Array.IndexOf(times, time));
    }

    public void SetGame(int value)
    {
        gamesManager.SetGame(games[value]);
    }
    public void RandomiseGame()
    {
        gamesManager.SetGame(games[UnityEngine.Random.Range(1, games.Length)]);
    }
    public void SetGameDropdown(Game game)
    {
        gamesDropdown.SetValueWithoutNotify(Array.IndexOf(games, game));

        if (game == Game.nameKeySignatures)
        {
            keyDropdown.interactable = false;
            keyRandomiseButton.interactable = false;
            modeDropdown.interactable = false;
            modeRandomiseButton.interactable = false;
        }
        else
        {
            keyDropdown.interactable = true;
            keyRandomiseButton.interactable = true;
            modeDropdown.interactable = true;
            modeRandomiseButton.interactable = true;
        }

        DisplayOptionsBox(game);
        SetTick(TickType.none);
    }

    public void SetTypingText(string text)
    {
        typingArea.text = text;
    }

    public void SetTick(TickType tickType)
    {
        if (tickType == TickType.tick)
        {
            tickImg.SetActive(true);
            crossImg.SetActive(false);
        }
        else if (tickType == TickType.cross)
        {
            tickImg.SetActive(false);
            crossImg.SetActive(true);
        }
        else if (tickType == TickType.none)
        {
            tickImg.SetActive(false);
            crossImg.SetActive(false);
        }
        else
        {
            throw new Exception("Unknown tick type: " + tickType);
        }
    }

    private void DisplayOptionsBox(Game game)
    {
        HideOptionBoxes();

        if (game == Game.nameKeySignatures)
        {
            nameKeySignaturesOptions.SetActive(true);
        }
        else if (game == Game.nameIntervals)
        {
            nameIntervalsOptions.SetActive(true);
        }
        else if (game == Game.singIntervals)
        {
            singIntervalsOptions.SetActive(true);
        }
        else if (game == Game.singMelody)
        {
            singMelodyOptions.SetActive(true);
        }
    }

    private void HideOptionBoxes()
    {
        nameKeySignaturesOptions.SetActive(false);
        nameIntervalsOptions.SetActive(false);
        singIntervalsOptions.SetActive(false);
        singMelodyOptions.SetActive(false);
    }

    public void SetNameKeySignaturesMode(int value)
    {
        gamesManager.SetNameKeySignaturesMode(nameKeySignaturesModeDropdown.options[value].text);
    }

    public void SetNameKeySignaturesMode(bool value)
    {
        gamesManager.SetNameIntervalsIncludeQuality(value);
    }

    public void PlayKeyScale()
    {
        audio.PlayPitches(stave.key.Scale(stave.key.tonic < NoteName.A ? 4 : 3, Duration.eighth), playScaleBPM, TimeSignature._4_4);
    }

    public void SetSingIntervalsDirection(int value)
    {
        gamesManager.SetSingIntervalsDirection(singIntervalsDirectionDropdown.options[value].text);
    }

    public void SetSingMelodyDifficulty(int value)
    {
        gamesManager.SetSingMelodyDifficulty(singMelodyDifficultyDropdown.options[value].text);
    }

    public void SetSingMelodyRhythmDifficulty(int value)
    {
        gamesManager.SetSingMelodyRhythmDifficulty(singMelodyRhythmDifficultyDropdown.options[value].text);
    }

    public void SetPlaybackBPM(float value)
    {
        SetPlaybackBPM(Mathf.FloorToInt(value));
    }
    public void SetPlaybackBPM(int value)
    {
        bpmSliderText.text = bpmOptions[value].ToString() + " BPM";
        gamesManager.SetPlaybackBPM(bpmOptions[value]);
    }
}
