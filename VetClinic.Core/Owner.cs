using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VetClinic.Core
{
    public class Owner
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ContactPhone { get; set; }
        
        [JsonIgnore] 
        public List<Pet> Pets { get; set; }
        public Owner()
        {
            Pets = new List<Pet>();
        }
    }
}
