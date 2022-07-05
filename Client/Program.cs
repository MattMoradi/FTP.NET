// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


void listFiles(String filepath)
{
    DirectoryInfo directory = new DirectoryInfo(@filepath);
    DirectoryInfo[] sub_directories = directory.GetDirectories();
    FileInfo[] files = directory.GetFiles();

    // list directories
    Console.WriteLine();
    foreach (DirectoryInfo i in sub_directories)
    {
        Console.WriteLine("./{0}", i.Name);
    }

    // list files
    Console.WriteLine();
    foreach (FileInfo j in files)
    {
        Console.WriteLine("{0}", j.Name);
    }
}

listFiles("C:\\");