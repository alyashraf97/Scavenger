using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using SharpCompress.Archives;
using SharpCompress.Common;



class Program
{
    static void Main(string[] pargs)
    {        
        Args? args = Args.HandleArgs(pargs);

        if (args == null)
        {
            Console.WriteLine("Must supply valid arguments (-d <target_dir> and -l <list_file> or have a dir named" +
                             " Scavenger and a list file called scavenger.txt).");
            return;
        }

        ListFile targets = ListFile.ParseListFile(args.ListFile);
        
        // Extract all compressed files in the directory
        try
        {
            TargetDirectory.ExtractCompressedFiles(args.TargetDirectory);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        // Read the list file and find all files according to the sections
        

        var foundFiles = new List<string>();

        // Find files by name
        if (targets.Files != null)
        {            
            foreach (var fileName in targets.Files)
            {
                foundFiles.AddRange(Directory.GetFiles(args.TargetDirectory, fileName, SearchOption.AllDirectories));
            }
        }

        // Find files by path
        if (targets.Paths != null)
        {
            foreach (var filePath in targets.Paths)
            {
                if (File.Exists(filePath))
                {
                    foundFiles.Add(filePath);
                }
            }
        }

        // Find files under directories
        if (targets.Dirs != null)
        {
            foreach (var dirPath in targets.Dirs)
            {
                if (Directory.Exists(dirPath))
                {
                    foundFiles.AddRange(Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories));
                }
            }
        }

        // Add the found files to a new compressed archive
        try
        {
            using (var archive = new ZipArchive(File.Open(args.OutputArchiveName, FileMode.Create), ZipArchiveMode.Create))
            {
                foreach (var file in foundFiles)
                {
                    Console.WriteLine($"Adding {file} to archive...");
                    archive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating the archive {args.OutputArchiveName}: {ex.Message}");
            return;
        }
    }

}


