using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;


namespace GeoAddin
{
    internal class ParamNameComparer : IComparer<Parameter>
    {
        public int Compare(Parameter x, Parameter y)
        {
            if (x.Definition.Name == y.Definition.Name)
                return 0;
            else return 1;
        }
    }
}
