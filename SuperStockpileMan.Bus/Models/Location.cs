using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Item> Items { get; } = new HashSet<Item>();
        public ICollection<CategoryBase> CategoryBases { get; } = new HashSet<CategoryBase>();
    }
}
