//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace comerciamarketing_webapp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DatosInventario_10
    {
        public int ID_detail { get; set; }
        public Nullable<int> ID_Task { get; set; }
        public Nullable<System.DateTime> VisitDate { get; set; }
        public Nullable<System.DateTime> CheckOut { get; set; }
        public string UserName { get; set; }
        public string IDCliente { get; set; }
        public string Cliente { get; set; }
        public string Producto { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public Nullable<decimal> Inventario { get; set; }
    }
}