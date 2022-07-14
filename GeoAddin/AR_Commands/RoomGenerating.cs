#region Namespaces
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

#endregion

namespace GeoAddin
{
    [Transaction(TransactionMode.Manual)]
    public class RoomGenerating : IExternalCommand
    {
        static UIApplication uiapp;
        static UIDocument uidoc;
        static RevitApplication app;
        static Document doc;
        Phase phase;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;
            RoomGenWindow win = new RoomGenWindow();
            win.ShowDialog();
            bool clickedon = win.clickedon;
            bool clickedoff = win.clickedoff;
            while (clickedon == false & clickedoff == false)
            {
                continue;
            }
            if (clickedon == true)
            {
                double upoffset = win.upoffset;
                double botoffset = win.botoffset;
                
                double loggieAreaCoef = 0.5;
                //double balconyAreaCoef = 0.3;
                double defaultAreaCoef = 1.0;
                double roomUp = 3000;
                double roomDown = 0;

                try
                {
                    
                     roomUp = upoffset;
                     roomDown = botoffset;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                
                IList<Element> roomsToRemove = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .WhereElementIsNotElementType()
                    .ToList();
                if (roomsToRemove.Count > 0)
                {
                    using (Transaction t = new Transaction(doc, "Удаление существующих помещений"))
                    { // Добвать проверку на наличие заполненых комнат
                        t.Start();
                        doc.Delete(roomsToRemove.Select(room => room.Id).ToList());
                        t.Commit();
                    }
                }

                List<Room> rooms = new List<Room>();
                phase = doc.Phases.get_Item(doc.Phases.Size - 1);
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Генерация помещений");

                    Level level = doc.ActiveView.GenLevel;

                    foreach (ElementId roomId in doc.Create.NewRooms2(level))
                    {
                        Room room = doc.GetElement(roomId) as Room;
                        rooms.Add(room);
                        room.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET).Set(UnitUtils.ConvertToInternalUnits(roomUp, UnitTypeId.Millimeters));
                        room.get_Parameter(BuiltInParameter.ROOM_LOWER_OFFSET).Set(UnitUtils.ConvertToInternalUnits(roomDown, UnitTypeId.Millimeters));
                    }
                    t.Commit();

                    t.Start("Смена тега помещений");
                    List<RoomTag> roomtags = new FilteredElementCollector(doc, doc.ActiveView.Id)
                        .OfCategory(BuiltInCategory.OST_RoomTags)
                        .WhereElementIsNotElementType()
                        .Cast<RoomTag>()
                        .ToList();
                    foreach (RoomTag roomTag in roomtags)
                    {
                        roomTag.ChangeTypeId(new ElementId(159738));
                    }
                    t.Commit();
                }

                List<Room> smallRooms = rooms.Where(room => UnitUtils.ConvertFromInternalUnits(room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble(), UnitTypeId.SquareMeters) <= 1)
                    .ToList();
                rooms = rooms.Where(room => UnitUtils.ConvertFromInternalUnits(room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble(), UnitTypeId.SquareMeters) > 1).ToList();
                if (smallRooms.Count > 0)
                {
                    using (Transaction t = new Transaction(doc, "Удаление малых помещений"))
                    {
                        t.Start();
                        doc.Delete(smallRooms.Select(room => room.Id).ToList());
                        t.Commit();
                    }
                }

                FilteredElementCollector plumbingFixtures = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_PlumbingFixtures).WhereElementIsNotElementType();

                List<FamilyInstance> kitchenPlumbs = new List<FamilyInstance>();
                List<FamilyInstance> toiletBowls = new List<FamilyInstance>();
                List<FamilyInstance> sinks = new List<FamilyInstance>();
                List<FamilyInstance> washers = new List<FamilyInstance>();
                List<FamilyInstance> baths = new List<FamilyInstance>();

                foreach (FamilyInstance fixture in plumbingFixtures)
                {
                    string fixtureName = fixture.Symbol.FamilyName;
                    if (fixtureName.Contains("Варочная_панель"))
                    {
                        kitchenPlumbs.Add(fixture);
                    }
                    else if (fixtureName.Contains("Унитаз"))
                    {
                        toiletBowls.Add(fixture);
                    }
                    else if (fixtureName.Contains("Умывальник"))
                    {
                        sinks.Add(fixture);
                    }
                    else if (fixtureName.Contains("Стиральная_машина"))
                    {
                        washers.Add(fixture);
                    }
                    else if (fixtureName.Contains("Ванная") || fixtureName.Contains("ДушевойПоддон"))
                    {
                        baths.Add(fixture);
                    }
                }

                FilteredElementCollector windows = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsNotElementType();
                FilteredElementCollector doors = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType();

                using (Transaction t = new Transaction(doc, "Определение помещений"))
                {
                    t.Start();
                    foreach (Room room in rooms)
                    {
                        List<string> roomFixtures = new List<string>();
                        foreach (FamilyInstance fixture in kitchenPlumbs)
                        {
                            XYZ located = (fixture.Location as LocationPoint).Point;
                            if (room.IsPointInRoom(located))
                            {
                                roomFixtures.Add("Кухня");
                            }
                        }
                        foreach (FamilyInstance fixture in toiletBowls)
                        {
                            if (room.IsPointInRoom((fixture.Location as LocationPoint).Point))
                            {
                                roomFixtures.Add("Унитаз");
                            }
                        }
                        foreach (FamilyInstance fixture in sinks)
                        {
                            if (room.IsPointInRoom((fixture.Location as LocationPoint).Point))
                            {
                                roomFixtures.Add("Умывальник");
                            }
                        }
                        foreach (FamilyInstance fixture in washers)
                        {
                            if (room.IsPointInRoom((fixture.Location as LocationPoint).Point))
                            {
                                roomFixtures.Add("Стиральная машина");
                            }
                        }
                        foreach (FamilyInstance fixture in baths)
                        {
                            if (room.IsPointInRoom((fixture.Location as LocationPoint).Point))
                            {
                                roomFixtures.Add("Ванна");
                            }
                        }

                        if (roomFixtures.Contains("Унитаз") && roomFixtures.Contains("Ванна"))
                        {
                            room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("С.У.");
                            
                        }
                        else if (!roomFixtures.Contains("Ванна") && roomFixtures.Contains("Умывальник") && roomFixtures.Contains("Унитаз"))
                        {
                            room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("Уборная");
                            
                        }
                        else if (!roomFixtures.Contains("Унитаз") && roomFixtures.Contains("Ванна"))
                        {
                            room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("Ванная");
                            
                        }
                        else if (!roomFixtures.Contains("Ванна") && !roomFixtures.Contains("Умывальник") && !roomFixtures.Contains("Унитаз") && roomFixtures.Contains("Стиральная машина"))
                        {
                            room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("Постирочная");
                            
                        }
                        else if (roomFixtures.Contains("Кухня"))
                        {
                            room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("Кухня");
                            
                        }

                        List<int> roomWindowsIds = new List<int>();
                        foreach (FamilyInstance window in windows)
                        {
                            Room windowToRoom = window.get_ToRoom(phase);
                            if (windowToRoom != null)
                            {
                                roomWindowsIds.Add(windowToRoom.Id.IntegerValue);
                            }
                        }
                        foreach (int elementId in roomWindowsIds.Distinct())
                        {
                            if (room.Id.IntegerValue == elementId && room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString().Contains("Помещение"))
                            {
                                room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("Жилая комната");
                                
                            }
                        }
                        if (room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString() == "Помещение")
                        {
                            room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("Коридор");
                            
                        }
                        room.LookupParameter("RM_Коэффициент площади").Set(defaultAreaCoef);
                    }
                    foreach (FamilyInstance door in doors)
                    {
                        Room doorFromRoom = door.get_FromRoom(phase);
                        if (doorFromRoom != null && door.Symbol.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).AsString().Contains("ПВХ"))
                        {
                            doorFromRoom.get_Parameter(BuiltInParameter.ROOM_NAME).Set("Лоджия");
                            
                            doorFromRoom.LookupParameter("RM_Коэффициент площади").Set(loggieAreaCoef);
                        }
                    }
                    t.Commit();
                }

                IList<FamilyInstance> entryDoors = new FilteredElementCollector(doc, doc.ActiveView.Id) // Находим входные двери квартиры
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .Where(door => door.Symbol.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).AsString().Contains("квартирная")) // Здесь и применяется тяжелый фильтр для поиска входных дверей
                    .ToList();
                FilteredElementCollector allRooms = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();

                using (Transaction t = new Transaction(doc, "Нумерация комнат квартир")) // Основная транзакция
                {
                    t.Start();
                    foreach (FamilyInstance entryDoor in entryDoors) // Проходим по каждой входной двери
                    {
                        List<Room> apartmnetRooms = GetApartmentRooms(entryDoor.get_FromRoom(phase), allRooms, null, entryDoor); // Эта функция отвечает за нахождение всех комнат в картире
                        Level lvl = doc.GetElement(entryDoor.LevelId) as Level; // Часть, просто отвечающая за взятие номера квартиры, у нас по форме L01_001 с указанием уровня и номера квартиры
                        string doorNumber = entryDoor.LookupParameter("ADSK_Зона").AsString();
                        string apartmentNumber = null;
                        if (!doorNumber.Contains("L") && !doorNumber.Contains("_"))
                        {
                            apartmentNumber = $"L{lvl.Name.Replace("Этаж ", "")}_{LeadingZeros(entryDoor.LookupParameter("ADSK_Зона").AsString())}";
                        }
                        else
                        {
                            apartmentNumber = doorNumber;
                        }
                        foreach (Room room in apartmnetRooms)
                        {
                            try
                            {
                                room.LookupParameter("ADSK_Зона").Set(apartmentNumber);
                                room.LookupParameter("ADSK_Этаж").Set($"L{room.Level.Name.Replace("Этаж ", "")}");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Ошибка");
                            }
                        }
                        entryDoor.LookupParameter("ADSK_Зона").Set(apartmentNumber);
                    }
                    t.Commit();
                }

                List<Room> MOP = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .WhereElementIsNotElementType()
                    .Cast<Room>()
                    .Where(room => room.LookupParameter("ADSK_Зона").AsString() == null)
                    .ToList();

                using (Transaction t = new Transaction(doc, "Удаление МОП-ов"))
                {
                    t.Start();
                    doc.Delete(MOP.Select(room => room.Id).ToList());
                    t.Commit();
                }
                using (Transaction t = new Transaction(doc, "Добавление цветовой схемы"))
                {
                    t.Start();
                    doc.ActiveView.SetColorFillSchemeId(new ElementId(BuiltInCategory.OST_Rooms), new ElementId(15969));
                    t.Commit();
                }

                return Result.Succeeded;
            }
            return Result.Succeeded;
            /*
             * Эта чудо-функция находит все комнаты.
             */
        }
            private List<Room> GetApartmentRooms(Room currentRoom, FilteredElementCollector allRooms, List<Room> apartmentRooms = null, FamilyInstance entryDoor = null)
        {
                if (apartmentRooms == null)
                {
                    apartmentRooms = new List<Room>();
                }
                apartmentRooms.Add(currentRoom);
                List<int> roomsIds = apartmentRooms.Select(room => room.Id.IntegerValue).ToList();

                List<FamilyInstance> allDoorsOfRoom = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .Where(door =>
                    door.get_FromRoom(phase).Id.IntegerValue == currentRoom.Id.IntegerValue || door.get_ToRoom(phase).Id.IntegerValue == currentRoom.Id.IntegerValue)
                    .ToList();
                if (entryDoor != null)
                {
                    allDoorsOfRoom = allDoorsOfRoom.Where(door => door.Id.IntegerValue != entryDoor.Id.IntegerValue).ToList();
                }

                if (allDoorsOfRoom.Count > 0)
                {
                    foreach (FamilyInstance door in allDoorsOfRoom)
                    {
                        if (door.get_FromRoom(phase).Id.IntegerValue != currentRoom.Id.IntegerValue)
                        {
                            if (!roomsIds.Contains(door.get_FromRoom(phase).Id.IntegerValue))
                            {
                                apartmentRooms = GetApartmentRooms(door.get_FromRoom(phase), allRooms, apartmentRooms, door);
                            }
                        }
                        else
                        {
                            if (!roomsIds.Contains(door.get_ToRoom(phase).Id.IntegerValue))
                            {
                                apartmentRooms = GetApartmentRooms(door.get_ToRoom(phase), allRooms, apartmentRooms, door);
                            }
                        }
                    }
                }
                Solid currentRoomSolid = GetSolidOfRoom(currentRoom);
                List<Room> adjoiningRooms = allRooms.Cast<Room>()
                    .Where(room => SolidsAreToching(currentRoomSolid, GetSolidOfRoom(room)) && !roomsIds.Contains(room.Id.IntegerValue))
                    .ToList();
                if (adjoiningRooms.Count > 0)
                {
                    foreach (Room room in adjoiningRooms)
                    {
                        apartmentRooms = GetApartmentRooms(room, allRooms, apartmentRooms);
                    }
                }

                return apartmentRooms;
            }
            private static bool SolidsAreToching(Solid solid1, Solid solid2)
            {
                Solid interSolid = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);
                Solid unionSolid = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Union);

                double sumArea = Math.Round(Math.Abs(solid1.SurfaceArea + solid2.SurfaceArea), 5);
                double sumFaces = Math.Abs(solid1.Faces.Size + solid2.Faces.Size);
                double unionArea = Math.Round(Math.Abs(unionSolid.SurfaceArea), 5);
                double unionFaces = Math.Abs(unionSolid.Faces.Size);

                if (sumArea > unionArea && sumFaces > unionFaces && interSolid.Volume < 0.00001)
                {
                    return true;
                }
                return false;
            }
            private static Solid GetSolidOfRoom(Room room)
            {
                SpatialElementGeometryCalculator calculator = new SpatialElementGeometryCalculator(doc);
                SpatialElementGeometryResults results = calculator.CalculateSpatialElementGeometry(room);
                Solid roomSolid = results.GetGeometry();

                return roomSolid;
            }
            private static string LeadingZeros(string str)
            {
                if (str.Length == 1) { return "00" + str; }
                if (str.Length == 2) { return "0" + str; }
                if (str.Length == 3) { return str; }
                return null;
            }
        }
    }

