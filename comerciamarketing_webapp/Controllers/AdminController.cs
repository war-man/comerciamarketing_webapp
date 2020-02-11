using comerciamarketing_webapp.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace comerciamarketing_webapp.Controllers
{
    public class AdminController : Controller
    {

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

        public class SimpleUserforRoutes
        {
            public int ID_user { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string img { get; set; }
            public int EstadoVisita  { get; set; }

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
            public List<SimpleUserforRoutes> lstUsers { get; set; }
        }
        public class CustomVisit {
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
            public List<SimpleUserWithImg> lstUsers { get; set; }
            public List<brandsinroute> lstBrands { get; set; }
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

        public ActionResult Map(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
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
                ViewBag.id_customer = id;

                List<VisitsInfoCalendar> lstVisitas;
                ArrayList myArrList = new ArrayList();
                using (var db = new dbComerciaEntities())
                {

                    if (customersel == null || customersel == "" || customersel == "0")
                    {
                        lstVisitas = (from a in db.ActivitiesM
                                      join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                      where (a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished == true )
                                      //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                      select new VisitsInfoCalendar
                                      {
                                          ID_visit = a.ID_visit,
                                          ID_store = b.ID_store,
                                          idroute = b.ID_route,
                                          visitDate = b.visit_date,
                                          ID_customer = a.ID_customer,
                                          ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault()
                                      }).ToList();

                    }
                    else
                    {
                        lstVisitas = (from a in db.ActivitiesM
                                      join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                      where (a.ID_customer == customersel && (a.date >= filtrostartdate && a.date <= filtroenddate) && a.isfinished == true  )
                                      //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                      select new VisitsInfoCalendar
                                      {
                                          ID_visit = a.ID_visit,
                                          ID_store = b.ID_store,
                                          idroute = b.ID_route,
                                          visitDate = b.visit_date,
                                          ID_customer = a.ID_customer,
                                          ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault()
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
                        var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
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

        public ActionResult Gallery(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners, int? idvisit, int? idroute, string customersel)
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
                ViewBag.routesel = idroute;
                DateTime filtrostartdate;
                DateTime filtroenddate;
                //filtros de fecha
                //filtros de fecha //MENSUAL
                //var sunday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                //var saturday = sunday.AddMonths(1).AddDays(-1);
                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);
                //Verificamos si es marca cliente
                var escliente = false;
                if (activeuser.ID_tipomembresia == 2 || activeuser.ID_tipomembresia == 3 || activeuser.ID_tipomembresia == 4)
                {
                    escliente = true;
                    ViewBag.escliente = 1;
                }
                else
                {
                    ViewBag.escliente = 0;
                }



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
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                List<ImgGallery> detalles;
                IQueryable<ActivitiesM> actividadesList;
                try
                {
                    using (var db = new dbComerciaEntities())
                    {
                        var visitsRoute = (from f in db.VisitsM where (f.ID_route == idroute) select f.ID_visit).ToArray();

                        if (visitsRoute.Length==0) {
                            visitsRoute = (from f in db.VisitsM where ((f.visit_date >= filtrostartdate && f.end_date <= filtroenddate)) select f.ID_visit).ToArray();
                        }

                        if (customersel == null || customersel == "" || customersel =="0")
                        {
                            actividadesList = (from e in db.ActivitiesM where (visitsRoute.Contains(e.ID_visit) && e.isfinished==true) select e);
                        }
                        else {
                            actividadesList = (from e in db.ActivitiesM where (visitsRoute.Contains(e.ID_visit) && e.ID_customer==customersel && e.isfinished==true) select e);
                        }
                            
                        var actividades = actividadesList.Select(a => a.ID_activity).ToArray();

                        //var detalles = (from b in db.FormsM_details where (actividades.Contains(b.ID_visit) && b.ID_formresourcetype == 5) select b).ToList();

                        detalles = (from a in db.FormsM_details
                                        join b in actividadesList on a.ID_visit equals b.ID_activity
                                        join c in db.VisitsM on b.ID_visit equals c.ID_visit
                                        where (actividades.Contains(a.ID_visit) && a.ID_formresourcetype == 5)
                                        select new ImgGallery
                                        {
                                            idImg = a.ID_details,
                                            Customer = b.Customer,
                                            Activity = b.description,
                                            Url = a.fsource,
                                            IDStore =c.ID_store,
                                            Store = c.ID_store + " - " +c.store,
                                            Section = a.fdescription == "PICTURE 1" ? "BEFORE" : a.fdescription == "PICTURE 2" ? "AFTER" : a.fdescription == "CONDICION FINAL DE LA TIENDA (DESPUES)" ? "AFTER" : a.fdescription == "Tomar fotografia inicial" ? "BEFORE" : a.fdescription == "Foto de herramientas" ? "TOOLS" : a.fdescription == "Foto de la competencia" ? "TOOLS" : "OTHER",
                                            Rep = (from usu in db.Usuarios where (usu.ID_usuario == b.ID_usuarioEnd) select usu.nombre + " " + usu.apellido).FirstOrDefault() == null ?  "" : (from usu in db.Usuarios where (usu.ID_usuario == b.ID_usuarioEnd) select usu.nombre + " " + usu.apellido).FirstOrDefault(),
                                            IDBrand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == b.ID_activity) select detalle.fvalueText).FirstOrDefault(),
                                            Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == b.ID_activity) select detalle.fdescription).FirstOrDefault(),

                                        }).ToList();

                        detalles = detalles.Where(c => c.Url != null && !c.Url.Equals(string.Empty) && c.Brand != null).ToList();


                        if (brandsel == null || brandsel == 0)
                        {

                        }
                        else {
                            var brandstring = brandsel.ToString();
                            detalles = detalles.Where(c => c.IDBrand == brandstring).ToList();
                        }


                            var brandslst = (from a in detalles select a.Brand).Distinct().OrderBy(a => a).ToList();
                        var sectionlst = (from a in detalles select a.Section).Distinct().OrderBy(a=>a).ToList();
                        var replst = (from a in detalles select a.Rep).Distinct().ToList();
                        var activitylst = (from a in detalles select a.Activity).Distinct().ToList();
                        var storeslst = (from a in detalles select a.Store).Distinct().ToList();

                        ViewBag.brandslst = brandslst;
                        ViewBag.sectionlst = sectionlst;
                        ViewBag.replst = replst;
                        ViewBag.activitylst = activitylst;
                        ViewBag.storelst = storeslst;

                        int[] visitsarr;

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            visitsarr = (from a in db.ActivitiesM
                                         join b in db.VisitsM
                                         on a.ID_visit equals b.ID_visit
                                         where ((b.visit_date >= filtrostartdate && b.end_date <= filtroenddate) && a.isfinished == true) select b.ID_route).Distinct().ToArray();
                        }
                        else {

                            visitsarr = (from a in db.ActivitiesM
                                         join b in db.VisitsM
                                         on a.ID_visit equals b.ID_visit
                                         where (a.ID_customer == customersel && (b.visit_date >= filtrostartdate && b.end_date <= filtroenddate) && a.isfinished == true)
                                         select b.ID_route).Distinct().ToArray();
                     
                        }

                  
                        // Convertimos la lista a array
                        //var rutas = db.VisitsM.Where(c => visitsarr.Contains(c.ID_visit)).Select(c=>c.ID_route).Distinct().ToArray();

                        var routes = (from c in db.RoutesM where (visitsarr.Contains(c.ID_route)) select c).ToList();
                            ViewBag.routeslst = routes;



                       


                      
                    }

                    try
                    {

                        using (var CMKdb = new COM_MKEntities())
                        {
                            //LISTADO DE CLIENTES
                            List<OCRD> customers;
                            
                            if (escliente == true)
                            {
                                customers = (from b in CMKdb.OCRD where (b.CardCode == customersel || b.U_CardCodeDLI==customersel) select b).ToList();

                            }
                            else
                            {
                                customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                            }
                            
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
                                else {
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

                    return View(detalles);
                }
                catch (Exception ex)
                {

                    TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                    return RedirectToAction("Gallery", "Admin", new { id = 0 });


                }


            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }

        public class ImgGallery
        {
            public int idImg { get; set; }
            public string Customer { get; set; }
            public string IDBrand { get; set; }
            public string Brand { get; set; }
            public string Activity { get; set; }
            public string Url { get; set; }
            public string Section { get; set; }
            public string Rep { get; set; }
            public string IDStore { get; set; }
            public string Store { get; set; }
        }

        public class brandsinroute
        {
            public string brand { get; set; }
            public int count { get; set; }
        }
        // GET: SalesRepresentatives
        public ActionResult Dashboard(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners,string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Sales Representatives";
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
                ViewBag.id_customer = id;
                ViewBag.Company = activeuser.Empresas.nombre;

                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                

                var rutas = new List<CustomVisit>();
                int[] visitasarray = new int[] { };
                IQueryable<activitiesVisitsBrands> lstActivities;

                using (var db = new dbComerciaEntities())
                    {

                    //filtros
                    if (customersel == null || customersel == "" || customersel=="0")
                    {
                        lstActivities = (from a in db.ActivitiesM
                                         join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                         where ((a.date >= filtrostartdate && a.date <= filtroenddate) )
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
                    }
                    else {
                        if (brandsel == null || brandsel == 0)
                        {

                            lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                             where ((a.date >= filtrostartdate && a.date <= filtroenddate)  && a.ID_customer == customersel)
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

                        }
                        else {

                            lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                             where ((a.date >= filtrostartdate && a.date <= filtroenddate)  && a.ID_customer == customersel)
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
                            var brandstring = brandsel.ToString();
                            lstActivities = lstActivities.Where(c => c.ID_brand == brandstring);
                        }


                    }




                    var visitsarr = (from a in lstActivities select a.ID_visit).Distinct().ToArray();


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
                                 ID_empresa = a.ID_empresa,
                                 lstUsers = (from t1 in a.VisitsM_representatives
                                             join t2 in db.Usuarios on t1.ID_usuario equals t2.ID_usuario
                                             select new SimpleUserWithImg { ID_user = t1.ID_usuario, Name = t2.nombre + " " + t2.apellido,Email=t2.correo, img = "" }).ToList(),
                                 lstBrands = (from t1 in lstActivities
                                              join t2 in db.FormsM_details on t1.ID_activity equals t2.ID_visit
                                              where (t1.ID_visit == a.ID_visit && t2.ID_formresourcetype == 13 && t2.fdescription != "")
                                              select new brandsinroute { brand = t2.fdescription, count = 0 }).Distinct().ToList()
                }).ToList();

                    //                List<activitiesTypes> distinct = new List<activitiesTypes>();
                    //                List<Brands> distinctBrands = new List<Brands>();
                    //                distinctBrands = (from b in lstActivities
                    //                                  group b by b.ID_brand into g
                    //                                  //where (g.Key != "" && g.Key != null && g.Key !="0")
                    //                                  select new Brands
                    //                                  {
                    //                                      id = g.Key == "" ? "0" : g.Key == null ? "0" : g.Key,
                    //                                      name = g.Key == "" ? "NA" : g.Key == null ? "NA" : g.Key == "0" ? "NA" : g.Select(m => m.Brand).FirstOrDefault(),
                    //                                      count = lstActivities
                    //.Where(x => x.ID_brand == g.Key)
                    //.Select(x => x).Count()
                    //                                  }).ToList();

                    //                distinctBrands = (from p in distinctBrands
                    //                                  group p by p.id into g
                    //                                  select new Brands
                    //                                  {
                    //                                      id = g.Key,
                    //                                      /**/
                    //                                      name = g.Select(e => e.name).FirstOrDefault(),
                    //                                      count = g.Select(e => e.count).FirstOrDefault()
                    //                                  }).ToList();

                    //                List<string> stringsBrands = distinctBrands.Select(s => "'" + s.name + "'").ToList();

                    //                var stringtextBrands = string.Join(",", stringsBrands);
                    //                ViewBag.brandschart = stringtextBrands;

                    //                List<string> brandscount = distinctBrands.Select(s => s.count.ToString()).ToList();

                    //                var stringtextBrandsCount = string.Join(",", brandscount);
                    //                ViewBag.brandschartCount = stringtextBrandsCount;


                    //                distinct = (from b in lstActivities
                    //                            select new activitiesTypes
                    //                            {
                    //                                id = b.ID_form,
                    //                                name = b.formName,
                    //                                activitytype = b.ActivityName,
                    //                                count = lstActivities
                    //.Where(x => x.ID_form == b.ID_form)
                    //.Select(x => x).Count()
                    //                            }).Distinct().ToList();

                    //                List<string> stringsTypes = distinct.Select(s => "'" + s.name + "'").ToList();

                    //                var stringtextTypes = string.Join(",", stringsTypes);
                    //                ViewBag.stringsTypes = stringtextTypes;

                    //                List<string> typescount = distinct.Select(s => s.count.ToString()).ToList();

                    //                var stringtextTypesCount = string.Join(",", typescount);
                    //                ViewBag.typescount = stringtextTypesCount;

                    //                ViewBag.lstTypes = distinct;
                    //                ViewBag.brands = distinctBrands;

                }

                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
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

                        if (brandsel == null || brandsel==0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else {
                            ViewBag.BrandLabel =  brandcmk.FirmName;
                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }
                
                catch {

                        ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
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

                ViewBag.estadisticasEstadosmapa = estadosEstadistica.OrderByDescending(c=>c.Count);

                return View(rutas);
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }
        public class demosVendor
        {
            public string id { get; set; }
            public string name { get; set; }
            public int count { get; set; }
        }
        public ActionResult Demos(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Sales Representatives";
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
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();
                var demos = new List<Demos>();
                List<Usuarios> usuarios = new List<Usuarios>();
                int[] visitasarray = new int[] { };
                using (var db = new dbComerciaEntities())
                {
                    if (customersel == null || customersel == "" || customersel == "0")
                    {

                        demos = (from a in db.Demos
                                 where ((a.visit_date >= filtrostartdate && a.end_date <= filtroenddate))
                                 select a
                 ).ToList();

                        //Lista de usuarios representantes

                        usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9).ToList();


                        List<FormsM> activeForms = new List<FormsM>();
                        activeForms = (from at in db.FormsM where (at.ID_empresa == 2 && at.ID_form==41) select at).ToList();
                        ViewBag.activeForms = activeForms;
                    }
                    else {

                        if (brandsel == null || brandsel == 0)
                        {
                            demos = (from a in db.Demos
                                     where ((a.visit_date >= filtrostartdate && a.end_date <= filtroenddate) && a.ID_Vendor == customersel)
                                     select a
                             ).ToList();

                            //Lista de usuarios representantes

                            usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9).ToList();


                            List<FormsM> activeForms = new List<FormsM>();
                            activeForms = (from at in db.FormsM where (at.ID_empresa == 2 && at.ID_form==41) select at).ToList();
                            ViewBag.activeForms = activeForms;
                        }
                        else {

                            var brandstring = brandsel.ToString();
                            demos = (from a in db.Demos
                                     where ((a.visit_date >= filtrostartdate && a.end_date <= filtroenddate) && a.ID_Vendor == customersel && a.ID_brands.Contains(brandstring))
                                     select a
                             ).ToList();

                            //Lista de usuarios representantes

                            usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9).ToList();


                            List<FormsM> activeForms = new List<FormsM>();
                            activeForms = (from at in db.FormsM where (at.ID_empresa == 2 && at.ID_form==41) select at).ToList();
                            ViewBag.activeForms = activeForms;
                        }

                    }





                }

                var distinct = (from b in demos
                            select new demosVendor
                            {
                                id = b.ID_Vendor,
                                name = b.vendor,
                                count = demos
.Where(x => x.ID_Vendor == b.ID_Vendor)
.Select(x => x).Count()
                            }).ToList();

                ViewBag.distinctVendor = distinct.GroupBy(c=>c.id).Select(i => i.FirstOrDefault()).ToList();
                ViewBag.countdemos = demos.Count();



                ViewBag.representatives = usuarios;
                using (var CMKdb = new COM_MKEntities())
                {
                    var usuariosdemo = CMKdb.OCRD.Where(b => b.Series == 70 && b.CardName != null && b.CardName != "" && b.CardType == "s").OrderBy(b => b.CardName).ToList();

                    List<demosReps> selectList_usuarios = new List<demosReps>();
                    selectList_usuarios = (from st in usuariosdemo
                                           select new demosReps
                                           {
                                               ID = Convert.ToString(st.CardCode),
                                               name = st.CardName.ToString() + " - " + st.E_Mail.ToString()
                                           }).ToList();
                    ViewBag.reps_demos = selectList_usuarios;

                    List<OCRD> customers = new List<OCRD>();
                    customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                    ViewBag.customers = customers.ToList();

                    //LISTADO DE TIENDAS
                    var storesd = (from b in CMKdb.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                    ViewBag.stores = storesd.ToList();
                }
                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
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


                return View(demos);
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }


        public ActionResult Tasks(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Admin";
                ViewData["Page"] = "Tasks";
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
                var tasks = new List<Tasks>();
                List<Usuarios> usuarios = new List<Usuarios>();
                int[] visitasarray = new int[] { };
                using (var db = new dbComerciaEntities())
                {
                    if (customersel == null || customersel == "" || customersel == "0")
                    {
                        tasks = (from a in db.Tasks where ((a.visit_date >= filtrostartdate && a.end_date <= filtroenddate && a.ID_empresa==2)) select a).ToList();
                    }
                    else
                    {
                        tasks = (from a in db.Tasks where ((a.visit_date >= filtrostartdate && a.end_date <= filtroenddate) && a.ID_Customer == customersel && a.ID_empresa == 2) select a).ToList();
                        if (brandsel == null || brandsel == 0)
                        {

                        }
                        else
                        {

                        }

                    }

                    List<FormsM> activeForms = new List<FormsM>();
                    activeForms = (from at in db.FormsM where (at.ID_empresa == 2 && at.ID_activity==5) select at).ToList();
                    ViewBag.activeForms = activeForms;

                    usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9).ToList();

                }



                ViewBag.representatives = usuarios;
                using (var CMKdb = new COM_MKEntities())
                {

                    List<OCRD> customers = new List<OCRD>();
                    customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                    ViewBag.customers = customers.ToList();

                    //LISTADO DE TIENDAS
                    var storesd = (from b in CMKdb.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                    ViewBag.stores = storesd.ToList();
                }
                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
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


                return View(tasks);
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }
        public ActionResult convertImages(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Admin";
                ViewData["Page"] = "Tasks";
                ViewBag.menunameid = "marketing_menu";
                ViewBag.submenunameid = "";

                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                List<imagesInfo> imageslist = new List<imagesInfo>();
       
                using (var db = new dbComerciaEntities())
                {
                    if (customersel == null || customersel == "" || customersel == "0")
                    {
                        imageslist = (from form in db.FormsM_details
                                      join activities in db.ActivitiesM on form.ID_visit equals activities.ID_activity into temp
                                      from last in temp.DefaultIfEmpty()
                                      where ((last.date >= filtrostartdate && last.date <= filtroenddate) && form.ID_formresourcetype == 5 && form.fsource.Contains("~"))
                                      select new imagesInfo
                                      {
                                          ID_image = form.ID_details,
                                          Activity = last.description,
                                          visitDate = last.date,
                                          ID_customer = last.ID_customer,
                                          Customer = last.Customer,
                                          url = form.fsource
                                      }).ToList();
                    }
                    else
                    {
                        imageslist = (from form in db.FormsM_details
                                      join activities in db.ActivitiesM on form.ID_visit equals activities.ID_activity into temp
                                      from last in temp.DefaultIfEmpty()
                                      where ((last.date >= filtrostartdate && last.date <= filtroenddate) && form.ID_formresourcetype == 5 && form.fsource.Contains("~") && last.ID_customer == customersel)
                                      select new imagesInfo
                                      {
                                          ID_image = form.ID_details,
                                          Activity = last.description,
                                          visitDate = last.date,
                                          ID_customer = last.ID_customer,
                                          Customer = last.Customer,
                                          url = form.fsource
                                      }).ToList();
                        if (brandsel == null || brandsel == 0)
                        {

                        }
                        else
                        {

                        }

                    }

              

                }

                Session["imageslist"] = imageslist;
                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
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


                return View(imageslist);
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }


        public class representativesVisit
        {
            public int ID { get; set; }
            public string name { get; set; }
            public string email { get; set; }
        }
        public class demosReps
        {
            public string ID { get; set; }
            public string name { get; set; }

        }

        public ActionResult Visit_details(int? id)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                using (var db = new dbComerciaEntities())
                {
                    VisitsM visitsM = db.VisitsM.Find(id);

                    if (visitsM == null)
                    {
                        return HttpNotFound();
                    }

                    //Datos de visita
                    //Route
                    ViewBag.route = visitsM.ID_route;
                    ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
                    ViewBag.idvisita = id;

                    ViewBag.storename = visitsM.store;

                    var geoLong = "";
                    var geoLat = "";

                    geoLong = visitsM.geoLong;
                    geoLat = visitsM.geoLat;

                    ViewBag.glong = geoLong;
                    ViewBag.glat = geoLat;
                    ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;
                    //---------------------------------

                    //Lista de usuarios representantes
                    List<Usuarios> usuarios = new List<Usuarios>();
                    usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9).ToList();
                    //Seleccionamos representantes que estan incluidos
                    var reps = (from rep in db.VisitsM_representatives
                                join users in db.Usuarios on rep.ID_usuario equals users.ID_usuario
                                where (rep.ID_visit == id)                               
                                select new representativesVisit
                                {
                                    ID = rep.ID_usuario,
                                    name = users.nombre + " " + users.apellido,
                                    email = users.correo
                                }
                                   ).ToList();

                    ViewBag.repslist = reps;
                    //FIN representantes
                    //ACTIVITIES
                    List<ActivitiesM> activities = new List<ActivitiesM>();
                    if ((activeuser.ID_tipomembresia == 8 && activeuser.ID_rol == 8) || activeuser.ID_tipomembresia == 1)//Admin
                    {
                        activities = (from a in db.ActivitiesM where (a.ID_visit == id) select a).OrderBy(a => a.ID_activitytype).ThenBy(a => a.description).ToList();
                    }
                    else
                    {
                        activities = (from a in db.ActivitiesM where (a.ID_visit == id && a.ID_usuarioEnd == activeuser.ID_usuario) select a).OrderBy(a => a.ID_activitytype).ThenBy(a => a.description).ToList();
                    }

                    foreach (var item in activities) {
                        item.query1 = "";
                        item.query1 = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == item.ID_activity) select detalle.fdescription).FirstOrDefault();
                    }
                    //CHECK OUT POR USUARIO
                    if (activeuser.ID_rol == 9 && activeuser.ID_tipomembresia == 8)
                    {
                        var rep = (from a in db.VisitsM_representatives
                                   where (a.ID_visit == id && a.ID_usuario == activeuser.ID_usuario)
                                   select a
                              ).FirstOrDefault();
                        ViewBag.estadovisita = Convert.ToInt32(rep.query1);//Utilizaremos este campo para filtrar el estado por usuario
                    }
                    else
                    {
                        ViewBag.estadovisita = visitsM.ID_visitstate;
                    }
                    List<ImgGallery> detalles;
                    IQueryable<ActivitiesM> actividadesList;



               
                   actividadesList = (from ed in db.ActivitiesM where (ed.ID_visit ==id && ed.isfinished == true && ed.ID_activitytype==1) select ed);
        

                    var actividades = actividadesList.Select(a => a.ID_activity).ToArray();


                    detalles = (from a in db.FormsM_details
                                join b in actividadesList on a.ID_visit equals b.ID_activity
                                join c in db.VisitsM on b.ID_visit equals c.ID_visit
                                where (c.ID_visit==id && a.ID_formresourcetype == 5)
                                select new ImgGallery
                                {
                                    idImg = a.ID_details,
                                    Customer = b.Customer,
                                    Activity = b.description,
                                    Url = a.fsource,
                                    IDStore = c.ID_store,
                                    Store = c.ID_store + " - " + c.store,
                                    Section = a.fdescription == "PICTURE 1" ? "BEFORE" : a.fdescription == "PICTURE 2" ? "AFTER" : a.fdescription == "CONDICION FINAL DE LA TIENDA (DESPUES)" ? "AFTER" : a.fdescription == "Tomar fotografia inicial" ? "BEFORE" : a.fdescription == "Foto de herramientas" ? "TOOLS" : a.fdescription == "Foto de la competencia" ? "TOOLS" : "OTHER",
                                    Rep = (from usu in db.Usuarios where (usu.ID_usuario == b.ID_usuarioEnd) select usu.nombre + " " + usu.apellido).FirstOrDefault() == null ? "" : (from usu in db.Usuarios where (usu.ID_usuario == b.ID_usuarioEnd) select usu.nombre + " " + usu.apellido).FirstOrDefault(),
                                    IDBrand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == b.ID_activity) select detalle.fvalueText).FirstOrDefault(),
                                    Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == b.ID_activity) select detalle.fdescription).FirstOrDefault(),

                                }).ToList();

                    detalles = detalles.Where(c => c.Url != null && !c.Url.Equals(string.Empty) && c.Brand != null).ToList();


                    var brandslst = (from a in detalles select a.Brand).Distinct().OrderBy(a => a).ToList();
                    var sectionlst = (from a in detalles select a.Section).Distinct().OrderBy(a => a).ToList();
                    var replst = (from a in detalles select a.Rep).Distinct().ToList();
                    var activitylst = (from a in detalles select a.Activity).Distinct().ToList();
                    var storeslst = (from a in detalles select a.Store).Distinct().ToList();

                    ViewBag.brandslst = brandslst;
                    ViewBag.sectionlst = sectionlst;
                    ViewBag.replst = replst;
                    ViewBag.activitylst = activitylst;
                    ViewBag.storelst = storeslst;

                    ViewBag.gallery = detalles;


                    //FIN ACTIVITIES
                    //representantes


                    ViewBag.representatives = usuarios;
                    using (var CMKdb = new COM_MKEntities())
                    {
                        var usuariosdemo = CMKdb.OCRD.Where(b => b.Series == 70 && b.CardName != null && b.CardName != "" && b.CardType == "s").OrderBy(b => b.CardName).ToList();

                        List<demosReps> selectList_usuarios = new List<demosReps>();
                        selectList_usuarios = (from st in usuariosdemo
                                                               select new demosReps
                                                               {
                                                                   ID = Convert.ToString(st.CardCode),
                                                                   name = st.CardName.ToString() + " - " + st.E_Mail.ToString()
                                                               }).ToList();
                        ViewBag.reps_demos = selectList_usuarios;

                        List<OCRD> customers = new List<OCRD>();
                        customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                        ViewBag.customers = customers.ToList();
                    }

                    List<FormsM> activeForms = new List<FormsM>();
                    activeForms = (from at in db.FormsM where (at.ID_empresa == GlobalVariables.ID_EMPRESA_USUARIO) select at).ToList();
                    ViewBag.activeForms = activeForms;
                    //FIN FORMULARIOS

                    return View(activities);

                }
                
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }

        }
        //CLASE PARA ALMACENAR OBJETOS
        public class MyObj
        {
            public string id_resource { get; set; }
            public string fsource { get; set; }
            public string fdescription { get; set; }
            public string fvalue { get; set; }
            public int idkey { get; set; }
            public int parent { get; set; }
            public IList<MyObj> children { get; set; }
        }

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


        public ActionResult Activityon(int? id)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);

                ViewBag.usuario = activeuser.nombre + " " + activeuser.apellido;
                var activity = new ActivitiesM();
                var formsM = new FormsM();
                List<FormsM_details> FormsMDet = new List<FormsM_details>();
                using (var db = new dbComerciaEntities())
                {
                    activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();
                    formsM = db.FormsM.Find(activity.ID_form);
                    FormsMDet = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();
                }
    

                if (activity == null)
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {

           

                    //LISTADO DE CLIENTES
                    //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                    if (activity.Customer != "")
                    {
                        using (var CMKdb = new COM_MKEntities())
                        {
                            var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                            ViewBag.customers = customers.ToList();
                        }

                    }
                    else
                    {
                        using (var CMKdb = new COM_MKEntities())
                        {
                            if ((activeuser.ID_tipomembresia == 8 && activeuser.ID_rol == 8) || activeuser.ID_tipomembresia == 1)//Administrador
                            {
                                var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                                ViewBag.customers = customers.ToList();
                            }
                            else
                            {

                                var customers = (from b in CMKdb.OCRD where (activeuser.estados_influencia.Contains(b.CardCode)) select b).OrderBy(b => b.CardName).ToList();
                                ViewBag.customers = customers.ToList();
                            }
                        }
                    }

                 


                    //NUEVO
                    //ID VISIT SE UTILIZA COMO RELACION
                    List<MyObj_tablapadre> listapadresActivities = (from item in FormsMDet
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
                                                                           query2 = item.query2,
                                                                           ID_empresa = item.ID_empresa
                                                                       }
                                          ).OrderBy(a => a.obj_order).ToList();


                    List<tablahijospadre> listahijasActivities = (from item in FormsMDet
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
                                                                      ID_empresa = item.ID_empresa

                                                                  }).OrderBy(a => a.obj_order).ToList();


                    List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                    ///
                    var showbuttondynamic = (from item in FormsMDet
                                             where (item.ID_formresourcetype == 3)
                                             select item).Count();

                    if (showbuttondynamic > 0)
                    {

                        ViewBag.mostrarboton = 1;
                    }
                    else
                    {
                        ViewBag.mostrarboton = 0;
                    }
                    //Deserealizamos  los datos
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                    ViewBag.idvisitareal = activity.ID_visit;
                    ViewBag.idvisita = activity.ID_activity;

                    ViewBag.details = categoriasListActivities;

                    ViewBag.detailssql = FormsMDet;

                    Session["detailsForm"] = FormsMDet;


                    return View();
                }


            }
            else
            {
                return RedirectToAction("Dashboard", "SalesRepresentatives");
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

        public class CustomRoutes {
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

        public ActionResult GetEvents(DateTime startf, DateTime endf)
        {
            try
            {

                var rutas = new List<CustomRoutes>();

                using (var db = new dbComerciaEntities())
                {
                    

                    rutas = (from a in db.RoutesM
                             where ((a.date >= startf && a.end_date <= endf) && a.ID_empresa == 2) //EMPRESA POR DEFECTO
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
                                                      ID_empresa = t1.ID_empresa,
                                                      lstUsers = (from t2 in t1.VisitsM_representatives
                                                                  join t3 in db.Usuarios on t2.ID_usuario equals t3.ID_usuario
                                                                  select new SimpleUserforRoutes { ID_user = t2.ID_usuario, Name = t3.nombre + " " + t3.apellido, Email = t3.correo, img = "", EstadoVisita = 0 }).ToList()
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
                    List<string> reps = new List<string>();

                    foreach (var vis in item.visitasenRuta)
                    {
                        foreach (var user in vis.lstUsers)
                        {
                            if (!reps.Contains(user.Name))
                            {
                                reps.Add(user.Name);
                            }
                        }
                    }

                    rt.porcentaje = item.porcentaje;
                    rt.lstReps = string.Join(",", reps);
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
                fstartd = null;
                fendd = null;
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

                    //if ((activeuser.ID_tipomembresia == 8 && activeuser.ID_rol == 8) || activeuser.ID_tipomembresia == 1)
                    //{

                        //visitas = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate && d.ID_empresa == empresadef).ToList();
                        //rutas = db.RoutesM.Where(d => d.date >= filtrostartdate && d.end_date <= filtroenddate && d.ID_empresa == empresadef).OrderByDescending(d => d.date).ToList();
                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            lstVisitas = (from a in db.VisitsM
                                          join b in db.ActivitiesM on a.ID_visit equals b.ID_visit into ps
                                          from p in ps.DefaultIfEmpty()
                                          where ((a.visit_date >= filtrostartdate && a.end_date <= filtroenddate))
                                          select new VisitsInfoCalendar
                                          {
                                              ID_visit = a.ID_visit,
                                              ID_store = a.ID_store,
                                              idroute = a.ID_route,
                                              visitDate = a.visit_date,
                                              ID_customer = p == null ? "Not Assigned" : p.ID_customer,
                                              ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == p.ID_activity).Select(c => c.fvalueText).FirstOrDefault() ?? "Not Assigned"
                                          }).ToList();

                        }
                        else
                        {
                            lstVisitas = (from a in db.VisitsM
                                          join b in db.ActivitiesM on a.ID_visit equals b.ID_visit into ps
                                          from p in ps.DefaultIfEmpty()
                                          where (p.ID_customer == customersel && (a.visit_date >= filtrostartdate && a.end_date <= filtroenddate) )
                                          select new VisitsInfoCalendar
                                          {
                                              ID_visit = a.ID_visit,
                                              ID_store = a.ID_store,
                                              idroute = a.ID_route,
                                              visitDate = a.visit_date,
                                              ID_customer = p == null ? "Not Assigned" : p.ID_customer,
                                              ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == p.ID_activity).Select(c => c.fvalueText).FirstOrDefault() ?? "Not Assigned"
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

                        var lstvisits = lstVisitas.Select(b => b.idroute).Distinct().ToArray();







                        rutas = (from a in db.RoutesM where(lstvisits.Contains(a.ID_route))
                                 select new CustomRoutes
                                 {
                                     ID_route = a.ID_route,
                                     date = a.date,
                                     query1 = a.query1,
                                     query2 = a.query2,
                                     query3 = a.query3,
                                     end_date = a.end_date,
                                     ID_empresa = a.ID_empresa,
                                     porcentaje=0,
                                     visitasfinalizadas=0,
                                     visitasagendadas=0,
                                     visitascanceladas=0,
                                     visitasenprogreso=0,
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
                                                          ID_empresa = t1.ID_empresa,
                                                          lstUsers = (from t2 in t1.VisitsM_representatives
                                                                      join t3 in db.Usuarios on t2.ID_usuario equals t3.ID_usuario
                                                                      select new SimpleUserforRoutes { ID_user = t2.ID_usuario, Name = t3.nombre + " " + t3.apellido, Email = t3.correo, img = "",EstadoVisita=0 }).ToList()
                                                      }).ToList()
                    }).ToList();


                    //}
                    //else
                    //{

                    //    var visitrep = (from gg in db.VisitsM_representatives where (gg.ID_usuario == activeuser.ID_usuario) select gg.ID_visit).ToArray();


                    //    visitas = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && r.visit_date >= filtrostartdate && r.end_date <= filtroenddate) select r).ToList();

                    //    var arrayVisiID = (from arr in visitas select arr.ID_route).ToArray();
                    //    //rutas = (from rut in db.RoutesM where (arrayVisiID.Contains(rut.ID_route)) select rut).ToList();

                    //    rutas = (from a in db.RoutesM
                    //             where (arrayVisiID.Contains(a.ID_route))
                    //             select new CustomRoutes
                    //             {

                    //                 ID_route = a.ID_route,
                    //                 date = a.date,
                    //                 query1 = a.query1,
                    //                 query2 = a.query2,
                    //                 query3 = a.query3,
                    //                 end_date = a.end_date,
                    //                 ID_empresa = a.ID_empresa,
                    //                 porcentaje = 0,
                    //                 visitasfinalizadas = 0,
                    //                 visitasagendadas = 0,
                    //                 visitascanceladas = 0,
                    //                 visitasenprogreso = 0,
                    //                 visitasenRuta = (from t1 in a.VisitsM
                    //                                  select new CustomVisitforRoutes
                    //                                  {
                    //                                      ID_visit = t1.ID_visit,
                    //                                      ID_customer = t1.ID_customer,
                    //                                      customer = t1.customer,
                    //                                      ID_store = t1.ID_store,
                    //                                      store = t1.store,
                    //                                      address = t1.address,
                    //                                      city = t1.city,
                    //                                      state = t1.state,
                    //                                      zipcode = t1.zipcode,
                    //                                      visit_date = t1.visit_date,
                    //                                      ID_visitstate = t1.ID_visitstate,
                    //                                      comments = t1.comments,
                    //                                      check_in = t1.check_in,
                    //                                      check_out = t1.check_out,
                    //                                      end_date = t1.end_date,
                    //                                      geoLat = t1.geoLat,
                    //                                      geoLong = t1.geoLong,
                    //                                      extra_hours = t1.extra_hours,
                    //                                      ID_route = t1.ID_route,
                    //                                      ID_empresa = t1.ID_empresa,
                    //                                      lstUsers = (from t2 in t1.VisitsM_representatives
                    //                                                  join t3 in db.Usuarios on t2.ID_usuario equals t3.ID_usuario
                    //                                                  select new SimpleUserforRoutes { ID_user = t2.ID_usuario, Name = t3.nombre + " " + t3.apellido, img = "", EstadoVisita=Convert.ToInt32(t2.query1) }).ToList()
                    //                                  }).ToList()
                    //             }).ToList();

                     

                    //}

                    //Agregamos los representantes y tambien el estado de cada visita por REP filtro
                    //if (activeuser.ID_tipomembresia == 8 && activeuser.ID_rol == 9)
                    //{

                    //    foreach (var itemVisita in visitas)
                    //    {
                    //        var repvisit = (from a in db.VisitsM_representatives where (a.ID_visit == itemVisita.ID_visit && a.ID_usuario == activeuser.ID_usuario) select a).FirstOrDefault();

                    //        itemVisita.ID_visitstate = Convert.ToInt32(repvisit.query1);
                    //    }

                    //}


                    //ESTADISTICA DE RUTAS POR ESTADO DE VISITAS
                    decimal totalRutas = rutas.Count();
                    foreach (var rutait in rutas)
                    {

                        rutait.visitascanceladas = rutait.visitasenRuta.Where(r=> r.ID_visitstate==1 && r.ID_route == rutait.ID_route).Count();
                        rutait.visitasfinalizadas = rutait.visitasenRuta.Where(r => (r.ID_visitstate == 4 || r.ID_visitstate==1)&& r.ID_route == rutait.ID_route).Count();
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

                    //MAPA DE RUTAS
                    //var demos_map = (from a in rutas select a).ToList();

                    List<Usuarios> usuarios = new List<Usuarios>();
                    //Convertimos la lista a array
                    usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9).ToList();

                    //Convertimos la lista a array
                    ArrayList myArrList = new ArrayList();
                    myArrList.AddRange((from p in usuarios
                                        select new
                                        {
                                            id = p.ID_usuario,
                                            text = p.nombre + " " + p.apellido
                                        }).ToList());

                    List<OCRD> storeslst = new List<OCRD>();
                    //LISTADO DE REPRESENTANTES
                    using (var CMKdb = new COM_MKEntities())
                    {
                        try
                        {
                            //LISTADO DE CLIENTES
                            //LISTADO DE CLIENTES

                            //if (escliente == true)
                            //{
                            var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                            ViewBag.customerssel = customers.ToList();
                 

                            //var customerslst = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                            //    ViewBag.customerssel = customerslst.ToList();

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

                        

                        catch
                        {

                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.customerssel = new List<OCRD>();
                            ViewBag.CustomerSelCode = "0";
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }


                        ViewBag.usuarios = JsonConvert.SerializeObject(myArrList);
                        //LISTADO DE RUTAS
                        var rutass = CMKdb.C_ROUTES.OrderBy(c => c.Code);
                        ViewBag.rutass = rutass.ToList();
                        //LISTADO DE TIENDAS

                        List<MyObj_tablapadreRutas> listapadres = (from p in CMKdb.C_ROUTES
                                                                   select
                                          new MyObj_tablapadreRutas
                                          {
                                              id = p.Code,
                                              text = p.Name
                                          }
                                                              ).ToList();

                        List<tablahijospadreRutas> listahijas = (from p in CMKdb.C_ROUTE
                                                                 join store in CMKdb.OCRD on p.U_CardCode equals store.CardCode
                                                                 select new tablahijospadreRutas
                                                                 {
                                                                     id = p.U_CardCode,
                                                                     text = store.CardName.Replace("\"", "\\\""),
                                                                     parent = p.U_Route
                                                                 }).ToList();


                        List<MyObj_tablapadreRutas> categoriasList = ObtenerCategoriarJerarquiaByNameRutas(listapadres, listahijas);

                        //var stores = (from b in CMKdb.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "" && b.QryGroup30 == "Y" && b.validFor == "Y") select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.stores = JsonConvert.SerializeObject(categoriasList);
                        //FIN LISTADO DE TIENDAS

                        //LISTADO DE ACTIVIDADES

                        List<MyObj_tablapadreRutas> listapadresActivities = (from a in db.ActivitiesM_types
                                                                             select
                                                                                new MyObj_tablapadreRutas
                                                                                {
                                                                                    id = a.ID_activity.ToString(),
                                                                                    text = a.description
                                                                                }
                                              ).ToList();

                        List<tablahijospadreRutas> listahijasActivities = (from p in db.FormsM
                                                                           select new tablahijospadreRutas
                                                                           {
                                                                               id = p.ID_form.ToString(),
                                                                               text = p.name,
                                                                               parent = p.ID_activity.ToString()
                                                                           }).ToList();


                        List<MyObj_tablapadreRutas> categoriasListActivities = ObtenerCategoriarJerarquiaByIDRutas(listapadresActivities, listahijasActivities);


                        ViewBag.activitieslist = JsonConvert.SerializeObject(categoriasListActivities);

                        //LISTADO DE CLIENTES
                        //var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                        //ViewBag.customers = customers.ToList();
                        //LISTADO DE ACTIVIDAD (TIMELINE)
                        //LISTADO DE TIENDAS
                  
                        storeslst = (from b in CMKdb.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.storeslst = storeslst;
                    }

                    //var log = new List<ActivitiesM_log>();



                    //if ((activeuser.ID_tipomembresia == 8 && activeuser.ID_rol == 8) || activeuser.ID_tipomembresia == 1)
                    //{
                    //    log = (from l in db.ActivitiesM_log where (l.fecha_conexion >= filtrostartdate && l.fecha_conexion <= filtroenddate) select l).ToList();
                    //}
                    //else
                    //{
                    //    log = (from l in db.ActivitiesM_log where (l.ID_usuario == ID && l.fecha_conexion >= filtrostartdate && l.fecha_conexion <= filtroenddate) select l).OrderBy(l => l.fecha_conexion).ToList();
                    //}

                    //ViewBag.log = log;

                    //Filtros Viewbag
                    //Filtros viewbag

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
                        else if (item.porcentaje > 1 && item.porcentaje <99)
                        {
                            rt.className = "block b-t b-t-2x b-success";//"#2081d6";
                        }
                        else if (item.porcentaje > 99) {
                            rt.className = "block b-t b-t-2x b-info";//"#2081d6";
                        }
                        List<string> reps = new List<string>();

                        foreach (var vis in item.visitasenRuta) {
                            foreach (var user in vis.lstUsers) {
                                if (!reps.Contains(user.Name)) {
                                    reps.Add(user.Name);
                                }
                            }
                        }

                        rt.porcentaje = item.porcentaje;
                        rt.lstReps = string.Join(",", reps);
                        rutaslst.Add(rt);
                    }
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string result = javaScriptSerializer.Serialize(rutaslst.ToArray());
                    ViewBag.calroutes = result;
                }
                return View(rutas.ToList());
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }



        
       
    }
}