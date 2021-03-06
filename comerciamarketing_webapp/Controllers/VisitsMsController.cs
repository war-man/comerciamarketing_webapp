using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using comerciamarketing_webapp.Models;
using CrystalDecisions.CrystalReports.Engine;
using Ionic.Zip;
using Newtonsoft.Json;
using Postal;

namespace comerciamarketing_webapp.Controllers
{
    public class VisitsMsController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();
        private COM_MKEntities CMKdb = new COM_MKEntities();
        // GET: VisitsMs
        public ActionResult Index()
        {
            var visitsM = db.VisitsM.Include(v => v.RoutesM);
            return View(visitsM.ToList());
        }
        public class demosReps
        {
            public string ID { get; set; }
            public string name { get; set; }

        }
        //CREACION DE JERARQUIAS Y OBJETOS
        //FORMULARIOS Y DETALLES DE FORMULARIOS (Se utiliza en Activities)
        public class tablahijospadre
        {
            public int ID_details { get; set; }
            public int id_resource { get; set; }
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
            public bool onserver { get; set; }
            public string action { get; set; }
            public int? ID_visitOriginal { get; set; }
            public bool isnew { get; set; }
            public int resourcetype { get; set; }
        }

        public class MyObj_tablapadre
        {
            public int ID_details { get; set; }
            public int id_resource { get; set; }
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
            public List<MyObj_tablapadre> children { get; set; }
            public bool onserver { get; set; }
            public string action { get; set; }
            public int? ID_visitOriginal { get; set; }
            public int resourcetype { get; set; }
            public bool isnew { get; set; }
        }
        public class VisitsMs_offline
        {
            public int ID_visit { get; set; }
            public string ID_customer { get; set; }
            public string customer { get; set; }
            public string ID_store { get; set; }
            public string store { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string zipcode { get; set; }
            public string state { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
            public System.DateTime visit_date { get; set; }
            public int ID_visitstate { get; set; }
            public string comments { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
            public System.DateTime check_in { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
            public System.DateTime check_out { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
            public System.DateTime end_date { get; set; }
            public string geoLong { get; set; }
            public string geoLat { get; set; }
            public int extra_hours { get; set; }
            public int ID_route { get; set; }
            public int ID_empresa { get; set; }
            public bool onserver { get; set; }
            public string action { get; set; }
        }

        public class ActivitiesM_offline{
            public int ID_activity { get; set; }
            public int ID_visit { get; set; }
            public int ID_form { get; set; }
            public string ID_customer { get; set; }
            public string usuarioasignado { get; set; }
            public string Customer { get; set; }
            public string comments { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
            public System.DateTime check_in { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
            public System.DateTime check_out { get; set; }
            public string query1 { get; set; }
            public int ID_empresa { get; set; }
            public bool isfinished { get; set; }
            public string description { get; set; }
            public int ID_usuarioCreate { get; set; }
            public int ID_usuarioEnd { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
            public System.DateTime date { get; set; }
            public int ID_activitytype { get; set; }
            public string ID_usuarioEndString { get; set; }
            public bool onserver { get; set; }
            public string action { get; set; } 
            public bool isnew { get; set; }
        }

        public class Visitstatea
        {
            public int state { get; set; }
            public bool onserver { get; set; }
            public string action { get; set; }
            public int? ID_visit { get; set; }
        }

        public ActionResult Detailsa(int? id)
        {
                if (Session["IDusuario"] != null)
                {
                    int ID = Convert.ToInt32(Session["IDusuario"]);
                    var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                    ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;


                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    VisitsM visitsM = db.VisitsM.Find(id);
                    if (visitsM == null)
                    {
                        return HttpNotFound();
                    }
                    //Lista de usuarios representantes
                var usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9);

                //Seleccionamos representantes que estan incluidos
                var reps = (from rep in db.VisitsM_representatives
                                where (rep.ID_visit == id)
                                select new representativesVisit
                                {
                                    ID = rep.ID_usuario,
                                    name = "",
                                    email = ""
                                }
                               ).ToList();

                    foreach (var itemR in reps)
                    {
                        var user = (from u in usuarios where (u.ID_usuario == itemR.ID) select u).FirstOrDefault();
                        if (user != null) { itemR.name = user.nombre + " " + user.apellido; itemR.email = user.correo; } else { itemR.name = ""; itemR.email = ""; }
                    }

                    ViewBag.repslist = reps;
                    //FIN representantes
                    //ACTIVITIES
                    if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)
                    {
                        var activities = (from a in db.ActivitiesM where (a.ID_visit == id) select a).OrderBy(a=> a.ID_activitytype).ThenBy(a=>a.description).ToList();

                        foreach (var itemac in activities)
                        {
                            var uassigned = (from u in usuarios where (u.ID_usuario == itemac.ID_usuarioEnd) select u).FirstOrDefault();
                            if (uassigned == null && itemac.ID_usuarioEnd == 0)
                            {
                                var usuario = (from a in CMKdb.OCRD where (a.CardCode == itemac.ID_usuarioEndString) select a).FirstOrDefault();
                            if (usuario != null) {
                                itemac.ID_customer = usuario.CardName.ToString() + " - " + usuario.E_Mail.ToString();
                            }
                                itemac.ID_customer = "No data found";
                            }
                            else
                            {
                                itemac.ID_customer = uassigned.nombre + " " + uassigned.apellido;
                            }



                        }


                        ViewBag.activities = activities;
                    }
                    else
                    {
                        var activities = (from a in db.ActivitiesM where (a.ID_visit == id && a.ID_usuarioEnd == ID) select a).OrderBy(a => a.ID_activitytype).ThenBy(a => a.description).ToList();

                        foreach (var itemac in activities)
                        {
                            var uassigned = (from u in usuarios where (u.ID_usuario == itemac.ID_usuarioEnd) select u).FirstOrDefault();

                            itemac.ID_customer = uassigned.nombre + " " + uassigned.apellido;

                        }
                        ViewBag.activities = activities;
                    }
                    //CHECK OUT POR USUARIO
                    if (datosUsuario.ID_rol == 9 && datosUsuario.ID_tipomembresia == 8)
                    {
                        var rep = (from a in db.VisitsM_representatives
                                   where (a.ID_visit == id && a.ID_usuario == datosUsuario.ID_usuario)
                                   select a
                              ).FirstOrDefault();
                        ViewBag.estadovisita = Convert.ToInt32(rep.query1);//Utilizaremos este campo para filtrar el estado por usuario
                    }
                    else
                    {
                        ViewBag.estadovisita = visitsM.ID_visitstate;
                    }



                    ViewBag.idvisita = id;

                    ViewBag.storename = visitsM.store;

                    //FIN ACTIVITIES
                    //representantes
                    

                    ViewBag.representatives = usuarios.ToList();

                    var usuariosdemo = CMKdb.OCRD.Where(b => b.Series == 70 && b.CardName != null && b.CardName != "" && b.CardType == "s").OrderBy(b => b.CardName).ToList();

                    List<demosReps> selectList_usuarios = (from st in usuariosdemo
                                                           select new demosReps
                                                           {
                                                               ID = Convert.ToString(st.CardCode),
                                                               name = st.CardName.ToString() + " - " + st.E_Mail.ToString()
                                                           }).ToList();
                    ViewBag.reps_demos = selectList_usuarios;
                    //Cargamos ruta 
                    //var ruta = (from r in db.RoutesM where (r.ID_route == visitsM.ID_route) select r).FirstOrDefault();
                    //FORMULARIOS

                    //List<int> FormsIds = ruta.query1.Split(',').Select(int.Parse).ToList();

                    //var activeForms = (from at in db.FormsM where (FormsIds.Contains(at.ID_form)) select at).ToList();

                    var activeForms = (from at in db.FormsM where (at.ID_empresa == GlobalVariables.ID_EMPRESA_USUARIO) select at).ToList();
                    ViewBag.activeForms = activeForms;
                    //FIN FORMULARIOS

                    var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                    ViewBag.customers = customers.ToList();


                    //Route
                    ViewBag.route = visitsM.ID_route;


                    var geoLong = "";
                    var geoLat = "";

                    geoLong = visitsM.geoLong;
                    geoLat = visitsM.geoLat;

                    ViewBag.glong = geoLong;
                    ViewBag.glat = geoLat;
                ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;

                    return View(visitsM);

                }
                else
                {
                    return RedirectToAction("Index", "Home", null);
                }



            

        }

        ///FIN DE MODELOS OFFLINE
        public ActionResult preDetails(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                //ESTE ACTION RESULT SE UTILIZA SOLAMENTE PARA CREAR EL MANIFEST FUERA DEL CACHE OFFLINE

                //PARA CREAR MANIFEST
                //LLAMAMOS LISTA DE ACTIVIDADES
                //ACTIVITIES
                List<ActivitiesM> activities = new List<ActivitiesM>();

                if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)
                {
                    activities = (from a in db.ActivitiesM
                                  where (a.ID_visit == id)
                                  select a).ToList();

                }
                else
                {
                    activities = (from a in db.ActivitiesM
                                  where (a.ID_visit == id && a.ID_usuarioEnd == ID)
                                  select a).ToList();

                }



                var root = Server.MapPath("~");
                StringBuilder SB = new StringBuilder();

                //Devolvemos versionamiento de cache manifest
                //var path_ver = Path.Combine(root, @"versionamiento.txt");
                //System.IO.StreamReader readingFile = new System.IO.StreamReader(path_ver);

                //string readingLine = readingFile.ReadLine();
                //Header
                SB.AppendLine("CACHE MANIFEST");
                SB.AppendLine("# version 1.0.0." + id + ". " + DateTime.Today.ToShortTimeString());
                SB.AppendLine();
                //Content
                SB.AppendLine("# Site Content");
                SB.AppendLine("CACHE:");
                SB.AppendLine(@"/VisitsMs/Details/" + id);
                foreach (var cache_act in activities)
                {
                    if (cache_act.ID_activitytype == 1)//Forms
                    {
                        SB.AppendLine(@"/FormsM/Activity/" + cache_act.ID_activity);
                    }
                    else if (cache_act.ID_activitytype == 2)
                    {//Audits
                        SB.AppendLine(@"/FormsM/Activityra/" + cache_act.ID_activity);
                    }
                    else if (cache_act.ID_activitytype == 3)//Sales Orders
                    {

                    }
                    else if (cache_act.ID_activitytype == 4)//Demos
                    {
                        SB.AppendLine(@"/FormsM/Activity/" + cache_act.ID_activity);
                    }



                }
                SB.AppendLine(@"/FormsM/Activity");

                SB.AppendLine();
                SB.AppendLine("# Scripts");
                SB.AppendLine(@"https://cdn.jsdelivr.net/jquery/latest/jquery.min.js");
                SB.AppendLine(@"/Content/newstyle2/vendors/js/vendor.bundle.base.js");
                SB.AppendLine(@"/Content/newstyle2/vendors/js/vendor.bundle.addons.js");
                SB.AppendLine(@"/Content/newstyle2/js/off-canvas.js");
                SB.AppendLine(@"/Content/newstyle2/js/hoverable-collapse.js");
                SB.AppendLine(@"/Content/newstyle2/js/misc.js");
                SB.AppendLine(@"/Content/newstyle2/js/settings.js");
                SB.AppendLine(@"/Content/newstyle2/js/todolist.js");
                SB.AppendLine(@"/Content/newstyle2/js/dashboard.js");
                SB.AppendLine(@"/Content/newstyle2/js/horizontal-menu.js");
                SB.AppendLine(@"/Content/newstyle2/js/formpickers.js");
                SB.AppendLine(@"/Content/newstyle2/js/calendar.js");
                SB.AppendLine(@"/Content/newstyle2/js/x-editable.js");
                SB.AppendLine(@"/Content/newstyle2/js/dropify.js");
                SB.AppendLine(@"/Content/newstyle2/js/dropzone.js");
                SB.AppendLine(@"/Content/newstyle2/js/jquery-file-upload.js");
                SB.AppendLine(@"/Content/newstyle2/js/formpickers.js");
                SB.AppendLine(@"/Content/newstyle2/js/form-repeater.js");
                SB.AppendLine(@"/Content/newstyle2/js/data-table.js");
                SB.AppendLine(@"/Content/newstyle2/js/select2.js");
                SB.AppendLine(@"/Content/newstyle2/js/screenfull.js");
                SB.AppendLine(@"/Content/newstyle2/js/tooltips.js");
                SB.AppendLine(@"/Content/newstyle2/js/popover.js");
                SB.AppendLine(@"/Content/newstyle2/js/iCheck.js");
                SB.AppendLine(@"/Content/newstyle2/js/typeahead.js");
                SB.AppendLine(@"/Content/newstyle2/js/dropify.js");
                SB.AppendLine(@"/Content/newstyle2/js/jquery.watermark.min.js");
                SB.AppendLine(@"https://cdn.jsdelivr.net/momentjs/latest/moment.min.js");
                SB.AppendLine(@"https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.min.js");
                SB.AppendLine(@"https://cdnjs.cloudflare.com/ajax/libs/select2/3.5.2/select2.min.js");
                SB.AppendLine(@"https://cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.3/modernizr.min.js");
                SB.AppendLine(@"https://cdn.datatables.net/responsive/2.2.3/js/dataTables.responsive.min.js");
                SB.AppendLine(@"/Content/js/signature.js");
                SB.AppendLine(@"/Content/newstyle2/js/JIC.js");

                //images
                SB.AppendLine(@"/Content/newstyle2/images/LogoCMK.png");
                SB.AppendLine(@"/Content/newstyle2/images/Logo_watermark.png");
                SB.AppendLine(@"/Content/newstyle2/images/samples/online-store.png");
                SB.AppendLine();
                SB.AppendLine("# Styles");
                SB.AppendLine(@"/Content/newstyle2/vendors/iconfonts/mdi/css/materialdesignicons.min.css");
                SB.AppendLine(@"/Content/newstyle2/vendors/iconfonts/puse-icons-feather/feather.css");
                SB.AppendLine(@"/Content/newstyle2/vendors/css/vendor.bundle.base.css");
                SB.AppendLine(@"/Content/newstyle2/vendors/css/vendor.bundle.addons.css");
                SB.AppendLine(@"/Content/newstyle2/vendors/iconfonts/flag-icon-css/css/flag-icon.min.css");
                SB.AppendLine(@"/Content/newstyle2/css/style.css");
                SB.AppendLine(@"/Content/newstyle2/images/favicon-16x16.png");
                SB.AppendLine(@"https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.css");
                SB.AppendLine(@"/Content/newstyle2/css/nestable.css");
                SB.AppendLine(@"/Content/css/multiple-select.css");
                SB.AppendLine(@"https://cdnjs.cloudflare.com/ajax/libs/select2/3.5.2/select2.min.css");
                SB.AppendLine(@"/Content/dist/css/map-icons.css");
                SB.AppendLine(@"https://cdn.datatables.net/responsive/2.2.3/css/responsive.dataTables.min.css");
                SB.AppendLine(@"/Content/newstyle2/css/retailauditresponsive.css");
                SB.AppendLine(@"/Content/newstyle2/css/owl.carousel.css");
                SB.AppendLine(@"/Content/newstyle2/css/owl.theme.css");
                SB.AppendLine(@"https://cdn.datatables.net/responsive/2.2.3/css/responsive.dataTables.min.css");
                SB.AppendLine();

                //NETWORK section to the mainfest file.
                SB.AppendLine("NETWORK:");
                SB.AppendLine(@"*");
                //SB.AppendLine("http://*");
                //SB.AppendLine("https://*");
                var nomcache = datosUsuario.ID_usuario + "offline.appcache";

                var path = Path.Combine(root, @"" + nomcache);




                System.IO.StreamWriter file = new System.IO.StreamWriter(path, false);


                file.WriteLine(SB.ToString());
                file.Close();

                var isReady = false;
                while (!isReady)
                {
                    isReady = IsFileReady(path);
                }

                if (Request.Cookies["currentvisit"] != null)
                {
                    //COMO YA EXISTE NO NECESITAMOS RECREARLA PERO LA ELIMINAMOS
                    var c = new HttpCookie("currentvisit");
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                    //LUEGO LA VOLVEMOS A CREAR

                    HttpCookie aCookie = new HttpCookie("currentvisit");
                    aCookie.Value = id.ToString();
                    aCookie.Expires = DateTime.Now.AddMonths(3);
                    Response.Cookies.Add(aCookie);
                }
                else
                {
                    HttpCookie aCookie = new HttpCookie("currentvisit");
                    aCookie.Value = id.ToString();
                    aCookie.Expires = DateTime.Now.AddMonths(3);
                    Response.Cookies.Add(aCookie);
                }

                if (Request.Cookies["currentcache"] != null)
                {
                    //COMO YA EXISTE NO NECESITAMOS RECREARLA PERO LA ELIMINAMOS
                    var d = new HttpCookie("currentcache");
                    d.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(d);
                    //LUEGO LA VOLVEMOS A CREAR

                    HttpCookie dCookie = new HttpCookie("currentcache");
                    dCookie.Value = path;
                    dCookie.Expires = DateTime.Now.AddMonths(3);
                    Response.Cookies.Add(dCookie);
                }
                else
                {
                    HttpCookie dCookie = new HttpCookie("currentcache");
                    dCookie.Value = path;
                    dCookie.Expires = DateTime.Now.AddMonths(3);
                    Response.Cookies.Add(dCookie);
                }
                return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = id });

            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }
        }
            // GET: VisitsMs/Details/5
            public ActionResult Details(int? id)
        {

            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;


                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                VisitsM visitsMorig = db.VisitsM.Find(id);
                if (visitsMorig == null)
                {
                    return HttpNotFound();
                }

                VisitsMs_offline visitsM = new VisitsMs_offline();

                visitsM.ID_visit = visitsMorig.ID_visit;
                visitsM.ID_customer = visitsMorig.ID_customer;
                visitsM.customer = visitsMorig.customer;
                visitsM.ID_store = visitsMorig.ID_store;
                visitsM.store = visitsMorig.store;
                visitsM.address = visitsMorig.address;
                visitsM.city = visitsMorig.city;
                visitsM.zipcode = visitsMorig.zipcode;
                visitsM.state = visitsMorig.state;
                visitsM.visit_date = visitsMorig.visit_date;
                visitsM.ID_visitstate = visitsMorig.ID_visitstate;
                visitsM.comments = visitsMorig.comments;
                visitsM.check_in = visitsMorig.check_in;
                visitsM.check_out = visitsMorig.check_out;
                visitsM.end_date = visitsMorig.end_date;
                visitsM.geoLong = visitsMorig.geoLong;
                visitsM.geoLat = visitsMorig.geoLat;
                visitsM.extra_hours = visitsMorig.extra_hours;
                visitsM.ID_route = visitsMorig.ID_route;
                visitsM.ID_empresa = visitsMorig.ID_empresa;
                visitsM.onserver = true;
                visitsM.action = "download";

                //Seleccionamos representantes que estan incluidos
                var reps = (from rep in db.VisitsM_representatives
                            where (rep.ID_visit == id)
                            select new representativesVisit
                            {
                                ID = rep.ID_usuario,
                                name = "",
                                email = ""
                            }
                           ).ToList();

                foreach (var itemR in reps)
                {
                    var user = (from u in db.Usuarios where (u.ID_usuario == itemR.ID) select u).FirstOrDefault();
                    if (user != null) { itemR.name = user.nombre + " " + user.apellido; itemR.email = user.correo; } else { itemR.name = ""; itemR.email = ""; }
                }

                ViewBag.repslist = reps;
                //FIN representantes
                //ACTIVITIES
                List<ActivitiesM_offline> activities = new List<ActivitiesM_offline>();

                if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)
                {
                    activities = (from a in db.ActivitiesM where (a.ID_visit == id) select new ActivitiesM_offline {
                        ID_activity = a.ID_activity, ID_visit = a.ID_visit, ID_form = a.ID_form, ID_customer = a.ID_customer, Customer = a.Customer, comments = a.comments,
                        check_in = a.check_in, check_out = a.check_out, query1 = a.query1, ID_empresa = a.ID_empresa, isfinished = a.isfinished, description = a.description,
                        ID_usuarioCreate = a.ID_usuarioCreate, ID_usuarioEnd = a.ID_usuarioEnd, date = a.date, ID_activitytype = a.ID_activitytype, ID_usuarioEndString = a.ID_usuarioEndString,
                        onserver = true, action = "download", isnew = false
                    }).ToList();

                    foreach (var itemac in activities) {
                        var uassigned = (from u in db.Usuarios where (u.ID_usuario == itemac.ID_usuarioEnd) select u).FirstOrDefault();
                        if (uassigned == null && itemac.ID_usuarioEnd == 0)
                        {
                            var usuario = (from a in CMKdb.OCRD where (a.CardCode == itemac.ID_usuarioEndString) select a).FirstOrDefault();
                            itemac.usuarioasignado = usuario.CardName.ToString() + " - " + usuario.E_Mail.ToString();
                        }
                        else {
                            itemac.usuarioasignado = uassigned.nombre + " " + uassigned.apellido;
                        }
                        
                       

                    }
                   

                    ViewBag.activities = activities;
                }
                else {
                    activities = (from a in db.ActivitiesM where (a.ID_visit == id && a.ID_usuarioEnd == ID) select new ActivitiesM_offline
                    {
                        ID_activity = a.ID_activity,
                        ID_visit = a.ID_visit,
                        ID_form = a.ID_form,
                        ID_customer = a.ID_customer,
                        Customer = a.Customer,
                        comments = a.comments,
                        check_in = a.check_in,
                        check_out = a.check_out,
                        query1 = a.query1,
                        ID_empresa = a.ID_empresa,
                        isfinished = a.isfinished,
                        description = a.description,
                        ID_usuarioCreate = a.ID_usuarioCreate,
                        ID_usuarioEnd = a.ID_usuarioEnd,
                        date = a.date,
                        ID_activitytype = a.ID_activitytype,
                        ID_usuarioEndString = a.ID_usuarioEndString,
                        onserver = true,
                        action = "download",
                        isnew = false
                    }).ToList();
                    foreach (var itemac in activities)
                    {
                        var uassigned = (from u in db.Usuarios where (u.ID_usuario == itemac.ID_usuarioEnd) select u).FirstOrDefault();

                        itemac.usuarioasignado = uassigned.nombre + " " + uassigned.apellido;

                    }
                    ViewBag.activities = activities;
                }
                //CHECK OUT POR USUARIO
                List<Visitstatea> lstestadov = new List<Visitstatea>();
                Visitstatea estadov = new Visitstatea();
                if (datosUsuario.ID_rol == 9 && datosUsuario.ID_tipomembresia == 8)
                {
                    try
                    {
                        var rep = (from a in db.VisitsM_representatives
                                   where (a.ID_visit == id && a.ID_usuario == datosUsuario.ID_usuario)
                                   select a
                              ).FirstOrDefault();

                        estadov.state = Convert.ToInt32(rep.query1);//Utilizaremos este campo para filtrar el estado por usuario
                        estadov.onserver = true;
                        estadov.action = "download";
                        estadov.ID_visit = id;
                        lstestadov.Add(estadov);

                        ViewBag.estadovisita = lstestadov.ToList();
                    }
                    catch {

                        estadov.state = 0;//Utilizaremos este campo para filtrar el estado por usuario
                        estadov.onserver = true;
                        estadov.action = "download";
                        estadov.ID_visit = id;
                        lstestadov.Add(estadov);

                        ViewBag.estadovisita = lstestadov.ToList();
                    }

                }
                else {
                    estadov.state = visitsM.ID_visitstate;//Utilizaremos este campo para filtrar el estado por usuario
                    estadov.onserver = true;
                    estadov.action = "download";
                    estadov.ID_visit = id;
                    lstestadov.Add(estadov);

                    ViewBag.estadovisita = lstestadov.ToList();
                    //ViewBag.estadovisita = visitsM.ID_visitstate;
                }

                

                ViewBag.idvisita = id;

                ViewBag.storename = visitsM.store;

                //FIN ACTIVITIES
                //representantes
                var usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9);

                ViewBag.representatives = usuarios.ToList();

                var usuariosdemo = CMKdb.OCRD.Where(b => b.Series == 70 && b.CardName != null && b.CardName != "" && b.CardType == "s").OrderBy(b => b.CardName).ToList();

                List<demosReps> selectList_usuarios = (from st in usuariosdemo
                                                                  select new demosReps
                                                                  {
                                                                      ID = Convert.ToString(st.CardCode),
                                                                      name = st.CardName.ToString() + " - " + st.E_Mail.ToString()
                                                                  }).ToList();
                ViewBag.reps_demos = selectList_usuarios;
                //Cargamos ruta 
                var ruta = (from r in db.RoutesM where (r.ID_route == visitsM.ID_route) select r).FirstOrDefault();
                //FORMULARIOS

                //List<int> FormsIds = ruta.query1.Split(',').Select(int.Parse).ToList();

                //var activeForms = (from at in db.FormsM where (FormsIds.Contains(at.ID_form)) select at).ToList();

                var activeForms = (from at in db.FormsM where (at.ID_empresa == GlobalVariables.ID_EMPRESA_USUARIO) select at).ToList();
                ViewBag.activeForms = activeForms;
                //FIN FORMULARIOS

                var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                ViewBag.customers = customers.ToList();
                //PARA OFFLINE
                List<VisitsMs_offline> lvisita = new List<VisitsMs_offline>();
                lvisita.Add(visitsM);
                ViewBag.visit_info = lvisita.ToList();

                //Route
                ViewBag.route = visitsM.ID_route;


                var geoLong = "";
                var geoLat = "";

                geoLong = visitsM.geoLong;
                geoLat = visitsM.geoLat;

                ViewBag.glong = geoLong;
                ViewBag.glat = geoLat;
                ViewBag.address = visitsM.address + ", " +  visitsM.city + ", " +  visitsM.state + ", " + visitsM.zipcode;


                ///DEVOLVEMOS VALORES DE ACTIVIDADES
                var activity = (from v in activities  select v.ID_activity).ToArray();

                if (activity == null)
                {
                   
                }
                else
                {

                    //LISTADO DE CLIENTES
                    //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                    List<MyObj_tablapadre> listapadresActivities = (from item in db.FormsM_details
                                                                    where (item.parent == 0 && activity.Contains(item.ID_visit) && item.original == false)
                                                                    select
                                                                       new MyObj_tablapadre
                                                                       {
                                                                           ID_details = item.ID_details,
                                                                           id_resource = item.ID_formresourcetype,
                                                                           fsource = item.fsource,
                                                                           fdescription = item.fdescription,
                                                                           fvalue = item.fvalue,
                                                                           fvalueDecimal = item.fvalueDecimal,
                                                                           fvalueText = item.fvalueText,
                                                                           ID_formM = item.ID_formM,
                                                                           ID_visit = item.ID_visit,
                                                                           original = item.original,
                                                                           obj_order = item.obj_order,
                                                                           obj_group = item.obj_group,
                                                                           idkey = item.idkey,
                                                                           parent = item.parent,
                                                                           query1 = item.query1,
                                                                           query2 = item.query2,
                                                                           ID_empresa = item.ID_empresa,
                                                                           onserver = true,
                                                                           action = "download",
                                                                           ID_visitOriginal = id,
                                                                           resourcetype = item.ID_formresourcetype,
                                                                           isnew = false
                                                                          
                                                                       }
                                          ).ToList();



                    List<tablahijospadre> listahijasActivities = (from item in db.FormsM_details
                                                                  where (activity.Contains(item.ID_visit) && item.original == false)
                                                                  select new tablahijospadre
                                                                  {
                                                                      ID_details = item.ID_details,
                                                                      id_resource = item.ID_formresourcetype,
                                                                      fsource = item.fsource,
                                                                      fdescription = item.fdescription,
                                                                      fvalue = item.fvalue,
                                                                      fvalueDecimal = item.fvalueDecimal,
                                                                      fvalueText = item.fvalueText,
                                                                      ID_formM = item.ID_formM,
                                                                      ID_visit = item.ID_visit,
                                                                      original = item.original,
                                                                      obj_order = item.obj_order,
                                                                      obj_group = item.obj_group,
                                                                      idkey = item.idkey,
                                                                      parent = item.parent,
                                                                      query1 = item.query1,
                                                                      query2 = item.query2,
                                                                      ID_empresa = item.ID_empresa,
                                                                      onserver = true,
                                                                      action = "download",
                                                                      ID_visitOriginal = id,
                                                                      resourcetype = item.ID_formresourcetype,
                                                                      isnew = false

                                                                  }).ToList();


                    List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);
                    ///FIN
                    ///

                    JavaScriptSerializer jsini = new JavaScriptSerializer();
                    jsini.MaxJsonLength = Int32.MaxValue;
                    var result = jsini.Serialize(categoriasListActivities);

                    ViewBag.details = result;

                    //DEVOLVEMOS FORMATOS DE FORMULARIOS USADOS

                    var lstformularios = (from f in activities select f.ID_form).ToList().Distinct().ToArray();

                    List<MyObj_tablapadre> listapadresActivities2 = (from item in db.FormsM_details
                                                                    where (item.parent == 0 && lstformularios.Contains(item.ID_formM) && item.original == true)
                                                                    select
                                                                       new MyObj_tablapadre
                                                                       {
                                                                           ID_details = item.ID_details,
                                                                           id_resource = item.ID_formresourcetype,
                                                                           fsource = item.fsource,
                                                                           fdescription = item.fdescription,
                                                                           fvalue = item.fvalue,
                                                                           fvalueDecimal = item.fvalueDecimal,
                                                                           fvalueText = item.fvalueText,
                                                                           ID_formM = item.ID_formM,
                                                                           ID_visit = item.ID_visit,
                                                                           original = item.original,
                                                                           obj_order = item.obj_order,
                                                                           obj_group = item.obj_group,
                                                                           idkey = item.idkey,
                                                                           parent = item.parent,
                                                                           query1 = item.query1,
                                                                           query2 = item.query2,
                                                                           ID_empresa = item.ID_empresa,
                                                                           onserver = true,
                                                                           action = "download",
                                                                           ID_visitOriginal = id,
                                                                           resourcetype = item.ID_formresourcetype,
                                                                           isnew = false

                                                                       }
                      ).ToList();



                    List<tablahijospadre> listahijasActivities2 = (from item in db.FormsM_details
                                                                  where (lstformularios.Contains(item.ID_formM) && item.original == true)
                                                                  select new tablahijospadre
                                                                  {
                                                                      ID_details = item.ID_details,
                                                                      id_resource = item.ID_formresourcetype,
                                                                      fsource = item.fsource,
                                                                      fdescription = item.fdescription,
                                                                      fvalue = item.fvalue,
                                                                      fvalueDecimal = item.fvalueDecimal,
                                                                      fvalueText = item.fvalueText,
                                                                      ID_formM = item.ID_formM,
                                                                      ID_visit = item.ID_visit,
                                                                      original = item.original,
                                                                      obj_order = item.obj_order,
                                                                      obj_group = item.obj_group,
                                                                      idkey = item.idkey,
                                                                      parent = item.parent,
                                                                      query1 = item.query1,
                                                                      query2 = item.query2,
                                                                      ID_empresa = item.ID_empresa,
                                                                      onserver = true,
                                                                      action = "download",
                                                                      ID_visitOriginal = id,
                                                                      resourcetype = item.ID_formresourcetype,
                                                                      isnew = false

                                                                  }).ToList();


                    List<MyObj_tablapadre> categoriasListActivities2 = ObtenerCategoriarJerarquiaByIDFormato(listapadresActivities2, listahijasActivities2);
                    var result2 = jsini.Serialize(categoriasListActivities2);

                    ViewBag.formatosForms = result2;

                }
                HttpCookie aCookie = Request.Cookies["currentcache"];
                string d = Server.HtmlEncode(aCookie.Value);
                ViewBag.cachem = d;
                return View(visitsM);

            }
            else
            {
                return RedirectToAction("Index","Home", null);
            }


            
        }

        public static List<MyObj_tablapadre> ObtenerCategoriarJerarquiaByID(List<MyObj_tablapadre> Categoriaspadre, List<tablahijospadre> Categoriashijas)
        {


            List<MyObj_tablapadre> query = (from item in Categoriaspadre

                                            select new MyObj_tablapadre
                                            {
                                                ID_details = item.ID_details,
                                                id_resource = item.id_resource,
                                                fsource = item.fsource,
                                                fdescription = item.fdescription,
                                                fvalue = item.fvalue,
                                                fvalueDecimal = item.fvalueDecimal,
                                                fvalueText = item.fvalueText,
                                                ID_formM = item.ID_formM,
                                                ID_visit = item.ID_visit,
                                                original = item.original,
                                                obj_order = item.obj_order,
                                                obj_group = item.obj_group,
                                                idkey = item.idkey,
                                                parent = item.parent,
                                                query1 = item.query1,
                                                query2 = item.query2,
                                                ID_empresa = item.ID_empresa,
                                                onserver = item.onserver,
                                                action = item.action,
                                                ID_visitOriginal = item.ID_visitOriginal,
                                                resourcetype = item.resourcetype,
                                                isnew = item.isnew,
                                                children = ObtenerHijosByID(item.idkey, Categoriashijas, item.ID_visit)

                                            }).ToList();

            return query;





        }

        private static List<MyObj_tablapadre> ObtenerHijosByID(int ID_parent, List<tablahijospadre> categoriashijas, int ID_visit)
        {



            List<MyObj_tablapadre> query = (from item in categoriashijas

                                            where item.parent == ID_parent && item.ID_visit == ID_visit
                                            select new MyObj_tablapadre
                                            {
                                                ID_details = item.ID_details,
                                                id_resource = item.id_resource,
                                                fsource = item.fsource,
                                                fdescription = item.fdescription,
                                                fvalue = item.fvalue,
                                                fvalueDecimal = item.fvalueDecimal,
                                                fvalueText = item.fvalueText,
                                                ID_formM = item.ID_formM,
                                                ID_visit = item.ID_visit,
                                                original = item.original,
                                                obj_order = item.obj_order,
                                                obj_group = item.obj_group,
                                                idkey = item.idkey,
                                                parent = item.parent,
                                                query1 = item.query1,
                                                query2 = item.query2,
                                                ID_empresa = item.ID_empresa,
                                                onserver = item.onserver,
                                                action = item.action,
                                                ID_visitOriginal = item.ID_visitOriginal,
                                                resourcetype = item.resourcetype,
                                                isnew = item.isnew,
                                                children = ObtenerHijosByID(item.idkey, categoriashijas, item.ID_visit)
                                            }).ToList();

            return query;

        }
        //PARA OFFLINE FORMATOS
        public static List<MyObj_tablapadre> ObtenerCategoriarJerarquiaByIDFormato(List<MyObj_tablapadre> Categoriaspadre, List<tablahijospadre> Categoriashijas)
        {


            List<MyObj_tablapadre> query = (from item in Categoriaspadre

                                            select new MyObj_tablapadre
                                            {
                                                ID_details = item.ID_details,
                                                id_resource = item.id_resource,
                                                fsource = item.fsource,
                                                fdescription = item.fdescription,
                                                fvalue = item.fvalue,
                                                fvalueDecimal = item.fvalueDecimal,
                                                fvalueText = item.fvalueText,
                                                ID_formM = item.ID_formM,
                                                ID_visit = item.ID_visit,
                                                original = item.original,
                                                obj_order = item.obj_order,
                                                obj_group = item.obj_group,
                                                idkey = item.idkey,
                                                parent = item.parent,
                                                query1 = item.query1,
                                                query2 = item.query2,
                                                ID_empresa = item.ID_empresa,
                                                onserver = item.onserver,
                                                action = item.action,
                                                ID_visitOriginal = item.ID_visitOriginal,
                                                resourcetype = item.resourcetype,
                                                isnew = item.isnew,
                                                children = ObtenerHijosByIDFormato(item.idkey, Categoriashijas, item.ID_formM)

                                            }).ToList();

            return query;





        }

        private static List<MyObj_tablapadre> ObtenerHijosByIDFormato(int ID_parent, List<tablahijospadre> categoriashijas, int ID_visit)
        {



            List<MyObj_tablapadre> query = (from item in categoriashijas

                                            where item.parent == ID_parent && item.ID_formM == ID_visit
                                            select new MyObj_tablapadre
                                            {
                                                ID_details = item.ID_details,
                                                id_resource = item.id_resource,
                                                fsource = item.fsource,
                                                fdescription = item.fdescription,
                                                fvalue = item.fvalue,
                                                fvalueDecimal = item.fvalueDecimal,
                                                fvalueText = item.fvalueText,
                                                ID_formM = item.ID_formM,
                                                ID_visit = item.ID_visit,
                                                original = item.original,
                                                obj_order = item.obj_order,
                                                obj_group = item.obj_group,
                                                idkey = item.idkey,
                                                parent = item.parent,
                                                query1 = item.query1,
                                                query2 = item.query2,
                                                ID_empresa = item.ID_empresa,
                                                onserver = item.onserver,
                                                action = item.action,
                                                ID_visitOriginal = item.ID_visitOriginal,
                                                resourcetype = item.resourcetype,
                                                isnew = item.isnew,
                                                children = ObtenerHijosByIDFormato(item.idkey, categoriashijas, item.ID_formM)
                                            }).ToList();

            return query;

        }
        public static bool IsFileReady(string filename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public ActionResult DetailsOffline(int? id)
        {

            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                //VisitsM visitsM = db.VisitsM.Find(id);

    
                //List<ActivitiesM> activities = new List<ActivitiesM>();

                //if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)
                //{
                //    activities = (from a in db.ActivitiesM where (a.ID_visit == id) select a).ToList();

                //    foreach (var itemac in activities)
                //    {
                //        var uassigned = (from u in db.Usuarios where (u.ID_usuario == itemac.ID_usuarioEnd) select u).FirstOrDefault();
                //        if (uassigned == null && itemac.ID_usuarioEnd == 0)
                //        {
                //            var usuario = (from a in CMKdb.OCRD where (a.CardCode == itemac.ID_usuarioEndString) select a).FirstOrDefault();
                //            itemac.ID_customer = usuario.CardName.ToString() + " - " + usuario.E_Mail.ToString();
                //        }
                //        else
                //        {
                //            itemac.ID_customer = uassigned.nombre + " " + uassigned.apellido;
                //        }



                //    }


                //    ViewBag.activities = activities;
                //}
                //else
                //{
                //    activities = (from a in db.ActivitiesM where (a.ID_visit == id && a.ID_usuarioEnd == ID) select a).ToList();

                //    foreach (var itemac in activities)
                //    {
                //        var uassigned = (from u in db.Usuarios where (u.ID_usuario == itemac.ID_usuarioEnd) select u).FirstOrDefault();

                //        itemac.ID_customer = uassigned.nombre + " " + uassigned.apellido;

                //    }
                //    //ViewBag.activities = activities;
                //}
                
                ////PARA CREAR MANIFEST

                //var root = Server.MapPath("~");
                //StringBuilder SB = new StringBuilder();

                ////Devolvemos versionamiento de cache manifest
                ////var path_ver = Path.Combine(root, @"versionamiento.txt");
                ////System.IO.StreamReader readingFile = new System.IO.StreamReader(path_ver);

                ////string readingLine = readingFile.ReadLine();
                ////Header
                //SB.AppendLine("CACHE MANIFEST");
                //SB.AppendLine("# version 1.0.0." + id);
                //SB.AppendLine();
                ////Content
                //SB.AppendLine("# Site Content");
                //SB.AppendLine("CACHE:");
                //SB.AppendLine(@"/VisitsMs/Details/" + id);
                //foreach (var cache_act in activities)
                //{
                //    if (cache_act.ID_activitytype == 1)//Forms
                //    {
                //        SB.AppendLine(@"/FormsM/Activity/" + cache_act.ID_activity);
                //    }
                //    else if (cache_act.ID_activitytype == 2)
                //    {//Audits
                //        SB.AppendLine(@"/FormsM/Activityra/" + cache_act.ID_activity);
                //    }
                //    else if (cache_act.ID_activitytype == 3)//Sales Orders
                //    {

                //    }
                //    else if (cache_act.ID_activitytype == 4)//Demos
                //    {
                //        SB.AppendLine(@"/FormsM/Activity/" + cache_act.ID_activity);
                //    }



                //}
                //SB.AppendLine();
                //SB.AppendLine("# Scripts");
                //SB.AppendLine(@"https://cdn.jsdelivr.net/jquery/latest/jquery.min.js");
                //SB.AppendLine(@"/Content/newstyle2/vendors/js/vendor.bundle.base.js");
                //SB.AppendLine(@"/Content/newstyle2/vendors/js/vendor.bundle.addons.js");
                //SB.AppendLine(@"/Content/newstyle2/js/off-canvas.js");
                //SB.AppendLine(@"/Content/newstyle2/js/hoverable-collapse.js");
                //SB.AppendLine(@"/Content/newstyle2/js/misc.js");
                //SB.AppendLine(@"/Content/newstyle2/js/settings.js");
                //SB.AppendLine(@"/Content/newstyle2/js/todolist.js");
                //SB.AppendLine(@"/Content/newstyle2/js/dashboard.js");
                //SB.AppendLine(@"/Content/newstyle2/js/horizontal-menu.js");
                //SB.AppendLine(@"/Content/newstyle2/js/formpickers.js");
                //SB.AppendLine(@"/Content/newstyle2/js/calendar.js");
                //SB.AppendLine(@"/Content/newstyle2/js/x-editable.js");
                //SB.AppendLine(@"/Content/newstyle2/js/dropify.js");
                //SB.AppendLine(@"/Content/newstyle2/js/dropzone.js");
                //SB.AppendLine(@"/Content/newstyle2/js/jquery-file-upload.js");
                //SB.AppendLine(@"/Content/newstyle2/js/formpickers.js");
                //SB.AppendLine(@"/Content/newstyle2/js/form-repeater.js");
                //SB.AppendLine(@"/Content/newstyle2/js/data-table.js");
                //SB.AppendLine(@"/Content/newstyle2/js/select2.js");
                //SB.AppendLine(@"/Content/newstyle2/js/screenfull.js");
                //SB.AppendLine(@"/Content/newstyle2/js/tooltips.js");
                //SB.AppendLine(@"/Content/newstyle2/js/popover.js");
                //SB.AppendLine(@"/Content/newstyle2/js/iCheck.js");
                //SB.AppendLine(@"/Content/newstyle2/js/typeahead.js");
                //SB.AppendLine(@"/Content/newstyle2/js/dropify.js");
                //SB.AppendLine(@"/Content/newstyle2/js/jquery.watermark.min.js");
                //SB.AppendLine(@"https://cdn.jsdelivr.net/momentjs/latest/moment.min.js");
                //SB.AppendLine(@"https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.min.js");
                //SB.AppendLine(@"https://cdnjs.cloudflare.com/ajax/libs/select2/3.5.2/select2.min.js");
                //SB.AppendLine(@"https://cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.3/modernizr.min.js");
                //SB.AppendLine(@"https://cdn.datatables.net/responsive/2.2.3/js/dataTables.responsive.min.js");
                //SB.AppendLine(@"/Content/js/signature.js");
                //SB.AppendLine(@"/Content/newstyle2/js/JIC.js");

                ////images
                //SB.AppendLine(@"/Content/newstyle2/images/LogoCMK.png");
                //SB.AppendLine(@"/Content/newstyle2/images/Logo_watermark.png");
                //SB.AppendLine(@"/Content/newstyle2/images/samples/online-store.png");
                //SB.AppendLine();
                //SB.AppendLine("# Styles");
                //SB.AppendLine(@"/Content/newstyle2/vendors/iconfonts/mdi/css/materialdesignicons.min.css");
                //SB.AppendLine(@"/Content/newstyle2/vendors/iconfonts/puse-icons-feather/feather.css");
                //SB.AppendLine(@"/Content/newstyle2/vendors/css/vendor.bundle.base.css");
                //SB.AppendLine(@"/Content/newstyle2/vendors/css/vendor.bundle.addons.css");
                //SB.AppendLine(@"/Content/newstyle2/vendors/iconfonts/flag-icon-css/css/flag-icon.min.css");
                //SB.AppendLine(@"/Content/newstyle2/css/style.css");
                //SB.AppendLine(@"/Content/newstyle2/images/favicon-16x16.png");
                //SB.AppendLine(@"https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.css");
                //SB.AppendLine(@"/Content/newstyle2/css/nestable.css");
                //SB.AppendLine(@"/Content/css/multiple-select.css");
                //SB.AppendLine(@"https://cdnjs.cloudflare.com/ajax/libs/select2/3.5.2/select2.min.css");
                //SB.AppendLine(@"/Content/dist/css/map-icons.css");
                //SB.AppendLine(@"https://cdn.datatables.net/responsive/2.2.3/css/responsive.dataTables.min.css");
                //SB.AppendLine(@"/Content/newstyle2/css/retailauditresponsive.css");
                //SB.AppendLine(@"/Content/newstyle2/css/owl.carousel.css");
                //SB.AppendLine(@"/Content/newstyle2/css/owl.theme.css");
                //SB.AppendLine(@"https://cdn.datatables.net/responsive/2.2.3/css/responsive.dataTables.min.css");
                //SB.AppendLine();

                ////NETWORK section to the mainfest file.
                //SB.AppendLine("NETWORK:");
                //SB.AppendLine(@"*");
                ////SB.AppendLine("http://*");
                ////SB.AppendLine("https://*");


                //var path = Path.Combine(root, @"offline.appcache");
                //System.IO.StreamWriter file = new System.IO.StreamWriter(path, false);


                //file.WriteLine(SB.ToString());
                //file.Close();

                //var isReady = false;
                //while (!isReady)
                //{
                //    isReady = IsFileReady(path);
                //}
                string result = "exito";
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            else
            {
                string result = "fallo";
                return Json(result, JsonRequestBehavior.AllowGet);
            }



        }
        public ActionResult DetailsC(int? id, string ID_Customer, string brand)
        {

            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;


                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                VisitsM visitsM = db.VisitsM.Find(id);
                if (visitsM == null)
                {
                    return HttpNotFound();
                }
                
                //ID_Customer = datosUsuario.Empresas.ID_SAP;

                //Seleccionamos representantes que estan incluidos
                var reps = (from rep in db.VisitsM_representatives
                            where (rep.ID_visit == id)
                            select new representativesVisit2
                            {
                                ID = rep.ID_usuario,
                                name = "",
                                email = "",
                                customer = ""
                            }
                           ).ToList();

                foreach (var itemR in reps)
                {
                    var user = (from u in db.Usuarios where (u.ID_usuario == itemR.ID) select u).FirstOrDefault();
                    if (user != null) { itemR.name = user.nombre + " " + user.apellido; itemR.email = user.correo; itemR.customer = user.estados_influencia; } else { itemR.name = ""; itemR.email = ""; itemR.customer = ""; }
                }

                List<representativesVisit2> listau = new List<representativesVisit2>();

                foreach (var user in reps)
                {
                    List<string> storeIds = user.customer.Split(',').ToList();

                    foreach (var it in storeIds)
                    {
                        if (it == ID_Customer)
                        {
                            listau.Add(user);
                        }
                    }

                }
           


                ViewBag.repslist = listau.ToList();
                //FIN representante
                
                    ViewBag.estadovisita = visitsM.ID_visitstate;
                



                ViewBag.idvisita = id;

                ViewBag.storename = visitsM.store;
                ViewBag.storead = visitsM.address;
                //FIN ACTIVITIES
                //representantes
                var usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9);
                ViewBag.representatives = usuarios.ToList();
                //Cargamos ruta 
                var ruta = (from r in db.RoutesM where (r.ID_route == visitsM.ID_route) select r).FirstOrDefault();
                //FORMULARIOS

                //List<int> FormsIds = ruta.query1.Split(',').Select(int.Parse).ToList();

                //var activeForms = (from at in db.FormsM where (FormsIds.Contains(at.ID_form)) select at).ToList();

                var activeForms = (from at in db.FormsM where (at.ID_empresa == GlobalVariables.ID_EMPRESA_USUARIO) select at).ToList();
                ViewBag.activeForms = activeForms;
                //FIN FORMULARIOS

                //var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                //ViewBag.customers = customers.ToList();
                List<ActivitiesM> actividades = new List<ActivitiesM>();

                if (brand == null || brand == "" || brand=="0")
                {
                     actividades = (from a in db.ActivitiesM where (a.ID_customer == ID_Customer && a.ID_visit == id) select a).ToList();
                }
                else {

                    actividades = (from a in db.ActivitiesM where (a.ID_customer == ID_Customer && a.ID_visit == id) select a).ToList();

                    var activitiesdef = (from a in actividades select a.ID_activity).Distinct().ToArray();

                    var formsdef = (from a in db.FormsM_details where (activitiesdef.Contains(a.ID_visit) && a.ID_formresourcetype == 13 && a.fvalueText.StartsWith(brand) && a.fvalueText.EndsWith(brand)) select a.ID_visit).Distinct().ToArray();


                    actividades = (from a in actividades where (formsdef.Contains(a.ID_activity)) select a).ToList();



                }

                foreach (var item in actividades) {
                    var dt = (from a in db.FormsM_details where (a.ID_formresourcetype == 13 && a.ID_visit == item.ID_activity) select a).FirstOrDefault();
                    if (dt != null) {
                        item.ID_customer = dt.fdescription;
                    }
                    else{
                        item.ID_customer = "NA";
                    }

                }

                ViewBag.branddef = brand;

                ViewBag.actividades = actividades;


                //Route
             
                ViewBag.customerID = ID_Customer;
                return View(visitsM);

            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }



        }

        public class representativesVisit{
              public int ID { get; set; }
            public string name { get; set; }
            public string email { get; set; }
        }
        public class representativesVisit2
        {
            public int ID { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string customer { get; set; }
        }
        public ActionResult GetCustomer_reps(string ID_usuario)
        {
            //Verificamos si se eligio un usuario Demo
            int isDemoUser = 0;
            int IDusuario = 0;
            try
            {
                IDusuario = Convert.ToInt32(ID_usuario);
                isDemoUser = 0;
            }
            catch {
                isDemoUser = 1;
            }
            try
            {
                if (isDemoUser == 0)//ES REPRESENTANTE, APLICAMOS FILTROS
                {
                    var usuario = (from a in db.Usuarios where (a.ID_usuario == IDusuario) select a).FirstOrDefault();



                    var lstCustomer = (from b in CMKdb.OCRD where (usuario.estados_influencia.Contains(b.CardCode)) select new { ID = b.CardCode, Name = b.CardName }).OrderBy(b => b.Name).ToList();

                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string result = javaScriptSerializer.Serialize(lstCustomer);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else {//NO ES ENTONCES MOSTRAMOS TODOS

                    var lstCustomer = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select new { ID = b.CardCode, Name = b.CardName }).OrderBy(b => b.Name).ToList();

                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string result = javaScriptSerializer.Serialize(lstCustomer);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Gettype(string ID_type)
        {

            try
            {
                int id = Convert.ToInt32(ID_type);

                FormsM form = db.FormsM.Find(id);


                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string result = javaScriptSerializer.Serialize(form.ID_activity);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateActivitySuggestedOrder(string ID_form, string ID_customer, string ID_visitaSO, string ID_rep)
        {
            try
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();
                ID_form = "27";
                ID_rep = datosUsuario.ID_usuario.ToString();
                int IDForm = 27; //Cambiar si se mueve, elimina o crea uno nuevo
                int IDRep = 0;

                ID_customer = "0";

                int esDemoUser = 0;//Variable para identificar si el usuario seleccionado es demo o es un reps

                try
                {
                    IDRep = Convert.ToInt32(ID_rep);
                    esDemoUser = 0;
                }
                catch
                {
                    esDemoUser = 1;
                }


                //CREAMOS LA ESTRUCTURA DE LA ACTIVIDAD
                ActivitiesM nuevaActivida = new ActivitiesM();

                nuevaActivida.ID_form = Convert.ToInt32(ID_form);
                nuevaActivida.ID_visit = Convert.ToInt32(ID_visitaSO);
                nuevaActivida.ID_customer = ID_customer;
                nuevaActivida.Customer = "";
                var customer = (from c in CMKdb.OCRD where (c.CardCode == ID_customer) select c).FirstOrDefault();
                if (customer != null)
                {
                    nuevaActivida.Customer = customer.CardName;
                }
                nuevaActivida.comments = "";
                nuevaActivida.check_in = DateTime.Today.Date;
                nuevaActivida.check_out = DateTime.Today.Date;
                nuevaActivida.query1 = "";
                nuevaActivida.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;
                nuevaActivida.isfinished = false;
                nuevaActivida.description = "";
                nuevaActivida.ID_activitytype = 0;

                nuevaActivida.date = DateTime.Today.Date;
                var form = (from c in db.FormsM where (c.ID_form == IDForm) select c).FirstOrDefault();
                if (form != null)
                {
                    nuevaActivida.description = form.name;
                    nuevaActivida.ID_activitytype = form.ID_activity;
                    nuevaActivida.query1 = "";

                }

                int ID_usuario = Convert.ToInt32(Session["IDusuario"]);
                nuevaActivida.ID_usuarioCreate = ID_usuario;

                //OJO ESTA PARTE SE AGREGO PARA COMERCIA ES PROPIA DE LA EMPRESA
                if (nuevaActivida.ID_activitytype != 4)
                {
                    nuevaActivida.ID_usuarioEnd = IDRep;  //Usuario que sera asignado
                    nuevaActivida.ID_usuarioEndString = "";
                }
                else
                {
                    //Guardamos el usuario de SAP en la variable tipo String
                    if (esDemoUser == 1)
                    {
                        nuevaActivida.ID_usuarioEnd = 0;
                    }
                    else
                    {
                        nuevaActivida.ID_usuarioEnd = IDRep;

                    }
                    nuevaActivida.ID_usuarioEndString = ID_rep;
                }

                //SOLO PARA COMERCIA
                if (nuevaActivida.ID_activitytype == 4)
                {

                    nuevaActivida.query1 = "start";
                }



                //guardamos
                db.ActivitiesM.Add(nuevaActivida);
                db.SaveChanges();

                //LUEGO ASIGNAMOS LA PLANTILLA DE FORMULARIO A LA ACTIVIDAD
                //Guardamos el detalle
                var sql = @"usp_CreateFormDetail @IDVisit, @IDForm";
                db.Database.ExecuteSqlCommand(sql,
                    new SqlParameter("@IDVisit", nuevaActivida.ID_activity),
                    new SqlParameter("@IDForm", IDForm));


                //var detalles_acopiar = (from a in db.FormsM_details where (a.ID_formM == IDForm && a.original == true) select a).AsNoTracking().ToList();
                //List<FormsM_details> dtlstoinsert = new List<FormsM_details>();
                //foreach (var item in detalles_acopiar)
                //{
                //    FormsM_details nuevodetalle = new FormsM_details();
                //    nuevodetalle = item;
                //    nuevodetalle.original = false;
                //    nuevodetalle.ID_visit = nuevaActivida.ID_activity; //TOMAREMOS ID VISIT COMO ID ACTIVITY PORQUE ES POR REPRESENTANTE Y NO POR VISITA

                //    //db.FormsM_details.Add(nuevodetalle);
                //    //db.SaveChanges();
                //    dtlstoinsert.Add(nuevodetalle);
                //}
                //db.BulkInsert(dtlstoinsert);

                //Por ultimo asignamos el usuario a la visita
                //Pero verificamos si ya existe

                var existeenvisita = (from v in db.VisitsM_representatives where (v.ID_visit == nuevaActivida.ID_visit && v.ID_usuario == IDRep) select v).Count();


                if (existeenvisita > 0)

                {
                }
                else
                {

                    if (esDemoUser == 1)

                    {
                        VisitsM_representatives repvisita = new VisitsM_representatives();

                        repvisita.ID_visit = nuevaActivida.ID_visit;
                        repvisita.ID_usuario = 0;
                        repvisita.query1 = nuevaActivida.ID_usuarioEndString;
                        repvisita.ID_empresa = nuevaActivida.ID_empresa;
                        db.VisitsM_representatives.Add(repvisita);
                        db.SaveChanges();
                    }
                    else
                    {
                        VisitsM_representatives repvisita = new VisitsM_representatives();

                        repvisita.ID_visit = nuevaActivida.ID_visit;
                        repvisita.ID_usuario = IDRep;
                        repvisita.query1 = "3";
                        repvisita.ID_empresa = nuevaActivida.ID_empresa;
                        db.VisitsM_representatives.Add(repvisita);
                        db.SaveChanges();

                    }

                }

                //enviamoS correo SI ES UN USUARIO DEMO
                if (esDemoUser == 1)
                {
                    //Obtenemos el nombre de las marcas o brands por cada articulo
                    var listadeItems = (from d in db.FormsM_details where (d.ID_visit == nuevaActivida.ID_activity && d.ID_formresourcetype == 3) select d).ToList();


                    foreach (var itemd in listadeItems)
                    {
                        itemd.fdescription = (from k in CMKdb.OITM join j in CMKdb.OMRC on k.FirmCode equals j.FirmCode where (k.ItemCode == itemd.fsource) select j.FirmName).FirstOrDefault();
                    }

                    var brands = listadeItems.GroupBy(test => test.fdescription).Select(grp => grp.First()).ToList();

                    var brandstoshow = "";
                    int count = 0;
                    foreach (var items in brands)
                    {
                        if (count == 0)
                        {
                            try
                            {
                                brandstoshow = items.fdescription.ToString();
                            }
                            catch
                            {
                                brandstoshow = "no data from db";
                            }

                        }
                        else
                        {
                            try
                            {
                                brandstoshow += ", " + items.fdescription.ToString();
                            }
                            catch
                            {
                                brandstoshow = "no data from db";
                            }
                        }
                        count += 1;
                    }
                    //*******************************
                    try
                    {
                        var usuario = (from a in CMKdb.OCRD where (a.CardCode == nuevaActivida.ID_usuarioEndString) select a).FirstOrDefault();

                        var store = (from s in db.VisitsM where (s.ID_visit == nuevaActivida.ID_visit) select s).FirstOrDefault();

                        //Enviamos correo para notificar
                        dynamic email = new Email("newdemo_alert");
                        email.To = usuario.E_Mail.ToString();
                        email.From = "customercare@comerciamarketing.com";
                        email.Vendor = brandstoshow;
                        email.Date = Convert.ToDateTime(nuevaActivida.date).ToLongDateString();
                        email.Time = Convert.ToDateTime(nuevaActivida.date).ToLongTimeString();
                        email.Place = store.store + ", " + store.address;
                        //email.link = "https://comerciamarketing.com/Home/Internal" + demos.ID_demo + Server.HtmlDecode("&") + "id_form=" + demos.ID_form;
                        email.link = "https://comerciamarketing.com/Home/Internal";
                        email.accesscode = "ACCMK00" + nuevaActivida.ID_activity.ToString();
                        email.enddate = Convert.ToDateTime(nuevaActivida.date).AddDays(1).ToLongDateString();
                        email.Send();

                        //FIN email
                    }
                    catch
                    {

                    }
                }

                //************

                TempData["exito"] = "Activity created successfully.";
                return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = ID_visitaSO });
            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = ID_visitaSO });
            }




        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTask(string ID_form, string ID_Vendor, string ID_rep, DateTime time)
        {
            try
            {
                int IDForm = Convert.ToInt32(ID_form);

                //CREAMOS LA ESTRUCTURA DE LA ACTIVIDAD
                Tasks nuevaActividad = new Tasks();

                nuevaActividad.ID_formM = Convert.ToInt32(ID_form);
                nuevaActividad.ID_Store = "";
                nuevaActividad.store = "";
                nuevaActividad.ID_brands = "";
                nuevaActividad.Brands = "";
                nuevaActividad.ID_Customer = ID_Vendor;
      
                var vendor = (from c in CMKdb.OCRD where (c.CardCode == ID_Vendor) select c).FirstOrDefault();
                if (vendor != null)
                {
                    nuevaActividad.Customer = vendor.CardName;
                }



                    nuevaActividad.ID_Store = "";
                    nuevaActividad.store = "";
                    nuevaActividad.address = "";
                    nuevaActividad.city = "";
                    nuevaActividad.zipcode = "";

                nuevaActividad.state = "";

                        nuevaActividad.geoLong = "";
                        nuevaActividad.geoLat = "";


                //FIN
                int idrepint = Convert.ToInt32(ID_rep);
                nuevaActividad.ID_taskstatus = 3;
                nuevaActividad.comments = "";
                nuevaActividad.check_in = DateTime.Today.Date;
                nuevaActividad.end_date = DateTime.Today.Date;
                nuevaActividad.ID_empresa = 2;
                nuevaActividad.ID_userCreate = 0;
                nuevaActividad.visit_date = DateTime.Today.Date;
                nuevaActividad.ID_formM = Convert.ToInt32(ID_form);
                nuevaActividad.ID_userEnd = idrepint;
                nuevaActividad.ID_ExternalUser = "";
                nuevaActividad.extra_hours = 0;

                //1 - Inventario
                //2 - 
                nuevaActividad.ID_taskType = 1;
                nuevaActividad.TaskType = "Inventory";
                nuevaActividad.Task_description = "Take products stock by Customer";

                var usuario = (from a in db.Usuarios where (a.ID_usuario == idrepint) select a).FirstOrDefault();

                nuevaActividad.UserName = usuario.nombre + " "  + usuario.apellido;

                //guardamos
                db.Tasks.Add(nuevaActividad);
                db.SaveChanges();

                //LUEGO ASIGNAMOS LA PLANTILLA DE FORMULARIO A LA ACTIVIDAD
                //Guardamos el detalle
                //var detalles_acopiar = (from a in db.FormsM_details where (a.ID_formM == IDForm && a.original == true) select a).AsNoTracking().ToList();

                var sql = @"usp_CreateFormDetailTasks @IDVisit, @IDForm";
                db.Database.ExecuteSqlCommand(sql,
                    new SqlParameter("@IDVisit", nuevaActividad.ID_task),
                    new SqlParameter("@IDForm", IDForm));



                TempData["exito"] = "Task created successfully.";
                return RedirectToAction("Tasks", "Admin", null);
            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Tasks", "Admin", null);
            }




        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateActivityDemo(string ID_form, string ID_customer, string ID_Vendor,string ID_brandSel, string ID_rep, DateTime time, string products_listSel)
        {
            try
            {
                int IDForm = Convert.ToInt32(ID_form);


                int esDemoUser = 0;//Variable para identificar si el usuario seleccionado es demo o es un reps
                esDemoUser = 1;

                //CREAMOS LA ESTRUCTURA DE LA ACTIVIDAD
                Demos nuevaActividad = new Demos();

                nuevaActividad.ID_formM = Convert.ToInt32(ID_form);
                nuevaActividad.ID_Vendor = ID_Vendor;
                nuevaActividad.vendor = "";
                nuevaActividad.ID_Store = "";
                nuevaActividad.store = "";
                nuevaActividad.ID_brands = ID_brandSel.ToString();
                nuevaActividad.Brands = "";

                List<Int16?> idbrand = new List<Int16?>();

                idbrand = ID_brandSel.Split(',').Select(Int16.Parse).Cast<Int16?>().ToList();
                

                var vendor = (from c in CMKdb.OCRD where (c.CardCode == ID_Vendor) select c).FirstOrDefault();
                if (vendor != null)
                {
                    nuevaActividad.vendor = vendor.CardName;
                }
        
                var brand = (from c in CMKdb.view_CMKEditorB where (idbrand.Contains(c.FirmCode)) select c.FirmName).Distinct().ToList();
                if (brand.Count>0)
                {
                    nuevaActividad.Brands = string.Join(",", brand);
                }



                //STORE
                var storeSAP = (from s in CMKdb.OCRD where (s.CardCode == ID_customer) select s).FirstOrDefault();
                if (storeSAP != null)
                {
                    nuevaActividad.ID_Store = ID_customer;
                    nuevaActividad.store = storeSAP.CardName;
                    nuevaActividad.address = storeSAP.MailAddres;
                    nuevaActividad.city = storeSAP.MailCity;
          

                    if (storeSAP.MailZipCod == null)
                    {
                        nuevaActividad.zipcode = "";
                    }
                    else { nuevaActividad.zipcode = storeSAP.MailZipCod; }

                    if (storeSAP.State2 == null)
                    {
                        nuevaActividad.state = "";
                    }
                    else { nuevaActividad.state = storeSAP.State2; }
                    //GEOLOCALIZACION
                    try
                    {
                        string address = storeSAP.CardName.ToString() + ", " + storeSAP.MailAddres.ToString() + ", " + storeSAP.MailCity.ToString() + ", " + storeSAP.MailZipCod.ToString();
                        string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyC3zDvE8enJJUHLSmhFAdWhPRy_tNSdQ6g&address={0}&sensor=false", Uri.EscapeDataString(address));

                        WebRequest request = WebRequest.Create(requestUri);
                        WebResponse response = request.GetResponse();
                        XDocument xdoc = XDocument.Load(response.GetResponseStream());

                        XElement result = xdoc.Element("GeocodeResponse").Element("result");
                        XElement locationElement = result.Element("geometry").Element("location");
                        XElement lat = locationElement.Element("lat");
                        XElement lng = locationElement.Element("lng");
                        //NO SE PORQUE LO TIRA AL REVEZ
                        nuevaActividad.geoLat = lng.Value;
                        nuevaActividad.geoLong = lat.Value;
                        //FIN

                    }
                    catch
                    {
                        nuevaActividad.geoLong = "";
                        nuevaActividad.geoLat = "";
                    }
                }

                //FIN
                nuevaActividad.ID_demostate = 3;
                nuevaActividad.comments = "";
                try
                {
                    nuevaActividad.check_in = time;
                    nuevaActividad.end_date = time;
                    nuevaActividad.visit_date = time;
                }
                catch {
                    nuevaActividad.check_in = DateTime.Today.Date;
                    nuevaActividad.end_date = DateTime.Today.Date;
                    nuevaActividad.visit_date = DateTime.Today.Date;
                }

               
                nuevaActividad.ID_empresa = 2;
                nuevaActividad.ID_userCreate = 0;
               
                nuevaActividad.ID_formM = Convert.ToInt32(ID_form);
                nuevaActividad.ID_userEnd = 0;
                nuevaActividad.ID_ExternalUser = ID_rep;
                nuevaActividad.extra_hours = 0;

                var usuario = (from a in CMKdb.OCRD where (a.CardCode == ID_rep) select a).FirstOrDefault();

                nuevaActividad.UserName = usuario.CardName;

                //guardamos
                db.Demos.Add(nuevaActividad);
                db.SaveChanges();

                //LUEGO ASIGNAMOS LA PLANTILLA DE FORMULARIO A LA ACTIVIDAD
                //Guardamos el detalle
                //var detalles_acopiar = (from a in db.FormsM_details where (a.ID_formM == IDForm && a.original == true) select a).AsNoTracking().ToList();


                if (IDForm == 41) //Si es nuevo diseno DEMO
                {
                    var sql = @"usp_CreateFormDetailDemoHeader @IDVisit, @IDForm";
                    db.Database.ExecuteSqlCommand(sql,
                    new SqlParameter("@IDVisit", nuevaActividad.ID_demo),
                    new SqlParameter("@IDForm", IDForm));


                    var countItems = 7;
                    var nuevacuenta = countItems + 2;
                    List<string> TagIds = products_listSel.Split(',').ToList();

                    //Recuperamos lista de items
                    //Tomamos el detalle saltando los primeros 6 registros y tomando los ultimos 34 (las fotos seran aparte y la firma es el 40)
                   var listadetalle = (from a in db.FormsM_details where (a.ID_formM == 41 && a.original == true) select new demosDetails
                   {
                       ID_details = a.ID_details,
                       ID_formresourcetype = a.ID_formresourcetype,
                       fsource = a.fsource,
                       fdescription = a.fdescription,
                       fvalue = a.fvalue,
                       fvalueDecimal = a.fvalueDecimal,
                       fvalueText = a.fvalueText,
                       ID_formM = a.ID_formM,
                       ID_visit = a.ID_visit,
                       original = a.original,
                       obj_order = a.obj_order,
                       obj_group = a.obj_group,
                       idkey = a.idkey,
                       parent = a.parent,
                       query1 = a.query1,
                       query2 = a.query2,
                       ID_empresa = a.ID_empresa
                   }).OrderBy(c=>c.idkey).Skip(6).Take(34).ToList();

                    //AGREGAMOS LOS PRODUCTOS
                    List<FormsM_detailsDemos> listaIngreso = new List<FormsM_detailsDemos>();

                    foreach (var item in TagIds)
                    {
                        try
                        {

                            var productinfo = (CMKdb.OITM.Where(x => x.ItemCode == item)).FirstOrDefault();

                            

                            FormsM_detailsDemos detalle_nuevo = new FormsM_detailsDemos();


                            detalle_nuevo.ID_formresourcetype = 3;
                            detalle_nuevo.fsource = productinfo.ItemCode;
                            detalle_nuevo.fdescription = productinfo.ItemName;
                            detalle_nuevo.fvalue = 0;
                            detalle_nuevo.fvalueDecimal = 0;
                            detalle_nuevo.fvalueText = "";
                            detalle_nuevo.ID_formM = Convert.ToInt32(ID_form);

                            detalle_nuevo.ID_visit = nuevaActividad.ID_demo;
                            detalle_nuevo.original = false;
                            //Colocamos numero de orden
                            detalle_nuevo.obj_order = nuevacuenta;
                            //Colocamos grupo si tiene
                            detalle_nuevo.obj_group = 0;
                            //Colocamos ID generado por editor
                            detalle_nuevo.idkey = nuevacuenta;
                            detalle_nuevo.query1 = "";
                            detalle_nuevo.query2 = "";
                            detalle_nuevo.parent = 0;
                            detalle_nuevo.ID_empresa = 2;

                            listaIngreso.Add(detalle_nuevo);

                            //nuevacuenta++;

                            foreach (var det in listadetalle) {
                                FormsM_detailsDemos detalle_producto = new FormsM_detailsDemos();

                                detalle_producto = JsonConvert.DeserializeObject<FormsM_detailsDemos>(JsonConvert.SerializeObject(det));


                                detalle_producto.fvalueText = productinfo.ItemCode;
                                detalle_producto.obj_order = det.obj_order + nuevacuenta;
                                detalle_producto.idkey = det.idkey + nuevacuenta;
                                detalle_producto.original = false;
                                detalle_producto.ID_visit = nuevaActividad.ID_demo;
                                if (det.parent > 0) {
                                    if (det.ID_formresourcetype == 8 || det.ID_formresourcetype == 18 || det.ID_formresourcetype == 6 || det.ID_formresourcetype == 5)
                                    {
                                        detalle_producto.parent = nuevacuenta;
                                    }
                                    else {
                                        detalle_producto.parent = detalle_producto.parent + nuevacuenta;
                                    }
                                    
                                }
                                listaIngreso.Add(detalle_producto);
                            }

                            nuevacuenta += 100;

                           // db.SaveChanges();


                        }
                        catch (Exception ex)
                        {
                            var error = ex.Message;

                        }

                        

                    }
                    //Fotos primero

                    var listadetalleFotos = (from a in db.FormsM_details
                                             where (a.ID_formM == 41 && a.original == true)
                                             select new demosDetails
                                             {
                                                 ID_details = a.ID_details,
                                                 ID_formresourcetype = a.ID_formresourcetype,
                                                 fsource = a.fsource,
                                                 fdescription = a.fdescription,
                                                 fvalue = a.fvalue,
                                                 fvalueDecimal = a.fvalueDecimal,
                                                 fvalueText = a.fvalueText,
                                                 ID_formM = a.ID_formM,
                                                 ID_visit = a.ID_visit,
                                                 original = a.original,
                                                 obj_order = a.obj_order,
                                                 obj_group = a.obj_group,
                                                 idkey = a.idkey,
                                                 parent = a.parent,
                                                 query1 = a.query1,
                                                 query2 = a.query2,
                                                 ID_empresa = a.ID_empresa
                                             }).OrderBy(c => c.idkey).Skip(40).Take(5).ToList();

                    foreach (var detfoto in listadetalleFotos)
                    {
                        FormsM_detailsDemos detalle_productofoto = new FormsM_detailsDemos();

                        detalle_productofoto = JsonConvert.DeserializeObject<FormsM_detailsDemos>(JsonConvert.SerializeObject(detfoto));


                        detalle_productofoto.fvalueText = "";
                        detalle_productofoto.obj_order = nuevacuenta + 1;
                        detalle_productofoto.idkey = nuevacuenta + 1;
                        detalle_productofoto.original = false;
                        detalle_productofoto.ID_visit = nuevaActividad.ID_demo;
                        detalle_productofoto.parent = 0;

                        listaIngreso.Add(detalle_productofoto);

                        nuevacuenta+=1;
                    }

                    db.BulkInsert(listaIngreso);

                    //AGREGAMOS FOOTER



                    try
                    {

                        FormsM_detailsDemos detalle_nuevoFooter = new FormsM_detailsDemos();


                        detalle_nuevoFooter.ID_formresourcetype = 9;
                        detalle_nuevoFooter.fsource = "";
                        detalle_nuevoFooter.fdescription = "FIRMA";
                        detalle_nuevoFooter.fvalue = 0;
                        detalle_nuevoFooter.fvalueDecimal = 0;
                        detalle_nuevoFooter.fvalueText = "";
                        detalle_nuevoFooter.ID_formM = Convert.ToInt32(ID_form);

                        detalle_nuevoFooter.ID_visit = nuevaActividad.ID_demo;
                        detalle_nuevoFooter.original = false;
                        //Colocamos numero de orden
                        detalle_nuevoFooter.obj_order = nuevacuenta +7;
                        //Colocamos grupo si tiene
                        detalle_nuevoFooter.obj_group = 0;
                        //Colocamos ID generado por editor
                        detalle_nuevoFooter.idkey = nuevacuenta + 7;
                        detalle_nuevoFooter.query1 = "";
                        detalle_nuevoFooter.query2 = "";
                        detalle_nuevoFooter.parent = 0;
                        detalle_nuevoFooter.ID_empresa = 2;



                        db.FormsM_detailsDemos.Add(detalle_nuevoFooter);
                        db.SaveChanges();
                        nuevacuenta++;
                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message;

                    }

                
            }
                else {
                    var sql = @"usp_CreateFormDetailDemo @IDVisit, @IDForm";
                    db.Database.ExecuteSqlCommand(sql,
                        new SqlParameter("@IDVisit", nuevaActividad.ID_demo),
                        new SqlParameter("@IDForm", IDForm));

                    var countItems = (from a in db.FormsM_detailsDemos where (a.ID_visit == nuevaActividad.ID_demo) select a).Count();
                    var nuevacuenta = countItems + 2;
                    List<string> TagIds = products_listSel.Split(',').ToList();
                    //AGREGAMOS LOS PRODUCTOS
                    foreach (var item in TagIds)
                    {
                        try
                        {

                            var productinfo = (CMKdb.OITM.Where(x => x.ItemCode == item)).FirstOrDefault();


                            FormsM_detailsDemos detalle_nuevo = new FormsM_detailsDemos();


                            detalle_nuevo.ID_formresourcetype = 3;
                            detalle_nuevo.fsource = productinfo.ItemCode;
                            detalle_nuevo.fdescription = productinfo.ItemName;
                            detalle_nuevo.fvalue = 0;
                            detalle_nuevo.fvalueDecimal = 0;
                            detalle_nuevo.fvalueText = "";
                            detalle_nuevo.ID_formM = Convert.ToInt32(ID_form);

                            detalle_nuevo.ID_visit = nuevaActividad.ID_demo;
                            detalle_nuevo.original = false;
                            //Colocamos numero de orden
                            detalle_nuevo.obj_order = nuevacuenta;
                            //Colocamos grupo si tiene
                            detalle_nuevo.obj_group = 0;
                            //Colocamos ID generado por editor
                            detalle_nuevo.idkey = nuevacuenta;
                            detalle_nuevo.query1 = "";
                            detalle_nuevo.query2 = "";
                            detalle_nuevo.parent = 0;
                            detalle_nuevo.ID_empresa = 2;



                            db.FormsM_detailsDemos.Add(detalle_nuevo);
                            db.SaveChanges();
                            nuevacuenta++;
                        }
                        catch (Exception ex)
                        {
                            var error = ex.Message;

                        }

                    }
                }


              



                //enviamoS correo SI ES UN USUARIO DEMO
                if (esDemoUser == 1)
                {
                    //Obtenemos el nombre de las marcas o brands por cada articulo
                    var listadeItems = (from d in db.FormsM_detailsDemos where (d.ID_visit == nuevaActividad.ID_demo && d.ID_formresourcetype == 3) select d).ToList();


                    foreach (var itemd in listadeItems)
                    {
                        itemd.fdescription = (from k in CMKdb.OITM join j in CMKdb.OMRC on k.FirmCode equals j.FirmCode where (k.ItemCode == itemd.fsource) select j.FirmName).FirstOrDefault();
                    }

                    var brands = listadeItems.GroupBy(test => test.fdescription).Select(grp => grp.First()).ToList();

                    var brandstoshow = "";
                    int count = 0;
                    foreach (var items in brands)
                    {
                        if (count == 0)
                        {
                            try
                            {
                                brandstoshow = items.fdescription.ToString();
                            }
                            catch
                            {
                                brandstoshow = "";
                            }

                        }
                        else
                        {
                            try
                            {
                                brandstoshow += ", " + items.fdescription.ToString();
                            }
                            catch
                            {
                                brandstoshow = "";
                            }
                        }
                        count += 1;
                    }
                    //*******************************
                    try
                    {

                        //Enviamos correo para notificar
                        dynamic email = new Email("newdemo_alert");
                        email.To = usuario.E_Mail.ToString();
                        email.From = "donotreply@comerciamarketing.com";
                        email.Vendor = brandstoshow;
                        email.Date = Convert.ToDateTime(nuevaActividad.visit_date).ToLongDateString();
                        email.Time = Convert.ToDateTime(nuevaActividad.visit_date).ToLongTimeString();
                        email.Place = nuevaActividad.store + ", " + nuevaActividad.address + ", " + nuevaActividad.city + ", " + nuevaActividad.state;
                        //email.link = "https://comerciamarketing.com/Home/Internal" + demos.ID_demo + Server.HtmlDecode("&") + "id_form=" + demos.ID_form;
                        email.link = "https://comerciamarketing.com/Home/Internal";
                        email.accesscode = "ACCMK00" + nuevaActividad.ID_demo.ToString();
                        email.enddate = Convert.ToDateTime(nuevaActividad.visit_date).AddDays(1).ToLongDateString();
                        email.Send();

                        //FIN email
                    }
                    catch
                    {

                    }
                }

                //************

                TempData["exito"] = "Demo created successfully.";
                return RedirectToAction("Demos", "Admin", null);
            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Demos", "Admin", null);
            }




        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateActivity(string ID_form, string ID_customer, string ID_visita, string ID_rep, DateTime time)
        {
            try
            {
                int IDForm = Convert.ToInt32(ID_form);
                int IDRep = 0;

                int esDemoUser = 0;//Variable para identificar si el usuario seleccionado es demo o es un reps

                try
                {
                    IDRep = Convert.ToInt32(ID_rep);
                    esDemoUser = 0;
                }
                catch {
                    esDemoUser = 1;
                }
             

                //CREAMOS LA ESTRUCTURA DE LA ACTIVIDAD
                ActivitiesM nuevaActivida = new ActivitiesM();

                nuevaActivida.ID_form = Convert.ToInt32(ID_form);
                nuevaActivida.ID_visit = Convert.ToInt32(ID_visita);
                nuevaActivida.ID_customer = ID_customer;
                nuevaActivida.Customer = "";
                var customer = (from c in CMKdb.OCRD where (c.CardCode == ID_customer) select c).FirstOrDefault();
                if (customer != null)
                {
                    nuevaActivida.Customer = customer.CardName;
                }
                nuevaActivida.comments = "";
                nuevaActivida.check_in = DateTime.Today.Date;
                nuevaActivida.check_out = DateTime.Today.Date;
                nuevaActivida.query1 = "";
                nuevaActivida.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;
                nuevaActivida.isfinished = false;
                nuevaActivida.description = "";
                nuevaActivida.ID_activitytype = 0;
                nuevaActivida.desnormalizado = false;
                nuevaActivida.date = DateTime.Today.Date;
                var form = (from c in db.FormsM where (c.ID_form == IDForm) select c).FirstOrDefault();
                if (form != null)
                {
                    nuevaActivida.description = form.name;
                    nuevaActivida.ID_activitytype = form.ID_activity;
                    nuevaActivida.query1 = "";
                    if (form.ID_activity == 4) {
                        nuevaActivida.date = time;
                    }
                }
                    

                int ID_usuario = Convert.ToInt32(Session["IDusuario"]);
                nuevaActivida.ID_usuarioCreate = ID_usuario;

                //OJO ESTA PARTE SE AGREGO PARA COMERCIA ES PROPIA DE LA EMPRESA
                if (nuevaActivida.ID_activitytype != 4)
                {
                    nuevaActivida.ID_usuarioEnd = IDRep;  //Usuario que sera asignado
                    nuevaActivida.ID_usuarioEndString = "";
                }
                else
                {
                    //Guardamos el usuario de SAP en la variable tipo String
                    if (esDemoUser == 1)
                    {
                        nuevaActivida.ID_usuarioEnd = 0;
                    }
                    else {
                        nuevaActivida.ID_usuarioEnd = IDRep;

                    }
                    nuevaActivida.ID_usuarioEndString = ID_rep;
                }

                //SOLO PARA COMERCIA
                if (nuevaActivida.ID_activitytype == 4) {

                    nuevaActivida.query1 = "start";
                }
                


                //guardamos
                db.ActivitiesM.Add(nuevaActivida);
                db.SaveChanges();

                //LUEGO ASIGNAMOS LA PLANTILLA DE FORMULARIO A LA ACTIVIDAD
                //Guardamos el detalle
                //var detalles_acopiar = (from a in db.FormsM_details where (a.ID_formM == IDForm && a.original == true) select a).AsNoTracking().ToList();

                var sql = @"usp_CreateFormDetail @IDVisit, @IDForm";
                db.Database.ExecuteSqlCommand(sql,
                    new SqlParameter("@IDVisit", nuevaActivida.ID_activity),
                    new SqlParameter("@IDForm", IDForm));

                //List<FormsM_details> dtlstoinsert = new List<FormsM_details>();

                //foreach (var item in detalles_acopiar)
                //{
                //    FormsM_details nuevodetalle = new FormsM_details();
                //    nuevodetalle = item;
                //    nuevodetalle.original = false;
                //    nuevodetalle.ID_visit = nuevaActivida.ID_activity; //TOMAREMOS ID VISIT COMO ID ACTIVITY PORQUE ES POR REPRESENTANTE Y NO POR VISITA

                //    dtlstoinsert.Add(nuevodetalle);
                    
                //}
                //db.BulkInsert(dtlstoinsert);

                //Por ultimo asignamos el usuario a la visita
                //Pero verificamos si ya existe

                var existeenvisita = (from v in db.VisitsM_representatives where (v.ID_visit == nuevaActivida.ID_visit && v.ID_usuario == IDRep) select v).Count();


                if (existeenvisita > 0)

                {
                }
                else
                {

                    if (esDemoUser == 1)

                    {
                        VisitsM_representatives repvisita = new VisitsM_representatives();

                        repvisita.ID_visit = nuevaActivida.ID_visit;
                        repvisita.ID_usuario = 0;
                        repvisita.query1 = nuevaActivida.ID_usuarioEndString;
                        repvisita.ID_empresa = nuevaActivida.ID_empresa;
                        db.VisitsM_representatives.Add(repvisita);
                        db.SaveChanges();
                    }
                    else {
                        VisitsM_representatives repvisita = new VisitsM_representatives();

                        repvisita.ID_visit = nuevaActivida.ID_visit;
                        repvisita.ID_usuario = IDRep;
                        repvisita.query1 = "3";
                        repvisita.ID_empresa = nuevaActivida.ID_empresa;
                        db.VisitsM_representatives.Add(repvisita);
                        db.SaveChanges();

                    }

                    }

                //enviamoS correo SI ES UN USUARIO DEMO
                if (esDemoUser == 1) {
                    //Obtenemos el nombre de las marcas o brands por cada articulo
                    var listadeItems = (from d in db.FormsM_details where (d.ID_visit == nuevaActivida.ID_activity && d.ID_formresourcetype == 3) select d).ToList();


                    foreach (var itemd in listadeItems)
                    {
                        itemd.fdescription = (from k in CMKdb.OITM join j in CMKdb.OMRC on k.FirmCode equals j.FirmCode where (k.ItemCode == itemd.fsource) select j.FirmName).FirstOrDefault();
                    }

                    var brands = listadeItems.GroupBy(test => test.fdescription).Select(grp => grp.First()).ToList();

                    var brandstoshow = "";
                    int count = 0;
                    foreach (var items in brands)
                    {
                        if (count == 0)
                        {
                            try
                            {
                                brandstoshow = items.fdescription.ToString();
                            }
                            catch {
                                brandstoshow = "no data from db";
                            }
                            
                        }
                        else
                        {                           
                            try
                            {
                                brandstoshow += ", " + items.fdescription.ToString();
                            }
                            catch
                            {
                                brandstoshow = "no data from db";
                            }
                        }
                        count += 1;
                    }
                    //*******************************
                    try
                    {
                        var usuario = (from a in CMKdb.OCRD where (a.CardCode == nuevaActivida.ID_usuarioEndString) select a).FirstOrDefault();

                        var store = (from s in db.VisitsM where (s.ID_visit == nuevaActivida.ID_visit) select s).FirstOrDefault();

                        //Enviamos correo para notificar
                        dynamic email = new Email("newdemo_alert");
                        email.To = usuario.E_Mail.ToString();
                        email.From = "customercare@comerciamarketing.com";
                        email.Vendor = brandstoshow;
                        email.Date = Convert.ToDateTime(nuevaActivida.date).ToLongDateString();
                        email.Time = Convert.ToDateTime(nuevaActivida.date).ToLongTimeString();
                        email.Place = store.store + ", " + store.address;
                        //email.link = "https://comerciamarketing.com/Home/Internal" + demos.ID_demo + Server.HtmlDecode("&") + "id_form=" + demos.ID_form;
                        email.link = "https://comerciamarketing.com/Home/Internal";
                        email.accesscode = "ACCMK00" + nuevaActivida.ID_activity.ToString();
                        email.enddate = Convert.ToDateTime(nuevaActivida.date).AddDays(1).ToLongDateString();
                        email.Send();

                        //FIN email
                    }
                    catch
                    {

                    }
                }

                //************

                TempData["exito"] = "Activity created successfully.";
                return RedirectToAction("Visit_details", "Admin", new { id= ID_visita});
            }
            catch (Exception ex){
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Visit_details", "Admin", new { id = ID_visita });
            }
         
            


        }
        public ActionResult copyActivityOffline(string ID_activityCopy, string ID_visitCopy)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                int IDActivity = Convert.ToInt32(ID_activityCopy);
                int IDVisit = Convert.ToInt32(ID_visitCopy);
                try
                {
                    //CREAMOS LA ESTRUCTURA DE LA ACTIVIDAD
                    ActivitiesM nuevaActivida = new ActivitiesM();

                    nuevaActivida = (from a in db.ActivitiesM where (a.ID_activity == IDActivity && a.ID_visit == IDVisit) select a).FirstOrDefault();

                    if (nuevaActivida == null)
                    {

                        var result = new { Result = "Something wrong happened, try again.", activity = "", details_activity = "" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        DateTime ndate = DateTime.Today.Date;
                        DateTime ndatetime = DateTime.Now;
                        //COPIAMOS Y GUARDAMOS LA NUEVA ACTIVIDAD
                        Random r = new Random();
                        int card = r.Next(172);
                        nuevaActivida.comments = "";
                        nuevaActivida.check_in = ndate;
                        nuevaActivida.check_out = ndate;
                        nuevaActivida.isfinished = false;

                        int ID_usuario = Convert.ToInt32(Session["IDusuario"]);
                        nuevaActivida.ID_usuarioCreate = ID_usuario;
                        nuevaActivida.date = DateTime.Today.Date;
                        nuevaActivida.query1 = "copy_" + card + ndatetime.Hour.ToString() + ndatetime.Minute.ToString() + "_act_" + IDActivity;

                        db.ActivitiesM.Add(nuevaActivida);
                        db.SaveChanges();

                        //LUEGO ASIGNAMOS LA PLANTILLA DE FORMULARIO A LA ACTIVIDAD
                        //Guardamos el detalle
                        var detalles_acopiar = (from a in db.FormsM_details where (a.ID_formM == nuevaActivida.ID_form && a.original == true) select a).ToList();
                        
                        foreach (var item in detalles_acopiar)
                        {
                            FormsM_details nuevodetalle = new FormsM_details();
                            nuevodetalle = item;
                            nuevodetalle.original = false;
                            nuevodetalle.query2 = "copy_" + card + ndatetime.Hour.ToString() + ndatetime.Minute.ToString() + "_act_" + IDActivity;
                            nuevodetalle.ID_visit = nuevaActivida.ID_activity; //TOMAREMOS ID VISIT COMO ID ACTIVITY PORQUE ES POR REPRESENTANTE Y NO POR VISITA
                            

                            db.FormsM_details.Add(nuevodetalle);
                            db.SaveChanges();
                        }
                        //Luego de crear la actividad, la retornamos
                        List<ActivitiesM_offline> nlst = new List<ActivitiesM_offline>();

                        var usuario = datosUsuario.nombre + " " + datosUsuario.apellido;      

                        ActivitiesM_offline nuevaoffline = new ActivitiesM_offline();

                        nuevaoffline.ID_activity = nuevaActivida.ID_activity;
                        nuevaoffline.ID_visit = nuevaActivida.ID_visit;
                        nuevaoffline.ID_form = nuevaActivida.ID_form;
                        nuevaoffline.ID_customer = nuevaActivida.ID_customer;
                        nuevaoffline.usuarioasignado = usuario;  //Asignamos el nombre de usuario aca
                        nuevaoffline.Customer = nuevaActivida.Customer;
                        nuevaoffline.comments = nuevaActivida.comments;
                        nuevaoffline.check_in = nuevaActivida.check_in;
                        nuevaoffline.check_out = nuevaActivida.check_out;
                        nuevaoffline.query1 = "copy_" + card + ndatetime.Hour.ToString() + ndatetime.Minute.ToString() + "_act_" + IDActivity;
                        nuevaoffline.ID_empresa = nuevaActivida.ID_empresa;
                        nuevaoffline.isfinished = nuevaActivida.isfinished;
                        nuevaoffline.description = nuevaActivida.description;
                        nuevaoffline.ID_usuarioCreate = nuevaActivida.ID_usuarioCreate;
                        nuevaoffline.ID_usuarioEnd = nuevaActivida.ID_usuarioEnd;
                        nuevaoffline.date = nuevaActivida.date;
                        nuevaoffline.ID_activitytype = nuevaActivida.ID_activitytype;
                        nuevaoffline.ID_usuarioEndString = nuevaActivida.ID_usuarioEndString;
                        nuevaoffline.onserver = false;
                        nuevaoffline.action = "saved";
                        nuevaoffline.isnew = true;
                        nlst.Add(nuevaoffline);
                        //////
                        int idvisit = Convert.ToInt32(ID_visitCopy);
                        //Retornamos el detalle
                        List<MyObj_tablapadre> listapadresActivities = (from item in detalles_acopiar
                                                                        where (item.parent == 0)
                                                                        select
                                                                           new MyObj_tablapadre
                                                                           {
                                                                               ID_details = item.ID_details,
                                                                               id_resource = item.ID_formresourcetype,
                                                                               fsource = item.fsource,
                                                                               fdescription = item.fdescription,
                                                                               fvalue = item.fvalue,
                                                                               fvalueDecimal = item.fvalueDecimal,
                                                                               fvalueText = item.fvalueText,
                                                                               ID_formM = item.ID_formM,
                                                                               ID_visit = item.ID_visit,
                                                                               original = item.original,
                                                                               obj_order = item.obj_order,
                                                                               obj_group = item.obj_group,
                                                                               idkey = item.idkey,
                                                                               parent = item.parent,
                                                                               query1 = item.query1,
                                                                               query2 = "copy_" + card + ndatetime.Hour.ToString() + ndatetime.Minute.ToString() + "_act_" + IDActivity,
                                                                               ID_empresa = item.ID_empresa,
                                                                               onserver = false,
                                                                               action = "saved",
                                                                               ID_visitOriginal = idvisit,
                                                                               resourcetype = item.ID_formresourcetype,
                                                                               isnew = false

                                                                           }
                                          ).ToList();



                        List<tablahijospadre> listahijasActivities = (from item in detalles_acopiar
                                                                     
                                                                      select new tablahijospadre
                                                                      {
                                                                          ID_details = item.ID_details,
                                                                          id_resource = item.ID_formresourcetype,
                                                                          fsource = item.fsource,
                                                                          fdescription = item.fdescription,
                                                                          fvalue = item.fvalue,
                                                                          fvalueDecimal = item.fvalueDecimal,
                                                                          fvalueText = item.fvalueText,
                                                                          ID_formM = item.ID_formM,
                                                                          ID_visit = item.ID_visit,
                                                                          original = item.original,
                                                                          obj_order = item.obj_order,
                                                                          obj_group = item.obj_group,
                                                                          idkey = item.idkey,
                                                                          parent = item.parent,
                                                                          query1 = item.query1,
                                                                          query2 = "copy_" + card + ndatetime.Hour.ToString() + ndatetime.Minute.ToString() + "_act_" + IDActivity,
                                                                          ID_empresa = item.ID_empresa,
                                                                          onserver = false,
                                                                          action = "saved",
                                                                          ID_visitOriginal =idvisit,
                                                                          resourcetype = item.ID_formresourcetype,
                                                                          isnew = false

                                                                      }).ToList();


                        List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);
                        ///FIN
                        ///

                        //JavaScriptSerializer jsini = new JavaScriptSerializer();
                        //jsini.MaxJsonLength = Int32.MaxValue;
                      
                        ///////FIN

                        var result = new { Result = "Activity created successfully", activity = nlst, details_activity= categoriasListActivities };
                        return Json(result, JsonRequestBehavior.AllowGet);
                     

                    }

                }
                catch (Exception ex)
                {

                    var result = new { Result = "Something wrong happened, try again. " + ex.Message, activity = "", details_activity = "" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }




        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult copyActivity(string ID_activityCopy, string ID_visitCopy)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                int IDActivity = Convert.ToInt32(ID_activityCopy);
                int IDVisit = Convert.ToInt32(ID_visitCopy);
                try
                {
                    //CREAMOS LA ESTRUCTURA DE LA ACTIVIDAD
                    ActivitiesM nuevaActivida = new ActivitiesM();

                    nuevaActivida = (from a in db.ActivitiesM where (a.ID_activity == IDActivity && a.ID_visit == IDVisit) select a).FirstOrDefault();

                    if (nuevaActivida == null)
                    {
                        if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)//Administrador
                        {
                            TempData["advertencia"] = "Something wrong happened, try again.";
                            return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = IDVisit });
                        }
                        else {
                            TempData["advertencia"] = "Something wrong happened, try again.";
                            return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = IDVisit });
                        }

                    }
                    else
                    {
                        //COPIAMOS Y GUARDAMOS LA NUEVA ACTIVIDAD
                        if (nuevaActivida.ID_activitytype == 3) {//Si es SALES ORDER

                            nuevaActivida.Customer = "";
                            nuevaActivida.ID_customer = "";
                        }
                        nuevaActivida.comments = "";
                        nuevaActivida.check_in = DateTime.Today.Date;
                        nuevaActivida.check_out = DateTime.Today.Date;
                        nuevaActivida.isfinished = false;

                        int ID_usuario = Convert.ToInt32(Session["IDusuario"]);
                        nuevaActivida.ID_usuarioCreate = ID_usuario;
                        nuevaActivida.date = DateTime.Today.Date;

                        db.ActivitiesM.Add(nuevaActivida);
                        db.SaveChanges();

                        //LUEGO ASIGNAMOS LA PLANTILLA DE FORMULARIO A LA ACTIVIDAD
                        //Guardamos el detalle
                        var sql = @"usp_CreateFormDetail @IDVisit, @IDForm";
                        db.Database.ExecuteSqlCommand(sql,
                            new SqlParameter("@IDVisit", nuevaActivida.ID_activity),
                            new SqlParameter("@IDForm", nuevaActivida.ID_form));


                        //var detalles_acopiar = (from a in db.FormsM_details where (a.ID_formM == nuevaActivida.ID_form && a.original == true) select a).AsNoTracking().ToList();
                        //List<FormsM_details> dtlstoinsert = new List<FormsM_details>();
                        //foreach (var item in detalles_acopiar)
                        //{
                        //    FormsM_details nuevodetalle = new FormsM_details();
                        //    nuevodetalle = item;
                        //    nuevodetalle.original = false;
                        //    nuevodetalle.ID_visit = nuevaActivida.ID_activity; //TOMAREMOS ID VISIT COMO ID ACTIVITY PORQUE ES POR REPRESENTANTE Y NO POR VISITA

                        //    //db.FormsM_details.Add(nuevodetalle);
                        //    //db.SaveChanges();
                        //    dtlstoinsert.Add(nuevodetalle);
                        //}
                        //db.BulkInsert(dtlstoinsert);
                        if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)//Administrador
                        {
                            TempData["exito"] = "Activity created successfully.";
                            return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = IDVisit });
                        }
                        else
                        {
                            TempData["exito"] = "Activity created successfully.";
                            return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = IDVisit });
                        }
                    }

                }
                catch (Exception ex)
                {
                    
                    if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)//Administrador
                    {
                        TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                        return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = IDVisit });
                    }
                    else
                    {
                        TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                        return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = IDVisit });
                    }
                }


            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }
            



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteActivity(int ID_activityD, int ID_visitA)
        {
            try
            {

                ActivitiesM activity = db.ActivitiesM.Find(ID_activityD);
                db.ActivitiesM.Remove(activity);
                db.SaveChanges();
                //Eliminamos detalle
                var sql = @"usp_DeleteFormDetail @IDVisit";
                db.Database.ExecuteSqlCommand(sql,
                    new SqlParameter("@IDVisit", ID_activityD));



                ////Eliminamos el detalle que genero la actividad en FormsM_details
                //var lista_eliminar = (from c in db.FormsM_details where (c.ID_visit ==ID_activityD && c.original == false) select c).ToList();
                //db.BulkDelete(lista_eliminar);

                TempData["exito"] = "Activity deleted successfully.";
                return RedirectToAction("Visit_details", "Admin", new { id = ID_visitA });

            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Visit_details", "Admin", new { id = ID_visitA });
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDemo(int ID_activityD)
        {
            try {

                //Eliminamos detalle
                var sql = @"usp_DeleteFormDetailDemo @IDVisit";
                db.Database.ExecuteSqlCommand(sql, new SqlParameter("@IDVisit", ID_activityD));


                Demos activity = db.Demos.Find(ID_activityD);
                db.Demos.Remove(activity);
                db.SaveChanges();

                TempData["exito"] = "Activity deleted successfully.";
                return RedirectToAction("Demos", "Admin", null);

            }
            catch (Exception ex){
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Demos", "Admin",null);
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTask(int ID_activityD)
        {
            try
            {

                //Eliminamos detalle
                var sql = @"usp_DeleteFormDetailTasks @IDVisit";
                db.Database.ExecuteSqlCommand(sql, new SqlParameter("@IDVisit", ID_activityD));


                Tasks activity = db.Tasks.Find(ID_activityD);
                db.Tasks.Remove(activity);
                db.SaveChanges();

                TempData["exito"] = "Task deleted successfully.";
                return RedirectToAction("Tasks", "Admin", null);

            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Tasks", "Admin", null);
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelActivity(int ID_activityCa, int ID_visitCa, string comment)
        {
            try
            {

                ActivitiesM activity = db.ActivitiesM.Find(ID_activityCa);

                activity.isfinished = true;
                activity.query1 = "cancel";
                activity.comments = comment;

                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
           

                TempData["exito"] = "Activity canceled successfully.";
                return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = ID_visitCa });

            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = ID_visitCa });
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRepfromActivity(int ID_repUD, int ID_visitU)
        {
            try
            {

                VisitsM_representatives repfromAct = (from b in db.VisitsM_representatives where (b.ID_usuario == ID_repUD && b.ID_visit == ID_visitU) select b).FirstOrDefault() ;
                db.VisitsM_representatives.Remove(repfromAct);
                db.SaveChanges();

               
                TempData["exito"] = "Representative removed successfully.";
                return RedirectToAction("Visit_details", "Admin", new { id = ID_visitU });

            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Visit_details", "Admin", new { id = ID_visitU });
            }


        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GalleryC(string id, string modulo, string IDcustomer, string brand)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;
                List<FormsM_details> detalles = new List<FormsM_details>();
                int IDV = Convert.ToInt32(id);

                try
                {
                    var actividadesList = (from e in db.ActivitiesM where (e.ID_visit == IDV && e.ID_customer == IDcustomer) select e).ToList();
                    var actividades = (from a in actividadesList select a.ID_activity).ToArray();

                    var detallesGlobales = (from b in db.FormsM_details where (actividades.Contains(b.ID_visit)) select b).ToList();
                    if (brand == null || brand == "" || brand == "0")
                    {
                     detalles = (from b in detallesGlobales where (b.ID_formresourcetype == 5) select b).ToList();
                    }
                    else {
                        var detallesGlobalesArr = (from b in detallesGlobales where (b.ID_formresourcetype == 13 && b.fvalueText.StartsWith(brand) && b.fvalueText.EndsWith(brand)) select b.ID_visit).ToArray();
                        detalles = (from b in detallesGlobales where (b.ID_formresourcetype == 5 && detallesGlobalesArr.Contains(b.ID_visit)) select b).ToList();
                    }



                    foreach (var item in detalles)
                    {
                        var f = (from c in actividadesList where (c.ID_activity == item.ID_visit) select c).FirstOrDefault();
                        var bra = (from br in detallesGlobales where (br.ID_formresourcetype == 13 && br.ID_visit == item.ID_visit) select br).FirstOrDefault();

                        var brandn = "";
                        if(bra != null){ brandn = bra.fdescription; }

                        item.fvalueText = f.Customer;
                        item.fdescription = f.description;
                        item.query1 = brandn;

                    }

                    ViewBag.branddef = brand;
                    ViewBag.imagenes = detalles;
                    ViewBag.id_visita = id;
                    ViewBag.customerID = IDcustomer;
                    return View();

                }
                catch (Exception ex)
                {
                    if (modulo == "visits")
                    {
                        TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                        return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = IDV });
                    }
                    else
                    {
                        TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                        return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = IDV, ID_customer = IDcustomer });
                    }

                }
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        public class brands
        {
            public string FirmCode { get; set; }
            public string FirmName { get; set; }
            public string Customer { get; set; }
            public Boolean isselected { get; set; }
        }

        public class ActForm
        {
            public int id { get; set; }
            public string text { get; set; }

        }

        [HttpPost]
        public ActionResult DownloadZip(string id, string IDcustomer, string brands, string activityform) {
            List<FormsM_details> detalles = new List<FormsM_details>();
            detalles =(List<FormsM_details>) Session["imagesDown"];
            List<FileModel> files = new List<FileModel>();
            foreach (var item in detalles)
            {
                files.Add(new FileModel()
                {
                    id = item.ID_details,
                    FileName = Path.GetFileName(item.fsource),
                    FileActivity = item.fdescription,
                    FilePath = item.fsource,
                    FileCustomer = item.fvalueText,
                    FileBrand = item.query1
                });
            }
           
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                zip.AddDirectoryByName("Files");
                foreach (FileModel file in files)
                {
                    //if (file.IsSelected)
                    //{
                    //    zip.AddFile(file.FilePath, "Files");
                    //}                    
                    if (System.IO.File.Exists(Server.MapPath(file.FilePath)))
                    {
                        try
                        {
                            zip.AddFile(Server.MapPath(file.FilePath), "Files");
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);

                        }
                    }
                    
                    
                }
                string zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd"));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    zip.Save(memoryStream);
                    return File(memoryStream.ToArray(), "application/zip", zipName);
                }
            }



        }

        public ActionResult GalleryG(string id, string IDcustomer, string brand, string activityform, bool showimg, string rep, string section)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;
                List<FormsM_details> detalles = new List<FormsM_details>();
                List<ActivitiesM> actividadesList = new List<ActivitiesM>();
                int IDV = Convert.ToInt32(id);
                int IDF = Convert.ToInt32(activityform);
                List<VisitsM> newlstvisits = new List<VisitsM>();

                    newlstvisits = (List<VisitsM>)Session["visitstemp"];
                

            
                
                var visitsArr = (from c in newlstvisits select c.ID_visit).ToArray();
                    try

                    {
                    string[] subtitles = new string[] { "BEFORE", "AFTER", "CONDICION INICIAL EN LA TIENDA (ANTES)", "ESPACIOS E INVENTARIOS", "ANALISIS DE LA COMPETENCIA", "CONDICION FINAL DE LA TIENDA (DESPUES)" };

                    if (id == null || showimg == false)
                        {

                        }
                        else
                        {
                            if ((id == null || id == "" || id == "0") && (activityform == null || activityform == "" || activityform == "0"))
                            {
                                actividadesList = (from e in db.ActivitiesM where (e.ID_customer == IDcustomer && visitsArr.Contains(e.ID_visit)) select e).ToList();
                            }
                            else if ((id != null || id != "" || id != "0") && (activityform == null || activityform == "" || activityform == "0"))
                            {
                                actividadesList = (from e in db.ActivitiesM where (e.ID_visit == IDV && e.ID_customer == IDcustomer) select e).ToList();
                            }
                            else if ((id == null || id == "" || id == "0") && (activityform != null || activityform != "" || activityform != "0"))
                            {
                                actividadesList = (from e in db.ActivitiesM where (visitsArr.Contains(e.ID_visit) && e.ID_customer == IDcustomer && e.ID_form == IDF) select e).ToList();

                            }
                            else if ((id != null || id != "" || id != "0") && (activityform != null || activityform != "" || activityform != "0"))
                            {
                                actividadesList = (from e in db.ActivitiesM where (e.ID_visit == IDV && e.ID_customer == IDcustomer && e.ID_form == IDF) select e).ToList();

                            }


                            var actividades = (from a in actividadesList select a.ID_activity).ToArray();

                            var detallesGlobales = (from b in db.FormsM_details where (actividades.Contains(b.ID_visit)) select b).ToList();



                            if (brand == null || brand == "" || brand == "0")
                            {
                                detalles = (from b in detallesGlobales where ((b.ID_formresourcetype == 5 || b.ID_formresourcetype == 8) && b.fsource != "") select b).ToList();
                            }
                            else
                            {
                                var detallesGlobalesArr = (from b in detallesGlobales where (b.ID_formresourcetype == 13 && b.fvalueText.StartsWith(brand) && b.fvalueText.EndsWith(brand)) select b.ID_visit).ToArray();
                                detalles = (from b in detallesGlobales where ((b.ID_formresourcetype == 5 || b.ID_formresourcetype == 8) && detallesGlobalesArr.Contains(b.ID_visit) && b.fsource != "") select b).ToList();
                            }

                        var cleardet = (from a in detalles where (!subtitles.Contains(a.fsource) && a.ID_formresourcetype==8) select a.ID_details).ToArray();
                        detalles = (from c in detalles where (!cleardet.Contains(c.ID_details)) select c).ToList();
                        var count = 0;
                            foreach (var item in detalles)
                            {
                            
                            if (item.ID_formresourcetype == 5) {
                                var f = (from c in actividadesList where (c.ID_activity == item.ID_visit) select c).FirstOrDefault();
                                var bra = (from br in detallesGlobales where (br.ID_formresourcetype == 13 && br.ID_visit == item.ID_visit) select br).FirstOrDefault();

                                var brandn = "";
                                if (bra != null) { brandn = bra.fdescription; }

                                item.fvalueText = f.Customer;
                                item.fdescription = f.ID_activity + " - " + f.description;
                                item.query1 = brandn;

                                try
                                {
                                    for (int i = count - 1; i >= 0; i--)
                                    {
                                        var current = detalles[i];
                                        //Do things
                                        if (current.ID_formresourcetype == 8) {
                                            item.query2 = current.fsource;
                                            break;
                                        }
                                        
                                    }
                                }
                                catch { }

                            }

                            count++;
                        }
                        
                    }

                    var lstContainers = detalles
                    .Where(i => i.ID_formresourcetype == 8 && subtitles.Contains(i.fsource)).GroupBy(p => p.fsource)
                    .Select(i => i.FirstOrDefault())

                    .ToList();


                    var lstbrands = CMKdb.view_CMKEditorB
.Where(i => i.U_CustomerCM == IDcustomer)
.Select(i => new brands { FirmCode = i.FirmCode.ToString(), FirmName = i.FirmName, isselected = false, Customer = "" })
.Distinct()
.OrderByDescending(i => i.FirmName)
.ToList();
            
                    //ViewBag.usuarios = JsonConvert.SerializeObject(myArrList);
                    ViewBag.brands = lstbrands;
                    ViewBag.containers = (from h in lstContainers select new brands { FirmCode = h.fsource, FirmName = h.fsource, isselected = false, Customer = "" });

                    var users = (from a in db.Usuarios where (a.ID_empresa == 2 && a.estados_influencia.Contains(IDcustomer) && a.ID_usuario != 59) select a).ToList();
                    ViewBag.usuarios = users;
                    //Actividades o tipo de forms

                    //var formsAct = (from f in actividadesList where(f.ID_activitytype==1) select f.ID_form).Distinct().ToArray();

                    var forms = (from form in db.FormsM where (form.ID_form==1 || form.ID_form ==3) select form).ToList();

                    ViewBag.lstvisits = newlstvisits;
                    ViewBag.formsAct = forms;
                    ViewBag.branddef = brand;
                    ViewBag.imagenes = detalles;
                    ViewBag.repdef = rep;
                    ViewBag.sectiondef = section;
                    ViewBag.id_visita = id;
                    ViewBag.customerID = IDcustomer;
                    ViewBag.activityFormdef = activityform;



                    List<FileModel> files = new List<FileModel>();
                    foreach (var item in detalles)
                    {
                        if(item.ID_formresourcetype == 5) { 
                        var actss = (from b in actividadesList where (b.ID_activity == item.ID_visit) select b).FirstOrDefault();
                            var stlst = (from d in newlstvisits where (d.ID_visit == actss.ID_visit) select d).FirstOrDefault();

                        var repname = "";
                            var idrep = "";
                            var store = "";
                        try {
                            var usu = (from a in db.Usuarios where (a.ID_usuario == actss.ID_usuarioEnd) select a).FirstOrDefault();
                            if (usu != null) {
                                repname = usu.nombre + " " + usu.apellido;
                                    idrep = usu.ID_usuario.ToString();
                            }

                                if (stlst != null) {

                                    store = stlst.store;
                                }

                                } catch { }

                        files.Add(new FileModel()
                        {
                            id = item.ID_details,
                            FileName = Path.GetFileName(item.fsource),
                            FileActivity = item.fdescription,
                            FilePath = item.fsource,
                            FileCustomer = item.fvalueText,
                            FileBrand = item.query1,
                            FileRep = repname,
                            FileSection = item.query2,
                            FileStore = store,
                            FileIDREP = idrep
                        });
                        }
                    }

                    if (rep != "0" && rep != null && rep !="") {
                        files = (from a in files where (a.FileIDREP == rep) select a).ToList();
                    }
                    if (section != "0" && section != null && section != "") {
                        files = (from a in files where (a.FileSection == section) select a).ToList();
                    }
                    var arrFiles = (from h in files select h.id).ToArray();

                    detalles = (from i in detalles where (arrFiles.Contains(i.ID_details)) select i).ToList();
                    Session["imagesDown"] = detalles;

                    return View(files);

                }
                catch (Exception ex)
                {

                        TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                    return RedirectToAction("Index", "Home", null);

                }
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        public ActionResult Gallery(string id, string modulo)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                int IDV = Convert.ToInt32(id);

                try
                {
                    var actividadesList = (from e in db.ActivitiesM where (e.ID_visit == IDV && e.ID_empresa == GlobalVariables.ID_EMPRESA_USUARIO) select e).ToList();
                    var actividades = (from a in db.ActivitiesM where (a.ID_visit == IDV && a.ID_empresa == GlobalVariables.ID_EMPRESA_USUARIO) select a.ID_activity).ToArray();

                    var detalles = (from b in db.FormsM_details where (actividades.Contains(b.ID_visit) && b.ID_formresourcetype == 5) select b).ToList();


                    foreach (var item in detalles) {
                        var f = (from c in actividadesList where (c.ID_activity == item.ID_visit) select c).FirstOrDefault();

                        item.fvalueText = f.Customer;
                        item.fdescription = f.description;
                        item.query1 = f.check_out.ToShortDateString();

                    }


                    ViewBag.imagenes = detalles;
                    ViewBag.id_visita = id;
                    return View();

                }
                catch (Exception ex)
                {
                    if (modulo == "visits")
                    {
                        TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                        return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = IDV });
                    }
                    else
                    {
                        TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                        return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = IDV });
                    }

                }
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        //CHECK IN
        public ActionResult Check_in(string ID_visit, string check_in, string lat, string lng)
        {
            try
            {
                int IDU = Convert.ToInt32(Session["IDusuario"]);
                int IDvisita = Convert.ToInt32(ID_visit);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == IDU) select c).FirstOrDefault();
                VisitsM visita = db.VisitsM.Find(Convert.ToInt32(ID_visit));

                DateTime check = DateTime.UtcNow;
                try //Validamos la hora ya que viene del lado del cliente y el formato no esta definido
                {
                    check = Convert.ToDateTime(check_in);
                }
                catch
                {
                    check = DateTime.UtcNow;
                }


                if (visita != null)
                {
                    //Cambiamos estado de visita global
                    visita.ID_visitstate = 2;
              
                        visita.check_in = check;
              
                    
                    db.Entry(visita).State = EntityState.Modified;
                    db.SaveChanges();

                    //Cambios estado de visita por rep
                    VisitsM_representatives rep = (from a in db.VisitsM_representatives
                               where (a.ID_visit == IDvisita && a.ID_usuario == datosUsuario.ID_usuario)
                               select a
                         ).FirstOrDefault();
                    rep.query1 = "2";
                    db.Entry(rep).State = EntityState.Modified;
                    db.SaveChanges();


                    if (lat != null || lat != "")
                    {
                        //Guardamos el log de la actividad
                        ActivitiesM_log nuevoLog = new ActivitiesM_log();
                        nuevoLog.latitude = lat;
                        nuevoLog.longitude = lng;
                        nuevoLog.ID_usuario = IDU;
                        nuevoLog.ID_activity =0;
               
                            nuevoLog.fecha_conexion = check;
                      
                 
                        nuevoLog.query1 = ID_visit;
                        nuevoLog.query2 = "";
                        nuevoLog.action = "CHECK IN  - " + visita.store;
                        nuevoLog.ip = "";
                        nuevoLog.hostname = "";
                        nuevoLog.typeh = "";
                        nuevoLog.continent_name = "";
                        nuevoLog.country_code = "";
                        nuevoLog.country_name = "";
                        nuevoLog.region_code = "";
                        nuevoLog.region_name = "";
                        nuevoLog.city = "";

                        db.ActivitiesM_log.Add(nuevoLog);
                        db.SaveChanges();
                    }

                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Fail" });
            }
            catch
            {
                return Json(new { Result = "Error" });
            }



        }

        public ActionResult getandsaveoffline(MyObj_tablapadre objects)
        {
            try
            {
                if (objects == null)
                {
                    return Json(new { Result = "Error no data" });
                }
                else
                {
                    if (objects.isnew == true)
                    {
                        if (objects.ID_visit == 201010101)
                        {
                            FormsM_details detail = new FormsM_details();

                            if (objects.fsource == null) { objects.fsource = ""; }
                            if (objects.fdescription == null) { objects.fdescription = ""; }
                            if (objects.fvalueText == null) { objects.fvalueText = ""; }
                            if (objects.query1 == null) { objects.query1 = ""; }

                            detail.fvalue = objects.fvalue;
                            detail.fsource = objects.fsource;
                            detail.fdescription = objects.fdescription;
                            detail.fvalueDecimal = objects.fvalueDecimal;
                            detail.fvalueText = objects.fvalueText;
                            detail.query1 = objects.query1;
                            detail.ID_formresourcetype = objects.resourcetype;
                            detail.ID_formM = objects.ID_formM;
                            detail.ID_visit = objects.ID_visit;
                            detail.original = objects.original;
                            detail.obj_order = objects.obj_order;
                            detail.obj_group = objects.obj_group;
                            detail.idkey = objects.idkey;
                            detail.parent = objects.parent;
                            detail.query2 = objects.query2;
                            detail.ID_empresa = objects.ID_empresa;

                            return Json(new { Result = "Success" });
                        }
                        else {
                            return Json(new { Result = "Success" });

                        }
                        
                    }
                    else {

                        int IDItem = Convert.ToInt32(objects.ID_details);
                        FormsM_details detail = (from f in db.FormsM_details where (f.ID_details == objects.ID_details) select f).FirstOrDefault();
                        if (detail == null)
                        {
                            return Json(new { Result = "Error no ID found" });
                        }
                        else
                        {

                            if (objects.fsource == null) { objects.fsource = ""; }
                            if (objects.fdescription == null) { objects.fdescription = ""; }
                            if (objects.fvalueText == null) { objects.fvalueText = ""; }
                            if (objects.query1 == null) { objects.query1 = ""; }

                            detail.fvalue = objects.fvalue;
                            detail.fsource = objects.fsource;
                            detail.fdescription = objects.fdescription;
                            detail.fvalueDecimal = objects.fvalueDecimal;
                            detail.fvalueText = objects.fvalueText;
                            detail.query1 = objects.query1;

                            db.Entry(detail).State = EntityState.Modified;
                            db.SaveChanges();
                            return Json(new { Result = "Success" });
                        }
                    }
                    



                }

                
      
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error: " + ex.Message });
            }



        }
        //CHECK OUT
        public ActionResult Check_out(string ID_visit, string check_in, string lat, string lng, List<ActivitiesM_offline> objects)
        {
            try
            {
                int idid = Convert.ToInt32(ID_visit);
                int IDU = Convert.ToInt32(Session["IDusuario"]);

                DateTime check = DateTime.UtcNow;
                try //Validamos la hora ya que viene del lado del cliente y el formato no esta definido
                {
                    check = Convert.ToDateTime(check_in);
                }
                catch
                {
                    check = DateTime.UtcNow;
                }


                ////Guardamos los datos de las actividades
                //if (objects != null) {

                //    foreach (var actobj in objects)
                //{
                //    ActivitiesM actividadObj = new ActivitiesM();//Creamos la actividad
                //    if (actobj.isnew == false)
                //    {
                //        //Evaluamos si es copia
                //        if (actobj.query1 == "copy")
                //        {//Si es copia y no es nuevo quiere decir que se guardo en el servidor pero como no se actualiza la query, tenemos que validar 
                //         //Para hacer un guardado y no un insert
                //         //Buscamos la actividad y la asignamos
                //            actividadObj = (from a in db.ActivitiesM where (a.query1 == actobj.query1) select a).FirstOrDefault();

                //            actividadObj.check_out = actobj.check_out;
                //            actividadObj.check_in = actobj.check_in;
                //            actividadObj.isfinished = actobj.isfinished;
                //            actividadObj.ID_usuarioEndString = "";
                //            db.Entry(actividadObj).State = EntityState.Modified;
                //            db.SaveChanges();
                //        }
                //        else
                //        {
                //            //NO es copia, por lo tanto
                //            //Buscamos la actividad y la asignamos
                //            actividadObj = (from a in db.ActivitiesM where (a.ID_activity == actobj.ID_activity) select a).FirstOrDefault();

                //            actividadObj.check_out = actobj.check_out;
                //            actividadObj.check_in = actobj.check_in;
                //            actividadObj.isfinished = actobj.isfinished;
                //            actividadObj.ID_usuarioEndString = "";
                //            db.Entry(actividadObj).State = EntityState.Modified;
                //            db.SaveChanges();
                //        }


                //    }
                //    else
                //    {
                //        //Como es nuevo, lo creamos completo

                //        actividadObj.check_out = actobj.check_out;
                //        actividadObj.check_in = actobj.check_in;
                //        actividadObj.isfinished = actobj.isfinished;
                //        actividadObj.query1 = actobj.query1;

                //        actividadObj.ID_visit = actobj.ID_visit;
                //        actividadObj.ID_form = actobj.ID_form;
                //        actividadObj.ID_customer = actobj.ID_customer;
                //        actividadObj.Customer = actobj.Customer;
                //        actividadObj.comments = actobj.comments;

                //        actividadObj.ID_empresa = actobj.ID_empresa;

                //        actividadObj.description = actobj.description;
                //        actividadObj.ID_usuarioCreate = actobj.ID_usuarioCreate;
                //        actividadObj.ID_usuarioEnd = actobj.ID_usuarioEnd;
                //        actividadObj.date = actobj.date;
                //        actividadObj.ID_activitytype = actobj.ID_activitytype;
                //        actividadObj.ID_usuarioEndString = "";

                //        db.ActivitiesM.Add(actividadObj);
                //        db.SaveChanges();


                //    }

                //}

                //}




                //CON ESTO EVALUAMOS LA VISITA COMPLETA
                bool flagok = true;
                var actvities = (from ac in db.ActivitiesM where (ac.ID_visit == idid) select ac).ToList();

                foreach (var item in actvities)
                {
                    if (item.isfinished == false) { flagok = false; }

                }


                if (flagok != false)
                {

                    VisitsM visita = db.VisitsM.Find(Convert.ToInt32(ID_visit));
                    if (visita != null)
                    {
                        visita.ID_visitstate = 4; //FINALIZADO
                        visita.check_out =check;
                        db.Entry(visita).State = EntityState.Modified;
                        db.SaveChanges();


                        if (lat != null || lat != "")
                        {
                            ////Guardamos el log de la actividad
                            ActivitiesM_log nuevoLog = new ActivitiesM_log();
                            nuevoLog.latitude = lat;
                            nuevoLog.longitude = lng;
                            nuevoLog.ID_usuario = IDU;
                            nuevoLog.ID_activity = 0;
                            nuevoLog.fecha_conexion = check;
                            nuevoLog.query1 = ID_visit;
                            nuevoLog.query2 = "";
                            nuevoLog.action = "CHECK OUT  - " + visita.store;
                            nuevoLog.ip = "";
                            nuevoLog.hostname = "";
                            nuevoLog.typeh = "";
                            nuevoLog.continent_name = "";
                            nuevoLog.country_code = "";
                            nuevoLog.country_name = "";
                            nuevoLog.region_code = "";
                            nuevoLog.region_name = "";
                            nuevoLog.city = "";

                            db.ActivitiesM_log.Add(nuevoLog);
                            db.SaveChanges();
                        }


                    }
                }


                bool flagokrep = true;
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == IDU) select c).FirstOrDefault();
                //Cambios estado de visita por rep
                VisitsM_representatives rep = (from a in db.VisitsM_representatives
                                               where (a.ID_visit == idid && a.ID_usuario == datosUsuario.ID_usuario)
                                               select a
                     ).FirstOrDefault();

                var actvitiesrep = (from ac in db.ActivitiesM where (ac.ID_visit == idid && ac.ID_usuarioEnd == datosUsuario.ID_usuario) select ac).ToList();

                foreach (var item in actvitiesrep)
                {
                    if (item.isfinished == false) { flagokrep = false; }

                }

                if (flagokrep != false)
                {
                    rep.query1 = "4";
                    db.Entry(rep).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { Result = "Success" });
                }
                else
                {

                    return Json(new { Result = "There are some incomplete activities. Please check and try again" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error: " + ex.Message });
            }



        }

        //CANCEL
        public ActionResult cancel_visit(string ID_visit, string check_in, string lat, string lng)
        {
            try
            {
                int IDU = Convert.ToInt32(Session["IDusuario"]);
                VisitsM visita = db.VisitsM.Find(Convert.ToInt32(ID_visit));
                DateTime check = DateTime.UtcNow;
                try //Validamos la hora ya que viene del lado del cliente y el formato no esta definido
                {
                    check = Convert.ToDateTime(check_in);
                }
                catch {
                    check = DateTime.UtcNow;
                }


                if (visita != null)
                {
                    visita.ID_visitstate = 1; //CANCELADO
                    visita.check_in = check;
                    db.Entry(visita).State = EntityState.Modified;
                    db.SaveChanges();


                    if (lat != null || lat != "")
                    {
                        //Guardamos el log de la actividad
                        ActivitiesM_log nuevoLog = new ActivitiesM_log();
                        nuevoLog.latitude = lat;
                        nuevoLog.longitude = lng;
                        nuevoLog.ID_usuario = IDU;
                        nuevoLog.ID_activity = 0;
                        nuevoLog.fecha_conexion =check;
                        nuevoLog.query1 = ID_visit;
                        nuevoLog.query2 = "";
                        nuevoLog.action = "CHECK IN  - " + visita.store;
                        nuevoLog.ip = "";
                        nuevoLog.hostname = "";
                        nuevoLog.typeh = "";
                        nuevoLog.continent_name = "";
                        nuevoLog.country_code = "";
                        nuevoLog.country_name = "";
                        nuevoLog.region_code = "";
                        nuevoLog.region_name = "";
                        nuevoLog.city = "";

                        db.ActivitiesM_log.Add(nuevoLog);
                        db.SaveChanges();
                    }

                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Fail" });
            }
            catch
            {
                return Json(new { Result = "Error" });
            }



        }
        //CANCEL
        public ActionResult cancel_ActivityOffline(string ID_activityCancel, string ID_visitCancel, string commentCancel, string check)
        {
            try
            {
                ActivitiesM activity = new ActivitiesM();
                try
                {
                    int idd = Convert.ToInt32(ID_activityCancel);
                    activity = db.ActivitiesM.Find(idd); //Es actividad normal desde su id
                }
                catch {// es generico por lo tanto tenemos que buscar por query1
                    var sqlQueryText = string.Format("SELECT * FROM ActivitiesM WHERE query1 LIKE '{0}'", ID_activityCancel);
                    activity = db.ActivitiesM.SqlQuery(sqlQueryText).FirstOrDefault(); //returns 0 or more rows satisfying sql query
                }
                

                activity.isfinished = true;
                activity.query1 = "cancel";
                activity.comments = commentCancel;
                activity.check_out = Convert.ToDateTime(check);

                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();


                var result = new { Result = "Activity canceled successfully" };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                var result = new { Result = "Error " + ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult PreviewSalesOrderResume(int? id)
        {

            var activity_header = (from a in db.ActivitiesM where (a.ID_activity == id) select a).FirstOrDefault();



            if (activity_header !=null)

            {

                //Existen datos
                //Buscamos los detalles
                //3 - Products 
                var activity_details = (from b in db.FormsM_details where (b.ID_visit == id && b.ID_formresourcetype == 3 && b.fvalue>0) select b).OrderBy(b => b.obj_order).ToList();
                var authorized = (from b in db.FormsM_details where (b.ID_visit == id && b.ID_formresourcetype == 6) select b).FirstOrDefault();

                foreach (var product in activity_details) {
                    var productinfo = (from p in CMKdb.OITM where (p.ItemCode == product.fsource) select p).FirstOrDefault();

                    if (productinfo != null) {
                        product.query1 = "x    " + productinfo.BuyUnitMsr;
                    }
                }

                ////Rep
                var rep = (from u in db.Usuarios where (u.ID_usuario == activity_header.ID_usuarioEnd) select u).FirstOrDefault();
                //
                var repname = "No data was found";
                if (rep != null) {
                    repname = rep.nombre + " " + rep.apellido;
                }

                var visit = (from f in db.VisitsM where (f.ID_visit == activity_header.ID_visit) select f).FirstOrDefault();
                var storename = "";

                if (visit != null)
                {
                    storename = visit.store + ", " + visit.address + ", " + visit.city + ", " + visit.zipcode;

                }

                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptSalesOrder.rpt"));


                //*******************************
                rd.SetDataSource(activity_details);

                rd.SetParameterValue("suggested_order", activity_header.ID_activity);
                rd.SetParameterValue("customer", activity_header.Customer);
                rd.SetParameterValue("store", storename);
                rd.SetParameterValue("date", activity_header.date);
                rd.SetParameterValue("rep", repname.ToString().ToUpper());
                rd.SetParameterValue("authorized", authorized.fsource.ToString().ToUpper());


                //Verificams si existe firma electronica
                var firma = (from d in db.FormsM_details where (d.ID_visit == id && d.ID_formresourcetype == 9) select d).ToList();

                int firmaC = firma.Count();




                if (firmaC == 1)
                {

                    string data = firma[0].fsource;
                    if (data != "")
                    {
                        var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                        var binData = Convert.FromBase64String(base64Data);

                        using (var streamf = new MemoryStream(binData))
                        {

                            Bitmap myImage = new Bitmap(streamf);

                            // Assumes myImage is the PNG you are converting
                            using (var b = new Bitmap(myImage.Width, myImage.Height))
                            {
                                b.SetResolution(myImage.HorizontalResolution, myImage.VerticalResolution);

                                using (var g = Graphics.FromImage(b))
                                {
                                    g.Clear(Color.White);
                                    g.DrawImageUnscaled(myImage, 0, 0);
                                }

                                // Now save b as a JPEG like you normally would

                                var path = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), "signdemod.jpg");
                                b.Save(path, ImageFormat.Jpeg);


                                rd.SetParameterValue("urlimgsign", Path.GetFullPath(path));
                            }



                        }
                    }
                    else
                    {
                        rd.SetParameterValue("urlimgsign", "");

                    }

                }
                else
                {
                    rd.SetParameterValue("urlimgsign", "");
                }


                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "Suggested Order Resume; ");



                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);



                return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }
        }
        public ActionResult PreviewFormStockResume(int? id)
        {

            var activity_header = (from a in db.ActivitiesM where (a.ID_activity == id) select a).FirstOrDefault();



            if (activity_header != null)

            {

                //Existen datos
                //Buscamos los detalles
                //3 - Products 
                var activity_details = (from b in db.FormsM_details where (b.ID_visit == id && b.ID_formresourcetype == 3) select b).OrderBy(b => b.obj_order).ToList();
                var activity_expdate = (from b in db.FormsM_details where (b.ID_visit == id && b.ID_formresourcetype == 22) select b).OrderBy(b => b.obj_order).ToList();
                var authorized = (from b in db.FormsM_details where (b.ID_visit == id && b.ID_formresourcetype == 13) select b).FirstOrDefault();

                int count = 0;
                foreach (var item in activity_details) {
                    item.fvalueText = activity_expdate[count].fvalueText;
                    count++;
                }

                //var authorized = (from b in db.FormsM_details where (b.ID_visit == id && b.ID_formresourcetype == 6) select b).FirstOrDefault();

                //foreach (var product in activity_details)
                //{
                //    var productinfo = (from p in CMKdb.OITM where (p.ItemCode == product.fsource) select p).FirstOrDefault();

                //    if (productinfo != null)
                //    {
                //        product.query1 = "x    " + productinfo.BuyUnitMsr;
                //    }
                //}

                ////Rep
                var rep = (from u in db.Usuarios where (u.ID_usuario == activity_header.ID_usuarioEnd) select u).FirstOrDefault();
                //
                var repname = "No data was found";
                if (rep != null)
                {
                    repname = rep.nombre + " " + rep.apellido;
                }

                var visit = (from f in db.VisitsM where (f.ID_visit == activity_header.ID_visit) select f).FirstOrDefault();
                var storename = "";

                if (visit != null)
                {
                    storename = visit.store + ", " + visit.address + ", " + visit.city + ", " + visit.zipcode;

                }

                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptStockControl.rpt"));


                //*******************************
                rd.SetDataSource(activity_details);

                rd.SetParameterValue("suggested_order", activity_header.ID_activity);
                rd.SetParameterValue("customer", activity_header.Customer);
                rd.SetParameterValue("store", storename);
                rd.SetParameterValue("date", activity_header.date);
                rd.SetParameterValue("rep", repname.ToString().ToUpper());
                rd.SetParameterValue("brand", authorized.fdescription.ToString().ToUpper());

                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "Stock inventory control Resume; ");



                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);



                return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }
        }

        public ActionResult PreviewSendDemoResume(int? id)
        {

            var demo_header = (from a in db.ActivitiesM where (a.ID_activity == id) select a).ToList();

        


            if (demo_header.Count > 0)

            {
                var id_visit = demo_header[0].ID_visit;
                var visit = (from b in db.VisitsM where (b.ID_visit == id_visit) select b).FirstOrDefault();
                foreach (var item in demo_header)
                {
                    var usuario = (from u in CMKdb.OCRD where (u.CardCode == item.ID_usuarioEndString) select u).FirstOrDefault();
                    if (usuario == null)
                    {

                    }
                    else
                    {
                        item.ID_usuarioEndString = usuario.CardName;
                    }
                   
                    item.Customer = visit.store + ",  " + visit.address;

                }

                //Existen datos
                //Buscamos los detalles
                //3 - Products | 4- Products samples | 6 - Input_text | 10- GIFT
                var demo_details = (from b in db.FormsM_details where (b.ID_visit == id && (b.ID_formresourcetype == 3 || b.ID_formresourcetype == 4 || b.ID_formresourcetype == 6 || b.ID_formresourcetype == 10)) select b).OrderBy(b => b.ID_formresourcetype).ToList();
                var result = demo_details
                        .GroupBy(l => new { ID_formresourcetype = l.ID_formresourcetype, fsource = l.fsource })
                        .Select(cl => new FormsM_details
                        {
                            ID_details = cl.First().ID_details,
                            ID_formresourcetype = cl.First().ID_formresourcetype,
                            fsource = cl.First().fsource,
                            fdescription = cl.First().fdescription,
                            fvalue = cl.Sum(c => c.fvalue),
                            ID_formM = cl.First().ID_formM,
                            ID_visit = cl.First().ID_visit,
                            original = cl.First().original,
                            obj_order = cl.First().obj_order,
                            obj_group = cl.First().obj_group
                        }).ToList();


                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptDemo.rpt"));

                //Obtenemos el nombre de las marcas o brands por cada articulo
                var listadeItems = (from d in db.FormsM_details where (d.ID_visit == id && d.ID_formresourcetype == 3) select d).ToList();

                var oitm = (from h in CMKdb.OITM select h).ToList();
                var omrc = (from i in CMKdb.OMRC select i).ToList();
                foreach (var itemd in listadeItems)
                {

                    itemd.fdescription = (from k in oitm join j in omrc on k.FirmCode equals j.FirmCode where (k.ItemCode == itemd.fsource) select j.FirmName).FirstOrDefault();
                    if (itemd.fdescription == null)
                    {
                        itemd.fdescription = "No data found";
                    }
                }

                var brands = listadeItems.GroupBy(test => test.fdescription).Select(grp => grp.First()).ToList();

                var brandstoshow = "";
                int count = 0;
                foreach (var items in brands)
                {
                    if (count == 0)
                    {
                        brandstoshow = items.fdescription.ToString();
                    }
                    else
                    {
                        brandstoshow += ", " + items.fdescription.ToString();
                    }
                    count += 1;
                }
                //*******************************

                demo_header[0].query1 = brandstoshow;

                rd.SetDataSource(demo_header);

                rd.Subreports[0].SetDataSource(result);

                //Verificamos si existen fotos en el demo (MAX 4 fotos)
                var fotos = (from c in db.FormsM_details where (c.ID_visit == id && c.ID_formresourcetype == 5) select c).ToList();

                int fotosC = fotos.Count();




                if (fotosC == 4)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }
                    if (fotos[2].fsource == "")
                    {
                        rd.SetParameterValue("urlimg3", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                    }
                    if (fotos[3].fsource == "")
                    {
                        rd.SetParameterValue("urlimg4", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg4", Path.GetFullPath(Server.MapPath(fotos[3].fsource)));
                    }


                }
                else if (fotosC == 3)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }
                    if (fotos[2].fsource == "")
                    {
                        rd.SetParameterValue("urlimg3", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                    }

                    rd.SetParameterValue("urlimg4", "");

                }
                else if (fotosC == 2)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }

                    rd.SetParameterValue("urlimg3", "");

                    rd.SetParameterValue("urlimg4", "");

                }
                else if (fotosC == 1)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }

                    rd.SetParameterValue("urlimg2", "");

                    rd.SetParameterValue("urlimg3", "");

                    rd.SetParameterValue("urlimg4", "");

                }
                else
                {

                    rd.SetParameterValue("urlimg1", "");
                    rd.SetParameterValue("urlimg2", "");
                    rd.SetParameterValue("urlimg3", "");
                    rd.SetParameterValue("urlimg4", "");
                }


                //Verificams si existe firma electronica
                var firma = (from d in db.FormsM_details where (d.ID_visit == id && d.ID_formresourcetype == 9) select d).ToList();

                int firmaC = firma.Count();




                if (firmaC == 1)
                {

                    string data = firma[0].fsource;
                    if (data != "")
                    {
                        var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                        var binData = Convert.FromBase64String(base64Data);

                        using (var streamf = new MemoryStream(binData))
                        {

                            Bitmap myImage = new Bitmap(streamf);

                            // Assumes myImage is the PNG you are converting
                            using (var b = new Bitmap(myImage.Width, myImage.Height))
                            {
                                b.SetResolution(myImage.HorizontalResolution, myImage.VerticalResolution);

                                using (var g = Graphics.FromImage(b))
                                {
                                    g.Clear(Color.White);
                                    g.DrawImageUnscaled(myImage, 0, 0);
                                }

                                // Now save b as a JPEG like you normally would

                                var path = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), "signdemod.jpg");
                                b.Save(path, ImageFormat.Jpeg);


                                rd.SetParameterValue("urlimgsign", Path.GetFullPath(path));
                            }



                        }
                    }
                    else
                    {
                        rd.SetParameterValue("urlimgsign", "");

                    }

                }
                else
                {
                    rd.SetParameterValue("urlimgsign", "");
                }


                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Resume; ");



                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);



                return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }
        }

        public ActionResult SendDemoResume(int? id)
        {

            var demo_header = (from a in db.ActivitiesM where (a.ID_activity == id) select a).ToList();




            if (demo_header.Count > 0)

            {
                var id_visit = demo_header[0].ID_visit;
                var visit = (from b in db.VisitsM where (b.ID_visit == id_visit) select b).FirstOrDefault();
                foreach (var item in demo_header)
                {
                    var usuario = (from u in CMKdb.OCRD where (u.CardCode == item.ID_usuarioEndString) select u).FirstOrDefault();
                    if (usuario == null)
                    {

                    }
                    else
                    {
                        item.ID_usuarioEndString = usuario.CardName;
                    }

                    item.Customer = visit.store + ",  " + visit.address;

                }

                //Existen datos
                //Buscamos los detalles
                //3 - Products | 4- Products samples | 6 - Input_text | 10- GIFT
                var detallesMaestro = (from b in db.FormsM_details where (b.ID_visit == id) select b).OrderBy(b => b.ID_formresourcetype).ToList();


                var demo_details = (from b in detallesMaestro where (b.ID_visit == id && (b.ID_formresourcetype == 3 || b.ID_formresourcetype == 4 || b.ID_formresourcetype == 6 || b.ID_formresourcetype == 10)) select b).OrderBy(b => b.ID_formresourcetype).ToList();
                var result = demo_details
                        .GroupBy(l => new { ID_formresourcetype = l.ID_formresourcetype, fsource = l.fsource })
                        .Select(cl => new FormsM_details
                        {
                            ID_details = cl.First().ID_details,
                            ID_formresourcetype = cl.First().ID_formresourcetype,
                            fsource = cl.First().fsource,
                            fdescription = cl.First().fdescription,
                            fvalue = cl.Sum(c => c.fvalue),
                            ID_formM = cl.First().ID_formM,
                            ID_visit = cl.First().ID_visit,
                            original = cl.First().original,
                            obj_order = cl.First().obj_order,
                            obj_group = cl.First().obj_group
                        }).ToList();


                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptDemo.rpt"));

                //Obtenemos el nombre de las marcas o brands por cada articulo
                var listadeItems = (from d in detallesMaestro where (d.ID_visit == id && d.ID_formresourcetype == 3) select d).ToList();

                var oitm = (from h in CMKdb.OITM select h).ToList();
                var omrc = (from i in CMKdb.OMRC select i).ToList();
                foreach (var itemd in listadeItems)
                {

                    itemd.fdescription = (from k in oitm join j in omrc on k.FirmCode equals j.FirmCode where (k.ItemCode == itemd.fsource) select j.FirmName).FirstOrDefault();
                    if (itemd.fdescription == null)
                    {
                        itemd.fdescription = "No data found";
                    }
                }

                var brands = listadeItems.GroupBy(test => test.fdescription).Select(grp => grp.First()).ToList();

                var brandstoshow = "";
                int count = 0;
                foreach (var items in brands)
                {
                    if (count == 0)
                    {
                        brandstoshow = items.fdescription.ToString();
                    }
                    else
                    {
                        brandstoshow += ", " + items.fdescription.ToString();
                    }
                    count += 1;
                }
                //*******************************

                demo_header[0].query1 = brandstoshow;

                rd.SetDataSource(demo_header);

                rd.Subreports[0].SetDataSource(result);

                //Verificamos si existen fotos en el demo (MAX 4 fotos)
                var fotos = (from c in detallesMaestro where (c.ID_visit == id && c.ID_formresourcetype == 5) select c).ToList();

                int fotosC = fotos.Count();




                if (fotosC == 4)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }
                    if (fotos[2].fsource == "")
                    {
                        rd.SetParameterValue("urlimg3", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                    }
                    if (fotos[3].fsource == "")
                    {
                        rd.SetParameterValue("urlimg4", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg4", Path.GetFullPath(Server.MapPath(fotos[3].fsource)));
                    }


                }
                else if (fotosC == 3)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }
                    if (fotos[2].fsource == "")
                    {
                        rd.SetParameterValue("urlimg3", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                    }

                    rd.SetParameterValue("urlimg4", "");

                }
                else if (fotosC == 2)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }

                    rd.SetParameterValue("urlimg3", "");

                    rd.SetParameterValue("urlimg4", "");

                }
                else if (fotosC == 1)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }

                    rd.SetParameterValue("urlimg2", "");

                    rd.SetParameterValue("urlimg3", "");

                    rd.SetParameterValue("urlimg4", "");

                }
                else
                {

                    rd.SetParameterValue("urlimg1", "");
                    rd.SetParameterValue("urlimg2", "");
                    rd.SetParameterValue("urlimg3", "");
                    rd.SetParameterValue("urlimg4", "");
                }


                //Verificams si existe firma electronica
                var firma = (from d in detallesMaestro where (d.ID_visit == id && d.ID_formresourcetype == 9) select d).ToList();

                int firmaC = firma.Count();




                if (firmaC == 1)
                {

                    string data = firma[0].fsource;
                    if (data != "")
                    {
                        var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                        var binData = Convert.FromBase64String(base64Data);

                        using (var streamf = new MemoryStream(binData))
                        {

                            Bitmap myImage = new Bitmap(streamf);

                            // Assumes myImage is the PNG you are converting
                            using (var b = new Bitmap(myImage.Width, myImage.Height))
                            {
                                b.SetResolution(myImage.HorizontalResolution, myImage.VerticalResolution);

                                using (var g = Graphics.FromImage(b))
                                {
                                    g.Clear(Color.White);
                                    g.DrawImageUnscaled(myImage, 0, 0);
                                }

                                // Now save b as a JPEG like you normally would

                                var path = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), "signdemod.jpg");
                                b.Save(path, ImageFormat.Jpeg);


                                rd.SetParameterValue("urlimgsign", Path.GetFullPath(path));
                            }



                        }
                    }
                    else
                    {
                        rd.SetParameterValue("urlimgsign", "");

                    }

                }
                else
                {
                    rd.SetParameterValue("urlimgsign", "");
                }


                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Report; ");



                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);


                //PARA PREVISULIZACION
                //return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

                //PARA ENVIAR POR CORREO

                try

                {
                    //limpiamos el directorio

                    System.IO.DirectoryInfo di = new DirectoryInfo(filePathOriginal);

                    foreach (FileInfo file in di.GetFiles())

                    {

                        file.Delete();

                    }

                    foreach (DirectoryInfo dir in di.GetDirectories())

                    {

                        dir.Delete(true);

                    }

                }

                catch (Exception e)

                {

                    var mensaje = e.ToString();

                }

                var path2 = "";
                var filename = "DEMO REPORT" + "" + ".pdf";
                path2 = Path.Combine(filePathOriginal, filename);
                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path2);

                //PdfDocument doc = new PdfDocument();
                //doc.LoadFromFile(path);
                ////Contamos numero total de paginas
                //int indexpages = doc.Pages.Count;

                //Image img = doc.SaveAsImage(0);
                //var imagename = "Accounts Receivable Report " + seller.SalesRepresentative + ".jpg";
                //pathimage = Path.Combine(filePathOriginal, imagename);
                //img.Save(pathimage);
                //doc.Close();


                //Para enviar correos
                //var data2 = demo_header.FirstOrDefault();
                //try
                //{
                    


                //    dynamic email = new Email("DemoResume");
                //    email.To = "f.velasquez@limenainc.net";
                //    email.From = "customercare@comerciamarketing.com";
                //    email.Subject = "(FINISHED) DEMO IN " + visit.store + "- " + visit.visit_date.ToShortDateString();
                //    email.Attach(new Attachment(path2));
                //    //email.Body = imagename;
                //    //return new EmailViewResult(email);





                //    email.Send();


                //}

                //catch (Exception e)
                //{
                //    Console.WriteLine("{0} Exception caught.", e);
                //}

                try
                {
                    var emp = demo_header[0].ID_customer;
                    var id_empresa = (from j in db.Empresas where (j.ID_SAP == emp) select j.ID_empresa).FirstOrDefault();
                    var contactos = (from u in db.Usuarios where (u.ID_tipomembresia == 7 && u.ID_empresa == id_empresa && u.activo == true) select u).ToList();
                    if (contactos.Count > 0)
                    {
                        foreach (var item in contactos)
                        {
                            if (item.correo != null)
                            {
                                dynamic email = new Email("DemoResume");
                                email.To = item.correo;
                                email.From = "customercare@comerciamarketing.com";
                                email.Subject = "DEMO REPORT FOR " + visit.store + "- " + visit.visit_date.ToShortDateString();
                                email.Attach(new Attachment(path2));
                                //email.Body = imagename;
                                //return new EmailViewResult(email);


                                email.Send();

                            }

                        }

                    }
                    else
                    {
                        TempData["exito"] = "No emails contacts.";
                        return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = visit.ID_visit });
                    }



                }

                catch (Exception e)
                {

                    TempData["advertenca"] = "Something went wrong, please try again. " + e.Message;
                    return RedirectToAction("Visit_details", "SalesRepresentatives", new { id = visit.ID_visit });
                }


                TempData["exito"] = "Emails sent successfully.";
                return RedirectToAction("Visit_details", "SalesRepresentatives", new { id=visit.ID_visit });
            }
            else
            {
                TempData["advertencia"] = "Something wrong happened, try again.";
                return RedirectToAction("Index", "Home", null);
            }
        }

        public class DemosRpt
        {
            public int ID_demo { get; set; }
            public string ID_Vendor { get; set; }
            public string vendor { get; set; }
            public string ID_Store { get; set; }
            public string store { get; set; }
            public System.DateTime visit_date { get; set; }
            public string ID_usuario { get; set; }
            public int ID_demostate { get; set; }
            public string comments { get; set; }
            public int ID_form { get; set; }
            public decimal extra_hours { get; set; }
            public System.DateTime end_date { get; set; }
            public System.DateTime check_in { get; set; }
            public string geoLong { get; set; }
            public string geoLat { get; set; }
        }
        public ActionResult Demo_preview(int? id)
        {

            var demo_header = (from a in db.Demos where (a.ID_demo == id) select a).ToList();

            if (demo_header.Count > 0)

            {


                //Existen datos
                //Buscamos los detalles
                //3 - Products | 4- Products samples | 6 - Input_text | 10- GIFT
                var demo_details = (from b in db.FormsM_detailsDemos where (b.ID_visit == id && (b.ID_formresourcetype == 3 || b.ID_formresourcetype == 4 || b.ID_formresourcetype == 6 || b.ID_formresourcetype == 10)) select b).OrderBy(b => b.ID_formresourcetype).ToList();
                var result = demo_details
                        .GroupBy(l => new { ID_formresourcetype = l.ID_formresourcetype, fsource = l.fsource })
                        .Select(cl => new FormsM_detailsDemos
                        {
                            ID_details = cl.First().ID_details,
                            ID_formresourcetype = cl.First().ID_formresourcetype,
                            fsource = cl.First().fsource,
                            fdescription = cl.First().fdescription,
                            fvalue = cl.Sum(c => c.fvalue),
                            ID_formM = cl.First().ID_formM,
                            ID_visit = cl.First().ID_visit,
                            original = cl.First().original,
                            obj_order = cl.First().obj_order,
                            obj_group = cl.First().obj_group
                        }).ToList();


                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptDemosPreview.rpt"));

                rd.SetDataSource(demo_header);

                rd.Subreports[0].SetDataSource(result);

                //Verificamos si existen fotos en el demo (MAX 3 fotos)
                var fotos = (from c in db.FormsM_detailsDemos where (c.ID_visit == id && c.ID_formresourcetype == 5) select c).ToList();

                int fotosC = fotos.Count();

                string fullAdd = demo_header[0].store + ", " + demo_header[0].address + ", " + demo_header[0].city + ", " + demo_header[0].state + ", " + demo_header[0].zipcode;

                rd.SetParameterValue("addressFull", fullAdd.ToUpper());

                if (fotosC == 4)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }
                    if (fotos[2].fsource == "")
                    {
                        rd.SetParameterValue("urlimg3", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                    }
                    if (fotos[3].fsource == "")
                    {
                        rd.SetParameterValue("urlimg4", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg4", Path.GetFullPath(Server.MapPath(fotos[3].fsource)));
                    }


                }
                else if (fotosC == 3)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }
                    if (fotos[2].fsource == "")
                    {
                        rd.SetParameterValue("urlimg3", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                    }

                    rd.SetParameterValue("urlimg4", "");


                }
                else if (fotosC == 2)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }

                    rd.SetParameterValue("urlimg3", "");
                    rd.SetParameterValue("urlimg4", "");

                }
                else if (fotosC == 1)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }

                    rd.SetParameterValue("urlimg2", "");

                    rd.SetParameterValue("urlimg3", "");
                    rd.SetParameterValue("urlimg4", "");


                }
                else
                {

                    rd.SetParameterValue("urlimg1", "");
                    rd.SetParameterValue("urlimg2", "");
                    rd.SetParameterValue("urlimg3", "");
                    rd.SetParameterValue("urlimg4", "");
                }


                //Verificams si existe firma electronica
                var firma = (from d in db.FormsM_detailsDemos where (d.ID_visit == id && d.ID_formresourcetype == 9) select d).ToList();

                int firmaC = firma.Count();




                if (firmaC == 1)
                {

                    string data = firma[0].fsource;
                    if (data != "")
                    {
                        var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                        var binData = Convert.FromBase64String(base64Data);

                        using (var streamf = new MemoryStream(binData))
                        {

                            Bitmap myImage = new Bitmap(streamf);

                            // Assumes myImage is the PNG you are converting
                            using (var b = new Bitmap(myImage.Width, myImage.Height))
                            {
                                b.SetResolution(myImage.HorizontalResolution, myImage.VerticalResolution);

                                using (var g = Graphics.FromImage(b))
                                {
                                    g.Clear(Color.White);
                                    g.DrawImageUnscaled(myImage, 0, 0);
                                }

                                // Now save b as a JPEG like you normally would

                                var path = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), "signdemod.jpg");
                                b.Save(path, ImageFormat.Jpeg);


                                rd.SetParameterValue("urlimgsign", Path.GetFullPath(path));
                            }



                        }
                    }
                    else
                    {
                        rd.SetParameterValue("urlimgsign", "");

                    }

                }
                else
                {
                    rd.SetParameterValue("urlimgsign", "");
                }


                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Resume; ");



                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);



                return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }
        }

        public ActionResult PreviewDemoResumeByCustomer(string id)
        {
            //DateTime today_init_hour = DateTime.Today;
            //DateTime today_end_hour = DateTime.Today.AddHours(23).AddMinutes(58);

            List<DemosRpt> total_demos = new List<DemosRpt>();
            var cust = "";
            //var total_demos = (from e in db.Demos where (e.ID_Vendor == id && e.visit_date >= today_init_hour && e.visit_date <= today_end_hour) select e).ToList();
            total_demos = (from e in db.ActivitiesM where (e.ID_customer == id && e.ID_activitytype == 4) select new DemosRpt
            {
                ID_demo = e.ID_activity,
                check_in = e.check_in,
                comments = e.comments,
                ID_demostate = 0,
                end_date = e.check_out,
                extra_hours = 0,
                geoLat = "0",
                geoLong = "0",
                ID_form = e.ID_form,
                ID_Store = e.ID_visit.ToString(),
                ID_usuario = e.ID_usuarioEnd.ToString(),
                vendor = e.Customer,
                visit_date = e.date,
                ID_Vendor = e.ID_customer,
                store = ""
            
            }).ToList();
            
            if (total_demos.Count > 0) { 

                cust = total_demos[0].vendor;
            //Recuperamos los IDS de las demos en el dia especifico y del customer especifico
            int[] demo_ids = (from f in total_demos select f.ID_demo).ToArray();

                //Existen datos
                //Buscamos los detalles

                var demo_details_items = (from b in db.FormsM_details where (demo_ids.Contains(b.ID_visit) && (b.ID_formresourcetype == 3 || b.ID_formresourcetype == 4 || b.ID_formresourcetype == 6)) select b).OrderBy(b => b.ID_formresourcetype).ToList();
                var result = demo_details_items
                                        .GroupBy(l => new { ID_formresourcetype = l.ID_formresourcetype, fsource = l.fsource })
                                        .Select(cl => new Forms_details
                                        {
                                            ID_details = cl.First().ID_details,
                                            ID_formresourcetype = cl.First().ID_formresourcetype,
                                            fsource = cl.First().fsource,
                                            fdescription = cl.First().fdescription,
                                            fvalue = cl.Sum(c => c.fvalue),
                                            ID_form = cl.First().ID_formM,
                                            ID_demo = cl.First().ID_visit,
                                            original = cl.First().original,
                                            obj_order = cl.First().obj_order,
                                            obj_group = cl.First().obj_group
                                        }).ToList();


                if (result.Count > 0)

                {

                    //CAMBIAMOS LOS DATOS A LOS QUE NECESITAMOS DESDE SAP COMERCIA
                    /*
                     STORE = NOMBRE DE TIENDA
                     ID_STORE = ESTADO
                     VENDOR = CIUDAD
                     ID_VENDOR = UNIDADES VENDIDAS
                     
                     */
                    //VARIABLE PARA TOTAL DE HORAS DEMO
                    TimeSpan totaldemohours = new TimeSpan(0, 0, 0);
                
                    foreach (var item in total_demos)
                    {
                        int idvi = Convert.ToInt32(item.ID_Store);

                        var store_details = (from CM in db.VisitsM where (CM.ID_visit == idvi) select CM).FirstOrDefault();
                        
                        if (store_details == null)
                        {
                            item.store = "NOT FOUND";
                            item.ID_Store = item.ID_Store + ": NOT FOUND";
                            //item.vendor = "NOT FOUND";

                            decimal sumLineTotal = (from s in db.FormsM_details where (s.ID_visit == item.ID_demo && s.ID_formresourcetype == 3) select s.fvalue).Sum();

                            item.ID_Vendor = Convert.ToString(sumLineTotal);

                            DateTime dt = item.check_in;
                            DateTime dt2 = item.end_date;
                            TimeSpan ts = (dt2 - dt);

                            item.check_in = item.check_in.AddHours(-(Convert.ToDouble(item.extra_hours)));
                            totaldemohours = totaldemohours + ts;
                        }
                        else
                        {
                            item.store = store_details.store;
                            item.ID_Store = store_details.state;
                            item.vendor = store_details.city;

                            decimal sumLineTotal = (from s in db.FormsM_details where (s.ID_visit == item.ID_demo && s.ID_formresourcetype == 3) select s.fvalue).Sum();

                            item.ID_Vendor = Convert.ToString(sumLineTotal);


                            DateTime dt = item.check_in;
                            DateTime dt2 = item.end_date;
                            TimeSpan ts = (dt2 - dt);

                            item.check_in = item.check_in.AddHours(-(Convert.ToDouble(item.extra_hours)));
                            totaldemohours = totaldemohours + ts;

                        }

                    }

                    ReportDocument rd = new ReportDocument();

                    rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptDemoDailyResume_v2.rpt"));



                    rd.SetDataSource(total_demos);

                    rd.Subreports[0].SetDataSource(result);
                    // rd.Subreports[1].SetDataSource(total_demos);

                    rd.SetParameterValue("totaldemohours", totaldemohours.ToString(@"hh\:mm"));
                    rd.SetParameterValue("Customer", cust);
                    var filePathOriginal = Server.MapPath("/Reports/pdfReports");

                    Response.Buffer = false;

                    Response.ClearContent();

                    Response.ClearHeaders();


                    //PARA VISUALIZAR
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Resume.pdf; ");



                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                    stream.Seek(0, SeekOrigin.Begin);



                    return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

                }
                else
                {
                    TempData["advertencia"] = "No data to send.";
                    return RedirectToAction("Index", "Empresas", null);
                }
            }
            else
            {
                TempData["advertencia"] = "No Demos assigned for today.";
                return RedirectToAction("Index", "Empresas", null);
            }
        }
    }
}
