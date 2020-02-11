using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using comerciamarketing_webapp.Models;
using System.Web.Mvc;

namespace comerciamarketing_webapp.Controllers
{
    public class SurveysController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();
        // GET: Surveys
        public ActionResult Form1()
        {
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }


        public JsonResult Save_activity(string nombre,string edad, string genero, string q1, string q2, string q3, string q4, string q5, string q6, string q7, string q8, string q9, string q10, string q11, string q12, string q13,
            string q14, string q15, string q16, string q17, string q18, string a1, string a2, string a3, string a4, string a5, string a6, string a7, string a8
            , string a9, string a10, string a11, string a12, string a13, string a14, string a15, string a16, string a17, string a18)
        {
       
            try
            {
                //Guardamos survey
                Tb_Surveys newsurvey = new Tb_Surveys();
                newsurvey.Name = nombre;
                newsurvey.Age = edad;
                newsurvey.Gender = genero;
                newsurvey.Email = "";
                newsurvey.Telephone = "";
                newsurvey.Country = "";
                newsurvey.State = "";
                newsurvey.Address = "";
                newsurvey.City = "";
                newsurvey.ZipCode = "";
                newsurvey.Question1 = q1;
                newsurvey.Question2 = q2;
                newsurvey.Question3 = q3;
                newsurvey.Question4 = q4;
                newsurvey.Question5 = q5;
                newsurvey.Question6 = q6;
                newsurvey.Question7 = q7;
                newsurvey.Question8 = q8;
                newsurvey.Question9 = q9;
                newsurvey.Question10 = q10;
                newsurvey.Question11 = q11;
                newsurvey.Question12 = q12;
                newsurvey.Question13 = q13;
                newsurvey.Question14 = q14;
                newsurvey.Question15 = q15;
                newsurvey.Question16 = q16;
                newsurvey.Question17 = q17;
                newsurvey.Question18 = q18;

                newsurvey.Answer1 = a1;
                newsurvey.Answer2 = a2;
                newsurvey.Answer3 = a3;
                newsurvey.Answer4 = a4;
                newsurvey.Answer5 = a5;
                newsurvey.Answer6 = a6;
                newsurvey.Answer7 = a7;
                newsurvey.Answer8 = a8;
                newsurvey.Answer9 = a9;
                newsurvey.Answer10 = a10;
                newsurvey.Answer11 = a11;
                newsurvey.Answer12 = a12;
                newsurvey.Answer13 = a13;
                newsurvey.Answer14 = a14;
                newsurvey.Answer15 = a15;
                newsurvey.Answer16 = a16;
                newsurvey.Answer17 = a17;
                newsurvey.Answer18 = a18;
                newsurvey.Brand = "MILL CREEK Brewing Company";
                newsurvey.Date = DateTime.UtcNow;
                db.Tb_Surveys.Add(newsurvey);
                db.SaveChanges();



                return Json(new { Result = "Success" });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Warning" + ex.Message });
            }


        }
    }
}