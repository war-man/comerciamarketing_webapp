using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comerciamarketing_webapp.Models
{
    public class activitiesVisitsBrands
    {
  
            public int ID_activity { get; set; }
            public int ID_visit { get; set; }
            public int ID_form { get; set; }
            public string formName { get; set; }
            public string ID_store { get; set; }
            public string store { get; set; }
            public DateTime visitDate { get; set; }
            public string ID_customer { get; set; }
            public string Customer { get; set; }
            public bool isfinished { get; set; }
            public int id_usuarioend { get; set; }
            public string id_usuarioendexternal { get; set; }
            public int id_activitytype { get; set; }
            public string ActivityName { get; set; }
            public string Comments { get; set; }
            public string ID_brand { get; set; }
            public string Brand { get; set; }
            public int count { get; set; }
        
    }

    public class activitiesCompleteInfo
    {

        public int ID_activity { get; set; }
        public int ID_visit { get; set; }
        public int ID_form { get; set; }
        public string formName { get; set; }
        public string ID_store { get; set; }
        public string store { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public DateTime visitDate { get; set; }
        public string ID_customer { get; set; }
        public string Customer { get; set; }
        public bool isfinished { get; set; }
        public int id_usuarioend { get; set; }
        public string usuarioName { get; set; }
        public string id_usuarioendexternal { get; set; }
        public int id_activitytype { get; set; }
        public string ActivityName { get; set; }
        public string Comments { get; set; }
        public string ID_brand { get; set; }
        public string Brand { get; set; }
        public int count { get; set; }

    }




    public class QuickVisit_report
    {

        public int ID_activity { get; set; }
        public int ID_visit { get; set; }
        public int ID_form { get; set; }
        public string formName { get; set; }
        public string ID_store { get; set; }
        public string store { get; set; }
        public DateTime visitDate { get; set; }
        public string ID_customer { get; set; }
        public string Customer { get; set; }
        public bool isfinished { get; set; }
        public int id_usuarioend { get; set; }
        public string id_usuarioendexternal { get; set; }
        public int id_activitytype { get; set; }
        public string ActivityName { get; set; }
        public string Comments { get; set; }
        public string ID_brand { get; set; }
        public string Brand { get; set; }
        public int count { get; set; }
        public string urlimg1 { get; set; }
        public string urlimg2 { get; set; }
        public string urlsign { get; set; }
    }

    public class QuickVisit_export {
        public int ID { get; set; }
        public string Store { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public DateTime date { get; set; }
        //public string SalesRepresentative { get; set; }
        public string Brand { get; set; }
        public string Activity { get; set; }
        public string PictureBefore { get; set; }
        public string CommentBefore { get; set; }
        public string PictureAfter { get; set; }
        public string CommentAfter { get; set; }


    }


    public class ActivitiesReport
    {
        public int ID_Activity { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Customer { get; set; }
        public string ID_Customer { get; set; }
        public string ID_Store { get; set; }
        public string Store { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string state { get; set; }
        public DateTime date { get; set; }
        public int ID_SalesRep { get; set; }
        public string SalesRep { get; set; }
        public string Category { get; set; }
        public string SubCategories { get; set; }

    }

    public class VisitsInfo
    {
        public int ID_visit { get; set; }
        public string ID_store { get; set; }
        public string store { get; set; }
        public string addresss { get; set; }
        public DateTime visitDate { get; set; }
        public string ID_customer { get; set; }
        public string Customer { get; set; }
        public int id_usuarioend { get; set; }
        public string id_usuarioendexternal { get; set; }
        public string usuario_name { get; set; }
        public string Comments { get; set; }
        public string ID_brand { get; set; }
        public string Brand { get; set; }
        public string pictureBefore { get; set; }
        public string pictureAfter { get; set; }
  

    }
    public class imagesInfo
    {
        public int ID_image { get; set; }
        public string Activity { get; set; }
        public DateTime visitDate { get; set; }
        public string ID_customer { get; set; }
        public string Customer { get; set; }
        public string url { get; set; }



    }
    public class VisitsInfoCalendar
    {
        public int ID_visit { get; set; }
        public string ID_store { get; set; }
        public DateTime visitDate { get; set; }
        public string ID_customer { get; set; }
        public string ID_brand { get; set; }
        public string Brand { get; set; }
        public int idroute { get; set; }


    }

    public class demosDetails {
        public int ID_details { get; set; }
        public int ID_formresourcetype { get; set; }
        public string fsource { get; set; }
        public string fdescription { get; set; }
        public int fvalue { get; set; }
        public decimal fvalueDecimal { get; set; }
        public string fvalueText { get; set; }
        public int ID_formM { get; set; }
        public int ID_visit { get; set; }
        public bool original { get; set; }
        public int obj_order { get; set; }
        public int obj_group { get; set; }
        public int idkey { get; set; }
        public int parent { get; set; }
        public string query1 { get; set; }
        public string query2 { get; set; }
        public int ID_empresa { get; set; }
    }

    public  class DatosInventario_10Report
    {
        public int ID_detail { get; set; }
        public int ID_Task { get; set; }
        public System.DateTime VisitDate { get; set; }
        public System.DateTime CheckOut { get; set; }
        public string UserName { get; set; }
        public string IDCliente { get; set; }
        public string Cliente { get; set; }
        public string Producto { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public decimal Inventario { get; set; }
    }

    public class DatosDemos_20Report {
        public int ID_detail { get; set; }
        public int ID_demo { get; set; }
        public string Id_Store { get; set; }
        public string Store { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public System.DateTime VisitDate { get; set; }
        public System.DateTime CheckIn { get; set; }
        public System.DateTime CheckOut { get; set; }
        public string UserName { get; set; }
        public string IDCliente { get; set; }
        public string Cliente { get; set; }
        public string Marcas { get; set; }
        public string Producto { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad_entregado { get; set; }
        public decimal Inventario_inicial { get; set; }
        public string Unidades_Disponibles { get; set; }
        public string Presentacion { get; set; }
        public string Tipo_empaque { get; set; }
        public string Promocion { get; set; }
        public string Descuento { get; set; }
        public string Regalia { get; set; }
        public string Competencia_directa { get; set; }
        public string ComentarioSabor { get; set; }
        public string ComentarioCalidad { get; set; }
        public string ComentarioPrecio { get; set; }
        public string ComentarioEmpaque { get; set; }
    }
}