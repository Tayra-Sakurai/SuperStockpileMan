using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Models
{
    public class Category : CategoryBase
    {
        public ICollection<CategoryBase> Children { get; } = new HashSet<CategoryBase>();
    }
}
