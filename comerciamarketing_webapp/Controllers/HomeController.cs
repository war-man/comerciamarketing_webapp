using comerciamarketing_webapp.Models;
using System;
using System.Collections.Generic;
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

                    return View();
                }
                else {
                    TempData["advertencia"] = "Aún no han asignado un recurso para mostrar.";
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
                    
                    return RedirectToAction("Main");
                }
                else
                {
                    //Si ingreso mal la contraseña o el usuario no existe
                    TempData["advertencia"] = "Correo o contraseña incorrecta.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                TempData["error"] = "Ocurrió un error." + exception;
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