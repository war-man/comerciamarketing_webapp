using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using comerciamarketing_webapp.Models;

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
                VisitsM visitsM = db.VisitsM.Find(id);
                if (visitsM == null)
                {
                    return HttpNotFound();
                }

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
                if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)
                {
                    var activities = (from a in db.ActivitiesM where (a.ID_visit == id) select a).ToList();
                    ViewBag.activities = activities;
                }
                else {
                    var activities = (from a in db.ActivitiesM where (a.ID_visit == id && a.ID_usuarioEnd==ID) select a).ToList();
                    ViewBag.activities = activities;
                }



                ViewBag.estadovisita = visitsM.ID_visitstate;

                ViewBag.idvisita = id;

                ViewBag.storename = visitsM.store;

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

                var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                ViewBag.customers = customers.ToList();
                return View(visitsM);

            }
            else
            {
                return RedirectToAction("Index","Home", null);
            }


            
        }
        public class representativesVisit{
              public int ID { get; set; }
            public string name { get; set; }
            public string email { get; set; }
        }


      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateActivity(string ID_form, string ID_customer, string ID_visita, string ID_rep)
        {
            try
            {
                int IDForm = Convert.ToInt32(ID_form);
                int IDRep = Convert.ToInt32(ID_rep);

                //CREAMOS LA ESTRUCTURA DE LA ACTIVIDAD
                ActivitiesM nuevaActivida = new ActivitiesM();

                nuevaActivida.ID_form =  Convert.ToInt32(ID_form);
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
                var form = (from c in db.FormsM where (c.ID_form == IDForm) select c).FirstOrDefault();
                if (form != null)
                {
                    nuevaActivida.description = form.name;
                    nuevaActivida.ID_activitytype = form.ID_activity;
                    nuevaActivida.query1 = form.query2;
                }

                int ID_usuario = Convert.ToInt32(Session["IDusuario"]);
                nuevaActivida.ID_usuarioCreate = ID_usuario;
                nuevaActivida.ID_usuarioEnd = IDRep;  //Usuario que sera asignado
                nuevaActivida.date = DateTime.Today.Date;
               


                db.ActivitiesM.Add(nuevaActivida);
                db.SaveChanges();

                //LUEGO ASIGNAMOS LA PLANTILLA DE FORMULARIO A LA ACTIVIDAD
                //Guardamos el detalle
                var detalles_acopiar = (from a in db.FormsM_details where (a.ID_formM == IDForm && a.original == true) select a).ToList();

                foreach (var item in detalles_acopiar)
                {
                    FormsM_details nuevodetalle = new FormsM_details();
                    nuevodetalle = item;
                    nuevodetalle.original = false;
                    nuevodetalle.ID_visit = nuevaActivida.ID_activity; //TOMAREMOS ID VISIT COMO ID ACTIVITY PORQUE ES POR REPRESENTANTE Y NO POR VISITA

                    db.FormsM_details.Add(nuevodetalle);
                    db.SaveChanges();
                }


                //Por ultimo asignamos el usuario a la visita

                VisitsM_representatives repvisita = new VisitsM_representatives();

                        repvisita.ID_visit = nuevaActivida.ID_visit;
                        repvisita.ID_usuario = IDRep;
                        repvisita.query1 = "";
                        repvisita.ID_empresa= nuevaActivida.ID_empresa;
                        db.VisitsM_representatives.Add(repvisita);
                        db.SaveChanges();
                //************

                TempData["exito"] = "Activity created successfully.";
                return RedirectToAction("Details", "VisitsMs", new { id= ID_visita});
            }
            catch (Exception ex){
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Details", "VisitsMs", new { id = ID_visita });
            }
         
            


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteActivity(int ID_activityD, int ID_visitA)
        {
            try {

                ActivitiesM activity = db.ActivitiesM.Find(ID_activityD);
                db.ActivitiesM.Remove(activity);
                db.SaveChanges();

                //Eliminamos el detalle que genero la actividad en FormsM_details
                var lista_eliminar = (from c in db.FormsM_details where (c.ID_visit ==ID_activityD && c.original == false) select c).ToList();

                foreach (var item in lista_eliminar)
                {
                    FormsM_details detailstodelete = db.FormsM_details.Find(item.ID_details);
                    db.FormsM_details.Remove(detailstodelete);
                    db.SaveChanges();

                }
                TempData["exito"] = "Activity deleted successfully.";
                return RedirectToAction("Details","VisitsMs", new { id=ID_visitA});

            }
            catch (Exception ex){
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Details", "VisitsMs", new { id = ID_visitA });
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
                return RedirectToAction("Details", "VisitsMs", new { id = ID_visitU });

            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Details", "VisitsMs", new { id = ID_visitU });
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


        //CHECK IN
        public ActionResult Check_in(string ID_visit, string check_in, string lat, string lng)
        {
            try
            {
                int IDU = Convert.ToInt32(Session["IDusuario"]);
                VisitsM visita = db.VisitsM.Find(Convert.ToInt32(ID_visit));
                if (visita != null)
                {
                    visita.ID_visitstate = 2;
                    visita.check_in = Convert.ToDateTime(check_in);
                    db.Entry(visita).State = EntityState.Modified;
                    db.SaveChanges();

                    
                    if (lat != null || lat != "")
                    {
                        //Guardamos el log de la actividad
                        ActivitiesM_log nuevoLog = new ActivitiesM_log();
                        nuevoLog.latitude = lat;
                        nuevoLog.longitude = lng;
                        nuevoLog.ID_usuario = IDU;
                        nuevoLog.ID_activity =0;
                        nuevoLog.fecha_conexion = Convert.ToDateTime(check_in);
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
        //CHECK OUT
        public ActionResult Check_out(string ID_visit, string check_in, string lat, string lng)
        {
            try
            {
                int IDU = Convert.ToInt32(Session["IDusuario"]);
                VisitsM visita = db.VisitsM.Find(Convert.ToInt32(ID_visit));
                if (visita != null)
                {
                    visita.ID_visitstate = 4; //FINALIZADO
                    visita.check_in = Convert.ToDateTime(check_in);
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
                        nuevoLog.fecha_conexion = Convert.ToDateTime(check_in);
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

        //CHECK OUT
        public ActionResult cancel_visit(string ID_visit, string check_in, string lat, string lng)
        {
            try
            {
                int IDU = Convert.ToInt32(Session["IDusuario"]);
                VisitsM visita = db.VisitsM.Find(Convert.ToInt32(ID_visit));
                if (visita != null)
                {
                    visita.ID_visitstate = 1; //FINALIZADO
                    visita.check_in = Convert.ToDateTime(check_in);
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
                        nuevoLog.fecha_conexion = Convert.ToDateTime(check_in);
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

    }
}
