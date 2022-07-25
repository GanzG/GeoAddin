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
//using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using RevitServices.Persistence;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;
using System.IO;

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
        static List<Element> elementsInView;
        static List<Parameter> parameter;
        List<Element> roughSample = new List<Element>();

        public OpeningWin_ElementSelection(UIApplication uiapp)
        {
            InitializeComponent();
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;
        }

        private void OpeningWin_ElementSelection_Load(object sender, EventArgs e)
        {
            foreach (var ComBox in CatGroup.Controls.OfType<System.Windows.Forms.ComboBox>().Concat(ParamGroup.Controls.OfType<System.Windows.Forms.ComboBox>())) //объединяю элементы двух групп, т.к. действие для всех одно
                ComBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; //я запрещаю вам ручной ввод

            elementsInView = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .Where(el => el.LevelId.ToString() != "-1") //на данном этапе интересуют только экземпляры
                .Where(el => el.Category.CategoryType.ToString() == "Model")
                .ToList();
            //elementsInView = (List<Element>)docCollector.ToElements();



            //прикручиваю обработчик событий для всех комбобоксов из catgroup//
            for (int i = 2; i <= 8; i++) (CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).TextChanged += new System.EventHandler(selectControl);
            //**************************************************************//

            for (int i = 1; i <= 5; i++)
            {
                (ParamGroup.Controls["rule_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).TextChanged += new System.EventHandler(ruleSelectControl); //нужно в реалтайме мониторить соответствие условия типу параметра
                (ParamGroup.Controls["rule_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add("Равно");
                (ParamGroup.Controls["rule_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add("Больше");
                (ParamGroup.Controls["rule_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add("Меньше");
                (ParamGroup.Controls["rule_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Items.Add("Содержит"); //пока думаю над реализацией. Возможно, стоит убрать
            } 

            foreach (var element in elementsInView)
            {
                try
                {
                    if ((CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains("-Все элементы-") == false) (CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Items.Add("-Все элементы-");
                    if ((CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains(element.Category.Name.ToString()) == false)
                    {
                        (CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Items.Add(element.Category.Name.ToString());
                        //parameter = (List<Parameter>)element.GetOrderedParameters();
                    }
                }
                catch
                {

                }
            }

            (CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Sorted = true; //сортирую по алфавиту


        }



        public void deformationControl()
        {
            CatGroup.Height = 27 * (count - 1) + 46;
            ParamGroup.Height = 110 + ruleCount * 27;
            relLabel.Location = new System.Drawing.Point(relLabel.Location.X, ParamGroup.Height - 37);
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
                if (((obj == "cat" && count <= 8) || (obj == "param" && ruleCount <= 4)) && (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Text != "" && (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Text != "-Все элементы-")
                {
                    if (obj == "cat") count++;
                    if (obj == "param") ruleCount++;

                    (GrBox.Controls[obj_name] as System.Windows.Forms.ComboBox).Location = new System.Drawing.Point(oldLoc.X, oldLoc.Y + 27);

                    var datasourceBuf = (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Items.Cast<Object>().Select(item => item.ToString()).ToList(); //буферный лист для коллекции
                    datasourceBuf.Remove((GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Text); //из-за datasource combox не даст удалить элемент коллекции, поэтому удаляем его в буферном листе
                    (GrBox.Controls[obj_name] as System.Windows.Forms.ComboBox).DataSource = datasourceBuf; //Добавляемому контролу должен передаваться список
                    (GrBox.Controls[obj_name] as System.Windows.Forms.ComboBox).SelectedItem = null; //чтобы появлялся без выбранной категории

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
                if ((obj == "cat" && count >= 3) || (obj == "param" && ruleCount >= 2))
                {
                    (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Text = "";
                    (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Location = new System.Drawing.Point((GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Location.X + 400, (GrBox.Controls[oldObj_name] as System.Windows.Forms.ComboBox).Location.Y);

                    //индивидуальные элементы paramgroup//
                    if (obj == "param")
                    {
                        (GrBox.Controls[oldRule_name] as System.Windows.Forms.ComboBox).Location = new System.Drawing.Point((GrBox.Controls[oldRule_name] as System.Windows.Forms.ComboBox).Location.X + 400, (GrBox.Controls[oldRule_name] as System.Windows.Forms.ComboBox).Location.Y);
                        (GrBox.Controls[oldRuleValue_name] as System.Windows.Forms.TextBox).Location = new System.Drawing.Point((GrBox.Controls[oldRuleValue_name] as System.Windows.Forms.TextBox).Location.X + 400, (GrBox.Controls[oldRuleValue_name] as System.Windows.Forms.TextBox).Location.Y);
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

        private void selectControl(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox combox = sender as System.Windows.Forms.ComboBox;

            string name = combox.Text;
            (ParamGroup.Controls["param_1_ComBox"] as System.Windows.Forms.ComboBox).Items.Clear();
            generalParameters(); //суть войда в том, чтобы обновить и составить актуальный список общих параметров

            Task.Factory.StartNew(roughSampling); //возможно, точка вызова будет меняться

        }

        private void ruleSelectControl(object sender, EventArgs e) //для контроля соответствия условия типу параметра
        {
            System.Windows.Forms.ComboBox combox = sender as System.Windows.Forms.ComboBox;
            string paramName = "param" + combox.Name.Remove(0, 4);
            //(ParamGroup.Controls[paramName] as System.Windows.Forms.ComboBox)

            /////временный фрагмент///
            //string path = Path.GetFullPath(@"C:\Users\1\Desktop\Sample.txt"); 
            //StreamWriter sr = new StreamWriter(path);
            //Element el = elementsInView[0];
            /////конец временного фрагмента///


            //foreach (var param in parameter)
            //{
            //    //if (param.Definition.Name.ToString() == (ParamGroup.Controls[paramName] as System.Windows.Forms.ComboBox).Text) MessageBox.Show(param.Definition.ParameterType.ToString());
            //    sr.Write(param.Definition.ParameterType.ToString());
            //    sr.Write(" - ");
            //    try
            //    {
            //        sr.WriteLine(param.AsDouble().ToString());
            //    }
            //    catch
            //    {
            //        sr.WriteLine(param.AsString());
            //    }

                

            //}
            //sr.Close();
        }

        static Element findElement(string name) //модуль находит и возвращает элемент из листа коллектора по его названию
        {
            Element el = null;

            foreach (var element in elementsInView) if (element.Category.Name.ToString() == name) el = element;

            return el;
        }

        static List<string> getParamNames(List<Parameter> recievedParam) //задуматься над тем, чтобы здесь же составлять результирующий лист с выборкой общих параметров
        {
            List<string> nameList = new List<string>();
            parameter = new List<Parameter>();

            foreach (Parameter param in recievedParam)
            {
                try //не у всех параметров есть безопасно возвращаемое "имя", такие параметры нас не интересуют
                {
                    nameList.Add(param.Definition.Name);
                    parameter.Add(param);
                }
                catch
                {
                }   
            }
            return nameList;
        }

        private void generalParameters()
        {
            List<string> nameList = null; //свожу задачу к более простой выборке, чтобы не делать компаратор для класса Parameter
            Element element;


            for (int i = 2; i <= count; i++)
            {
                if ((CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text != "") //пропускаем пустые ComBox
                {
                    
                    element = findElement((CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text); //получаем нужный элемент по названию 
                    //считаю название доверительным параметром, т.к. в Load-блоке названия формируются из параметров этих же элементов, а внешнее редактирование запрещено

                    if (nameList == null)
                        nameList = new List<string>(getParamNames(element.GetOrderedParameters().Concat(doc.GetElement(element.GetTypeId()).GetOrderedParameters()).ToList()));
                    else 
                        nameList = nameList.Intersect(getParamNames(element.GetOrderedParameters().Concat(doc.GetElement(element.GetTypeId()).GetOrderedParameters()).ToList())).ToList(); 
                }
            }


            foreach (var name in nameList)
            {
                if ((ParamGroup.Controls["param_1_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains(name) == false && name[0].ToString() != "-")
                    (ParamGroup.Controls["param_1_ComBox"] as System.Windows.Forms.ComboBox).Items.Add(name);
                (ParamGroup.Controls["param_1_ComBox"] as System.Windows.Forms.ComboBox).Sorted = true;
            }


        }

        private void roughSampling() //осуществляется грубая выборка (без учета параметров) элементов выбранных категорий, 
        { //чтобы при учете параметров не было необходимости проверять все элементы

            if ((CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Text != "Все элементы")
            {
                roughSample.Clear();
                List<string> selectedCategories = new List<string>();
                for (int i = 2; i <= count; i++) selectedCategories.Add((CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text);

                foreach (var element in elementsInView)
                    if (selectedCategories.Contains(element.Category.Name.ToString()) == true) roughSample.Add(element);
            }
        }


        private void result_bt_Click(object sender, EventArgs e)
        {
            result_DGV.Rows.Clear();
            result_DGV.Columns.Clear();
            result_DGV.Refresh();
            result_DGV.Columns.Add("ID", "ID");
            result_DGV.Columns.Add("Cat", "Category");
            result_DGV.Columns.Add("Name", "Name");
            //временный блок
            if ((ParamGroup.Controls["param_1_ComBox"] as System.Windows.Forms.ComboBox).Text != "")
            for (int i = 1; i <= ruleCount; i++)
                result_DGV.Columns.Add((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text, (ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text);
            //конец временного блока 


            foreach (var el in roughSample)
            {
                int row = result_DGV.Rows.Add();
                result_DGV.Rows[row].Cells[0].Value = el.Id.ToString();
                result_DGV.Rows[row].Cells[1].Value = el.Category.Name.ToString();
                result_DGV.Rows[row].Cells[2].Value = el.Name.ToString();



                if ((ParamGroup.Controls["param_1_ComBox"] as System.Windows.Forms.ComboBox).Text != "")
                    
                    for (int i = 1; i <= ruleCount; i++)
                    {
                        try
                        {

                            //result_DGV.Rows[row].Cells[(ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text].Value = el.LookupParameter((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text).AsString();
                        }
                        catch { }

                    }


            }
            result_DGV.Sort(result_DGV.Columns[1], ListSortDirection.Ascending);
        }
    }
}
