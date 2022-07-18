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
            System.Drawing.Image add, delete, dialog;
            add = new Bitmap((System.Drawing.Image)Properties.Resources.add ,28,28);
            delete = new Bitmap((System.Drawing.Image)Properties.Resources.delete, 28, 28);
            dialog = new Bitmap((System.Drawing.Image)Properties.Resources.dialog, 16, 16);
            
            add_bt.BackgroundImage = add;
            delete_bt.BackgroundImage = delete;

            FilteredElementCollector docCollector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            elementsInView = (List<Element>)docCollector.ToElements();

            

            foreach (var element in elementsInView)
            {
                for (int i = 2; i<=9; i++) 
                    try
                    {
                        if ((CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains("Все элементы") == false) (CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add("Все элементы");
                            if ((CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains(element.Category.Name.ToString()) == false) 
                            (CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add(element.Category.Name.ToString());   
                    }
                    catch
                    {

                    }
                //System.Windows.Forms.MessageBox.Show(element.Category.Name.ToString());
            }

            Task.Factory.StartNew(deformationControl);

        }

        public void deformationControl()
        {
            while (threadState)
            {
                //System.Windows.Forms.MessageBox.Show("");
                //Thread.Sleep(500);
            }
        }

        private void close_bt_Click(object sender, EventArgs e)
        {
            threadState = false;
            this.Close();
        }

        private void add_bt_Click(object sender, EventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(Convert.ToString(count));
            if (count <= 8 && (CatGroup.Controls["cat_" + count + "_ComBox"] as System.Windows.Forms.ComboBox).Text != "" && (CatGroup.Controls["cat_" + count + "_ComBox"] as System.Windows.Forms.ComboBox).Text != "Все элементы")
            {
                System.Drawing.Point oldLoc = (CatGroup.Controls["cat_" + count + "_ComBox"] as System.Windows.Forms.ComboBox).Location;
                count++;

                CatGroup.Height = 27 * (count - 1) + 46;

                string cb_name = "cat_" + count + "_ComBox";
                (CatGroup.Controls[cb_name] as System.Windows.Forms.ComboBox).Location = new System.Drawing.Point(oldLoc.X, oldLoc.Y + 27);

                if (mainGroup.Location.Y - (CatGroup.Location.Y + CatGroup.Height) <= 17)
                {
                    mainGroup.Location = new System.Drawing.Point(mainGroup.Location.X, mainGroup.Location.Y + (17 - (mainGroup.Location.Y - (CatGroup.Location.Y + CatGroup.Height))));
                    this.Height = mainGroup.Location.Y + mainGroup.Height + 56;
                }
            }
        }

        private void delete_bt_Click(object sender, EventArgs e)
        {

        }

        private void result_bt_Click(object sender, EventArgs e)
        {

        }
    }
}
