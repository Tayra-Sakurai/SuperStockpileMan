// SPDX-FileCopyrightText: 2026 SAKURAI Tayra <tayra_sakurai@icloud.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SuperStockpileMan.Bus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan
{
    public sealed partial class CategoryTreeViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CategoryTemplate { get; set; }
        public DataTemplate SmallestCategoryTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Category)
                return CategoryTemplate;
            else if (item is SmallestCategory)
                return SmallestCategoryTemplate;

            throw new NotImplementedException();
        }
    }
}
