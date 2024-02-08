using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class Functions
{
    /// <summary>
    /// Does a mod b, giving a non-negative answer.
    /// </summary>
    /// <returns></returns>
    public static int Mod(int a, int b)
    {
        return ((a % b) + b) % b;
    }

    /// <summary>
    /// Mod(), but counting from 1 to b instead of 0 to b-1.
    /// </summary>
    /// <returns></returns>
    public static int ModCount1(int a, int b)
    {
        return Mod(a - 1, b) + 1;
    }

    public static string ToUpperFirstLetter(string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return string.Empty;
        }

        char[] letters = source.ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new string(letters);
    }

    public static string ArrayToString<T>(T[] array)
    {
        string str = "{ ";
        for (int i = 0; i < array.Length - 1; i++)
        {
            str += array[i].ToString() + ", ";
        }
        str += array[array.Length - 1].ToString();
        str += " }";
        return str;
    }

    public static string RemoveWhitespace(string str)
    {
        return String.Concat(str.Where(c => !Char.IsWhiteSpace(c)));
    }

    public static int FloorTowardsZero(float f)
    {
        if (f >= 0)
        {
            return Mathf.FloorToInt(f);
        }
        else
        {
            return Mathf.CeilToInt(f);
        }
    }
    public static int CeilFromZero(float f)
    {
        if (f >= 0)
        {
            return Mathf.CeilToInt(f);
        }
        else
        {
            return Mathf.FloorToInt(f);
        }
    }

    public static bool RandomBool(float probability = 0.5f)
    {
        return UnityEngine.Random.Range(0f, 1f) < probability;
    }

    public static float DecimalPart(float x)
    {
        return x - IntegerPart(x);
    }
    public static float IntegerPart(float x)
    {
        return FloorTowardsZero(x);
    }

    /// <summary>
    /// Returns the smallest multiple of multipleOf greater than or equal to startingPoint. For negative startingPoint, it is less than or equal.
    /// </summary>
    public static float NextMultipleOf(float startingPoint, float multipleOf)
    {
        return CeilFromZero(startingPoint / multipleOf) * multipleOf;
    }
    /// <summary>
    /// Returns the smallest multiple of multipleOf greater than or equal to startingPoint. For negative startingPoint, it is less than or equal.
    /// </summary>
    public static int NextMultipleOf(float startingPoint, int multipleOf)
    {
        return CeilFromZero(startingPoint / multipleOf) * multipleOf;
    }

    public static bool IsInteger(float x)
    {
        return Mathf.Round(x) == x;
    }

    public static int WeightedRand(float[] weights)
    {
        if (weights.Length == 0)
        {
            throw new Exception("Weights cannot be empty.");
        }

        float sum = Enumerable.Sum(weights);

        float rand = UnityEngine.Random.Range(0f, sum);

        float runningTotal = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            runningTotal += weights[i];
            if (runningTotal >= rand)
            {
                return i;
            }
        }

        throw new Exception("Something went wrong and we came out without getting the weighted rand.");
    }

    public static bool InRange(float x, float start, float end, bool leftInclusive = true, bool rightInclusive = true)
    {
        return (start < x && x < end) || (x == start && leftInclusive) || (x == end && rightInclusive);
    }
}
