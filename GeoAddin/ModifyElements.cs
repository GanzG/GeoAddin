using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoAddin
{
    [Transaction(TransactionMode.Manual)]
    internal class ModifyElements : IExternalCommand
    {
        static UIApplication uiapp;
        static UIDocument uidoc;
        //static RevitApplication app;
        static Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            //app = uiapp.Application;
            doc = uidoc.Document;

            return Result.Succeeded;
        }

        public void deleteElement(List<ElementId> listId)
        {
            using (Transaction t = new Transaction(doc, "Удаление элементов"))
            {
                t.Start();
                doc.Delete(listId);
                t.Commit();
            }
        }


    }
}
