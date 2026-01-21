using System.Text;

namespace FamilyTreeUtilities.Services
{
    public class SoundexService
    {
        public string GetSoundex(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "0000";

            // Convert to uppercase and remove non-letters
            name = name.ToUpper();
            var cleanName = new StringBuilder();
            foreach (char c in name)
            {
                if (char.IsLetter(c))
                    cleanName.Append(c);
            }

            if (cleanName.Length == 0)
                return "0000";

            string word = cleanName.ToString();
            var soundex = new StringBuilder();

            // Keep first letter
            soundex.Append(word[0]);

            // Encode remaining letters
            char previousCode = GetSoundexCode(word[0]);

            for (int i = 1; i < word.Length && soundex.Length < 4; i++)
            {
                char code = GetSoundexCode(word[i]);

                // Skip vowels and H, W, Y
                if (code == '0')
                    continue;

                // Don't add duplicate codes
                if (code != previousCode)
                {
                    soundex.Append(code);
                    previousCode = code;
                }
            }

            // Pad with zeros
            while (soundex.Length < 4)
                soundex.Append('0');

            return soundex.ToString();
        }

        private char GetSoundexCode(char c)
        {
            switch (c)
            {
                case 'B':
                case 'F':
                case 'P':
                case 'V':
                    return '1';
                case 'C':
                case 'G':
                case 'J':
                case 'K':
                case 'Q':
                case 'S':
                case 'X':
                case 'Z':
                    return '2';
                case 'D':
                case 'T':
                    return '3';
                case 'L':
                    return '4';
                case 'M':
                case 'N':
                    return '5';
                case 'R':
                    return '6';
                default:
                    return '0'; // A, E, I, O, U, H, W, Y
            }
        }
    }
}