using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data.Model.APIWeb
{
    public class JsonResultsModel
    {
        public int Status { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}