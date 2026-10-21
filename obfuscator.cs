using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LuauObfuscator
{
    private static readonly Random random = new Random();

    public static string Obfuscate(string sourceCode)
    {
        string code = StripComments(sourceCode);
        code = RenameLocalVariables(code);
        return code;
    }

    private static string StripComments(string code)
    {
        code = Regex.Replace(code, @"--\[\[.*?\]\]", "", RegexOptions.Singleline);
        code = Regex.Replace(code, @"--.*$", "", RegexOptions.Multiline);
        return code;
    }

    private static string RenameLocalVariables(string code)
    {
        var localPattern = new Regex(@"\blocal\s+([a-zA-Z_][a-zA-Z0-9_]*)");
        var matches = localPattern.Matches(code);
        
        var renameMap = new Dictionary<string, string>();
        int counter = 0;

        foreach (Match match in matches)
        {
            if (match.Groups.Count > 1)
            {
                string originalName = match.Groups[1].Value;
                if (!renameMap.ContainsKey(originalName))
                {
                    string newName = "_" + GenerateRandomString(6);
                    renameMap[originalName] = newName;
                }
            }
        }

        foreach (var pair in renameMap)
        {
            var wordPattern = new Regex(@"\b" + pair.Key + @"\b");
            code = wordPattern.Replace(code, pair.Value);
        }

        return code;
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] stringChars = new char[length];
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
        return new string(stringChars);
    }

    public static void Main(string[] args)
    {
        string sampleScript = @"
            local IntegrityMesh = {}
            local Players = game:GetService('Players')
            function IntegrityMesh.init()
                local localPlayer = Players.LocalPlayer
                print(localPlayer.Name)
            end
            return IntegrityMesh
        ";

        string obfuscated = Obfuscate(sampleScript);
        Console.WriteLine(obfuscated);
    }
}
