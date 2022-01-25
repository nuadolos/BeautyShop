using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using BeautyShop.Entities;

namespace BeautyShop.Utilities
{
    internal class Transition
    {
        public static Frame MainFrame { get; set; }

        private static KosmeticDBEntities context;
        public static KosmeticDBEntities Context
        {
            get 
            {
                if (context == null)
                    context = new KosmeticDBEntities();

                return context; 
            }
        }
    }
}
