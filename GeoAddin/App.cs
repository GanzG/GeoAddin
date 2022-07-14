#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endregion

namespace GeoAddin
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            string tabName = "ООО Георекон";

            string archpanelName = "Архитектура";
            a.CreateRibbonTab(tabName);
            var archpanel = a.CreateRibbonPanel(tabName,archpanelName);

            string commonpanelName = "Общее";
            var commonpanel = a.CreateRibbonPanel(tabName, commonpanelName);

            string openingpanelName = "Отверстия";
            var openingpanel = a.CreateRibbonPanel(tabName, openingpanelName);

            string sortingName = "Сортировка";
            var sortingpanel = a.CreateRibbonPanel(tabName, sortingName);

            //Создание кнопки генерациии помещений
            var ApartGenButton = new PushButtonData("Генерация квартир", "Генерация квартир", Assembly.GetExecutingAssembly().Location, "GeoAddin.RoomGenerating");
            var ApartGenPushBtn = archpanel.AddItem(ApartGenButton) as PushButton;
            Image RoomGenPic = Properties.Resources.RoomGenPic;
            ApartGenPushBtn.LargeImage = Convert(RoomGenPic, new Size(32, 32));
            ApartGenPushBtn.Image = Convert(RoomGenPic, new Size(16, 16));


            //Создание кнопки квартирографии
            var ApartmentgraphyButton = new PushButtonData("Квартирография", "Квартирография", Assembly.GetExecutingAssembly().Location, "GeoAddin.Apartmentgraphy");
            var ApartmentgraphyPushBtn = archpanel.AddItem(ApartmentgraphyButton) as PushButton;
            Image ApartmentgraphyButtonPic = Properties.Resources.ApartmentgraphyPic;
            ApartmentgraphyPushBtn.LargeImage = Convert(ApartmentgraphyButtonPic, new Size(32, 32));
            ApartmentgraphyPushBtn.Image = Convert(ApartmentgraphyButtonPic, new Size(16, 16));

            //Создание кнопки заполнения окон
            var WindowsFillingButton = new PushButtonData("Заполнение окон", "Заполнение окон", Assembly.GetExecutingAssembly().Location, "GeoAddin.WindowsFilling");
            var WindowsFillingPushBtn = archpanel.AddItem(WindowsFillingButton) as PushButton;
            Image WindowsFillingButtonPic = Properties.Resources.WindowFilling;
            WindowsFillingPushBtn.LargeImage = Convert(WindowsFillingButtonPic, new Size(32, 32));
            WindowsFillingPushBtn.Image = Convert(WindowsFillingButtonPic, new Size(16, 16));

            //Создание кнопки создания оконных схем
            var WindowsSchemaButton = new PushButtonData("Схема окон", "Схема окон", Assembly.GetExecutingAssembly().Location, "GeoAddin.WindowsSchema");
            var WindowsSchemaPushBtn = archpanel.AddItem(WindowsSchemaButton) as PushButton;
            Image WindowsSchemaButtonPic = Properties.Resources.WindowSchema;
            WindowsSchemaPushBtn.LargeImage = Convert(WindowsSchemaButtonPic, new Size(32, 32));
            WindowsSchemaPushBtn.Image = Convert(WindowsSchemaButtonPic, new Size(16, 16));

            //Создание кнопки отсоединения файла
            var DetachFileButton = new PushButtonData("Отсоединение файла", "Отсоединение файла", Assembly.GetExecutingAssembly().Location, "GeoAddin.DetachFile");
            var DetachFilePushBtn = commonpanel.AddItem(DetachFileButton) as PushButton;
            Image DetachFileButtonPic = Properties.Resources.DetachFilePic;
            DetachFilePushBtn.LargeImage =  Convert(DetachFileButtonPic, new Size(32, 32)) ;
            DetachFilePushBtn.Image = Convert(DetachFileButtonPic, new Size(16, 16));

            //Создание кнопки генерации отверстий в МЕР
            var OpeningGeneratingButton = new PushButtonData("Генерация отверстий", "Генерация отверстий", Assembly.GetExecutingAssembly().Location, "GeoAddin.OpeningGenerating");
            var OpeningGeneratingPushBtn = openingpanel.AddItem(OpeningGeneratingButton) as PushButton;
            Image OpeningGeneratingButtonPic = Properties.Resources.OpeningPic;
            OpeningGeneratingPushBtn.LargeImage = Convert(OpeningGeneratingButtonPic, new Size(32, 32));
            OpeningGeneratingPushBtn.Image = Convert(OpeningGeneratingButtonPic, new Size(16, 16));

            //Создание кнопки открытия селектора элементов
            var ElementSelectionButton = new PushButtonData("Селектор элементов", "Селектор элементов", Assembly.GetExecutingAssembly().Location, "GeoAddin.ElementSelection");
            var ElementSelectionPushBtn = sortingpanel.AddItem(ElementSelectionButton) as PushButton;
            Image ElementSelectionButtonPic = Properties.Resources.OpeningPic;
            ElementSelectionPushBtn.LargeImage = Convert(ElementSelectionButtonPic, new Size(32, 32));
            ElementSelectionPushBtn.Image = Convert(ElementSelectionButtonPic, new Size(16, 16));

            return Result.Succeeded;
        }
        //Метод для конвертации картинки
        public BitmapImage Convert (Image img, Size size)
        {   
            img = (Image)(new Bitmap(img, size));
            using (var memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;

            }
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
