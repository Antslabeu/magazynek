using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Magazynek.Data;

public static class PasswordMeter
{
    public enum Level { Bad, Enough, Good }

    public static Level Evaluate(string? pwd)
    {
        pwd ??= string.Empty;
        int len = pwd.Length;

        bool hasLower = pwd.Any(char.IsLower);
        bool hasUpper = pwd.Any(char.IsUpper);
        bool hasDigit = pwd.Any(char.IsDigit);
        bool hasSymbol = pwd.Any(ch => !char.IsLetterOrDigit(ch));

        int pool = 0;
        if (hasLower) pool += 26;
        if (hasUpper) pool += 26;
        if (hasDigit) pool += 10;
        if (hasSymbol) pool += 33;           // przybliżenie ASCII znaków specjalnych

        double bits = (len > 0 && pool > 0) ? len * Math.Log(pool, 2) : 0;

        // --- KARY (proste, ale skuteczne) ---
        double penalty = 0;

        // 1) Duże powtórzenia
        double distinctRatio = (len == 0) ? 1 : (pwd.Distinct().Count() / (double)len);
        if (distinctRatio < 0.5) penalty += 10;
        else if (distinctRatio < 0.7) penalty += 5;

        // 2) Sekwencje rosnące/malejące (≥3 znaki)
        if (HasSequentialRun(pwd, 3)) penalty += 10;

        // 3) Popularne wzorce/słowa (PL/EN)
        var lower = pwd.ToLowerInvariant();
        string[] badFragments = { "password", "haslo", "qwerty", "admin", "iloveyou", "1234", "1111", "letmein", "monkey", "dragon" };
        if (badFragments.Any(lower.Contains)) penalty += 15;

        bits = Math.Max(0, bits - penalty);

        // --- Mapa poziomów ---
        Level level =
            (len < 8 || bits < 40) ? Level.Bad :
            (bits < 60) ? Level.Enough : Level.Good;

        return level;
    }

    private static bool HasSequentialRun(string s, int minLen)
    {
        if (s.Length < minLen) return false;
        int up = 1, down = 1;
        for (int i = 1; i < s.Length; i++)
        {
            if (s[i] == s[i - 1] + 1) { up++; down = 1; }       // rosnąca
            else if (s[i] == s[i - 1] - 1) { down++; up = 1; }   // malejąca
            else { up = down = 1; }
            if (up >= minLen || down >= minLen) return true;
        }
        return false;
    }
}