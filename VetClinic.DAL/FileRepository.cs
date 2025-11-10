using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Core;


namespace VetClinic.DAL
{
    public class FileRepository<T> where T : class 
                                                   
    {
        private readonly string _filePath;

        public FileRepository(string fileName)
        {
            _filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        }
        public List<T> ReadAll()
        {
            if (!File.Exists(_filePath))
            {
                return new List<T>();
            }

            try
            {
                string json = File.ReadAllText(_filePath);

                var data = JsonConvert.DeserializeObject<List<T>>(json);

                return data ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileRepository] ОШИБКА чтения файла {_filePath}: {ex.Message}");
                return new List<T>();
            }
        }

        public void SaveChanges(List<T> data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileRepository] ОШИБКА записи в файл {_filePath}: {ex.Message}");
            }
        }
    }
}