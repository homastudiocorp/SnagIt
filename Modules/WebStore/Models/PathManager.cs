using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace WebStore.Models
{
    public class PathManager
    {
        public String GetPath(String rawValue)
        {
            return Regex.Replace(Regex.Replace(rawValue, @"[^0-9a-zA-Z\._]", "_"), "(_{2,})", "_");
        }
    }
}