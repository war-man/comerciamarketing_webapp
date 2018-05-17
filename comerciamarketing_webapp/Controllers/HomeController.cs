﻿using comerciamarketing_webapp.Models;
using Postal;
using System;
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

namespace comerciamarketing_webapp.Controllers
{

    public class HomeController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();
        public ActionResult Main()
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
                        var ultimaconexion = (from b in db.historial_conexiones where (b.ID_usuario == obj.ID_usuario) select b).OrderByDescending(b => b.fecha_conexion).FirstOrDefault();
                        if (ultimaconexion == null)
                        {
                            Session["ultimaconexion"] = "";
                            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            Session["ultimaconexion"] = Convert.ToDateTime(ultimaconexion.fecha_conexion).ToLocalTime();
                            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                        }
                        
                    }
                    }


                    //return RedirectToAction("Main");
                else
                {
                    //Si ingreso mal la contraseña o el usuario no existe
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                    //TempData["advertencia"] = "Wrong email or password.";
                    //return RedirectToAction("Index");
                }
            }
            catch 
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
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
                    datosusuario.fecha_conexion = DateTime.Now;

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
    }
}
