using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleDegree
{
    public int number { get; private set; }
    public Accidental accidental { get; private set; }

    public ScaleDegree(string scaleDegree)
    {
        bool foundStringMatch = false;
        number = 0;
        accidental = Accidental.natural;

        for (int i = 1; i <= 13; i++)
        {
            foreach (Accidental acc in new Accidental[] { Accidental.natural, Accidental.flat, Accidental.sharp, Accidental.doubleFlat, Accidental.doubleSharp })
            {
                if (new ScaleDegree(i, acc).ToString() == scaleDegree)
                {
                    if (!(i == 1 && acc == Accidental.flat) && !(i == 1 && acc == Accidental.doubleFlat))
                    {
                        number = i;
                        accidental = acc;
                        foundStringMatch = true;
                        break;
                    }
                }
            }
        }

        if (!foundStringMatch)
        {
            throw new System.Exception("Invalid scale degree string: " + scaleDegree);
        }
    }
    public ScaleDegree(int number, Accidental accidental)
    {
        this.number = number;
        this.accidental = accidental;
    }

    public static bool operator ==(ScaleDegree scaleDegree1, ScaleDegree scaleDegree2)
    {
        return scaleDegree1.number == scaleDegree2.number && scaleDegree1.accidental == scaleDegree2.accidental;
    }
    public static bool operator !=(ScaleDegree scaleDegree1, ScaleDegree scaleDegree2)
    {
        return !(scaleDegree1 == scaleDegree2);
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(ScaleDegree))
        {
            return (ScaleDegree)obj == this;
        }
        return base.Equals(obj);
    }

    public override string ToString()
    {
        switch (accidental)
        {
            case Accidental.natural: return number.ToString();
            case Accidental.flat: return "b" + number;
            case Accidental.sharp: return "#" + number;
            case Accidental.doubleFlat: return "bb" + number;
            case Accidental.doubleSharp: return "x" + number;
            default: throw new System.Exception("Unknown accidental: " + accidental);
        }
    }
}
