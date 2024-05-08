using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class Functions : MonoBehaviour
{
    public static string UserInputCheck(string input, int minLength, int newCreateLength)
    {
        if (input.IsNullOrEmpty() || input.Length <= minLength)
        {
            return CreateRandomString(newCreateLength);
        }
        return input.Trim().Replace(' ', '_').ToString();
    }

    public static string CreateRandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";
        var stringChars = new char[length];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new String(stringChars);

    }
}
