using comerciamarketing_webapp.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace comerciamarketing_webapp.Controllers
{
    public class SalesRepresentativesController : Controller
    {
        // GET: SalesRepresentatives
        public ActionResult Dashboard(string id, string fstartd, string fendd, string stores, string brands, string spartners)
        {

            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                return RedirectToAction("Index", "Home", new { access = false });
                //try
                //{
                //    if (Request.Cookies["correo"] != null)
                //    {
                //        HttpCookie aCookieCorreo = Request.Cookies["correo"];
                //        HttpCookie aCookiePassword = Request.Cookies["pass"];

                //        var correo = Server.HtmlEncode(aCookieCorreo.Value).ToString();
                //        var pass = Server.HtmlEncode(aCookiePassword.Value).ToString();

                //        using (var db = new dbComerciaEntities())
                //        {
                //            Session["activeUser"] = (from c in db.Usuarios where (c.correo == correo && c.contrasena == pass) select c).FirstOrDefault();
                //        }

                //        activeuser = Session["activeUser"] as Usuarios;

                //        if (activeuser != null)
                //        {
                //            Session["IDusuario"] = activeuser.ID_usuario.ToString();
                //            Session["tipousuario"] = activeuser.ID_tipomembresia.ToString();
                //            Session["tiporol"] = activeuser.ID_rol.ToString();
                //            Session["ultimaconexion"] = "";
                //            GlobalVariables.ID_EMPRESA_USUARIO = Convert.ToInt32(activeuser.ID_empresa);

                //            return RedirectToAction("Iniciar_sesion", "Home", new { usuariocorreo = correo, password = pass, rememberme = true });
                //        }
                //        else
                //        {
                //            return RedirectToAction("Index", "Home", new { access = false });
                //        }
                //    }
                //    else
                //    {
                //        return RedirectToAction("Index", "Home", new { access = false });

                //    }


                //}
                //catch
                //{
                //    return RedirectToAction("Index", "Home", new { access = false });
                //}
            }
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

                var rutas = new List<VisitsM>();
                int[] visitasarray = new int[] { };
                if ((activeuser.ID_tipomembresia == 8 && activeuser.ID_rol == 8) || activeuser.ID_tipomembresia == 1)
                {
                    using (var db = new dbComerciaEntities())
                    {
                        rutas = db.VisitsM.Where(d => (d.visit_date == filtrostartdate || d.end_date == filtroenddate) && d.ID_empresa == activeuser.ID_empresa).ToList();
                        visitasarray = rutas.Select(d => d.ID_visit).ToArray();
                    }


                }
                else
                {
                    using (var db = new dbComerciaEntities())
                    {
                        var visitrep = (from gg in db.VisitsM_representatives where (gg.ID_usuario == activeuser.ID_usuario) select gg.ID_visit).ToArray();

                       // rutas = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && r.ID_empresa == activeuser.ID_empresa) select r).ToList();
                        //rutas = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && (r.visit_date == filtrostartdate && r.end_date == filtroenddate) && r.ID_empresa == activeuser.ID_empresa) select r).ToList();
                        rutas = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && (r.visit_date >= filtrostartdate && r.end_date <= filtroenddate) && r.ID_empresa == activeuser.ID_empresa) select r).ToList();

                        visitasarray = rutas.Select(d => d.ID_visit).ToArray();
                    }


                }

                List<string> Vendors = new List<string>(activeuser.estados_influencia.Split(new string[] { "," }, StringSplitOptions.None));
                List<demosReps> lstVendors = new List<demosReps>();
                if (Vendors.Count > 0)
                {
                    foreach (var item in Vendors)
                    {
                        demosReps newitem = new demosReps();
                        newitem.ID = item;
                        using (var dbcmk = new COM_MKEntities())
                        {
                            newitem.name = (from a in dbcmk.OCRD where (a.CardCode == item) select a.CardName).FirstOrDefault();
                        }

                        lstVendors.Add(newitem);
                        
                    }
                }

                ViewBag.lstVendors = lstVendors.OrderBy(c=>c.name);
                ViewBag.lstVendorsCount = lstVendors.Count();
                return View(rutas);

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
            }
            else
            {
                return RedirectToAction("Index", "Home", new { access = false });

            }

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

                    ViewBag.idvisita = id;

                    ViewBag.storename = visitsM.store;
                    ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
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

                    foreach (var item in activities)
                    {
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
                    activeForms = (from at in db.FormsM where (at.ID_empresa == activeuser.ID_empresa) select at).ToList();
                    ViewBag.activeForms = activeForms;
                    //FIN FORMULARIOS

                    return View(activities);

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


        public ActionResult Calendar(string id, string fstartd, string fendd, string stores, string brands, string spartners)
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
                ViewBag.id_customer = id;


                var visitas = new List<VisitsM>();
                var rutas = new List<RoutesM>();

                DateTime filtrostartdate;
                DateTime filtroenddate;
                int empresadef = 2;
                //filtros de fecha
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
                //FIN FILTROS*******************
                using (var db = new dbComerciaEntities())
                {

                    if ((activeuser.ID_tipomembresia == 8 && activeuser.ID_rol == 8) || activeuser.ID_tipomembresia == 1)
                    {

                        visitas = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate && d.ID_empresa == empresadef).ToList();
                        rutas = db.RoutesM.Where(d => d.date >= filtrostartdate && d.end_date <= filtroenddate && d.ID_empresa == empresadef).OrderByDescending(d => d.date).ToList();

                    }
                    else
                    {

                        var visitrep = (from gg in db.VisitsM_representatives where (gg.ID_usuario == activeuser.ID_usuario) select gg.ID_visit).ToArray();


                        visitas = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && r.visit_date >= filtrostartdate && r.end_date <= filtroenddate) select r).ToList();

                        var arrayVisiID = (from arr in visitas select arr.ID_route).ToArray();
                        rutas = (from rut in db.RoutesM where (arrayVisiID.Contains(rut.ID_route)) select rut).ToList();

                    }

                    //Agregamos los representantes y tambien el estado de cada visita por REP filtro
                    if (activeuser.ID_tipomembresia == 8 && activeuser.ID_rol == 9)
                    {

                        foreach (var itemVisita in visitas)
                        {
                            var repvisit = (from a in db.VisitsM_representatives where (a.ID_visit == itemVisita.ID_visit && a.ID_usuario == activeuser.ID_usuario) select a).FirstOrDefault();

                            itemVisita.ID_visitstate = Convert.ToInt32(repvisit.query1);
                        }

                    }


                    //ESTADISTICA DE RUTAS POR ESTADO DE VISITAS
                    decimal totalRutas = visitas.Count();
                    foreach (var rutait in rutas)
                    {

                        //int finishedorCanceled = (from e in visitas where ((e.ID_visitstate == 4 || e.ID_visitstate==1) && e.ID_route == rutait.ID_route) select e).Count();
                        decimal finishedorCanceled = (from e in visitas where ((e.ID_visitstate == 4) && e.ID_route == rutait.ID_route) select e).Count();
                        decimal inprogressv = (from e in visitas where (e.ID_visitstate == 2 && e.ID_route == rutait.ID_route) select e).Count();
                        totalRutas = (from e in visitas where (e.ID_route == rutait.ID_route) select e).Count();

                        ViewBag.finished = finishedorCanceled;

                        if (totalRutas != 0)
                        {
                            if (inprogressv != 0 && finishedorCanceled != 0)
                            {
                                decimal n = (finishedorCanceled / totalRutas) * 100;
                                decimal m = (inprogressv / totalRutas) * 50;
                                rutait.query3 = (n + m).ToString();

                            }
                            else if (inprogressv == 0 && finishedorCanceled != 0)
                            {

                                rutait.query3 = (((Convert.ToDecimal(finishedorCanceled) / totalRutas) * 100)).ToString();
                            }
                            else if (inprogressv != 0 && finishedorCanceled == 0)
                            {
                                rutait.query3 = (((Convert.ToDecimal(inprogressv) / totalRutas) * 50)).ToString();
                            }
                            else
                            {
                                rutait.query3 = (Convert.ToDecimal(0)).ToString();
                            }


                        }
                        else
                        {
                            rutait.query3 = "0";
                        }
                    }

                    //MAPA DE RUTAS
                    var demos_map = (from a in rutas select a).ToList();

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


                    //LISTADO DE REPRESENTANTES
                    using (var CMKdb = new COM_MKEntities())
                    {
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
                        var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                        //LISTADO DE ACTIVIDAD (TIMELINE)
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
                    ViewBag.filtrofechaend = filtroenddate.ToShortDateString(); ;
                    //*****************

                    List<Routes_calendar> rutaslst = new List<Routes_calendar>();

                    foreach (var item in rutas)
                    {
                        Routes_calendar rt = new Routes_calendar();

                        rt.title = item.ID_route + " - " + item.query2.ToUpper();
                        rt.url = "";
                        rt.start = item.date.ToString("yyyy-MM-dd");
                        rt.end = item.end_date.AddDays(1).ToString("yyyy-MM-dd");
                        rt.className = "block b-t b-t-2x b-info";//"#2081d6";
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