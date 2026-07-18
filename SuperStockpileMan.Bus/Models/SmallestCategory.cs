using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Models
{
    public class SmallestCategory : CategoryBase
    {
        public ICollection<Item> Items { get; } = new HashSet<Item>();
    }
}
