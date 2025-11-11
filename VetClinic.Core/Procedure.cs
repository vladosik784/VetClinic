using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Core
{
    public class Procedure
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public decimal Price { get; set; }
        
        public decimal CostPrice { get; set; }         
        public List<string> Tags { get; set; }         
        public bool IsBlocked { get; set; }            

    }
}
