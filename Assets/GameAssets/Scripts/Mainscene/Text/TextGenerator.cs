using UnityEngine;
using TMPro;
using System.Collections.Generic;
public static class TextGenerator
{
    /// <summary>
    /// Creates a TextMeshPro-compatible string using sprite asset glyphs.
    /// </summary>
    /// <param name="input">Input string to convert.</param>
    /// <param name="Prefix">Base prefix for lowercase glyphs (e.g., "Font_main_").</param>
    /// <param name="Prefix_UpperCase">Uppercase identifier (e.g., "Uc_").</param>
    /// <param name="text">TMP_Text component to apply output.</param>
    /// <param name="spriteAsset">Sprite asset containing glyphs.</param>
    /// <param name="charReferences">Custom character replacements, if any.</param>
    public static void CreateSpriteAssetText (
          string input ,
          string Prefix ,
          string Prefix_UpperCase ,
          TMP_Text text ,
          TMP_SpriteAsset spriteAsset ,
          List<(string actual, string available)> charReferences = null )
    {
        string spriteCode = "<sprite name=";
        string prefix = spriteCode + Prefix;
        string suffix = ">";
        string result = string.Empty;
        List<int> spaceIndex = new List<int>();
        List<(string character, bool isUpperCase)> characters = new List<(string character, bool isUpperCase)>();

        foreach (char k in input)
        {
            if (char.IsUpper(k))
            {
                characters.Add((k.ToString(), true));
            }
            else if (char.IsLower(k))
            {
                characters.Add((k.ToString(), false));
            }
            else
            {
                characters.Add((k.ToString(), false));
            }
        }

        Debug.Log(input);

        foreach (var (character, isUpperCase) in characters)
        {
            if (character != null)
            {
                string spriteName = null;

                if (character == " ")
                {
                    result += " ";
                    continue;
                }

                foreach (var (actual, available) in charReferences)
                {
                    if (actual == character)
                    {
                        spriteName = available;
                        result += prefix + spriteName + suffix;
                        continue;
                    }
                }

                string finalPrefix = isUpperCase ? prefix + Prefix_UpperCase : prefix;

                if (spriteAsset.GetSpriteIndexFromName(finalPrefix + character) < 0)
                {
                    result += finalPrefix + character + suffix;
                }
                else
                {
                    result += character;
                }
            }
        }

        text.spriteAsset = spriteAsset;
        text.text = result;
    }
}
