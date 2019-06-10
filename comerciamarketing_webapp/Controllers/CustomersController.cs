using ClosedXML.Excel;
using comerciamarketing_webapp.Models;
using CrystalDecisions.CrystalReports.Engine;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace comerciamarketing_webapp.Controllers
{
    public class CustomersController : Controller
    {
        //private dbComerciaEntities db = new dbComerciaEntities();
        //private COM_MKEntities CMKdb = new COM_MKEntities();


        //using (var db = new dbComerciaEntities())
        //{

        //}
        //using (var CMKdb = new COM_MKEntities())
        //{

        //}


        // GET: Customers
        public ActionResult Dashboard(string id, string fstartd, string fendd, string stores, string brands, string spartners)
        {

            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Dashboard";
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
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
                ViewBag.Membresia = activeuser.Tipo_membresia.descripcion;
                ViewBag.id_customer = id;
                ViewBag.Company = activeuser.Empresas.nombre;


                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }

        public ActionResult Sales_Representatives(string id, string fstartd, string fendd, string stores, string brands, string spartners)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Sales Representatives";
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
                List<Usuarios> lstUsuarios = new List<Usuarios>();
                using (var db = new dbComerciaEntities())
                {
                    lstUsuarios = (from a in db.Usuarios where (a.estados_influencia.Contains(id)) select a).ToList();
                }


                return View(lstUsuarios);
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }

        public ActionResult Calendar_customers(string id, string fstartd, string fendd, string stores, string brands, string spartners)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Calendar";
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


                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }

        public ActionResult Map_customers(string id, string fstartd, string fendd, string stores, string brands, string spartners)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
                ViewData["Page"] = "Map";
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
                //FILTROS
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
                ViewBag.id_customer = id;

                ArrayList myArrList = new ArrayList();
                using (var db = new dbComerciaEntities())
                {
                    var visitsarr = (from a in db.ActivitiesM where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate)) select a.ID_visit).Distinct().Take(10).ToArray();
                    // Convertimos la lista a array
                    var rutas = db.VisitsM.Where(c => visitsarr.Contains(c.ID_visit)).OrderBy(d => d.visit_date).ToList().Take(5);



                    myArrList.AddRange((from p in rutas
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


                }
                ViewBag.routes_map = JsonConvert.SerializeObject(myArrList);



                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }


        public class activitiesTypes
        {
            public int id { get; set; }
            public string name { get; set; }
            public string activitytype { get; set; }
            public int count { get; set; }
        }
        public ActionResult Reports_customers(string id, string fstartd, string fendd, string stores, string brands, string spartners)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
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
                List<activitiesTypes> distinct = new List<activitiesTypes>();
                int contf = 0;
                List<activitiesVisitsBrands> getActivities = Session["getActivities"] as List<activitiesVisitsBrands>;

                if (getActivities != null && getActivities.Count > 0)
                {
                    contf = getActivities.Count();
                    distinct = (from b in getActivities
                                select new activitiesTypes
                                {
                                    id = b.ID_form,
                                    name = b.formName,
                                    activitytype = b.ActivityName,
                                    count = getActivities
    .Where(x => x.ID_form == b.ID_form)
    .Select(x => x).Count()
                                }).GroupBy(c => c.id).Select(c => c.First()).ToList();
                }
                else
                {
                    using (var db = new dbComerciaEntities())
                    {



                        var lstActivities = (from a in db.ActivitiesM
                                             join b in db.VisitsM on a.ID_visit equals b.ID_visit
                                            
                                             where (a.ID_customer == id && (a.date >= filtrostartdate && a.date <= filtroenddate))
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
                                                 ID_brand = "",
                                                 Brand = "Brand test",
                                                 count = 0
                                             }).Take(10);
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
                        Session["getActivities"] = lstActivities.ToList();
                    }
                }


                ViewBag.lstTypes = distinct;
                ViewBag.lstCount = contf;

                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }
        public ActionResult Reports_details(string id, int report, string fstartd, string fendd, string stores, string brands, string spartners)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
                //HEADER
                //PAGINAS ACTIVAS
                ViewData["Menu"] = "Customers";
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

                List<activitiesVisitsBrands> getActivities = Session["getActivities"] as List<activitiesVisitsBrands>;

                if (getActivities.Count == 0)
                {
                    return RedirectToAction("Reports_customers", "Customers", new { id = id });
                }

                ViewBag.ActivityName = getActivities.Where(a => a.ID_form == report).FirstOrDefault().formName;
                ViewBag.ActivityType = getActivities.Where(a => a.ID_form == report).FirstOrDefault().ActivityName;
                ViewBag.lstActivities = getActivities.Where(a => a.ID_form == report).ToList();



                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }


        }
        public class MyObj_formtemplate
        {
            public string id { get; set; }
            public string text { get; set; }
            public string value { get; set; }
        }

        public DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public DataSet getDataSetExportToExcel(string id, List<int> lstact)
        {
            List<QuickVisit_export> activity_header = new List<QuickVisit_export>();
            List<FormsM_details> details;

            //using (var dbs = new dbComerciaEntities())
            //{
                
            //}

            using (var db = new dbComerciaEntities())
            {
                details = (from a in db.FormsM_details where (lstact.Contains(a.ID_visit) && (a.ID_formresourcetype==13 || a.ID_formresourcetype==5 || a.ID_formresourcetype==6)) select a).ToList();

                activity_header = (from a in db.ActivitiesM
                                   join b in db.VisitsM on a.ID_visit equals b.ID_visit

                                   where (a.ID_customer == id && lstact.Contains(a.ID_activity))
                                   select new QuickVisit_export
                                   {
                                       ID = a.ID_activity,
                                       Store = b.store,
                                       Address = b.address,
                                       City = b.city,
                                       ZipCode = b.zipcode,
                                       State = b.state,
                                       date = b.visit_date,
                                       //SalesRepresentative = a.ID_customer,
                                       Brand = "",
                                       //Brand = details.Where(c=>c.ID_visit == a.ID_activity && c.ID_formresourcetype==13).Select(c=>c.fdescription).FirstOrDefault(),//  (from det in details where (det.ID_visit == a.ID_activity && det.ID_formresourcetype == 13) select det.fdescription).SingleOrDefault(),
                                       Activity = a.description,
                                       PictureBefore = "",
                                       PictureAfter = "",
                                       CommentBefore = "",
                                       CommentAfter = ""
                                       //PictureBefore = (from b in details where (b.ID_visit == a.ID_activity && b.ID_formresourcetype == 5 && b.fdescription.Contains("PICTURE 1")) select b.fsource).FirstOrDefault(),
                                       //CommentBefore = (from b in details where (b.ID_visit == a.ID_activity && b.ID_formresourcetype == 6 && b.fdescription.Contains("COMMENT 1")) select b.fsource).FirstOrDefault(),
                                       //PictureAfter = (from b in details where (b.ID_visit == a.ID_activity && b.ID_formresourcetype == 5 && b.fdescription.Contains("PICTURE 2")) select b.fsource).FirstOrDefault(),
                                       //CommentAfter = (from b in details where (b.ID_visit == a.ID_activity && b.ID_formresourcetype == 6 && b.fdescription.Contains("COMMENT 2")) select b.fsource).FirstOrDefault()


                                   }).ToList();
                //form_details = (from c in db.FormsM_details where (lstact.Contains(c.ID_visit)) select c).ToList();



            }

            DataSet ds = new DataSet();
            DataTable dtEmp = new DataTable("ActivitiesData");
            dtEmp = ToDataTable(activity_header);
            //DataTable dtEmpOrder = new DataTable("ActivitiesDetails");  
            //dtEmpOrder = ToDataTable(form_details);

            ds.Tables.Add(dtEmp);
            //ds.Tables.Add(dtEmpOrder);
            return ds;
        }

        public ActionResult Export_QuickVisit(string id, List<MyObj_formtemplate> objects)
        {
            List<int> list = new List<int>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            //UTILIZANDO LIBRERIA 
            DataSet ds = getDataSetExportToExcel(id, list);
            var filename = "QUICK_VISIT" + "" + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Worksheet(1).Name = "ActivitiesData";
                //wb.Worksheet(2).Name = "ActivitiesDetails";
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                var filePathOriginal = Server.MapPath("/Reportes/excel");
                var path2 = "";
                path2 = Path.Combine(filePathOriginal, filename);
                wb.SaveAs(path2);
            }
            return Json(filename, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [DeleteFileAttribute] //Action Filter, it will auto delete the file after download, 
                              //I will explain it later
        public ActionResult Download(string file)
        {
            //get the temp folder and file path in server
            string fullPath = Path.Combine(Server.MapPath("/Reportes/excel"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file);
        }

        public class DeleteFileAttribute : ActionFilterAttribute
        {
            public override void OnResultExecuted(ResultExecutedContext filterContext)
            {
                filterContext.HttpContext.Response.Flush();

                //convert the current filter context to file and get the file path
                string filePath = (filterContext.Result as FilePathResult).FileName;

                //delete the file after download
                System.IO.File.Delete(filePath);
            }
        }

        public ActionResult Print_QuickVisit(string id, List<MyObj_formtemplate> objects)
        {
            List<int> list = new List<int>();
            foreach (var item in objects) {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }


            List<QuickVisit_report> activity_header = new List<QuickVisit_report>();
            List<FormsM_details> form_details = new List<FormsM_details>();
            using (var db = new dbComerciaEntities())
            {
                activity_header = (from a in db.ActivitiesM
                                   join b in db.VisitsM on a.ID_visit equals b.ID_visit
                            
                                   where (a.ID_customer == id && list.Contains(a.ID_activity))
                                   select new QuickVisit_report
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
                                       ID_brand = "",
                                       Brand = "Brand test",
                                       count = 0,
                                       urlimg1 = "",
                                       urlimg2 = "",
                                       urlsign=""
                
                                   }).ToList();


                form_details = (from c in db.FormsM_details where (list.Contains(c.ID_visit)) select c).ToList();

                if (activity_header.Count > 0)               
                {
                    foreach (var item in activity_header) {
                        //Verificamos si existen fotos en el demo (MAX 2 fotos QUICK VISIT)
                        var fotos = (from c in form_details where (c.ID_formresourcetype == 5 && c.ID_visit==item.ID_activity) select c).ToList();

                        int fotosC = fotos.Count();


                        if (fotosC == 2)
                        {
                            if (fotos[0].fsource == "")
                            {
                              
                            }
                            else
                            {
                              item.urlimg1=  Path.GetFullPath(Server.MapPath(fotos[0].fsource));
                            }
                            if (fotos[1].fsource == "")
                            {
                               
                            }
                            else
                            {
                               item.urlimg2=  Path.GetFullPath(Server.MapPath(fotos[1].fsource));
                            }

                        }
                        else if (fotosC == 1)
                        {
                            if (fotos[0].fsource == "")
                            {
                             
                            }
                            else
                            {
                                item.urlimg1 = Path.GetFullPath(Server.MapPath(fotos[0].fsource));
                            }

               
                        }
        
                    }
                }

                
}


            if (activity_header.Count > 0)

            {
                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptQuick_visit.rpt"));

                rd.SetDataSource(activity_header);




                ////Verificams si existe firma electronica
                //var firma = (from d in form_details where (d.ID_formresourcetype == 9) select d).ToList();

                //int firmaC = firma.Count();




                //if (firmaC == 1)
                //{

                //    string data = firma[0].fsource;
                //    if (data != "")
                //    {
                //        var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                //        var binData = Convert.FromBase64String(base64Data);

                //        using (var streamf = new MemoryStream(binData))
                //        {

                //            Bitmap myImage = new Bitmap(streamf);

                //            // Assumes myImage is the PNG you are converting
                //            using (var b = new Bitmap(myImage.Width, myImage.Height))
                //            {
                //                b.SetResolution(myImage.HorizontalResolution, myImage.VerticalResolution);

                //                using (var g = Graphics.FromImage(b))
                //                {
                //                    g.Clear(Color.White);
                //                    g.DrawImageUnscaled(myImage, 0, 0);
                //                }

                //                // Now save b as a JPEG like you normally would

                //                var path = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), "signdequickvisit.jpg");
                //                b.Save(path, ImageFormat.Jpeg);


                //                rd.SetParameterValue("urlimgsign", Path.GetFullPath(path));
                //            }



                //        }
                //    }
                //    else
                //    {
                //        rd.SetParameterValue("urlimgsign", "");

                //    }

                //}
                //else
                //{
                //    rd.SetParameterValue("urlimgsign", "");
                //}


                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                Response.AppendHeader("Content-Disposition", "inline; filename=Quick Visit.pdf;");
       


                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);


                //PARA PREVISULIZACION
                //return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

                //PARA ENVIAR POR CORREO

                try

                {
                    //limpiamos el directorio

                    System.IO.DirectoryInfo di = new DirectoryInfo(filePathOriginal);

                    foreach (FileInfo file in di.GetFiles())

                    {

                        file.Delete();

                    }

                    foreach (DirectoryInfo dir in di.GetDirectories())

                    {

                        dir.Delete(true);

                    }

                }

                catch (Exception e)

                {

                    var mensaje = e.ToString();

                }

                var path2 = "";
                var filename = "QUICK VISIT" + "" + ".pdf";
                path2 = Path.Combine(filePathOriginal, filename);
                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path2);

                var urlcontent = Url.Content("~/Reportes/pdf/" + filename + "");

                return Json(urlcontent);


            }
            else
            {

                return RedirectToAction("Index", "Home", new { access = false });

            }
        }

        [HttpPost]
        public ActionResult DownloadZip(string id, List<MyObj_formtemplate> objects)
        {

            List<int> list = new List<int>();
            foreach (var item in objects)
            {
                var idact = item.id.Substring(4);
                list.Add(idact.ToInt32());
            }

            List<FormsM_details> detalles = new List<FormsM_details>();
            List<FileModel> files = new List<FileModel>();

            using (var db = new dbComerciaEntities())
            {
                detalles = (from c in db.FormsM_details where (list.Contains(c.ID_visit) && c.ID_formresourcetype==5) select c).ToList();
            }

                foreach (var item in detalles)
            {
                files.Add(new FileModel()
                {
                    id = item.ID_details,
                    FileName = Path.GetFileName(item.fsource),
                    FileActivity = item.fdescription,
                    FilePath = item.fsource,
                    FileCustomer = item.fvalueText,
                    FileBrand = item.query1
                });
            }
            string zipName = "";
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                zip.AddDirectoryByName("Files");
                foreach (FileModel file in files)
                {
                    //if (file.IsSelected)
                    //{
                    //    zip.AddFile(file.FilePath, "Files");
                    //}                    
                    if (System.IO.File.Exists(Server.MapPath(file.FilePath)))
                    {
                        try
                        {
                            zip.AddFile(Server.MapPath(file.FilePath), "Files");
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);

                        }
                    }


                }
                zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd"));
    
                    var filePathOriginal = Server.MapPath("/Reportes/excel");
                    var path2 = "";
                    path2 = Path.Combine(filePathOriginal, zipName);
                    zip.Save(path2);

                
            }

            return Json(zipName, JsonRequestBehavior.AllowGet);

        }
    }
}