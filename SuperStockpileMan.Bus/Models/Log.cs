// SPDX-FileCopyrightText: 2026 SAKURAI Tayra <tayra_sakurai@icloud.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
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
