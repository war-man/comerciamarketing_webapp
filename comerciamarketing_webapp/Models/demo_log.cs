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
    
    public partial class demo_log
    {
        public int ID_historial { get; set; }
        public Nullable<int> ID_demo { get; set; }
        public string ip { get; set; }
        public string hostname { get; set; }
        public string typeh { get; set; }
        public string continent_name { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string region_code { get; set; }
        public string region_name { get; set; }
        public string city { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public Nullable<System.DateTime> fecha_conexion { get; set; }
        public string action { get; set; }
    }
}
