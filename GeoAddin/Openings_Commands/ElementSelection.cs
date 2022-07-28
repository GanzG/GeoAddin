using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using RevitApplication = Autodesk.Revit.ApplicationServices.Application;


namespace GeoAddin
{
    [Transaction(TransactionMode.Manual)]
    public class ElementSelection : IExternalCommand
    {
        static UIApplication uiapp;
        static UIDocument uidoc;
        static RevitApplication app;
        static Document doc;
        ExternalEvent _exEvent;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;

            Openings_Windows.OpeningWin_ElementSelection win = new Openings_Windows.OpeningWin_ElementSelection(uiapp);


            Openings_Windows.EventRegHandler _exeventHander = new Openings_Windows.EventRegHandler();
            _exEvent = ExternalEvent.Create(_exeventHander);
            win.ExEvent = _exEvent;
            win.Show();

            return Result.Succeeded;
        }

    }
    }
