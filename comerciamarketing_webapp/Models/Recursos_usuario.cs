//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Recursos_usuario
    {
        public int ID_recursousuario { get; set; }
        public string descripcion { get; set; }
        public string url { get; set; }
        public Nullable<System.DateTime> fultima_actualizacion { get; set; }
        public Nullable<int> ID_usuario { get; set; }
        public Nullable<int> tipo_recurso { get; set; }
    
        public virtual Usuarios Usuarios { get; set; }
    }
}
