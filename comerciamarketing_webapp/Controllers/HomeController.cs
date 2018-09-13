using comerciamarketing_webapp.Models;
using Newtonsoft.Json;
using Postal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace comerciamarketing_webapp.Controllers
{

    public class HomeController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();
        private COM_MKEntities CMKdb = new COM_MKEntities();

        public ActionResult Home(string idioma)
        {
            
            return View();
        }

            public ActionResult Main(string startdate, string finishdate)
        {

            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();
                //var registro_conexiones = (from b in db.historial_conexiones where (b.ID_usuario == ID) select b).OrderByDescending(b=> b.fecha_conexion).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " +  datosUsuario.apellido;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;
                ViewBag.tipomembresia = datosUsuario.Tipo_membresia.descripcion;
                ViewBag.ultimavisita = Session["ultimaconexion"].ToString();//datosUsuario.fultima_visita.ToString();
                ViewBag.bloquearcontenido = "no";


                //var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                //var saturday = sunday.AddDays(6).AddHours(23);

                //if (startdate == null)
                //{
                //    if (finishdate == null)
                //    {


                //    }

                //}
                //else {
                //    if (finishdate == null)
                //    {

                //    }
                //    else {
                //        try
                //        {
                //            sunday = Convert.ToDateTime(startdate);
                //            saturday = Convert.ToDateTime(finishdate);
                //            saturday = saturday.AddHours(23);
                //        }
                //        catch {
                //            sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                //            saturday = sunday.AddDays(6).AddHours(23);
                //        }


                //    }

                //}



                //int onhold = (from e in db.Demos where (e.ID_demostate == 3 && e.visit_date >= sunday && e.end_date <= saturday) select e).Count();
                //int inprogress = (from e in db.Demos where (e.ID_demostate == 2 && e.visit_date >= sunday && e.end_date <= saturday) select e).Count();
                //int canceled = (from e in db.Demos where (e.ID_demostate == 1 && e.visit_date >= sunday && e.end_date <= saturday) select e).Count();
                //int finished = (from e in db.Demos where (e.ID_demostate == 4 && e.visit_date >= sunday && e.end_date <= saturday) select e).Count();


                //var demos_map = (from a in db.Demos where (a.visit_date >= sunday && a.end_date <= saturday) select a).ToList();


                ////Asignamos la geoubicacion
                //foreach (var item in demos_map) {


                //    var usuario = (from h in CMKdb.OCRD where (h.CardCode == item.ID_usuario) select h).FirstOrDefault();
                //    if (usuario == null) { } else { item.ID_usuario = usuario.CardName; }


                //}


                //// Convertimos la lista a array
                //ArrayList myArrList = new ArrayList();
                //myArrList.AddRange((from p in demos_map
                //                    select new
                //                    {
                //                        id = p.ID_demo,
                //                        nombre = p.ID_usuario,
                //                        PlaceName = p.store,
                //                        GeoLong = p.geoLong,
                //                        GeoLat = p.geoLat,
                //                        demo_state = p.Demo_state.sdescription,
                //                        vendor = p.vendor,
                //                        date = p.visit_date,
                //                        comment = p.comments
                //                    }).ToList());


                //ViewBag.demos_map = JsonConvert.SerializeObject(myArrList);



                //ViewBag.bloquearcontenido = "no";
                //ViewBag.desdehasta = "From " + sunday.ToShortDateString() + " to " + saturday.ToShortDateString(); 
                //ViewBag.onhold_demos = onhold;
                //ViewBag.inprogress_demos = inprogress;
                //ViewBag.canceled_demos = canceled;
                //ViewBag.finished_demos = finished;
                //Actualizamos datos del usuario
                //Usuarios actualizardatosUsuario = new Usuarios();

                //actualizardatosUsuario = (from u in db.Usuarios where (u.ID_usuario == ID) select u).FirstOrDefault();

                //actualizardatosUsuario.contador_visitas = actualizardatosUsuario.contador_visitas + 1;
                //actualizardatosUsuario.fultima_visita = DateTime.Now;

                //db.Entry(actualizardatosUsuario).State = EntityState.Modified;
                //db.SaveChanges();
                //**************************
                //VISITAS
                //SELECCIONAMOS RUTAS
                var rutas = db.VisitsM.ToList();

                //ESTADISTICA DE RUTAS POR ESTADO
                int totalRutas = rutas.Count();

                int onhold = (from e in db.VisitsM where (e.ID_visitstate == 3) select e).Count();
                int inprogress = (from e in db.VisitsM where (e.ID_visitstate == 2) select e).Count();
                int canceled = (from e in db.VisitsM where (e.ID_visitstate == 1) select e).Count();
                int finished = (from e in db.VisitsM where (e.ID_visitstate == 4) select e).Count();


                ViewBag.onhold = onhold;
                ViewBag.inprogress = inprogress;
                ViewBag.canceled = canceled;
                ViewBag.finished = finished;

                if (totalRutas != 0)
                {
                    ViewBag.onholdP = ((Convert.ToDecimal(onhold) / totalRutas) * 100);
                    ViewBag.inprogressP = ((Convert.ToDecimal(inprogress) / totalRutas) * 100);
                    ViewBag.canceledP = ((Convert.ToDecimal(canceled) / totalRutas) * 100);
                    ViewBag.finishedP = ((Convert.ToDecimal(finished) / totalRutas) * 100);
                }
                else
                {

                    ViewBag.onholdP = 0;
                    ViewBag.inprogressP = 0;
                    ViewBag.canceledP = 0;
                    ViewBag.finishedP = 0;
                }
                //Agregamos los representantes
                foreach (var itemVisita in rutas) {
                    var nombreRep = "";
                    var reps = (from e in db.VisitsM_representatives where (e.ID_visit == itemVisita.ID_visit) select e).ToList();

                    foreach (var itemrep in reps) {
                        var usuario = (from u in db.Usuarios where (u.ID_usuario == itemrep.ID_usuario) select u).FirstOrDefault();
                        if (reps.Count() == 1)
                        {
                            nombreRep = usuario.nombre + " " + usuario.apellido;
                        }
                        else if(reps.Count() >1) {
                            nombreRep += usuario.nombre + " " + usuario.apellido + ", ";
                        }
                    }
                    //utiliamos esta variable para el nombre del representante
                    itemVisita.city = nombreRep;
                }



                //MAPA DE RUTAS
                var demos_map = rutas;

                


                // Convertimos la lista a array
                ArrayList myArrList = new ArrayList();
                myArrList.AddRange((from p in demos_map
                                    select new
                                    {
                                        id = p.ID_route,
                                        representatives = p.city,
                                        store = p.store,
                                        address = p.address,
                                        GeoLong = p.geoLong,
                                        GeoLat = p.geoLat,
                                        demo_state = p.ID_visitstate,
                                        customer = p.customer,
                                        date = p.visit_date.ToShortDateString(),
                                        comment = p.comments
                                    }).ToList());

                ViewBag.routes_map = JsonConvert.SerializeObject(myArrList);

                //LISTADO DE REPRESENTANTES
                var usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9).Include(u => u.Tipo_membresia).Include(u => u.Roles);
                ViewBag.usuarios = usuarios.ToList();

                //LISTADO DE TIENDAS
                var stores = (from b in CMKdb.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "" && b.QryGroup30 == "Y" && b.validFor == "Y") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.stores = stores.ToList();

                //LISTADO DE CLIENTES
                var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.customers = customers.ToList();

                ViewBag.visitas = rutas;

                var activities = (from a in db.ActivitiesM select a).ToList();
                ViewBag.actlist = activities;
               

                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }


        }


        public ActionResult Index()
        {
            ViewBag.IPLOCAL = "";
            Session["ip_user"] = GetExternalIp();
            return View();
        }

        public ActionResult RoutesM()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                //VISITAS
                //SELECCIONAMOS RUTAS
                var rutas = db.RoutesM.OrderByDescending(d => d.date);
                var visitas = db.VisitsM.ToList();
                //ESTADISTICA DE RUTAS POR ESTADO DE VISITAS
                int totalRutas = visitas.Count();
                foreach (var rutait in rutas)
                {

                    int finishedorCanceled = (from e in db.VisitsM where ((e.ID_visitstate == 4 || e.ID_visitstate==1) && e.ID_route == rutait.ID_route) select e).Count();
                    totalRutas = (from e in db.VisitsM where ( e.ID_route == rutait.ID_route) select e).Count();

                    ViewBag.finished = finishedorCanceled;

                    if (totalRutas != 0)
                    {
                        rutait.query3 = ((Convert.ToDecimal(finishedorCanceled) / totalRutas) * 100).ToString();
                    }
                    else
                    {
                        rutait.query3 = "0";
                    }
                }

                //MAPA DE RUTAS
                var demos_map = (from a in db.RoutesM select a).ToList();


                // Convertimos la lista a array
                var usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9);
                // Convertimos la lista a array
                ArrayList myArrList = new ArrayList();
                myArrList.AddRange((from p in usuarios
                                    select new
                                    {
                                        id = p.ID_usuario,
                                        text = p.nombre + " " + p.apellido
                                    }).ToList());


                //LISTADO DE REPRESENTANTES
                
                ViewBag.usuarios = JsonConvert.SerializeObject(myArrList);
                //LISTADO DE RUTAS
                var rutass = CMKdb.C_ROUTES.OrderBy(c => c.Code);
                ViewBag.rutass = rutass.ToList();
                //LISTADO DE TIENDAS

                List<MyObj_tablapadre> listapadres = (from p in CMKdb.C_ROUTES select 
                                                      new MyObj_tablapadre{
                                                          id= p.Code,
                                                          text = p.Name
                                                      }
                                                      ).ToList();

                List<tablahijospadre> listahijas = (from p in CMKdb.C_ROUTE
                                             join store in CMKdb.OCRD on p.U_CardCode equals store.CardCode   
                                            select new tablahijospadre{
                                                id = p.U_CardCode,
                                                text = store.CardName.Replace("\"", "\\\""),
                                                parent = p.U_Route
                                            }).ToList();


                List<MyObj_tablapadre> categoriasList = ObtenerCategoriarJerarquiaByName(listapadres, listahijas);
  
                //var stores = (from b in CMKdb.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "" && b.QryGroup30 == "Y" && b.validFor == "Y") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.stores = JsonConvert.SerializeObject(categoriasList);
                //FIN LISTADO DE TIENDAS

                //LISTADO DE ACTIVIDADES

                List<MyObj_tablapadre> listapadresActivities = (from a in db.ActivitiesM_types
                                                      select
                                                         new MyObj_tablapadre
                                                         {
                                                             id = a.ID_activity.ToString(),
                                                             text = a.description
                                                         }
                                      ).ToList();

                List<tablahijospadre> listahijasActivities = (from p in db.FormsM
                                                    select new tablahijospadre
                                                    {
                                                        id = p.ID_form.ToString(),
                                                        text = p.name,
                                                        parent = p.ID_activity.ToString()
                                                    }).ToList();


                List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);


                ViewBag.activitieslist = JsonConvert.SerializeObject(categoriasListActivities);

                //LISTADO DE CLIENTES
                var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.customers = customers.ToList();


                return View(rutas.ToList());
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
       
        public ActionResult RoutesM_details(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                //VISITAS
                //SELECCIONAMOS RUTAS
                var rutas = db.VisitsM.Where(c=> c.ID_route ==id).ToList();

                //ESTADISTICA DE RUTAS POR ESTADO
                int totalRutas = rutas.Count();

                int onhold = (from e in db.VisitsM where (e.ID_visitstate == 3  && e.ID_route == id) select e).Count();
                int inprogress = (from e in db.VisitsM where (e.ID_visitstate == 2 && e.ID_route == id) select e).Count();
                int canceled = (from e in db.VisitsM where (e.ID_visitstate == 1 && e.ID_route == id) select e).Count();
                int finished = (from e in db.VisitsM where (e.ID_visitstate == 4 && e.ID_route == id) select e).Count();


                ViewBag.onhold = onhold;
                ViewBag.inprogress = inprogress;
                ViewBag.canceled = canceled;
                ViewBag.finished = finished;

                if (totalRutas != 0)
                {
                    ViewBag.onholdP = ((Convert.ToDecimal(onhold) / totalRutas) * 100);
                    ViewBag.inprogressP = ((Convert.ToDecimal(inprogress) / totalRutas) * 100);
                    ViewBag.canceledP = ((Convert.ToDecimal(canceled) / totalRutas) * 100);
                    ViewBag.finishedP = ((Convert.ToDecimal(finished) / totalRutas) * 100);
                }
                else
                {

                    ViewBag.onholdP = 0;
                    ViewBag.inprogressP = 0;
                    ViewBag.canceledP = 0;
                    ViewBag.finishedP = 0;
                }
                //Agregamos los representantes
                foreach (var itemVisita in rutas)
                {
                    var nombreRep = "";
                    var reps = (from e in db.VisitsM_representatives where (e.ID_visit == itemVisita.ID_visit) select e).ToList();

                    foreach (var itemrep in reps)
                    {
                        var usuario = (from u in db.Usuarios where (u.ID_usuario == itemrep.ID_usuario) select u).FirstOrDefault();
                        if (reps.Count() == 1)
                        {
                            nombreRep = usuario.nombre + " " + usuario.apellido;
                        }
                        else if (reps.Count() > 1)
                        {
                            nombreRep += usuario.nombre + " " + usuario.apellido + ", ";
                        }
                    }
                    //utiliamos esta variable para el nombre del representante
                    itemVisita.city = nombreRep;
                }



                //MAPA DE RUTAS
                var demos_map = rutas;




                // Convertimos la lista a array
                ArrayList myArrList = new ArrayList();
                myArrList.AddRange((from p in demos_map
                                    select new
                                    {
                                        id = p.ID_route,
                                        representatives = p.city,
                                        store = p.store,
                                        address = p.address,
                                        GeoLong = p.geoLong,
                                        GeoLat = p.geoLat,
                                        demo_state = p.ID_visitstate,
                                        customer = p.customer,
                                        date = p.visit_date.ToShortDateString(),
                                        comment = p.comments
                                    }).ToList());

                ViewBag.routes_map = JsonConvert.SerializeObject(myArrList);

                //LISTADO DE REPRESENTANTES
                var usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9).Include(u => u.Tipo_membresia).Include(u => u.Roles);
                ViewBag.usuarios = usuarios.ToList();


                //LISTADO DE TIENDAS
                var stores = (from b in CMKdb.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "" && b.QryGroup30 == "Y" && b.validFor == "Y") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.stores = stores.ToList();

                //LISTADO DE CLIENTES
                var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.customers = customers.ToList();

                var ruta = (from r in db.RoutesM where (r.ID_route == id) select r).FirstOrDefault();

                ViewBag.routename = ruta.query2;

                ViewBag.visitas = rutas;
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        public ActionResult Dashboard_merchandising()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                //Consultamos los recursos

                var recursos = (from b in db.Recursos_usuario where (b.ID_usuario == ID && b.ID_tiporecurso == 1) select b).FirstOrDefault();

                if (recursos != null)
                {
                    ViewBag.url = recursos.url;
                    ViewBag.bloquearcontenido = "si";
                    return View();
                }
                else
                {
                    TempData["advertencia"] = "No resources to show.";
                    return RedirectToAction("Main");
                }

            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Dashboard_demos()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                //Consultamos los recursos

                var recursos = (from b in db.Recursos_usuario where (b.ID_usuario == ID && b.ID_tiporecurso == 2) select b).FirstOrDefault();

                if (recursos != null)
                {
                    ViewBag.url = recursos.url;
                    ViewBag.bloquearcontenido = "si";
                    return View();
                }
                else
                {
                    TempData["advertencia"] = "No resources to show.";
                    return RedirectToAction("Main");
                }

            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Iniciar_sesion(string usuariocorreo, string password)
        {
            //Validamos del lado del cliente que ambos parametros no vengan vacios
            try
            {
                var obj = (from c in db.Usuarios where (c.correo == usuariocorreo && c.contrasena == password) select c).FirstOrDefault();
                if (obj != null)
                {
                    //PARA DASHBOARD
                    //(obj.Tipo_membresia.descripcion == "Demo")

                    //PARA DEMOS
                    //(obj.Tipo_membresia.descripcion == "Professional" || obj.Tipo_membresia.descripcion == "Enterprise" || obj.Tipo_membresia.descripcion == "Premium")

                    //PARA MERCHANDISING
                    //((obj.Tipo_membresia.descripcion == "Demo") || (obj.Tipo_membresia.descripcion == "Professional" || obj.Tipo_membresia.descripcion == "Enterprise" || obj.Tipo_membresia.descripcion == "Premium"))

                    if ((obj.Tipo_membresia.descripcion == "Demo") || (obj.Tipo_membresia.descripcion == "Professional" || obj.Tipo_membresia.descripcion == "Enterprise" || obj.Tipo_membresia.descripcion == "Premium"))
                    {
                        return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Session["IDusuario"] = obj.ID_usuario.ToString();
                        Session["tipousuario"] = obj.ID_tipomembresia.ToString();
                        Session["tiporol"] = obj.ID_rol.ToString();
                        Session["ultimaconexion"] = "";
                        GlobalVariables.ID_EMPRESA_USUARIO = Convert.ToInt32(obj.ID_empresa);

                        //OJO: SOLO PARA DEMOS, ESTO EVALUA SI UN USUARIO DEMO NUEVO COMPLETO EL FORMULARIO. ****ELIMINAR CUANDO SE UTILICE DASHBOARD
                        //6 demos ,rol: 7 demo_user
                        //if (obj.ID_tipomembresia == 6 && obj.ID_rol == 7) {
                        //    var formw9 = (from a in db.user_form_w9 where (a.ID_usuario == obj.ID_usuario) select a).FirstOrDefault();

                        //    if (formw9 != null)
                        //    {
                        //        if (formw9.iscomplete == false && string.IsNullOrEmpty(formw9.name))
                        //        {
                        //            return Json(new { success = true, redireccion = formw9.ID_form }, JsonRequestBehavior.AllowGet);
                        //        }
                        //        else if (formw9.iscomplete = false && formw9.name != "")
                        //        {
                        //            return Json(new { success = false, redireccion = "" }, JsonRequestBehavior.AllowGet);

                        //        }


                        //    }
                        //    else {
                        //        return Json(new { success = false, redireccion = "" }, JsonRequestBehavior.AllowGet);
                        //    }


                        //}


                        //Verificamos si el usuario esta activo
                        if (obj.activo != false)
                        {
                            var ultimaconexion = (from b in db.historial_conexiones where (b.ID_usuario == obj.ID_usuario) select b).OrderByDescending(b => b.fecha_conexion).FirstOrDefault();
                            if (ultimaconexion == null)
                            {
                                Session["ultimaconexion"] = "";
                                return Json(new { success = true, redireccion = "" }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                Session["ultimaconexion"] = Convert.ToDateTime(ultimaconexion.fecha_conexion).ToLocalTime();
                                return Json(new { success = true, redireccion = "" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else {

                            return Json(new { success = false, redireccion = "" }, JsonRequestBehavior.AllowGet);
                        }


                        
                    }
                    }


                    //return RedirectToAction("Main");
                else
                {
                    //Si ingreso mal la contraseña o el usuario no existe
                    return Json(new { success = false, redireccion = "" }, JsonRequestBehavior.AllowGet);
                    //TempData["advertencia"] = "Wrong email or password.";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, redireccion = "" }, JsonRequestBehavior.AllowGet);
                //TempData["error"] = "An error was handled ." + exception;
                //return RedirectToAction("Index");
            }

        }




        public ActionResult Cerrar_sesion()
        {
            Session.RemoveAll();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Historial_conexiones(historial_conexiones datosusuario)
        {
            if (datosusuario != null)
            {
                try
                {
                    if (datosusuario.ip == null) {
                        datosusuario.ip = "";
                    }
                    if (datosusuario.hostname == null)
                    {
                        datosusuario.hostname = "";
                    }
                    if (datosusuario.typeh == null)
                    {
                        datosusuario.typeh = "";
                    }
                    if (datosusuario.continent_name == null)
                    {
                        datosusuario.continent_name = "";
                    }
                    if (datosusuario.country_code == null)
                    {
                        datosusuario.country_code = "";
                    }
                    if (datosusuario.country_name == null)
                    {
                        datosusuario.country_name = "";
                    }
                    if (datosusuario.region_code == null)
                    {
                        datosusuario.region_code = "";
                    }
                    if (datosusuario.region_name == null)
                    {
                        datosusuario.region_name = "";
                    }
                    if (datosusuario.city == null)
                    {
                        datosusuario.city = "";
                    }
                    if (datosusuario.latitude == null)
                    {
                        datosusuario.latitude = "";
                    }
                    if (datosusuario.longitude == null)
                    {
                        datosusuario.longitude = "";
                    }

                    int ID = Convert.ToInt32(Session["IDusuario"]);
                    datosusuario.ID_usuario = ID;
                    datosusuario.fecha_conexion = DateTime.UtcNow;

                    db.historial_conexiones.Add(datosusuario);
                    db.SaveChanges();

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                catch {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

               
            }
            else {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

           
        }

        public string GetExternalIp()
        {

            return GetIPAddress(HttpContext.Request.ServerVariables["HTTP_VIA"],
                                                HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"],
                                                HttpContext.Request.UserHostAddress);
            //string ip = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if (string.IsNullOrEmpty(ip))
            //{
            //    ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            //}

            //if (string.IsNullOrEmpty(ip))
            //{
            //    ip = HttpContext.Request.UserHostAddress;
            //}


            //try
            //{
            //    if (ip.Length > 0)
            //    {
            //        return ip.Substring(0, ip.LastIndexOf(':'));
            //    }
            //    else
            //    {
            //        return "";

            //    }
            //}
            //catch (Exception ex)
            //{
            //    var mensaje = ex.Message;
            //    return "";
            //}



        }


            public static string GetIPAddress(string HttpVia, string HttpXForwardedFor, string RemoteAddr)
        {
            // Use a default address if all else fails.
            string result = "127.0.0.1";

            // Web user - if using proxy
            string tempIP = string.Empty;
            if (HttpVia != null)
                tempIP = HttpXForwardedFor;
            else // Web user - not using proxy or can't get the Client IP
                tempIP = RemoteAddr;

            // If we can't get a V4 IP from the above, try host address list for internal users.
            if (!IsIPV4(tempIP) || tempIP == "127.0.0.1 ")
            {
                try
                {
                    string hostName = System.Net.Dns.GetHostName();
                    foreach (System.Net.IPAddress ip in System.Net.Dns.GetHostAddresses(hostName))
                    {
                        if (IsIPV4(ip))
                        {
                            result = ip.ToString();
                            break;
                        }
                    }
                }
                catch { }
            }
            else
            {
                result = tempIP;
            }

            return result;
        }

        /// <summary>
        /// Determines weather an IP Address is V4
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>Is IPV4 True or False</returns>
        private static bool IsIPV4(string input)
        {
            bool result = false;
            System.Net.IPAddress address = null;

            if (System.Net.IPAddress.TryParse(input, out address))
                result = IsIPV4(address);

            return result;
        }

        /// <summary>
        /// Determines weather an IP Address is V4
        /// </summary>
        /// <param name="address">input IP address</param>
        /// <returns>Is IPV4 True or False</returns>
        private static bool IsIPV4(System.Net.IPAddress address)
        {
            bool result = false;

            switch (address.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:   // we have IPv4
                    result = true;
                    break;
                case System.Net.Sockets.AddressFamily.InterNetworkV6: // we have IPv6
                    break;
                default:
                    break;
            }

            return result;
        }
 

        public ActionResult Settings()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();
                //var registro_conexiones = (from b in db.historial_conexiones where (b.ID_usuario == ID) select b).OrderByDescending(b=> b.fecha_conexion).FirstOrDefault();
                ViewBag.usuario = datosUsuario.correo;

                //datos usuario cliente
                ViewBag.ultimavisita = Session["ultimaconexion"].ToString();
                ViewBag.empresa = datosUsuario.Empresas.nombre;
                ViewBag.cargo = datosUsuario.cargo;
                ViewBag.nombre = datosUsuario.nombre + " " + datosUsuario.apellido;
                ViewBag.correo = datosUsuario.correo;
                ViewBag.membresia = datosUsuario.Tipo_membresia.descripcion;
                ViewBag.ID_usuario = ID;
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Change_password(string password, string newpassword,string retrypassword)
        {
            if (Session["IDusuario"] != null)
            {

                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();


                if (password == datosUsuario.contrasena) {
                    if (newpassword == retrypassword)
                    {
                        Usuarios usuario = new Usuarios();
                        usuario = datosUsuario;

                        usuario.contrasena = newpassword;
                        db.Entry(usuario).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["exito"] = "Data saved succefully";

                        try
                        {
                            //Enviamos correo para notificar
                            dynamic email = new Email("email_changepassword");
                            email.To = usuario.correo;
                            email.From = "customercare@comerciamarketing.com";

                            email.NombreCliente = usuario.nombre + " " + usuario.apellido;
                            email.CorreoCliente = usuario.correo;
                            email.NuevaContrasena = usuario.contrasena;
                            email.Send();

                            //FIN email
                        }
                        catch
                        {

                        }

                        return RedirectToAction("Settings");
                    }
                    else {
                        TempData["advertencia"] = "Wrong data. Try again";
                        return RedirectToAction("Settings");
                    }
                }
                else
                {
                    TempData["advertencia"] = "Wrong password. Try again";
                    return RedirectToAction("Settings");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Gallery()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                //Consultamos los Vendors de SAP dependiendo el usuario

                    ViewBag.ID_vendor = new SelectList(CMKdb.OCRD.Where(b => b.Series == 61 && b.CardName != null && b.CardName != "").OrderBy(b => b.CardName), "CardCode", "CardName");
                    ViewBag.ID_demos = new SelectList(db.Demos.Where(b => b.ID_Vendor == "444445481"), "ID_demo", "visit_date");




                ViewBag.bloquearcontenido = "no";
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Galleryfilter(string ID_vendor, int? ID_demos)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                //Consultamos los Vendors de SAP
                if (Session["tipousuario"].ToString() == "1" || Session["tipousuario"].ToString() == "6")
                {
                    ViewBag.ID_vendor = new SelectList(CMKdb.OCRD.Where(b => b.Series == 61 && b.CardName != null && b.CardName != "").OrderBy(b => b.CardName), "CardCode", "CardName");
                }
                else
                {
                    ViewBag.ID_vendor = new SelectList(CMKdb.OCRD.Where(b => b.Series == 61 && b.CardName != null && b.CardName != "" && b.CardCode == datosUsuario.Empresas.ID_SAP).OrderBy(b => b.CardName), "CardCode", "CardName");
                    ID_vendor = datosUsuario.Empresas.ID_SAP;
                }

                var demos = (from b in db.Demos where (b.ID_Vendor == ID_vendor) select b).ToList();

                IEnumerable<SelectListItem> selectList = from s in demos
                                                         select new SelectListItem
                                                         {
                                                             Value = Convert.ToString(s.ID_demo),
                                                             Text = s.visit_date.ToShortDateString() + " - " + s.store.ToString()
                                                         };


                ViewBag.ID_demos = new SelectList(selectList, "Value", "Text", ID_demos);

                var demo_details_items = (from a in db.Forms_details where (a.ID_demo==ID_demos && a.ID_formresourcetype == 5) select a).ToList();

                for (int i = demo_details_items.Count - 1; i >= 0; i--)
                {
                    if (demo_details_items[i].fsource == "") demo_details_items.RemoveAt(i);
                }

                if (demo_details_items.Count > 0)
                {
                    ViewBag.bloquearcontenido = "no";
                    ViewBag.imagenes = demo_details_items;

                    return View();
                }
                else
                {
                    ViewBag.imagenes = demo_details_items;
                    if (ID_demos != null) {
                        TempData["advertencia"] = "No images to show.";
                    }
                    
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult GetDemosGallery(string vendorID)
        {
            //List<Demos> lstdemos = new List<Demos>();
            //lstdemos = (db.Demos.Where(x => x.ID_Vendor == vendoriD)).ToList();
            
            //JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            //string result = javaScriptSerializer.Serialize(lstdemos);
            return this.Json((from obj in db.Demos where(obj.ID_Vendor == vendorID) select new { ID_demo = obj.ID_demo, visit_date = obj.visit_date, store = obj.store }), JsonRequestBehavior.AllowGet);
        }


        //MERCHANDISING

        public ActionResult Representatives()
        {
            if (Session["IDusuario"] != null)
            {
                //GENERAL
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                //GENERAL END
                //2 es la empresa DEFAULT
                var usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9).Include(u => u.Tipo_membresia).Include(u => u.Roles);


                return View(usuarios.ToList());




            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        public ActionResult CustomersM()
        {
            if (Session["IDusuario"] != null)
            {
                //GENERAL
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                //GENERAL END
                //2 es la empresa DEFAULT
                var customers = (from b in CMKdb.OCRD where(b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                ViewBag.customers = customers.ToList();

                return View();




            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult StoresM()
        {
            if (Session["IDusuario"] != null)
            {
                //GENERAL
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                //GENERAL END
                //2 es la empresa DEFAULT
                var stores = (from b in CMKdb.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "" && b.QryGroup30 == "Y" && b.validFor == "Y") select b).OrderBy(b => b.CardName).ToList();

                ViewBag.stores = stores.ToList();

                return View();




            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Representative_stats(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;



                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult StoreM_stats(string id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;



                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult CustomerM_stats(string id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;



                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult FormsM()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                var forms = (from a in db.FormsM select a

                             
                             );

                foreach (var item in forms) {
                   var tp = (from b in db.ActivitiesM_types where(b.ID_activity == item.ID_activity) select b).FirstOrDefault();
                    if (tp == null) { item.query1 = ""; } else {
                        item.query1 = tp.description;
                    } 
                }


                ViewBag.ID_activity = new SelectList(db.ActivitiesM_types, "ID_activity", "description");
                //Seleccionamos los tipos de recursos a utilizar en el caso de Merchandising

                List<string> uids = new List<string>() { "1", "3", "5", "6","8","9","12","13","14","15","16","17","18","19","20","21" };

                ViewBag.ID_formresourcetype = new SelectList(db.form_resource_type.Where(c=> uids.Contains(c.ID_formresourcetype.ToString())).OrderBy(c=>c.fdescription), "ID_formresourcetype", "fdescription");
                ViewBag.vendors = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                ViewBag.formslist = forms.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        //MERCHANDISING
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRoute(string descriptionN,DateTime date,DateTime enddate, string listatiendas, string listarepresentantes, string idform,string listatiposactividades)
        {
            try
            {

                //Comenzamos con el maestro de rutas
                RoutesM rutamaestra = new RoutesM();

                rutamaestra.date = date;
                rutamaestra.end_date = enddate;
                rutamaestra.query1 = listatiposactividades;
                rutamaestra.query2 = descriptionN;
                rutamaestra.query3 = "";

                rutamaestra.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;

                db.RoutesM.Add(rutamaestra);
                db.SaveChanges();
                //FIN ruta maestra



                //Guardamos detalle de visita
                //Se guarda el detalle por cada tienda a visitar
                List<string> storeIds = listatiendas.Split(',').ToList();

                foreach (var store in storeIds) {
                    var storeSAP =(from s in CMKdb.OCRD where(s.CardCode==store) select s).FirstOrDefault();
                    if (storeSAP != null)
                    {
                        VisitsM visita = new VisitsM();
                        visita.ID_customer = "";
                        visita.customer = "";
                        visita.ID_store = store;
                        visita.store = storeSAP.CardName;
                        visita.address = storeSAP.MailAddres;
                        visita.city = storeSAP.MailCity;
                        if (storeSAP.MailZipCod == null)
                        {
                            visita.zipcode = "";
                        }
                        else { visita.zipcode = storeSAP.MailZipCod; }
                        
                        if (storeSAP.State2 == null) {
                            visita.state = "";
                        }
                        else { visita.state = storeSAP.State2; }
                        visita.visit_date = date;
                        visita.ID_visitstate = 3; //On Hold
                        visita.comments = "";
                        visita.check_in = date;
                        visita.check_out = date;
                        visita.end_date = date;
                        visita.extra_hours = 0;
                        visita.ID_route = rutamaestra.ID_route;
                        visita.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;
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
                            visita.geoLat = lng.Value;
                            visita.geoLong = lat.Value;
                            //FIN

                        }
                        catch
                        {
                            visita.geoLong = "";
                            visita.geoLat = "";
                        }

                        db.VisitsM.Add(visita);
                        db.SaveChanges();


                        List<int> repIds = listarepresentantes.Split(',').Select(int.Parse).ToList();

                        foreach (var rep in repIds)
                        {

                            if (rep != 0)
                            {
                                VisitsM_representatives repvisita = new VisitsM_representatives();

                                repvisita.ID_visit = visita.ID_visit;
                                repvisita.ID_usuario = rep;
                                repvisita.query1 = "";
                                repvisita.ID_empresa= visita.ID_empresa;
                                db.VisitsM_representatives.Add(repvisita);
                                db.SaveChanges();
                            }

                        }


                    }

                //FIN detalle visita

                   


                }
                //FIn detalle de representantes

                TempData["exito"] = "Route created successfully.";
                return RedirectToAction("RoutesM", "Home", null);

            }
            catch (Exception ex) {
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("RoutesM", "Home", null);
            }

            

        }

        //CREACION DE JERARQUIAS Y OBJETOS
        //TIENDAS Y RUTAS (Se utiliza en Routes)
        public class tablahijospadre
        {
            public string id { get; set; }
            public string text { get; set; }
            public string parent { get; set; }
        }

        public class MyObj_tablapadre
        {
            public string id { get; set; }
            public string text { get; set; }
            public List<MyObj_tablapadre> children { get; set; }
        }


        //CABE MENCIONAR QUE CON ESTOS DOS METODOS SE RELACIONA EL PADRE POR LA CARACTERISTICA TEXT LO CUAL VIENE SIENDO EQUIVALENTE
        //AL NOMBRE O DESCRIPCION DEL ITEM. ESTO SE HIZO POR LA RELACION EN LA BASE DE DATOS QUE ES PARA RUTAS, PERO EN TEORIA TENDRIA QUE SER POR ID Y NO POR NAME
        public static List<MyObj_tablapadre> ObtenerCategoriarJerarquiaByName(List<MyObj_tablapadre> Categoriaspadre, List<tablahijospadre> categoriashijas)
        {


            List<MyObj_tablapadre> query = (from item in Categoriaspadre

                                 select new MyObj_tablapadre
                                 {
                                     id = "", //SI QUEREMOS AGRUPAR POR ID SE LO PONEMOS, SINO SE LO QUITAMOS PARA QUE NOS CARGUE LAS TIENDAS DESPLEGADAS
                                     text = item.text.Replace("'", ""),
                                     children = ObtenerHijos(item.text, categoriashijas)
                                 }).ToList();

            return query;





        }

        private static List<MyObj_tablapadre> ObtenerHijos(string Categoria, List<tablahijospadre> categoriashijas)
        {



            List<MyObj_tablapadre> query = (from item in categoriashijas

                                 where item.parent == Categoria
                                 select new MyObj_tablapadre
                                 {
                                     id = item.id,
                                     text = item.text.Replace("'", ""),
                                     children = null
                                 }).ToList();

            return query;

        }


        //FIN ROUTES
        public static List<MyObj_tablapadre> ObtenerCategoriarJerarquiaByID(List<MyObj_tablapadre> Categoriaspadre, List<tablahijospadre> categoriashijas)
        {


            List<MyObj_tablapadre> query = (from item in Categoriaspadre

                                            select new MyObj_tablapadre
                                            {
                                                id = "", //SI QUEREMOS AGRUPAR POR ID SE LO PONEMOS, SINO SE LO QUITAMOS PARA QUE NOS CARGUE LAS TIENDAS DESPLEGADAS
                                                text = item.text.Replace("'", ""),
                                                children = ObtenerHijos(item.id, categoriashijas)
                                            }).ToList();

            return query;





        }

        private static List<MyObj_tablapadre> ObtenerHijosByID(string Categoria, List<tablahijospadre> categoriashijas)
        {



            List<MyObj_tablapadre> query = (from item in categoriashijas

                                            where item.parent == Categoria
                                            select new MyObj_tablapadre
                                            {
                                                id = item.id,
                                                text = item.text.Replace("'", ""),
                                                children = null
                                            }).ToList();

            return query;

        }
        //Se utiliza para formularios (en Routes)

        //MERCHANDISING ACTIVITIES 
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

        public ActionResult Getformdata(string IDform)
        {

            FormsM form = db.FormsM.Find(Convert.ToInt32(IDform));

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = form.query2;

            //Deserealizamos  los datos
            JavaScriptSerializer js = new JavaScriptSerializer();
            MyObj[] details = js.Deserialize<MyObj[]>(form.query2);

            return Json(details, JsonRequestBehavior.AllowGet);
        }
    }
}
