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
    
    public partial class RoutesM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RoutesM()
        {
            this.VisitsM = new HashSet<VisitsM>();
        }
    
        public int ID_route { get; set; }
        public System.DateTime date { get; set; }
        public string query1 { get; set; }
        public string query2 { get; set; }
        public string query3 { get; set; }
        public System.DateTime end_date { get; set; }
        public int ID_empresa { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VisitsM> VisitsM { get; set; }
    }
}
