using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChiLearn.Models.User;
using System.Text.Json;

namespace ChiLearn.Services
{
    public class UserDataService
    {
        private const string FileName = "userdata.json";
        private static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, FileName);

        public static async Task SaveAsync(UserDataJson data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных пользователя: {ex.Message}");
            }
        }

        public static async Task<UserDataJson?> LoadAsync()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    var defaultData = new UserDataJson();
                    await SaveAsync(defaultData);
                    return defaultData;
                }

                var json = await File.ReadAllTextAsync(FilePath);
                return JsonSerializer.Deserialize<UserDataJson>(json) ?? new UserDataJson();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении данных пользователя: {ex.Message}");
                return null;
            }
        }


        public static void Delete()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }

}
