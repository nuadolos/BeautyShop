//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BeautyShop.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class AttachedProduct
    {
        public int MainProductID { get; set; }
        public int AttachedProductID { get; set; }
        public string Nothing { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Product Product1 { get; set; }
    }
}
