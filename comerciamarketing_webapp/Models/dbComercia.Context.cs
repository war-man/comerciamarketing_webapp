﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
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
    }
}
