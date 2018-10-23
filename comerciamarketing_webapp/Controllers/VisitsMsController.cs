﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using comerciamarketing_webapp.Models;
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

                    foreach (var itemac in activities) {
                        var uassigned = (from u in db.Usuarios where (u.ID_usuario == itemac.ID_usuarioEnd) select u).FirstOrDefault();
                        if (uassigned == null && itemac.ID_usuarioEnd == 0)
                        {
                            var usuario = (from a in CMKdb.OCRD where (a.CardCode == itemac.ID_usuarioEndString) select a).FirstOrDefault();
                            itemac.ID_customer = usuario.CardName.ToString() + " - " + usuario.E_Mail.ToString();
                        }
                        else {
                            itemac.ID_customer = uassigned.nombre + " " + uassigned.apellido;
                        }
                        
                       

                    }
                   

                    ViewBag.activities = activities;
                }
                else {
                    var activities = (from a in db.ActivitiesM where (a.ID_visit == id && a.ID_usuarioEnd==ID) select a).ToList();

                    foreach (var itemac in activities)
                    {
                        var uassigned = (from u in db.Usuarios where (u.ID_usuario == itemac.ID_usuarioEnd) select u).FirstOrDefault();

                        itemac.ID_customer = uassigned.nombre + " " + uassigned.apellido;

                    }
                    ViewBag.activities = activities;
                }
                //CHECK OUT POR USUARIO
                if (datosUsuario.ID_rol == 9 && datosUsuario.ID_tipomembresia == 8)
                {
                    var rep = (from a in db.VisitsM_representatives
                                where (a.ID_visit ==id && a.ID_usuario == datosUsuario.ID_usuario) select a
                          ).FirstOrDefault();
                    ViewBag.estadovisita = Convert.ToInt32(rep.query1);//Utilizaremos este campo para filtrar el estado por usuario
                }
                else {
                    ViewBag.estadovisita = visitsM.ID_visitstate;
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


                //Route
                ViewBag.route = visitsM.ID_route;


                var geoLong = "";
                var geoLat = "";

                geoLong = visitsM.geoLong;
                geoLat = visitsM.geoLat;

                ViewBag.glong = geoLong;
                ViewBag.glat = geoLat;
                ViewBag.address = visitsM.address + ", " + visitsM.zipcode  + ", " + visitsM.city  + ", " + visitsM.state;

                return View(visitsM);

            }
            else
            {
                return RedirectToAction("Index","Home", null);
            }


            
        }

        public ActionResult DetailsC(int? id, string ID_Customer)
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
                //FIN representantes
                //ACTIVITIES

                    var activities = (from a in db.ActivitiesM where (a.ID_visit == id && a.ID_customer==ID_Customer) select a).ToList();

                    foreach (var itemac in activities)
                    {
                        var uassigned = (from u in db.Usuarios where (u.ID_usuario == itemac.ID_usuarioEnd) select u).FirstOrDefault();

                        itemac.ID_customer = uassigned.nombre + " " + uassigned.apellido;

                    }


                    ViewBag.activities = activities;
                


                
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

                var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                ViewBag.customers = customers.ToList();


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

                nuevaActivida.date = DateTime.Today.Date;
                var form = (from c in db.FormsM where (c.ID_form == IDForm) select c).FirstOrDefault();
                if (form != null)
                {
                    nuevaActivida.description = form.name;
                    nuevaActivida.ID_activitytype = form.ID_activity;
                    nuevaActivida.query1 = form.query2;
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
                //Pero verificamos si ya existe

                    var existeenvisita = (from v in db.VisitsM_representatives where (v.ID_visit == nuevaActivida.ID_visit && v.ID_usuario == IDRep) select v).Count();


                if (existeenvisita > 0)

                {
                }
                else
                {

                    if (esDemoUser == 1)

                    {

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
                            brandstoshow = items.fdescription.ToString();
                        }
                        else
                        {
                            brandstoshow += ", " + items.fdescription.ToString();
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
                return RedirectToAction("Details", "VisitsMs", new { id= ID_visita});
            }
            catch (Exception ex){
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Details", "VisitsMs", new { id = ID_visita });
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
                        TempData["advertencia"] = "Something wrong happened, try again.";
                        return RedirectToAction("Details", "VisitsMs", new { id = IDVisit });
                    }
                    else
                    {
                        //COPIAMOS Y GUARDAMOS LA NUEVA ACTIVIDAD

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
                        var detalles_acopiar = (from a in db.FormsM_details where (a.ID_formM == nuevaActivida.ID_form && a.original == true) select a).ToList();

                        foreach (var item in detalles_acopiar)
                        {
                            FormsM_details nuevodetalle = new FormsM_details();
                            nuevodetalle = item;
                            nuevodetalle.original = false;
                            nuevodetalle.ID_visit = nuevaActivida.ID_activity; //TOMAREMOS ID VISIT COMO ID ACTIVITY PORQUE ES POR REPRESENTANTE Y NO POR VISITA

                            db.FormsM_details.Add(nuevodetalle);
                            db.SaveChanges();
                        }


                        TempData["exito"] = "Activity created successfully.";
                        return RedirectToAction("Details", "VisitsMs", new { id = IDVisit });

                    }


                }
                catch (Exception ex)
                {
                    TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                    return RedirectToAction("Details", "VisitsMs", new { id = IDVisit });
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
                return RedirectToAction("Details", "VisitsMs", new { id = ID_visitCa });

            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                return RedirectToAction("Details", "VisitsMs", new { id = ID_visitCa });
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

        public ActionResult GalleryC(string id, string modulo, string IDcustomer)
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
                    var actividadesList = (from e in db.ActivitiesM where (e.ID_visit == IDV && e.ID_customer == IDcustomer) select e).ToList();
                    var actividades = (from a in db.ActivitiesM where (a.ID_visit == IDV && a.ID_customer == IDcustomer) select a.ID_activity).ToArray();

                    var detalles = (from b in db.FormsM_details where (actividades.Contains(b.ID_visit) && b.ID_formresourcetype == 5) select b).ToList();


                    foreach (var item in detalles)
                    {
                        var f = (from c in actividadesList where (c.ID_activity == item.ID_visit) select c).FirstOrDefault();

                        item.fvalueText = f.Customer;
                        item.fdescription = f.description;
                        item.query1 = f.check_out.ToShortDateString();

                    }


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
                        return RedirectToAction("Details", "VisitsMs", new { id = IDV });
                    }
                    else
                    {
                        TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                        return RedirectToAction("DetailsC", "VisitsMs", new { id = IDV, ID_customer = IDcustomer });
                    }

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
                        return RedirectToAction("Details", "VisitsMs", new { id = IDV });
                    }
                    else
                    {
                        TempData["advertencia"] = "Something wrong happened, try again." + ex.Message;
                        return RedirectToAction("Details", "VisitsMs", new { id = IDV });
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
                if (visita != null)
                {
                    //Cambiamos estado de visita global
                    visita.ID_visitstate = 2;
                    visita.check_in = Convert.ToDateTime(check_in);
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
                int idid = Convert.ToInt32(ID_visit);
                int IDU = Convert.ToInt32(Session["IDusuario"]);

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
                else { 

                 return Json(new { Result = "There are some incomplete activities. Please check and try again" });
                } 
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error: " + ex.Message });
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
