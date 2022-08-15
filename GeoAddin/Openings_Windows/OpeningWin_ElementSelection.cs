using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
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

        Stopwatch sw;

        public int count = 2; //не хочется переименовывать 8 combobox'ов из-за того, что удалил первый по необходимой нумерации
        public int ruleCount = 1;
        static List<Element> elementsInView;
        static List<Autodesk.Revit.DB.Parameter> parameter;
        List<Element> roughSample = new List<Element>();
        public static List<ElementId> idList;
        public static List<ElementId> selectedID;


        public static DataTable allElID;
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

            allElID = new DataTable();
            allElID.Columns.Add("ID");
            allElID.Columns.Add("Category"); //в теории можно и расширить, чтобы иметь базу не только id и категорий, но и чего-то еще (имен, например)
            allElID.PrimaryKey = new DataColumn[] { allElID.Columns["ID"] }; //По этому primary key можно находить загруженные элементы

            result_DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            foreach (var element in elementsInView)
            {
                DataRow row = allElID.NewRow();
                try
                {
                    if ((CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains("-Все элементы-") == false) (CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Items.Add("-Все элементы-");
                    if ((CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Items.Contains(element.Category.Name.ToString()) == false)
                    {
                        (CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Items.Add(element.Category.Name.ToString());
                    }

                    row["ID"] = element.Id.ToString();
                    row["Category"] = element.Category.Name.ToString();
                    allElID.Rows.Add(row);
                }
                catch
                {

                }
            }

            (CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Sorted = true; //сортирую по алфавиту

            ToolStripMenuItem fileItem = new ToolStripMenuItem("Файл");
            fileItem.DropDownItems.Add("Сохранить выборку").Click += (snd, ee) => { saveSampleOfSearch(); };
            fileItem.DropDownItems.Add("Экспортировать выборку (.xlsx)", null, new EventHandler(saveDGV_bt_Click));
            fileItem.DropDownItems.Add("Сохранить шаблон поиска").Click += (snd, ee) => { saveRuleSample("save"); }; ;

            menu_ms.Items.Add(fileItem);

            ToolStripMenuItem addItem = new ToolStripMenuItem("Добавить");
            addItem.DropDownItems.Add("Добавить категорию").Click += (snd, ee) => { addDelControl("add", "cat"); };
            addItem.DropDownItems.Add("Добавить правило").Click += (snd, ee) => { addDelControl("add", "param"); };
            menu_ms.Items.Add(addItem);

            ToolStripMenuItem delItem = new ToolStripMenuItem("Удалить");
            delItem.DropDownItems.Add("Удалить категорию").Click += (snd, ee) => { addDelControl("del", "cat"); };
            delItem.DropDownItems.Add("Удалить правило").Click += (snd, ee) => { addDelControl("del", "param"); };
            menu_ms.Items.Add(delItem);

            ToolStripMenuItem loadItem = new ToolStripMenuItem("Загрузить");
            loadItem.DropDownItems.Add("Загрузить xlsx-файл", null, new EventHandler(loadDGV_bt_Click));
            menu_ms.Items.Add(loadItem);

            ToolStripMenuItem savedSamplesItem = new ToolStripMenuItem("Сохраненные выборки");
            loadSavedSearches(ref savedSamplesItem);
            menu_ms.Items.Add(savedSamplesItem);

            ToolStripMenuItem saveSearchItem = new ToolStripMenuItem("Шаблоны поиска");
            loadRuleList(ref saveSearchItem);
            menu_ms.Items.Add(saveSearchItem);

        }

        private void loadRuleList(ref ToolStripMenuItem item)
        {
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SavedSamples");
            if (dirInfo.Exists)
            {
                item.DropDownItems.Clear();
                string[] SSFilesNames = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SavedSamples", "SavedSample - " + doc.Title + " *.*");
                foreach (var name in SSFilesNames)
                {
                    item.DropDownItems.Add(Path.GetFileName(name)).Click += (snd, ee) =>
                    {
                        openFile_ofd.FileName = name;
                        var load = new Thread(() => saveRuleSample("load"));
                        load.Start();
                    };
                }
            }
        }


        private void saveRuleSample(string action)
        {
            IXLWorkbook xls_table;
            IXLWorksheet xls_sheet;
            int count_local = count;
            int ruleCount_local = ruleCount;
            int errors = 0;


            if (action == "save")
            {
                xls_table = new XLWorkbook();
                xls_sheet = xls_table.Worksheets.Add(relationship_ComBox.Text);
            }
            else
            {
                xls_table = new XLWorkbook(openFile_ofd.FileName);
                xls_sheet = xls_table.Worksheet(1);
                relationship_ComBox.Text = xls_sheet.Name;
                count_local = 1 + xls_sheet.Column(1).CellsUsed().Count();
                ruleCount_local = xls_sheet.Column(2).CellsUsed().Count();
            }

            for (int i = 1; i <= 2; i++)
            {
                for (int j = 1; j < 8; j++)
                {
                    if (i == 1 && j < count_local)
                    {
                        int ind = 1 + j;
                        if (action == "save") xls_sheet.Cell(j, i).Value = (CatGroup.Controls["cat_" + ind + "_ComBox"] as System.Windows.Forms.ComboBox).Text;
                        if (action == "load")
                        {
                            try 
                            {
                                (CatGroup.Controls["cat_" + ind + "_ComBox"] as System.Windows.Forms.ComboBox).SelectedIndex =
                                (CatGroup.Controls["cat_" + ind + "_ComBox"] as System.Windows.Forms.ComboBox).Items.IndexOf(xls_sheet.Cell(j, i).Value.ToString());
                                if (j != -1 + count_local)
                                    addDelControl("add", "cat");
                            }
                            catch { errors++; }
                        }
                    }

                    if (i == 2 && j <= ruleCount)
                    {
                        if (action == "save")
                        {
                            xls_sheet.Cell(j, i).Value = (ParamGroup.Controls["param_" + j + "_ComBox"] as System.Windows.Forms.ComboBox).Text;
                            xls_sheet.Cell(j, 1 + i).Value = (ParamGroup.Controls["rule_" + j + "_ComBox"] as System.Windows.Forms.ComboBox).Text;
                            xls_sheet.Cell(j, 2 + i).Value = (ParamGroup.Controls["ruleValue_" + j + "_tb"] as System.Windows.Forms.TextBox).Text;
                        }
                        if (action == "load")
                        {
                            try
                            {
                                (ParamGroup.Controls["param_" + j + "_ComBox"] as System.Windows.Forms.ComboBox).SelectedIndex =
                                (ParamGroup.Controls["param_" + j + "_ComBox"] as System.Windows.Forms.ComboBox).Items.IndexOf(xls_sheet.Cell(j, i).Value.ToString());
                                (ParamGroup.Controls["rule_" + j + "_ComBox"] as System.Windows.Forms.ComboBox).SelectedIndex =
                                    (ParamGroup.Controls["rule_" + j + "_ComBox"] as System.Windows.Forms.ComboBox).Items.IndexOf(xls_sheet.Cell(j, 1 + i).Value.ToString());
                                (ParamGroup.Controls["ruleValue_" + j + "_tb"] as System.Windows.Forms.TextBox).Text = xls_sheet.Cell(j, 2 + i).Value.ToString();
                                if (j < ruleCount_local)
                                    addDelControl("add", "param");
                            }
                            catch{ errors++; }
                        }
                    }

                }
            }
            if (action == "save")
            {
                xls_table.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SavedSamples\\SavedSample - " + doc.Title + " - " + DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss") + ".xlsx");
                ToolStripMenuItem item = new ToolStripMenuItem("Шаблоны поиска");
                loadRuleList(ref item);
                menu_ms.Items.RemoveAt(5);
                menu_ms.Items.Add(item);
            }
            xls_table.Dispose();
            timeLabel.Text = errors.ToString();
        }

        private void loadSavedSearches(ref ToolStripMenuItem item)
        {
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SavedSearches");
            if (dirInfo.Exists)
            {
                item.DropDownItems.Clear();
                string[] SSFilesNames = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SavedSearches", "SavedSearch - " + doc.Title + " *.*");
                foreach (var name in SSFilesNames) 
                {
                    item.DropDownItems.Add(Path.GetFileName(name)).Click += (snd, ee) => 
                    { 
                        openFile_ofd.FileName = name;
                        var import = new Thread(() => Import("loadSavedSearches"));
                        import.Start();
                    }; 
                }
            }
        }



        private void saveSampleOfSearch()
        {
            if (result_DGV.Rows.Count >= 1)
            {
                getPath_sfd.FileName = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SavedSearches\\SavedSearch - " + doc.Title + " - " + DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss") + ".xlsx";
                getPath_sfd.AddExtension = true;
                getPath_sfd.DefaultExt = "xlsx";
                getPath_sfd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                var export = new Thread(() => Export("saveSampleOfSearch"));
                export.Start();
            }
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
            timeLabel.Location = new System.Drawing.Point(timeLabel.Location.X, this.Height - 52);
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
            this.Close();
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
            int localCount = count;
            string catName;

            if ((CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Text == "-Все элементы-")
                localCount = (CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Items.Count;

            for (int i = 2; i <= localCount; i++)
            {
                if ((CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Text == "-Все элементы-")
                {
                    catName = (CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Items[-1+i].ToString();
                }
                else catName = (CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text;

                if (catName != "") //пропускаем пустые ComBox
                {
                    try
                    {
                        element = findElement(catName); //получаем нужный элемент по названию 
                                                        //считаю название доверительным параметром, т.к. в Load-блоке названия формируются из параметров этих же элементов, а внешнее редактирование запрещено
                        if (nameList == null)
                            nameList = new List<string>(getParamNames(element.GetOrderedParameters().Concat(doc.GetElement(element.GetTypeId()).GetOrderedParameters()).ToList()));
                        else
                            nameList = nameList.Intersect(getParamNames(element.GetOrderedParameters().Concat(doc.GetElement(element.GetTypeId()).GetOrderedParameters()).ToList())).ToList();
                    }
                    catch { }
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

            if ((CatGroup.Controls["cat_2_ComBox"] as System.Windows.Forms.ComboBox).Text != "-Все элементы-")
            {
                roughSample.Clear();
                List<string> selectedCategories = new List<string>();
                for (int i = 2; i <= count; i++) selectedCategories.Add((CatGroup.Controls["cat_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text);

                foreach (var element in elementsInView)   
                    if (selectedCategories.Contains(element.Category.Name.ToString()) == true) 
                        roughSample.Add(element);
            }
            else
                roughSample = elementsInView;
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
                else if ((ParamGroup.Controls["rule_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text == "") 
                    res = true;
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



        private void result(string sender)
        {
            DataTable localTable = new DataTable();

            if (sender == "result_bt")
            {
                result_DGV.Refresh();
                localTable.Columns.Add("ID"); 
                localTable.Columns.Add("Category");
                localTable.Columns.Add("Name");

                localTable.PrimaryKey = new DataColumn[] { localTable.Columns["ID"] };

                if ((ParamGroup.Controls["param_1_ComBox"] as System.Windows.Forms.ComboBox).Text != "")
                    for (int i = 1; i <= ruleCount; i++)
                        localTable.Columns.Add((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text);
            }

            if (sender == "modify_bt")
            {
                localTable = (DataTable)result_DGV.DataSource;
                for (int i = 1; i <= ruleCount; i++)
                {
                    if (localTable.Columns.Contains((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text) == false && (ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text != "")
                        localTable.Columns.Add((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text);
                }
            }

            progress_pb.Visible = true;
            progress_pb.Value = 0;
            progress_pb.Maximum = roughSample.Count;

            idList = new List<ElementId>(); //либо получать elementid из первого столбца dgv - думаю, более жизнеспособный подход, чем плодить переменные

            sw = new Stopwatch();
            sw.Start();

            foreach (var el in roughSample)
            {
                progress_pb.Value++;
                try
                {
                    //высота в ревите представлена в мм, а выводится в футах. Надо подумать, где стоит конвертировать, а где нет
                    //возможно, что размером в мм является все, что хранится в double

                    if (localTable.Rows.Contains(el.Id.ToString()) == false && checkRules(el))
                    {
                        DataRow row = localTable.NewRow();
                        row[0] = el.Id.ToString();
                        row[1] = el.Category.Name.ToString();
                        row[2] = el.Name.ToString();

                        idList.Add(el.Id);

                        for (int i = 1; i <= ruleCount; i++)
                            if ((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text != "")
                                try
                                {

                                    row[2 + i] = getParamValue((ParamGroup.Controls["param_" + i + "_ComBox"] as System.Windows.Forms.ComboBox).Text, el);
                                }
                                catch
                                {
                                    row[2 + i] = "Error";
                                }

                        localTable.Rows.Add(row);

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Source + " - " + ex.Message);
                }
            }
            result_DGV.DataSource = localTable;
            result_DGV.Sort(result_DGV.Columns[1], ListSortDirection.Ascending);
            result_DGV.ClearSelection();
            sw.Stop();
            timeLabel.Text = (sw.ElapsedMilliseconds).ToString() + " мс. на " + result_DGV.Rows.Count.ToString() + " элементов с " + result_DGV.ColumnCount.ToString() + " параметрами вывода";


            if (result_DGV.Rows.Count >= 1) action_bt.Enabled = true;
            progress_pb.Visible = false;


        }

        private void mouseClickOnDGV(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (result_DGV.Rows[e.RowIndex].Selected)
            //{
            //    result_DGV.Rows[e.RowIndex].Selected = false;
            //}
        }


        private void result_bt_Click(object sender, EventArgs e)
        {
            result("result_bt");
        }


        private void action_bt_Click(object sender, EventArgs e) //надо не забыть сделать действия направленными на выделенные в DGV элементы, но это чуть позже
        {
            if (result_DGV.Rows.Count >= 1)
            {
                selectedID = new List<ElementId>();
                if (result_DGV.SelectedCells.Count > 0)
                    for (int i = 0; i < result_DGV.SelectedRows.Count; i++)
                        selectedID.Add(idList[idList.FindIndex(x => x.ToString() == result_DGV.SelectedRows[i].Cells[0].Value.ToString())]);
                        
                else selectedID = idList;

                switch (action_ComBox.Text)
                {
                    case "":
                        break;
                    case "Выделить":
                        uidoc.Selection.SetElementIds(selectedID); 
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
                
                
        }

        private void saveDGV_bt_Click(object sender, EventArgs e)
        {
            if (result_DGV.Rows.Count >= 1)
            {
                getPath_sfd.FileName = "ElSample - " + doc.Title + " - " + DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss");
                getPath_sfd.AddExtension = true;
                getPath_sfd.DefaultExt = "xlsx";
                DialogResult dialogResult = DialogResult.None;

                if (getPath_sfd.ShowDialog() == DialogResult.OK) 
                    dialogResult = DialogResult.OK;


                if (dialogResult == DialogResult.OK)
                {
                    var export = new Thread(() => Export("saveDGV_bt_Click"));
                    export.Start();
                }
            }

        }

        private void Export(string source)
        {
            sw = new Stopwatch();
            sw.Start();

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
            sw.Stop();
            timeLabel.Text = (sw.ElapsedMilliseconds).ToString() + " мс., - OK";


            if (source == "saveDGV_bt_Click") Process.Start("explorer.exe", " /select, " + getPath_sfd.FileName);
            if (source == "saveSampleOfSearch")
            {
                ToolStripMenuItem item = new ToolStripMenuItem("Сохраненные выборки");
                loadSavedSearches(ref item);
                menu_ms.Items.RemoveAt(4);
                menu_ms.Items.Add(item);
            }
        }



        private void loadDGV_bt_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.None;
            openFile_ofd.Filter = "Excel файлы (*.xlsx)|*.xlsx";

            if (openFile_ofd.ShowDialog() == DialogResult.OK) 
                dialogResult = DialogResult.OK;


            if (dialogResult == DialogResult.OK)
            {
                var import = new Thread(() => Import("loadDGV_bt_Click"));
                import.Start();
            }

            //Надо не забыть починить скроллбар
        }
        
        private bool checkID(string ID, string category)
        {
            if (allElID.Rows.Contains(ID))
            {
                DataRow foundRow = allElID.Rows.Find(ID);
                if (null != foundRow && foundRow["Category"].ToString() == category)
                    return true;
                else return false;
            }
            else return false;
        }
        private void Import(string source)
        {
            var excelFile = new XLWorkbook(openFile_ofd.FileName);
            var workSheet = excelFile.Worksheet(1);
            bool check = false;

            if (source == "loadDGV_bt_Click" && workSheet.Name != doc.Title)
                if (MessageBox.Show("Таблица, вероятно, не является выгруженной из текущего документа Revit. Все равно загрузить?", "Несоответствие наименований", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    Thread.ResetAbort();

            if (source == "loadDGV_bt_Click" && MessageBox.Show("Проверить элементы на соответствие текущему документу?", "Проверка соответствия", MessageBoxButtons.YesNo) == DialogResult.Yes)
                check = true;

            result_DGV.Refresh();

            int rowsUsed = workSheet.RowsUsed().Count();
            int columnsUsed = workSheet.ColumnsUsed().Count();
            DataTable localTable = new DataTable();

            progress_pb.Value = 0;
            progress_pb.Maximum = rowsUsed;

            for (int i = 1; i <= columnsUsed; i++)
                localTable.Columns.Add(workSheet.Cell(1, i).Value.ToString());

            progress_pb.Visible = true;

            sw = new Stopwatch();
            sw.Start();

            for (int i = 2; i <= rowsUsed; i++)
            {
                progress_pb.Value++;
                DataRow row = localTable.NewRow();

                if (check)
                    if (checkID(workSheet.Cell(i, 1).Value.ToString(), workSheet.Cell(i, 2).Value.ToString()) == false)
                        continue;

                for (int j = 1; j <= columnsUsed; j++)
                    row[j - 1] = workSheet.Cell(i, j).Value.ToString();

                localTable.Rows.Add(row);
            }

            MessageBox.Show("Список успешно загружен."); //смешно, но надо понять, почему без этого не работает
            result_DGV.DataSource = localTable;

            sw.Stop();
            if (check == false) timeLabel.Text = (sw.ElapsedMilliseconds).ToString() + " мс., выгружено элементов: " + result_DGV.RowCount.ToString();
            if (check) timeLabel.Text = $"{(sw.ElapsedMilliseconds).ToString()} мс. | Элементов в списке было: {rowsUsed}; из них загружено: {result_DGV.RowCount.ToString()}";

            localTable = null;
            workSheet = null;
            excelFile.Dispose();

            progress_pb.Visible = false;

        }

        private void addToResult_bt_Click(object sender, EventArgs e)
        {
            result("modify_bt");
        }

        private void clearAll_bt_Click(object sender, EventArgs e)
        {
            //foreach (var ComBox in ParamGroup.Controls.OfType<System.Windows.Forms.ComboBox>())
            //    ComBox.Text = "";

            while (ruleCount > 1) addDelControl("del", "param");

            foreach (var TB in ParamGroup.Controls.OfType<System.Windows.Forms.TextBox>())
                TB.Text = "";

            param_1_ComBox.Text = "";
        }
    }

    public class EventRegHandler : IExternalEventHandler
    {
        public bool EventRegistered { get; set; }
        public void Execute(UIApplication app) //внедрение новых действий осуществлять здесь, основной класс просто ссылается на этот
        {
            if (OpeningWin_ElementSelection.actionType != "" && OpeningWin_ElementSelection.actionType != "Done")
            {
                using (Transaction t = new Transaction(OpeningWin_ElementSelection.doc, "Действие над выделенными элементами"))
                {
                    t.Start();
                    switch (OpeningWin_ElementSelection.actionType)
                    {
                        case "Скрыть":
                            OpeningWin_ElementSelection.doc.ActiveView.HideElements(OpeningWin_ElementSelection.selectedID);
                            break;
                        case "Показать все":
                            OpeningWin_ElementSelection.doc.ActiveView.UnhideElements(OpeningWin_ElementSelection.selectedID);
                            break;
                        case "Удалить":
                            OpeningWin_ElementSelection.doc.Delete(OpeningWin_ElementSelection.selectedID);
                            break;
                    }

                    t.Commit();
                    OpeningWin_ElementSelection.actionType = "Done";
                }
            }    
        }


        public string GetName()
        {
            return "EventRegHandler";
        }
    }

}
