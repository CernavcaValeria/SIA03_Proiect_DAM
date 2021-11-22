using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using DAMS_Proiect.Utilities;
//using static DAMS_Proiect.Entity;

namespace DAMS_Proiect
{
    public partial class Form1 : Form
    {
        private readonly DateTimePicker dateTimePicker = new DateTimePicker();
        private Rectangle rectangle;
        protected ComboBox GridComboForFonts = new ComboBox();
        protected ComboBox CellComboForFonts = new ComboBox();
        protected ComboBox RowComboForFonts = new ComboBox();
        protected ComboBox GridComboForSize = new ComboBox();
        protected ComboBox CellComboForSize = new ComboBox();
        protected ComboBox RowComboForSize = new ComboBox();
        protected Label fontsLabelCell = new Label();
        protected Label fontsLabelGrid = new Label();
        protected Label fontsLabelRow = new Label();
        protected Label sizesLabelCell = new Label();
        protected Label sizesLabelGrid = new Label();
        protected Label sizesLabelRow = new Label();
        protected static DataGridViewCell currentCell;
        protected Dictionary<int, int> oldDurationsCollection = new Dictionary<int, int>();
        protected Dictionary<float, float> oldCompleteValueCollection = new Dictionary<float, float>();

        public Form1()
        {
            InitializeComponent();
            FillGridWithRows();
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[7].ReadOnly = true;

            this.logInToolStripMenuItem.Text = (Entity.Acces.IsUserLoggedIn ? "Log out" : "Log In");
            Entity.project = new Project();

            dataGridView1.Controls.Add(dateTimePicker);
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.TextChanged += (s, e) => { dataGridView1.CurrentCell.Value = dateTimePicker.Text.ToString(); };
            dateTimePicker.Visible = false;

            exitItem.Click += Menu_File_Exit;
            initItem.Click += Menu_File_Refresh;
            //openItem.Click += MenuOpen_ItemClicked;

            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellClick += DataGridView1_CellClick;
            comboBox1.TextChanged += ComboPrioritySelectedValue;
            //fonts change events
            InitializeComboForFonts();
            AddToFontsComboSizes();
            CellComboForFonts.TextChanged += CellComboForFontsSelected;
            GridComboForFonts.TextChanged += GirdComboForFontsSelected;
            RowComboForFonts.TextChanged += RowComboForFontsSelected;
            //size change events
            InitializeComboForSize();
            AddToSizeComboSizes();
            CellComboForSize.TextChanged += CellComboForFontsSelected;
            RowComboForSize.TextChanged += RowComboForSizeSelected;
            GridComboForSize.TextChanged += GridComboForSizeSelected;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex + 1].Cells[e.ColumnIndex] != null)
                currentCell = dataGridView1.Rows[e.RowIndex + 1].Cells[e.ColumnIndex];
            try
            {
                dateTimePicker.Visible = false;

                if (e.ColumnIndex == 4 || e.ColumnIndex == 5)
                {
                    rectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    dateTimePicker.SetBounds(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                    dateTimePicker.Visible = true;
                    dateTimePicker.MinimumSize = new Size(rectangle.Width, rectangle.Height);
                    dateTimePicker.Format = DateTimePickerFormat.Custom;
                    dateTimePicker.CustomFormat = "dd.MM.yyyy";
                }
                else if (e.ColumnIndex == 6)
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)
                    {
                        dataGridView1.Columns[6].ReadOnly = true;
                    }
                    else
                    {
                        rectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        comboBox1.Location = new Point(rectangle.X, rectangle.Y + rectangle.Height + 5);
                        comboBox1.MinimumSize = new Size(rectangle.Width, rectangle.Height);

                        comboBox1.Name = "comboBox1";
                        comboBox1.Visible = true;
                        comboBox1.ForeColor = Color.Black;
                        comboBox1.DropDownHeight = 3 * rectangle.Height;
                        comboBox1.DropDownWidth = rectangle.Width;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }


        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                switch (e.ColumnIndex)
                {
                    case 1://name
                        InsertNameValue(e);
                        break;

                    case 2://duration
                        InsertDurationValue(e);
                        break;
                    case 3:
                        InsertCompleteValue(e);
                        break;
                    case 4://start
                    case 5://finish
                        InsertDateValue(e);
                        break;
                    case 6:
                        InsertPriority(e);
                        break;
                    case 7:
                        InsertResource(e);
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void InsertPriority(DataGridViewCellEventArgs e)
        {
            try
            {
                comboBox1.Visible = false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void InsertResource(DataGridViewCellEventArgs e)
        {
            try
            {
                if (!Entity.Acces.IsFillingFromDataBase)
                {
                    string resourceValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn7"].Value == null)
                    {
                        Entity.project.tasksList.Add(new Task(e.RowIndex + 1, resourceValue, "Resource"));
                        GenerateDefaultValues(e);
                    }
                    else//Task already exists,so just change the resource value
                    {
                        Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Resource = resourceValue;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }


        private void InsertNameValue(DataGridViewCellEventArgs e)
        {
            if (!Entity.Acces.IsFillingFromDataBase)
            {
                try
                {
                    string nameValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                    if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null || Entity.Acces.IsFillingFromDataBase)//check ID existence
                    {
                        Entity.project.tasksList.Add(new Task(e.RowIndex + 1, nameValue, "Name"));
                        GenerateDefaultValues(e);
                    }
                    else//Task already exists,so just change the name value
                    {
                        Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Name = nameValue;
                    }

                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
            }
        }

        private void InsertCompleteValue(DataGridViewCellEventArgs e)
        {
            if (!Entity.Acces.IsFillingFromDataBase)
            {
                var stringValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Replace("%", "");
                try
                {
                    if (float.TryParse(stringValue, out float compeletValue))
                    {
                        if (compeletValue <= 100)
                        {
                            if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)//check ID existence
                            {
                                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = compeletValue + "%";
                                if (!oldCompleteValueCollection.ContainsKey(e.RowIndex + 1))
                                {
                                    oldCompleteValueCollection.Add(e.RowIndex + 1, compeletValue);
                                    Entity.project.tasksList.Add(new Task(e.RowIndex + 1, compeletValue, "c"));
                                }
                                GenerateDefaultValues(e);
                            }
                            else//Task already exists,so just change the compl value
                            {
                                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = compeletValue + "%";
                                if (oldCompleteValueCollection.ContainsKey(e.RowIndex + 1))
                                {
                                    oldCompleteValueCollection[e.RowIndex + 1] = compeletValue;
                                }
                                Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Complete = compeletValue;
                            }
                        }
                        else
                        {
                            if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)//check ID existence
                            {
                                dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn4"].Value = 0 + "%";
                                Entity.project.tasksList.Add(new Task(e.RowIndex + 1, 0f, "c"));
                                GenerateDefaultValues(e);
                            }
                            else//Task already exists; because an invalid value was entered,sets the compete to previous value
                            {
                                bool oldCompletionValueExists = oldCompleteValueCollection.ContainsKey(e.RowIndex + 1);
                                MessageBox.Show("Percent complete must take value less than 100%!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn4"].Value = oldCompletionValueExists ?
                                    oldCompleteValueCollection[e.RowIndex + 1] + "%" : 0 + "%";

                                Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Complete = oldCompletionValueExists ?
                                    oldCompleteValueCollection[e.RowIndex + 1] : 0;
                            }
                        }
                    }
                    else
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)//check ID existence
                        {
                            dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn4"].Value = 0;
                            Entity.project.tasksList.Add(new Task(e.RowIndex + 1, 0, "c"));
                            GenerateDefaultValues(e);
                        }
                        else//Task already exists; because an invalid value was entered,sets the duration to previous value
                        {
                            bool oldCompletionValueExists = oldCompleteValueCollection.ContainsKey(e.RowIndex + 1);
                            MessageBox.Show("Only numbers are allowed!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn4"].Value = oldCompletionValueExists ?
                                oldCompleteValueCollection[e.RowIndex + 1] + "%" : 0 + "%";

                            Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Complete = oldCompletionValueExists ?
                                oldCompleteValueCollection[e.RowIndex + 1] : 0;
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
            }
        }


        private void InsertDurationValue(DataGridViewCellEventArgs e)
        {
            if (!Entity.Acces.IsFillingFromDataBase)
            {
                var stringValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Replace("h", "");
                try
                {
                    if (int.TryParse(stringValue, out int durationValue))
                    {
                        if (durationValue <= 100)
                        {
                            if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)//check ID existence
                            {
                                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = durationValue + "h";
                                if (!oldDurationsCollection.ContainsKey(e.RowIndex + 1))
                                {
                                    oldDurationsCollection.Add(e.RowIndex + 1, durationValue);
                                    Entity.project.tasksList.Add(new Task(e.RowIndex + 1, durationValue, "d"));
                                }
                                GenerateDefaultValues(e);
                            }
                            else//Task already exists,so just change the compl value
                            {
                                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = durationValue + "h";
                                if (oldDurationsCollection.ContainsKey(e.RowIndex + 1))
                                {
                                    oldDurationsCollection[e.RowIndex + 1] = durationValue;
                                }
                                Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Duration = durationValue;
                            }
                        }
                        else
                        {
                            if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)//check ID existence
                            {
                                dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn3"].Value = 1 + "h";
                                Entity.project.tasksList.Add(new Task(e.RowIndex + 1, 1, "d"));
                                GenerateDefaultValues(e);
                            }
                            else//Task already exists; because an invalid value was entered,sets the compete to previous value
                            {
                                bool oldDurationValueExists = oldDurationsCollection.ContainsKey(e.RowIndex + 1);
                                MessageBox.Show("Duration must take value less than 100%!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn3"].Value = oldDurationValueExists ?
                                    oldDurationsCollection[e.RowIndex + 1] + "h" : 0 + "h";

                                Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Duration = oldDurationValueExists ?
                                    oldDurationsCollection[e.RowIndex + 1] : 0;
                            }
                        }
                    }
                    else
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)//check ID existence
                        {
                            dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn3"].Value = 1;
                            Entity.project.tasksList.Add(new Task(e.RowIndex + 1, 1, "c"));
                            GenerateDefaultValues(e);
                        }
                        else//Task already exists; because an invalid value was entered,sets the duration to previous value
                        {
                            bool oldDurationValueExists = oldDurationsCollection.ContainsKey(e.RowIndex + 1);
                            MessageBox.Show("Only numbers are allowed!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn3"].Value = oldDurationValueExists ?
                                oldDurationsCollection[e.RowIndex + 1] + "h" : 1 + "h";

                            Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Duration = oldDurationValueExists ?
                                oldDurationsCollection[e.RowIndex + 1] : 1;
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
            }
        }


        /*
                private void InsertDurationValue(DataGridViewCellEventArgs e)
                {
                    if (!IsFillingFromDataBase)
                    {
                        try
                        {
                            if (int.TryParse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out int durationValue))
                            {
                                if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)//check ID existence
                                {
                                    oldDurationsCollection.Add(e.RowIndex + 1, durationValue);
                                    Entity.project.tasksList.Add(new Task(e.RowIndex + 1, durationValue, "d"));
                                    GenerateDefaultValues(e);
                                }
                                else//Task already exists,so just change the duration value
                                {
                                    oldDurationsCollection[e.RowIndex + 1] = durationValue;
                                    Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Duration = durationValue;
                                }
                            }
                            else
                            {
                                if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)//check ID existence
                                {
                                    dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn3"].Value = 1;
                                    Entity.project.tasksList.Add(new Task(e.RowIndex + 1, 1, "d"));
                                    GenerateDefaultValues(e);
                                }
                                else//Task already exists; because an invalid value was entered,sets the duration to previous value
                                {
                                    bool oldDurationExists = oldDurationsCollection.ContainsKey(e.RowIndex + 1);
                                    MessageBox.Show("Only numbers are allowed!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                    dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn3"].Value = oldDurationExists ?
                                        oldDurationsCollection[e.RowIndex + 1] : 1;

                                    Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Duration = oldDurationExists ?
                                        oldDurationsCollection[e.RowIndex + 1] : 1;
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.ToString());
                        }
                    }
                }
                */

        private void InsertDateValue(DataGridViewCellEventArgs e)
        {
            if (!Entity.Acces.IsFillingFromDataBase)
            {
                try
                {
                    if (dateTimePicker.Visible == true)
                    {
                        bool validDateFormat = DateTime.TryParse(dateTimePicker.Value.ToString(), out DateTime date);
                        if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)//check ID existence
                        {
                            var dateTimeToInsert = validDateFormat ? date : DateTime.Now;
                            Entity.project.tasksList.Add(new Task(e.RowIndex + 1, dateTimeToInsert, dataGridView1.Columns[e.ColumnIndex].Name.ToString()));
                            GenerateDefaultValues(e);
                        }
                        else//Task already exists,so just change the start/finish value
                        {
                            if (dataGridView1.Columns[e.ColumnIndex].Name.Equals("dataGridViewTextBoxColumn5"))
                            {
                                Task t = Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1);
                                if (validDateFormat)
                                    t.Start = date;
                                else
                                    t.Start = DateTime.Now;
                            }
                            else if (dataGridView1.Columns[e.ColumnIndex].Name.Equals("dataGridViewTextBoxColumn6"))
                            {
                                Entity.project.tasksList.Find(task => task.TaskId == e.RowIndex + 1).Finish = validDateFormat ? date : DateTime.Now;
                            }
                        }
                        dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[1];
                        DataGridView1_Click(dataGridView1, new EventArgs());
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
            }
        }


        private void Menu_File_Refresh(object sender, EventArgs e)
        {
            ClearAll();
        }

        public void ClearAll()
        {
            oldDurationsCollection.Clear();
            oldCompleteValueCollection.Clear();
            dataGridView1.Rows.Clear();
            Entity.project.tasksList.Clear();
            FillGridWithRows();
        }

        private void Menu_File_Save(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0)
            {
                if (Entity.Acces.IsUserLoggedIn)
                {
                    if(new UpdateData().UpdateCurrentProjectTaskListAfterSaveClick())
                        MessageBox.Show("Project succesfully saved in DB!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Fail to save this project: '"+Entity.project.Name+" in DB!", "Save error alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }          
                else MessageBox.Show("Fail! You need to log in for saving this project in Data Base", "Save error alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Menu_File_Open_fromXML(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName,
                Filter = "XML Files (*.xml)|*.xml",
                FilterIndex = 0,
                DefaultExt = "xml"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ClearAll();

                if (Path.GetExtension(openFileDialog.FileName).Equals(".xml"))
                {
                    Entity.project.xmlPath = openFileDialog.FileName;
                    Entity.project.DeserializeTasks();
                    FillGridWithTaskListContent();
                }
                else
                    MessageBox.Show("You must select an XML file!", "Invalid input file type", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static Action OpenProjectAfterSelectFromList;
        private void Menu_File_Open_fromDB(object sender, EventArgs e)
        {
            var accesToProjectsDataBase = new AccesForms(this);
            if (Entity.Acces.IsUserLoggedIn)
            {
                accesToProjectsDataBase.DisplayAndProposeForSelectProjectfromDB(dataGridView1);
                dataGridView1.Rows.Clear();
                FillGridWithTaskListContent();
            }
            else//redirect to register-login
                accesToProjectsDataBase.CreateAccesForm();
        }

        private void Menu_Report_PDF(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    InitialDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName,
                    Filter = "PDF Files (*.pdf)|*.pdf"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pdfFilePath = saveFileDialog.FileName;
                    PDFClass pdfHelper = new PDFClass();
                    pdfHelper.CreatePdfFile(Entity.project, pdfFilePath);
                    MessageBox.Show("File successfully converted to PDF!", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Menu_Report_CSV(object sender, EventArgs e)
        {
            CSVFunctionHelper();
        }

        private void Menu_Report_XML(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    InitialDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName,
                    Filter = "XML Files (*.xml)|*.xml"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Entity.project.xmlPath = saveFileDialog.FileName;
                    string message = Entity.project.SerializeTasks();
                    MessageBox.Show(message, "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void FillGridWithTaskListContent()
        {
            try
            {
                FillGridWithRows();
                foreach (Task task in Entity.project.tasksList)
                {
                    dataGridView1.Rows[task.TaskId - 1].Cells[0].Value = task.TaskId;
                    dataGridView1.Rows[task.TaskId - 1].Cells[1].Value = task.Name;
                    dataGridView1.Rows[task.TaskId - 1].Cells[2].Value = task.Duration;
                    dataGridView1.Rows[task.TaskId - 1].Cells[3].Value = task.Complete;
                    dataGridView1.Rows[task.TaskId - 1].Cells[4].Value = task.Start;
                    dataGridView1.Rows[task.TaskId - 1].Cells[5].Value = task.Finish;
                    dataGridView1.Rows[task.TaskId - 1].Cells[6].Value = task.Priority;
                    dataGridView1.Rows[task.TaskId - 1].Cells[7].Value = task.Resource;
                    // dataGridView1.Rows[task.TaskId - 1].Cells[7].Value = string.Join(", ", task.ResourceNames.ToList());
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }


        private void Menu_File_Exit(object sender, EventArgs e)
        {
            const string message = "Do you want to exit?";
            const string winName = "Exit dialog";
            var result = MessageBox.Show(message, winName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void GenerateDefaultValues(DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value == null)
            {
                dataGridView1.Columns[0].ReadOnly = false;
                dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value = e.RowIndex + 1;
                dataGridView1.Columns[0].ReadOnly = true;
            }
            if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn4"].Value == null)
                dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn4"].Value = 0 + "%";

            if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn2"].Value == null)
                dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn2"].Value = "<task" + (e.RowIndex + 1) + ">";

            if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn3"].Value == null)
                dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn3"].Value = 1 + "h";

            if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn7"].Value == null)
                dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn7"].Value = "Medium";

            if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn8"].Value == null)
                dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn8"].Value = "---";

            switch (dataGridView1.Columns[e.ColumnIndex].Name)
            {
                case "dataGridViewTextBoxColumn3":
                case "dataGridViewTextBoxColumn2":
                case "dataGridViewTextBoxColumn4":
                    if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn5"].Value == null)
                        dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn5"].Value = DateTime.Now.ToString("dd.MM.yyyy");

                    if (dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn6"].Value == null)
                        dataGridView1.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn6"].Value = DateTime.Now.ToString("dd.MM.yyyy");
                    break;
            }
        }

        private void FillGridWithRows()
        {
            for (int i = 0; i < 20; i++)
                dataGridView1.Rows.Add();
        }

        private void ComboPrioritySelectedValue(object sender, EventArgs e)
        {
            if (dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value != null)
            {
                Entity.project.tasksList.Find(task => task.TaskId == dataGridView1.CurrentCell.RowIndex + 1).Priority = comboBox1.Text.ToString();
                dataGridView1.CurrentCell.Value = comboBox1.Text.ToString();
            }
        }
        private void DateTimePicker_TextChange(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell.Value = dateTimePicker.Text.ToString();
        }

        private void DataGridView1_Click(object sender, EventArgs e)
        {
            dateTimePicker.Visible = false;
        }


        //Fonts-------------------------------------------------
        public void InitializeComboForFonts()
        {
            List<string> fonts = new List<string>();
            foreach (FontFamily font in FontFamily.Families)
            {
                fonts.Add(font.Name);
            }
            CellComboForFonts.Items.AddRange(fonts.ToArray());
            GridComboForFonts.Items.AddRange(fonts.ToArray());
            RowComboForFonts.Items.AddRange(fonts.ToArray());
            fontsLabelCell.Text = "- Font for selected cell:";
            fontsLabelRow.Text = "- Font for selected row:";
            fontsLabelGrid.Text = "- Font for entire grid:";
        }
        public void InitializeComboForSize()
        {
            List<string> sizes = new List<string>();
            for (int i = 2; i <= 40; i += 2)
                sizes.Add(i.ToString());
            CellComboForSize.Items.AddRange(sizes.ToArray());
            GridComboForSize.Items.AddRange(sizes.ToArray());
            RowComboForSize.Items.AddRange(sizes.ToArray());
            sizesLabelCell.Text = "- Text size for current cell:";
            sizesLabelRow.Text = "- Text size for selected row:";
            sizesLabelGrid.Text = "- Text size for entire grid:";
        }

        private void Menu_View_Font(object sender, EventArgs e)
        {
            Form newform = new Form();
            newform.Text = "Text Fonts";
            newform.Size = new Size(450, 200);
            newform.Location = new Point(500, 350);
            AddOwnedForm(newform);
            newform.Controls.Add(CellComboForFonts);
            newform.Controls.Add(GridComboForFonts);
            newform.Controls.Add(RowComboForFonts);
            newform.Controls.Add(fontsLabelCell);
            newform.Controls.Add(fontsLabelGrid);
            newform.Controls.Add(fontsLabelRow);
            newform.ShowDialog();
        }


        private void Menu_View_Size(object sender, EventArgs e)
        {
            Form newformSize = new Form();
            newformSize.Text = "Text Size";
            newformSize.Size = new Size(350, 180);
            newformSize.Location = new Point(500, 350);
            AddOwnedForm(newformSize);
            newformSize.Controls.Add(CellComboForSize);
            newformSize.Controls.Add(GridComboForSize);
            newformSize.Controls.Add(RowComboForSize);
            newformSize.Controls.Add(sizesLabelRow);
            newformSize.Controls.Add(sizesLabelCell);
            newformSize.Controls.Add(sizesLabelGrid);
            newformSize.ShowDialog();
        }

        private void AddToFontsComboSizes()
        {
            CellComboForFonts.Size = new Size(150, 30);
            CellComboForFonts.Location = new Point(220, 10);
            CellComboForFonts.ForeColor = Color.Black;
            CellComboForFonts.DropDownHeight = 70;
            CellComboForFonts.DropDownWidth = 150;
            CellComboForFonts.Visible = true;
            fontsLabelCell.Location = new Point(10, 12);
            fontsLabelCell.Width = 200;

            RowComboForFonts.Size = CellComboForFonts.Size;
            RowComboForFonts.Location = new Point(220, 50);
            RowComboForFonts.ForeColor = Color.Black;
            RowComboForFonts.DropDownHeight = 70;
            RowComboForFonts.DropDownWidth = 150;
            RowComboForFonts.Visible = true;
            fontsLabelRow.Location = new Point(10, 52);
            fontsLabelRow.Width = 200;

            GridComboForFonts.Size = CellComboForFonts.Size;
            GridComboForFonts.Location = new Point(220, 90);
            GridComboForFonts.ForeColor = Color.Black;
            GridComboForFonts.DropDownHeight = 70;
            GridComboForFonts.DropDownWidth = 150;
            GridComboForFonts.Visible = true;
            fontsLabelGrid.Location = new Point(10, 92);
            fontsLabelGrid.Width = 200;
        }

        private void AddToSizeComboSizes()
        {
            CellComboForSize.Size = new Size(60, 30);
            CellComboForSize.Location = new Point(220, 10);
            CellComboForSize.ForeColor = Color.Black;
            CellComboForSize.DropDownHeight = 70;
            CellComboForSize.DropDownWidth = 60;
            CellComboForSize.Visible = true;
            sizesLabelCell.Location = new Point(10, 12);
            sizesLabelCell.Width = 200;

            RowComboForSize.Size = new Size(60, 30);
            RowComboForSize.Location = new Point(220, 50);
            RowComboForSize.ForeColor = Color.Black;
            RowComboForSize.DropDownHeight = 70;
            RowComboForSize.DropDownWidth = 60;
            RowComboForSize.Visible = true;
            sizesLabelRow.Location = new Point(10, 52);
            sizesLabelRow.Width = 200;

            GridComboForSize.Size = new Size(60, 30);
            GridComboForSize.Location = new Point(220, 90);
            GridComboForSize.ForeColor = Color.Black;
            GridComboForSize.DropDownHeight = 70;
            GridComboForSize.DropDownWidth = 60;
            GridComboForSize.Visible = true;
            sizesLabelGrid.Location = new Point(10, 92);
            sizesLabelGrid.Width = 200;
        }

        private void CellComboForFontsSelected(object sender, EventArgs e)
        {
            if (CellComboForFonts.Text != null)
            {
                if (currentCell != null)
                {
                    if (!string.IsNullOrEmpty(currentCell.Value.ToString()))
                        currentCell.Style.Font = new Font(CellComboForFonts.Text.ToString(), 10);
                }
                else MessageBox.Show("Empty text cell", "Font Editing Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void RowComboForFontsSelected(object sender, EventArgs e)
        {
            if (RowComboForFonts.Text != null)
                dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].DefaultCellStyle.Font = new Font(RowComboForFonts.Text.ToString(), 10);
        }

        private void GirdComboForFontsSelected(object sender, EventArgs e)
        {
            if (GridComboForFonts.Text != null)
                dataGridView1.Font = new Font(GridComboForFonts.Text.ToString(), 10);
        }

        public void CellComboForSizeSelected(object sender, EventArgs e)
        {
            if (CellComboForSize.Text != null)
            {
                if (currentCell != null)
                {
                    if (!string.IsNullOrEmpty(currentCell.Value.ToString()))
                        currentCell.Style.Font = new Font(CellComboForSize.Text.ToString(), 10);
                }
                else MessageBox.Show("Empty text cell", "Font Editing Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        public void RowComboForSizeSelected(object sender, EventArgs e)
        {
            if (RowComboForSize.Text != null)
                dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].DefaultCellStyle.Font = new Font(RowComboForFonts.Text.ToString(), 10);
        }
        public void GridComboForSizeSelected(object sender, EventArgs e)
        {
            if (GridComboForSize.Text != null)
                dataGridView1.Font = new Font(GridComboForSize.Text.ToString(), 10);
        }

        private void ChangeColorText(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0)
            {
                ColorDialog colorDialog = new ColorDialog();
                colorDialog.AllowFullOpen = false;
                colorDialog.ShowHelp = true;
                colorDialog.Color = currentCell.Style.ForeColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                    currentCell.Style.BackColor = colorDialog.Color;
            }
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        //Project Information----------------------------------------------------------------------------
        private void Menu_Information_Duration(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0)
            {
                double summ = 0;
                foreach (Task t in Entity.project.tasksList)
                    summ += t.Duration;
                MessageBox.Show("Total duration of the project: " + summ + "h", "Project Information: Duration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Menu_Information_Deadline(object sender, EventArgs e)
        {
            DateTime deadline = DateTime.Now;
            if (Entity.project.tasksList.Count > 0)
            {
                deadline = Entity.project.tasksList.OrderBy(a => a.Finish).Reverse().Last().Finish;
                MessageBox.Show("The project deadline: " + deadline.ToString(), "Project Information: Deadline", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Menu_Information_Start(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            if (Entity.project.tasksList.Count > 0)
            {
                start = Entity.project.tasksList.OrderBy(a => a.Start).First().Start;
                MessageBox.Show("Project start: " + start.ToString(), "Project Information: Start", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void GetInfoByPriorityHigh(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) ShowTaskInfoDependingOfPriority("High");
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void GetInfoByPriorityMedium(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) ShowTaskInfoDependingOfPriority("Medium");
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void GetInfoByPriorityLow(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) ShowTaskInfoDependingOfPriority("Low");
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public void ShowTaskInfoDependingOfPriority(string priority)
        {
            if (Entity.project.tasksList.Count > 0)
            {
                StringBuilder sb = new StringBuilder(priority + " priority tasks:");
                foreach (Task t in Entity.project.tasksList)
                    if (t.Priority == priority)
                        sb.Append("\n " + t.Name);
                MessageBox.Show(sb.ToString(), "Project Information by task priority", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        //Ordering --------------------------------------------------
        public void PerformOrdering(string field, bool ascendent)
        {
            if (Entity.project.tasksList.Count > 0)
            {
                switch (field)
                {
                    case "duration":
                        if (ascendent) Entity.project.tasksList.Sort((x, y) => x.Duration.CompareTo(y.Duration));
                        else
                        {
                            Entity.project.tasksList.Sort((x, y) => x.Duration.CompareTo(y.Duration));
                            Entity.project.tasksList.Reverse();
                        }
                        break;
                    case "finish":
                        if (ascendent) Entity.project.tasksList = BubleSort(Entity.project.tasksList, "f", "a");
                        else
                        {
                            Entity.project.tasksList = BubleSort(Entity.project.tasksList, "f", "d");
                        }
                        break;
                    case "start":
                        if (ascendent) Entity.project.tasksList = BubleSort(Entity.project.tasksList, "s", "a");
                        else
                        {
                            Entity.project.tasksList = BubleSort(Entity.project.tasksList, "s", "d");
                        }
                        break;
                    case "completion":
                        if (ascendent)
                            Entity.project.tasksList.Sort((x, y) => (int)x.Complete.CompareTo((int)y.Complete));
                        else
                        {
                            Entity.project.tasksList.Sort((x, y) => (int)x.Complete.CompareTo((int)y.Complete));
                            Entity.project.tasksList.Reverse();
                        }
                        break;
                }
                if (field.Equals("duration") || field.Equals("completion"))
                {
                    dataGridView1.Rows.Clear();
                    List<Task> newList = new List<Task>();
                    for (int i = 0; i < Entity.project.tasksList.Count; i++)
                    {
                        Task t = Entity.project.tasksList[i];
                        t.TaskId = i + 1;
                        newList.Add(t);
                    }
                    Entity.project.tasksList = newList;
                }
                FillGridWithTaskListContent();
            }
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public List<Task> BubleSort(List<Task> taskList, string flag, string orderType)
        {
            List<Task> l = taskList;
            for (int write = 0; write < l.Count; write++)
            {
                for (int sort = 0; sort < l.Count - 1; sort++)
                {
                    bool condition = orderType.Equals("a") ?
                        (flag.Equals("s") ? l[sort].Start > l[sort + 1].Start : l[sort].Finish > l[sort + 1].Finish) :
                        (flag.Equals("s") ? l[sort].Start < l[sort + 1].Start : l[sort].Finish < l[sort + 1].Finish);
                    if (condition)
                    {
                        var temp = l[sort + 1];
                        l[sort + 1] = l[sort];
                        l[sort] = temp;
                    }
                }
            }
            dataGridView1.Rows.Clear();
            List<Task> newList = new List<Task>();
            for (int i = 0; i < l.Count; i++)
            {
                Task t = l[i];
                t.TaskId = i + 1;
                newList.Add(t);
            }
            return newList;
        }

        private void OrderDurationascendentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PerformOrdering("duration", true);
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OrderDurationascendentToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PerformOrdering("duration", false);
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OrederStartascendentToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PerformOrdering("start", true);
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OrederStartdescendentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PerformOrdering("start", false);
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OrderFinishascendentToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PerformOrdering("finish", true);
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OrderFinishdescendentToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PerformOrdering("finish", false);
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OrderCompletionascendentToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PerformOrdering("completion", true);
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OrderCompletiondescendentToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PerformOrdering("completion", false);
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }



        //Grouping & Sorting-----------------------------------------
        private void GroupByStatus(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PerformOrdering("completion", false);
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void PriorityGrouping(string typeGrouping)
        {
            List<Task> newList = new List<Task>();
            List<string> pList = new List<string>();
            switch (typeGrouping)
            {
                case ("all"):
                    pList.Add("High"); pList.Add("Medium"); pList.Add("Low");
                    break;
                case ("high"):
                    pList.Add("High");
                    break;
                case ("medium"):
                    pList.Add("Medium");
                    break;
                case ("low"):
                    pList.Add("Low");
                    break;
            }

            foreach (string priority in pList)
                foreach (Task t in Entity.project.tasksList)
                    if (t.Priority.Equals(priority))
                        newList.Add(t);

            List<Task> newList1 = new List<Task>();
            for (int i = 0; i < newList.Count; i++)
            {
                Task t = newList[i];
                t.TaskId = i + 1;
                newList1.Add(t);
            }
            dataGridView1.Rows.Clear();
            Entity.project.tasksList.Clear();
            Entity.project.tasksList = newList1;
            FillGridWithTaskListContent();
        }
        private void GroupBuPriority(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PriorityGrouping("all");
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SortTaskByPriorityHigh(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PriorityGrouping("high");
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SortTaskByPriorityMedium(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PriorityGrouping("medium");
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SortTaskByPriorityLow(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) PriorityGrouping("low");
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void SortTasksByStatus(string typeStatus)
        {
            List<Task> newList = new List<Task>();
            switch (typeStatus)
            {
                case ("done"):
                    foreach (Task t in Entity.project.tasksList)
                        if (t.Complete == 100)
                            newList.Add(t);
                    break;
                case ("notStart"):
                    foreach (Task t in Entity.project.tasksList)
                        if (t.Complete == 0)
                            newList.Add(t);
                    break;
                case ("inProcess"):
                    foreach (Task t in Entity.project.tasksList)
                        if (t.Complete < 100 && t.Complete > 0)
                            newList.Add(t);
                    break;
            }

            List<Task> newList1 = new List<Task>();
            for (int i = 0; i < newList.Count; i++)
            {
                Task t = newList[i];
                t.TaskId = i + 1;
                newList1.Add(t);
            }
            dataGridView1.Rows.Clear();
            Entity.project.tasksList.Clear();
            Entity.project.tasksList = newList1;
            FillGridWithTaskListContent();
        }

        private void SortNotStartedTask(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) SortTasksByStatus("notStart");
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SortInProcessTasks(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) SortTasksByStatus("inProcess");
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SortDoneTasks(object sender, EventArgs e)
        {
            if (Entity.project.tasksList.Count > 0) SortTasksByStatus("done");
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SortCriticalTasks(object sender, EventArgs e)
        {
            List<Task> newList = new List<Task>();
            foreach (Task t in Entity.project.tasksList)
            {
                if (t.Finish < DateTime.Now)
                    if (t.Complete < 100)
                        newList.Add(t);
            }
            if (newList.Count > 0)
            {
                List<Task> newList1 = new List<Task>();
                for (int i = 0; i < newList.Count; i++)
                {
                    Task t = newList[i];
                    t.TaskId = i + 1;
                    newList1.Add(t);
                }
                dataGridView1.Rows.Clear();
                Entity.project.tasksList.Clear();
                Entity.project.tasksList = newList1;
                FillGridWithTaskListContent();
            }
            else MessageBox.Show("There is no late tasks", "Project Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Menu_Saveas_Newproject(object sender, EventArgs e)
        {
            if (Entity.Acces.IsUserLoggedIn)
            {
                if (Entity.project.tasksList.Count > 0)
                {
                    NewProject saveasNewProjectForm = new NewProject(this);
                    saveasNewProjectForm.CreateSaveAsNewprojectForm(Entity.project);
                }
                else MessageBox.Show("Fail! Empt project", "Save alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else MessageBox.Show("Fail! You are not logged in. Log in then try again to save this project.", "Save alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);



        }

        private void Menu_SaveAs_XML(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName,
                Filter = "XML Files (*.xml)|*.xml"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Entity.project.xmlPath = saveFileDialog.FileName;
                string message = Entity.project.SerializeTasks();
                MessageBox.Show(message, "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void CSVFunctionHelper()
        {
            if (Entity.project.tasksList.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    InitialDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName,
                    Filter = "CSV Files (*.csv)|*.csv"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string CSVFilePath = saveFileDialog.FileName;
                    if (this.dataGridView1.ToCSV(CSVFilePath))
                        MessageBox.Show("File successfully converted to CSV!", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else MessageBox.Show("Fail! The tasks list is empty", "Reporting", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
        private void SaveAscSVFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CSVFunctionHelper();
        }

        private void Menu_Account_Log_out_in(object sender, EventArgs e)
        {
            if (Entity.Acces.IsUserLoggedIn)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Log out", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    Entity.Acces.IsUserLoggedIn = false;
                    Entity.user = null;
                    Entity.account = null;
                    logInToolStripMenuItem.Text = "Log In";
                }
            }
            else
            {
                var accesToProjectsDataBase = new AccesForms(this);
                accesToProjectsDataBase.CreateAccesForm();
                logInToolStripMenuItem.Text = "Log out";
            }
        }

        private void Menu_Account_Regiser(object sender, EventArgs e)
        {
            var accesToProjectsDataBase = new AccesForms(this);
            accesToProjectsDataBase.CreateAccesForm();
        }

        private void Menu_Account_Info(object sender, EventArgs e)
        {
            if (Entity.Acces.IsUserLoggedIn && Entity.user != null && Entity.account != null)
            {
                var userInfoForm = new AccesForms(this);
                userInfoForm.CreateAccountInfoDialog();
            }
            else
                MessageBox.Show("There is no user logged in!", "Info account error alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
    } 
}

