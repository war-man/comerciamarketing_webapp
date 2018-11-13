﻿using comerciamarketing_webapp.Models;
using Newtonsoft.Json;
using Postal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
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

        public ActionResult Promotions()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SendDataPromotion(string name, string email, string phone, string estado)
        {
            try
            {
                //  Get all files from Request object  
                HttpFileCollectionBase files = Request.Files;

                for (int i = 0; i < files.Count; i++)
                {
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  

                    HttpPostedFileBase file = files[i];

                    if (file.ContentLength == 0)
                    {
                        TempData["advertencia"] = "No image selected.";
                        return RedirectToAction("Promotions", "Home", null);
                    }
                    else
                    {
                        if (estado == "NA")
                        {
                            TempData["advertencia"] = "No State selected.";
                            return RedirectToAction("Promotions", "Home", null);
                        }
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        Image TargetImg = Image.FromStream(file.InputStream, true, true);

                        if (TargetImg.PropertyIdList.Contains(0x0112))
                        {
                            try
                            {
                                int or = Convert.ToInt32(TargetImg.GetPropertyItem(0x0112).Value[0]);

                                switch (or)
                                {
                                    case 1: // landscape, do nothing
                                        break;

                                    case 8: // rotated 90 right
                                            // de-rotate:
                                        TargetImg.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                                        break;

                                    case 3: // bottoms up
                                        TargetImg.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                                        break;

                                    case 6: // rotated 90 left
                                        TargetImg.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                                        break;
                                }

                            }
                            catch
                            {

                            }
                        }
                        //buscamos el id del detalle

                        promotions_v1 detail = new promotions_v1();
                        detail.name = name;
                        detail.email = email;
                        detail.phone = phone;

                        DateTime time = DateTime.Now;


                        using (Image watermark = Image.FromFile(Server.MapPath("~/Content/images/Logo_watermark.png")))
                        using (Graphics g = Graphics.FromImage(TargetImg))
                        {

                            Image thumb = watermark.GetThumbnailImage((TargetImg.Width / 2), (TargetImg.Height / 3), null, IntPtr.Zero);

                            var destX = (TargetImg.Width / 2 - thumb.Width / 2);
                            var destY = (TargetImg.Height / 2 - thumb.Height / 2);

                            g.DrawImage(watermark, new Rectangle(destX,
                                        destY,
                                        TargetImg.Width / 2,
                                        TargetImg.Height / 4));


                            // display a clone for demo purposes
                            //pb2.Image = (Image)TargetImg.Clone();
                            Image imagenfinal = (Image)TargetImg.Clone();
                            var path = Path.Combine(Server.MapPath("~/Content/images/promotions"), "promotion_" + name.Substring(0, 3) + "_" + time.Minute + time.Second + ".jpg");


                            var tam = file.ContentLength;

                            //if (tam < 600000)
                            //{
                            imagenfinal.Save(path, ImageFormat.Jpeg);
                            //}
                            //else {
                            //    //Antes de guardar cambiamos el tamano de la imagen
                            //    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                            //    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                            //    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            //    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 10L);
                            //    myEncoderParameters.Param[0] = myEncoderParameter;


                            //    imagenfinal.Save(path, jpgEncoder, myEncoderParameters);
                            //}



                        }


                        //fname = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), fname);
                        //file.SaveAs(fname);

                        //Luego guardamos la url en la db
                        //Forms_details detail = db.Forms_details.Find(Convert.ToInt32(id));  //se movio hacia arriba
                        detail.imgpath = "~/Content/images/activities/promotion_" + name.Substring(0, 3) + "_" + time.Minute + time.Second + ".jpg";

                        db.promotions_v1.Add(detail);
                        db.SaveChanges();

                        // Returns message that successfully uploaded  
                        TempData["exito"] = "Data saved succefully. Thank you for participating.";
                        return RedirectToAction("Home", "Home", null);

                    }



                }
                TempData["advertencia"] = "No image selected. Please, try again";
                return RedirectToAction("Promotions", "Home", null);


            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Error: " + ex.Message;
                return RedirectToAction("Promotions", "Home", null);

            }
        }
        public ActionResult Main(string fstartd, string fendd, string store)
        {

            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();
                //var registro_conexiones = (from b in db.historial_conexiones where (b.ID_usuario == ID) select b).OrderByDescending(b=> b.fecha_conexion).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;
                ViewBag.tipomembresia = datosUsuario.Tipo_membresia.descripcion;
                ViewBag.ultimavisita = Session["ultimaconexion"].ToString();//datosUsuario.fultima_visita.ToString();
                ViewBag.bloquearcontenido = "no";

                //FILTROS VARIABLES
                DateTime filtrostartdate;
                DateTime filtroenddate;



                //filtros de fecha (DIARIO)
                var sunday = DateTime.Today;
                var saturday = sunday.AddHours(23);
                ////filtros de fecha (SEMANAL)
                //var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                //var saturday = sunday.AddDays(6).AddHours(23);
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

                //VISITAS
                //SELECCIONAMOS RUTAS

                var rutas = new List<VisitsM>();
                int[] visitasarray = new int[] { };
                if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)
                {
                    if (store == null || store == "") {
                        rutas = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate).ToList();
                        visitasarray = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate).Select(d => d.ID_visit).ToArray();
                    }
                    else {
                        rutas = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate && d.ID_store == store).ToList();
                        visitasarray = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate && d.ID_store == store).Select(d => d.ID_visit).ToArray();
                    }

                }
                else
                {
                    var visitrep = (from gg in db.VisitsM_representatives where (gg.ID_usuario == ID) select gg.ID_visit).ToArray();

                    if (store == null || store == "")
                    {

                        rutas = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && r.visit_date >= filtrostartdate && r.end_date <= filtroenddate) select r).ToList();
                        visitasarray = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && r.visit_date >= filtrostartdate && r.end_date <= filtroenddate) select r.ID_visit).ToArray();
                    }
                    else {
                        rutas = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && r.visit_date >= filtrostartdate && r.end_date <= filtroenddate && r.ID_store == store) select r).ToList();
                        visitasarray = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && r.visit_date >= filtrostartdate && r.end_date <= filtroenddate && r.ID_store == store) select r.ID_visit).ToArray();
                    }

                }

                //Agregamos los representantes y tambien el estado de cada visita por REP filtro
                if (datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 9)
                {


                    foreach (var itemVisita in rutas)
                    {
                        var repvisit = (from a in db.VisitsM_representatives where (a.ID_visit == itemVisita.ID_visit && a.ID_usuario == datosUsuario.ID_usuario) select a).FirstOrDefault();
                        var nombreRuta = "";
                        var rutitalist = (from e in db.RoutesM where (e.ID_route == itemVisita.ID_route) select e).FirstOrDefault();

                        nombreRuta = rutitalist.query2;
                        //utiliamos esta variable para el nombre del representante
                        itemVisita.city = nombreRuta;

                        itemVisita.ID_visitstate = Convert.ToInt32(repvisit.query1);
                    }
                }
                else
                {
                    foreach (var itemVisita in rutas)
                    {
                        var nombreRuta = "";
                        var rutitalist = (from e in db.RoutesM where (e.ID_route == itemVisita.ID_route) select e).FirstOrDefault();

                        nombreRuta = rutitalist.query2;
                        //utiliamos esta variable para el nombre del representante
                        itemVisita.city = nombreRuta;
                    }
                }


                //Agregamos los representantes
                foreach (var itemVisita in rutas)
                {
                    var nombreRep = "";
                    var reps = (from e in db.VisitsM_representatives where (e.ID_visit == itemVisita.ID_visit) select e).ToList();

                    foreach (var itemrep in reps)
                    {
                        if (itemrep.ID_usuario == 0)//Es usuario demo
                        {
                            if (reps.Count() == 1)
                            {
                                nombreRep = itemrep.query1;
                            }
                            else if (reps.Count() > 1)
                            {
                                nombreRep += itemrep.query1 + ", ";
                            }
                        }
                        else
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
                    }
                    //utiliamos esta variable para el nombre del representante
                    itemVisita.customer = nombreRep;
                }


                //ESTADISTICA DE RUTAS POR ESTADO
                int totalRutas = rutas.Count();

                int onhold = (from e in rutas where (e.ID_visitstate == 3) select e).Count();
                int inprogress = (from e in rutas where (e.ID_visitstate == 2) select e).Count();
                int canceled = (from e in rutas where (e.ID_visitstate == 1) select e).Count();
                int finished = (from e in rutas where (e.ID_visitstate == 4) select e).Count();


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

                var activities = (from a in db.ActivitiesM where (visitasarray.Contains(a.ID_visit)) select a).ToList();
                foreach (var itemac in activities)
                {
                    var uassigned = (from u in db.Usuarios where (u.ID_usuario == itemac.ID_usuarioEnd) select u).FirstOrDefault();

                    if (uassigned == null && itemac.ID_usuarioEnd == 0)
                    {
                        var usuario = (from a in CMKdb.OCRD where (a.CardCode == itemac.ID_usuarioEndString) select a).FirstOrDefault();
                        itemac.ID_customer = usuario.CardName.ToString() + " - " + usuario.E_Mail.ToString();
                    }
                    else
                    {
                        itemac.ID_customer = uassigned.nombre + " " + uassigned.apellido;
                    }

                }
                ViewBag.actlist = activities;

                //Filtros viewbag

                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();
                //*****************
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }


        }
        public ActionResult Internal()
        {
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.IPLOCAL = "";
            //Session["ip_user"] = GetExternalIp();

            if (Request.Cookies["GLOBALUSERID"] != null) //remember me active
            {
                //cookies
                HttpCookie aCookie = Request.Cookies["GLOBALUSERID"];
                if (Session["IDusuario"] != null)
                {
                   
                }
                else {//Como tiene activado remember me, iniciamos sesion automaticamente
                   int IDUSER = Convert.ToInt32(Server.HtmlEncode(aCookie.Value));
                   var obj = (from c in db.Usuarios where (c.ID_usuario == IDUSER) select c).FirstOrDefault();

                    Session["IDusuario"] = obj.ID_usuario.ToString();
                    Session["tipousuario"] = obj.ID_tipomembresia.ToString();
                    Session["tiporol"] = obj.ID_rol.ToString();
                    Session["ultimaconexion"] = "";
                    GlobalVariables.ID_EMPRESA_USUARIO = Convert.ToInt32(obj.ID_empresa);

                   
                }
                ////
                return RedirectToAction("Main");
            }
            return View();
        }

        public ActionResult RoutesM(string fstartd, string fendd)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                //VISITAS

                var visitas = new List<VisitsM>();
                var rutas = new List<RoutesM>();

                DateTime filtrostartdate;
                DateTime filtroenddate;

                //filtros de fecha
                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);
                //FILTROS**************

                if (fstartd == null || fstartd == "")
                {
                    filtrostartdate = sunday;
                }
                else {
                    filtrostartdate = Convert.ToDateTime(fstartd);
                }

                if (fendd == null || fendd == "")
                {
                    filtroenddate = saturday;
                }
                else {
                    filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59);
                }
                //FIN FILTROS*******************


                if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)
                {
                    visitas = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate).ToList();
                    rutas = db.RoutesM.Where(d => d.date >= filtrostartdate && d.end_date <= filtroenddate).OrderByDescending(d => d.date).ToList();
                }
                else
                {
                    var visitrep = (from gg in db.VisitsM_representatives where (gg.ID_usuario == ID) select gg.ID_visit).ToArray();


                    visitas = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && r.visit_date >= filtrostartdate && r.end_date <= filtroenddate) select r).ToList();

                    var arrayVisiID = (from arr in visitas select arr.ID_route).ToArray();
                    rutas = (from rut in db.RoutesM where (arrayVisiID.Contains(rut.ID_route)) select rut).ToList();
                }

                //Agregamos los representantes y tambien el estado de cada visita por REP filtro
                if (datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 9)
                {


                    foreach (var itemVisita in visitas)
                    {
                        var repvisit = (from a in db.VisitsM_representatives where (a.ID_visit == itemVisita.ID_visit && a.ID_usuario == datosUsuario.ID_usuario) select a).FirstOrDefault();

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
                        else {
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


                // Convertimos la lista a array
                //var usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9);
                // Convertimos la lista a array
                //ArrayList myArrList = new ArrayList();
                //myArrList.AddRange((from p in usuarios
                //                    select new
                //                    {
                //                        id = p.ID_usuario,
                //                        text = p.nombre + " " + p.apellido
                //                    }).ToList());


                ////LISTADO DE REPRESENTANTES

                //ViewBag.usuarios = JsonConvert.SerializeObject(myArrList);
                //LISTADO DE RUTAS
                var rutass = CMKdb.C_ROUTES.OrderBy(c => c.Code);
                ViewBag.rutass = rutass.ToList();
                //LISTADO DE TIENDAS

                List<MyObj_tablapadre> listapadres = (from p in CMKdb.C_ROUTES select
                                                      new MyObj_tablapadre {
                                                          id = p.Code,
                                                          text = p.Name
                                                      }
                                                      ).ToList();

                List<tablahijospadre> listahijas = (from p in CMKdb.C_ROUTE
                                                    join store in CMKdb.OCRD on p.U_CardCode equals store.CardCode
                                                    select new tablahijospadre {
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
                //LISTADO DE ACTIVIDAD (TIMELINE)

                var log = new List<ActivitiesM_log>();



                if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)
                {
                    log = (from l in db.ActivitiesM_log where (l.fecha_conexion >= filtrostartdate && l.fecha_conexion <= filtroenddate) select l).ToList();
                }
                else
                {
                    log = (from l in db.ActivitiesM_log where (l.ID_usuario == ID && l.fecha_conexion >= filtrostartdate && l.fecha_conexion <= filtroenddate) select l).OrderBy(l => l.fecha_conexion).ToList();
                }

                ViewBag.log = log;

                //Filtros Viewbag
                //Filtros viewbag

                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString(); ;
                //*****************
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
                var rutas = new List<VisitsM>();

                if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)
                {
                    rutas = db.VisitsM.Where(c => c.ID_route == id).ToList();
                }
                else
                {
                    var visitrep = (from gg in db.VisitsM_representatives where (gg.ID_usuario == ID) select gg.ID_visit).ToArray();


                    rutas = (from r in db.VisitsM where (visitrep.Contains(r.ID_visit) && r.ID_route == id) select r).ToList();
                }
                //Agregamos los representantes y tambien el estado de cada visita por REP filtro
                if (datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 9)
                {


                    foreach (var itemVisita in rutas)
                    {
                        var repvisit = (from a in db.VisitsM_representatives where (a.ID_visit == itemVisita.ID_visit && a.ID_usuario == datosUsuario.ID_usuario) select a).FirstOrDefault();

                        itemVisita.ID_visitstate = Convert.ToInt32(repvisit.query1);
                    }
                }

                //ESTADISTICA DE RUTAS POR ESTADO
                int totalRutas = rutas.Count();

                int onhold = (from e in rutas where (e.ID_visitstate == 3) select e).Count();
                int inprogress = (from e in rutas where (e.ID_visitstate == 2) select e).Count();
                int canceled = (from e in rutas where (e.ID_visitstate == 1) select e).Count();
                int finished = (from e in rutas where (e.ID_visitstate == 4) select e).Count();


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
                        if (itemrep.ID_usuario == 0)//Es usuario demo
                        {
                            if (reps.Count() == 1)
                            {
                                nombreRep = itemrep.query1;
                            }
                            else if (reps.Count() > 1)
                            {
                                nombreRep += itemrep.query1 + ", ";
                            }
                        }
                        else {
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

                ViewBag.routes_map = JsonConvert.SerializeObject(myArrList);

                //Para ruta animada
                ArrayList myArrList2 = new ArrayList();
                myArrList2.AddRange((from p in demos_map where (p.ID_visitstate == 4 || p.ID_visitstate == 2)
                                     select new
                                     {
                                         GeoLong = p.geoLong,
                                         GeoLat = p.geoLat,
                                         demo_state = p.ID_visitstate,
                                         check_ind = p.check_in,
                                     }).OrderByDescending(c => c.demo_state).ThenBy(c => c.check_ind).ToList());

                ViewBag.routes_animated = JsonConvert.SerializeObject(myArrList2);
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

                ViewBag.id_route = id;



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
        public ActionResult Test_barcode()
        {

            return View();

            
    
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
        public ActionResult Iniciar_sesion(string usuariocorreo, string password, bool rememberme)
        {
            //Validamos del lado del cliente que ambos parametros no vengan vacios
            try
            {
                var obj = (from c in db.Usuarios where (c.correo == usuariocorreo && c.contrasena == password) select c).FirstOrDefault();
                if (obj != null) { 
                //PARA DASHBOARD
                //(obj.Tipo_membresia.descripcion == "Demo")

                //PARA DEMOS
                //(obj.Tipo_membresia.descripcion == "Professional" || obj.Tipo_membresia.descripcion == "Enterprise" || obj.Tipo_membresia.descripcion == "Premium")

                //PARA MERCHANDISING
                //((obj.Tipo_membresia.descripcion == "Demo") || (obj.Tipo_membresia.descripcion == "Professional" || obj.Tipo_membresia.descripcion == "Enterprise" || obj.Tipo_membresia.descripcion == "Premium"))

                //if ((obj.Tipo_membresia.descripcion == "Demo") || (obj.Tipo_membresia.descripcion == "Professional" || obj.Tipo_membresia.descripcion == "Enterprise" || obj.Tipo_membresia.descripcion == "Premium"))
                //{
                //    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                Session["IDusuario"] = obj.ID_usuario.ToString();
                    Session["tipousuario"] = obj.ID_tipomembresia.ToString();
                    Session["tiporol"] = obj.ID_rol.ToString();
                    Session["ultimaconexion"] = "";
                    GlobalVariables.ID_EMPRESA_USUARIO = Convert.ToInt32(obj.ID_empresa);


                    ///PARA RECORDAR DATOS
                    if (rememberme == true) {
                    if (Request.Cookies["GLOBALUSERID"] != null)
                    {
                        //COMO YA EXISTE NO NECESITAMOS RECREARLA
                    }
                    else { 
                        HttpCookie aCookie = new HttpCookie("GLOBALUSERID");
                        aCookie.Value =  obj.ID_usuario.ToString();
                        aCookie.Expires = DateTime.Now.AddMonths(3);
                        Response.Cookies.Add(aCookie);
                    }
                }


                    ///


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
                            if (obj.ID_tipomembresia == 2 || obj.ID_tipomembresia == 3 || obj.ID_tipomembresia == 4) {
                                Session["ultimaconexion"] = "";
                                return Json(new { success = true, redireccion = "customer", cm = obj.Empresas.ID_SAP }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                Session["ultimaconexion"] = "";
                                return Json(new { success = true, redireccion = "" }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        else
                        {
                            if (obj.ID_tipomembresia == 2 || obj.ID_tipomembresia == 3 || obj.ID_tipomembresia == 4)
                            {
                                Session["ultimaconexion"] = Convert.ToDateTime(ultimaconexion.fecha_conexion).ToLocalTime();
                                return Json(new { success = true, redireccion = "customer", cm = obj.Empresas.ID_SAP }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                Session["ultimaconexion"] = Convert.ToDateTime(ultimaconexion.fecha_conexion).ToLocalTime();
                                return Json(new { success = true, redireccion = "" }, JsonRequestBehavior.AllowGet);
                            }

                        }
                    }
                    else {

                        return Json(new { success = false, redireccion = "" }, JsonRequestBehavior.AllowGet);
                    }



                    //}
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
            if (Request.Cookies["GLOBALUSERID"] != null)
            {
                var c = new HttpCookie("GLOBALUSERID");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
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

        public ActionResult Change_password(string password, string newpassword, string retrypassword)
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

                var demo_details_items = (from a in db.Forms_details where (a.ID_demo == ID_demos && a.ID_formresourcetype == 5) select a).ToList();

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
            return this.Json((from obj in db.Demos where (obj.ID_Vendor == vendorID) select new { ID_demo = obj.ID_demo, visit_date = obj.visit_date, store = obj.store }), JsonRequestBehavior.AllowGet);
        }


        //MERCHANDISING

        public ActionResult Users()
        {
            if (Session["IDusuario"] != null)
            {
                //GENERAL
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                //GENERAL END
                //2 es la empresa DEFAULT

                //List<string> uids_rol = new List<string>() { "1", "3", "5", "6", "8", "9", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };

                var usuarios = db.Usuarios.Where(c => c.ID_rol !=9).Include(u => u.Tipo_membresia).Include(u => u.Roles);


    
                return View(usuarios.ToList());




            }
            else
            {
                return RedirectToAction("Index");
            }
        }
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

                var lstCustomer = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select new { ID = b.CardCode, Name = b.CardName.Replace("\"", "\\\"") }).OrderBy(b => b.Name).ToList();



                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string result = javaScriptSerializer.Serialize(lstCustomer);

                ViewBag.customers = result;
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
                var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

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
        public class customerList{
            public string id { get; set; }
            public string name { get; set; }
            public string address { get; set; }
        }
        public ActionResult Representative_stats(int? id, string fstartd, string fendd)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                DateTime filtrostartdate;
                DateTime filtroenddate;

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



                var rep = (from a in db.Usuarios where (a.ID_usuario == id) select a).FirstOrDefault();
                if (rep != null)
                {
                    ViewBag.nombre = rep.nombre + " " + rep.apellido;
                    ViewBag.correo = rep.correo;

                    ViewBag.ID_rep = rep.ID_usuario;

                    //LISTADO DE CUSTOMERS

                    List<customerList> clist = new List<customerList>();
                    List<string> customerIds = rep.estados_influencia.Split(',').ToList();

                    foreach (var it in customerIds)
                    {
                        var customer = (from s in CMKdb.OCRD where (s.CardCode == it) select new customerList { id=s.CardCode, name =s.CardName, address = (s.MailCity + ", " + s.State2 + ", " + s.Country)  }).FirstOrDefault();
                        if (customer != null) {
                            
                            clist.Add(customer);
                        }
                    }                
                    ViewBag.customers = clist.OrderBy(c=> c.name);
                    ///***********************
                   
                    //LOG (TIMELINE)
                    var log = new List<ActivitiesM_log>();
                    log = (from l in db.ActivitiesM_log where (l.ID_usuario == rep.ID_usuario) select l).OrderByDescending(l => l.fecha_conexion).Take(5).ToList();
                    
                    ViewBag.log = log;
                    //********

                    //Activities 
                    var activity = new List<ActivitiesM>();
                    activity = (from l in db.ActivitiesM where (l.ID_usuarioEnd == rep.ID_usuario && l.date >= filtrostartdate && l.date <= filtroenddate) select l).OrderByDescending(l => l.date).ToList();
                    //Contamos y definimos por tipo
                    int totalActivities = activity.Count();
                    //1.Forms - 2.Audit - 3.SalesOrder - 4.Demos
                    int totalForms = (from a in activity where (a.ID_activitytype == 1) select a).Count();
                    int totalAudits = (from a in activity where (a.ID_activitytype == 2) select a).Count();
                    int totalSales = (from a in activity where (a.ID_activitytype == 3) select a).Count();
                    int totalDemos = (from a in activity where (a.ID_activitytype == 4) select a).Count();

                    
                    ViewBag.totalAct = totalActivities;
                    ViewBag.totalForm = totalForms;
                    ViewBag.totalAudit = totalAudits;
                    ViewBag.totalSale = totalSales;
                    ViewBag.totalDemo = totalDemos;
                    //********
                    //VISITS
                    var visitas = new List<VisitsM>();

                    var asv = (from e in db.VisitsM_representatives where (e.ID_usuario == rep.ID_usuario) select e.ID_visit).ToArray();

                    visitas = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate && asv.Contains(d.ID_visit)).ToList();

                    int totalVisits = visitas.GroupBy(d => d.ID_store).Count();
                    ViewBag.totalVisitas = totalVisits;

                    //LISTADO DE TIENDAS
                    var stores = (from b in CMKdb.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "" && b.QryGroup30 == "Y" && b.validFor == "Y") select b).OrderBy(b => b.CardName).Count();
                    ViewBag.stores = stores;

                    ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                    ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                    return View();
                }
                else {
                    TempData["advertencia"] = "No data to show.";
                    return View("Representatives");
                }

                
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult StoreM_stats(string id, string fstartd, string fendd)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                DateTime filtrostartdate;
                DateTime filtroenddate;

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



                var store = (from a in CMKdb.OCRD where (a.CardCode == id) select a).FirstOrDefault();
                if (store != null)
                {
                    ViewBag.name = store.CardName;
                    ViewBag.address = store.MailAddres + ", " + store.MailCity +", "+ store.State2 + ", " + store.MailCounty;

                    ViewBag.ID_store = store.CardCode;

                    //VISITS
                    var visitas = new List<VisitsM>();

                    visitas = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate && d.ID_store==id).ToList();
                    var visitsarray = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate && d.ID_store == id).Select(d=>d.ID_visit).ToArray();
                    int totalVisits = visitas.Count();
                 


                    int totalScheduled = (from c in visitas where (c.ID_visitstate == 3) select c).Count();
                    int totalInpro = (from c in visitas where (c.ID_visitstate == 2) select c).Count();
                    int totalFini = (from c in visitas where (c.ID_visitstate == 4) select c).Count();
                    int totalCancl = (from c in visitas where (c.ID_visitstate == 1) select c).Count();

                    ViewBag.totalVisitas = totalVisits;
                    ViewBag.totalSD = totalScheduled;
                    ViewBag.totalFS = totalFini;
                    ViewBag.totalInpro = totalInpro;
                    ViewBag.totalCanc = totalCancl;


                    //**************
                    //Activities 
                    var activity = new List<ActivitiesM>();
                    activity = (from l in db.ActivitiesM where (visitsarray.Contains(l.ID_visit)) select l).OrderByDescending(l => l.date).ToList();
                    //Contamos y definimos por tipo
                    int totalActivities = activity.Count();
                    //1.Forms - 2.Audit - 3.SalesOrder - 4.Demos
                    int totalForms = (from a in activity where (a.ID_activitytype == 1) select a).Count();
                    int totalAudits = (from a in activity where (a.ID_activitytype == 2) select a).Count();
                    int totalSales = (from a in activity where (a.ID_activitytype == 3) select a).Count();
                    int totalDemos = (from a in activity where (a.ID_activitytype == 4) select a).Count();


                    ViewBag.totalAct = totalActivities;
                    ViewBag.totalForm = totalForms;
                    ViewBag.totalAudit = totalAudits;
                    ViewBag.totalSale = totalSales;
                    ViewBag.totalDemo = totalDemos;
                    //********



                    ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                    ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                    var geoLong="";
                    var geoLat = "";

                    try
                    {
                        string address = store.CardName.ToString() + ", " + store.MailAddres.ToString() + ", " + store.MailCity.ToString() + ", " + store.MailZipCod.ToString();
                        string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyC3zDvE8enJJUHLSmhFAdWhPRy_tNSdQ6g&address={0}&sensor=false", Uri.EscapeDataString(address));

                        WebRequest request = WebRequest.Create(requestUri);
                        WebResponse response = request.GetResponse();
                        XDocument xdoc = XDocument.Load(response.GetResponseStream());

                        XElement result = xdoc.Element("GeocodeResponse").Element("result");
                        XElement locationElement = result.Element("geometry").Element("location");
                        XElement lat = locationElement.Element("lat");
                        XElement lng = locationElement.Element("lng");
                        //NO SE PORQUE LO TIRA AL REVEZ
                        geoLat = lng.Value;
                       geoLong = lat.Value;
                        //FIN

                    }
                    catch
                    {
                        geoLong = "";
                        geoLat = "";
                    }

                    ViewBag.glong = geoLong;
                    ViewBag.glat = geoLat;
                    return View();
                }
                else
                {
                    TempData["advertencia"] = "No data to show.";
                    return View("StoresM");
                }


            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult CustomerM_stats(string id, string fstartd, string fendd, string store)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                var customer = (from a in CMKdb.OCRD where (a.CardCode == id) select a).FirstOrDefault();


                //Precargamos los filtros, comenzando por el de fecha
                //FILTROS VARIABLES
                DateTime filtrostartdate;
                DateTime filtroenddate;
                //filtros de fecha (DIARIO)
                var sunday = DateTime.Today;
                var saturday = sunday.AddHours(23);
                ////filtros de fecha (SEMANAL)
                //var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                //var saturday = sunday.AddDays(6).AddHours(23);
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

                //VISITAS
                //SELECCIONAMOS RUTAS

                var rutas = new List<VisitsM>();
                int[] visitasarray = new int[] { };

                    if (store == null || store == "")
                    {
                        rutas = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate).ToList();
                        visitasarray = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate).Select(d => d.ID_visit).ToArray();
                    }
                    else
                    {
                        rutas = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate && d.ID_store == store).ToList();
                        visitasarray = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate && d.ID_store == store).Select(d => d.ID_visit).ToArray();
                    }

                //Buscamos en las actividades para filtrar por rutas

                var activities = (from a in db.ActivitiesM where (visitasarray.Contains(a.ID_visit) && a.ID_customer == id) select a.ID_visit).Distinct().ToArray();
                var activitiesCount = (from a in db.ActivitiesM where (visitasarray.Contains(a.ID_visit) && a.ID_customer == id) select a).Count();




                //**************
                //Activities 
                var activity = new List<ActivitiesM>();
                activity = (from l in db.ActivitiesM where (activities.Contains(l.ID_visit)) select l).OrderByDescending(l => l.date).ToList();
                //Contamos y definimos por tipo
                int totalActivities = activity.Count();
                //1.Forms - 2.Audit - 3.SalesOrder - 4.Demos
                int totalForms = (from a in activity where (a.ID_activitytype == 1) select a).Count();
                int totalAudits = (from a in activity where (a.ID_activitytype == 2) select a).Count();
                int totalSales = (from a in activity where (a.ID_activitytype == 3) select a).Count();
                int totalDemos = (from a in activity where (a.ID_activitytype == 4) select a).Count();


                ViewBag.totalAct = totalActivities;
                ViewBag.totalForm = totalForms;
                ViewBag.totalAudit = totalAudits;
                ViewBag.totalSale = totalSales;
                ViewBag.totalDemo = totalDemos;
                //********


                rutas = (from r in rutas where (activities.Contains(r.ID_visit)) select r).ToList();
                //Agregamos los representantes y tambien el estado de cada visita por REP filtro

                foreach (var itemVisita in rutas)
                    {
                        var nombreRuta = "";
                    var nombreStra = "";
                        var rutitalist = (from e in db.RoutesM where (e.ID_route == itemVisita.ID_route) select e).FirstOrDefault();
                    var nombreStrategicPart = (from f in CMKdb.OCRD where (f.CardCode == itemVisita.ID_store) select f).FirstOrDefault();

                    if (nombreStrategicPart != null) {
                        nombreStra = nombreStrategicPart.SlpCode.ToString();
                    }
                    

                    if (nombreStra == "18")
                    {
                        itemVisita.comments = "D. Limena Tennesse";
                    }
                    else if (nombreStra == "19") {
                        itemVisita.comments = "D. Limena Georgia";
                    }
                    else{
                        itemVisita.comments = "";
                    }
                        nombreRuta = rutitalist.query2;
                        //utiliamos esta variable para nombre de ruta
                        itemVisita.city = nombreRuta;
                    }

                //VISITAS//RUTAAAAAAAAAAAAAAAAAAAAAAAs

                var visitas = new List<VisitsM>();
                var rutas2 = new List<RoutesM>();



                if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)
                {
                    visitas = db.VisitsM.Where(d => d.visit_date >= filtrostartdate && d.end_date <= filtroenddate && activities.Contains(d.ID_visit)).ToList();
                    rutas2 = db.RoutesM.Where(d => d.date >= filtrostartdate && d.end_date <= filtroenddate).OrderByDescending(d => d.date).ToList();
                }
                else
                {

                    visitas = (from r in db.VisitsM where (activities.Contains(r.ID_visit) && r.visit_date >= filtrostartdate && r.end_date <= filtroenddate) select r).ToList();

                    var arrayVisiID = (from arr in visitas select arr.ID_route).ToArray();
                    rutas2 = (from rut in db.RoutesM where (arrayVisiID.Contains(rut.ID_route)) select rut).ToList();
                }

                //Agregamos los representantes y tambien el estado de cada visita por REP filtro
                //if (datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 9)
                //{


                //    foreach (var itemVisita in visitas)
                //    {
                //        var repvisit = (from a in db.VisitsM_representatives where (a.ID_visit == itemVisita.ID_visit && a.ID_usuario == datosUsuario.ID_usuario) select a).FirstOrDefault();

                //        itemVisita.ID_visitstate = Convert.ToInt32(repvisit.query1);
                //    }
                //}


                //ESTADISTICA DE RUTAS POR ESTADO DE VISITAS
                decimal totalRutas = visitas.Count();
                foreach (var rutait in rutas2)
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
                    itemVisita.customer = nombreRep;
                }


                //ESTADISTICA DE RUTAS POR ESTADO
                int totalRutas2 = rutas.Count();


                int totalScheduled = (from c in rutas where (c.ID_visitstate == 3) select c).Count();
                int totalInpro = (from c in rutas where (c.ID_visitstate == 2) select c).Count();
                int totalFini = (from c in rutas where (c.ID_visitstate == 4) select c).Count();
                int totalCancl = (from c in rutas where (c.ID_visitstate == 1) select c).Count();

                ViewBag.totalVisitas = totalRutas;
                ViewBag.totalSD = totalScheduled;
                ViewBag.totalFS = totalFini;
                ViewBag.totalInpro = totalInpro;
                ViewBag.totalCanc = totalCancl;

       

                if (totalRutas2 != 0)
                {
                    ViewBag.onholdP = ((Convert.ToDecimal(totalScheduled) / totalRutas2) * 100);
                    ViewBag.inprogressP = ((Convert.ToDecimal(totalInpro) / totalRutas2) * 100);
                    ViewBag.canceledP = ((Convert.ToDecimal(totalCancl) / totalRutas2) * 100);
                    ViewBag.finishedP = ((Convert.ToDecimal(totalFini) / totalRutas2) * 100);
                }
                else
                {

                    ViewBag.onholdP = 0;
                    ViewBag.inprogressP = 0;
                    ViewBag.canceledP = 0;
                    ViewBag.finishedP = 0;
                }



                //LISTADO DE REPRESENTANTES
                var usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia == 8 && c.ID_rol == 9).Include(u => u.Tipo_membresia).Include(u => u.Roles);

                List <Usuarios> listau = new List<Usuarios>();

                foreach (var user in usuarios) {
                    List<string> storeIds = user.estados_influencia.Split(',').ToList();

                    foreach (var it in storeIds) {
                        if (it == id) {
                            listau.Add(user);
                        }
                    }

                }
                ViewBag.usuarios =listau;


                //MAPA DE RUTAS
                var demos_map = rutas;




                // Convertimos la lista a array
                ArrayList myArrList = new ArrayList();
                myArrList.AddRange((from p in demos_map
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

                ViewBag.routes_map = JsonConvert.SerializeObject(myArrList);

                //LISTADO DE TIENDAS
                var stores = (from b in CMKdb.OCRD where (b.Series == 68 && b.CardName != null && b.CardName != "" && b.QryGroup30 == "Y" && b.validFor == "Y") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.stores = stores.ToList();


                ViewBag.visitas = rutas;


                //COUNTERS
                ViewBag.rutascount = rutas.Count();
                ViewBag.actlist = activitiesCount;

                //Filtros viewbag

                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                ViewData["customerName"] = customer.CardName;
                ViewBag.customerID = customer.CardCode;

                //SOLO PARA CLIENTES
                var recursos = (from b in db.Recursos_usuario where (b.ID_usuario == ID) select b).ToList(); ;

                if (recursos != null)
                {
                    
                    ViewBag.bloquearcontenido = "si";
                    
                }
                ViewBag.recursos = recursos;

                return View(rutas2.ToList());
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Analytics(string id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                var customer = (from a in CMKdb.OCRD where (a.CardCode == id) select a).FirstOrDefault();


                ViewBag.customerID = customer.CardCode;
                //SOLO PARA CLIENTES
                var recursos = (from b in db.Recursos_usuario where (b.ID_usuario == ID) select b).ToList(); ;

                if (recursos != null)
                {

                    ViewBag.bloquearcontenido = "si";

                }
                ViewBag.recursos = recursos;

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
                    var tp = (from b in db.ActivitiesM_types where (b.ID_activity == item.ID_activity) select b).FirstOrDefault();
                    if (tp == null) { item.query1 = ""; } else {
                        item.query1 = tp.description;
                    }
                }


                ViewBag.ID_activity = new SelectList(db.ActivitiesM_types, "ID_activity", "description");
                //Seleccionamos los tipos de recursos a utilizar en el caso de Merchandising

                List<string> uids = new List<string>() { "1", "3", "5", "6", "8", "9", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22","23", "24" };

                ViewBag.ID_formresourcetype = new SelectList(db.form_resource_type.Where(c => uids.Contains(c.ID_formresourcetype.ToString())).OrderBy(c => c.fdescription), "ID_formresourcetype", "fdescription");

                //PARA RECURSOS DE RETAIL AUDIT O COLUMN
                List<string> uidsColumn = new List<string>() { "16","21","3" };

                ViewBag.ID_formresourcetypeRetail = new SelectList(db.form_resource_type.Where(c => uidsColumn.Contains(c.ID_formresourcetype.ToString())).OrderBy(c => c.fdescription), "ID_formresourcetype", "fdescription");

                ViewBag.vendors = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                ViewBag.displaylist = (from d in db.Items_displays where (d.ID_empresa == GlobalVariables.ID_EMPRESA_USUARIO) select d).ToList();

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
        public ActionResult CreateRoute(string descriptionN, DateTime date, DateTime enddate, string listatiendas, string listarepresentantes, string idform, string listatiposactividades)
        {
            try
            {

                //Comenzamos con el maestro de rutas
                RoutesM rutamaestra = new RoutesM();

                rutamaestra.date = date;
                rutamaestra.end_date = enddate;
                rutamaestra.query1 = "";
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
                    var storeSAP = (from s in CMKdb.OCRD where (s.CardCode == store) select s).FirstOrDefault();
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
                        visita.end_date = enddate;
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


                        //List<int> repIds = listarepresentantes.Split(',').Select(int.Parse).ToList();

                        //foreach (var rep in repIds)
                        //{

                        //    if (rep != 0)
                        //    {
                        //        VisitsM_representatives repvisita = new VisitsM_representatives();

                        //        repvisita.ID_visit = visita.ID_visit;
                        //        repvisita.ID_usuario = rep;
                        //        repvisita.query1 = "";
                        //        repvisita.ID_empresa= visita.ID_empresa;
                        //        db.VisitsM_representatives.Add(repvisita);
                        //        db.SaveChanges();
                        //    }

                        //}


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

        public ActionResult Getformdata(string id_activity)
        {

            ActivitiesM activity = db.ActivitiesM.Find(Convert.ToInt32(id_activity));


            //NUEVO
            //ID VISIT SE UTILIZA COMO RELACION
            List<MyObj_tablapadrev2> listapadresActivities = (from item in db.FormsM_details
                                                              where (item.parent == 0 && item.ID_visit == activity.ID_activity && item.original == false)
                                                              select
                                                                 new MyObj_tablapadrev2
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
                                  ).ToList();





            List<tablahijospadrev2> listahijasActivities = (from item in db.FormsM_details
                                                            where (item.ID_visit == activity.ID_activity && item.original == false)
                                                            select new tablahijospadrev2
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

                                                            }).ToList();


            List<MyObj_tablapadrev2> categoriasListActivities = ObtenerCategoriarJerarquiaByIDv2(listapadresActivities, listahijasActivities);

 
            //Deserealizamos  los datos
            JavaScriptSerializer js = new JavaScriptSerializer();


            return Json(categoriasListActivities, JsonRequestBehavior.AllowGet);
        }


        //Estas se utilizan para las vistas previas en modales


        public class tablahijospadrev2
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

    public class MyObj_tablapadrev2
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
        public List<MyObj_tablapadrev2> children { get; set; }
    }

    public static List<MyObj_tablapadrev2> ObtenerCategoriarJerarquiaByIDv2(List<MyObj_tablapadrev2> Categoriaspadre, List<tablahijospadrev2> Categoriashijas)
    {


        List<MyObj_tablapadrev2> query = (from item in Categoriaspadre

                                        select new MyObj_tablapadrev2
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
                                            children = ObtenerHijosByIDv2(item.idkey, Categoriashijas)

                                        }).ToList();

        return query;





    }

    private static List<MyObj_tablapadrev2> ObtenerHijosByIDv2(int ID_parent, List<tablahijospadrev2> categoriashijas)
    {



        List<MyObj_tablapadrev2> query = (from item in categoriashijas

                                          where item.parent == ID_parent
                                          select new MyObj_tablapadrev2
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
                                              children = ObtenerHijosByIDv2(item.idkey, categoriashijas)
                                          }).ToList();

        return query;

    }
        //DISPLAY ITEMS
        public ActionResult Displayitems()
        {
            if (Session["IDusuario"] != null)
            {
                //GENERAL
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                //GENERAL END
                //2 es la empresa DEFAULT
                var items = db.Items_displays.Where(c => c.ID_empresa == 2);


                return View(items.ToList());

            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDisplayItem(string sku, string description)
        {
            Items_displays item = new Items_displays();

            if (sku == null || sku == "") {
                sku = "";
            }

            item.SKU = sku;
            item.description = description;
            item.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;
            item.active = true;

            if (ModelState.IsValid)
            {
                db.Items_displays.Add(item);
                db.SaveChanges();
                TempData["exito"] = "Item created successfully.";
                return RedirectToAction("Displayitems", "Home", null);
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Displayitems", "Home", null);
        }

        [ValidateAntiForgeryToken]
        public ActionResult DeleteDisplayItem(string iditemD)
        {
            try
            {
                int IDitem = Convert.ToInt32(iditemD);

                Items_displays brand = db.Items_displays.Find(IDitem);
                db.Items_displays.Remove(brand);
                db.SaveChanges();


                TempData["exito"] = "Item deleted successfully.";
                return RedirectToAction("Displayitems", "Home", null);
            }
            catch
            {
                TempData["advertencia"] = "Something wrong happened, try again.";
                return RedirectToAction("Displayitems", "Home", null);
            }



        }

        //********************************FIN DISPLAY ITEMS
        //BRAND COMPETITORS
        public ActionResult Brandcompetitors()
        {
            if (Session["IDusuario"] != null)
            {
                //GENERAL
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                //GENERAL END
                //2 es la empresa DEFAULT
                var items = db.Brand_competitors.Where(c => c.ID_empresa == 2);

                //Customers
                var customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.customers = customers.ToList();
                //*************
                return View(items.ToList());

            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public class brands
        {
            public string FirmCode { get; set; }
            public string FirmName { get; set; }
            public string Customer { get; set; }
            public Boolean isselected { get; set; }
        }
        public ActionResult Getbrands(string customerID)
        {
            if (customerID != null)
            {

                var lstbrands = CMKdb.view_CMKEditorB
                        .Where(i => i.U_CustomerCM == customerID)
                        .Select(i => new brands { FirmCode = i.FirmCode.ToString(), FirmName = i.FirmName, isselected = false, Customer = "" })
                        .Distinct()
                        .OrderByDescending(i => i.FirmName)
                        .ToList();

                //}
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string result = javaScriptSerializer.Serialize(lstbrands);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("error", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBrandCompetitor(string name, string ID_customer, string idbrand)
        {
            Brand_competitors item = new Brand_competitors();

            item.Name = name;
            item.ID_customer = ID_customer;

            //GET customer name
            var custom = (from b in CMKdb.OCRD where (b.CardCode == ID_customer) select b).FirstOrDefault();
            if (custom == null) { item.Costumer_name = ""; } else { item.Costumer_name = custom.CardName; }
            
            item.ID_brand = idbrand;
            //GET brand name
            var brandn = (from a in CMKdb.view_CMKEditorB where (a.FirmCode.ToString() == idbrand) select a).FirstOrDefault();
            if (brandn == null) { item.Brand_name = ""; } else { item.Brand_name = brandn.FirmName; }

            item.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;
    

            if (ModelState.IsValid)
            {
                db.Brand_competitors.Add(item);
                db.SaveChanges();
                TempData["exito"] = "Brand competitor created successfully.";
                return RedirectToAction("Brandcompetitors", "Home", null);
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Brandcompetitors", "Home", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVisitRoute(string id_store, string ID_route, DateTime date)
        {
            int IDR = Convert.ToInt32(ID_route);
            var route = db.RoutesM.Find(IDR);


            try
            {
            if (route != null)
            {
                    var storeSAP = (from a in CMKdb.OCRD where (a.CardCode == id_store) select a).FirstOrDefault();

                    VisitsM visita = new VisitsM();
                    visita.ID_customer = "";
                    visita.customer = "";
                    visita.ID_store = id_store;
                    visita.store = storeSAP.CardName;
                    visita.address = storeSAP.MailAddres;
                    visita.city = storeSAP.MailCity;
                    if (storeSAP.MailZipCod == null)
                    {
                        visita.zipcode = "";
                    }
                    else { visita.zipcode = storeSAP.MailZipCod; }

                    if (storeSAP.State2 == null)
                    {
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
                    visita.ID_route = IDR;
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

                    TempData["exito"] = "Visit created successfully.";
                return RedirectToAction("RoutesM_details", "Home", new { id=IDR });
            }
            else {
                TempData["advertencia"] = "Something wrong happened, try again.";
                    return RedirectToAction("RoutesM_details", "Home", new { id = IDR });
                }
            }
            catch {
                TempData["advertencia"] = "Something wrong happened, try again.";
                return RedirectToAction("RoutesM_details", "Home", new { id = IDR });
            }

           
        }

        [ValidateAntiForgeryToken]
        public ActionResult DeleteBrandCompetitor(string idbrandD)
        {
            try
            {
                int IDbrand = Convert.ToInt32(idbrandD);

                Brand_competitors brand = db.Brand_competitors.Find(IDbrand);
                db.Brand_competitors.Remove(brand);
                db.SaveChanges();



                TempData["exito"] = "Brand competitor deleted successfully.";
                return RedirectToAction("Brandcompetitors", "Home", null);
            }
            catch {
                TempData["advertencia"] = "Something wrong happened, try again.";
                return RedirectToAction("Brandcompetitors", "Home", null);
            }

            
     
        }
        //********************************FIN Brandcompetitors
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoute(string ID_routeD)
        {
            try
            {
                int id = Convert.ToInt32(ID_routeD);

                //Eliminamos las visitas
                var visitas = (from a in db.VisitsM where(a.ID_empresa== GlobalVariables.ID_EMPRESA_USUARIO && a.ID_route== id) select a).ToList();
                var visitasArray = (from b in db.VisitsM where (b.ID_empresa == GlobalVariables.ID_EMPRESA_USUARIO && b.ID_route == id) select b.ID_visit).ToArray();

                foreach (var visit in visitas) {
                    VisitsM visita = db.VisitsM.Find(visit.ID_visit);
                    if (visita != null){
                        db.VisitsM.Remove(visita);
                        db.SaveChanges();
                    }
                }

                //Eliminamos las actividades
                var actividades = (from e in db.ActivitiesM where (e.ID_empresa == GlobalVariables.ID_EMPRESA_USUARIO && visitasArray.Contains(e.ID_visit)) select e).ToList();
                var actividadesArray = (from f in db.ActivitiesM where (f.ID_empresa == GlobalVariables.ID_EMPRESA_USUARIO && visitasArray.Contains(f.ID_visit)) select f.ID_activity).ToArray();


                foreach (var act in actividades)
                {
                    ActivitiesM actividad = db.ActivitiesM.Find(act.ID_activity);
                    if (actividad != null)
                    {
                        db.ActivitiesM.Remove(actividad);
                        db.SaveChanges();
                    }
                }
                //Eliminamos los detalles de formulario

                var detalles = (from h in db.FormsM_details where (actividadesArray.Contains(h.ID_visit)) select h).ToList();
                //var detallesArray = (from k in db.FormsM_details where (actividadesArray.Contains(k.ID_visit)) select k.ID_details).ToArray();
                foreach (var det in detalles)
                {
                    FormsM_details detalle = db.FormsM_details.Find(det.ID_details);
                    if (detalle != null)
                    {
                        db.FormsM_details.Remove(detalle);
                        db.SaveChanges();
                    }
                }

                //Eliminamos la ruta
                RoutesM ruta = db.RoutesM.Find(id);
                db.RoutesM.Remove(ruta);
                db.SaveChanges();

                TempData["exito"] = "Route deleted successfully.";
                return RedirectToAction("RoutesM", "Home", null);
            }
            catch
            {
                TempData["advertencia"] = "Something wrong happened, try again.";
                return RedirectToAction("RoutesM", "Home", null);
            }



        }
        //
        [ValidateAntiForgeryToken]
        public ActionResult DeleteVisit(string ID_visitD)
        {
            int routeid = 0;
            try
            {
                
                int id = Convert.ToInt32(ID_visitD);


                    VisitsM visita = db.VisitsM.Find(id);
                    if (visita != null)
                    {
                        routeid = visita.ID_route;
                        db.VisitsM.Remove(visita);
                        db.SaveChanges();
                    }
                

                //Eliminamos las actividades
                var actividades = (from e in db.ActivitiesM where (e.ID_visit==id) select e).ToList();
                var actividadesArray = (from f in db.ActivitiesM where (f.ID_visit == id) select f.ID_activity).ToArray();


                foreach (var act in actividades)
                {
                    ActivitiesM actividad = db.ActivitiesM.Find(act.ID_activity);
                    if (actividad != null)
                    {
                        db.ActivitiesM.Remove(actividad);
                        db.SaveChanges();
                    }
                }
                //Eliminamos los detalles de formulario

                var detalles = (from h in db.FormsM_details where (actividadesArray.Contains(h.ID_visit)) select h).ToList();
                //var detallesArray = (from k in db.FormsM_details where (actividadesArray.Contains(k.ID_visit)) select k.ID_details).ToArray();
                foreach (var det in detalles)
                {
                    FormsM_details detalle = db.FormsM_details.Find(det.ID_details);
                    if (detalle != null)
                    {
                        db.FormsM_details.Remove(detalle);
                        db.SaveChanges();
                    }
                }



                TempData["exito"] = "Visit deleted successfully.";
                return RedirectToAction("RoutesM_details", "Home", new { id=routeid});
            }
            catch
            {
                TempData["advertencia"] = "Something wrong happened, try again.";
                return RedirectToAction("RoutesM_details", "Home", new { id = routeid });
            }



        }
    }
}
