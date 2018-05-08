using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LancuchyMarkowa
{
    class Program
    {
        static private Dictionary<KeyValuePair<string, string>, int> dictionary =
            new Dictionary<KeyValuePair<string, string>, int>();

        static void LoadDict(string[][] texts)
        {
            foreach (string[] words in texts)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    KeyValuePair<string, string> key;
                    key = i == 0 ? new KeyValuePair<string, string>(".", words[i]) : new KeyValuePair<string, string>(words[i-1], words[i]);
                    if (dictionary.ContainsKey(key))
                    {
                        dictionary[key] += 1;
                    }
                    else
                    {
                        dictionary.Add(key, 1);
                    }
                }
            }
        }

        static string[] OpenFile(string path)
        {
            string s = "";
            using (var sr = File.OpenText(path))
            {
                s = sr.ReadToEnd();
            }


            List<string> words = new List<string>();
            string word = "";
            foreach (char c in s)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    word += c;
                }
                else if (word.Length > 0)
                {
                    words.Add(word);
                    word = "";
                    if (c == '.')
                    {
                        string temp = "";
                        temp += c;
                        words.Add(temp);
                    }
                }
            }

            return words.ToArray();
        }

        static string[] GetFiles(DirectoryInfo directory)
        {
            List<string> list = new List<string>();

            foreach (FileInfo file in directory.GetFiles())
            {
                list.Add(file.FullName);
            }

            return list.ToArray();
        }

        static string Generate(int numberOfWords)
        {
            Random random = new Random();
            string result = "";
            var keys = dictionary.Keys;
            string prev = ".";
            for (int i = 0; i < numberOfWords; i++)
            {
                Dictionary<string, double> tempDict = new Dictionary<string, double>();
                foreach (var key in keys)
                {
                    if (key.Key == prev)
                    {
                        if (tempDict.ContainsKey(key.Value))
                        {
                            tempDict[key.Value] += 1;
                        }
                        else
                        {
                            tempDict.Add(key.Value, 1);
                        }
                    }
                }

                int sum = 0;
                foreach (var x in tempDict)
                {
                    sum += (int)x.Value;
                }
                foreach (var x in tempDict.ToList())
                {
                    tempDict[x.Key] /= sum ;
                }

                double rand = random.NextDouble();
                foreach (var x in tempDict)
                {
                    if (rand <= x.Value)
                    {
                        result += x.Key + " ";
                        prev = x.Key;
                        break;
                    }
                    else
                    {
                        rand -= x.Value;
                    }
                }

            }

            return result;
        }

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid args number");
                Console.ReadKey();
                return;
            }

            string[] filesPaths = GetFiles(new DirectoryInfo(args[0]));
            List<string[]> texts = new List<string[]>();
            foreach (string filePath in filesPaths)
            {
                texts.Add(OpenFile(filePath));
            }

            LoadDict(texts.ToArray());

            Console.WriteLine(Generate(int.Parse(args[1])));
            Console.ReadKey();
        }
    }
}
