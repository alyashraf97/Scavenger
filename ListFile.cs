using System.Text.RegularExpressions;

public class ListFile
{
    public List<string>? FileNames { get; set; }
    public List<string>? Paths { get; set; }
    public List<string>? Dirs { get; set; }

    public static ListFile ParseListFile(string filePath)
    {
        var fileData = new ListFile
        {
            FileNames = [],
            Paths = [],
            Dirs = []
        };

        List<string>? currentList = null;
        var regex = new Regex(@"^\[(files|paths|dirs)\]$");
        var sectionsFound = new HashSet<string>();

        foreach (var line in File.ReadLines(filePath))
        {
            if (regex.IsMatch(line))
            {
                if (sectionsFound.Contains(line))
                {
                    throw new Exception($"Duplicate section header: {line}");
                }
                sectionsFound.Add(line);

                switch (line)
                {
                    case "[files]":
                        currentList = fileData.FileNames;
                        break;
                    case "[paths]":
                        currentList = fileData.Paths;
                        break;
                    case "[dirs]":
                        currentList = fileData.Dirs;
                        break;
                    default:
                        currentList = null;
                        break;
                }
            }
            else if (currentList != null && !string.IsNullOrWhiteSpace(line))
            {
                currentList.Add(line);
            }
        }

        if (sectionsFound.Count < 1)
        {
            throw new Exception("At least one section header is required.");
        }

        return fileData;
    }
}


/*

    private static List<string> ReadSection(StreamReader reader)
    {
        var list = new List<string>();
        string? line;
        while (reader.ReadLine() != null && !line.StartsWith("["))
        {
            list.Add(line);
        }
        return list;
    }
    
    */
