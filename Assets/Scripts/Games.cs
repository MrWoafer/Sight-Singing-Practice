using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Game
{
    none,
    nameKeySignatures,
    nameNotes,
    nameScaleDegrees,
    plotScaleDegrees,
    nameIntervals,
    singIntervals,
    singMelody,
    transcribeMelody
}

public enum TypingMode
{
    noteLetter,
    scaleDegree,
    interval
}

public class Games : MonoBehaviour
{
    [Header("Settings")]
    public Color nextNoteColour = Color.blue;
    public float playbackBPM = 120f;

    [Header("References")]
    [SerializeField]
    private Stave stave;
    [SerializeField]
    private StaveGUI gui;
    [SerializeField]
    private AudioManager audio;

    public Game game
    {
        get; private set;
    }

    private TypingMode typingMode = TypingMode.noteLetter;
    private bool typing = false;
    private string typingStr = "";
    private string displayTypingStr = "";
    private bool typingIntervalIncludeQuality = false;

    private Note noteToGuess = Note.C4;
    private Note noteToGuess2 = Note.C4;
    private Note noteGuessed = Note.C4;

    private bool revealedAnswer = false;

    private string nameKeySignatureMode = "major";
    private string nameKeySignatureModeThisRound = "major";

    private bool nameIntervalsIncludeQuality = false;

    private string singIntervalsDirection = "ascending";
    private string singIntervalsDirectionThisRound = "ascending";
    private bool singIntervalsFromTonic = true;
    private bool singIntervalsPlayStartingNote = true;

    private List<NoteWithDuration> melody = new List<NoteWithDuration>();
    private List<NoteWithDuration> guessedMelody = new List<NoteWithDuration>();

    private int singMelodyLength = 8;
    private bool singMelodyPlayStartingNote = true;
    private bool singMelodyStartFromTonic = true;
    private bool singMelodyShowScaleDegrees = false;
    private string singMelodyDifficulty = "easy";
    private string singMelodyRhythmDifficulty = "easy";

    private Duration selectedDuration = Duration.crotchet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (game == Game.nameKeySignatures)
            {
                NextRound();
            }
            else if (game == Game.nameNotes)
            {
                NextRound();
            }
            else if (game == Game.nameScaleDegrees)
            {
                NextRound();
            }
            else if (game == Game.plotScaleDegrees)
            {
                NextRound();
            }
            else if (game == Game.nameIntervals)
            {
                NextRound();
            }
            else if (game == Game.singIntervals)
            {
                if (revealedAnswer)
                {
                    revealedAnswer = false;
                    audio.StopAll();
                    NextRound();
                    if (singIntervalsPlayStartingNote)
                    {
                        audio.PlayPitchForSeconds(noteToGuess + Interval.octave * OctaveBoost(stave.clef), 1f);
                    }
                }
                else
                {
                    revealedAnswer = true;
                    audio.PlayPitchForSeconds(noteToGuess2 + Interval.octave * OctaveBoost(stave.clef), 1f);
                }
            }
            else if (game == Game.singMelody)
            {
                if (revealedAnswer)
                {
                    revealedAnswer = false;
                    audio.StopAll();
                    NextRound();
                    if (singMelodyPlayStartingNote)
                    {
                        audio.PlayPitchForSeconds(melody[0] + Interval.octave * OctaveBoost(stave.clef), 1f);
                    }
                }
                else
                {
                    revealedAnswer = true;
                    audio.PlayPitches(AddInterval(melody, Interval.octave * OctaveBoost(stave.clef)), playbackBPM, stave.time);
                }
            }
            else if (game == Game.transcribeMelody)
            {
                if (revealedAnswer)
                {
                    revealedAnswer = false;
                    audio.StopAll();
                    NextRound();
                    audio.PlayPitches(AddInterval(melody, Interval.octave * OctaveBoost(stave.clef)), playbackBPM, stave.time);
                }
                else
                {
                    revealedAnswer = true;
                    //audio.PlayPitches(AddInterval(guessedMelody, Interval.octave * OctaveBoost(stave.clef)), playbackBPM, stave.time);
                    CheckAnswer();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (game == Game.nameKeySignatures)
            {
                CheckAnswer();
                typingStr = "";
            }
            else if (game == Game.nameNotes)
            {
                CheckAnswer();
                typingStr = "";
            }
            else if (game == Game.nameScaleDegrees)
            {
                CheckAnswer();
                typingStr = "";
            }
            else if (game == Game.nameIntervals)
            {
                CheckAnswer();
                typingStr = "";
            }
            else if (game == Game.singIntervals)
            {
                if (revealedAnswer)
                {
                    audio.PlayPitchForSeconds(noteToGuess2 + Interval.octave * OctaveBoost(stave.clef), 1f);
                }
                else
                {
                    audio.PlayPitchForSeconds(noteToGuess + Interval.octave * OctaveBoost(stave.clef), 1f);
                }
            }
            else if (game == Game.singMelody)
            {
                if (revealedAnswer)
                {
                    audio.PlayPitches(AddInterval(melody, Interval.octave * OctaveBoost(stave.clef)), playbackBPM, stave.time);
                }
                else
                {
                    audio.PlayPitchForSeconds(melody[0] + Interval.octave * OctaveBoost(stave.clef), 1f);
                }
            }
            else if (game == Game.transcribeMelody)
            {
                audio.PlayPitches(AddInterval(melody, Interval.octave * OctaveBoost(stave.clef)), playbackBPM, stave.time);
            }
        }

        if (typing)
        {
            if (typingMode == TypingMode.noteLetter)
            {
                if (Input.GetKeyDown(KeyCode.Backspace) && typingStr.Length > 0)
                {
                    typingStr = typingStr.Remove(typingStr.Length - 1);
                }

                else if (Input.GetKeyDown(KeyCode.A) && typingStr.Length == 0)
                {
                    typingStr += "A";
                }
                else if (Input.GetKeyDown(KeyCode.B) && typingStr.Length == 0)
                {
                    typingStr += "B";
                }
                else if (Input.GetKeyDown(KeyCode.C) && typingStr.Length == 0)
                {
                    typingStr += "C";
                }
                else if (Input.GetKeyDown(KeyCode.D) && typingStr.Length == 0)
                {
                    typingStr += "D";
                }
                else if (Input.GetKeyDown(KeyCode.E) && typingStr.Length == 0)
                {
                    typingStr += "E";
                }
                else if (Input.GetKeyDown(KeyCode.F) && typingStr.Length == 0)
                {
                    typingStr += "F";
                }
                else if (Input.GetKeyDown(KeyCode.G) && typingStr.Length == 0)
                {
                    typingStr += "G";
                }

                else if (Input.GetKeyDown(KeyCode.B) && typingStr.Length == 1)
                {
                    typingStr += "b";
                }
                /// Unity uses the US keyboard layout, so the UK Hashtag key is actually the US Quote key
                else if (Input.GetKeyDown(KeyCode.Quote) && typingStr.Length == 1)
                {
                    typingStr += "#";
                }
            }
            else if (typingMode == TypingMode.scaleDegree)
            {
                if (Input.GetKeyDown(KeyCode.Backspace) && typingStr.Length > 0)
                {
                    typingStr = typingStr.Remove(typingStr.Length - 1);
                }

                if (typingStr.Length == 0 || (typingStr.Length > 0 && new char[] { 'b', '#', 'x' }.Contains(typingStr[typingStr.Length - 1])))
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        typingStr += "1";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        typingStr += "2";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        typingStr += "3";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        typingStr += "4";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        typingStr += "5";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha6))
                    {
                        typingStr += "6";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha7))
                    {
                        typingStr += "7";
                    }
                }

                if (Input.GetKeyDown(KeyCode.B))
                {
                    if (typingStr.Length == 0 || (typingStr.Length == 1 && typingStr[0] == 'b'))
                    {
                        typingStr += "b";
                    }
                }
                /// Unity uses the US keyboard layout, so the UK Hashtag key is actually the US Quote key
                else if (Input.GetKeyDown(KeyCode.Quote))
                {
                    if (typingStr.Length == 0)
                    {
                        typingStr += "#";
                    }
                    else if(typingStr.Length == 1 && typingStr[0] == '#')
                    {
                        typingStr = "x";
                    }
                }
            }
            else if (typingMode == TypingMode.interval)
            {
                if (Input.GetKeyDown(KeyCode.Backspace) && typingStr.Length > 0)
                {
                    typingStr = typingStr.Remove(typingStr.Length - 1);
                }

                if (typingIntervalIncludeQuality)
                {
                    if (typingStr.Length == 0 || (typingStr.Length > 0 && new char[] { 'd', 'A', 'P', 'm', 'M' }.Contains(typingStr[typingStr.Length - 1])))
                    {
                        if (Input.GetKeyDown(KeyCode.Alpha0) && new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' }.Contains(typingStr[typingStr.Length - 1]))
                        {
                            typingStr += "0";
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha1))
                        {
                            typingStr += "1";
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha2))
                        {
                            typingStr += "2";
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha3))
                        {
                            typingStr += "3";
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha4))
                        {
                            typingStr += "4";
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha5))
                        {
                            typingStr += "5";
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha6))
                        {
                            typingStr += "6";
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha7))
                        {
                            typingStr += "7";
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha8))
                        {
                            typingStr += "8";
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha9))
                        {
                            typingStr += "9";
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        if (typingStr.Length == 0 || (typingStr.Length == 1 && typingStr[0] == 'd'))
                        {
                            typingStr += "d";
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.A))
                    {
                        if (typingStr.Length == 0 || (typingStr.Length == 1 && typingStr[0] == 'A'))
                        {
                            typingStr += "A";
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.P))
                    {
                        if (typingStr.Length == 0)
                        {
                            typingStr += "P";
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.M))
                    {
                        if (typingStr.Length == 0)
                        {
                            typingStr += "m";
                        }
                        else if (typingStr.Length == 1 && typingStr[0] == 'm')
                        {
                            typingStr = "M";
                        }
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0) && typingStr.Length > 0)
                    {
                        typingStr += "0";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        typingStr += "1";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        typingStr += "2";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        typingStr += "3";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        typingStr += "4";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        typingStr += "5";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha6))
                    {
                        typingStr += "6";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha7))
                    {
                        typingStr += "7";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha8))
                    {
                        typingStr += "8";
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha9))
                    {
                        typingStr += "9";
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (game == Game.plotScaleDegrees)
            {
                int lineNumber = GetLineMouseOn();
                noteGuessed = stave.system1.LineNumberToNote(lineNumber);

                if (Mathf.Abs(lineNumber) <= 8)
                {
                    CheckAnswer();
                }
            }
            else if (game == Game.transcribeMelody)
            {
                int lineNumber = GetLineMouseOn();
                Note note = stave.system1.LineNumberToNote(lineNumber);

                if (Mathf.Abs(lineNumber) <= 8)
                {
                    guessedMelody.Add(note.SetDuration(selectedDuration));
                }
            }
        }

        if (game == Game.plotScaleDegrees)
        {
            stave.ClearNotes();

            int lineNumber = GetLineMouseOn();
            Note note = stave.system1.LineNumberToNote(lineNumber);

            if (Mathf.Abs(lineNumber) <= 8)
            {
                stave.PlotNoteCentre(note.SetDuration(Duration.whole));
            }
        }
        else if (game == Game.transcribeMelody)
        {
            if (Input.GetMouseButtonDown(2))
            {
                //Debug.Log(stave.BeatsAtPosition(guessedMelody.ToArray(), guessedMelody.Count - 1, stave.time));
            }
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1))
            {
                if (guessedMelody.Count > 0)
                {
                    guessedMelody.RemoveAt(guessedMelody.Count - 1);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectedDuration = Duration.sixteenth;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectedDuration = Duration.eighth;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                selectedDuration = Duration.quarter;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                selectedDuration = Duration.half;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                selectedDuration = Duration.whole;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                selectedDuration = Duration.doubleWhole;
            }
            else if (Input.GetKeyDown(KeyCode.Period))
            {
                if (selectedDuration.isDotted)
                {
                    selectedDuration = selectedDuration.MakeUndotted();
                }
                else
                {
                    selectedDuration = selectedDuration.MakeDotted();
                }
            }

            stave.ClearNotes();

            int lineNumber = GetLineMouseOn();
            Note note = stave.system1.LineNumberToNote(lineNumber);
            List<NoteWithDuration> tempMelody = new List<NoteWithDuration>(guessedMelody);

            if (Mathf.Abs(lineNumber) <= 8)
            {
                tempMelody.Add(note.SetDuration(selectedDuration));
            }

            stave.PlotNotes(tempMelody);
        }

        displayTypingStr = typingStr;
        if (game == Game.nameKeySignatures)
        {
            if (nameKeySignatureModeThisRound == "major")
            {
                displayTypingStr = (typingStr + " major").TrimStart();
            }
            else if (nameKeySignatureModeThisRound == "minor")
            {
                displayTypingStr = (typingStr + " minor").TrimStart();
            }
        }

        gui.SetTypingText(displayTypingStr);
    }

    public void SetGame(Game game)
    {
        this.game = game;
        gui.SetGameDropdown(game);
        stave.ClearNotes();

        typingStr = "";
        typing = false;

        revealedAnswer = false;

        melody = new List<NoteWithDuration>();
        guessedMelody = new List<NoteWithDuration>();

        if (game == Game.none)
        {

        }
        else if (game == Game.nameKeySignatures)
        {
            typing = true;
            typingMode = TypingMode.noteLetter;
            stave.SetNumOfSystems(1);
            stave.SetSystemScale(2f);
        }
        else if (game == Game.nameNotes)
        {
            typing = true;
            typingMode = TypingMode.noteLetter;
            stave.SetNumOfSystems(1);
            stave.SetSystemScale(2f);
        }
        else if (game == Game.nameScaleDegrees)
        {
            typing = true;
            typingMode = TypingMode.scaleDegree;
            stave.SetNumOfSystems(1);
            stave.SetSystemScale(2f);
        }
        else if (game == Game.plotScaleDegrees)
        {
            stave.SetNumOfSystems(1);
            stave.SetSystemScale(2f);
        }
        else if (game == Game.nameIntervals)
        {
            typing = true;
            typingMode = TypingMode.interval;
            SetNameIntervalsIncludeQuality(nameIntervalsIncludeQuality);

            stave.SetNumOfSystems(1);
            stave.SetSystemScale(2f);
        }
        else if (game == Game.singIntervals)
        {
            stave.SetNumOfSystems(1);
            stave.SetSystemScale(2f);
            revealedAnswer = true;
        }
        else if (game == Game.singMelody)
        {
            stave.SetNumOfSystems(1);
            stave.SetSystemScale(1.5f);
            revealedAnswer = true;
        }
        else if (game == Game.transcribeMelody)
        {
            stave.SetNumOfSystems(1);
            stave.SetSystemScale(1.5f);
            revealedAnswer = true;
        }
        else
        {
            throw new System.Exception("Unimplemented game: " + game);
        }
    }

    public string GetGameString(Game game)
    {
        switch (game)
        {
            case Game.none: return "None";
            case Game.nameKeySignatures: return "Name Key Signature";
            case Game.nameNotes: return "Name Note";
            case Game.nameScaleDegrees: return "Name Scale Degree";
            case Game.plotScaleDegrees: return "Plot Scale Degree";
            case Game.nameIntervals: return "Name Interval";
            case Game.singIntervals: return "Sing Interval";
            case Game.singMelody: return "Sing Melody";
            case Game.transcribeMelody: return "Transcribe Melody";
            default: throw new System.Exception("Unknown game: " + game);
        }
    }

    private void NextRound()
    {
        if (game == Game.nameKeySignatures)
        {
            NoteName tonic = stave.key.tonic;

            while (tonic == stave.key.tonic)
            {
                tonic = MusicTheory.circleOfKeys[UnityEngine.Random.Range(0, MusicTheory.circleOfKeys.Length)];
            }

            stave.SetKey(new Key(tonic, stave.key.mode));

            SetNameKeySignaturesMode(nameKeySignatureMode);

            switch (nameKeySignatureModeThisRound)
            {
                case "major": gui.SetModeDropdown(Mode.major); break;
                case "minor": gui.SetModeDropdown(Mode.minor); break;
                default: break;
            }
        }
        else if (game == Game.nameNotes)
        {
            Note newNote = noteToGuess;

            while (newNote == noteToGuess)
            {
                newNote = RandomNote(stave.key, stave.clef);
            }

            noteToGuess = newNote;
            //Debug.Log(noteToGuess);
            //audio.PlayPitch(noteToGuess, 1f);

            stave.ClearNotes();
            stave.PlotNoteCentre(noteToGuess.SetDuration(Duration.whole));
        }
        else if (game == Game.nameScaleDegrees)
        {
            Note newNote = noteToGuess;

            while (newNote == noteToGuess)
            {
                newNote = RandomNote(stave.key, stave.clef);
            }

            noteToGuess = newNote;

            stave.ClearNotes();
            stave.PlotNoteCentre(noteToGuess.SetDuration(Duration.whole));
        }
        else if (game == Game.plotScaleDegrees)
        {
            Note newNote = noteToGuess;

            while (stave.key.ScaleDegreeOf(newNote) == stave.key.ScaleDegreeOf(noteToGuess))
            {
                newNote = RandomNote(stave.key, stave.clef);
            }

            noteToGuess = newNote;

            stave.ClearNotes();

            typingStr = stave.key.ScaleDegreeOf(newNote).ToString();
        }
        else if (game == Game.nameIntervals)
        {
            Note newNote = noteToGuess;
            Note newNote2 = noteToGuess2;

            while (newNote == noteToGuess)
            {
                newNote = RandomNote(stave.key, stave.clef);
            }
            while (newNote2 == noteToGuess2 || newNote == newNote2 || Mathf.Abs((newNote - newNote2).number) > 8)
            {
                newNote2 = RandomNote(stave.key, stave.clef);
            }

            noteToGuess = newNote;
            noteToGuess2 = newNote2;
    
            stave.ClearNotes();
            stave.PlotNoteCentre(noteToGuess.SetDuration(Duration.whole));
            stave.PlotNoteCentre(noteToGuess2.SetDuration(Duration.whole));
        }
        else if (game == Game.singIntervals)
        {
            SetSingIntervalsDirection(singIntervalsDirection);

            Note newNote;
            Note newNote2;

            int attempts = 0;
            do
            {
                newNote = noteToGuess;
                newNote2 = noteToGuess2;

                if (singIntervalsFromTonic)
                {
                    if (singIntervalsDirectionThisRound == "ascending")
                    {
                        newNote = GetTonic(stave.key, stave.clef);
                    }
                    else
                    {
                        newNote = GetTonic(stave.key, stave.clef) + Interval.octave;
                    }
                }
                else
                {
                    while (newNote == noteToGuess)
                    {
                        newNote = RandomNote(stave.key, stave.clef);
                    }
                }
                /*while ((singIntervalsDirectionThisRound == "ascending" && newNote == GetTonic(stave.key, stave.clef) + Interval.octave) || (singIntervalsDirectionThisRound == "descending" &&
                    newNote == GetTonic(stave.key, stave.clef)))
                {
                    newNote = RandomNote(stave.key, stave.clef);
                }*/

                attempts = 0;
                while (attempts < 1000 && (newNote2 == noteToGuess2 || newNote == newNote2 || Mathf.Abs((newNote - newNote2).number) > 8 || (singIntervalsDirectionThisRound == "ascending" && newNote2 < newNote) ||
                    (singIntervalsDirectionThisRound == "descending" && newNote2 > newNote)))
                {
                    newNote2 = RandomNote(stave.key, stave.clef);
                    attempts++;
                }
            } while (attempts >= 1000);

            noteToGuess = newNote;
            noteToGuess2 = newNote2;

            stave.ClearNotes();
            stave.PlotNote(noteToGuess.SetDuration(Duration.whole), -0.4f);
            stave.PlotNote(noteToGuess2.SetDuration(Duration.whole), 0.4f, nextNoteColour);
        }
        else if (game == Game.singMelody)
        {
            melody = new List<NoteWithDuration>(CreateMelody(stave.key, stave.clef, singMelodyLength, singMelodyDifficulty, singMelodyRhythmDifficulty));

            stave.ClearNotes();
            stave.SetNumOfSystems(1);
            stave.PlotNotes(melody);
            if (singMelodyShowScaleDegrees)
            {
                stave.ShowNoteScaleDegrees(true);
            }
        }
        else if (game == Game.transcribeMelody)
        {
            stave.ClearNotes();
            stave.SetNumOfSystems(1);
            melody = new List<NoteWithDuration>(CreateMelody(stave.key, stave.clef, singMelodyLength, singMelodyDifficulty, singMelodyRhythmDifficulty));
            guessedMelody = new List<NoteWithDuration>();
        }
    }

    private bool CheckAnswer()
    {
        if (game == Game.nameKeySignatures)
        {
            if (nameKeySignatureModeThisRound == "major")
            {
                if (typingStr == stave.key.relativeMajor.tonic.ToString())
                {
                    gui.SetTick(TickType.tick);
                    NextRound();
                    return true;
                }
                else
                {
                    gui.SetTick(TickType.cross);
                    return false;
                }
            }
            else if (nameKeySignatureModeThisRound == "minor")
            {
                if (typingStr == stave.key.relativeMinor.tonic.ToString())
                {
                    gui.SetTick(TickType.tick);
                    NextRound();
                    return true;
                }
                else
                {
                    gui.SetTick(TickType.cross);
                    return false;
                }
            }
            else
            {
                throw new System.Exception("Unknown 'name key signatures' mode: " + nameKeySignatureMode);
            }
        }
        else if (game == Game.nameNotes)
        {
            if (typingStr == noteToGuess.noteName.ToString())
            {
                gui.SetTick(TickType.tick);
                NextRound();
                return true;
            }
            else
            {
                gui.SetTick(TickType.cross);
                return false;
            }
        }
        else if (game == Game.nameScaleDegrees)
        {
            if (typingStr == stave.key.ScaleDegreeOf(noteToGuess.noteName).ToString())
            {
                gui.SetTick(TickType.tick);
                NextRound();
                return true;
            }
            else
            {
                gui.SetTick(TickType.cross);
                return false;
            }
        }
        else if (game == Game.plotScaleDegrees)
        {
            if (stave.key.ScaleDegreeOf(noteToGuess) == stave.key.ScaleDegreeOf(noteGuessed))
            {
                gui.SetTick(TickType.tick);
                NextRound();
                return true;
            }
            else
            {
                gui.SetTick(TickType.cross);
                return false;
            }
        }
        else if (game == Game.nameIntervals)
        {
            if (nameIntervalsIncludeQuality)
            {
                if (typingStr == (noteToGuess - noteToGuess2).ToString() || typingStr == (noteToGuess2 - noteToGuess).ToString())
                {
                    gui.SetTick(TickType.tick);
                    NextRound();
                    return true;
                }
                else
                {
                    gui.SetTick(TickType.cross);
                    return false;
                }
            }
            else
            {
                if (typingStr == (noteToGuess - noteToGuess2).number.ToString() || typingStr == (noteToGuess2 - noteToGuess).number.ToString())
                {
                    gui.SetTick(TickType.tick);
                    NextRound();
                    return true;
                }
                else
                {
                    gui.SetTick(TickType.cross);
                    return false;
                }
            }
        }
        else if (game == Game.transcribeMelody)
        {
            if (melody.SequenceEqual(guessedMelody))
            {
                gui.SetTick(TickType.tick);
                return true;
            }
            else
            {
                gui.SetTick(TickType.cross);
                return false;
            }
        }
        else
        {
            throw new System.Exception("Unknown / unimplemented game: " + game);
        }
    }

    public Note RandomNote(Key key, Clef clef)
    {
        List<Note> notes = new List<Note>();

        /// A3-C6
        if (clef == Clef.treble)
        {
            for (int i = 1; i < 18; i++)
            {
                notes.Add(key.AddIntervalNumber(new Note(key[NoteLetter.A], 3), i));
            }
        }
        /// B2-D5
        else if (clef == Clef.alto)
        {
            for (int i = 1; i < 18; i++)
            {
                notes.Add(key.AddIntervalNumber(new Note(key[NoteLetter.B], 2), i));
            }
        }
        /// C2-E4
        else if (clef == Clef.bass)
        {
            for (int i = 1; i < 18; i++)
            {
                notes.Add(key.AddIntervalNumber(new Note(key[NoteLetter.C], 2), i));
            }
        }
        else
        {
            throw new System.Exception("Unimplemented clef: " + clef);
        }

        //Debug.Log(Functions.ArrayToString(notes.ToArray()));

        return notes[UnityEngine.Random.Range(0, notes.Count)];
    }

    public Note GetTonic(Key key, Clef clef)
    {
        switch (clef)
        {
            case Clef.treble: return new Note(key.tonic, 4);
            case Clef.alto: return new Note(key.tonic, key.tonic <= NoteName.C ? 5 : 4);
            case Clef.bass: return new Note(key.tonic, key.tonic <= NoteName.D ? 3 : 2);
            default: throw new System.Exception("Unimplemented clef: " + clef);
        }
    }

    public int OctaveBoost(Clef clef)
    {
        switch (clef)
        {
            case Clef.treble: return 0;
            case Clef.alto: return 1;
            case Clef.bass: return 2;
            default: throw new System.Exception("Unimplemented clef: " + clef);
        }
    }

    public void SetNameKeySignaturesMode(string mode)
    {
        nameKeySignatureMode = mode.ToLower();

        if (nameKeySignatureMode == "major")
        {
            nameKeySignatureModeThisRound = "major";
        }
        else if (nameKeySignatureMode == "minor")
        {
            nameKeySignatureModeThisRound = "minor";
        }
        else if (nameKeySignatureMode == "mixed")
        {
            nameKeySignatureModeThisRound = Functions.RandomBool() ? "major" : "minor";
        }
        else
        {
            throw new System.Exception("Unknown 'name key signature' mode: " + nameKeySignatureMode);
        }
    }

    public void SetNameIntervalsIncludeQuality(bool include)
    {
        nameIntervalsIncludeQuality = include;
        typingIntervalIncludeQuality = nameIntervalsIncludeQuality;
    }

    public void SetSingIntervalsDirection(string direction)
    {
        singIntervalsDirection = direction.ToLower();

        if (singIntervalsDirection == "ascending")
        {
            singIntervalsDirectionThisRound = "ascending";
        }
        else if (singIntervalsDirection == "descending")
        {
            singIntervalsDirectionThisRound = "descending";
        }
        else if (singIntervalsDirection == "mixed")
        {
            singIntervalsDirectionThisRound = Functions.RandomBool() ? "ascending" : "descending";
        }
        else
        {
            throw new System.Exception("Unknown 'sing intervals' mode: " + singIntervalsDirection);
        }
    }

    public void SetSingIntervalsFromTonic(bool fromTonic)
    {
        singIntervalsFromTonic = fromTonic;
    }

    public void SetSingIntervalsPlayStartingNote(bool playStartingNote)
    {
        singIntervalsPlayStartingNote = playStartingNote;
    }

    /// <summary>
    /// Randomly generate a melody in the given key for the given clef in the given time signature.
    /// </summary>
    /// <param name="key">The key of the melody.</param>
    /// <param name="clef">The clef the melody will be made for.</param>
    /// <param name="length">How many beats the melody will have.</param>
    /// <param name="pitchDifficulty">How difficult the intervals will be.</param>
    /// <param name="rhythmDifficulty">How difficult the rhythms will be.</param>
    /// <returns></returns>
    private NoteWithDuration[] CreateMelody(Key key, Clef clef, float length, string pitchDifficulty, string rhythmDifficulty)
    {
        List<float> beatLengths = new List<float>();

        if (rhythmDifficulty == "crotchets")
        {
            for (int i = 0; i < Mathf.FloorToInt(length); i++)
            {
                beatLengths.Add(1f);
            }
        }
        else if (rhythmDifficulty == "easy")
        {
            float beatRunningTotal = 0f;

            int attempts = 0;
            while (attempts < 10000 && beatRunningTotal < length)
            {
                attempts++;

                float[] nextBeatPattern = new float[0];
                switch (Functions.WeightedRand(new float[] { 1f, 0.5f, 0.5f, 0.25f }))
                {
                    case 0: nextBeatPattern = new float[] { 1f }; break;
                    case 1: nextBeatPattern = new float[] { 2f }; break;
                    case 2: nextBeatPattern = new float[] { 0.5f, 0.5f }; break;
                    case 3: nextBeatPattern = new float[] { 0.5f, 0.5f, 0.5f, 0.5f }; break;
                    default: throw new System.Exception("Unexpected switch value.");
                }

                if (beatRunningTotal + Enumerable.Sum(nextBeatPattern) <= length)
                {
                    attempts = 0;

                    for (int i = 0; i < nextBeatPattern.Length; i++)
                    {
                        beatLengths.Add(nextBeatPattern[i]);
                        beatRunningTotal += nextBeatPattern[i];
                    }
                }
            }
        }
        else if (rhythmDifficulty == "hard")
        {
            float beatRunningTotal = 0f;

            int attempts = 0;
            while (attempts < 10000 && beatRunningTotal < length)
            {
                attempts++;

                float[] nextBeatPattern = new float[0];
                switch (Functions.WeightedRand(new float[] { 0.5f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 0.8f, 0.3f }))
                {
                    case 0: nextBeatPattern = new float[] { 0.5f }; break;
                    case 1: nextBeatPattern = new float[] { 1f }; break;
                    case 2: nextBeatPattern = new float[] { 1.5f }; break;
                    case 3: nextBeatPattern = new float[] { 2f }; break;
                    case 4: nextBeatPattern = new float[] { 0.5f, 0.5f }; break;
                    case 5: nextBeatPattern = new float[] { 0.5f, 0.5f, 0.5f, 0.5f }; break;
                    case 6: nextBeatPattern = new float[] { 1.5f, 0.5f }; break;
                    case 7: nextBeatPattern = new float[] { 1.5f, 1.5f }; break;
                    case 8: nextBeatPattern = new float[] { 0.5f, 1f, 0.5f }; break;
                    default: throw new System.Exception("Unexpected switch value.");
                }

                if (beatRunningTotal + Enumerable.Sum(nextBeatPattern) <= length)
                {
                    attempts = 0;

                    for (int i = 0; i < nextBeatPattern.Length; i++)
                    {
                        beatLengths.Add(nextBeatPattern[i]);
                        beatRunningTotal += nextBeatPattern[i];
                    }
                }
            }
        }
        else
        {
            throw new System.Exception("Unknown CreateMelody() rhythm difficulty: " + rhythmDifficulty);
        }


        List<NoteWithDuration> melody = new List<NoteWithDuration>();

        if (singMelodyStartFromTonic)
        {
            melody.Add(GetTonic(key, clef).SetDuration(new Duration(beatLengths[0])));
        }
        else
        {
            if (key.mode == Mode.major)
            {
                int r = UnityEngine.Random.Range(0, 3);
                if (r == 0)
                {
                    melody.Add(GetTonic(key, clef).SetDuration(new Duration(beatLengths[0])));
                }
                else if (r == 1)
                {
                    melody.Add(key.AddIntervalNumber(GetTonic(key, clef), 3).SetDuration(new Duration(beatLengths[0])));
                }
                else if (r == 2)
                {
                    melody.Add(key.AddIntervalNumber(GetTonic(key, clef), 5).SetDuration(new Duration(beatLengths[0])));
                }
                else
                {
                    throw new System.Exception("Unexpected value of r: " + r);
                }
            }
            else
            {
                throw new System.Exception("Unimplemented mode: " + key.mode);
            }
        }

        for (int i = 1; i < beatLengths.Count; i++)
        {
            Note nextNote = melody[i - 1];
            bool noteAccepted = false;

            while (!noteAccepted)
            {
                Interval previousInterval = null;
                if (i > 1)
                {
                    previousInterval = melody[i - 1] - melody[i - 2];
                }

                nextNote = RandomNote(key, clef);

                if (Mathf.Abs((nextNote - melody[i - 1]).number) > 8)
                {
                    noteAccepted = false;
                }
                else
                {
                    if (singMelodyDifficulty == "stepwise")
                    {
                        int interval = (nextNote - melody[i - 1]).number;

                        if (Mathf.Abs(interval) == 1)
                        {
                            noteAccepted = Functions.RandomBool(0.1f);
                        }
                        else if (Mathf.Abs(interval) == 2)
                        {
                            if (i == 1)
                            {
                                noteAccepted = Functions.RandomBool(1f);
                            }
                            else
                            {
                                if (previousInterval.Sign() == Mathf.Sign(interval))
                                {
                                    noteAccepted = Functions.RandomBool(1f);
                                }
                                else
                                {
                                    noteAccepted = Functions.RandomBool(0.6f);
                                }
                            }
                        }
                    }
                    else if (singMelodyDifficulty == "easy")
                    {
                        if (beatLengths[i] == Duration.quaver && beatLengths[i - 1] == Duration.quaver)
                        {
                            switch (Mathf.Abs((nextNote - melody[i - 1]).number))
                            {
                                case 1: noteAccepted = Functions.RandomBool(0.3f); break;
                                case 2: noteAccepted = Functions.RandomBool(1f); break;
                                case 3: noteAccepted = Functions.RandomBool(0f); break;
                                case 4: noteAccepted = Functions.RandomBool(0f); break;
                                case 5: noteAccepted = Functions.RandomBool(00f); break;
                                case 6: noteAccepted = Functions.RandomBool(0f); break;
                                case 7: noteAccepted = Functions.RandomBool(0f); break;
                                case 8: noteAccepted = Functions.RandomBool(0f); break;
                                default: throw new System.Exception("Unexpected interval number: " + (nextNote - melody[i - 1]).number);
                            }
                        }
                        else
                        {
                            switch (Mathf.Abs((nextNote - melody[i - 1]).number))
                            {
                                case 1: noteAccepted = Functions.RandomBool(0.9f); break;
                                case 2: noteAccepted = Functions.RandomBool(0.9f); break;
                                case 3: noteAccepted = Functions.RandomBool(0.8f); break;
                                case 4: noteAccepted = Functions.RandomBool(0.5f); break;
                                case 5: noteAccepted = Functions.RandomBool(0.3f); break;
                                case 6: noteAccepted = Functions.RandomBool(0f); break;
                                case 7: noteAccepted = Functions.RandomBool(0f); break;
                                case 8: noteAccepted = Functions.RandomBool(0f); break;
                                default: throw new System.Exception("Unexpected interval number: " + (nextNote - melody[i - 1]).number);
                            }
                        }
                    }
                    else if (singMelodyDifficulty == "medium")
                    {
                        switch (Mathf.Abs((nextNote - melody[i - 1]).number))
                        {
                            case 1: noteAccepted = Functions.RandomBool(0.7f); break;
                            case 2: noteAccepted = Functions.RandomBool(0.7f); break;
                            case 3: noteAccepted = Functions.RandomBool(0.8f); break;
                            case 4: noteAccepted = Functions.RandomBool(0.6f); break;
                            case 5: noteAccepted = Functions.RandomBool(0.4f); break;
                            case 6: noteAccepted = Functions.RandomBool(0.2f); break;
                            case 7: noteAccepted = Functions.RandomBool(0.2f); break;
                            case 8: noteAccepted = Functions.RandomBool(0.2f); break;
                            default: throw new System.Exception("Unexpected interval number: " + (nextNote - melody[i - 1]).number);
                        }
                    }
                    else if (singMelodyDifficulty == "hard")
                    {
                        switch (Mathf.Abs((nextNote - melody[i - 1]).number))
                        {
                            case 1: noteAccepted = Functions.RandomBool(0.9f); break;
                            case 2: noteAccepted = Functions.RandomBool(0.9f); break;
                            case 3: noteAccepted = Functions.RandomBool(0.8f); break;
                            case 4: noteAccepted = Functions.RandomBool(0.6f); break;
                            case 5: noteAccepted = Functions.RandomBool(0.5f); break;
                            case 6: noteAccepted = Functions.RandomBool(0.5f); break;
                            case 7: noteAccepted = Functions.RandomBool(0.5f); break;
                            case 8: noteAccepted = Functions.RandomBool(0.5f); break;
                            default: throw new System.Exception("Unexpected interval number: " + (nextNote - melody[i - 1]).number);
                        }
                    }
                    else if (singMelodyDifficulty == "extreme")
                    {
                        switch (Mathf.Abs((nextNote - melody[i - 1]).number))
                        {
                            case 1: noteAccepted = Functions.RandomBool(1f); break;
                            case 2: noteAccepted = Functions.RandomBool(1f); break;
                            case 3: noteAccepted = Functions.RandomBool(1f); break;
                            case 4: noteAccepted = Functions.RandomBool(1f); break;
                            case 5: noteAccepted = Functions.RandomBool(1f); break;
                            case 6: noteAccepted = Functions.RandomBool(1f); break;
                            case 7: noteAccepted = Functions.RandomBool(1f); break;
                            case 8: noteAccepted = Functions.RandomBool(1f); break;
                            default: throw new System.Exception("Unexpected interval number: " + (nextNote - melody[i - 1]).number);
                        }
                    }
                    else
                    {
                        throw new System.Exception("Unknow CreateMelody() difficulty: " + pitchDifficulty);
                    }
                }
            }

            melody.Add(nextNote.SetDuration(new Duration(beatLengths[i])));
        }

        return melody.ToArray();
    }

    /// <summary>
    /// Sum the elements of 'elements' up to and including the 'sumUpToIndex'-th element.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="sumUpToIndex"></param>
    /// <returns></returns>
    private float SumFirstElements(float[] elements, int sumUpToIndex)
    {
        float sum = 0;
        for (int i = 0; i < sumUpToIndex && i < elements.Length; i++)
        {
            sum += elements[i];
        }
        return sumUpToIndex;
    }

    private List<NoteWithDuration> AddInterval(List<NoteWithDuration> notes, Interval interval)
    {
        List<NoteWithDuration> newNotes = new List<NoteWithDuration>();
        for (int i = 0; i < notes.Count; i++)
        {
            newNotes.Add(notes[i] + interval);
        }
        return newNotes;
    }

    public void SetSingMelodyStartFromTonic(bool fromTonic)
    {
        singMelodyStartFromTonic = fromTonic;
    }

    public void SetSingMelodyShowScaleDegrees(bool show)
    {
        singMelodyShowScaleDegrees = show;
        stave.ShowNoteScaleDegrees(singMelodyShowScaleDegrees);
    }

    public void SetSingMelodyDifficulty(string difficulty)
    {
        singMelodyDifficulty = difficulty.ToLower();
    }

    public void SetSingMelodyRhythmDifficulty(string difficulty)
    {
        singMelodyRhythmDifficulty = difficulty.ToLower();
    }

    public void SetPlaybackBPM(float bpm)
    {
        playbackBPM = bpm;
    }

    private int GetLineMouseOn()
    {
        int lineNumber = stave.system1.GetLineMouseOn();
        if (Functions.InRange(Mathf.Abs(lineNumber), 9, 10))
        {
            lineNumber = (int)Mathf.Sign(lineNumber) * 8;
        }
        return lineNumber;
    }
}
