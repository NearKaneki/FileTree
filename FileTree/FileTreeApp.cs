using System.Text;

namespace FileTree
{
    public class FileTreeApp
    {
        ArgumentInstaller _argumentInstaller;

        private string _checkFolderPath;
        private string _outputFilePath;
        private bool _quite;
        private bool _humanread;

        private List<Entity> _entities;
        private StringBuilder _treeString;

        public FileTreeApp(ArgumentInstaller argumentInstaller)
        {
            _argumentInstaller = argumentInstaller;
            _entities = new List<Entity>();
            _treeString = new StringBuilder();
        }

        public void Start()
        {
            InstallArgument();
            BuildTree(1, new DirectoryInfo(_checkFolderPath));
            GenerateString();
            if (!_quite)
            {
                ConsoleOutput();
            }
            FileOutput();
        }

        private void InstallArgument()
        {
            _argumentInstaller.SetupDefaultValues(out _checkFolderPath, out _outputFilePath);
            _argumentInstaller.CheckParams(ref _checkFolderPath, ref _outputFilePath, ref _quite, ref _humanread);
        }

        private long BuildTree(int nestingCount, DirectoryInfo currentDirectory)
        {
            try
            {
                _entities.Add(new Entity
                {
                    NameFile = currentDirectory.Name,
                    Size = 0,
                    NestingCount = nestingCount
                });
                int idDirectoryInList = _entities.Count - 1;
                nestingCount++;
                long size = 0;

                FileInfo[] files;
                try
                {
                    files = currentDirectory.GetFiles();
                }
                catch (UnauthorizedAccessException)
                {
                    _entities[idDirectoryInList].NameFile = "UnauthorizedAccessException.folder";
                    _entities[idDirectoryInList].Size = 0;
                    return 0;
                }
                foreach (var file in files)
                {
                    size += file.Length;
                    _entities.Add(new Entity
                    {
                        NameFile = file.Name,
                        Size = file.Length,
                        NestingCount = nestingCount
                    });
                }

                DirectoryInfo[] directories = null;
                try
                {
                    directories = currentDirectory.GetDirectories();
                }
                catch (UnauthorizedAccessException)
                {
                    _entities[idDirectoryInList].NameFile = "UnauthorizedAccessException.folder";
                    _entities[idDirectoryInList].Size = size;
                    return 0;
                }
                foreach (var directory in directories)
                {
                    size += BuildTree(nestingCount, directory);
                }

                _entities[idDirectoryInList].Size = size;
                return size;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void GenerateString()
        {
            _entities[0].NameFile = _checkFolderPath;
            if (_humanread)
            {
                List<string> humanreadSizes = MakeHumanSizes();
                for (int i = 0; i < _entities.Count; ++i)
                {
                    _treeString.Append($"{new string('-', _entities[i].NestingCount)} {_entities[i].NameFile} {humanreadSizes[i]}{Environment.NewLine}");
                }
                return;
            }
            foreach (var item in _entities)
            {
                _treeString.Append($"{new string('-', item.NestingCount)} {item.NameFile} ({item.Size} byte){Environment.NewLine}");
            }
        }

        private List<string> MakeHumanSizes()
        {
            const long TiB = 1099511627776;
            const long GiB = 1073741824;
            const long MiB = 1048576;
            const long KiB = 1024;
            List<string> humanreadSizes = new List<string>();
            foreach (var item in _entities)
            {
                switch (item.Size)
                {
                    case < KiB:
                        humanreadSizes.Add($"({item.Size} B)");
                        break;
                    case < MiB:
                        humanreadSizes.Add($"({Math.Round((double)item.Size / KiB, 2)} KB)");
                        break;
                    case < GiB:
                        humanreadSizes.Add($"({Math.Round((double)item.Size / MiB, 2)} MB)");
                        break;
                    case < TiB:
                        humanreadSizes.Add($"({Math.Round((double)item.Size / GiB, 2)} GB)");
                        break;
                    case >= TiB:
                        humanreadSizes.Add($"({Math.Round((double)item.Size / TiB, 2)} TB)");
                        break;
                }
            }
            return humanreadSizes;
        }

        //Думал переопределить ToString, но вызывать или не вызывать консоль из Program - не понятно(не знаем есть ли требуемый атрибут)
        private void ConsoleOutput()
        {
            Console.WriteLine(_treeString);
        }

        //Думал переопределить ToString, но путь файла, куда сохранять, из Program - не ясен(не знаем есть ли требуемый атрибут)
        private void FileOutput()
        {
            using (StreamWriter output = new StreamWriter(_outputFilePath))
            {
                output.Write(_treeString);
            }
        }
    }
}