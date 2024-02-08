using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum StemDirection
{
    up = 1,
    down = -1
}

public class StaveNote : MonoBehaviour
{
    [Header("Note")]
    [Tooltip("This is not an attribute; I'm using it as a button.")]
    public bool printDuration = false;
    public Duration duration = Duration.crotchet;
    public Note note = Note.C4;

    [Header("Display")]
    [Tooltip("Whether the stem goes up or down.")]
    public StemDirection stemDirection = StemDirection.up;
    [Tooltip("Whether to show the stem or not.")]
    public bool showStem = true;
    [Tooltip("The height of the stem.")]
    [Min(0.001f)]
    public float stemHeight = 3f;
    [Tooltip("The width of the stem.")]
    [Min(0.001f)]
    public float stemThickness = 0.4f;
    [Tooltip("The height of the notehead.")]
    [Min(0.001f)]
    public float noteheadHeight = 1f;
    [Tooltip("The width of the notehead.")]
    [Min(0.001f)]
    public float noteheadWidth = 1.3f;
    [Tooltip("The thickness of the notehead line.")]
    [Min(0.001f)]
    public float noteheadThickness = 0.2f;
    [Tooltip("The colour of the lines.")]
    public Color colour = Color.black;

    [Header("Scale Degree")]
    public bool showScaleDegree = false;
    public Color scaleDegreePrimaryColour = Color.white;
    public Color scaleDegreeSecondaryColour = Color.black;
    public ScaleDegree scaleDegree = new ScaleDegree("1");

    [Header("References")]
    [SerializeField]
    private Transform notehead;
    [SerializeField]
    private Transform noteheadWhite;
    [SerializeField]
    private Transform stem;
    [SerializeField]
    private Transform flickTop;
    [SerializeField]
    private Transform flickBottom;
    [SerializeField]
    private Transform dot;
    [SerializeField]
    private TextMeshProUGUI numberText;
    [SerializeField]
    private LineRenderer lr;

    public Vector3 endOfStem
    {
        get
        {
            return stem.position + new Vector3(0f, (float)stemDirection * stem.lossyScale.y / 2f, 0f);
        }
    }
    
    public bool stemLinked
    {
        get
        {
            return lr.positionCount > 0;
        }
    }

    public NoteWithDuration noteWithDuration
    {
        get
        {
            return new NoteWithDuration(note, duration);
        }
    }

    public bool noteheadFilledIn
    {
        get
        {
            return !noteheadWhite.GetComponent<SpriteMask>().enabled;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (printDuration)
        {
            Debug.Log(duration);
            printDuration = false;
        }
    }

    private void OnValidate()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        notehead.localScale = new Vector3(noteheadWidth, noteheadHeight, 1f);
        noteheadWhite.localScale = new Vector3(1f - noteheadThickness, 1f - noteheadThickness, 1f);
        stem.localScale = new Vector3(stemThickness, stemHeight, 1f);

        notehead.localPosition = new Vector3(0f, 0f, 0f);
        noteheadWhite.localPosition = new Vector3(0f, 0f, 0f);
        if (stemDirection == StemDirection.up)
        {
            stem.localPosition = new Vector3(noteheadWidth / 2f - stemThickness / 2f, stemHeight / 2f, 0f);
        }
        else
        {
            stem.localPosition = new Vector3(-noteheadWidth / 2f + stemThickness / 2f, -stemHeight / 2f, 0f);
        }

        numberText.text = scaleDegree.ToString();

        if (showScaleDegree)
        {
            numberText.enabled = true;
        }
        else
        {
            numberText.enabled = false;
        }

        if (duration == Duration.half || duration == Duration.dottedHalf || duration == Duration.whole || duration == Duration.doubleWhole)
        {
            noteheadWhite.GetComponent<SpriteMask>().enabled = true;
        }
        else
        {
            noteheadWhite.GetComponent<SpriteMask>().enabled = false;
        }

        if (duration == Duration.whole || duration == Duration.doubleWhole)
        {
            showStem = false;
        }
        else
        {
            showStem = true;
        }

        flickTop.gameObject.SetActive(false);
        flickBottom.gameObject.SetActive(false);
        if (!stemLinked && (duration == Duration.quaver || duration == Duration.semiquaver || duration == Duration.dottedQuaver))
        {
            if (stemDirection == StemDirection.up)
            {
                flickTop.gameObject.SetActive(true);
            }
            else
            {
                flickBottom.gameObject.SetActive(true);
            }
        }

        flickTop.transform.position = endOfStem;
        flickTop.transform.localScale = stemThickness * 2f * Vector3.one;
        flickBottom.transform.position = endOfStem;
        flickBottom.transform.localScale = stemThickness * 2f * Vector3.one;

        if (showStem)
        {
            stem.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            stem.GetComponent<SpriteRenderer>().enabled = false;
        }

        lr.startColor = colour;
        lr.startWidth = stem.transform.lossyScale.x;

        if (duration.isDotted)
        {
            dot.gameObject.SetActive(true);
        }
        else
        {
            dot.gameObject.SetActive(false);
        }

        notehead.GetComponent<SpriteRenderer>().color = colour;
        stem.GetComponent<SpriteRenderer>().color = colour;

        if (noteheadFilledIn)
        {
            numberText.color = scaleDegreePrimaryColour;
        }
        else
        {
            numberText.color = scaleDegreeSecondaryColour;
        }

    }

    public void SetNote(Note note)
    {
        this.note = note;
    }
    public void SetNote(NoteWithDuration note)
    {
        this.note = note;
        duration = note.duration;
    }

    public void SetDuration(Duration duration)
    {
        this.duration = duration;
        UpdateDisplay();
    }

    public void SetColour(Color colour)
    {
        this.colour = colour;
        UpdateDisplay();
    }

    public void SetStemDirection(StemDirection direction)
    {
        stemDirection = direction;
        UpdateDisplay();
    }

    public void SetStemHeight(float height)
    {
        stemHeight = height;
        UpdateDisplay();
    }

    public void SetScaleDegree(ScaleDegree scaleDegree)
    {
        this.scaleDegree = scaleDegree;
        UpdateDisplay();
    }

    public void SetShowScaleDegree(bool show)
    {
        showScaleDegree = show;
        UpdateDisplay();
    }

    public void BeamTo(StaveNote otherNote)
    {
        this.BeamToNoNotify(otherNote);
        otherNote.BeamToNoNotify(this);
    }
    public void BeamToNoNotify(StaveNote otherNote)
    {
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { endOfStem, otherNote.endOfStem });

        UpdateDisplay();
    }
}
