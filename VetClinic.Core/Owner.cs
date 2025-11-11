using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VetClinic.Core
{
    // Клас, що описує Власника тварини
    public class Owner
    {
        // Унікальний номер
        public int Id { get; set; }
        // Повне ім'я
        public string FullName { get; set; }
        // Контактний телефон
        public string ContactPhone { get; set; }

        [JsonIgnore]
        public List<Pet> Pets { get; set; }

        // Конструктор
        public Owner()
        {
            Pets = new List<Pet>();
        }
    }
}
