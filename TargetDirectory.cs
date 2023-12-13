using SharpCompress.Archives;
using SharpCompress.Common;

public class TargetDirectory
{
    public static readonly HashSet<string> archiveExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".xz"
    };


    public static void ExtractCompressedFiles(string dirName)
    {
        // Get all files in the directory and subdirectories
        var files = Directory.GetFiles(dirName, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            if (!archiveExtensions.Contains(Path.GetExtension(file)))
            {
                continue;
            }

            try
            {
                ExtractArchive(file);
                File.Delete(file);
                string? directoryName = Path.GetDirectoryName(file);
                if (directoryName != null)
                {
                    ExtractCompressedFiles(directoryName);
                }
                else
                {
                    Console.WriteLine($"The extracted directory {directoryName} Doesn't exist.");
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during extraction or deletion
                Console.WriteLine($"An error occurred while processing {file}: {ex.Message}");
            }
        }
    }

    public static void ExtractArchive(string filePath)
    {
        using var archive = ArchiveFactory.Open(filePath);
        foreach (var entry in archive.Entries)
        {
            if (entry.IsDirectory)
            {
                continue;
            }

            string? destinationDirectory = Path.GetDirectoryName(filePath);
            if (destinationDirectory != null)
            {
                entry.WriteToDirectory(destinationDirectory, new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
            }
            else
            {
                // Handle the case where destinationDirectory is null
                Console.WriteLine($"Could not get directory name for {filePath}");
            }
        }
    }
}
