using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dependency_Solution.Models
{
    public class Dependency
    {
        public string Input { get; set; }
    }

    public class DependencyInput
    {
        public List<Dependency> Inputs { get; set; }
    }
}