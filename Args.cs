public class Args
{
    public string TargetDirectory {get; set;} = "Scavenger";
    public string ListFile {get; set;} = "scavenger.txt";
    public string OutputDirectory {get; set;}
    public string OutputArchiveName {get; set;} = $"request-{DateTime.Now:yyyy-MMM-dd-HH-mm}.zip";
    public bool Verbosity {get; set;} = false;

    public Args()
    {
        OutputDirectory = Directory.GetCurrentDirectory();
    }

    public static Args? HandleArgs(string[] args)
    {
        Args pargs = new();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-d":
                    pargs.TargetDirectory = args[++i];
                    break;
                case "-l":
                    pargs.ListFile = args[++i];
                    break;
                case "-p":
                    pargs.OutputDirectory = args[++i];
                    break;
                case "-n":
                    pargs.OutputArchiveName = args[++i];
                    break;
                case "-v":
                    pargs.Verbosity = true;
                    break;
            }
        }

        if (!Directory.Exists(pargs.TargetDirectory))
        {
            Console.WriteLine($"The directory {pargs.TargetDirectory} does not exist.");
            return null;
        }

        if (!File.Exists(pargs.ListFile))
        {
            Console.WriteLine($"The list file {pargs.ListFile} does not exist.");
            return null;
        }

        if (pargs.OutputDirectory == null)
        {
            pargs.OutputDirectory = Directory.GetCurrentDirectory();
        }

        if (pargs.OutputArchiveName == null)
        {
            pargs.OutputArchiveName = $"request-{DateTime.Now:yyyy-MMM-dd-HH-mm}.zip";
        }
        
        return pargs;
    }
}
