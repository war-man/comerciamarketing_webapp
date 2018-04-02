using comerciamarketing_webapp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

                ViewBag.usuario = datosUsuario.usuario;
                ViewBag.nomusuarioSAP = datosUsuario.NOM_clienteSAP;
                ViewBag.tipomembresia = datosUsuario.Tipo_membresia.descripcion;
                ViewBag.ultimavisita = datosUsuario.fultima_visita.ToString();
                ViewBag.bloquearcontenido = "si";

                //Actualizamos datos del usuario
                Usuarios actualizardatosUsuario = new Usuarios();

                actualizardatosUsuario = (from u in db.Usuarios where (u.ID_usuario == ID) select u).FirstOrDefault();

                actualizardatosUsuario.contador_visitas = actualizardatosUsuario.contador_visitas + 1;
                actualizardatosUsuario.fultima_visita = DateTime.Now;

                db.Entry(actualizardatosUsuario).State = EntityState.Modified;
                db.SaveChanges();
                //**************************

                return View();
            }
            else {
                return RedirectToAction("Index");
            }

                
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.usuario;
                ViewBag.nomusuarioSAP = datosUsuario.NOM_clienteSAP;

                //Consultamos los recursos

                var recursos = (from b in db.Recursos_usuario where (b.ID_usuario == ID && b.tipo_recurso == 1) select b).FirstOrDefault();

                if (recursos != null)
                {
                    ViewBag.url = recursos.url;
                    ViewBag.bloquearcontenido = "si";
                    return View();
                }
                else {
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
                var obj = (from c in db.Usuarios where (c.usuario == usuariocorreo && c.contrasena == password) select c).FirstOrDefault();
                if (obj != null)
                {

                        Session["IDusuario"] = obj.ID_usuario.ToString();
                        Session["tipousuario"] = obj.ID_tipomembresia.ToString();
                        return RedirectToAction("Main");
                    

                }
                else
                {
                    //Si ingreso mal la contraseña o el usuario no existe
                    TempData["advertencia"] = "Wrong email or password.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                TempData["error"] = "An error was handled ." + exception;
                return RedirectToAction("Index");
            }

        }

        public ActionResult Cerrar_sesion()
        {
            Session.RemoveAll();
            return RedirectToAction("Index");
        }
    }
}