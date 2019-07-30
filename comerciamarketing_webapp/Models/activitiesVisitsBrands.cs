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

}