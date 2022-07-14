#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using System.Windows;
#endregion

namespace GeoAddin
{
    [Transaction(TransactionMode.Manual)]
    public class DetachFile: IExternalCommand
    {

        static UIApplication uiapp;
        static UIDocument uidoc;
        static RevitApplication app;
        static Document doc;
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;
            DetachFileWindow win = new DetachFileWindow();
            win.ShowDialog();
            bool clickedon = win.clickedon;
            bool clickedoff = win.clickedoff;
            while (clickedon == false & clickedoff == false)
            {
                continue;
            }
            if (clickedon == true)
            {
                //Синхронизация файла
                TransactWithCentralOptions toptions = new TransactWithCentralOptions();
                SynchronizeWithCentralOptions soptions = new SynchronizeWithCentralOptions();
                soptions.SaveLocalBefore = true;
                soptions.SaveLocalAfter = true;
                soptions.Comment = win.comment;
                doc.SynchronizeWithCentral(toptions, soptions);

                //Сохранение файла
                string activedoccentralpath = ModelPathUtils.ConvertModelPathToUserVisiblePath(doc.GetWorksharingCentralModelPath());
                string activedocpath = doc.PathName;
                string sharedpath = activedoccentralpath.Replace("02.PROJECT", "03.SHARED");
                string sharedoctitlepath = sharedpath.Replace(".rvt", "_Отсоединено.rvt");
                string detachdocpath = sharedoctitlepath;
                SaveAsOptions saveoptions = new SaveAsOptions();
                WorksharingSaveAsOptions wsoptions = new WorksharingSaveAsOptions();
                wsoptions.SaveAsCentral = true;
                wsoptions.OpenWorksetsDefault = SimpleWorksetConfiguration.LastViewed;
                saveoptions.SetWorksharingOptions(wsoptions);
                saveoptions.OverwriteExistingFile = true;
                doc.SaveAs(detachdocpath, saveoptions);
                Document detachdoc = uidoc.Document;

                //Удаление неиспользуемых элементов в отсоединенном документе
                var methods = new List<MethodInfo>
                {
                    detachdoc.GetType().GetMethod("GetUnusedAppearances", BindingFlags.NonPublic | BindingFlags.Instance),
                    detachdoc.GetType().GetMethod("GetUnusedMaterials", BindingFlags.NonPublic | BindingFlags.Instance),
                    detachdoc.GetType().GetMethod("GetUnusedFamilies", BindingFlags.NonPublic | BindingFlags.Instance),
                    detachdoc.GetType().GetMethod("GetUnusedImportCategories", BindingFlags.NonPublic | BindingFlags.Instance),
                    detachdoc.GetType().GetMethod("GetUnusedStructures", BindingFlags.NonPublic | BindingFlags.Instance),
                    detachdoc.GetType().GetMethod("GetUnusedSymbols", BindingFlags.NonPublic | BindingFlags.Instance),
                    detachdoc.GetType().GetMethod("GetUnusedThermals", BindingFlags.NonPublic | BindingFlags.Instance),
                    detachdoc.GetType().GetMethod("GetNonDeletableUnusedElements", BindingFlags.NonPublic | BindingFlags.Instance)
                };
        
                var num = 0;
                var tryCount = 0;
                while (true)
                {
                    tryCount++;

                    if (tryCount >= 5)
                        break;

                    var hashSet = new HashSet<ElementId>();

                    foreach (var methodInfo in methods)
                    {
                        if (methodInfo?.Invoke(detachdoc, null) is ICollection<ElementId> c)
                        {
                            foreach (var id in c)
                                hashSet.Add(id);
                        }
                    }

                    if (hashSet.Count != num && hashSet.Count != 0)
                    {
                        num += hashSet.Count;
                        using (var tr = new Transaction(detachdoc, "purge unused"))
                        {
                            tr.Start();
                            foreach (var elementId in hashSet)
                            {
                                try
                                {
                                    detachdoc.Delete(elementId);
                                }
                                catch
                                {
                                    num--;
                                }
                            }

                            tr.Commit();
                        }

                        continue;
                    }

                    break;
                }


            //Открытие локального файла и закрытие сохраненной отсоединенной копии
                OpenOptions openoptions = new OpenOptions();
                FilePath filePath = new FilePath(activedocpath);
                uiapp.OpenAndActivateDocument(filePath, openoptions, false);

                detachdoc.Close();





            }
            return Result.Succeeded;
        }
        
    }
}
