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
        public virtual DbSet<form_resource_type> form_resource_type { get; set; }
        public virtual DbSet<Forms> Forms { get; set; }
        public virtual DbSet<Forms_details> Forms_details { get; set; }
        public virtual DbSet<demo_log> demo_log { get; set; }
        public virtual DbSet<user_form_w9> user_form_w9 { get; set; }
        public virtual DbSet<Stock_items> Stock_items { get; set; }
        public virtual DbSet<ActivitiesM> ActivitiesM { get; set; }
        public virtual DbSet<FormsM> FormsM { get; set; }
        public virtual DbSet<RoutesM> RoutesM { get; set; }
        public virtual DbSet<VisitsM> VisitsM { get; set; }
        public virtual DbSet<FormsM_details> FormsM_details { get; set; }
        public virtual DbSet<ActivitiesM_types> ActivitiesM_types { get; set; }
        public virtual DbSet<VisitsM_representatives> VisitsM_representatives { get; set; }
        public virtual DbSet<ActivitiesM_log> ActivitiesM_log { get; set; }
        public virtual DbSet<Brand_competitors> Brand_competitors { get; set; }
        public virtual DbSet<Items_displays> Items_displays { get; set; }
        public virtual DbSet<promotions_v1> promotions_v1 { get; set; }
        public virtual DbSet<Demos> Demos { get; set; }
        public virtual DbSet<FormsM_detailsDemos> FormsM_detailsDemos { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<FormsM_detailsTasks> FormsM_detailsTasks { get; set; }
        public virtual DbSet<Tb_Surveys> Tb_Surveys { get; set; }
    }
}
