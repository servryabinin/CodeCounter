using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        string folderPath;

        if (args.Length == 1)
        {
            folderPath = args[0];
        }
        else
        {
            Console.Write("Введите путь к папке с JSON-файлами: ");
            folderPath = Console.ReadLine();
        }

        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
        {
            Console.WriteLine("Указанная папка не существует.");
            return;
        }

        string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
        string outputFilePath = Path.Combine(folderPath, "result.sql");

        using (StreamWriter writer = new StreamWriter(outputFilePath, false))
        {
            foreach (string filePath in jsonFiles)
            {
                try
                {
                    string jsonText = File.ReadAllText(filePath);
                    int codeCount = CountCodeOccurrences(jsonText);

                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string[] parts = fileName.Split(' ');

                    if (parts.Length < 2 || !int.TryParse(parts[^1], out int jobId))
                    {
                        Console.WriteLine($"Не удалось извлечь Id из имени файла: {fileName}");
                        continue;
                    }

                    string updateStatement = $"UPDATE Jobs SET CurrentCode = {codeCount} WHERE Id = {jobId} GO";
                    writer.WriteLine(updateStatement);
                    Console.WriteLine($"Обработан файл: {fileName} — Найдено кодов: {codeCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка обработки файла {filePath}: {ex.Message}");
                }
            }
        }

        Console.WriteLine($"\nГотово! Результаты сохранены в файл: {outputFilePath}");
    }

    static int CountCodeOccurrences(string jsonText)
    {
        return Regex.Matches(jsonText, "\"code\"").Count;
    }
}
