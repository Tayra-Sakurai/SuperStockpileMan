using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTimeOffset? DueDate { get; set; }
        public DateTimeOffset PurchaseDate { get; set; } = DateTimeOffset.Now;
        public bool IsStocked { get; set; } = true;
        public int SmallestCategoryId { get; set; }
        public SmallestCategory? SmallestCategory { get; set; }
        public int LocationId { get; set; }
        public Location? Location { get; set; }
    }
}
