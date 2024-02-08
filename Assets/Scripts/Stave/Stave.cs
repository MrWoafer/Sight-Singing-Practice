using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Stave : MonoBehaviour
{
    [Tooltip("This is not a toggle - this is a button.")]
    public bool replotNotes = false;

    [Header("Key")]
    public Clef clef = Clef.treble;
    public Key key = Key.Cmajor;
    public TimeSignature time = TimeSignature._4_4;

    [Header("Notes")]
    [Tooltip("If two notes are on consecutive stave lines at the same x coord, this determines by how much one of them will be shifted to the side.")]
    [Min(0.001f)]
    public float noteClusterShift = 0.235f;
    [Tooltip("How far apart the notes are, horiontally.")]
    [Min(0.01f)]
    public float noteSpacing = 0.3f;
    [Tooltip("How much higher stems are made when angling beams.")]
    [Min(0.01f)]
    public float beamAnglingOffset = 0.2f;
    [Tooltip("The minimum vertical distance between an notehead and a beam.")]
    [Min(0.001f)]
    public float minBeamGap = 0.05f;
    [Tooltip("The minimum vertical distance between an notehead and a tie.")]
    [Min(0.001f)]
    public float tieGap = 0.05f;

    [Header("Bars")]
    [Tooltip("How thick the barlines are.")]
    [Min(0.01f)]
    public float barlineThickness = 0.025f;

    [Header("Systems")]
    [Tooltip("How much the systems are shrunk / enlarged.")]
    public float systemScale = 1f;
    [Tooltip("How wide the stave is.")]
    [Min(0.001f)]
    public float width = 10f;
    [Tooltip("How far apart the stave lines are, vertically.")]
    [Min(0.001f)]
    public float lineSpacing = 1f;
    [Tooltip("How thick the stave lines are.")]
    [Min(0.001f)]
    public float lineThickness = 1f;
    [Tooltip("The colour of the stave lines.")]
    public Color lineColour = Color.black;
    [Tooltip("How far the clef is from the left of the stave.")]
    public float clefMargin = 0.1f;
    [Tooltip("How far apart the clef and the key siganture are.")]
    [Min(0.01f)]
    public float clefKeySignatureGap = 1f;
    [Tooltip("How big the key signature accidentals are.")]
    [Min(0.001f)]
    public float keySignatureSize = 0.1f;
    [Tooltip("How far apart the key signature accidentals are, horizontally.")]
    [Min(0.001f)]
    public float keySignatureSpacing = 0.1f;
    [Tooltip("How far apart the key signature and the time siganture are.")]
    [Min(0.01f)]
    public float keySignatureTimeSignatureGap = 1f;
    [Tooltip("How far apart the time signature and the first note is.")]
    [Min(0.01f)]
    public float timeSignatureNoteGap = 1f;


    [Header("References")]
    [SerializeField]
    private List<StaveSystem> systems = new List<StaveSystem>();
    [SerializeField]
    private StaveGUI gui;
    [SerializeField]
    private GameObject systemPrefab;
    [SerializeField]
    private GameObject tiePrefab;

    public StaveSystem system1
    {
        get
        {
            if (systems.Count > 0)
            {
                return systems[0];
            }
            else
            {
                return null;
            }
        }
    }

    private List<Tie> ties = new List<Tie>();
    private int[] tieIndices = new int[0];

    // Start is called before the first frame update
    void Start()
    {
        //systemScale = 1f;
        //width = 17f;
        SetSystemScale(1f);

        SetNumOfSystems(1);
        UpdateSystems();

        List<NoteWithDuration> melody = new List<NoteWithDuration>();
        melody.Add(new NoteWithDuration(Note.C3, Duration.crotchet));
        melody.Add(new NoteWithDuration(Note.E3, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.A3, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.F3, Duration.crotchet));
        melody.Add(new NoteWithDuration(Note.C3, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.B2, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.A2, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.B2, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.C3, Duration.minim));
        melody.Add(new NoteWithDuration(Note.A2, Duration.dottedCrotchet));
        melody.Add(new NoteWithDuration(Note.E2, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.C3, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.D2, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.C2, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.G2, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.D4, Duration.quaver));
        melody.Add(new NoteWithDuration(Note.B2, Duration.quaver));
        //melody.Add(new NoteWithDuration(Note.C3, Duration.crotchet));

        PlotNotes(melody);
    }

    // Update is called once per frame
    void Update()
    {
        if (replotNotes)
        {
            replotNotes = false;
            ReplotNotes();
        }
    }

    private void UpdateDisplay()
    {
        UpdateSystems();
    }

    private void UpdateSystems()
    {
        if (systems.Count == 1)
        {
            systems[0].transform.localPosition = Vector3.zero;
        }
        else if (systems.Count == 2)
        {
            systems[0].transform.localPosition = new Vector3(0f, 1.5f, 0f);
            systems[1].transform.localPosition = new Vector3(0f, -1.5f, 0f);
        }
        else if (systems.Count == 3)
        {
            systems[0].transform.localPosition = new Vector3(0f, 3f, 0f);
            systems[1].transform.localPosition = new Vector3(0f, 0f, 0f);
            systems[2].transform.localPosition = new Vector3(0f, -3f, 0f);
        }

        for (int i = 0; i < systems.Count; i++)
        {
            StaveSystem s = systems[i];

            s.transform.localScale = systemScale * Vector3.one;

            s.SetClef(clef);
            s.SetKey(key);
            s.SetTimeSignature(time);

            s.width = width;
            s.lineSpacing = lineSpacing;
            s.lineThickness = lineThickness;
            s.lineColour = lineColour;
            s.clefMargin = clefMargin;
            s.clefKeySignatureGap = clefKeySignatureGap;
            s.keySignatureSize = keySignatureSize;
            s.keySignatureSpacing = keySignatureSpacing;
            s.keySignatureTimeSignatureGap = keySignatureTimeSignatureGap;
            s.timeSignatureNoteGap = timeSignatureNoteGap;

            s.noteClusterShift = noteClusterShift;
            s.noteSpacing = noteSpacing;
            s.beamAnglingOffset = beamAnglingOffset;
            s.minBeamGap = minBeamGap;

            s.barlineThickness = barlineThickness;

            s.UpdateDisplay();

            if (i == 0)
            {
                s.ShowTimeSignature(true);
            }
            else
            {
                s.ShowTimeSignature(false);
            }
        }

        ReplotNotes();
    }

    private void OnValidate()
    {
        UpdateDisplay();
    }

    public void SetKey(Key key)
    {
        this.key = key;
        UpdateSystems();
        UpdateDropdowns();
    }

    public void SetClef(Clef clef)
    {
        this.clef = clef;
        UpdateSystems();
        UpdateDropdowns();
        ReplotNotes();
    }

    public void SetTime(TimeSignature time)
    {
        this.time = time;
        UpdateSystems();
        UpdateDropdowns();
    }

    private void UpdateDropdowns()
    {
        gui.SetClefDropdown(clef);
        gui.SetKeyDropdown(key);
        gui.SetTimeDropdown(time);
    }

    public void SetWidth(float scale)
    {
        width = scale;
        UpdateDisplay();
    }

    public void SetSystemScale(float scale, bool scaleWidthAccordingly = true)
    {
        if (scaleWidthAccordingly)
        {
            SetWidth(width * systemScale / scale);
        }
        systemScale = scale;
        UpdateDisplay();
    }

    public void SetNumOfSystems(int number)
    {
        if (number < 0)
        {
            throw new System.Exception("Cannot have " + number + " systems");
        }
        if (number > 3)
        {
            throw new System.Exception("Support not yet implemented for " + number + " systems. Max of 3.");
        }
        
        foreach(StaveSystem system in systems)
        {
            Destroy(system.gameObject);
        }
        systems = new List<StaveSystem>();

        for (int i = 0; i < number; i++)
        {
            AddSystem();
        }
    }

    public void AddSystem()
    {
        systems.Add(Instantiate(systemPrefab, transform).GetComponent<StaveSystem>());
        StaveSystem system = systems[systems.Count - 1];
        system.transform.localPosition = Vector3.zero;
        system.transform.localScale = 2f * Vector3.one;

        UpdateSystems();
    }

    public void ClearNotes()
    {
        foreach (StaveSystem system in systems)
        {
            system.ClearNotes();
        }

        foreach (Tie tie in ties)
        {
            Destroy(tie.gameObject);
        }
        ties = new List<Tie>();
    }

    public void PlotNote(NoteWithDuration note, float x)
    {
        system1.PlotNote(note, x, Color.black);
    }
    public void PlotNote(NoteWithDuration note, float x, Color colour)
    {
        system1.PlotNote(note, x, colour);
    }

    public void PlotNoteCentre(NoteWithDuration note)
    {
        PlotNote(note, 0f, Color.black);
    }
    public void PlotNoteCentre(NoteWithDuration note, Color colour)
    {
        PlotNote(note, 0f, colour);
    }

    public void ReplotNotes()
    {
        List<NoteWithDuration> notes = new List<NoteWithDuration>();
        foreach (StaveSystem system in systems)
        {
            foreach (NoteWithDuration note in system.notes)
            {
                notes.Add(note);
            }
        }

        ClearNotes();

        PlotNotes(notes, false);
    }

    public void PlotNotes(List<NoteWithDuration> notes, bool redoTies = true)
    {
        PlotNotes(notes.ToArray(), redoTies);
    }
    public void PlotNotes(NoteWithDuration[] notes, bool redoTies = true)
    {
        Tuple<NoteWithDuration[], int[]> regroupedRhythms = RegroupRhythms(notes);
        notes = regroupedRhythms.Item1;

        if (redoTies)
        {
            tieIndices = regroupedRhythms.Item2;
        }

        int system = 0;
        int startOfCurrentBar = 0;
        int startOfCurrentSystem = 0;

        StaveNote previousNote = null;
        StaveNote currentNote = null;

        float x = systems[system].noteStartPosition;

        for (int i = 0; i < notes.Length; i++)
        {
            if (x > systems[system].endOfSystem)
            {
                if (system >= systems.Count - 1)
                {
                    AddSystem();
                }
                    
                for (int j = startOfCurrentBar; j < i; j++)
                {
                    /// note it's startOfCurrentBar, not j - startOfCurrentSystem, since the index will change when notes are removed from the list
                    systems[system].UnplotNote(startOfCurrentBar);
                    if (tieIndices.Contains(j))
                    {
                        int index = Array.IndexOf(tieIndices, j);
                        Destroy(ties[index].gameObject);
                        ties.RemoveAt(index);
                    }
                }

                system++;
                x = systems[system].noteStartPosition;

                i = startOfCurrentBar;
                startOfCurrentSystem = i;
            }

            if ((i == 1 && Mathf.FloorToInt(BeatsAtPosition(notes, i - 1, time) / time.topNumber) >= 1) ||
                (i > 1 && Mathf.FloorToInt(BeatsAtPosition(notes, i - 1, time) / time.topNumber) > Mathf.FloorToInt(BeatsAtPosition(notes, i - 2, time) / time.topNumber)))
            {
                startOfCurrentBar = i;
                systems[system].AddBarline(x);
                x += noteSpacing;
            }

            previousNote = currentNote;
            currentNote = systems[system].PlotNote(notes[i], x);

            x += noteSpacing;

            if (i > 0 && tieIndices.Contains(i - 1))
            {
                TieNotes(previousNote, currentNote, systems[system]);
            }
        }

        CheckBeams();
    }

    private void CheckBeams()
    {
        foreach (StaveSystem system in systems)
        {
            system.CheckBeams();
        }
    }

    private void TieNotes(StaveNote note1, StaveNote note2, StaveSystem system)
    {
        Tie tie = Instantiate(tiePrefab, system.transform).GetComponent<Tie>();
        tie.transform.localScale = new Vector3(tie.transform.localScale.x, tie.transform.localScale.y * (float)note2.stemDirection, tie.transform.localScale.z);
        tie.SetPoints(system.transform.TransformPoint(note1.transform.localPosition - new Vector3(0f, (float)note2.stemDirection * tieGap, 0f)),
            system.transform.TransformPoint(note2.transform.localPosition - new Vector3(0f, (float)note2.stemDirection * tieGap, 0f)));
        ties.Add(tie);
    }

    private float BeatsAtPosition(NoteWithDuration[] notes, int index, TimeSignature timeSignature)
    {
        float beats = 0f;
        for (int i = 0; i <= index; i++)
        {
            beats += notes[i].duration.GetBeats(timeSignature);
        }
        return beats;
    }

    private Tuple<NoteWithDuration[], int[]> RegroupRhythms(NoteWithDuration[] notes)
    {
        List<NoteWithDuration> noteList = new List<NoteWithDuration>(notes);
        List<NoteWithDuration> newNotes = new List<NoteWithDuration>();

        List<int> ties = new List<int>();

        float beatsThisBar = 0f;
        float beatsThisWeakBeatBar = 0f;
        while (noteList.Count > 0)
        {
            float noteBeats = noteList[0].duration.GetBeats(time);

            bool regroup = false;
            Duration firstHalf = null;
            Duration secondHalf = null;

            if (beatsThisBar + noteBeats > time.topNumber)
            {
                regroup = true;
                firstHalf = new Duration(time.topNumber - beatsThisBar, time);
                secondHalf = new Duration(noteBeats - (time.topNumber - beatsThisBar), time);
            }
            if (beatsThisWeakBeatBar + noteBeats > time.weakBeat && !regroup)
            {
                if (!((noteList[0].duration == Duration.minim || noteList[0].duration == Duration.dottedMinim) && Functions.IsInteger(beatsThisBar)) &&
                    !(noteList[0].duration == Duration.semibreve && beatsThisBar == 0) && !(noteList[0].duration == Duration.breve && beatsThisBar == 0))
                {
                    regroup = true;
                    firstHalf = new Duration(time.weakBeat - beatsThisWeakBeatBar, time);
                    secondHalf = new Duration(noteBeats - (time.weakBeat - beatsThisWeakBeatBar), time);
                }
            }

            if (regroup)
            {
                noteList.Insert(0, new NoteWithDuration(noteList[0], firstHalf));
                noteList.Insert(1, new NoteWithDuration(noteList[0], secondHalf));
                noteList.RemoveAt(2);

                ties.Add(newNotes.Count);
            }

            beatsThisBar += noteList[0].duration.GetBeats(time);
            beatsThisWeakBeatBar += noteList[0].duration.GetBeats(time);
            newNotes.Add(noteList[0]);
            noteList.RemoveAt(0);

            while (beatsThisBar >= time.topNumber)
            {
                beatsThisBar -= time.topNumber;
            }
            while (beatsThisWeakBeatBar >= time.weakBeat)
            {
                beatsThisWeakBeatBar -= time.weakBeat;
            }
        }

        return new Tuple<NoteWithDuration[], int[]> (newNotes.ToArray(), ties.ToArray());
    }

    public void ColourNote(int noteIndex, Color colour)
    {
        system1.ColourNote(noteIndex, colour);
    }
    public void ColourNotes(Color colour)
    {
        system1.ColourNotes(colour);
    }

    public void ShowNoteScaleDegrees(bool show)
    {
        foreach(StaveSystem system in systems)
        {
            system.ShowNoteScaleDegrees(show);
        }
    }
    public void ClearNoteScaleDegrees()
    {
        foreach (StaveSystem system in systems)
        {
            system.ClearNoteScaleDegrees();
        }
    }
}
