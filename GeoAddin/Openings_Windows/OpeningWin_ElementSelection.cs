using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;
using System.Diagnostics;
using System.IO;
using ClosedXML.Excel;



namespace GeoAddin.Openings_Windows
{
    [Transaction(TransactionMode.Manual)]
    public partial class OpeningWin_ElementSelection : System.Windows.Forms.Form
    {
        public UIDocument uidoc;
        public static Document doc;
        public UIApplication uiapp;
        public RevitApplication app;

        //public bool threadState = true;
        public int count = 2; //не хочется переименовывать 8 combobox'ов из-за того, что удалил первый по необходимой нумерации
        public int ruleCount = 1;
        static List<Element> elementsInView;
        static List<Autodesk.Revit.DB.Parameter> parameter;
        List<Element> roughSample = new List<Element>();
        public static List<ElementId> idList;

        public static string[,] paramNamesAndTypes;
        public static string actionType = "";


        public ExternalEvent ExEvent { get; set; }
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

            progress_pb.Visible = false;


            //прикручиваю обработчик событий для всех комбобоксов из catgroup//
            for (int i = 2; i <= 8; i++) (CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).TextChanged += new System.EventHandler(selectControl);
            //**************************************************************//

            for (int i = 1; i <= 5; i++)
                (ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).TextChanged += new System.EventHandler(ruleSelectControl); 
            //нужно в реалтайме подбирать актуальные типы условий


            //---действия над элементами из DGV---//
            action_ComBox.Items.Add("Выделить");
            action_ComBox.Items.Add("Скрыть");
            action_ComBox.Items.Add("Показать все");
            action_ComBox.Items.Add("Удалить");
            //---действия над элементами из DGV---//

            //---логические отношения между правилами---//
            relationship_ComBox.Items.Add("И");
            relationship_ComBox.Items.Add("ИЛИ");
            relationship_ComBox.Text = "ИЛИ";
            //---логические отношения между правилами---//

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
            //threadState = false;
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
            System.Windows.Forms.ComboBox paramBox = sender as System.Windows.Forms.ComboBox;

            string ruleName = "rule" + paramBox.Name.Remove(0, 5);
            System.Windows.Forms.ComboBox ruleBox = (ParamGroup.Controls[ruleName] as System.Windows.Forms.ComboBox);
            //string paramName = "param" + combox.Name.Remove(0, 4);
            string storageType = "";
            //надо написать класс, который будет содержать методы для возврата имен параметров, их типов и т.п.

            for (int i = 0; i < paramNamesAndTypes.Length / paramNamesAndTypes.Rank; i++)
                if (paramNamesAndTypes[i, 0] == paramBox.Text)
                    storageType = paramNamesAndTypes[i, 1];


            switch (storageType) //при внедрении дополнительных функций необходимо изменять и блок checkRules
            {
                case "String":
                    ruleBox.Items.Clear();
                    ruleBox.Items.Add("Равен");
                    break;
                case "None":
                    ruleBox.Items.Clear();
                    break;
                default:
                    ruleBox.Items.Clear();
                    ruleBox.Items.Add("Равен");
                    ruleBox.Items.Add("Больше");
                    ruleBox.Items.Add("Меньше");
                    break;
            }
        }

        static Element findElement(string name) //модуль находит и возвращает элемент из листа коллектора по его названию
        {
            Element el = null;

            foreach (var element in elementsInView) if (element.Category.Name.ToString() == name) el = element;

            return el;
        }

        static List<string> getParamNames(List<Autodesk.Revit.DB.Parameter> recievedParam) //задуматься над тем, чтобы здесь же составлять результирующий лист с выборкой общих параметров
        {
            List<string> nameList = new List<string>();
            parameter = new List<Autodesk.Revit.DB.Parameter>();

            paramNamesAndTypes = new string[recievedParam.Count, 2];
            int i = 0;

            foreach (Autodesk.Revit.DB.Parameter param in recievedParam)
            {
                try //не у всех параметров есть безопасно возвращаемое "имя", такие параметры нас не интересуют
                {
                    nameList.Add(param.Definition.Name);
                    parameter.Add(param);

                    //---Возможно, имеет смысл перейти на использование такой локальной "бд"---//
                    paramNamesAndTypes[i, 0] = param.Definition.Name.ToString();
                    paramNamesAndTypes[i, 1] = param.StorageType.ToString();
                    i++;
                    //------//

                }
                catch
                {
                }   
            }
            return nameList;
        }

        static string getParamValue(string name, Element element)
        {
            Autodesk.Revit.DB.Parameter parameter = null;
            foreach (Autodesk.Revit.DB.Parameter param in element.Parameters) if (param.Definition.Name.ToString() == name) parameter = param;
            var storage_type = parameter.StorageType;

            switch(storage_type)
            {
                case StorageType.String:
                    return parameter.AsString();
                    break;
                case StorageType.Double:
                    return Convert.ToString(Math.Round(parameter.AsDouble() * 304.8, 2)); //перевод из футов в мм. В какой-то степени костыль, ибо неизвестно, только ли размеры хранятся в double
                    break;
                case StorageType.Integer:
                    return Convert.ToString(parameter.AsInteger());
                    break;
                case StorageType.ElementId:
                    return Convert.ToString(parameter.AsElementId());
                    break;

                default: return "None";
            }
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

        public bool checkRules(Element element)
        {
            bool res = false;
            string relationship = relationship_ComBox.Text; //узнаю текущие отношения между правилами
            string paramName; //будет получать имя параметра
            string storageType; //будет получать тип параметра
            string value;

            int checkRuleCount = 0; //число правил, которым текущий элемент удовлетворяет

            for (int i = 1; i <= ruleCount; i++)
            {
                if ((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text != "" && (ParamGroup.Controls["rule_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text != "")
                {
                    paramName = (ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text;

                    int index = -1;
                    for (int j = 0; j < paramNamesAndTypes.Length / 2; j++)
                        if (paramNamesAndTypes[j, 0] == paramName) index = j; //изначально было Array.IndexOf, но оно не отрабатывало, пришлось цикл с условием фигачить

                    storageType = paramNamesAndTypes[index, 1];
                    value = getParamValue(paramName, element);

                    double doubleValue = 0;
                    double doubleSourceValue = 0;
                    string rule = (ParamGroup.Controls["rule_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text;

                    switch (storageType)
                    {
                        case "String":
                            if (rule == "Равен" && value == (ParamGroup.Controls["ruleValue_" + i + "_tb"] as System.Windows.Forms.TextBox).Text)
                                checkRuleCount++;
                            break;

                        case "None":
                            break;

                        default:
                            if (Double.TryParse(value, out doubleValue) && Double.TryParse((ParamGroup.Controls["ruleValue_" + i + "_tb"] as System.Windows.Forms.TextBox).Text, out doubleSourceValue))

                                if (((rule == "Равен" && doubleValue == doubleSourceValue) || (rule == "Больше" && doubleValue > doubleSourceValue) || (rule == "Меньше" && doubleValue < doubleSourceValue)))
                                    checkRuleCount++;
                            break;
                    }

                }
                else if ((ParamGroup.Controls["rule_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text == "") res = true;
            }

            switch(relationship) //число выполненных правил сопоставляется с их количеством в зависимости от отношений между ними
            {
                case "И": //количество выполненных правил должно быть равным количеству правил
                    if (checkRuleCount == ruleCount) res = true;
                    break;
                case "ИЛИ": //количество должно быть хотя бы равным 1
                    if (checkRuleCount >= 1) res = true;
                    break;
            }

            return res;
        }


        private void result_bt_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(ruleCount.ToString());
            
            result_DGV.Rows.Clear();
            result_DGV.Columns.Clear();
            result_DGV.Refresh();
            result_DGV.Columns.Add("ID", "ID");
            result_DGV.Columns.Add("Category", "Category");
            result_DGV.Columns.Add("Name", "Name");
            progress_pb.Visible = true;
            progress_pb.Value = 0;
            progress_pb.Maximum = roughSample.Count;

            //временный блок
            if ((ParamGroup.Controls["param_1_ComBox"] as System.Windows.Forms.ComboBox).Text != "")
            for (int i = 1; i <= ruleCount; i++)
                result_DGV.Columns.Add((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text, (ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text);
            //конец временного блока 

            idList = new List<ElementId>(); //либо получать elementid из первого столбца dgv - думаю, более жизнеспособный подход, чем плодить переменные

            foreach (var el in roughSample)
            {       progress_pb.Value++;
                        try
                        {
                            //высота в ревите представлена в мм, а выводится в футах. Надо подумать, где стоит конвертировать, а где нет
                            //возможно, что размером в мм является все, что хранится в double
                            
                            if (checkRules(el))
                            {
                                int row = result_DGV.Rows.Add();
                                result_DGV.Rows[row].Cells[0].Value = el.Id.ToString();
                                idList.Add(el.Id);
                                result_DGV.Rows[row].Cells[1].Value = el.Category.Name.ToString();
                                result_DGV.Rows[row].Cells[2].Value = el.Name.ToString();

                            for (int i = 1; i <= ruleCount; i++) 
                            result_DGV.Rows[row].Cells[(ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text].Value = getParamValue((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text, el);

                            }
                        }
                        catch { }
            }
            result_DGV.Sort(result_DGV.Columns[1], ListSortDirection.Ascending);


            if (result_DGV.Rows.Count >= 1) action_bt.Enabled = true;
            progress_pb.Visible = false;
        }


        private void action_bt_Click(object sender, EventArgs e) //надо не забыть сделать действия направленными на выделенные в DGV элементы, но это чуть позже
        {
            if (result_DGV.Rows.Count >= 1) 
                switch(action_ComBox.Text)
                {
                    case "":
                        break;
                    case "Выделить":
                        uidoc.Selection.SetElementIds(idList); //первоначальное рабочее решение. Надо подумать, как сделать более корректно/юзерабельно 
                        break;
                    default:
                        if (ExEvent != null)
                        {
                            actionType = action_ComBox.Text;
                            ExEvent.Raise();
                        }
                        else
                            MessageBox.Show("External event handler is null");
                        break;
                }
        }

        private void saveDGV_bt_Click(object sender, EventArgs e)
        {

            Task.Factory.StartNew(Export);

        }

        private void Export()
        {
            
            if (result_DGV.Rows.Count >= 1)
            {
                getPath_sfd.FileName = "ElSample - " + doc.Title + " - " + DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss");
                getPath_sfd.AddExtension = true;
                getPath_sfd.DefaultExt = "xlsx";

                DialogResult dialogResult = DialogResult.None;
                this.Invoke(new Action(() =>
                {
                    //getPath_sfd.ShowDialog();
                    if (getPath_sfd.ShowDialog() == DialogResult.OK) dialogResult = DialogResult.OK;

                })); 
                //это запуск немодального окна, его необходимо выполнить в рамках основного потока. Invoke выполняет это действие "от лица" основного потока
                
                if (dialogResult == DialogResult.OK)
                {
                    progress_pb.Visible = true;
                    progress_pb.Value = 0;
                    progress_pb.Maximum = result_DGV.ColumnCount + result_DGV.Rows.Count * result_DGV.ColumnCount;
                    IXLWorkbook xls_table = new XLWorkbook();
                    IXLWorksheet xls_sheet = xls_table.Worksheets.Add(doc.Title);

                    for (int i = 0; i < result_DGV.ColumnCount; i++)
                    {
                        xls_sheet.Cell(1, 1 + i).Value = result_DGV.Columns[i].Name;
                        xls_sheet.Cell(1, 1 + i).Style.Fill.BackgroundColor = XLColor.Peach;
                        progress_pb.Value++;
                    }
                    XLColor cellColor = XLColor.MediumAquamarine;

                    for (int i = 0; i <= result_DGV.Rows.Count - 1; i++)
                    {
                        if ((i > 0) && (result_DGV.Rows[i].Cells["Category"].Value.ToString() != result_DGV.Rows[i - 1].Cells["Category"].Value.ToString()))
                        {
                            if (cellColor == XLColor.MediumAquamarine) cellColor = XLColor.LightBlue;
                            else cellColor = XLColor.MediumAquamarine;
                        }

                        for (int j = 0; j < result_DGV.ColumnCount; j++)
                        {
                            xls_sheet.Cell(2 + i, 1 + j).Value = result_DGV.Rows[i].Cells[j].Value;
                            xls_sheet.Cell(2 + i, 1 + j).Style.Fill.BackgroundColor = cellColor;
                            progress_pb.Value++;
                        }
                    }

                    progress_pb.Visible = false;
                    xls_sheet.Columns().AdjustToContents();
                    xls_sheet.Columns().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    xls_table.SaveAs(getPath_sfd.FileName);

                    Process.Start("explorer.exe", " /select, " + getPath_sfd.FileName);

                }
            }
        }



        private void loadDGV_bt_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(Import);

        }
        private void Import()
        {

            DialogResult dialogResult = DialogResult.None;
            openFile_ofd.Filter = "Excel файлы (*.xlsx)|*.xlsx";
            bool check = false;
            this.Invoke(new Action(() =>
            {
                if (openFile_ofd.ShowDialog() == DialogResult.OK) dialogResult = DialogResult.OK;

            }));

            if (dialogResult == DialogResult.OK)
            {
                var excelFile = new XLWorkbook(openFile_ofd.FileName);
                var workSheet = excelFile.Worksheet(1);

                if (workSheet.Name != doc.Title)
                    if (MessageBox.Show("Таблица, вероятно, не является выгруженной из текущего документа Revit. Все равно загрузить?", "Несоответствие наименований", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        Thread.ResetAbort();

                if (MessageBox.Show("Проверить элементы на соответствие текущему документу?", "Проверка соответствия", MessageBoxButtons.YesNo) == DialogResult.OK)
                    check = true;

                result_DGV.Rows.Clear();
                result_DGV.Columns.Clear();
                result_DGV.Refresh();


                for (int i = 1; i <= workSheet.ColumnsUsed().Count(); i++)
                    result_DGV.Columns.Add(workSheet.Cell(1, i).Value.ToString(), workSheet.Cell(1, i).Value.ToString());

                for (int i = 2; i <= workSheet.RowsUsed().Count(); i++)
                {
                    int row = result_DGV.Rows.Add();
                    for (int j = 1; j <= workSheet.ColumnsUsed().Count(); j++)
                    {
                        result_DGV.Rows[row].Cells[j - 1].Value = workSheet.Cell(i, j).Value.ToString();
                    }
                }

            }


        }

    }

    public class EventRegHandler : IExternalEventHandler
    {
        public bool EventRegistered { get; set; }
        public void Execute(UIApplication app) //внедрение новых действий осуществлять здесь, основной класс просто ссылается на этот
        {
            if (OpeningWin_ElementSelection.actionType != "" && OpeningWin_ElementSelection.actionType != "Done")
                using (Transaction t = new Transaction(OpeningWin_ElementSelection.doc, "Действие над выделенными элементами"))
                {
                    t.Start();
                    switch (OpeningWin_ElementSelection.actionType)
                    {
                        case "Скрыть":
                            OpeningWin_ElementSelection.doc.ActiveView.HideElements(OpeningWin_ElementSelection.idList);
                            break;
                        case "Показать все":
                            OpeningWin_ElementSelection.doc.ActiveView.UnhideElements(OpeningWin_ElementSelection.idList);
                            break;
                        case "Удалить":
                            OpeningWin_ElementSelection.doc.Delete(OpeningWin_ElementSelection.idList);
                            break;
                    }

                    t.Commit();
                    OpeningWin_ElementSelection.actionType = "Done";
                }

        }


        public string GetName()
        {
            return "EventRegHandler";
        }
    }

}
