
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DAMS_Proiect.Entity;
using static DAMS_Proiect.Convertor;
using System.Drawing;
using System.IO;

namespace DAMS_Proiect
{
    public class NewProject
    {
        private Form MainForm;
        public NewProject(Form form)
        {
            MainForm = form;
        }

        public void CreateSaveAsNewprojectForm(Project project)
        {
            Form saveasNewForm = new Form
            {
                Size = new Size(500, 270),
                Location = new Point(900, 500),
                Text = "Save as: new project",
                Name = "newProject"
            };

            var saveasImage = new PictureBox
            {
                ImageLocation = @Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\DAMS_Proiect\\Resources\\saveasnewproject.png",
                Location = new Point(50, 30),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            Label ProjectNameLabel = new Label
            {
                Location = new Point(220, 50),
                Size = new Size(300, 25),
                Text = "Enter a  project name:"
            }; saveasNewForm.Controls.Add(ProjectNameLabel);
            TextBox ProjectNameTextBox = new TextBox
            {
                Location = new Point(220, 90),
                Size = new Size(150, 40),
            }; saveasNewForm.Controls.Add(ProjectNameTextBox);
            Label ProjectNameErrLabel = new Label()
            {
                Location = new Point(375, 90),
                Size = new Size(25, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 7, FontStyle.Bold),
                Text = ".*",
                Visible = false
            }; saveasNewForm.Controls.Add(ProjectNameErrLabel);


            var saveButton = new GelButton
            {
                Location = new Point(220, 150),
                Size = new Size(150, 30),
                Text = "Save to DB"
            };

            saveButton.Click += (s, ev) =>
            {
                if (string.IsNullOrEmpty(ProjectNameTextBox.Text))
                {
                    MessageBox.Show("Enter a specific name for this project!", "Save alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    if(new CreateData().CreateAndSaveNewProject( ProjectNameTextBox.Text))
                        MessageBox.Show("Project succesfuly saved in BD, named: " + ProjectNameTextBox.Text, "Save succes", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                CloseAccesForms();
            };

            saveasNewForm.Controls.Add(saveButton);
            saveasNewForm.Controls.Add(saveasImage);

            MainForm.AddOwnedForm(saveasNewForm);
            saveasNewForm.ShowDialog();
        }


        public void CloseAccesForms()
        {
            var formsList = MainForm.OwnedForms;
            var saveasNewForm = formsList.ToList().Find(form => form.Name == "newProject");
            if (saveasNewForm != null) saveasNewForm.Close();
        }
    }
}
