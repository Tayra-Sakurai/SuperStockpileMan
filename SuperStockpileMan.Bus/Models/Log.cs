using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Models
{
    public class Log
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public DateTimeOffset DateTime { get; set; } = DateTimeOffset.Now;
        public string? Message { get; set; }
        public ActionType Action { get; set; }
    }
}
