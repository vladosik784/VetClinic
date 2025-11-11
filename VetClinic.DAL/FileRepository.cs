using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Core;


namespace VetClinic.DAL
{
    // Універсальний клас для роботи з файлами JSON
    public class FileRepository<T> where T : class
    {
        private readonly string _filePath;

        public FileRepository(string fileName)
        {
            _filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        }

        // Читає всі дані з файлу
        public List<T> ReadAll()
        {
            if (!File.Exists(_filePath))
            {
                return new List<T>();
            }

            try
            {
                string json = File.ReadAllText(_filePath, Encoding.UTF8);
                var data = JsonConvert.DeserializeObject<List<T>>(json);
                return data ?? new List<T>();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        // Повністю перезаписує файл новими даними
        public void SaveChanges(List<T> data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(_filePath, json, Encoding.UTF8);
            }
            catch (Exception)
            {

            }
        }
    }
}