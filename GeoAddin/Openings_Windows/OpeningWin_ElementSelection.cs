using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RevitServices.Persistence;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;

namespace GeoAddin.Openings_Windows
{
    public partial class OpeningWin_ElementSelection : System.Windows.Forms.Form
    {
        public UIDocument uidoc;
        public Document doc;
        public UIApplication uiapp;
        public RevitApplication app;

        public bool threadState = true;
        public int count = 2; //не хочется переименовывать 8 combobox'ов из-за того, что удалил первый по необходимой нумерации
        public int ruleCount = 1;
        List<Element> elementsInView;
        public OpeningWin_ElementSelection(UIApplication uiapp)
        {
            InitializeComponent();
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;
        }

        private void OpeningWin_ElementSelection_Load(object sender, EventArgs e)
        {
            foreach (var ComBox in CatGroup.Controls.OfType<System.Windows.Forms.ComboBox>().Concat(ParamGroup.Controls.OfType<System.Windows.Forms.ComboBox>())) 
                ComBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; //я запрещаю вам ручной ввод

            //************************************************************************************//
            //System.Drawing.Image add, delete, dialog;
            //add = new Bitmap((System.Drawing.Image)Properties.Resources.add, 28, 28);
            //delete = new Bitmap((System.Drawing.Image)Properties.Resources.delete, 28, 28);
            //dialog = new Bitmap((System.Drawing.Image)Properties.Resources.dialog, 16, 16);

            //add_bt.BackgroundImage = add;
            //delete_bt.BackgroundImage = delete;
            //************************************************************************************//

            FilteredElementCollector docCollector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            elementsInView = (List<Element>)docCollector.ToElements();



            foreach (var element in elementsInView)
            {
                for (int i = 2; i <= 9; i++)
                    try
                    {
                        if ((CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains("Все элементы") == false) (CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add("Все элементы");
                        if ((CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains(element.Category.Name.ToString()) == false)
                            (CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add(element.Category.Name.ToString());
                    }
                    catch
                    {

                    }
            }

            //Task.Factory.StartNew(deformationControl);

        }

        public void deformationControl()
        {
            CatGroup.Height = 27 * (count - 1) + 46;
            ParamGroup.Height = 110 + ruleCount*27;
            System.Drawing.Point location;
            if (CatGroup.Height > ParamGroup.Height) location = new System.Drawing.Point(CatGroup.Location.X, CatGroup.Location.Y + CatGroup.Height);
            else location = new System.Drawing.Point(CatGroup.Location.X, ParamGroup.Location.Y + ParamGroup.Height);
            mainGroup.Location = location;
            this.Height = mainGroup.Location.Y + mainGroup.Height + 56;
        }

        private void addDelControl(string action, string obj)
        {
            var GrBox = new System.Windows.Forms.GroupBox();
            if (obj == "cat") GrBox = CatGroup;
            if (obj == "param") GrBox = CatGroup;

            int i = count + 1;
            string oldObj_name = obj + "_" + count + "_ComBox";
            string obj_name = obj + "_" + i + "_ComBox";


            System.Drawing.Point oldLoc = (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Location;

            if (action == "add")
            {
                if (count <= 8 && (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Text != "" && (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Text != "Все элементы")
                {
                    count++;
                    
                    GrBox.Height = 27 * (count - 1) + 46;
                    (GrBox.Controls[obj_name] as System.Windows.Forms.ComboBox).Location = new System.Drawing.Point(oldLoc.X, oldLoc.Y + 27);

                    //не забыть для параметров уточнить детали расширения

                    deformationControl();
                }
            }

            if (action == "del")
            {
                if (count >= 3)
                {
                    System.Drawing.Point hidePoint = new System.Drawing.Point((GrBox.Controls[obj_name] as System.Windows.Forms.ComboBox).Location.X + 400, (CatGroup.Controls[obj_name] as System.Windows.Forms.ComboBox).Location.Y);
                    (GrBox.Controls[obj_name] as System.Windows.Forms.ComboBox).Text = "";
                    (GrBox.Controls[obj_name] as System.Windows.Forms.ComboBox).Location = hidePoint;
                    count--;

                    deformationControl();
                }
            }


        }

        private void close_bt_Click(object sender, EventArgs e)
        {
            threadState = false;
            this.Close();
        }

        private void add_bt_Click(object sender, EventArgs e)
        {

            if (cat_rb.Checked) addDelControl("add", "cat");
            if (rule_rb.Checked) addDelControl("add", "param");

        }

        private void delete_bt_Click(object sender, EventArgs e)
        {
            if (cat_rb.Checked) addDelControl("del", "cat");
            if (rule_rb.Checked) addDelControl("del", "param");
            
        }


        private void result_bt_Click(object sender, EventArgs e)
        {

        }
    }
}
