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
    
    public partial class Forms_details
    {
        public int ID_details { get; set; }
        public Nullable<int> ID_formresourcetype { get; set; }
        public string fsource { get; set; }
        public string fdescription { get; set; }
        public Nullable<decimal> fvalue { get; set; }
        public Nullable<int> ID_form { get; set; }
        public Nullable<int> ID_demo { get; set; }
        public Nullable<bool> original { get; set; }
    
        public virtual form_resource_type form_resource_type { get; set; }
        public virtual Forms Forms { get; set; }
    }
}
