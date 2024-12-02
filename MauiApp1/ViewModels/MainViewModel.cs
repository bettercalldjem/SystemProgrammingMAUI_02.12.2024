using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers; // Используем System.Timers.Timer
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;

namespace MauiApp1.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // Свойства для отображения данных
        public ObservableCollection<ProcessModel> Processes { get; } = new();
        public ObservableCollection<FileSystemItem> FileSystemItems { get; } = new();

        // Свойства для ресурсов
        [ObservableProperty]
        private string cpuUsage;

        [ObservableProperty]
        private string availableMemory;

        private readonly System.Timers.Timer _resourceMonitorTimer;

        public MainViewModel()
        {
            // Инициализация таймера для мониторинга ресурсов
            _resourceMonitorTimer = new System.Timers.Timer(1000); // Интервал в миллисекундах
            _resourceMonitorTimer.Elapsed += (s, e) => UpdateResourceUsage();
            _resourceMonitorTimer.AutoReset = true;
            _resourceMonitorTimer.Start();

            LoadProcesses();
            LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }

        // Метод для загрузки процессов
        private void LoadProcesses()
        {
            Processes.Clear();
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    Processes.Add(new ProcessModel
                    {
                        Name = process.ProcessName,
                        PID = process.Id,
                        MemoryUsage = $"{process.WorkingSet64 / 1024 / 1024} MB"
                    });
                }
                catch
                {
                    // Игнорируем недоступные процессы
                }
            }
        }

        // Команды
        [RelayCommand]
        private void RefreshProcesses()
        {
            LoadProcesses();
        }

        [RelayCommand]
        private void LoadDirectory(string path = "")
        {
            FileSystemItems.Clear();

            try
            {
                // Получение пути для мобильных платформ
                string basePath = string.IsNullOrEmpty(path)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                    : path;

                var directoryInfo = new DirectoryInfo(basePath);

                foreach (var dir in directoryInfo.GetDirectories())
                {
                    FileSystemItems.Add(new FileSystemItem
                    {
                        Name = dir.Name,
                        Path = dir.FullName,
                        IsDirectory = true,
                        Size = "N/A",
                        LastModified = dir.LastWriteTime.ToString()
                    });
                }

                foreach (var file in directoryInfo.GetFiles())
                {
                    FileSystemItems.Add(new FileSystemItem
                    {
                        Name = file.Name,
                        Path = file.FullName,
                        IsDirectory = false,
                        Size = $"{file.Length / 1024} KB",
                        LastModified = file.LastWriteTime.ToString()
                    });
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Обработка ошибок доступа
            }
        }


        [RelayCommand]
        private void CreateFile(string path)
        {
            try
            {
                File.WriteAllText(Path.Combine(path, "NewFile.txt"), "New file content");
                LoadDirectory(path);
            }
            catch
            {
                // Обработка ошибок
            }
        }

        [RelayCommand]
        private void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    LoadDirectory(Path.GetDirectoryName(filePath)!);
                }
            }
            catch
            {
                // Обработка ошибок
            }
        }

        private void UpdateResourceUsage()
        {
#if WINDOWS
    // Реализация для Windows
    CpuUsage = "CPU: Not Implemented (Windows-only example)";
    AvailableMemory = "Memory: Not Implemented (Windows-only example)";
#else
            // Реализация для Android/iOS
            CpuUsage = "CPU: Monitoring not available on mobile";
            AvailableMemory = "Memory: Monitoring not available on mobile";
#endif
        }

    }
}
