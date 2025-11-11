using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VetClinic.Core
{
    // Клас, що описує Тварину
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; } 
        public int Age { get; set; }

        public int OwnerId { get; set; }
        [JsonIgnore]
        public Owner Owner { get; set; }
    }
}
