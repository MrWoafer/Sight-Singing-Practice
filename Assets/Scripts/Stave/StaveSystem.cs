using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StaveSystem : MonoBehaviour
{
    [Tooltip("This is not a toggle - this is a button.")]
    public bool replotNotes = false;

    [Header("Key")]
    [SerializeField]
    private Clef clef = Clef.treble;
    [SerializeField]
    private Key key = Key.Dmajor;
    [SerializeField]
    private TimeSignature time = TimeSignature._4_4;

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

    [Header("Bars")]
    [Tooltip("How thick the barlines are.")]
    [Min(0.01f)]
    public float barlineThickness = 0.025f;

    [Header("System")]
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
    private Transform line2;
    [SerializeField]
    private Transform line1;
    [SerializeField]
    private Transform line0;
    [SerializeField]
    private Transform line_1;
    [SerializeField]
    private Transform line_2;
    [SerializeField]
    private Transform background;
    [SerializeField]
    private Transform clefTransform;
    [SerializeField]
    private GameObject trebleClef;
    [SerializeField]
    private GameObject bassClef;
    [SerializeField]
    private GameObject altoClef;
    [SerializeField]
    private GameObject flatPrefab;
    [SerializeField]
    private GameObject sharpPrefab;
    [SerializeField]
    private GameObject keySignatureObj;
    [SerializeField]
    private GameObject timeSignatureObj;
    [SerializeField]
    private TextMeshProUGUI timeTopText;
    [SerializeField]
    private TextMeshProUGUI timeBottomText;
    [SerializeField]
    private GameObject cursor;
    [SerializeField]
    private GameObject notePrefab;
    [SerializeField]
    private GameObject ledgerLinePrefab;
    [SerializeField]
    private GameObject barlinePrefab;

    private Transform[] lines
    {
        get
        {
            return new Transform[] { line_2, line_1, line0, line1, line2 };
        }
    }

    private GameObject[] clefs
    {
        get
        {
            return new GameObject[] { trebleClef, bassClef, altoClef };
        }
    }

    public float noteStartPosition
    {
        get
        {
            return timeSignatureObj.transform.localPosition.x + timeSignatureNoteGap;
        }
    }

    public float endOfSystem
    {
        get
        {
            return width / 2f;
        }
    }

    private List<GameObject> keySignatureAccidentals = new List<GameObject>();
    private List<StaveNote> noteList = new List<StaveNote>();
    public NoteWithDuration[] notes
    {
        get
        {
            NoteWithDuration[] notesWithDuration = new NoteWithDuration[noteList.Count];
            for (int k = 0; k < noteList.Count; k++)
            {
                notesWithDuration[k] = noteList[k].noteWithDuration;
            }
            return notesWithDuration;
        }
    }
    private List<GameObject> ledgerLines = new List<GameObject>();
    private List<GameObject> barlines = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

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

    private void OnValidate()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        UpdateStaff();
        UpdateClef();
        UpdateKeySignature();
        UpdateTimeSignature();
        ReplotNotes();
    }

    private void UpdateStaff()
    {
        background.transform.localScale = new Vector3(width, 4f * lineSpacing, 1f);
        foreach (Transform l in lines)
        {
            l.transform.localScale = new Vector3(width, lineThickness, 1f);
            l.GetComponent<SpriteRenderer>().color = lineColour;
        }

        line2.transform.localPosition = new Vector3(0f, 2f * lineSpacing, 0f);
        line1.transform.localPosition = new Vector3(0f, lineSpacing, 0f);
        line0.transform.localPosition = new Vector3(0f, 0f, 0f);
        line_1.transform.localPosition = new Vector3(0f, -lineSpacing, 0f);
        line_2.transform.localPosition = new Vector3(0f, -2f * lineSpacing, 0f);

        clefTransform.localPosition = new Vector3(-width / 2f + 0.3f + clefMargin, 0f, 0f);

        UpdateKeySignature();
        UpdateTimeSignature();
    }

    private void UpdateClef()
    {
        foreach (GameObject c in clefs)
        {
            c.SetActive(false);
        }
        if (clef == Clef.treble)
        {
            trebleClef.SetActive(true);
        }
        else if (clef == Clef.bass)
        {
            bassClef.SetActive(true);
        }
        else if (clef == Clef.alto)
        {
            altoClef.SetActive(true);
        }

        UpdateKeySignature();
    }

    private void UpdateKeySignature()
    {
        /// Do not update the key signature in edit mode
        if (Application.isPlaying)
        {
            foreach (GameObject obj in keySignatureAccidentals)
            {
                Destroy(obj);
            }
            keySignatureAccidentals = new List<GameObject>();

            if (key.IsModeOf(Key.Cmajor))
            {

            }
            else if (key.IsModeOf(Key.Gmajor))
            {
                AddKeySignatureAccidental(NoteLetter.F, Accidental.sharp);
            }
            else if (key.IsModeOf(Key.Dmajor))
            {
                AddKeySignatureAccidental(NoteLetter.F, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.C, Accidental.sharp);
            }
            else if (key.IsModeOf(Key.Amajor))
            {
                AddKeySignatureAccidental(NoteLetter.F, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.C, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.G, Accidental.sharp);
            }
            else if (key.IsModeOf(Key.Emajor))
            {
                AddKeySignatureAccidental(NoteLetter.F, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.C, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.G, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.D, Accidental.sharp);
            }
            else if (key.IsModeOf(Key.Bmajor))
            {
                AddKeySignatureAccidental(NoteLetter.F, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.C, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.G, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.D, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.A, Accidental.sharp);
            }
            else if (key.IsModeOf(Key.FsharpMajor))
            {
                AddKeySignatureAccidental(NoteLetter.F, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.C, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.G, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.D, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.A, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.E, Accidental.sharp);
            }
            else if (key.IsModeOf(Key.CsharpMajor))
            {
                AddKeySignatureAccidental(NoteLetter.F, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.C, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.G, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.D, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.A, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.E, Accidental.sharp);
                AddKeySignatureAccidental(NoteLetter.B, Accidental.sharp);
            }
            else if (key.IsModeOf(Key.Fmajor))
            {
                AddKeySignatureAccidental(NoteLetter.B, Accidental.flat);
            }
            else if (key.IsModeOf(Key.Bbmajor))
            {
                AddKeySignatureAccidental(NoteLetter.B, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.E, Accidental.flat);
            }
            else if (key.IsModeOf(Key.Ebmajor))
            {
                AddKeySignatureAccidental(NoteLetter.B, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.E, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.A, Accidental.flat);
            }
            else if (key.IsModeOf(Key.Abmajor))
            {
                AddKeySignatureAccidental(NoteLetter.B, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.E, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.A, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.D, Accidental.flat);
            }
            else if (key.IsModeOf(Key.Dbmajor))
            {
                AddKeySignatureAccidental(NoteLetter.B, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.E, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.A, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.D, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.G, Accidental.flat);
            }
            else if (key.IsModeOf(Key.Gbmajor))
            {
                AddKeySignatureAccidental(NoteLetter.B, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.E, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.A, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.D, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.G, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.C, Accidental.flat);
            }
            else if (key.IsModeOf(Key.Cbmajor))
            {
                AddKeySignatureAccidental(NoteLetter.B, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.E, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.A, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.D, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.G, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.C, Accidental.flat);
                AddKeySignatureAccidental(NoteLetter.F, Accidental.flat);
            }
            else
            {
                throw new System.Exception("Unimplemented key signature: " + key);
            }
        }
    }

    private void AddKeySignatureAccidental(NoteLetter note, Accidental accidental)
    {
        int octave = 3;
        GameObject obj = null;
        if (accidental == Accidental.flat)
        {
            if (clef == Clef.treble)
            {
                switch (note)
                {
                    case NoteLetter.C: octave = 5; break;
                    case NoteLetter.D: octave = 5; break;
                    case NoteLetter.E: octave = 5; break;
                    case NoteLetter.F: octave = 4; break;
                    case NoteLetter.G: octave = 4; break;
                    case NoteLetter.A: octave = 4; break;
                    case NoteLetter.B: octave = 4; break;
                    default: throw new System.Exception("Unknown note letter: " + note);
                }
            }
            else if (clef == Clef.bass)
            {
                switch (note)
                {
                    case NoteLetter.C: octave = 3; break;
                    case NoteLetter.D: octave = 3; break;
                    case NoteLetter.E: octave = 3; break;
                    case NoteLetter.F: octave = 2; break;
                    case NoteLetter.G: octave = 2; break;
                    case NoteLetter.A: octave = 2; break;
                    case NoteLetter.B: octave = 2; break;
                    default: throw new System.Exception("Unknown note letter: " + note);
                }
            }
            else if (clef == Clef.alto)
            {
                switch (note)
                {
                    case NoteLetter.C: octave = 4; break;
                    case NoteLetter.D: octave = 4; break;
                    case NoteLetter.E: octave = 4; break;
                    case NoteLetter.F: octave = 3; break;
                    case NoteLetter.G: octave = 3; break;
                    case NoteLetter.A: octave = 3; break;
                    case NoteLetter.B: octave = 3; break;
                    default: throw new System.Exception("Unknown note letter: " + note);
                }
            }
            else
            {
                throw new System.Exception("Unimplemented clef: " + clef);
            }

            obj = Instantiate(flatPrefab, keySignatureObj.transform);
        }
        else if (accidental == Accidental.sharp)
        {
            if (clef == Clef.treble)
            {
                switch (note)
                {
                    case NoteLetter.C: octave = 5; break;
                    case NoteLetter.D: octave = 5; break;
                    case NoteLetter.E: octave = 5; break;
                    case NoteLetter.F: octave = 5; break;
                    case NoteLetter.G: octave = 5; break;
                    case NoteLetter.A: octave = 4; break;
                    case NoteLetter.B: octave = 4; break;
                    default: throw new System.Exception("Unknown note letter: " + note);
                }
            }
            else if (clef == Clef.bass)
            {
                switch (note)
                {
                    case NoteLetter.C: octave = 3; break;
                    case NoteLetter.D: octave = 3; break;
                    case NoteLetter.E: octave = 3; break;
                    case NoteLetter.F: octave = 3; break;
                    case NoteLetter.G: octave = 3; break;
                    case NoteLetter.A: octave = 2; break;
                    case NoteLetter.B: octave = 2; break;
                    default: throw new System.Exception("Unknown note letter: " + note);
                }
            }
            else if (clef == Clef.alto)
            {
                switch (note)
                {
                    case NoteLetter.C: octave = 4; break;
                    case NoteLetter.D: octave = 4; break;
                    case NoteLetter.E: octave = 4; break;
                    case NoteLetter.F: octave = 4; break;
                    case NoteLetter.G: octave = 4; break;
                    case NoteLetter.A: octave = 3; break;
                    case NoteLetter.B: octave = 3; break;
                    default: throw new System.Exception("Unknown note letter: " + note);
                }
            }
            else
            {
                throw new System.Exception("Unimplemented clef: " + clef);
            }

            obj = Instantiate(sharpPrefab, keySignatureObj.transform);
        }
        else
        {
            throw new System.Exception("Invalid accidental: " + accidental);
        }

        float y = GetY(note, octave);
        obj.transform.localScale = keySignatureSize * Vector3.one;
        obj.transform.localPosition = new Vector3(clefTransform.localPosition.x + clefKeySignatureGap + keySignatureAccidentals.Count * keySignatureSpacing, y, 0f);
        keySignatureAccidentals.Add(obj);
    }

    public void SetClef(Clef clef)
    {
        this.clef = clef;
        UpdateClef();
    }

    public void SetKey(Key key)
    {
        this.key = key;
        UpdateKeySignature();
    }

    /// <summary>
    /// Returns the local y-coordinate of where the given note should be displayed on the stave.
    /// </summary>
    private float GetY(NoteLetter note, int octave)
    {
        return LineNumberToY(Note.LetterDifference(new Note(new NoteName(note, Accidental.natural), octave), Note.C4)) + GetYMiddleC();
    }
    /// <summary>
    /// Returns the local y-coordinate of where the given note should be displayed on the stave.
    /// </summary>
    private float GetY(Note note)
    {
        return GetY(note.noteLetter, note.octave);
    }

    /// <summary>
    /// Returns the local y-coordinate of where C4 (Middle C) should be displayed on the stave.
    /// </summary>
    private float GetYMiddleC()
    {
        if (clef == Clef.treble)
        {
            return -3f * lineSpacing;
        }
        else if (clef == Clef.bass)
        {
            return 3f * lineSpacing;
        }
        if (clef == Clef.alto)
        {
            return 0f;
        }
        else
        {
            throw new System.Exception("Unimplemented clef: " + clef);
        }
    }

    public void SetTimeSignature(TimeSignature time)
    {
        this.time = time;
        UpdateTimeSignature();
    }

    public void ShowTimeSignature(bool show)
    {
        timeSignatureObj.SetActive(show);
    }

    private void UpdateTimeSignature()
    {
        timeTopText.text = time.topNumber.ToString();
        timeBottomText.text = time.bottomNumber.ToString();

        timeTopText.rectTransform.localPosition = new Vector3(0f, lineSpacing * 100f, 0f);
        timeBottomText.rectTransform.localPosition = new Vector3(0f, -lineSpacing * 100f, 0f);

        if (keySignatureAccidentals.Count == 0)
        {
            timeSignatureObj.transform.localPosition = new Vector3(clefTransform.localPosition.x + keySignatureTimeSignatureGap + 0.2f, 0f, 0f);
        }
        else
        {
            timeSignatureObj.transform.localPosition = new Vector3(keySignatureAccidentals[keySignatureAccidentals.Count - 1].transform.localPosition.x + keySignatureTimeSignatureGap, 0f, 0f);
        }
    }

    public Note GetCentreNote()
    {
        NoteLetter noteLetter;
        int octave;
        switch (clef)
        {
            case Clef.treble: noteLetter = NoteLetter.B; octave = 4; break;
            case Clef.alto: noteLetter = NoteLetter.C; octave = 4; break;
            case Clef.bass: noteLetter = NoteLetter.D; octave = 3; break;
            default: throw new System.Exception("Unimplemented clef: " + clef);
        }

        return new Note(key[noteLetter], octave);
    }

    public int GetLineNumber(Note note)
    {
        float y = GetY(note);
        return Mathf.RoundToInt(y / lineSpacing * 2f);
    }

    public Note LineNumberToNote(int lineNumber)
    {
        if (lineNumber >= 0)
        {
            lineNumber++;
        }
        else
        {
            lineNumber--;
        }

        return key.AddIntervalNumber(GetCentreNote(), lineNumber);
    }

    public float LineNumberToY(int lineNumber)
    {
        return LineNumberToY((float)lineNumber);
    }
    public float LineNumberToY(float lineNumber)
    {
        return lineSpacing / 2f * lineNumber;
    }

    public StaveNote PlotNote(NoteWithDuration note, float x)
    {
        return PlotNote(note, x, Color.black);
    }
    public StaveNote PlotNote(NoteWithDuration note, float x, Color colour)
    {
        foreach (StaveNote noteCheck in noteList)
        {
            if (Mathf.Abs(noteCheck.transform.position.x - x) < 0.01f && Mathf.Abs(GetLineNumber(noteCheck.note) - GetLineNumber(note)) == 1)
            {
                if (note > noteCheck.note)
                {
                    x += noteClusterShift;
                }
                else
                {
                    noteCheck.transform.localPosition += new Vector3(noteClusterShift, 0f, 0f);
                    ReplotNotes();
                }
            }
        }

        StaveNote noteObj = Instantiate(notePrefab, transform).GetComponent<StaveNote>();
        noteObj.transform.localPosition = new Vector3(x, GetY(note), 0f);
        noteObj.SetNote(note);
        noteObj.SetDuration(note.duration);
        noteObj.SetColour(colour);

        int line = GetLineNumber(note);

        if (line >= 0)
        {
            noteObj.SetStemDirection(StemDirection.down);
        }
        else
        {
            noteObj.SetStemDirection(StemDirection.up);
        }

        if (line >= 6)
        {
            for (int i = 2 * Mathf.FloorToInt(line / 2f); i > 4; i -= 2)
            {
                GameObject ledgerLine = Instantiate(ledgerLinePrefab, transform);
                ledgerLine.transform.localPosition = new Vector3(x, LineNumberToY(i), 0f);
                ledgerLines.Add(ledgerLine);
            }
        }
        else if (line <= -6)
        {
            for (int i = 2 * Mathf.CeilToInt(line / 2f); i < -4; i += 2)
            {
                GameObject ledgerLine = Instantiate(ledgerLinePrefab, transform);
                ledgerLine.transform.localPosition = new Vector3(x, LineNumberToY(i), 0f);
                ledgerLines.Add(ledgerLine);
            }
        }

        noteList.Add(noteObj);

        return noteObj;
    }

    public void CheckBeams()
    {
        int i = 0;
        while (i < noteList.Count)
        {
            //if (i < noteList.Count - 1 && notes[i].duration == Duration.eighth && Functions.IsInteger(BeatsAtPosition(notes, i - 1, time)))
            if (i < noteList.Count - 1 && notes[i].duration == Duration.eighth)
            {
                int j = i + 1;
                while (j < Mathf.Min(noteList.Count))
                {
                    if (notes[j].duration != Duration.eighth ||
                        BeatsAtPosition(notes, j, time) > Functions.NextMultipleOf(BeatsAtPosition(notes, i, time), time.weakBeat))
                    {
                        break;
                    }
                    j++;
                }
                //Debug.Log(i + " " + (j - 1));

                if (j > i + 1)
                {
                    int furthestNoteFromCentre = i;
                    for (int k = i + 1; k < j; k++)
                    {
                        if (Mathf.Abs(GetLineNumber(notes[k])) > Mathf.Abs(GetLineNumber(notes[i])) || 
                            (Mathf.Abs(GetLineNumber(notes[k])) == Mathf.Abs(GetLineNumber(notes[i])) && noteList[k].stemDirection == StemDirection.down))
                        {
                            furthestNoteFromCentre = k;
                        }
                    }
                    StemDirection stemDirection = noteList[furthestNoteFromCentre].stemDirection;

                    for (int k = i; k < j; k++)
                    {
                        noteList[k].SetStemDirection(stemDirection);
                    }

                    float minStemPoint = stemDirection == StemDirection.up ? LineNumberToY(5) : LineNumberToY(-5);
                    //float minStemPoint = 0f;
                    //Debug.Log(minStemPoint + " " + noteList[i].transform.localPosition.y);
                    for (int k = i; k < j; k++)
                    {
                        if (stemDirection == StemDirection.up)
                        {
                            minStemPoint = Mathf.Max(minStemPoint, noteList[k].transform.localPosition.y + minBeamGap);
                        }
                        else
                        {
                            minStemPoint = Mathf.Min(minStemPoint, noteList[k].transform.localPosition.y - minBeamGap);
                        }
                    }

                    noteList[i].SetStemHeight(Mathf.Abs(minStemPoint - noteList[i].transform.localPosition.y) / noteList[i].transform.localScale.y);
                    noteList[j - 1].SetStemHeight(Mathf.Abs(minStemPoint - noteList[j - 1].transform.localPosition.y) / noteList[j - 1].transform.localScale.y);

                    float beamAnglingOffsetScaled = Mathf.Min(4f, Mathf.Abs(GetLineNumber(notes[i]) - GetLineNumber(notes[j - 1]))) * beamAnglingOffset;
                    if (notes[i] > notes[j - 1])
                    {
                        if (stemDirection == StemDirection.up)
                        {
                            noteList[i].SetStemHeight(noteList[i].stemHeight + beamAnglingOffsetScaled);
                        }
                        else
                        {
                            noteList[j - 1].SetStemHeight(noteList[j - 1].stemHeight + beamAnglingOffsetScaled);
                        }
                    }
                    else if (notes[i] < notes[j - 1])
                    {
                        if (stemDirection == StemDirection.up)
                        {
                            noteList[j - 1].SetStemHeight(noteList[j - 1].stemHeight + beamAnglingOffsetScaled);
                        }
                        else
                        {
                            noteList[i].SetStemHeight(noteList[i].stemHeight + beamAnglingOffsetScaled);
                        }
                    }

                    for (int k = i; k < j; k++)
                    {                       
                        noteList[k].SetStemHeight(Mathf.Abs(Mathf.Lerp(noteList[i].stemHeight * noteList[i].transform.localScale.y * (float)stemDirection + noteList[i].transform.localPosition.y,
                            noteList[j - 1].stemHeight * noteList[j - 1].transform.localScale.y * (float)stemDirection + noteList[j - 1].transform.localPosition.y,
                            ((float)k - i) / (j - 1 - i)) - noteList[k].transform.localPosition.y) / noteList[k].transform.localScale.y);
                    }

                    for (int k = i; k < j - 1; k++)
                    {
                        noteList[k].BeamTo(noteList[k + 1]);
                        //Debug.Log(i + " " + (j - 1) + "; " + k + " " + (k + 1));
                    }

                    i = j - 1;
                }
            }

            i++;
        }
    }

    public void AddBarline(float x)
    {
        GameObject barline = Instantiate(barlinePrefab, transform);
        barline.transform.localScale = new Vector3(barlineThickness, lineSpacing * 4f, 0f);
        barline.transform.localPosition = new Vector3(x, 0f, 0f);
        barlines.Add(barline);
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

    public int GetLineMouseOn()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseYRelative = (mousePos.y - transform.position.y) / transform.lossyScale.y;
        cursor.transform.localPosition = new Vector3(0f, mouseYRelative, 0f);

        return Mathf.RoundToInt(mouseYRelative / lineSpacing * 2f);
    }
    public Note GetNoteMouseOn()
    {
        return LineNumberToNote(GetLineMouseOn());
    }

    public void ClearNotes()
    {
        foreach (StaveNote note in noteList)
        {
            Destroy(note.gameObject);
        }
        noteList = new List<StaveNote>();

        foreach (GameObject line in ledgerLines)
        {
            Destroy(line);
        }
        ledgerLines = new List<GameObject>();

        foreach (GameObject line in barlines)
        {
            Destroy(line);
        }
        barlines = new List<GameObject>();
    }

    public void ReplotNotes()
    {
        List<StaveNote> oldNotes = new List<StaveNote>(noteList);
        ClearNotes();

        foreach (StaveNote note in oldNotes)
        {
            PlotNote(note.note.SetDuration(note.duration), note.transform.localPosition.x, note.colour);
        }
    }

    public void UnplotNote(int index)
    {
        for (int i = 0; i < ledgerLines.Count; i++)
        {
            if (Mathf.Abs(ledgerLines[i].transform.localPosition.x - noteList[index].transform.localPosition.x) < 0.01f)
            {
                Destroy(ledgerLines[i].gameObject);
                ledgerLines.RemoveAt(i);
                i--;
            }
        }

        Destroy(noteList[index].gameObject);
        noteList.RemoveAt(index);
    }

    /*public void PlotNotes(List<NoteWithDuration> notes)
    {
        PlotNotes(notes.ToArray());
    }
    public void PlotNotes(NoteWithDuration[] notes)
    {
        float x = timeSignatureObj.transform.localPosition.x + timeSignatureNoteGap;

        for (int i = 0; i < notes.Length; i++)
        {
            PlotNote(notes[i], x);
            x += noteSpacing;
        }
    }*/

    public void ShowNoteScaleDegrees(bool show)
    {
        if (show)
        {
            for (int i = 0; i < noteList.Count; i++)
            {
                noteList[i].SetScaleDegree(key.ScaleDegreeOf(noteList[i].note));
                noteList[i].SetShowScaleDegree(true);
            }
        }
        else
        {
            ClearNoteScaleDegrees();
        }
    }
    public void ClearNoteScaleDegrees()
    {
        for (int i = 0; i < noteList.Count; i++)
        {
            noteList[i].SetShowScaleDegree(false);
        }
    }

    public void ColourNote(int noteIndex, Color colour)
    {
        noteList[noteIndex].SetColour(colour);
    }
    public void ColourNotes(Color colour)
    {
        for (int i = 0; i < noteList.Count; i++)
        {
            ColourNote(i, colour);
        }
    }
}
