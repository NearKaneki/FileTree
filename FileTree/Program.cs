using FileTree;

FileTreeApp treeBuilder = new FileTreeApp(new ArgumentInstaller(args));
treeBuilder.Start();

Console.WriteLine("Enter any key to exit.");
Console.ReadKey();