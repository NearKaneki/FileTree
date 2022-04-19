namespace FileTree
{
    public class ArgumentInstaller
    {
        private List<string> _args;
        public ArgumentInstaller(string[] args)
        {
            _args = args.ToList();
        }
        public void SetupDefaultValues(out string checkFolderPath, out string outputFilePath)
        {
            SetupDefaultFolder(out checkFolderPath);
            SetupDefaultFile(out outputFilePath);
        }

        static private void SetupDefaultFolder(out string checkFolderPath)
        {
            checkFolderPath = Directory.GetCurrentDirectory();
        }

        static private void SetupDefaultFile(out string outputFilePath)
        {
            DateTime dateTime = DateTime.Now;
            string month = Make2Symbols(dateTime.Month.ToString());
            string day = Make2Symbols(dateTime.Day.ToString());

            outputFilePath = $"{Directory.GetCurrentDirectory()}\\sizes-{dateTime.Year}-{month}-{day}.txt";

            using (File.Create(outputFilePath)) { };
        }

        static private string Make2Symbols(string digit)
        {
            if (digit.Length == 1)
            {
                return $"0{digit}";
            }
            return digit;
        }

        public void CheckParams(ref string checkFolderPath, ref string outputFilePath, ref bool quite, ref bool humanread)
        {
            if (_args.Contains("-q") || _args.Contains("--quite"))
            {
                quite = true;
            }

            if (_args.Contains("-h") || _args.Contains("--humanread"))
            {
                humanread = true;
            }

            if (_args.Contains("-p") || _args.Contains("--path"))
            {
                int indexP = _args.FindIndex(x => x == "-p" || x == "--path") + 1;
                if (indexP < _args.Count && Directory.Exists(_args[indexP]))
                {
                    checkFolderPath = _args[indexP];
                }
                else
                {
                    Console.WriteLine("Incorrect path for folder.");
                    Environment.Exit(1);
                }
            }

            if (_args.Contains("-o") || _args.Contains("--output"))
            {
                int indexO = _args.FindIndex(x => x == "-o" || x == "--output") + 1;
                if (indexO < _args.Count && File.Exists(_args[indexO]) && Path.GetExtension(_args[indexO]) == ".txt")
                {
                    outputFilePath = _args[indexO];
                }
                else
                {
                    Console.WriteLine("Incorrect path for output file.");
                    Environment.Exit(1);
                }
            }
        }
    }
}