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
    using System.ComponentModel.DataAnnotations;

    public partial class Demos
    {
        public int ID_demo { get; set; }
        public string ID_Vendor { get; set; }
        public string vendor { get; set; }
        public string ID_Store { get; set; }
        public string store { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        public System.DateTime visit_date { get; set; }
        public string ID_usuario { get; set; }
        public int ID_demostate { get; set; }
        public string comments { get; set; }
        public int ID_form { get; set; }
        public decimal extra_hours { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddThh:mm}")]

        public System.DateTime end_date { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddThh:mm}")]

        public System.DateTime check_in { get; set; }
        public string geoLong { get; set; }
        public string geoLat { get; set; }

        public virtual Demo_state Demo_state { get; set; }
        public virtual Forms Forms { get; set; }
    }
}
