using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rest
{
    public Duration duration { get; private set; }

    public Rest(Duration duration)
    {
        this.duration = duration;
    }

    public static bool operator ==(Rest rest1, Rest rest2)
    {
        return rest1.duration == rest2.duration;
    }
    public static bool operator !=(Rest rest1, Rest rest2)
    {
        return !(rest1 == rest2);
    }

    public float GetSeconds(float bpm, TimeSignature timeSignature)
    {
        return 60f / bpm * duration.GetBeats(timeSignature);
    }

    ///

    public static readonly Rest semiquaver = new Rest(Duration.semiquaver);
    public static readonly Rest sixteenth = new Rest(Duration.sixteenth);

    public static readonly Rest quaver = new Rest(Duration.quaver);
    public static readonly Rest eighth = new Rest(Duration.eighth);

    public static readonly Rest crotchet = new Rest(Duration.crotchet);
    public static readonly Rest quarter = new Rest(Duration.quarter);

    public static readonly Rest minim = new Rest(Duration.minim);
    public static readonly Rest half = new Rest(Duration.half);

    public static readonly Rest semibreve = new Rest(Duration.semibreve);
    public static readonly Rest whole = new Rest(Duration.whole);

    public static readonly Rest breve = new Rest(Duration.breve);
    public static readonly Rest doubleWhole = new Rest(Duration.doubleWhole);
}
