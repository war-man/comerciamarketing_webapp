﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class COM_MKEntities : DbContext
    {
        public COM_MKEntities()
            : base("name=COM_MKEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C_ROUTES> C_ROUTES { get; set; }
        public virtual DbSet<OCRD> OCRD { get; set; }
        public virtual DbSet<OITM> OITM { get; set; }
        public virtual DbSet<OMRC> OMRC { get; set; }
        public virtual DbSet<view_CMKEditorB> view_CMKEditorB { get; set; }
        public virtual DbSet<C_ROUTE> C_ROUTE { get; set; }
        public virtual DbSet<BI_Dim_Products> BI_Dim_Products { get; set; }
    }
}
