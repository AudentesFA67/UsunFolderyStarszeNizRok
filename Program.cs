using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

class Program
{
    static void Main()
    {
        string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
        string PathOfArchive = "";

        // 1. Odczyt konfiguracji z pliku .ini
        if (File.Exists(iniPath))
        {
            string[] lines = File.ReadAllLines(iniPath);
            foreach (var line in lines)
            {
                if (line.StartsWith("Path=", StringComparison.OrdinalIgnoreCase))
                {
                    // Pobieramy nazwę i oczyszczamy z rozszerzenia .exe, jeśli użytkownik je wpisał
                    PathOfArchive = line.Split('=')[1].Trim();
                }
            }
        }

        // 2. Walidacja – czy podano nazwę aplikacji
        if (string.IsNullOrEmpty(PathOfArchive))
        {
            MessageBox.Show("Nie wskazano ścieżki do archiwum w config.ini.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }


        DateTime referenceDate = DateTime.Today;
        string referenceDateString = referenceDate.ToString("yyyy-MM-dd");
        Console.WriteLine($"Dzisiejsza data: {referenceDateString}");
        int counter1 = 0;


        if (!DateTime.TryParseExact(
                referenceDateString,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime referenceDate2))
        {
            Console.WriteLine("Nieprawidłowy format daty.");
            return;
        }

        string path = PathOfArchive;
        Console.WriteLine($"Ścieżka do katalogu: {path}");
        Console.WriteLine("");

        if (!Directory.Exists(path))
        {
            Console.WriteLine("Podany katalog nie istnieje.");
            return;
        }

        DateTime limitDate = referenceDate2.AddYears(-1);

        foreach (string dir in Directory.GetDirectories(path))
        {
            string folderName = Path.GetFileName(dir);

            if (DateTime.TryParseExact(
                    folderName,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime folderDate))
            {
                if (folderDate < limitDate)
                {
                    try
                    {
                        Directory.Delete(dir, true); // true = usuwa również zawartość

                        Console.WriteLine($"Usunięto: {folderName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd podczas usuwania {folderName}: {ex.Message}");
                    }
                }
                else
                {
                    if (counter1 == 0)
                    {
                        Console.WriteLine(" ");
                        Console.Write("Pozostawiono: ");
                    }

                    Console.Write($"{folderName}; ");
                    counter1++;
                }
            }
            else
            {
                Console.WriteLine($"Pominięto (niepoprawna nazwa): {folderName}");
            }

        }
        Console.WriteLine(); // Nowa linia po wypisaniu pozostawionych folderów
        Console.WriteLine();
        Console.WriteLine("Operacja zakończona.");

        for (int i = 10; i > 0; i--)
        {
            Console.Write($"\rZamknięcie programu za {i} sekund...   ");
            Thread.Sleep(1000);
        }

        Console.WriteLine("\rZamknięcie programu...           ");
    }
}