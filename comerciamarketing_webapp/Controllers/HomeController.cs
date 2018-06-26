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



        public ActionResult Main(string startdate, string finishdate)
        {

            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();
                //var registro_conexiones = (from b in db.historial_conexiones where (b.ID_usuario == ID) select b).OrderByDescending(b=> b.fecha_conexion).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;
                ViewBag.tipomembresia = datosUsuario.Tipo_membresia.descripcion;
                ViewBag.ultimavisita = Session["ultimaconexion"].ToString();//datosUsuario.fultima_visita.ToString();
                ViewBag.bloquearcontenido = "si";


                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (startdate == null)
                {
                    if (finishdate == null)
                    {


                    }

                }
                else {
                    if (finishdate == null)
                    {

                    }
                    else {
                        try
                        {
                            sunday = Convert.ToDateTime(startdate);
                            saturday = Convert.ToDateTime(finishdate);
                        }
                        catch {
                            sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                            saturday = sunday.AddDays(6).AddHours(23);
                        }


                    }

                }



                int onhold = (from e in db.Demos where (e.ID_demostate == 3 && e.visit_date >= sunday && e.end_date <= saturday) select e).Count();
                int inprogress = (from e in db.Demos where (e.ID_demostate == 2 && e.visit_date >= sunday && e.end_date <= saturday) select e).Count();
                int canceled = (from e in db.Demos where (e.ID_demostate == 1 && e.visit_date >= sunday && e.end_date <= saturday) select e).Count();
                int finished = (from e in db.Demos where (e.ID_demostate == 4 && e.visit_date >= sunday && e.end_date <= saturday) select e).Count();


                var demos_map = (from a in db.Demos where (a.visit_date >= sunday && a.end_date <= saturday) select a).ToList();


                //Asignamos la geoubicacion
                foreach (var item in demos_map) {


                    var usuario = (from h in CMKdb.OCRDs where (h.CardCode == item.ID_usuario) select h).FirstOrDefault();
                    if (usuario == null) { } else { item.ID_usuario = usuario.CardName; }
                    

                }


                // Convertimos la lista a array
                ArrayList myArrList = new ArrayList();
                myArrList.AddRange((from p in demos_map
                                    select new
                                    {
                                        id = p.ID_demo,
                                        nombre = p.ID_usuario,
                                        PlaceName = p.store,
                                        GeoLong = p.geoLong,
                                        GeoLat = p.geoLat,
                                        demo_state = p.Demo_state.sdescription,
                                        vendor = p.vendor,
                                        date = p.visit_date,
                                        comment = p.comments
                                    }).ToList());


                ViewBag.demos_map = JsonConvert.SerializeObject(myArrList);



                ViewBag.bloquearcontenido = "no";
                ViewBag.desdehasta = "From " + sunday.ToShortDateString() + " to " + saturday.ToShortDateString(); 
                ViewBag.onhold_demos = onhold;
                ViewBag.inprogress_demos = inprogress;
                ViewBag.canceled_demos = canceled;
                ViewBag.finished_demos = finished;
                //Actualizamos datos del usuario
                //Usuarios actualizardatosUsuario = new Usuarios();

                //actualizardatosUsuario = (from u in db.Usuarios where (u.ID_usuario == ID) select u).FirstOrDefault();

                //actualizardatosUsuario.contador_visitas = actualizardatosUsuario.contador_visitas + 1;
                //actualizardatosUsuario.fultima_visita = DateTime.Now;

                //db.Entry(actualizardatosUsuario).State = EntityState.Modified;
                //db.SaveChanges();
                //**************************

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

        public ActionResult Dashboard()
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


                    if (obj.Tipo_membresia.descripcion == "Professional" || obj.Tipo_membresia.descripcion == "Enterprise" || obj.Tipo_membresia.descripcion == "Premium")
                    {
                        return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Session["IDusuario"] = obj.ID_usuario.ToString();
                        Session["tipousuario"] = obj.ID_tipomembresia.ToString();
                        Session["ultimaconexion"] = "";


                        //OJO: SOLO PARA DEMOS, ESTO EVALUA SI UN USUARIO DEMO NUEVO COMPLETO EL FORMULARIO. ****ELIMINAR CUANDO SE UTILICE DASHBOARD
                        //6 demos ,rol: 7 demo_user
                        if (obj.ID_tipomembresia == 6 && obj.ID_rol == 7) {
                            var formw9 = (from a in db.user_form_w9 where (a.ID_usuario == obj.ID_usuario) select a).FirstOrDefault();

                            if (formw9 != null)
                            {
                                if (formw9.iscomplete == false && string.IsNullOrEmpty(formw9.name))
                                {
                                    return Json(new { success = true, redireccion = formw9.ID_form }, JsonRequestBehavior.AllowGet);
                                }
                                else if (formw9.iscomplete = false && formw9.name != "")
                                {
                                    return Json(new { success = false, redireccion = "" }, JsonRequestBehavior.AllowGet);

                                }


                            }
                            else {
                                return Json(new { success = false, redireccion = "" }, JsonRequestBehavior.AllowGet);
                            }


                        }



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
            catch 
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

                //Consultamos los Vendors de SAP
                ViewBag.ID_vendor = new SelectList(CMKdb.OCRDs.Where(b => b.Series == 61 && b.CardName != null && b.CardName != "").OrderBy(b => b.CardName), "CardCode", "CardName");
                ViewBag.ID_demos = new SelectList(db.Demos.Where(b => b.ID_Vendor == "444445481"), "ID_demo", "visit_date");
                //Recuperamos los IDS de las demos del customer especifico
                //int[] demo_ids = (from f in db.Demos where (f.ID_Vendor == datosUsuario.Empresas.ID_SAP) select f.ID_demo).ToArray();

                //var demo_details_items = (from b in db.Forms_details where (demo_ids.Contains(b.ID_demo) && b.ID_formresourcetype == 5) select b).ToList();

                //for (int i = demo_details_items.Count - 1; i >= 0; i--)
                //{
                //    if (demo_details_items[i].fsource =="") demo_details_items.RemoveAt(i);
                //   }

                //if (demo_details_items.Count > 0)
                //    {
                //        IEnumerable<SelectListItem> selectList = from s in demos
                //                                                 select new SelectListItem
                //                                                 {
                //                                                     Value = Convert.ToString(s.ID_demo),
                //                                                     Text = s.visit_date.ToShortDateString()  + " - " + s.store.ToString()
                //                                                 };


                //        ViewBag.ID_demos = new SelectList(selectList, "Value", "Text");


                //        ViewBag.bloquearcontenido = "no";
                //        //ViewBag.imagenes = demo_details_items;

                //        return View();
                //    }
                //    else
                //    {
                //        TempData["advertencia"] = "No images to show.";
                //        return View();
                //    }

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
                ViewBag.ID_vendor = new SelectList(CMKdb.OCRDs.Where(b => b.Series == 61 && b.CardName != null && b.CardName != "").OrderBy(b => b.CardName), "CardCode", "CardName",ID_vendor);
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
                    TempData["advertencia"] = "No images to show.";
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
    }
}
