using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comerciamarketing_webapp.Models
{
    public class FileModel
    {
        public int id { get; set; }
        public string FileActivity { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileCustomer { get; set; }
        public string FileBrand { get; set; }
        public string FileRep { get; set; }
        public string FileSection { get; set; }
        public string FileStore { get; set; }
        public string FileIDREP { get; set; }
    }
}