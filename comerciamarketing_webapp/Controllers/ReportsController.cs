using comerciamarketing_webapp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace comerciamarketing_webapp.Controllers
{
    public class ReportsController : Controller
    {
        public class activitiesTypes
        {
            public int id { get; set; }
            public string name { get; set; }
            public string activitytype { get; set; }
            public int count { get; set; }
        }

        public class Brands
        {
            public string id { get; set; }
            public string name { get; set; }
            public int count { get; set; }
        }
        // GET: Reports
        public ActionResult Reports(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Admin";
                ViewData["Page"] = "Reports";
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

                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                //Verificamos si es marca cliente
                var escliente = false;
                if (activeuser.ID_tipomembresia == 2 || activeuser.ID_tipomembresia == 3 || activeuser.ID_tipomembresia == 4)
                {
                    escliente = true;
                    ViewBag.escliente = 1;
                }
                else {
                    ViewBag.escliente = 0;
                }

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();
                List<activitiesTypes> distinct = new List<activitiesTypes>();
                List<Brands> distinctBrands = new List<Brands>();
                int contf = 0;

                using (var db = new dbComerciaEntities())
                {


                    IQueryable<activitiesVisitsBrands> lstActivities;


                    if (customersel == null || customersel == "" || customersel == "0")
                        {
                        lstActivities = (from a in db.ActivitiesM
                                         join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                         where (a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished == true)
                                         //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                         select new activitiesVisitsBrands
                                         {
                                             ID_activity = a.ID_activity,
                                             ID_visit = a.ID_visit,
                                             ID_form = a.ID_form,
                                             formName = a.description,
                                             ID_store = b.ID_store,
                                             store = b.store,
                                             visitDate = b.visit_date,
                                             ID_customer = a.ID_customer,
                                             Customer = a.Customer,
                                             isfinished = a.isfinished,
                                             id_usuarioend = a.ID_usuarioEnd,
                                             id_usuarioendexternal = a.ID_usuarioEndString,
                                             id_activitytype = a.ID_activitytype,
                                             ActivityName = "",
                                             Comments = a.comments,
                                             ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                             Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                             count = 0
                                         });

                    }
                    else {
                        if (brandsel == null || brandsel == 0)
                        {
                            lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                             where (a.ID_customer == customersel && (a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished == true))
                                             select new activitiesVisitsBrands
                                             {
                                                 ID_activity = a.ID_activity,
                                                 ID_visit = a.ID_visit,
                                                 ID_form = a.ID_form,
                                                 formName = a.description,
                                                 ID_store = b.ID_store,
                                                 store = b.store,
                                                 visitDate = b.visit_date,
                                                 ID_customer = a.ID_customer,
                                                 Customer = a.Customer,
                                                 isfinished = a.isfinished,
                                                 id_usuarioend = a.ID_usuarioEnd,
                                                 id_usuarioendexternal = a.ID_usuarioEndString,
                                                 id_activitytype = a.ID_activitytype,
                                                 ActivityName = "",
                                                 Comments = a.comments,
                                                 ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                                 Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                                 count = 0
                                             });
                        }
                        else {
                            lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                             where (a.ID_customer == customersel && (a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished == true))
                                             select new activitiesVisitsBrands
                                             {
                                                 ID_activity = a.ID_activity,
                                                 ID_visit = a.ID_visit,
                                                 ID_form = a.ID_form,
                                                 formName = a.description,
                                                 ID_store = b.ID_store,
                                                 store = b.store,
                                                 visitDate = b.visit_date,
                                                 ID_customer = a.ID_customer,
                                                 Customer = a.Customer,
                                                 isfinished = a.isfinished,
                                                 id_usuarioend = a.ID_usuarioEnd,
                                                 id_usuarioendexternal = a.ID_usuarioEndString,
                                                 id_activitytype = a.ID_activitytype,
                                                 ActivityName = "",
                                                 Comments = a.comments,
                                                 ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                                 Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                                 count = 0
                                             });
                            var brandstring = brandsel.ToString();
                            lstActivities = lstActivities.Where(c => c.ID_brand == brandstring);
                        }
   
                    }

                    contf = lstActivities.Count();

                    distinct = (from b in lstActivities
                                select new activitiesTypes
                                {
                                    id = b.ID_form,
                                    name = b.formName,
                                    activitytype = b.ActivityName,
                                    count = lstActivities
    .Where(x => x.ID_form == b.ID_form)
    .Select(x => x).Count()
                                }).Distinct().ToList();


                    distinctBrands = (from b in lstActivities
                                      group b by b.ID_brand into g
                                      where (g.Key != "" && g.Key != null && g.Key !="0")
                                      select new Brands
                                      {
                                          id = g.Key,
                                          name = g.Select(m => m.Brand).FirstOrDefault(),
                                          count = 0
                                      }).ToList();



                }
                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        List<OCRD> customers;
                        if (escliente == true)
                        {
                            customers = (from b in CMKdb.OCRD where (b.U_CardCodeDLI==customersel) select b).ToList();
              
                        }
                        else {
                            customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                           
                        }
                        ViewBag.customerssel = customers.ToList();

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.CustomerSelCode = "0";
                        }
                        else
                        {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                        var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                        if (brandsel == null || brandsel == 0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else
                        {
                            ViewBag.BrandLabel = brandcmk.FirmName;
                            //ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }

                catch
                {

                    ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
                }


                ViewBag.lstTypes = distinct;
                ViewBag.brands = distinctBrands;
                ViewBag.lstCount = contf;

                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }

        public ActionResult Visits(string id, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Admin";
                ViewData["Page"] = "Reports";
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

                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                //Verificamos si es marca cliente
                var escliente = false;
                if (activeuser.ID_tipomembresia == 2 || activeuser.ID_tipomembresia == 3 || activeuser.ID_tipomembresia == 4)
                {
                    escliente = true;
                    ViewBag.escliente = 1;
                }
                else
                {
                    ViewBag.escliente = 0;
                }

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();
                List<activitiesTypes> distinct = new List<activitiesTypes>();
                List<Brands> distinctBrands = new List<Brands>();

                List<VisitsInfo> lstVisitas;
 

                using (var db = new dbComerciaEntities())
                {
                  




                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            lstVisitas = (from a in db.ActivitiesM
                                          join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                          where (a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished == true)
                                          //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                          select new VisitsInfo
                                          {
                                              ID_visit = a.ID_visit,
                                              ID_store = b.ID_store,
                                              store = b.store,
                                              addresss = b.address + ", " + b.city + ", " + b.state + ", " + b.zipcode,
                                              visitDate = b.visit_date,
                                              ID_customer = a.ID_customer,
                                              Customer = a.Customer,
                                              id_usuarioend = a.ID_usuarioEnd,
                                              id_usuarioendexternal = a.ID_usuarioEndString,
                                              usuario_name = db.Usuarios.Where(user => user.ID_usuario == a.ID_usuarioEnd).Select(c => c.nombre + " " + c.apellido).FirstOrDefault() != null ? db.Usuarios.Where(user => user.ID_usuario == a.ID_usuarioEnd).Select(c => c.nombre + " " + c.apellido).FirstOrDefault() : "External user",
                                              Comments = a.comments,
                                              ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                              Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                              pictureBefore = "",
                                              pictureAfter = ""
                                          }).ToList();

                        }
                        else
                        {
                            lstVisitas = (from a in db.ActivitiesM
                                          join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                          where (a.ID_customer == customersel && (a.date >= filtrostartdate && a.date <= filtroenddate) && a.isfinished == true)
                                          //where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
                                          select new VisitsInfo
                                          {
                                              ID_visit = a.ID_visit,
                                              ID_store = b.ID_store,
                                              store = b.store,
                                              addresss = b.address + ", " + b.city + ", " + b.state + ", " + b.zipcode,
                                              visitDate = b.visit_date,
                                              ID_customer = a.ID_customer,
                                              Customer = a.Customer,
                                              id_usuarioend = a.ID_usuarioEnd,
                                              id_usuarioendexternal = a.ID_usuarioEndString,
                                              usuario_name = db.Usuarios.Where(user => user.ID_usuario == a.ID_usuarioEnd).Select(c => c.nombre + " " + c.apellido).FirstOrDefault() != null ? db.Usuarios.Where(user => user.ID_usuario == a.ID_usuarioEnd).Select(c => c.nombre + " " + c.apellido).FirstOrDefault() : "External User",
                                              Comments = a.comments,
                                              ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                              Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                              pictureBefore = "",
                                              pictureAfter = ""
                                          }).ToList();
                            if (brandsel == null || brandsel == 0)
                            {

                            }
                            else
                            {
                                var brandstring = brandsel.ToString();
                                lstVisitas = lstVisitas.Where(c => c.ID_brand == brandstring).ToList();
                            }

                        }

                    lstVisitas = (from p in lstVisitas
                                  group p by new
                                  {
                                      p.ID_visit,
                                      p.ID_store,
                                      p.store,
                                      p.addresss,
                                      p.visitDate,
                                      p.ID_customer,
                                      p.Customer,
                                      p.id_usuarioend,
                                      p.id_usuarioendexternal,
                                      p.usuario_name,
                                      p.Comments,
                                      p.ID_brand,
                                      p.Brand,
                                      p.pictureBefore,
                                      p.pictureAfter
                                  } into g
                                  select new VisitsInfo{ ID_visit = g.Key.ID_visit, ID_store = g.Key.ID_store, store = g.Key.store, addresss = g.Key.addresss,
                                      visitDate = g.Key.visitDate,
                                      ID_customer = g.Key.ID_customer,
                                      Customer = g.Key.Customer,
                                      id_usuarioend = g.Key.id_usuarioend,
                                      id_usuarioendexternal = g.Key.id_usuarioendexternal,
                                      usuario_name = g.Key.usuario_name,
                                      Comments = g.Key.Comments,
                                      ID_brand = g.Key.ID_brand,
                                      Brand = g.Key.Brand,
                                      pictureBefore=g.Key.pictureBefore,
                                      pictureAfter=g.Key.pictureAfter

                                  }).ToList();

                    Session["VisitsInfo"] = lstVisitas;

                    //var visits = (from j in db.VisitsM where (distinctVisits.Contains(j.ID_visit)) select j).ToList();

                    //                distinct = (from b in lstActivities
                    //                            select new activitiesTypes
                    //                            {
                    //                                id = b.ID_form,
                    //                                name = b.formName,
                    //                                activitytype = b.ActivityName,
                    //                                count = lstActivities
                    //.Where(x => x.ID_form == b.ID_form)
                    //.Select(x => x).Count()
                    //                            }).Distinct().ToList();


                    //                distinctBrands = (from b in lstActivities
                    //                                  group b by b.ID_brand into g
                    //                                  where (g.Key != "" && g.Key != null && g.Key != "0")
                    //                                  select new Brands
                    //                                  {
                    //                                      id = g.Key,
                    //                                      name = g.Select(m => m.Brand).FirstOrDefault(),
                    //                                      count = 0
                    //                                  }).ToList();



                }
                    try
                    {

                        using (var CMKdb = new COM_MKEntities())
                        {
                            //LISTADO DE CLIENTES
                            List<OCRD> customers;
                            if (escliente == true)
                            {
                                customers = (from b in CMKdb.OCRD where ((b.CardCode == customersel || b.U_CardCodeDLI == customersel)) select b).ToList();

                            }
                            else
                            {
                                customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                            }
                            ViewBag.customerssel = customers.ToList();

                            if (customersel == null || customersel == "" || customersel == "0")
                            {
                                ViewBag.CustomersLabel = "All Customers";
                                ViewBag.CustomerSelCode = "0";
                            }
                            else
                            {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                            var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                            if (brandsel == null || brandsel == 0)
                            {
                                ViewBag.BrandLabel = "All Brands";
                                ViewBag.BrandSelCode = "0";
                            }
                            else
                            {
                                ViewBag.BrandLabel = brandcmk.FirmName;
                                //ViewBag.BrandLabel = "All Brands";
                                ViewBag.BrandSelCode = brandcmk.FirmCode;
                            }


                        }

                    }

                    catch
                    {

                        ViewBag.CustomersLabel = "All Customers";
                        ViewBag.customerssel = new List<OCRD>();
                        ViewBag.CustomerSelCode = "0";
                        ViewBag.BrandLabel = "All Brands";
                        ViewBag.BrandSelCode = "0";
                    }


                    ViewBag.lstTypes = distinct;
                    ViewBag.brands = distinctBrands;


                    return View(lstVisitas);
                
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }

        //QUICK VISIT
        public ActionResult RD_FORM1(string id, int report, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Admin";
                ViewData["Page"] = "Reports";
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
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                //Verificamos si es marca cliente
                var escliente = false;
                if (activeuser.ID_tipomembresia == 2 || activeuser.ID_tipomembresia == 3 || activeuser.ID_tipomembresia == 4)
                {
                    escliente = true;
                }
                using (var db = new dbComerciaEntities())
                {
                    IQueryable<activitiesCompleteInfo> lstActivities;

                    if (customersel == null || customersel == "" || customersel == "0")
                    { //Es Vendor
                        lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                             where ((a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished == true && a.ID_form ==1))
                                             select new activitiesCompleteInfo
                                             {
                                                 ID_activity = a.ID_activity,
                                                 ID_visit = a.ID_visit,
                                                 ID_form = a.ID_form,
                                                 formName = a.description,
                                                 ID_store = b.ID_store,
                                                 store = b.store,
                                                 address = b.address,
                                                 city = b.city,
                                                 state = b.state,
                                                 usuarioName = (from usu in db.Usuarios where(usu.ID_usuario==a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                                 visitDate = b.visit_date,
                                                 ID_customer = a.ID_customer,
                                                 Customer = a.Customer,
                                                 isfinished = a.isfinished,
                                                 id_usuarioend = a.ID_usuarioEnd,
                                                 id_usuarioendexternal = a.ID_usuarioEndString,
                                                 id_activitytype = a.ID_activitytype,
                                                 ActivityName = "",
                                                 Comments = a.comments,
                                                 ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                                 Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                                 count = 0
                                             });
                    }
                    else{
                        lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                             where (a.ID_customer == customersel && a.date >= filtrostartdate && a.date <= filtroenddate && a.ID_form == 1 && a.isfinished==true)
                                             select new activitiesCompleteInfo
                                             {
                                                 ID_activity = a.ID_activity,
                                                 ID_visit = a.ID_visit,
                                                 ID_form = a.ID_form,
                                                 formName = a.description,
                                                 ID_store = b.ID_store,
                                                 store = b.store,
                                                 address = b.address,
                                                 city = b.city,
                                                 state = b.state,
                                                 usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                                 visitDate = b.visit_date,
                                                 ID_customer = a.ID_customer,
                                                 Customer = a.Customer,
                                                 isfinished = a.isfinished,
                                                 id_usuarioend = a.ID_usuarioEnd,
                                                 id_usuarioendexternal = a.ID_usuarioEndString,
                                                 id_activitytype = a.ID_activitytype,
                                                 ActivityName = "",
                                                 Comments = a.comments,
                                                 ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                                 Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                                 count = 0
                                             });
                    }

                    if (brandsel != null && brandsel != 0)
                    {
                        var brandint = brandsel.ToString();
                        lstActivities = lstActivities.Where(a => a.ID_brand == brandint);
                    }

                    //ViewBag.ActivityName = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().formName;
                    //ViewBag.ActivityType = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().ActivityName;
                    ViewBag.lstActivities = lstActivities.Where(a => a.ID_form == report).ToList();
                }
                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        List<OCRD> customers;
                        if (escliente == true)
                        {
                            customers = (from b in CMKdb.OCRD where (b.CardCode == customersel) select b).ToList();

                        }
                        else
                        {
                            customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                        }
                        ViewBag.customerssel = customers.ToList();

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.CustomerSelCode = "0";
                        }
                        else
                        {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                        var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                        if (brandsel == null || brandsel == 0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else
                        {
                            ViewBag.BrandLabel = brandcmk.FirmName;
                            
                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }

                catch
                {

                    ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
                }
                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }
        //QUICK VISIT
        public ActionResult RD_FORM32(string id, int report, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Reports";
                ViewData["Page"] = "Reports";
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
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                //Verificamos si es marca cliente
                var escliente = false;
                if (activeuser.ID_tipomembresia == 2 || activeuser.ID_tipomembresia == 3 || activeuser.ID_tipomembresia == 4)
                {
                    escliente = true;
                }
                using (var db = new dbComerciaEntities())
                {
                    IQueryable<activitiesCompleteInfo> lstActivities;
                    if (customersel == null || customersel == "" || customersel == "0")
                    { //Es Vendor
                        lstActivities = (from a in db.ActivitiesM
                                         join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                         where ((a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished && a.ID_form == 32))
                                         select new activitiesCompleteInfo
                                         {
                                             ID_activity = a.ID_activity,
                                             ID_visit = a.ID_visit,
                                             ID_form = a.ID_form,
                                             formName = a.description,
                                             ID_store = b.ID_store,
                                             store = b.store,
                                             address = b.address,
                                             city = b.city,
                                             state = b.state,
                                             usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                             visitDate = b.visit_date,
                                             ID_customer = a.ID_customer,
                                             Customer = a.Customer,
                                             isfinished = a.isfinished,
                                             id_usuarioend = a.ID_usuarioEnd,
                                             id_usuarioendexternal = a.ID_usuarioEndString,
                                             id_activitytype = a.ID_activitytype,
                                             ActivityName = "",
                                             Comments = a.comments,
                                             ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                             Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                             count = 0
                                         });
                    }
                    else
                    {
                        lstActivities = (from a in db.ActivitiesM
                                         join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                         where (a.ID_customer == customersel && a.date >= filtrostartdate && a.date <= filtroenddate && a.ID_form == 32  && a.isfinished == true)
                                         select new activitiesCompleteInfo
                                         {
                                             ID_activity = a.ID_activity,
                                             ID_visit = a.ID_visit,
                                             ID_form = a.ID_form,
                                             formName = a.description,
                                             ID_store = b.ID_store,
                                             store = b.store,
                                             address = b.address,
                                             city = b.city,
                                             state = b.state,
                                             usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                             visitDate = b.visit_date,
                                             ID_customer = a.ID_customer,
                                             Customer = a.Customer,
                                             isfinished = a.isfinished,
                                             id_usuarioend = a.ID_usuarioEnd,
                                             id_usuarioendexternal = a.ID_usuarioEndString,
                                             id_activitytype = a.ID_activitytype,
                                             ActivityName = "",
                                             Comments = a.comments,
                                             ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                             Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                             count = 0
                                         });
                    }

                    if (brandsel != null && brandsel != 0)
                    {
                        var brandint = brandsel.ToString();
                        lstActivities = lstActivities.Where(a => a.ID_brand == brandint);
                    }


                    //ViewBag.ActivityName = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().formName;
                    //ViewBag.ActivityType = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().ActivityName;
                    ViewBag.lstActivities = lstActivities.Where(a => a.ID_form == report).ToList();
                }
                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        List<OCRD> customers;
                        if (escliente == true)
                        {
                            customers = (from b in CMKdb.OCRD where ((b.CardCode == customersel || b.U_CardCodeDLI == customersel)) select b).ToList();

                        }
                        else
                        {
                            customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                        }
                        ViewBag.customerssel = customers.ToList();

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.CustomerSelCode = "0";
                        }
                        else
                        {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                        var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                        if (brandsel == null || brandsel == 0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else
                        {
                            ViewBag.BrandLabel = brandcmk.FirmName;
                      
                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }

                catch
                {

                    ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
                }
                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }


        //VISITA 1.1
        public ActionResult RD_FORM3(string id, int report, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Reports";
                ViewData["Page"] = "Reports";
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
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();
                //Verificamos si es marca cliente
                var escliente = false;
                if (activeuser.ID_tipomembresia == 2 || activeuser.ID_tipomembresia == 3 || activeuser.ID_tipomembresia == 4)
                {
                    escliente = true;
                }
                using (var db = new dbComerciaEntities())
                {
                    IQueryable<activitiesCompleteInfo> lstActivities;
                    if (customersel == null || customersel == "" || customersel == "0")
                    { //Es Vendor
                        lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                             where ((a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished==true && a.ID_form == 3))
                                             select new activitiesCompleteInfo
                                             {
                                                 ID_activity = a.ID_activity,
                                                 ID_visit = a.ID_visit,
                                                 ID_form = a.ID_form,
                                                 formName = a.description,
                                                 ID_store = b.ID_store,
                                                 store = b.store,
                                                 address = b.address,
                                                 city = b.city,
                                                 state = b.state,
                                                 usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                                 visitDate = b.visit_date,
                                                 ID_customer = a.ID_customer,
                                                 Customer = a.Customer,
                                                 isfinished = a.isfinished,
                                                 id_usuarioend = a.ID_usuarioEnd,
                                                 id_usuarioendexternal = a.ID_usuarioEndString,
                                                 id_activitytype = a.ID_activitytype,
                                                 ActivityName = "",
                                                 Comments = a.comments,
                                                 ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                                 Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                                 count = 0
                                             });
                    }
                    else {
                        lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                             where (a.ID_customer == customersel && a.date >= filtrostartdate && a.date <= filtroenddate && a.ID_form == 3 && a.isfinished==true)                                           
                                             select new activitiesCompleteInfo
                                             {
                                                 ID_activity = a.ID_activity,
                                                 ID_visit = a.ID_visit,
                                                 ID_form = a.ID_form,
                                                 formName = a.description,
                                                 ID_store = b.ID_store,
                                                 store = b.store,
                                                 address = b.address,
                                                 city = b.city,
                                                 state = b.state,
                                                 usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                                 visitDate = b.visit_date,
                                                 ID_customer = a.ID_customer,
                                                 Customer = a.Customer,
                                                 isfinished = a.isfinished,
                                                 id_usuarioend = a.ID_usuarioEnd,
                                                 id_usuarioendexternal = a.ID_usuarioEndString,
                                                 id_activitytype = a.ID_activitytype,
                                                 ActivityName = "",
                                                 Comments = a.comments,
                                                 ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                                 Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                                 count = 0
                                             });
                    }

                    if (brandsel != null && brandsel != 0)
                    {
                        var brandint = brandsel.ToString();
                        lstActivities = lstActivities.Where(a => a.ID_brand == brandint);
                    }
                    //ViewBag.ActivityName = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().formName;
                    //ViewBag.ActivityType = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().ActivityName;
                    ViewBag.lstActivities = lstActivities.Where(a => a.ID_form == report).ToList();
                }
                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        List<OCRD> customers;
                        if (escliente == true)
                        {
                            customers = (from b in CMKdb.OCRD where (b.CardCode == customersel) select b).ToList();

                        }
                        else
                        {
                            customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                        }
                        ViewBag.customerssel = customers.ToList();

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.CustomerSelCode = "0";
                        }
                        else
                        {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                        var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                        if (brandsel == null || brandsel == 0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else
                        {
                            ViewBag.BrandLabel = brandcmk.FirmName;
                     
                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }

                catch
                {

                    ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
                }
                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }

        //VISITA 2.0
        public ActionResult RD_FORM42(string id, int report, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Reports";
                ViewData["Page"] = "Reports";
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
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();
                //Verificamos si es marca cliente
                var escliente = false;
                if (activeuser.ID_tipomembresia == 2 || activeuser.ID_tipomembresia == 3 || activeuser.ID_tipomembresia == 4)
                {
                    escliente = true;
                }
                using (var db = new dbComerciaEntities())
                {
                    IQueryable<activitiesCompleteInfo> lstActivities;
                    if (customersel == null || customersel == "" || customersel == "0")
                    { //Es Vendor
                        lstActivities = (from a in db.ActivitiesM
                                         join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                         where ((a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished==true && a.ID_form == 42))
                                         select new activitiesCompleteInfo
                                         {
                                             ID_activity = a.ID_activity,
                                             ID_visit = a.ID_visit,
                                             ID_form = a.ID_form,
                                             formName = a.description,
                                             ID_store = b.ID_store,
                                             store = b.store,
                                             address = b.address,
                                             city = b.city,
                                             state = b.state,
                                             usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                             visitDate = b.visit_date,
                                             ID_customer = a.ID_customer,
                                             Customer = a.Customer,
                                             isfinished = a.isfinished,
                                             id_usuarioend = a.ID_usuarioEnd,
                                             id_usuarioendexternal = a.ID_usuarioEndString,
                                             id_activitytype = a.ID_activitytype,
                                             ActivityName = "",
                                             Comments = a.comments,
                                             ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                             Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                             count = 0
                                         });
                    }
                    else
                    {
                        lstActivities = (from a in db.ActivitiesM
                                         join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                         where (a.ID_customer == customersel && a.date >= filtrostartdate && a.date <= filtroenddate && a.ID_form == 42 && a.isfinished == true)
                                         select new activitiesCompleteInfo
                                         {
                                             ID_activity = a.ID_activity,
                                             ID_visit = a.ID_visit,
                                             ID_form = a.ID_form,
                                             formName = a.description,
                                             ID_store = b.ID_store,
                                             store = b.store,
                                             address = b.address,
                                             city = b.city,
                                             state = b.state,
                                             usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                             visitDate = b.visit_date,
                                             ID_customer = a.ID_customer,
                                             Customer = a.Customer,
                                             isfinished = a.isfinished,
                                             id_usuarioend = a.ID_usuarioEnd,
                                             id_usuarioendexternal = a.ID_usuarioEndString,
                                             id_activitytype = a.ID_activitytype,
                                             ActivityName = "",
                                             Comments = a.comments,
                                             ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                             Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                             count = 0
                                         });
                    }


                    if (brandsel != null && brandsel != 0)
                    {
                        var brandint = brandsel.ToString();
                        lstActivities= lstActivities.Where(a => a.ID_brand == brandint);
                    }
                    //ViewBag.ActivityName = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().formName;
                    //ViewBag.ActivityType = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().ActivityName;
                    ViewBag.lstActivities = lstActivities.Where(a => a.ID_form == report).ToList();
                }
                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        List<OCRD> customers;
                        if (escliente == true)
                        {
                            customers = (from b in CMKdb.OCRD where (b.CardCode == customersel) select b).ToList();

                        }
                        else
                        {
                            customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                        }
                        ViewBag.customerssel = customers.ToList();

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.CustomerSelCode = "0";
                        }
                        else
                        {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                        var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                        if (brandsel == null || brandsel == 0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else
                        {
                            ViewBag.BrandLabel = brandcmk.FirmName;

                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }

                catch
                {

                    ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
                }
                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }

        //AUDIT REPORT 1
        public ActionResult RD_AUDITGRL1(string id, int report, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Reports";
                ViewData["Page"] = "Reports";
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
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                //Verificamos si es marca cliente
                var escliente = false;
                if (activeuser.ID_tipomembresia == 2 || activeuser.ID_tipomembresia == 3 || activeuser.ID_tipomembresia == 4)
                {
                    escliente = true;
                }
                using (var db = new dbComerciaEntities())
                {

                    List<int> lstAudits = new List<int>() { 15,
17,
18,
19,
20,
21,
23,
24,
33};
                    IQueryable<activitiesCompleteInfo> lstActivities;

                    if (customersel == null || customersel == "" || customersel == "0")
                    { //Es Vendor

                       lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                             where ((a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished==true && lstAudits.Contains(a.ID_form)))
                                             select new activitiesCompleteInfo
                                             {
                                                 ID_activity = a.ID_activity,
                                                 ID_visit = a.ID_visit,
                                                 ID_form = a.ID_form,
                                                 formName = a.description,
                                                 ID_store = b.ID_store,
                                                 store = b.store,
                                                 address = b.address,
                                                 city = b.city,
                                                 state = b.state,
                                                 usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                                 visitDate = b.visit_date,
                                                 ID_customer = a.ID_customer,
                                                 Customer = a.Customer,
                                                 isfinished = a.isfinished,
                                                 id_usuarioend = a.ID_usuarioEnd,
                                                 id_usuarioendexternal = a.ID_usuarioEndString,
                                                 id_activitytype = a.ID_activitytype,
                                                 ActivityName = "",
                                                 Comments = a.comments,
                                                 ID_brand = "",
                                                 Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                                 count = 0
                                             });
                    }
                    else {

                        lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                             where (a.ID_customer == customersel && a.date >= filtrostartdate && a.date <= filtroenddate && lstAudits.Contains(a.ID_form) && a.isfinished==true)
                                             select new activitiesCompleteInfo
                                             {
                                                 ID_activity = a.ID_activity,
                                                 ID_visit = a.ID_visit,
                                                 ID_form = a.ID_form,
                                                 formName = a.description,
                                                 ID_store = b.ID_store,
                                                 store = b.store,
                                                 address = b.address,
                                                 city = b.city,
                                                 state = b.state,
                                                 usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                                 visitDate = b.visit_date,
                                                 ID_customer = a.ID_customer,
                                                 Customer = a.Customer,
                                                 isfinished = a.isfinished,
                                                 id_usuarioend = a.ID_usuarioEnd,
                                                 id_usuarioendexternal = a.ID_usuarioEndString,
                                                 id_activitytype = a.ID_activitytype,
                                                 ActivityName = "",
                                                 Comments = a.comments,
                                                 ID_brand = "",
                                                 Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                                 count = 0
                                             });
                    }



                    ViewBag.ActivityName = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().formName;
                    //ViewBag.ActivityType = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().ActivityName;
                    ViewBag.lstActivities = lstActivities.Where(a => a.ID_form == report).ToList();
                }

                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        List<OCRD> customers;
                        if (escliente == true)
                        {
                            customers = (from b in CMKdb.OCRD where (b.CardCode == customersel) select b).ToList();

                        }
                        else
                        {
                            customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                        }
                        ViewBag.customerssel = customers.ToList();

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.CustomerSelCode = "0";
                        }
                        else
                        {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                        var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                        if (brandsel == null || brandsel == 0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else
                        {
                            ViewBag.BrandLabel = brandcmk.FirmName;
                         
                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }

                catch
                {

                    ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
                }

                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }

        //AUDIT REPORT 2.0
        public ActionResult RD_AUDITGRL2(string id, int report, string fstartd, string fendd, string stores, int? brandsel, string spartners, string customersel)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Reports";
                ViewData["Page"] = "Reports";
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
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                //Verificamos si es marca cliente
                var escliente = false;
                if (activeuser.ID_tipomembresia == 2 || activeuser.ID_tipomembresia == 3 || activeuser.ID_tipomembresia == 4)
                {
                    escliente = true;
                }
                using (var db = new dbComerciaEntities())
                {

                    List<int> lstAudits = new List<int>() { 40};
                    IQueryable<activitiesCompleteInfo> lstActivities;

                    if (customersel == null || customersel == "" || customersel == "0")
                    { //Es Vendor

                        lstActivities = (from a in db.ActivitiesM
                                         join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                         where ((a.date >= filtrostartdate && a.date <= filtroenddate && a.isfinished == true && lstAudits.Contains(a.ID_form)))
                                         select new activitiesCompleteInfo
                                         {
                                             ID_activity = a.ID_activity,
                                             ID_visit = a.ID_visit,
                                             ID_form = a.ID_form,
                                             formName = a.description,
                                             ID_store = b.ID_store,
                                             store = b.store,
                                             address = b.address,
                                             city = b.city,
                                             state = b.state,
                                             usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                             visitDate = b.visit_date,
                                             ID_customer = a.ID_customer,
                                             Customer = a.Customer,
                                             isfinished = a.isfinished,
                                             id_usuarioend = a.ID_usuarioEnd,
                                             id_usuarioendexternal = a.ID_usuarioEndString,
                                             id_activitytype = a.ID_activitytype,
                                             ActivityName = "",
                                             Comments = a.comments,
                                             ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                             Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                             count = 0
                                         });
                    }
                    else
                    {

                        lstActivities = (from a in db.ActivitiesM
                                         join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                         where (a.ID_customer == customersel && a.date >= filtrostartdate && a.date <= filtroenddate && lstAudits.Contains(a.ID_form) && a.isfinished == true)
                                         select new activitiesCompleteInfo
                                         {
                                             ID_activity = a.ID_activity,
                                             ID_visit = a.ID_visit,
                                             ID_form = a.ID_form,
                                             formName = a.description,
                                             ID_store = b.ID_store,
                                             store = b.store,
                                             address = b.address,
                                             city = b.city,
                                             state = b.state,
                                             usuarioName = (from usu in db.Usuarios where (usu.ID_usuario == a.ID_usuarioEnd) select (usu.nombre + " " + usu.apellido)).FirstOrDefault(),
                                             visitDate = b.visit_date,
                                             ID_customer = a.ID_customer,
                                             Customer = a.Customer,
                                             isfinished = a.isfinished,
                                             id_usuarioend = a.ID_usuarioEnd,
                                             id_usuarioendexternal = a.ID_usuarioEndString,
                                             id_activitytype = a.ID_activitytype,
                                             ActivityName = "",
                                             Comments = a.comments,
                                             ID_brand = db.FormsM_details.Where(detalle => detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity).Select(c => c.fvalueText).FirstOrDefault(),
                                             Brand = (from detalle in db.FormsM_details where (detalle.ID_formresourcetype == 13 && detalle.ID_visit == a.ID_activity) select detalle.fdescription).FirstOrDefault(),
                                             count = 0
                                         });
                    }
                    if (brandsel != null && brandsel != 0)
                    {
                        var brandint = brandsel.ToString();
                        lstActivities = lstActivities.Where(a => a.ID_brand == brandint);
                    }


                    //ViewBag.ActivityName = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().formName;
                    //ViewBag.ActivityType = lstActivities.Where(a => a.ID_form == report).FirstOrDefault().ActivityName;
                    ViewBag.lstActivities = lstActivities.Where(a => a.ID_form == report).ToList();
                }

                try
                {

                    using (var CMKdb = new COM_MKEntities())
                    {
                        //LISTADO DE CLIENTES
                        List<OCRD> customers;
                        if (escliente == true)
                        {
                            customers = (from b in CMKdb.OCRD where (b.CardCode == customersel) select b).ToList();

                        }
                        else
                        {
                            customers = (from b in CMKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

                        }
                        ViewBag.customerssel = customers.ToList();

                        if (customersel == null || customersel == "" || customersel == "0")
                        {
                            ViewBag.CustomersLabel = "All Customers";
                            ViewBag.CustomerSelCode = "0";
                        }
                        else
                        {
                            var nameC = customers.Where(a => a.CardCode == customersel).FirstOrDefault();

                            if (nameC == null)
                            {
                                var nameDLI = customers.Where(a => a.U_CardCodeDLI == customersel).FirstOrDefault();
                                ViewBag.CustomersLabel = nameDLI.CardName;
                                ViewBag.CustomerSelCode = nameDLI.U_CardCodeDLI;
                            }
                            else
                            {
                                ViewBag.CustomersLabel = nameC.CardName;
                                ViewBag.CustomerSelCode = nameC.CardCode;
                            }
                        }

                        var brandcmk = CMKdb.view_CMKEditorB.Where(i => i.FirmCode == brandsel).FirstOrDefault();

                        if (brandsel == null || brandsel == 0)
                        {
                            ViewBag.BrandLabel = "All Brands";
                            ViewBag.BrandSelCode = "0";
                        }
                        else
                        {
                            ViewBag.BrandLabel = brandcmk.FirmName;

                            ViewBag.BrandSelCode = brandcmk.FirmCode;
                        }


                    }

                }

                catch
                {

                    ViewBag.CustomersLabel = "All Customers";
                    ViewBag.customerssel = new List<OCRD>();
                    ViewBag.CustomerSelCode = "0";
                    ViewBag.BrandLabel = "All Brands";
                    ViewBag.BrandSelCode = "0";
                }

                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }

    }
}