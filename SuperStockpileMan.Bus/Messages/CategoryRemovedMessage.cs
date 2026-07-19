using CommunityToolkit.Mvvm.Messaging.Messages;
using SuperStockpileMan.Bus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Messages
{
    public class CategoryRemovedMessage : ValueChangedMessage<Category>
    {
        public CategoryRemovedMessage(Category value) : base(value) { }
    }
}
