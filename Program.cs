﻿using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharpCompress.Archives;
using SharpCompress.Common;

class Program
{
    static void Main(string[] args)
    {
        string? directory = null;
        string? listFile = null;
        string? outputPath = null;
        string? outputArchiveName = null;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-d":
                    directory = args[++i];
                    break;
                case "-l":
                    listFile = args[++i];
                    break;
                case "-p":
                    outputPath = args[++i];
                    break;
                case "-n":
                    outputArchiveName = args[++i];
                    break;
            }
        }

        if (directory == null || listFile == null)
        {
            Console.WriteLine("Missing required arguments.");
            return;
        }

        if (outputPath == null)
        {
            outputPath = Directory.GetCurrentDirectory();
        }

        if (outputArchiveName == null)
        {
            outputArchiveName = $"request-{DateTime.Now:yyyy-MMM-dd-HH-mm}.zip";
        }

        // Extract all compressed files in the directory
        foreach (var file in Directory.EnumerateFiles(directory))
        {
            using (var archive = ArchiveFactory.Open(file))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(directory, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }

        // Read the list file and find all files according to the sections
        var fileNames = new List<string>();
        var filePaths = new List<string>();
        var dirPaths = new List<string>();

        var section = "";

        foreach (var line in File.ReadLines(listFile))
        {
            if (Regex.IsMatch(line, @"^\[.*\]$"))
            {
                section = line;
            }
            else
            {
                switch (section)
                {
                    case "[file_names]":
                        fileNames.Add(line);
                        break;
                    case "[file_paths]":
                        filePaths.Add(line);
                        break;
                    case "[files_under_directories]":
                        dirPaths.Add(line);
                        break;
                }
            }
        }

        var foundFiles = new List<string>();

        // Find files by name
        foreach (var fileName in fileNames)
        {
            foundFiles.AddRange(Directory.GetFiles(directory, fileName, SearchOption.AllDirectories));
        }

        // Find files by path
        foreach (var filePath in filePaths)
        {
            if (File.Exists(filePath))
            {
                foundFiles.Add(filePath);
            }
        }

        // Find files under directories
        foreach (var dirPath in dirPaths)
        {
            if (Directory.Exists(dirPath))
            {
                foundFiles.AddRange(Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories));
            }
        }

        // Add the found files to a new compressed archive
        using (var archive = new ZipArchive(File.Open(outputArchiveName, FileMode.Create), ZipArchiveMode.Create))
        {
            foreach (var file in foundFiles)
            {
                archive.CreateEntryFromFile(file, Path.GetFileName(file));
            }
        }
    }
}
