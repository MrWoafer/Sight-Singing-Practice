using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum Clef
{
    treble = 2,
    alto = 1,
    bass = 0
}

public static class MusicTheory
{
    public static readonly Note middleC = Note.C4;

    /// <summary>Not yet implemented.</summary>
    public static readonly NoteName[] circleOfFifths = new NoteName[] { };
    public static readonly NoteName[] circleOfKeys = new NoteName[] { NoteName.Cb, NoteName.Gb, NoteName.Db, NoteName.Ab, NoteName.Eb, NoteName.Bb, NoteName.F, NoteName.C, NoteName.G, NoteName.D,
        NoteName.A, NoteName.E, NoteName.B, NoteName.Fsharp, NoteName.Csharp};

    public static readonly Mode[] modesOfMajor = new Mode[] { Mode.ionian, Mode.dorian, Mode.phrygian, Mode.lydian, Mode.mixolydian, Mode.aeolian, Mode.locrian };
}