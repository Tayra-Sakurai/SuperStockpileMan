using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Models
{
    public class CategoryBase
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public CategoryBase? Parent { get; set; }
        public ICollection<Location> Locations { get; } = new HashSet<Location>();
    }
}
