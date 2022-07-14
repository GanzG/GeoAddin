using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;



using Autodesk.Revit.UI.Selection;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using System.Windows;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;

namespace GeoAddin
{
    [Transaction(TransactionMode.Manual)]
    public class WindowsSchema : IExternalCommand
    {
        //Основные переменные

        static UIApplication uiapp;
        static UIDocument uidoc;
        static RevitApplication app;
        static Document doc;

        public  Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;
            IList<View> views = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).WhereElementIsNotElementType().Cast<View>().ToList();
            uiapp.ActiveUIDocument.ActiveView = GetWindowSchemaView(views);
            //Получение всех окон и видов в проекте, получение типоразмеров окон, используемых в проекте
            List<FamilyInstance> windowInstances = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();
            List<string> windowTypesNames = new HashSet<string>(windowInstances.Select(el => el.Symbol.Name)).ToList();
            List<FamilySymbol> allWindowTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsElementType().Cast<FamilySymbol>().ToList();
            List<FamilySymbol> activeWindowTypes = new List<FamilySymbol>();
            foreach (FamilySymbol windowType in allWindowTypes)
            {
                if ((windowType.IsActive) && (windowType.FamilyName.Contains("Окно"))) { activeWindowTypes.Add(windowType); }

            }
            List<FamilySymbol> windowTypes = new List<FamilySymbol>();
            
            foreach (FamilySymbol familySymbol in activeWindowTypes)
            {
                foreach (string windowTypesName in windowTypesNames)
                {
                    if (familySymbol.Name == windowTypesName) { windowTypes.Add(familySymbol);}
                }
            }
            int count = windowTypes.Count;
            //Получение легенду на виде и типизированный список его Id
            List<Element> makedLegendComponent = new FilteredElementCollector(doc, GetWindowSchemaView(views).Id).OfCategory(BuiltInCategory.OST_LegendComponents).ToList();
            List<Element> textNotes = new FilteredElementCollector(doc, GetWindowSchemaView(views).Id).OfCategory(BuiltInCategory.OST_TextNotes).ToList();
            List<Element> detailCurves = new FilteredElementCollector(doc, GetWindowSchemaView(views).Id).OfClass(typeof(CurveElement)).ToList();
            //Удаление компонентов и текстовых примечаний если они уже существуют на виде
            using (Transaction t = new Transaction(doc, "Удаление существующих компонентов"))
            {
                t.Start();
                if (textNotes != null) { foreach (TextNote texNote in textNotes) { doc.Delete(texNote.Id); } }
                if (detailCurves != null) { foreach (CurveElement detailCurve in detailCurves) { doc.Delete(detailCurve.Id); } }

                if (makedLegendComponent.Count > 1)
                {
                    for (int i = 1; i < makedLegendComponent.Count; i++)
                    {
                        doc.Delete(makedLegendComponent[i].Id);
                    }
                }
                t.Commit();
            }
            List<Element> legendComponent = new FilteredElementCollector(doc, GetWindowSchemaView(views).Id).OfCategory(BuiltInCategory.OST_LegendComponents).ToList();
            ICollection<ElementId> elementIds = legendComponent.Select(el => el.Id).ToList();

            double deltaX = 0;
            
            foreach (FamilySymbol familySymbol in windowTypes)
            {

                using (Transaction t = new Transaction(doc, "Копирование компонентов легенды"))
                {
                    t.Start();
                    //Group group = doc.Create.NewGroup(uidoc.Selection.GetElementIds());
                    Group group = doc.Create.NewGroup(elementIds);
                    LocationPoint location = group.Location as LocationPoint;
                    XYZ windowLocationPoint = new XYZ(location.Point.X + deltaX, location.Point.Y, location.Point.Z);
                    Group newGroup = doc.Create.PlaceGroup(windowLocationPoint, group.GroupType);
                    group.UngroupMembers();
                    newGroup.UngroupMembers();
                    t.Commit();

                }
                
                deltaX += 7;
            }
            using (Transaction t = new Transaction(doc, "Удаление лишней схемы"))
            {
                t.Start();
                doc.Delete(legendComponent[0].Id);
                t.Commit();
            }
           
            using (Transaction t = new Transaction(doc, "Сопоставление типов окон"))
            {
                t.Start();
                List<Element> newLegendComponents = new FilteredElementCollector(doc, GetWindowSchemaView(views).Id).OfCategory(BuiltInCategory.OST_LegendComponents).ToList();
                
                for (int i = 0; i < newLegendComponents.Count; i++)
                {
                    newLegendComponents[i].LookupParameter("Тип компонента").Set(windowTypes[i].Id);
                    
                }
                
                t.Commit();
            }
            using (Transaction t = new Transaction(doc, "Вставка текстовых марок"))
            {
                t.Start();
                List<Element> changedLegendComponents = new FilteredElementCollector(doc, GetWindowSchemaView(views).Id).OfCategory(BuiltInCategory.OST_LegendComponents).ToList();

                for (int i = 0; i < changedLegendComponents.Count; i++)
                {
                    
                    BoundingBoxXYZ bb = changedLegendComponents[i].get_BoundingBox(GetWindowSchemaView(views));
                    XYZ minLocation = bb.Min;
                    XYZ maxLocation = bb.Max;
                    XYZ markLocation = new XYZ((minLocation.X + maxLocation.X) / 2 - 0.25, minLocation.Y + windowTypes[i].LookupParameter("Примерная высота").AsDouble() + 1, 0);
                    XYZ squareLocation = new XYZ((minLocation.X + maxLocation.X) / 2 - 0.5, minLocation .Y + windowTypes[i].LookupParameter("Примерная высота").AsDouble() + 0.6, 0);

                    TextNote markTextNote = TextNote.Create(doc, GetWindowSchemaView(views).Id, markLocation, windowTypes[i].LookupParameter("ADSK_Марка").AsString(), new TextNoteOptions(new ElementId(8047)));
                    string windowSquare = "S = " + Math.Round(((UnitUtils.ConvertFromInternalUnits(windowTypes[i].LookupParameter("Примерная ширина").AsDouble(), UnitTypeId.Millimeters) * UnitUtils.ConvertFromInternalUnits(windowTypes[i].LookupParameter("Примерная высота").AsDouble(), UnitTypeId.Millimeters)) / 1000000), 2).ToString() + " м²";
                    TextNote squareTextNote = TextNote.Create(doc, GetWindowSchemaView(views).Id, squareLocation, windowSquare, new TextNoteOptions(new ElementId(8047)));
                }

                t.Commit();
            }
            using (Transaction t = new Transaction(doc, "Образмеривание"))
            {
                t.Start();
                List<Element> changedLegendComponents = new FilteredElementCollector(doc, GetWindowSchemaView(views).Id).OfCategory(BuiltInCategory.OST_LegendComponents).ToList();

                for (int i = 0; i < changedLegendComponents.Count; i++)
                {
                      
                    BoundingBoxXYZ bb = changedLegendComponents[i].get_BoundingBox(GetWindowSchemaView(views));

                    double higth = windowTypes[i].LookupParameter("Примерная высота").AsDouble();
                    double width = windowTypes[i].LookupParameter("Примерная ширина").AsDouble();
                    double minX = (bb.Min.X + bb.Max.X) / 2 - width / 2;
                    double minY = ((bb.Min.Y + bb.Max.Y) / 2 - higth / 2) + UnitUtils.ConvertToInternalUnits(15, UnitTypeId.Millimeters);

                    Line xline = Line.CreateBound(new XYZ(bb.Min.X, bb.Min.Y - 0.5, 0), new XYZ(bb.Max.X, bb.Min.Y - 0.5, 0));
                    Line refXline1 = Line.CreateBound(new XYZ(minX, minY, 0), new XYZ(minX, minY + higth, 0));
                    Line refXline2 = Line.CreateBound(new XYZ(minX + width, minY, 0), new XYZ(minX + width, minY + higth, 0));
                    Reference refXcurve1 = doc.Create.NewDetailCurve(GetWindowSchemaView(views), refXline1).GeometryCurve.Reference;
                    Reference refXcurve2 = doc.Create.NewDetailCurve(GetWindowSchemaView(views), refXline2).GeometryCurve.Reference;
                    ReferenceArray xArray = new ReferenceArray();
                    xArray.Append(refXcurve1);
                    xArray.Append(refXcurve2);

                    Line yline = Line.CreateBound(new XYZ(bb.Min.X - 0.5, bb.Min.Y, 0), new XYZ(bb.Min.X - 0.5, bb.Max.Y, 0));
                    Line refYline1 = Line.CreateBound(new XYZ(minX, minY, 0), new XYZ(minX + width, minY, 0));
                    Line refYline2 = Line.CreateBound(new XYZ(minX, minY + higth, 0), new XYZ(minX + width, minY + higth, 0));
                    Reference refYcurve1 = doc.Create.NewDetailCurve(GetWindowSchemaView(views), refYline1).GeometryCurve.Reference;
                    Reference refYcurve2 = doc.Create.NewDetailCurve(GetWindowSchemaView(views), refYline2).GeometryCurve.Reference;
                    ReferenceArray yArray = new ReferenceArray();
                    yArray.Append(refYcurve1);
                    yArray.Append(refYcurve2);

                    DimensionType dimensionType = (DimensionType)doc.GetElement(new ElementId(7354));

                    doc.Create.NewDimension(GetWindowSchemaView(views), xline, xArray, dimensionType);
                    doc.Create.NewDimension(GetWindowSchemaView(views), yline, yArray, dimensionType);

                }

                t.Commit();
            }

            return Result.Succeeded;
        }
        //Функция для нахождения легенды схем окон
        private View GetWindowSchemaView(IList<View> views)
        { 
            View view = null;
            foreach (View element in views)
            {
                if (element.Name == "Схемы окон") { view = element; }
            }
            return view;    
        }

       
    }
}
