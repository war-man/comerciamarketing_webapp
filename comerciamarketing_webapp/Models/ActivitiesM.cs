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
    
    public partial class ActivitiesM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ActivitiesM()
        {
            this.FormsM = new HashSet<FormsM>();
        }
    
        public int ID_activity { get; set; }
        public int ID_visit { get; set; }
        public int ID_form { get; set; }
        public string ID_customer { get; set; }
        public string Customer { get; set; }
        public string comments { get; set; }
        public System.DateTime check_in { get; set; }
        public System.DateTime check_out { get; set; }
        public string query1 { get; set; }
        public int ID_empresa { get; set; }
        public bool isfinished { get; set; }
        public string description { get; set; }
        public int ID_usuarioCreate { get; set; }
        public int ID_usuarioEnd { get; set; }
        public System.DateTime date { get; set; }
        public int ID_activitytype { get; set; }
        public string ID_usuarioEndString { get; set; }
        public Nullable<bool> desnormalizado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormsM> FormsM { get; set; }
        public virtual ActivitiesM_types ActivitiesM_types { get; set; }
    }
}
