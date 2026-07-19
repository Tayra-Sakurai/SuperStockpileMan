using CommunityToolkit.Mvvm.Messaging.Messages;
using SuperStockpileMan.Bus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Messages
{
    public class ItemInvokedMessage : ValueChangedMessage<Item>
    {
        public ItemInvokedMessage(Item value) : base(value)
        {
        }
    }
}
