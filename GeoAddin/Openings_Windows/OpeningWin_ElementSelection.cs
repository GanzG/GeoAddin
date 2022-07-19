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

            FilteredElementCollector docCollector = new FilteredElementCollector(doc);
            elementsInView = (List<Element>)docCollector.ToElements();



            foreach (var element in elementsInView)
            {
                for (int i = 1; i <= 9; i++)
                    try
                    {
                        if (i >= 2)
                        {
                            if ((CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains("Все элементы") == false) (CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add("Все элементы");
                            if ((CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains(element.Category.Name.ToString()) == false)
                                (CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add(element.Category.Name.ToString());
                        }
                            
                    
                        if (i <=5)
                        {
                            //(ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add(element.GetParameters());

                            //foreach (Parameter param in element.GetType())
                            //{
                            //switch (param.StorageType)
                            //{
                            //    case StorageType.Double:

                            //        break;
                            //    default:

                            //        break;
                            //}

                            IList<Parameter> param = element.GetOrderedParameters();
                            foreach (Parameter p in param)
                            {
                                if ((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains(p.Definition.Name) == false) (ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add(p.Definition.Name);
                            }
                            //for (int j = 0; j < param.Count; j++) 



                            //}

                        }
                    }
                    catch
                    {

                    }
            }

            //временный блок//
            //for (int i = 1; i <= 5; i++)
            //{
            //    (ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add(i);
            //}
            //конец временного блока//

        }

        public void deformationControl()
        {
            CatGroup.Height = 27 * (count - 1) + 46;
            ParamGroup.Height = 110 + ruleCount*27;
            relLabel.Location = new System.Drawing.Point(relLabel.Location.X, ParamGroup.Height-37);
            relationship_ComBox.Location = new System.Drawing.Point(relationship_ComBox.Location.X, ParamGroup.Height - 37);
            System.Drawing.Point location;
            if (CatGroup.Height > ParamGroup.Height) location = new System.Drawing.Point(CatGroup.Location.X, CatGroup.Location.Y + CatGroup.Height);
            else location = new System.Drawing.Point(CatGroup.Location.X, ParamGroup.Location.Y + ParamGroup.Height);
            mainGroup.Location = location;
            this.Height = mainGroup.Location.Y + mainGroup.Height + 56;
        }

        private void addDelControl(string action, string obj)
        {
            var GrBox = new System.Windows.Forms.GroupBox();

            //******** Здесь происходит максимальная унификация модуля под оба groupbox **********//
            int i = 1;
            string oldObj_name = "";
            string obj_name = "";
            if (obj == "cat")
            {
                GrBox = CatGroup;
                if (count <= 8) i = count + 1; else i = 9;
                oldObj_name = obj + "_" + count + "_ComBox";
                obj_name = obj + "_" + i + "_ComBox";
            }
            if (obj == "param")
            {
                GrBox = ParamGroup;
                if (ruleCount <= 4) i = ruleCount + 1; else i = 5;
                oldObj_name = obj + "_" + ruleCount + "_ComBox";
                obj_name = obj + "_" + i + "_ComBox";
            }

            //дополнительные наименования для блока правил//
            string oldRule_name = "rule_" + ruleCount + "_ComBox";
            string rule_name = "rule_" + i + "_ComBox";
            string oldRuleValue_name = "ruleValue_" + ruleCount + "_tb";
            string ruleValue_name = "ruleValue_" + i + "_tb";
            //************************************************************************************//

            System.Drawing.Point oldLoc = (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Location;

            if (action == "add")
            {
                if (((obj == "cat" && count <= 8) || (obj == "param" && ruleCount <=4)) && (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Text != "" && (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Text != "Все элементы")
                {
                    if (obj == "cat") count++;
                    if (obj == "param") ruleCount++;

                    (GrBox.Controls[obj_name] as System.Windows.Forms.ComboBox).Location = new System.Drawing.Point(oldLoc.X, oldLoc.Y + 27);

                    //индивидуальные элементы paramgroup//
                    if (obj == "param")
                    {
                        (GrBox.Controls[rule_name] as System.Windows.Forms.ComboBox).Location = new System.Drawing.Point((GrBox.Controls[oldRule_name] as System.Windows.Forms.ComboBox).Location.X, (GrBox.Controls[oldRule_name] as System.Windows.Forms.ComboBox).Location.Y + 27);
                        (GrBox.Controls[ruleValue_name] as System.Windows.Forms.TextBox).Location = new System.Drawing.Point((GrBox.Controls[oldRuleValue_name] as System.Windows.Forms.TextBox).Location.X, (GrBox.Controls[oldRuleValue_name] as System.Windows.Forms.TextBox).Location.Y + 28);
                    }
                    //*********************************//

                        deformationControl();
                }
            }

            if (action == "del")
            {
                if ((obj == "cat" && count >= 3) || (obj == "param" && ruleCount >=2))
                {
                    (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Text = "";
                    (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Location = new System.Drawing.Point((GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Location.X+400, (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Location.Y);

                    //индивидуальные элементы paramgroup//
                    if (obj == "param")
                    {
                        (GrBox.Controls[oldRule_name] as System.Windows.Forms.ComboBox).Location = new System.Drawing.Point((GrBox.Controls[oldRule_name] as System.Windows.Forms.ComboBox).Location.X+400, (GrBox.Controls[oldRule_name] as System.Windows.Forms.ComboBox).Location.Y);
                        (GrBox.Controls[oldRuleValue_name] as System.Windows.Forms.TextBox).Location = new System.Drawing.Point((GrBox.Controls[oldRuleValue_name] as System.Windows.Forms.TextBox).Location.X+400, (GrBox.Controls[oldRuleValue_name] as System.Windows.Forms.TextBox).Location.Y);
                    }
                    //*********************************//

                    if (obj == "cat") count--;
                    if (obj == "param") ruleCount--;

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
