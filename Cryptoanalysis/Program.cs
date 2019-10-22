using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptoanalysis
{

    class Program
    {
        private enum Options
        {
            CAESAR_ENCRYPT,
            MONOALPHABETIC_ENCRYPT,
            VIGINERE_ENCRYPT,
            CAESAR_DECRYPT,
            MONOALPHABETIC_BREAK,
            VIGINERE_BREAK
        }

        static private Dictionary<Options, Action> OptionsMap = new Dictionary<Options, Action>
        {
            { Options.CAESAR_ENCRYPT, CaesarEncrypt },
            { Options.MONOALPHABETIC_ENCRYPT, MonoEncrypt },
            { Options.VIGINERE_ENCRYPT, ViginereEncrypt },
            { Options.CAESAR_DECRYPT, CaesarDecrypt },
            { Options.MONOALPHABETIC_BREAK, MonoBreak },
            { Options.VIGINERE_BREAK, ViginereBreak }
        };

        public static char[] englishLetterFrequency = "ETAOINSHRDLUCMFWPGYBVKXQJZ".ToCharArray();

        public static char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();


        private static string MonoMapper(string input)
        {
            return new string(input.Select(a => (char)(a * 2)).ToArray());
        }


        private static int Mod(int a, int b)
        {
            return (a % b + b) % b;
        }

        private static string Viginere(string input, string key, bool encipher)
        {
            for (int i = 0; i < key.Length; ++i)
                if (!char.IsLetter(key[i]))
                    return null; // Error

            string output = string.Empty;
            int nonAlphaCharCount = 0;

            for (int i = 0; i < input.Length; ++i)
            {
                if (char.IsLetter(input[i]))
                {
                    bool cIsUpper = char.IsUpper(input[i]);
                    char offset = cIsUpper ? 'A' : 'a';
                    int keyIndex = (i - nonAlphaCharCount) % key.Length;
                    int k = (cIsUpper ? char.ToUpper(key[keyIndex]) : char.ToLower(key[keyIndex])) - offset;
                    k = encipher ? k : -k;
                    char ch = (char)((Mod(((input[i] + k) - offset), 26)) + offset);
                    output += ch;
                }
                else
                {
                    output += input[i];
                    ++nonAlphaCharCount;
                }
            }

            return output;
        }

        private static string Caesar(string text, int key)
        {
            StringBuilder ret = new StringBuilder("");

            foreach (char c in text)
            {
                if (Char.IsLetter(c))
                {
                    if (Char.IsUpper(c))
                        ret.Append(Char.ToUpper(alphabet[(Array.IndexOf(alphabet, Char.ToLower(c)) + key) % 26]));
                    else
                        ret.Append(alphabet[(Array.IndexOf(alphabet, c) + key) % 26]);
                }
                else
                    ret.Append(c);
            }

            return ret.ToString();
        }

        static void Main(string[] args)
        {
            List<Options> options = Enum.GetValues(typeof(Options)).Cast<Options>().ToList();
            string optionsString = string.Join(", ", options.Select(o => o.ToString()));

            string input = GetInput($"Please choose an option: {optionsString}\n")[0];

            Options option = (Options)Enum.Parse(typeof(Options), input);

            OptionsMap[option].Invoke();
        }



        private static List<string> GetInput(params string[] prompts)
        {
            if (prompts.Equals(Array.Empty<string>()))
            {
                return new List<string>() { Console.ReadLine() };
            }

            List<string> outputs = new List<string>();

            foreach(string prompt in prompts)
            {
                Console.Write(prompt);
                outputs.Add(Console.ReadLine());
            }


            return outputs;
        }



        private static void CaesarEncrypt()
        {
            string text;
            int key;

            List<string> input = GetInput("Enter text to encrypt: \n", "Enter encryption key: ");
            Console.WriteLine();

            text = input[0];
            key = int.Parse(input[1]);


            Console.WriteLine(Caesar(text, key));
            Console.ReadKey();
        }

        private static void MonoEncrypt()
        {
            string input = GetInput("Enter text to encrypt (default mapping function that maps each character to it's ASCII double will be used): \n")[0];
            Console.WriteLine(MonoMapper(input));
            Console.ReadKey();
        }

        private static void ViginereEncrypt()
        {
            string text = GetInput("Enter text to encrypt: \n")[0];
            string key = GetInput("Enter encryption key: ")[0];

            Console.WriteLine(Viginere(text, key, true));
            Console.ReadKey();
        }

        private static void CaesarDecrypt()
        {
            string text = GetInput("Enter text to decrypt: \n")[0];
            int key = -int.Parse(GetInput("Enter decryption key: ")[0]);


            Console.WriteLine(Caesar(text, key));
            Console.ReadKey();
        }

        private static void MonoBreak()
        {
            string text = GetInput("Enter text to run analysis on: \n")[0].ToLower();

            Dictionary<char, int> frequencies = new Dictionary<char, int>();

            foreach(char c in text)
            {
                try
                {
                    ++frequencies[c];
                }
                catch
                {
                    frequencies.Add(c, 1);
                }
            }

            List<char> ordered = frequencies.OrderByDescending(kp => kp.Value).Select(kp => kp.Key).ToList();
            for(int i = 0; i < ordered.Count; ++i)
            {
                text = text.Replace(ordered[i], englishLetterFrequency[i]);
            }


            GetInput("Probable original text: \n");
            Console.WriteLine(text);
            Console.ReadKey();
        }

        private static void ViginereBreak()
        {

        }
    }
}
