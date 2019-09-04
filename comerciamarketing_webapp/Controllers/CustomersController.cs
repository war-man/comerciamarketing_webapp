using ClosedXML.Excel;
using comerciamarketing_webapp.Models;
using CrystalDecisions.CrystalReports.Engine;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace comerciamarketing_webapp.Controllers
{
    public class CustomersController : Controller
    {
        //private dbComerciaEntities db = new dbComerciaEntities();
        //private COM_MKEntities CMKdb = new COM_MKEntities();


        //using (var db = new dbComerciaEntities())
        //{

        //}
        //using (var CMKdb = new COM_MKEntities())
        //{

        //}
        public class Mapa
        {
            public string State { get; set; }
            public int Count { get; set; }

        }
        public class Estados
        {
            public string State { get; set; }

        }

        public class SimpleUserWithImg
        {
            public int ID_user { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string img { get; set; }

        }

        public class CustomVisit
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

        }

        public class activitiesTypes
        {
            public int id { get; set; }
            public string name { get; set; }
            public string activitytype { get; set; }
            public int count { get; set; }
        }
        public class Brands
        {
            public string id { get; set; }
            public string name { get; set; }
            public int count { get; set; }
        }

        // GET: Customers
        public ActionResult Dashboard(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {

            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Dashboard";
                ViewBag.menunameid = "marketing_menu";
                ViewBag.submenunameid = "";
                //List<string> d = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstDepartments = JsonConvert.SerializeObject(d);
                //List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstRoles = JsonConvert.SerializeObject(r);

                //ViewData["nameUser"] = 
                ////NOTIFICATIONS
                //DateTime now = DateTime.Today;
                //List<Tb_Alerts> lstAlerts = (from a in dblim.Tb_Alerts where (a.ID_user == activeuser.ID_User && a.Active == true && a.Date == now) select a).OrderByDescending(x => x.Date).Take(5).ToList();
                //ViewBag.lstAlerts = lstAlerts;

                //FIN HEADER
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
                ViewBag.Membresia = activeuser.Tipo_membresia.descripcion;
                ViewBag.id_customer = customersel;
                ViewBag.Company = activeuser.Empresas.nombre;
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();
                var lstUsuarios = 0;
                decimal totalCustomers = 0;

                var rutas = new List<CustomVisit>();
                int[] visitasarray = new int[] { };

                IQueryable<activitiesVisitsBrands> lstActivities;
                using (var db = new dbComerciaEntities())
                {


                    lstUsuarios = (from a in db.Usuarios where (a.estados_influencia.Contains(customersel)) select a).Count();

                    lstActivities = (from a in db.ActivitiesM
                                     join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                     where (a.ID_customer == customersel && (a.date >= filtrostartdate && a.date <= filtroenddate) && a.isfinished == true)
                                     select new activitiesVisitsBrands
                                     {
                                         ID_activity = a.ID_activity,
                                         ID_visit = a.ID_visit,
                                         ID_form = a.ID_form,
                                         formName = a.description,
                                         ID_store = b.ID_store,
                                         store = b.store,
                                         visitDate = b.visit_date,
                                         ID_customer = a.ID_customer,
                                         Customer = a.Customer,
                                         isfinished = a.isfinished,
                                         id_usuarioend = a.ID_usuarioEnd,
                                         id_usuarioendexternal = a.ID_usuarioEndString,
                                         id_activitytype = a.ID_activitytype,
                                         ActivityName = "",
                                         Comments = a.comments,
                                         ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                         Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                         count = 0
                                     });

                    if (brandsel == null || brandsel == 0)
                    {

                    }
                    else {
                        var brandstring = brandsel.ToString();
                        lstActivities = lstActivities.Where(a => a.ID_brand == brandstring);
                    }

                        var visitsarr = (from a in lstActivities select a.ID_visit).Distinct().ToArray();
                    // Convertimos la lista a array
          

                    rutas = (from a in db.VisitsM
                             where ((visitsarr.Contains(a.ID_visit)))
                             select new CustomVisit
                             {
                                 ID_visit = a.ID_visit,
                                 ID_customer = a.ID_customer,
                                 customer = a.customer,
                                 ID_store = a.ID_store,
                                 store = a.store,
                                 address = a.address,
                                 city = a.city,
                                 state = a.state,
                                 zipcode = a.zipcode,
                                 visit_date = a.visit_date,
                                 ID_visitstate = a.ID_visitstate,
                                 comments = a.comments,
                                 check_in = a.check_in,
                                 check_out = a.check_out,
                                 end_date = a.end_date,
                                 geoLat = a.geoLat,
                                 geoLong = a.geoLong,
                                 extra_hours = a.extra_hours,
                                 ID_route = a.ID_route,
                                 ID_empresa = a.ID_empresa
                             }).ToList();



                    List<activitiesTypes> distinct = new List<activitiesTypes>();
                    List<Brands> distinctBrands = new List<Brands>();



                    distinctBrands = (from b in lstActivities
                                      group b by b.ID_brand into g
                                      //where (g.Key != "" && g.Key != null && g.Key !="0")
                                      select new Brands
                                      {
                                          id = g.Key == "" ? "0" : g.Key == null ? "0" : g.Key,
                                          name = g.Key == "" ? "NA" : g.Key == null ? "NA" : g.Key=="0" ? "NA" : g.Select(m => m.Brand).FirstOrDefault() ,
                                          count = lstActivities
    .Where(x => x.ID_brand == g.Key)
    .Select(x => x).Count()
                                      }).ToList();

                    distinctBrands = (from p in distinctBrands
                                      group p by p.id into g
                                      select new Brands
                                      {
                                          id = g.Key,
                                          /**/
                                          name = g.Select(e => e.name).FirstOrDefault(),
                                          count = g.Select(e => e.count).FirstOrDefault()
                                      }).ToList();
                  
                    List<string> stringsBrands = distinctBrands.Select(s => "'" + s.name + "'").ToList();

                    var stringtextBrands = string.Join(",", stringsBrands);
                    ViewBag.brandschart = stringtextBrands;

                    List<string> brandscount = distinctBrands.Select(s =>s.count.ToString()).ToList();

                    var stringtextBrandsCount = string.Join(",", brandscount);
                    ViewBag.brandschartCount = stringtextBrandsCount;


                    distinct = (from b in lstActivities
                                select new activitiesTypes
                                {
                                    id = b.ID_form,
                                    name = b.formName,
                                    activitytype = b.ActivityName,
                                    count = lstActivities
    .Where(x => x.ID_form == b.ID_form)
    .Select(x => x).Count()
                                }).Distinct().ToList();

                    ViewBag.lstTypes = distinct;
                    ViewBag.brands = distinctBrands;
                }



                var result = rutas.GroupBy(x => x.state).
                    Select(x => new Estados
                    {
                        State = x.Select(c => c.state).FirstOrDefault().ToLower() + " : " + " Utils.color(theme.color.info, 0.5)"
                    }).ToList();

                List<string> strings = result.Select(s => (string)s.State).ToList();

                var stringtext = string.Join(",", strings);
                ViewBag.visitsbystaste = stringtext;

                var estadosEstadistica = rutas.GroupBy(x => x.state).
     Select(x => new Mapa
     {
         State = x.Select(c => c.state).FirstOrDefault().ToUpper(),
         Count = x.Select(v => v.state).Count()
     }).ToList();

                ViewBag.estadisticasEstadosmapa = estadosEstadistica.OrderByDescending(c => c.Count);

               



                using (var dbcmk = new COM_MKEntities())
                {
                    totalCustomers = (from b in dbcmk.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "") select b).Count();
                }
                ViewBag.lstUsuariosCount = lstUsuarios;
                ViewBag.totalCustomers = totalCustomers;
                decimal rtcunt = rutas.Count();
                int suma = 0;
                if (totalCustomers == 0)
                {
                    suma = 0;
                }
                else {
                    suma = Convert.ToInt32(Math.Floor((rtcunt / totalCustomers) * 100));
                }
               
                ViewBag.totalporc = suma;

                decimal totalvisits = rutas.Count();
                decimal totalTN = rutas.Where(c => c.state != "GA").Count();
                decimal totalGA = rutas.Where(c => c.state == "GA").Count();
                if (totalvisits == 0)
                {
                    ViewBag.TNperc = 0;
                    ViewBag.GAperc = 0;
                }
                else {
                    ViewBag.TNperc = Math.Round(((totalTN / totalvisits) * 100), 2);
                    ViewBag.GAperc = Math.Round(((totalGA / totalvisits) * 100), 2);
                }


                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        List<OCRD> customers;
                        //if (escliente == true)
                        //{

                        customers = (from b in CMKdb.OCRD where ((b.CardCode == customersel || b.U_CardCodeDLI == customersel) && b.Series==61) select b).ToList();


                        ViewBag.customerssel = customers.ToList();

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.CustomerSelCode = "0";
                        }
                        else
                        {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                        var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                        if (brandsel == null || brandsel == 0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else
                        {
                            //ViewBag.BrandLabel = brandcmk.FirmName;
                            ViewBag.BrandLabel = brandcmk.FirmName;
                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }

                catch
                {

                    ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
                }


                return View(rutas);
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }

        public ActionResult Sales_Representatives(string id, string fstartd, string fendd, string stores, string brands, string spartners)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Sales Representatives";
                ViewBag.menunameid = "marketing_menu";
                ViewBag.submenunameid = "";
                //List<string> d = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstDepartments = JsonConvert.SerializeObject(d);
                //List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstRoles = JsonConvert.SerializeObject(r);

                //ViewData["nameUser"] = 
                ////NOTIFICATIONS
                //DateTime now = DateTime.Today;
                //List<Tb_Alerts> lstAlerts = (from a in dblim.Tb_Alerts where (a.ID_user == activeuser.ID_User && a.Active == true && a.Date == now) select a).OrderByDescending(x => x.Date).Take(5).ToList();
                //ViewBag.lstAlerts = lstAlerts;

                //FIN HEADER
                ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
                ViewBag.id_customer = id;
                List<Usuarios> lstUsuarios = new List<Usuarios>();
                using (var db = new dbComerciaEntities())
                {
                    lstUsuarios = (from a in db.Usuarios where (a.estados_influencia.Contains(id)) select a).ToList();
                }


                return View(lstUsuarios);
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }


        public class Routes_calendar
        {
            public string title { get; set; }
            public string url { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string className { get; set; }
            public string lstReps { get; set; }
            public decimal porcentaje { get; set; }

        }
        public class CustomVisitforRoutes
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
          
        }
        public class SimpleUserforRoutes
        {
            public int ID_user { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string img { get; set; }
            public int EstadoVisita { get; set; }

        }
        public class CustomRoutes
        {
            public int ID_route { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
            public System.DateTime date { get; set; }
            public string query1 { get; set; }
            public string query2 { get; set; }
            public string query3 { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
            public System.DateTime end_date { get; set; }
            public int ID_empresa { get; set; }
            public List<CustomVisitforRoutes> visitasenRuta { get; set; }
            public decimal porcentaje { get; set; }
            public int visitasfinalizadas { get; set; }
            public int visitasenprogreso { get; set; }
            public int visitasagendadas { get; set; }
            public int visitascanceladas { get; set; }
        }

        //CLASE PARA ALMACENAR OBJETOS


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
        }

        //TIENDAS Y RUTAS (Se utiliza en Routes)
        public class tablahijospadreRutas
        {
            public string id { get; set; }
            public string text { get; set; }
            public string parent { get; set; }
        }

        public class MyObj_tablapadreRutas
        {
            public string id { get; set; }
            public string text { get; set; }
            public List<MyObj_tablapadreRutas> children { get; set; }
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
                                                children = ObtenerHijosByID(item.idkey, Categoriashijas)

                                            }).ToList();

            return query;





        }

        private static List<MyObj_tablapadre> ObtenerHijosByID(int ID_parent, List<tablahijospadre> categoriashijas)
        {



            List<MyObj_tablapadre> query = (from item in categoriashijas

                                            where item.parent == ID_parent
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
                                                children = ObtenerHijosByID(item.idkey, categoriashijas)
                                            }).ToList();

            return query;

        }

        public ActionResult GetEvents(DateTime startf, DateTime endf, string id, int? id_brand)
        {
            try
            {

                var rutas = new List<CustomRoutes>();

                using (var db = new dbComerciaEntities())
                {

                    //Nueva forma para tomar datos
                    List<VisitsInfoCalendar> lstVisitas;
                    //FIN FILTROS*******************

                    if (id == null || id == "" || id == "0")
                    {
                        lstVisitas = (from a in db.VisitsM
                                      join b in db.ActivitiesM on a.ID_visit equals b.ID_visit into ps
                                      from p in ps.DefaultIfEmpty()
                                      where ((a.visit_date >= startf && a.end_date <= endf))
                                      //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                      select new VisitsInfoCalendar
                                      {
                                          ID_visit = a.ID_visit,
                                          ID_store = a.ID_store,
                                          idroute = a.ID_route,
                                          visitDate = a.visit_date,
                                          ID_customer = p == null ? "Not Assigned" : p.ID_customer,
                                          ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == p.ID_activity).Select(c => c.fvalueText).FirstOrDefault() ?? "Not Assigned",
                                          Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == p.ID_activity) select detalle.fdescription).FirstOrDefault() ?? "Not Assigned"
                                      }).ToList();

                    }
                    else
                    {
                        lstVisitas = (from a in db.VisitsM
                                      join b in db.ActivitiesM on a.ID_visit equals b.ID_visit into ps
                                      from p in ps.DefaultIfEmpty()
                                      where (p.ID_customer == id &&  (a.visit_date >= startf && a.end_date <= endf))
                                      //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                      select new VisitsInfoCalendar
                                      {
                                          ID_visit = a.ID_visit,
                                          ID_store = a.ID_store,
                                          idroute = a.ID_route,
                                          visitDate = a.visit_date,
                                          ID_customer = p == null ? "Not Assigned" : p.ID_customer,
                                          ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == p.ID_activity).Select(c => c.fvalueText).FirstOrDefault() ?? "Not Assigned",
                                          Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == p.ID_activity) select detalle.fdescription).FirstOrDefault() ?? "Not Assigned"
                                      }).ToList();
                        if (id_brand == null || id_brand == 0)
                        {

                        }
                        else
                        {
                            var brandstring = id_brand.ToString();
                            lstVisitas = lstVisitas.Where(c => c.ID_brand == brandstring).ToList();
                        }

                    }



                    var arryavi = lstVisitas.Select(a => a.idroute).ToArray();

                    //var lstvisits = (from a in db.ActivitiesM
                    //                 join b in db.VisitsM on a.ID_visit equals b.ID_visit
                    //                 where (a.ID_customer == id && (a.date >= startf && a.date <= endf))
                    //                 select b.ID_route).Distinct().ToArray();

                    rutas = (from a in db.RoutesM
                             where (arryavi.Contains(a.ID_route))
                             select new CustomRoutes
                             {

                                 ID_route = a.ID_route,
                                 date = a.date,
                                 query1 = a.query1,
                                 query2 = a.query2,
                                 query3 = a.query3,
                                 end_date = a.end_date,
                                 ID_empresa = a.ID_empresa,
                                 porcentaje = 0,
                                 visitasfinalizadas = 0,
                                 visitasagendadas = 0,
                                 visitascanceladas = 0,
                                 visitasenprogreso = 0,
                                 visitasenRuta = (from t1 in a.VisitsM
                                                  select new CustomVisitforRoutes
                                                  {
                                                      ID_visit = t1.ID_visit,
                                                      ID_customer = t1.ID_customer,
                                                      customer = t1.customer,
                                                      ID_store = t1.ID_store,
                                                      store = t1.store,
                                                      address = t1.address,
                                                      city = t1.city,
                                                      state = t1.state,
                                                      zipcode = t1.zipcode,
                                                      visit_date = t1.visit_date,
                                                      ID_visitstate = t1.ID_visitstate,
                                                      comments = t1.comments,
                                                      check_in = t1.check_in,
                                                      check_out = t1.check_out,
                                                      end_date = t1.end_date,
                                                      geoLat = t1.geoLat,
                                                      geoLong = t1.geoLong,
                                                      extra_hours = t1.extra_hours,
                                                      ID_route = t1.ID_route,
                                                      ID_empresa = t1.ID_empresa
                                                  }).ToList()
                             }).ToList();
                }

                //ESTADISTICA DE RUTAS POR ESTADO DE VISITAS
                decimal totalRutas = rutas.Count();
                foreach (var rutait in rutas)
                {

                    rutait.visitascanceladas = rutait.visitasenRuta.Where(r => r.ID_visitstate == 1 && r.ID_route == rutait.ID_route).Count();
                    rutait.visitasfinalizadas = rutait.visitasenRuta.Where(r => (r.ID_visitstate == 4 || r.ID_visitstate == 1) && r.ID_route == rutait.ID_route).Count();
                    rutait.visitasenprogreso = rutait.visitasenRuta.Where(r => r.ID_visitstate == 2 && r.ID_route == rutait.ID_route).Count();
                    rutait.visitasagendadas = rutait.visitasenRuta.Where(r => r.ID_visitstate == 3 && r.ID_route == rutait.ID_route).Count();
                    totalRutas = (from e in rutait.visitasenRuta where (e.ID_route == rutait.ID_route) select e).Count();

                    //ViewBag.finished = finishedorCanceled;

                    if (totalRutas != 0)
                    {
                        if (rutait.visitasenprogreso != 0 && rutait.visitasfinalizadas != 0)
                        {
                            decimal n = (rutait.visitasfinalizadas / totalRutas) * 100;
                            decimal m = (rutait.visitasenprogreso / totalRutas) * 50;
                            rutait.porcentaje = (n + m);

                        }
                        else if (rutait.visitasenprogreso == 0 && rutait.visitasfinalizadas != 0)
                        {

                            rutait.porcentaje = (((Convert.ToDecimal(rutait.visitasfinalizadas) / totalRutas) * 100));
                        }
                        else if (rutait.visitasenprogreso != 0 && rutait.visitasfinalizadas == 0)
                        {
                            rutait.porcentaje = (((Convert.ToDecimal(rutait.visitasenprogreso) / totalRutas) * 50));
                        }
                        else
                        {
                            rutait.query3 = (Convert.ToDecimal(0)).ToString();
                        }


                    }
                    else
                    {
                        rutait.porcentaje = 0;
                    }
                }

                List<Routes_calendar> rutaslst = new List<Routes_calendar>();
                foreach (var item in rutas)
                {
                    Routes_calendar rt = new Routes_calendar();

                    rt.title = item.ID_route + " - " + item.query2.ToUpper();
                    rt.url = "";
                    rt.start = item.date.ToString("yyyy-MM-dd");
                    rt.end = item.end_date.AddDays(1).ToString("yyyy-MM-dd");
                    if (item.porcentaje == 0)
                    {
                        rt.className = "block b-t b-t-2x b-warning";//"#2081d6";
                    }
                    else if (item.porcentaje > 1 && item.porcentaje < 99)
                    {
                        rt.className = "block b-t b-t-2x b-success";//"#2081d6";
                    }
                    else if (item.porcentaje > 99)
                    {
                        rt.className = "block b-t b-t-2x b-info";//"#2081d6";
                    }

                    rt.porcentaje = item.porcentaje;
                    rutaslst.Add(rt);
                }
                //}
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string result = javaScriptSerializer.Serialize(rutaslst);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }


        }

        public class ImgGallery
        {
            public int idImg { get; set; }
            public string Customer { get; set; }
            public string Brand { get; set; }
            public string Activity { get; set; }
            public string Url { get; set; }
            public string Section { get; set; }
            public string Rep { get; set; }
        }

        public ActionResult Gallery(string id, string fstartd, string fendd, string stores, string brands, string spartners, int? idvisit, int? idroute)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Gallery";
                ViewBag.menunameid = "marketing_menu";
                ViewBag.submenunameid = "";
                //List<string> d = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstDepartments = JsonConvert.SerializeObject(d);
                //List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstRoles = JsonConvert.SerializeObject(r);

                //ViewData["nameUser"] = 
                ////NOTIFICATIONS
                //DateTime now = DateTime.Today;
                //List<Tb_Alerts> lstAlerts = (from a in dblim.Tb_Alerts where (a.ID_user == activeuser.ID_User && a.Active == true && a.Date == now) select a).OrderByDescending(x => x.Date).Take(5).ToList();
                //ViewBag.lstAlerts = lstAlerts;

                //FIN HEADER
                ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
                ViewBag.id_customer = id;

                DateTime filtrostartdate;
                DateTime filtroenddate;
                //filtros de fecha
                //filtros de fecha //MENSUAL
                //var sunday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                //var saturday = sunday.AddMonths(1).AddDays(-1);
                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);
                //FILTROS**************

                if (fstartd == null || fstartd == "")
                {
                    filtrostartdate = sunday;
                }
                else
                {
                    filtrostartdate = Convert.ToDateTime(fstartd);
                }

                if (fendd == null || fendd == "")
                {
                    filtroenddate = saturday;
                }
                else
                {
                    filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59);
                }


                

                try
                {
                    using (var db = new dbComerciaEntities())
                    {
                        var visitsRoute = (from f in db.VisitsM where (f.ID_route == idroute) select f.ID_visit).ToArray();

                        var actividadesList = (from e in db.ActivitiesM where (visitsRoute.Contains(e.ID_visit)) select e);
                        var actividades =  actividadesList.Select(a=> a.ID_activity).ToArray();

                        //var detalles = (from b in db.FormsM_details where (actividades.Contains(b.ID_visit) && b.ID_formresourcetype == 5) select b).ToList();

                        var detalles = (from a in db.FormsM_details
                                        join b in actividadesList on a.ID_visit equals b.ID_activity
                                        where (actividades.Contains(a.ID_visit) && a.ID_formresourcetype == 5)
                                        select new ImgGallery
                                        {
                                            idImg = a.ID_details,
                                            Customer = b.Customer,
                                            Activity = b.description,
                                            Url = a.fsource,
                                            Section = a.fdescription == "PICTURE 1" ? "BEFORE" : a.fdescription == "PICTURE 2" ? "AFTER" : "OTHER",
                                            Rep = (from usu in db.Usuarios where (usu.ID_usuario==b.ID_usuarioEnd) select usu.nombre + " " + usu.apellido).FirstOrDefault(),
                                             Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == b.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                            
                                         }).ToList();

                        var brandslst = (from a in detalles select a.Brand).Distinct().ToList();
                        var sectionlst = (from a in detalles select a.Section).Distinct().ToList();
                        var replst = (from a in detalles select a.Rep).Distinct().ToList();
                        var activitylst = (from a in detalles select a.Activity).Distinct().ToList();

                        ViewBag.brandslst = brandslst;
                        ViewBag.sectionlst = sectionlst;
                        ViewBag.replst = replst;
                        ViewBag.activitylst = activitylst;

                        return View(detalles);
                    }
              

                }
                catch (Exception ex)
                {

                        TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                        return RedirectToAction("Gallery", "Customers", new { id = 0 });
                    

                }

  
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }

        public ActionResult Calendar(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Calendar";
                ViewBag.menunameid = "marketing_menu";
                ViewBag.submenunameid = "";
                //List<string> d = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstDepartments = JsonConvert.SerializeObject(d);
                //List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstRoles = JsonConvert.SerializeObject(r);

                //ViewData["nameUser"] = 
                ////NOTIFICATIONS
                //DateTime now = DateTime.Today;
                //List<Tb_Alerts> lstAlerts = (from a in dblim.Tb_Alerts where (a.ID_user == activeuser.ID_User && a.Active == true && a.Date == now) select a).OrderByDescending(x => x.Date).Take(5).ToList();
                //ViewBag.lstAlerts = lstAlerts;

                //FIN HEADER
                ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
                ViewBag.id_customer = customersel;
                ViewBag.id_brand = brandsel;


                var visitas = new List<VisitsM>();
                var rutas = new List<CustomRoutes>();

                DateTime filtrostartdate;
                DateTime filtroenddate;
                int empresadef = 2;
                //filtros de fecha
                //filtros de fecha //MENSUAL
                //var sunday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                //var saturday = sunday.AddMonths(1).AddDays(-1);
                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);
                //FILTROS**************
                //Enviamos fecha por defecto
                fstartd = "";
                fendd = "";
                if (fstartd == null || fstartd == "")
                {
                    filtrostartdate = sunday;
                }
                else
                {
                    filtrostartdate = Convert.ToDateTime(fstartd);
                }

                if (fendd == null || fendd == "")
                {
                    filtroenddate = saturday;
                }
                else
                {
                    filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59);
                }

                List<VisitsInfoCalendar> lstVisitas;
                //FIN FILTROS*******************
                using (var db = new dbComerciaEntities())
                {
     


                    if (customersel == null || customersel == "" || customersel == "0")
                    {
                        lstVisitas = (from a in db.ActivitiesM
                                      join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                      where (a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished == true)
                                      //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                      select new VisitsInfoCalendar
                                      {
                                          ID_visit = a.ID_visit,
                                          ID_store = b.ID_store,
                                          idroute=b.ID_route,
                                          visitDate = b.visit_date,
                                          ID_customer = a.ID_customer,
                                          ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                          Brand = ""
                                      }).ToList();

                    }
                    else
                    {
                        lstVisitas = (from a in db.ActivitiesM
                                      join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                      where (a.ID_customer == customersel && (a.date >= filtrostartdate && a.date <= filtroenddate) && a.isfinished == true)
                                      //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                      select new VisitsInfoCalendar
                                      {
                                          ID_visit = a.ID_visit,
                                          ID_store = b.ID_store,
                                          idroute = b.ID_route,
                                          visitDate = b.visit_date,
                                          ID_customer = a.ID_customer,
                                          ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                          Brand = ""
                                      }).ToList();
                        if (brandsel == null || brandsel == 0)
                        {

                        }
                        else
                        {
                            var brandstring = brandsel.ToString();
                            lstVisitas = lstVisitas.Where(c => c.ID_brand == brandstring).ToList();
                        }

                    }

                    var lstvisits = lstVisitas.Select(b=>b.idroute).Distinct().ToArray();




                    rutas = (from a in db.RoutesM
                             where (lstvisits.Contains(a.ID_route))
                             select new CustomRoutes
                             {

                                 ID_route = a.ID_route,
                                 date = a.date,
                                 query1 = a.query1,
                                 query2 = a.query2,
                                 query3 = a.query3,
                                 end_date = a.end_date,
                                 ID_empresa = a.ID_empresa,
                                 porcentaje = 0,
                                 visitasfinalizadas = 0,
                                 visitasagendadas = 0,
                                 visitascanceladas = 0,
                                 visitasenprogreso = 0,
                                 visitasenRuta = (from t1 in a.VisitsM
                                                  select new CustomVisitforRoutes
                                                  {
                                                      ID_visit = t1.ID_visit,
                                                      ID_customer = t1.ID_customer,
                                                      customer = t1.customer,
                                                      ID_store = t1.ID_store,
                                                      store = t1.store,
                                                      address = t1.address,
                                                      city = t1.city,
                                                      state = t1.state,
                                                      zipcode = t1.zipcode,
                                                      visit_date = t1.visit_date,
                                                      ID_visitstate = t1.ID_visitstate,
                                                      comments = t1.comments,
                                                      check_in = t1.check_in,
                                                      check_out = t1.check_out,
                                                      end_date = t1.end_date,
                                                      geoLat = t1.geoLat,
                                                      geoLong = t1.geoLong,
                                                      extra_hours = t1.extra_hours,
                                                      ID_route = t1.ID_route,
                                                      ID_empresa = t1.ID_empresa
                                                  }).ToList()
                             }).ToList();


                





                    //ESTADISTICA DE RUTAS POR ESTADO DE VISITAS
                    decimal totalRutas = rutas.Count();
                    foreach (var rutait in rutas)
                    {

                        rutait.visitascanceladas = rutait.visitasenRuta.Where(r => r.ID_visitstate == 1 && r.ID_route == rutait.ID_route).Count();
                        rutait.visitasfinalizadas = rutait.visitasenRuta.Where(r => (r.ID_visitstate == 4 || r.ID_visitstate == 1) && r.ID_route == rutait.ID_route).Count();
                        rutait.visitasenprogreso = rutait.visitasenRuta.Where(r => r.ID_visitstate == 2 && r.ID_route == rutait.ID_route).Count();
                        rutait.visitasagendadas = rutait.visitasenRuta.Where(r => r.ID_visitstate == 3 && r.ID_route == rutait.ID_route).Count();
                        totalRutas = (from e in rutait.visitasenRuta where (e.ID_route == rutait.ID_route) select e).Count();

                        //ViewBag.finished = finishedorCanceled;

                        if (totalRutas != 0)
                        {
                            if (rutait.visitasenprogreso != 0 && rutait.visitasfinalizadas != 0)
                            {
                                decimal n = (rutait.visitasfinalizadas / totalRutas) * 100;
                                decimal m = (rutait.visitasenprogreso / totalRutas) * 50;
                                rutait.porcentaje = (n + m);

                            }
                            else if (rutait.visitasenprogreso == 0 && rutait.visitasfinalizadas != 0)
                            {

                                rutait.porcentaje = (((Convert.ToDecimal(rutait.visitasfinalizadas) / totalRutas) * 100));
                            }
                            else if (rutait.visitasenprogreso != 0 && rutait.visitasfinalizadas == 0)
                            {
                                rutait.porcentaje = (((Convert.ToDecimal(rutait.visitasenprogreso) / totalRutas) * 50));
                            }
                            else
                            {
                                rutait.query3 = (Convert.ToDecimal(0)).ToString();
                            }


                        }
                        else
                        {
                            rutait.porcentaje = 0;
                        }
                    }

                    ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                    ViewBag.filtrofechaend = filtroenddate.ToShortDateString();
                    //*****************

                    List<Routes_calendar> rutaslst = new List<Routes_calendar>();

                    foreach (var item in rutas)
                    {
                        Routes_calendar rt = new Routes_calendar();

                        rt.title = item.ID_route + " - " + item.query2.ToUpper();
                        rt.url = "";
                        rt.start = item.date.ToString("yyyy-MM-dd");
                        rt.end = item.end_date.AddDays(1).ToString("yyyy-MM-dd");
                        if (item.porcentaje == 0)
                        {
                            rt.className = "block b-t b-t-2x b-warning";//"#2081d6";
                        }
                        else if (item.porcentaje > 1 && item.porcentaje < 99)
                        {
                            rt.className = "block b-t b-t-2x b-success";//"#2081d6";
                        }
                        else if (item.porcentaje > 99)
                        {
                            rt.className = "block b-t b-t-2x b-info";//"#2081d6";
                        }

                        rt.porcentaje = item.porcentaje;
                        rutaslst.Add(rt);
                    }
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string result = javaScriptSerializer.Serialize(rutaslst.ToArray());
                    ViewBag.calroutes = result;



                }

                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        List<OCRD> customers;
                        //if (escliente == true)
                        //{

                        customers = (from b in CMKdb.OCRD where ((b.CardCode == customersel || b.U_CardCodeDLI == customersel) && b.Series == 61) select b).ToList();


                        ViewBag.customerssel = customers.ToList();

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.CustomerSelCode = "0";
                        }
                        else
                        {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                        var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                        if (brandsel == null || brandsel == 0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else
                        {
                            ViewBag.BrandLabel = brandcmk.FirmName;
                            //ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }

                catch
                {

                    ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
                }

                return View(rutas.ToList());
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }


        //CABE MENCIONAR QUE CON ESTOS DOS METODOS SE RELACIONA EL PADRE POR LA CARACTERISTICA TEXT LO CUAL VIENE SIENDO EQUIVALENTE
        //AL NOMBRE O DESCRIPCION DEL ITEM. ESTO SE HIZO POR LA RELACION EN LA BASE DE DATOS QUE ES PARA RUTAS, PERO EN TEORIA TENDRIA QUE SER POR ID Y NO POR NAME
        public static List<MyObj_tablapadreRutas> ObtenerCategoriarJerarquiaByNameRutas(List<MyObj_tablapadreRutas> Categoriaspadre, List<tablahijospadreRutas> categoriashijas)
        {


            List<MyObj_tablapadreRutas> query = (from item in Categoriaspadre

                                                 select new MyObj_tablapadreRutas
                                                 {
                                                     id = "", //SI QUEREMOS AGRUPAR POR ID SE LO PONEMOS, SINO SE LO QUITAMOS PARA QUE NOS CARGUE LAS TIENDAS DESPLEGADAS
                                                     text = item.text.Replace("'", ""),
                                                     children = ObtenerHijosRutas(item.text, categoriashijas)
                                                 }).ToList();

            return query;





        }

        public static List<MyObj_tablapadreRutas> ObtenerCategoriarJerarquiaByIDRutas(List<MyObj_tablapadreRutas> Categoriaspadre, List<tablahijospadreRutas> categoriashijas)
        {


            List<MyObj_tablapadreRutas> query = (from item in Categoriaspadre

                                                 select new MyObj_tablapadreRutas
                                                 {
                                                     id = "", //SI QUEREMOS AGRUPAR POR ID SE LO PONEMOS, SINO SE LO QUITAMOS PARA QUE NOS CARGUE LAS TIENDAS DESPLEGADAS
                                                     text = item.text.Replace("'", ""),
                                                     children = ObtenerHijosRutas(item.id, categoriashijas)
                                                 }).ToList();

            return query;





        }

        private static List<MyObj_tablapadreRutas> ObtenerHijosRutas(string Categoria, List<tablahijospadreRutas> categoriashijas)
        {



            List<MyObj_tablapadreRutas> query = (from item in categoriashijas

                                                 where item.parent == Categoria
                                                 select new MyObj_tablapadreRutas
                                                 {
                                                     id = item.id,
                                                     text = item.text.Replace("'", ""),
                                                     children = null
                                                 }).ToList();

            return query;

        }

        public class SimpleImagesForRoutes
        {
            public int ID_activity { get; set; }
            public string url { get; set; }
            public int id_visit { get; set; }


        }
        public ActionResult Map_customers(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Map";
                ViewBag.menunameid = "marketing_menu";
                ViewBag.submenunameid = "";
                //List<string> d = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstDepartments = JsonConvert.SerializeObject(d);
                //List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstRoles = JsonConvert.SerializeObject(r);

                //ViewData["nameUser"] = 
                ////NOTIFICATIONS
                //DateTime now = DateTime.Today;
                //List<Tb_Alerts> lstAlerts = (from a in dblim.Tb_Alerts where (a.ID_user == activeuser.ID_User && a.Active == true && a.Date == now) select a).OrderByDescending(x => x.Date).Take(5).ToList();
                //ViewBag.lstAlerts = lstAlerts;

                //FIN HEADER
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();
                ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
                ViewBag.id_customer = customersel;

                ArrayList myArrList = new ArrayList();
                List<VisitsInfoCalendar> lstVisitas;
                using (var db = new dbComerciaEntities())
                {

                    if (customersel == null || customersel == "" || customersel == "0")
                    {
                        lstVisitas = (from a in db.ActivitiesM
                                      join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                      where (a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished == true)
                                      //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                      select new VisitsInfoCalendar
                                      {
                                          ID_visit = a.ID_visit,
                                          ID_store = b.ID_store,
                                          idroute = b.ID_route,
                                          Brand="",
                                          visitDate = b.visit_date,
                                          ID_customer = a.ID_customer,
                                          ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault()
                                      }).ToList();

                    }
                    else
                    {
                        lstVisitas = (from a in db.ActivitiesM
                                      join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                      where (a.ID_customer == customersel && (a.date >= filtrostartdate && a.date <= filtroenddate) && a.isfinished == true)
                                      //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                      select new VisitsInfoCalendar
                                      {
                                          ID_visit = a.ID_visit,
                                          ID_store = b.ID_store,
                                          idroute = b.ID_route,
                                          visitDate = b.visit_date,
                                          ID_customer = a.ID_customer,
                                          ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                          Brand = ""
                                      }).ToList();
                        if (brandsel == null || brandsel == 0)
                        {

                        }
                        else
                        {
                            var brandstring = brandsel.ToString();
                            lstVisitas = lstVisitas.Where(c => c.ID_brand == brandstring).ToList();
                        }

                    }

                    var lstvisits = lstVisitas.Select(b => b.ID_visit).Distinct().ToArray();

                    var rutas = db.VisitsM.Where(c => lstvisits.Contains(c.ID_visit)).OrderBy(d => d.visit_date).ToList();

                    myArrList.AddRange((from p in rutas
                                        select new
                                        {
                                            id = p.ID_visit,
                                            representatives = p.city,
                                            store = p.store,
                                            address = p.address,
                                            GeoLong = p.geoLong,
                                            GeoLat = p.geoLat,
                                            demo_state = p.ID_visitstate,
                                            customer = p.customer,
                                            date = p.visit_date.ToLongDateString(),
                                            comment = p.comments
                                        }).ToList());








                }
                ViewBag.routes_map = JsonConvert.SerializeObject(myArrList);

                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        List<OCRD> customers;
                        //if (escliente == true)
                        //{

                        customers = (from b in CMKdb.OCRD where ((b.CardCode == customersel || b.U_CardCodeDLI == customersel) && b.Series == 61) select b).ToList();


                        ViewBag.customerssel = customers.ToList();

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.CustomerSelCode = "0";
                        }
                        else
                        {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                        var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                        if (brandsel == null || brandsel == 0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else
                        {
                            ViewBag.BrandLabel = brandcmk.FirmName;
                            
                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }

                catch
                {

                    ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
                }

                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }

        public ActionResult GetImagesVisit(string id_customer, string idvisit)
        {
            try
            {   
                    int IDV = Convert.ToInt32(idvisit);
                using (var db = new dbComerciaEntities())
                {
                    var lstimages = (from t2 in db.FormsM_details
                                     join t3 in db.ActivitiesM on t2.ID_visit equals t3.ID_activity
          
                                     where (t2.ID_formresourcetype == 5 && t3.ID_customer == id_customer && t3.ID_visit == IDV && t2.fsource.StartsWith("~/Content/"))
                                     select new SimpleImagesForRoutes { ID_activity = t3.ID_activity, url = t2.fsource, id_visit=t3.ID_visit
                                          }
                                 ).ToList();

                    foreach (var item in lstimages) {
                        item.url = Url.Content(item.url);
                    }

                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string result = javaScriptSerializer.Serialize(lstimages);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
 
                
            }
            catch (Exception ex)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
           

        }

        public ActionResult Reports_customers(string id, string fstartd, string fendd, string stores, string brands, string spartners)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Reports";
                ViewBag.menunameid = "marketing_menu";
                ViewBag.submenunameid = "";
                //List<string> d = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstDepartments = JsonConvert.SerializeObject(d);
                //List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstRoles = JsonConvert.SerializeObject(r);

                //ViewData["nameUser"] = 
                ////NOTIFICATIONS
                //DateTime now = DateTime.Today;
                //List<Tb_Alerts> lstAlerts = (from a in dblim.Tb_Alerts where (a.ID_user == activeuser.ID_User && a.Active == true && a.Date == now) select a).OrderByDescending(x => x.Date).Take(5).ToList();
                //ViewBag.lstAlerts = lstAlerts;

                //FIN HEADER
                ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
                ViewBag.id_customer = id;

                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                List<activitiesTypes> distinct = new List<activitiesTypes>();
                int contf = 0;
                List<activitiesVisitsBrands> getActivities = Session["getActivities"] as List<activitiesVisitsBrands>;

                if (getActivities != null && getActivities.Count > 0)
                {
                    contf = getActivities.Count();
                    distinct = (from b in getActivities
                                select new activitiesTypes
                                {
                                    id = b.ID_form,
                                    name = b.formName,
                                    activitytype = b.ActivityName,
                                    count = getActivities
    .Where(x => x.ID_form == b.ID_form)
    .Select(x => x).Count()
                                }).GroupBy(c => c.id).Select(c => c.First()).ToList();
                }
                else
                {
                    using (var db = new dbComerciaEntities())
                    {



                        var lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                            
                                             where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                             select new activitiesVisitsBrands
                                             {
                                                 ID_activity = a.ID_activity,
                                                 ID_visit = a.ID_visit,
                                                 ID_form = a.ID_form,
                                                 formName = a.description,
                                                 ID_store = b.ID_store,
                                                 store = b.store,
                                                 visitDate = b.visit_date,
                                                 ID_customer = a.ID_customer,
                                                 Customer = a.Customer,
                                                 isfinished = a.isfinished,
                                                 id_usuarioend = a.ID_usuarioEnd,
                                                 id_usuarioendexternal = a.ID_usuarioEndString,
                                                 id_activitytype = a.ID_activitytype,
                                                 ActivityName = "",
                                                 Comments = a.comments,
                                                 ID_brand = "",
                                                 Brand = "Brand test",
                                                 count = 0
                                             }).Take(10);
                        contf = lstActivities.Count();
                        distinct = (from b in lstActivities
                                    select new activitiesTypes
                                    {
                                        id = b.ID_form,
                                        name = b.formName,
                                        activitytype = b.ActivityName,
                                        count = lstActivities
        .Where(x => x.ID_form == b.ID_form)
        .Select(x => x).Count()
                                    }).Distinct().ToList();
                        Session["getActivities"] = lstActivities.ToList();
                    }
                }


                ViewBag.lstTypes = distinct;
                ViewBag.lstCount = contf;

                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }
        public ActionResult Reports_details(string id, int report, string fstartd, string fendd, string stores, string brands, string spartners)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Reports";
                ViewBag.menunameid = "marketing_menu";
                ViewBag.submenunameid = "";
                //List<string> d = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstDepartments = JsonConvert.SerializeObject(d);
                //List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                //ViewBag.lstRoles = JsonConvert.SerializeObject(r);

                //ViewData["nameUser"] = 
                ////NOTIFICATIONS
                //DateTime now = DateTime.Today;
                //List<Tb_Alerts> lstAlerts = (from a in dblim.Tb_Alerts where (a.ID_user == activeuser.ID_User && a.Active == true && a.Date == now) select a).OrderByDescending(x => x.Date).Take(5).ToList();
                //ViewBag.lstAlerts = lstAlerts;

                //FIN HEADER


                ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
                ViewBag.id_customer = id;
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //

                List<activitiesVisitsBrands> getActivities = Session["getActivities"] as List<activitiesVisitsBrands>;

                if (getActivities.Count == 0)
                {
                    return RedirectToAction("Reports_customers", "Customers", new { id = id });
                }

                ViewBag.ActivityName = getActivities.Where(a => a.ID_form == report).FirstOrDefault().formName;
                ViewBag.ActivityType = getActivities.Where(a => a.ID_form == report).FirstOrDefault().ActivityName;
                ViewBag.lstActivities = getActivities.Where(a => a.ID_form == report).ToList();



                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }
        public class MyObj_formtemplate
        {
            public string id { get; set; }
            public string text { get; set; }
            public string value { get; set; }
        }

        public DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public DataSet getDataSetExportToExcel(string id, List<int> lstact)
        {
            List<QuickVisit_export> activity_header = new List<QuickVisit_export>();
            IQueryable<FormsM_details> details;

            //using (var dbs = new dbComerciaEntities())
            //{
                
            //}

            using (var db = new dbComerciaEntities())
            {
                details = (from a in db.FormsM_details where (lstact.Contains(a.ID_visit) && (a.ID_formresourcetype==13 || a.ID_formresourcetype==5 || a.ID_formresourcetype==6)) select a);

                activity_header = (from a in db.ActivitiesM
                                   join b in db.VisitsM on a.ID_visit equals b.ID_visit

                                   //where (a.ID_customer == id && lstact.Contains(a.ID_activity))
                                   where (lstact.Contains(a.ID_activity))
                                   select new QuickVisit_export
                                   {
                                       ID = a.ID_activity,
                                       Store = b.store,
                                       Address = b.address,
                                       City = b.city,
                                       ZipCode = b.zipcode,
                                       State = b.state,
                                       date = b.visit_date,
                                       //SalesRepresentative = a.ID_customer,
                                       //Brand = "",
                                       Brand = details.Where(c=>c.ID_visit == a.ID_activity && c.ID_formresourcetype==13).Select(c=>c.fdescription).FirstOrDefault(),//  (from det in details where (det.ID_visit == a.ID_activity && det.ID_formresourcetype == 13) select det.fdescription).SingleOrDefault(),
                                       Activity = a.description,
                                       //PictureBefore = "",
                                       //PictureAfter = "",
                                       //CommentBefore = "",
                                       //CommentAfter = ""
                                       PictureBefore = (from b in details where (b.ID_visit == a.ID_activity && b.ID_formresourcetype == 5 && b.fdescription.Contains("PICTURE 1")) select b.fsource).FirstOrDefault(),
                                       CommentBefore = (from b in details where (b.ID_visit == a.ID_activity && b.ID_formresourcetype == 6 && b.fdescription.Contains("COMMENT 1")) select b.fsource).FirstOrDefault(),
                                       PictureAfter = (from b in details where (b.ID_visit == a.ID_activity && b.ID_formresourcetype == 5 && b.fdescription.Contains("PICTURE 2")) select b.fsource).FirstOrDefault(),
                                       CommentAfter = (from b in details where (b.ID_visit == a.ID_activity && b.ID_formresourcetype == 6 && b.fdescription.Contains("COMMENT 2")) select b.fsource).FirstOrDefault()


                                   }).ToList();
                //form_details = (from c in db.FormsM_details where (lstact.Contains(c.ID_visit)) select c).ToList();



            }

            DataSet ds = new DataSet();
            DataTable dtEmp = new DataTable("ActivitiesData");
            dtEmp = ToDataTable(activity_header);
            //DataTable dtEmpOrder = new DataTable("ActivitiesDetails");  
            //dtEmpOrder = ToDataTable(form_details);

            ds.Tables.Add(dtEmp);
            //ds.Tables.Add(dtEmpOrder);
            return ds;
        }

        public DataSet getDataSetExportToExcel_RD_FORM3(string id, List<int?> lstact)
        {
            List<Estructura_1_1_Comercia> activity_header = new List<Estructura_1_1_Comercia>();

            //using (var dbs = new dbComerciaEntities())
            //{

            //}

            using (var db = new Interna_CMKEntities())
            {

                activity_header = (from a in db.Estructura_1_1_Comercia
                                   where (lstact.Contains(a.Id_Visit)) select a) //Utilizar esto, lo quite por prueba
                                  
                                  // select a).Take(1000) //para pruebas
                                   .ToList();
  



            }

            DataSet ds = new DataSet();
            DataTable dtEmp = new DataTable("ActivitiesData");
            dtEmp = ToDataTable(activity_header);
            //DataTable dtEmpOrder = new DataTable("ActivitiesDetails");  
            //dtEmpOrder = ToDataTable(form_details);

            ds.Tables.Add(dtEmp);
            //ds.Tables.Add(dtEmpOrder);
            return ds;
        }
        public DataSet getDataSetExportToExcel_RD_FORM42(string id, List<int?> lstact)
        {
            List<Visita_2_0> activity_header = new List<Visita_2_0>();

            //using (var dbs = new dbComerciaEntities())
            //{

            //}

            using (var db = new Interna_CMKEntities())
            {

                activity_header = (from a in db.Visita_2_0
                                   where (lstact.Contains(a.Id_Visit))
                                   select a) //Utilizar esto, lo quite por prueba

                                   // select a).Take(1000) //para pruebas
                                   .ToList();




            }

            DataSet ds = new DataSet();
            DataTable dtEmp = new DataTable("ActivitiesData");
            dtEmp = ToDataTable(activity_header);
            //DataTable dtEmpOrder = new DataTable("ActivitiesDetails");  
            //dtEmpOrder = ToDataTable(form_details);

            ds.Tables.Add(dtEmp);
            //ds.Tables.Add(dtEmpOrder);
            return ds;
        }

        public DataSet getDataSetExportToExcel_RD_FORM32(string id, List<int?> lstact)
        {
            List<Datos_Marketing_Activity> activity_header = new List<Datos_Marketing_Activity>();

            //using (var dbs = new dbComerciaEntities())
            //{

            //}

            using (var db = new Interna_CMKEntities())
            {

                activity_header = (from a in db.Datos_Marketing_Activity
                                   where (lstact.Contains(a.Id_Visit))
                                   select a) //Utilizar esto, lo quite por prueba

                                   // select a).Take(1000) //para pruebas
                                   .ToList();




            }

            DataSet ds = new DataSet();
            DataTable dtEmp = new DataTable("ActivitiesData");
            dtEmp = ToDataTable(activity_header);
            //DataTable dtEmpOrder = new DataTable("ActivitiesDetails");  
            //dtEmpOrder = ToDataTable(form_details);

            ds.Tables.Add(dtEmp);
            //ds.Tables.Add(dtEmpOrder);
            return ds;
        }
        public DataSet getDataSetExportToExcel_RD_AUDITGRL1(string id, List<int?> lstact)
        {
            List<Datos_Audit_11> activity_header = new List<Datos_Audit_11>();

            //using (var dbs = new dbComerciaEntities())
            //{

            //}

            using (var db = new Interna_CMKEntities())
            {

                activity_header = (from a in db.Datos_Audit_11
                                       where (lstact.Contains(a.Id_Visit)) select a) //Utilizar esto, lo quite por prueba

                                   //select a).Take(1000) // pruebas
                                   .ToList();




            }

            DataSet ds = new DataSet();
            DataTable dtEmp = new DataTable("ActivitiesData");
            dtEmp = ToDataTable(activity_header);
            //DataTable dtEmpOrder = new DataTable("ActivitiesDetails");  
            //dtEmpOrder = ToDataTable(form_details);

            ds.Tables.Add(dtEmp);
            //ds.Tables.Add(dtEmpOrder);
            return ds;
        }

        public DataSet getDataSetExportToExcel_RD_AUDITGRL2(string id, List<int?> lstact)
        {
            List<Datos_Audit_2> activity_header = new List<Datos_Audit_2>();

            //using (var dbs = new dbComerciaEntities())
            //{

            //}

            using (var db = new Interna_CMKEntities())
            {

                activity_header = (from a in db.Datos_Audit_2
                                   where (lstact.Contains(a.Id_Visit))
                                   select a) //Utilizar esto, lo quite por prueba

                                   //select a).Take(1000) // pruebas
                                   .ToList();
            }

            DataSet ds = new DataSet();
            DataTable dtEmp = new DataTable("ActivitiesData");
            dtEmp = ToDataTable(activity_header);
            //DataTable dtEmpOrder = new DataTable("ActivitiesDetails");  
            //dtEmpOrder = ToDataTable(form_details);

            ds.Tables.Add(dtEmp);
            //ds.Tables.Add(dtEmpOrder);
            return ds;
        }

        public ActionResult Export_QuickVisit(string id, List<MyObj_formtemplate> objects)
        {
            List<int> list = new List<int>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            //UTILIZANDO LIBRERIA 
            DataSet ds = getDataSetExportToExcel(id, list);
            var filename = "QUICK_VISIT" + "" + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Worksheet(1).Name = "ActivitiesData";
                //wb.Worksheet(2).Name = "ActivitiesDetails";
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                var filePathOriginal = Server.MapPath("/Reportes/excel");
                var path2 = "";
                path2 = Path.Combine(filePathOriginal, filename);
                wb.SaveAs(path2);
            }
            return Json(filename, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [DeleteFileAttribute] //Action Filter, it will auto delete the file after download, 
                              //I will explain it later
        public ActionResult Download(string file)
        {
            //get the temp folder and file path in server
            string fullPath = Path.Combine(Server.MapPath("/Reportes/excel"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file);
        }

        public class DeleteFileAttribute : ActionFilterAttribute
        {
            public override void OnResultExecuted(ResultExecutedContext filterContext)
            {
                filterContext.HttpContext.Response.Flush();

                //convert the current filter context to file and get the file path
                string filePath = (filterContext.Result as FilePathResult).FileName;

                //delete the file after download
                System.IO.File.Delete(filePath);
            }
        }
        public ActionResult Print_QuickVisit(string id, List<MyObj_formtemplate> objects)
        {
            List<int> list = new List<int>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }


            List<QuickVisit_report> activity_header = new List<QuickVisit_report>();
            IQueryable<FormsM_details> form_details;
            using (var db = new dbComerciaEntities())
            {
                form_details = (from c in db.FormsM_details where (list.Contains(c.ID_visit)) select c);
                activity_header = (from a in db.ActivitiesM
                                   join b in db.VisitsM on a.ID_visit equals b.ID_visit

                                   where (list.Contains(a.ID_activity))
                                   select new QuickVisit_report
                                   {
                                       ID_activity = a.ID_activity,
                                       ID_visit = a.ID_visit,
                                       ID_form = a.ID_form,
                                       formName = a.description,
                                       ID_store = b.ID_store,
                                       store = b.store + ", " + b.address + ", " + b.city + ", " + b.state + ", " + b.zipcode,
                                       visitDate = b.visit_date,
                                       ID_customer = a.ID_customer,
                                       Customer = a.Customer,
                                       isfinished = a.isfinished,
                                       id_usuarioend = a.ID_usuarioEnd,
                                       id_usuarioendexternal = a.ID_usuarioEndString,
                                       id_activitytype = a.ID_activitytype,
                                       ActivityName = "",
                                       Comments = a.comments,
                                       ID_brand = "",
                                       Brand = (from detalle in form_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                       count = 0,
                                       urlimg1 = "",
                                       urlimg2 = "",
                                       urlsign = ""

                                   }).ToList();





                if (activity_header.Count > 0)
                {
                    foreach (var item in activity_header)
                    {
                        //Verificamos si existen fotos en el demo (MAX 2 fotos QUICK VISIT)
                        var fotos = (from c in form_details where (c.ID_formresourcetype == 5 && c.ID_visit == item.ID_activity) select c).ToList();

                        int fotosC = fotos.Count();


                        if (fotosC == 2)
                        {
                            if (fotos[0].fsource == "")
                            {

                            }
                            else
                            {
                                item.urlimg1 = Path.GetFullPath(Server.MapPath(fotos[0].fsource));
                            }
                            if (fotos[1].fsource == "")
                            {

                            }
                            else
                            {
                                item.urlimg2 = Path.GetFullPath(Server.MapPath(fotos[1].fsource));
                            }

                        }
                        else if (fotosC == 1)
                        {
                            if (fotos[0].fsource == "")
                            {

                            }
                            else
                            {
                                item.urlimg1 = Path.GetFullPath(Server.MapPath(fotos[0].fsource));
                            }


                        }

                    }
                }


            }


            if (activity_header.Count > 0)

            {
                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptQuick_visit.rpt"));

                rd.SetDataSource(activity_header);




                ////Verificams si existe firma electronica
                //var firma = (from d in form_details where (d.ID_formresourcetype == 9) select d).ToList();

                //int firmaC = firma.Count();




                //if (firmaC == 1)
                //{

                //    string data = firma[0].fsource;
                //    if (data != "")
                //    {
                //        var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                //        var binData = Convert.FromBase64String(base64Data);

                //        using (var streamf = new MemoryStream(binData))
                //        {

                //            Bitmap myImage = new Bitmap(streamf);

                //            // Assumes myImage is the PNG you are converting
                //            using (var b = new Bitmap(myImage.Width, myImage.Height))
                //            {
                //                b.SetResolution(myImage.HorizontalResolution, myImage.VerticalResolution);

                //                using (var g = Graphics.FromImage(b))
                //                {
                //                    g.Clear(Color.White);
                //                    g.DrawImageUnscaled(myImage, 0, 0);
                //                }

                //                // Now save b as a JPEG like you normally would

                //                var path = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), "signdequickvisit.jpg");
                //                b.Save(path, ImageFormat.Jpeg);


                //                rd.SetParameterValue("urlimgsign", Path.GetFullPath(path));
                //            }



                //        }
                //    }
                //    else
                //    {
                //        rd.SetParameterValue("urlimgsign", "");

                //    }

                //}
                //else
                //{
                //    rd.SetParameterValue("urlimgsign", "");
                //}


                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                Response.AppendHeader("Content-Disposition", "inline; filename=Quick Visit.pdf;");



                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);


                //PARA PREVISULIZACION
                //return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);



                //try

                //{
                //    //limpiamos el directorio

                //    System.IO.DirectoryInfo di = new DirectoryInfo(filePathOriginal);

                //    foreach (FileInfo file in di.GetFiles())

                //    {

                //        file.Delete();

                //    }

                //    foreach (DirectoryInfo dir in di.GetDirectories())

                //    {

                //        dir.Delete(true);

                //    }

                //}

                //catch (Exception e)

                //{

                //    var mensaje = e.ToString();

                //}

                //var path2 = "";
                //var filename = "QUICK VISIT" + "" + ".pdf";
                //path2 = Path.Combine(filePathOriginal, filename);
                //rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path2);

                //var urlcontent = Url.Content("~/Reportes/pdf/" + filename + "");
                //Nueva descarga
                TempData["Output"] = stream;

                //return Json(urlcontent);
                return Json("Success", JsonRequestBehavior.AllowGet);

            }
            else
            {

                return Json("Error", JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Print_VisitReport(string id, List<MyObj_formtemplate> objects)
        {
            List<int> list = new List<int>();
            foreach (var item in objects) {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            List<VisitsInfo> detailsForm = Session["VisitsInfo"] as List<VisitsInfo>;

            if (detailsForm.Count > 0)

            {
                detailsForm = detailsForm.Where(c => list.Contains(c.ID_visit)).OrderBy(c => c.visitDate).ToList();

                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptVisitsList.rpt"));

                rd.SetDataSource(detailsForm);

                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                Response.AppendHeader("Content-Disposition", "inline; filename=Visit Report.pdf;");
       

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);

                
                //Nueva descarga
                TempData["Output"] = stream;

                //return Json(urlcontent);
                return Json("Success", JsonRequestBehavior.AllowGet);

            }
            else
            {

                return Json("Error", JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Print_VisitReportImg(string id, List<MyObj_formtemplate> objects)
        {
            List<int> list = new List<int>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            List<VisitsInfo> detailsForm = Session["VisitsInfo"] as List<VisitsInfo>;

            if (detailsForm.Count > 0)

            {



                detailsForm = detailsForm.Where(c=> list.Contains(c.ID_visit)).OrderBy(c => c.visitDate).ToList();

                using (var db = new dbComerciaEntities())
                {

                    foreach (var visit in detailsForm)
                    {
                        var activity = (from a in db.ActivitiesM
                                        join b in db.FormsM_details on a.ID_activity equals b.ID_visit
                                        where (a.ID_visit == visit.ID_visit && (b.ID_formM == 1 || b.ID_formM == 3 || b.ID_formM == 42) && b.ID_formresourcetype == 13 && b.fvalueText == visit.ID_brand)
                                        select b.ID_visit).FirstOrDefault();

                        var activitydetails = (from d in db.FormsM_details where (d.ID_visit == activity) select d).ToList();

                        if (activitydetails.Count > 0)
                        {
                            var pic1 = activitydetails.Where(c => c.ID_formresourcetype == 5 && (c.fdescription.Contains("Tomar fotografia inicial") || c.fdescription.Contains("PICTURE 1") || c.fdescription.Contains("Fotografia inicial"))).Select(c => c.fsource).FirstOrDefault();
                            var pic2 = activitydetails.Where(c => c.ID_formresourcetype == 5 && (c.fdescription.Contains("CONDICION FINAL DE LA TIENDA (DESPUES)") || c.fdescription.Contains("PICTURE 2") || c.fdescription.Contains("Foto final 1"))).Select(c => c.fsource).FirstOrDefault();

                            try
                            {
                                visit.pictureBefore = Path.GetFullPath(Server.MapPath(pic1));
                                visit.pictureAfter = Path.GetFullPath(Server.MapPath(pic2));
                            }
                            catch {
                                visit.pictureBefore = "";
                                visit.pictureAfter = "";
                            }
                          
                        }

                    }

                }

                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptVisitsListImg.rpt"));

                rd.SetDataSource(detailsForm);

                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                Response.AppendHeader("Content-Disposition", "inline; filename=Visit Report.pdf;");


                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);


                //Nueva descarga
                TempData["Output"] = stream;

                //return Json(urlcontent);
                return Json("Success", JsonRequestBehavior.AllowGet);

            }
            else
            {

                return Json("Error", JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult DownloadPDF(string activityname)
        {
            // retrieve byte array here
            var report = TempData["Output"] as Stream;
            if (report != null)
            {
                return File(report, System.Net.Mime.MediaTypeNames.Application.Pdf, activityname +  ".pdf");
            }
            else
            {
                return new EmptyResult();
            }
        }

        public class MyObj_formtemplateMarketingReport
        {
            public string id { get; set; }
            public string text { get; set; }
            public string value { get; set; }
        }



        public ActionResult Print_RD_FORM32(List<MyObj_formtemplateMarketingReport> objects)
        {
            List<int> list = new List<int>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(Convert.ToInt32(idact));
            }

            IQueryable<FormsM_details> general_Details;
            using (var dbcmk = new dbComerciaEntities())
            {
                //Recuperamos los IDS de las demos en el dia especifico y del customer especifico
                int[] demo_ids = list.ToArray();

                general_Details = (from b in dbcmk.FormsM_details where (demo_ids.Contains(b.ID_visit)) select b);

                var total_demos = (from a in dbcmk.ActivitiesM
                                   join b in dbcmk.VisitsM on a.ID_visit equals b.ID_visit


                                   where (list.Contains(a.ID_activity))
                                   //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                   select new ActivitiesReport
                                   {
                                       ID_Activity = a.ID_activity,
                                       Description = a.description,
                                       Brand = (from detalle in general_Details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                       Customer = a.Customer,
                                       ID_Customer = a.ID_customer,
                                       ID_Store = b.ID_store,
                                       Store = b.store,
                                       Address = b.address + b.city + b.state,
                                       City = b.city,
                                       state = b.state,
                                       date = a.date,
                                       ID_SalesRep = 0,
                                       SalesRep = "",
                                       Category = (from detalle in general_Details where (detalle.ID_formresourcetype == 30 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                       SubCategories = (from detalle in general_Details where (detalle.ID_formresourcetype == 31 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault()

                                   }).ToList();

                if (total_demos.Count > 0)
                {
                   

                    //Existen datos
                    //Buscamos los detalles

                    var demo_details_items = (from b in general_Details where (demo_ids.Contains(b.ID_visit) && (b.ID_formresourcetype == 5)) select b).OrderBy(b => b.ID_formresourcetype).ToList();

                    foreach (var item in demo_details_items)
                    {
                        item.fsource = Path.GetFullPath(Server.MapPath(item.fsource));
                    }
                    var result = demo_details_items;


                    if (result.Count > 0)

                    {


                        ReportDocument rd = new ReportDocument();

                        rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptMarketingActivity.rpt"));



                        rd.SetDataSource(total_demos);

                        rd.Subreports[0].SetDataSource(result);
                        // rd.Subreports[1].SetDataSource(total_demos);

                        var filePathOriginal = Server.MapPath("/Reportes/pdf");

                        Response.Buffer = false;

                        Response.ClearContent();

                        Response.ClearHeaders();


                        //PARA VISUALIZAR
                        Response.AppendHeader("Content-Disposition", "inline; filename=" + "Marketing Activity.pdf; ");



                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                        stream.Seek(0, SeekOrigin.Begin);



                        //return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

                        //try

                        //{
                        //    //limpiamos el directorio

                        //    System.IO.DirectoryInfo di = new DirectoryInfo(filePathOriginal);

                        //    foreach (FileInfo file in di.GetFiles())

                        //    {

                        //        file.Delete();

                        //    }

                        //    foreach (DirectoryInfo dir in di.GetDirectories())

                        //    {

                        //        dir.Delete(true);

                        //    }

                        //}

                        //catch (Exception e)

                        //{

                        //    var mensaje = e.ToString();

                        //}

                        //var path2 = "";
                        //var filename = "MarketingActivity" + "" + ".pdf";
                        //path2 = Path.Combine(filePathOriginal, filename);
                        //rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path2);

                        //var urlcontent = Url.Content("~/Reportes/pdf/" + filename + "");
                        ////var other = Path.Combine(Server.MapPath("~/Reports"), "rptMarketingActivity.rpt"));
                        //return Json(urlcontent);

                        TempData["Output"] = stream;

                        //return Json(urlcontent);
                        return Json("Success", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Error", JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                  
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public ActionResult DownloadZip(string id, List<MyObj_formtemplate> objects)
        {

            List<int> list = new List<int>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            List<FormsM_details> detalles = new List<FormsM_details>();
            List<FileModel> files = new List<FileModel>();

            using (var db = new dbComerciaEntities())
            {
                detalles = (from c in db.FormsM_details where (list.Contains(c.ID_visit) && c.ID_formresourcetype==5) select c).ToList();
            }

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
            string zipName = "";
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
                zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd"));
    
                    var filePathOriginal = Server.MapPath("/Reportes/excel");
                    var path2 = "";
                    path2 = Path.Combine(filePathOriginal, zipName);
                    zip.Save(path2);

                
            }

            return Json(zipName, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Export_RD_FORM3(string id, List<MyObj_formtemplate> objects)
        {
            List<int?> list = new List<int?>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            //UTILIZANDO LIBRERIA 
            DataSet ds = getDataSetExportToExcel_RD_FORM3(id, list);
            var filename = "VISITA_1.1" + "" + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Worksheet(1).Name = "ActivitiesData";
                //wb.Worksheet(2).Name = "ActivitiesDetails";
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                var filePathOriginal = Server.MapPath("/Reportes/excel");
                var path2 = "";
                path2 = Path.Combine(filePathOriginal, filename);
                wb.SaveAs(path2);
            }
            return Json(filename, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export_RD_FORM42(string id, List<MyObj_formtemplate> objects)
        {
            List<int?> list = new List<int?>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            //UTILIZANDO LIBRERIA 
            DataSet ds = getDataSetExportToExcel_RD_FORM42(id, list);
            var filename = "VISITA_2.0" + "" + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Worksheet(1).Name = "ActivitiesData";
                //wb.Worksheet(2).Name = "ActivitiesDetails";
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                var filePathOriginal = Server.MapPath("/Reportes/excel");
                var path2 = "";
                path2 = Path.Combine(filePathOriginal, filename);
                wb.SaveAs(path2);
            }
            return Json(filename, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Export_RD_FORM32(string id, List<MyObj_formtemplate> objects)
        {
            List<int?> list = new List<int?>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            //UTILIZANDO LIBRERIA 
            DataSet ds = getDataSetExportToExcel_RD_FORM32(id, list);
            var filename = "Marketing_Activities" + "" + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Worksheet(1).Name = "ActivitiesData";
                //wb.Worksheet(2).Name = "ActivitiesDetails";
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                var filePathOriginal = Server.MapPath("/Reportes/excel");
                var path2 = "";
                path2 = Path.Combine(filePathOriginal, filename);
                wb.SaveAs(path2);
            }
            return Json(filename, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export_RD_AUDITGRL1(string id, List<MyObj_formtemplate> objects)
        {
            List<int?> list = new List<int?>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            //UTILIZANDO LIBRERIA 
            DataSet ds = getDataSetExportToExcel_RD_AUDITGRL1(id, list);
            var filename = "AUDIT_REPORT" + "" + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Worksheet(1).Name = "ActivitiesData";
                //wb.Worksheet(2).Name = "ActivitiesDetails";
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                var filePathOriginal = Server.MapPath("/Reportes/excel");
                var path2 = "";
                path2 = Path.Combine(filePathOriginal, filename);
                wb.SaveAs(path2);
            }
            return Json(filename, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Export_RD_AUDITGRL2(string id, List<MyObj_formtemplate> objects)
        {
            List<int?> list = new List<int?>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            //UTILIZANDO LIBRERIA 
            DataSet ds = getDataSetExportToExcel_RD_AUDITGRL2(id, list);
            var filename = "AUDIT_REPORT2" + "" + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Worksheet(1).Name = "ActivitiesData";
                //wb.Worksheet(2).Name = "ActivitiesDetails";
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                var filePathOriginal = Server.MapPath("/Reportes/excel");
                var path2 = "";
                path2 = Path.Combine(filePathOriginal, filename);
                wb.SaveAs(path2);
            }
            return Json(filename, JsonRequestBehavior.AllowGet);
        }

    }
}