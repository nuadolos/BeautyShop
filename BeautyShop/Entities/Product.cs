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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.AttachedProduct = new HashSet<AttachedProduct>();
            this.AttachedProduct1 = new HashSet<AttachedProduct>();
            this.ProductPhoto = new HashSet<ProductPhoto>();
            this.ProductSale = new HashSet<ProductSale>();
        }
    
        public int ID { get; set; }
        public string Title { get; set; }
        public decimal Cost { get; set; }
        public bool IsActive { get; set; }
        public string ActiveProd
        {
            get => IsActive ? "активен" : "неактивен";
        }
        public string AttachedTitle
        {
            get
            {
                string allTitles = null;

                foreach (var item in AttachedProduct)
                {
                    allTitles += item.Product1.Title + ", ";
                }

                if (allTitles != null)
                    return allTitles.Remove(allTitles.Length - 2, 2).Insert(0, "(").Insert(allTitles.Length - 1, ")");
                else
                    return "";
            }
        }
        public string Description { get; set; }
        public string MainImagePath { get; set; }
        public int ManufacturerID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AttachedProduct> AttachedProduct { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AttachedProduct> AttachedProduct1 { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductPhoto> ProductPhoto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductSale> ProductSale { get; set; }
    }
}
