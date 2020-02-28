using comerciamarketing_webapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comerciamarketing_webapp.Controllers
{
    public class clsGeneral
    {
        private dbComerciaEntities dbcmk = new dbComerciaEntities();
        public bool checkSession()
        {
            var flag = false;
            Usuarios activeuser = HttpContext.Current.Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                flag = true;
            }
            else
            {
                if (HttpContext.Current.Request.Cookies["correo"] != null)
                {
                    //COMO YA EXISTE NO NECESITAMOS RECREARLA Y SOLO VOLVEMOS A INICIAR SESION
                    flag = true;
                    var email = HttpContext.Current.Request.Cookies["correo"].Value;
                    var password = HttpContext.Current.Request.Cookies["pass"].Value;
                    HttpContext.Current.Session["activeUser"] = (from a in dbcmk.Usuarios where (a.correo == email && a.contrasena == password && a.activo == true) select a).FirstOrDefault();
                    Usuarios activeuserAgain = HttpContext.Current.Session["activeUser"] as Usuarios;
                    if (activeuserAgain != null)
                    {
                        flag = true;
                    }
                    else { flag = false; }


                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }

    }
}