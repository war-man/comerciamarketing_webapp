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
    
    public partial class dbComerciaEntities : DbContext
    {
        public dbComerciaEntities()
            : base("name=dbComerciaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Recursos_usuario> Recursos_usuario { get; set; }
        public virtual DbSet<Tipo_membresia> Tipo_membresia { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<tipo_recurso> tipo_recurso { get; set; }
        public virtual DbSet<Empresas> Empresas { get; set; }
        public virtual DbSet<historial_conexiones> historial_conexiones { get; set; }
        public virtual DbSet<Demo_state> Demo_state { get; set; }
        public virtual DbSet<Demos> Demos { get; set; }
        public virtual DbSet<form_resource_type> form_resource_type { get; set; }
        public virtual DbSet<Forms> Forms { get; set; }
        public virtual DbSet<Forms_details> Forms_details { get; set; }
        public virtual DbSet<demo_log> demo_log { get; set; }
    }
}
