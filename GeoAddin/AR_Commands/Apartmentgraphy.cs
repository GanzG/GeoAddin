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
    public class Apartmentgraphy : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            RevitApplication app = uiapp.Application;
            Document doc = uidoc.Document;

            ApartmentgraphyWindow win = new ApartmentgraphyWindow();
            win.ShowDialog();
            bool clickedon = win.clickedon;
            bool clickedoff = win.clickedoff;
            while (clickedon == false & clickedoff == false)
            {
                continue;
            }
            if (clickedon == true)
            {
                FilteredElementCollector areas = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();
                Dictionary<string, List<Room>> apartments = new Dictionary<string, List<Room>>();

                int roundNum = win.roundNum;
                double loggieAreaCoef = win.loggiacoef;
                double balconyAreaCoef = win.balconycoef;

                foreach (Room room in areas)
                {
                    if (room.LookupParameter("ADSK_����").AsString() != null)
                    {
                        if (!apartments.ContainsKey(room.LookupParameter("ADSK_����").AsString()))
                        {
                            List<Room> rooms = new List<Room>() { room };
                            apartments.Add(room.LookupParameter("ADSK_����").AsString(), rooms);
                        }
                        else
                        {
                            apartments[room.LookupParameter("ADSK_����").AsString()].Add(room);
                        }

                    }
                }
                using (Transaction t = new Transaction(doc, "��������������"))
                {
                    t.Start();
                    foreach (string num in apartments.Keys)
                    {
                        Room biggestRoom = apartments[num][0]; // ����� ������� �������

                        int numberOfLivingRooms = 0;
                        double apartmentAreaLivingRooms = 0;            // ADSK_������� �������� �����
                        double apartmaneAreaWithoutSummerRooms = 0;     // ADSK_������� �������� � ��
                        double apartmentAreaGeneral = 0;                // ADSK_������� �������� �����
                        double apartmentAreaGeneralWithoutCoef = 0;     // TRGR_������� �������� ��� ��
                        foreach (Room room in apartments[num])
                        {
                            double biggestArea = Math.Round(UnitUtils.ConvertFromInternalUnits(biggestRoom.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble(), UnitTypeId.SquareMeters), roundNum);
                            double areaOfRoom = Math.Round(UnitUtils.ConvertFromInternalUnits(room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble(), UnitTypeId.SquareMeters), roundNum);
                            if (biggestArea < areaOfRoom) // ���������� ������ ��������� ���������
                            {
                                biggestArea = areaOfRoom;
                                biggestRoom = room;
                            }
                            try
                            {
                                room.LookupParameter("RMs_�������").Set(UnitUtils.ConvertToInternalUnits(Math.Round(areaOfRoom, roundNum), UnitTypeId.SquareMeters));
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.ToString());
                            }
                            try
                            {
                                double coefficent = 1.0;



                                if (room.LookupParameter("���").AsString() != "������" && room.LookupParameter("���").AsString() != "������")

                                {
                                    apartmaneAreaWithoutSummerRooms += areaOfRoom;
                                    apartmentAreaGeneral += areaOfRoom;
                                    room.LookupParameter("RM_����������� �������").Set(coefficent);
                                }
                                if (room.LookupParameter("���").AsString() == "������" || room.LookupParameter("���").AsString() == "������")
                                {
                                    if (room.LookupParameter("���").AsString() == "������")
                                    {
                                        room.LookupParameter("RM_����������� �������").Set(loggieAreaCoef);
                                    }
                                    else if (room.LookupParameter("���").AsString() == "������")
                                    {
                                        room.LookupParameter("RM_����������� �������").Set(balconyAreaCoef);
                                    }
                                    apartmentAreaGeneral += Math.Round(areaOfRoom * room.LookupParameter("RM_����������� �������").AsDouble(), roundNum);
                                }
                                if (room.LookupParameter("���").AsString() == "����� �������" || room.LookupParameter("���").AsString() == "��������" || room.LookupParameter("���").AsString() == "�������")
                                {
                                    numberOfLivingRooms++;
                                    apartmentAreaLivingRooms += areaOfRoom;
                                }
                                room.LookupParameter("RMs_������� � ��").Set(UnitUtils.ConvertToInternalUnits(Math.Round(areaOfRoom * room.LookupParameter("RM_����������� �������").AsDouble(), roundNum), UnitTypeId.SquareMeters));
                                apartmentAreaGeneralWithoutCoef += areaOfRoom;
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.ToString());
                            }
                        }

                        apartmaneAreaWithoutSummerRooms = UnitUtils.ConvertToInternalUnits(Math.Round(apartmaneAreaWithoutSummerRooms, roundNum), UnitTypeId.SquareMeters);
                        apartmentAreaLivingRooms = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaLivingRooms, roundNum), UnitTypeId.SquareMeters);
                        apartmentAreaGeneral = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaGeneral, roundNum), UnitTypeId.SquareMeters);
                        apartmentAreaGeneralWithoutCoef = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaGeneralWithoutCoef, roundNum), UnitTypeId.SquareMeters);
                        foreach (Room room in apartments[num])
                        {
                            try
                            {
                                room.LookupParameter("AP_���������� ������").Set(numberOfLivingRooms);
                                room.LookupParameter("APs_����� �������").Set(apartmaneAreaWithoutSummerRooms);
                                room.LookupParameter("APs_����� �������").Set(apartmentAreaLivingRooms);
                                room.LookupParameter("APs_����� ������� � ������ ���. ���.").Set(apartmentAreaGeneral);
                                room.LookupParameter("APs_����� ������� � ������ ���. ���. ��� ��").Set(apartmentAreaGeneralWithoutCoef);
                                if (room.LookupParameter("RM_����������� �������").AsDouble() < 1)
                                {
                                    CreateRoomTag(room, doc, 159750, "bottom", "right");
                                }
                                else
                                {
                                    CreateRoomTag(room, doc, 159742, "bottom", "right");
                                }
                                
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.ToString());
                            }
                        }
                        CreateRoomTag(biggestRoom, doc, 159393, "top", "right", 1, 1);
                    }
                    t.Commit();
                }

                return Result.Succeeded;
            }
            return Result.Succeeded;
        }
        
        private static void CreateRoomTag(Room room, Document doc, int tagType,
        string vert = "bottom", string horiz = "right",
        double padx = 0.0, double pady = 0.0)
        {
            List<RoomTag> tags = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_RoomTags)
                .WhereElementIsNotElementType()
                .Cast<RoomTag>()
                .Where(t
                 => t.Room.Id.IntegerValue == room.Id.IntegerValue
                 && t.GetTypeId().IntegerValue == tagType)
                .ToList(); // ����������� ������������� ���� ���������� ���� � �������
            if (tags.Count == 0) // ��� ���������� �������� ����, �������
            {
                double x = 0;
                double y = 0;
                SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
                options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;
                IList<IList<BoundarySegment>> boundarySegmentsList = room.GetBoundarySegments(options);
                foreach (IList<BoundarySegment> boundarySegments in boundarySegmentsList) // ���������� ����, � ������� ������ ��������� ����������� ���
                {
                    y = Math.Round(boundarySegments[0].GetCurve().GetEndPoint(0).Y, 8);
                    x = Math.Round(boundarySegments[0].GetCurve().GetEndPoint(0).X, 8);
                    foreach (BoundarySegment segment in boundarySegments)
                    {
                        XYZ startXYZ = segment.GetCurve().GetEndPoint(0);
                        XYZ endXYZ = segment.GetCurve().GetEndPoint(1);
                        if (vert == "bottom" && Math.Round(startXYZ.Y, 8) == Math.Round(endXYZ.Y, 8))
                        {
                            if (y >= Math.Round(startXYZ.Y, 8))
                            {
                                y = Math.Round(startXYZ.Y, 8);
                                if (horiz == "right")
                                {
                                    x = Math.Round(startXYZ.X, 8) > Math.Round(endXYZ.X, 8) ? Math.Round(startXYZ.X, 8) : Math.Round(endXYZ.X, 8);
                                }
                                else if (horiz == "left")
                                {
                                    x = Math.Round(startXYZ.X, 8) < Math.Round(endXYZ.X, 8) ? Math.Round(startXYZ.X, 8) : Math.Round(endXYZ.X, 8);
                                }
                            }
                        }
                        else if (vert == "top" && Math.Round(startXYZ.Y, 8) == Math.Round(endXYZ.Y, 8))
                        {
                            if (y <= Math.Round(startXYZ.Y, 8))
                            {
                                y = Math.Round(startXYZ.Y, 8);
                                if (horiz == "right")
                                {
                                    x = Math.Round(startXYZ.X, 8) > Math.Round(endXYZ.X, 8) ? Math.Round(startXYZ.X, 8) : Math.Round(endXYZ.X, 8);
                                }
                                else if (horiz == "left")
                                {
                                    x = Math.Round(startXYZ.X, 8) < Math.Round(endXYZ.X, 8) ? Math.Round(startXYZ.X, 8) : Math.Round(endXYZ.X, 8);
                                }
                            }
                        }
                    }
                }


                RoomTag tag = doc.Create.NewRoomTag(new LinkElementId(room.Id), new UV(x, y), null); // �������� ����
                tag.ChangeTypeId(new ElementId(tagType)); // ����� ���� ����

                double a = 0;
                double b = 0;
                BoundingBoxXYZ size = tag.get_BoundingBox(tag.View);
                if (horiz == "right")
                {
                    a = size.Min.X - size.Max.X - padx;
                }
                else if (horiz == "left")
                {
                    a = size.Max.X - size.Min.X + padx;
                }
                if (vert == "top")
                {
                    b = size.Min.Y - size.Max.Y - pady;
                }
                else if (vert == "bottom")
                {
                    b = size.Max.Y - size.Min.Y + pady;
                }
                tag.Location.Move(new XYZ(a / 2.0, b / 2.0, 0)); // �������� ���� �� ���� � ����������� �� ������� � ��������������



                Debug.Print("Complited the task.");

            }
        }
    }
}

