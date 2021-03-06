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
    public class AccesForms
    {
        private Form MainForm;
        private DataGridView dataGridView;
        public AccesForms(Form form, DataGridView dg)
        {
            MainForm = form;
            dataGridView = dg;
        }
        public void CreateAccesForm()
        {
            Form accesForm = new Form
            {
                Size = new Size(575, 325),
                Location = new Point(1000, 750),
                Text = "Get Acces",
                Name = "acces"
            };

            var register = new PictureBox
            {
                ImageLocation = @Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\DAMS_Proiect\\Resources\\register.png",
                Location = new Point(100, 30),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            var login = new PictureBox
            {
                ImageLocation = @Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\DAMS_Proiect\\Resources\\login.png",
                Location = new Point(300, 30),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            var registerButton = new GelButton
            {
                Location = new Point(100, 200),
                Size = new Size(150, 30),
                Text = "Register"
            };
            registerButton.Click += RegisterUserChoose;

            var loginButton = new GelButton
            {
                Location = new Point(300, 200),
                Size = new Size(150, 30),
                Text = "Login"
            };
            loginButton.Click += LoginUserChoose;

            accesForm.Controls.Add(register);
            accesForm.Controls.Add(registerButton);
            accesForm.Controls.Add(login);
            accesForm.Controls.Add(loginButton);

            MainForm.AddOwnedForm(accesForm);
            accesForm.ShowDialog();
        }

        public static AccesType currentAccesType;
        public enum AccesType
        {
            Login,
            Register
        };

        public static UserType currentUserType = UserType.Simple;
        public enum UserType
        {
            Simple,
            Leader
        };

        private void RegisterUserChoose(object sender, EventArgs e)
        {
            currentAccesType = AccesType.Register;
            CreateAccesFormForUserTypeChoosing();
        }

        private void LoginUserChoose(object sender, EventArgs e)
        {
            currentAccesType = AccesType.Login;
            CreateAccesFormForUserTypeChoosing();
        }

        public void CreateAccesFormForUserTypeChoosing()
        {
            Form userTypeForm = new Form
            {
                Size = new Size(575, 325),
                Location = new Point(1000, 750),
                Text = "Choose User Type",
                Name = "userTypeForm"
            };

            var leader = new PictureBox
            {
                ImageLocation = @Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\DAMS_Proiect\\Resources\\liderr.png",
                Location = new Point(103, 30),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            var simpleUser = new PictureBox
            {
                ImageLocation = @Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\DAMS_Proiect\\Resources\\simpleuset.png",
                Location = new Point(300, 30),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            var leaderButton = new GelButton
            {
                Location = new Point(103, 200),
                Size = new Size(150, 30),
                Text = "Leader"
            };


            var simpleUserButton = new GelButton
            {
                Location = new Point(300, 200),
                Size = new Size(150, 30),
                Text = "Simple User"
            };

            leaderButton.Click += LeaderTypeClick;
            simpleUserButton.Click += UserTypeClick;

            userTypeForm.Controls.Add(leader);
            userTypeForm.Controls.Add(simpleUser);
            userTypeForm.Controls.Add(leaderButton);
            userTypeForm.Controls.Add(simpleUserButton);

            MainForm.AddOwnedForm(userTypeForm);
            userTypeForm.ShowDialog();
        }


        private void LeaderTypeClick(object sender, EventArgs e)
        {
            currentUserType = UserType.Leader;
            if (currentAccesType.Equals(AccesType.Login))
            {
                Login();
            }
            else
            {
                Register();
            }
        }
        private void UserTypeClick(object sender, EventArgs e)
        {
            currentUserType = UserType.Simple;
            if (currentAccesType.Equals(AccesType.Login))
            {
                Login();
            }
            else
            {
                Register();
            }
        }


        private void Login()
        {
            Form loginForm = new Form
            {
                Size = new Size(650, 325),
                Location = new Point(1000, 750),
                Text = "Login",
                Name = "login"
            };

            var loginPict = new PictureBox
            {
                ImageLocation = @Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\DAMS_Proiect\\Resources\\login.png",
                Location = new Point(95, 60),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            TextBox nameTextBox = new TextBox
            {
                Location = new Point(385, 65),
                Size = new Size(150, 40)
            };

            TextBox pwdTextBox = new TextBox
            {
                Location = new Point(385, 115),
                Size = new Size(150, 40),
                PasswordChar = '*'
             };

            Label nameLabel = new Label
            {
                Location = new Point(300, 65),
                Size = new Size(75, 40),
                Text = "Name:"
            };

            Label pwdLabel = new Label
            {
                Location = new Point(300, 115),
                Size = new Size(75, 40),
                Text = "Password:"
            };

            Label errorLabelForName = new Label()
            {
                Location = new Point(525, 65),
                Size = new Size(30, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 7, FontStyle.Bold),
                Text = ".*",
                Visible = false
             };

            Label errorLabelForPassword = new Label()
            {
                Location = new Point(525, 115),
                Size = new Size(30, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 7, FontStyle.Bold),
                Text = ".*",
                Visible = false
            };

            var loginButton = new GelButton
            {
                Size = new Size(150, 30),
                Text = "Login"
            };

            Label errorLabelForLeader = new Label();
            Label leaderKeyLabel = new Label();
            TextBox leaderKeyTextBox = new TextBox();
            if (currentUserType.Equals(UserType.Leader))
            {
                leaderKeyTextBox.Location = new Point(385, 170);
                leaderKeyTextBox.Size = new Size(150, 40);

                leaderKeyLabel.Location = new Point(300, 170);
                leaderKeyLabel.Size = new Size(75, 40);
                leaderKeyLabel.Text = "LeadKey:";

                errorLabelForLeader.Location = new Point(525, 170);
                errorLabelForLeader.Size = new Size(100, 30);
                errorLabelForLeader.ForeColor = Color.Red;
                errorLabelForLeader.Font = new Font("Arial", 7, FontStyle.Bold);
                errorLabelForLeader.Text = "Wrong Key";
                errorLabelForLeader.Visible = false;

                loginForm.Controls.Add(errorLabelForLeader);
                loginForm.Controls.Add(leaderKeyLabel);
                loginForm.Controls.Add(leaderKeyTextBox);

                leaderKeyTextBox.Visible = leaderKeyLabel.Visible = true;

                loginButton.Location = new Point(385, 235);
            }
            else
            {
                leaderKeyTextBox.Visible = leaderKeyLabel.Visible = false;
                loginButton.Location = new Point(385, 170);
            }

            loginButton.Click += (s, ev) =>
            {
                errorLabelForPassword.Visible = string.IsNullOrEmpty(pwdTextBox.Text);
                errorLabelForName.Visible = string.IsNullOrEmpty(nameTextBox.Text);
                errorLabelForLeader.Visible =(string.IsNullOrEmpty(leaderKeyTextBox.Text) || !leaderKeyTextBox.Text.Equals(LeaderKey)) && currentUserType.Equals(UserType.Leader);

                if (string.IsNullOrEmpty(pwdTextBox.Text) || string.IsNullOrEmpty(nameTextBox.Text))
                    return;

                else if (errorLabelForLeader.Visible)
                    return;
                else
                {
                    GetData getData = new GetData();
                    if (!getData.CheckUserNameExistence(nameTextBox.Text))
                    {
                        MessageBox.Show("No existing user name. Try again!", "Log In", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (!getData.ValidatePassword(pwdTextBox.Text, nameTextBox.Text))
                    {
                        MessageBox.Show("Wrong password. Try again!", "Log In", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    Acces.IsUserLoggedIn = true;
                    Acces.IsLeaderCurrentLoggedUser = (currentUserType.Equals(UserType.Leader));
                    MessageBox.Show("Log in Succesful" + (currentUserType.Equals(UserType.Leader) ? " as a Leader! " : "! ") +" Now you can acess your projects from Data Base", "Log In", MessageBoxButtons.OK, MessageBoxIcon.Information);                   
                }
                CloseAccesForms();
            };


            loginForm.Controls.Add(errorLabelForName);
            loginForm.Controls.Add(errorLabelForPassword);
            loginForm.Controls.Add(loginPict);
            loginForm.Controls.Add(nameTextBox);
            loginForm.Controls.Add(nameLabel);
            loginForm.Controls.Add(pwdLabel);
            loginForm.Controls.Add(pwdTextBox);
            loginForm.Controls.Add(loginButton);

            MainForm.AddOwnedForm(loginForm);
            loginForm.ShowDialog();
        }

        private void Register()
        {
            Form registerForm = new Form
            {
                Size = new Size(900, 400),
                Location = new Point(1000, 750),
                Text = "Register",
                Name = "register"
            };

            var registerPict = new PictureBox
            {
                ImageLocation = @Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\DAMS_Proiect\\Resources\\register.png",
                Location = new Point(70, 90),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.AutoSize
            }; registerForm.Controls.Add(registerPict);


            Label nameLabel = new Label
            {
                Location = new Point(275, 65),
                Size = new Size(75, 40),
                Text = "Name:"
            }; registerForm.Controls.Add(nameLabel);
            TextBox nameTextBox = new TextBox
            {
                Location = new Point(370, 65),
                Size = new Size(150, 40)
            }; registerForm.Controls.Add(nameTextBox);
            Label errorLabelForName = new Label()
            {
                Location = new Point(525, 65),
                Size = new Size(25, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 7, FontStyle.Bold),
                Text = ".*",
                Visible = false
            }; registerForm.Controls.Add(errorLabelForName);



            Label userNameLabel = new Label
            {
                Location = new Point(550, 65),
                Size = new Size(75, 40),
                Text = "UName:"
            }; registerForm.Controls.Add(userNameLabel);
            TextBox userNameTextBox = new TextBox
            {
                Location = new Point(645, 65),
                Size = new Size(150, 40)
            }; registerForm.Controls.Add(userNameTextBox);
            Label errorLabelForUserName = new Label()
            {
                Location = new Point(800, 65),
                Size = new Size(25, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 7, FontStyle.Bold),
                Text = ".*",
                Visible = false
            }; registerForm.Controls.Add(errorLabelForUserName);


            Label addressLabel = new Label
            {
                Location = new Point(275, 165),
                Size = new Size(75, 40),
                Text = "Address:"
            }; registerForm.Controls.Add(addressLabel);
            TextBox addressTextBox = new TextBox
            {
                Location = new Point(370, 165),
                Size = new Size(150, 40)
            }; registerForm.Controls.Add(addressTextBox);
            Label errorLabelForAaddress = new Label()
            {
                Location = new Point(525, 165),
                Size = new Size(25, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 7, FontStyle.Bold),
                Text = ".*",
                Visible = false
            }; registerForm.Controls.Add(errorLabelForAaddress);


            Label phoneLabel = new Label
            {
                Location = new Point(550, 165),
                Size = new Size(75, 40),
                Text = "Phone:"
            }; registerForm.Controls.Add(phoneLabel);
            TextBox phoneTextBox = new TextBox
            {
                Location = new Point(645, 165),
                Size = new Size(150, 40)
            }; registerForm.Controls.Add(phoneTextBox);
            Label errorLabelForUserPhone = new Label()
            {
                Location = new Point(800, 165),
                Size = new Size(25, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 7, FontStyle.Bold),
                Text = ".*",
                Visible = false
            }; registerForm.Controls.Add(errorLabelForUserPhone);


            Label pwdLabel = new Label
            {
                Location = new Point(275, 115),
                Size = new Size(75, 40),
                Text = "Password:"
            }; registerForm.Controls.Add(pwdLabel);
            TextBox pwdTextBox = new TextBox
            {
                Location = new Point(370, 115),
                Size = new Size(150, 40),
                PasswordChar = '*'
            }; registerForm.Controls.Add(pwdTextBox);
            Label errorLabelForPassword = new Label()
            {
                Location = new Point(525, 115),
                Size = new Size(25, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 7, FontStyle.Bold),
                Text = ".*",
                Visible = false
            }; registerForm.Controls.Add(errorLabelForPassword);


            Label repeadPwdLabel = new Label
            {
                Location = new Point(550, 115),
                Size = new Size(75, 40),
                Text = "Repeat:"
            }; registerForm.Controls.Add(repeadPwdLabel);
            TextBox repeatPwdTextBox = new TextBox
            {
                Location = new Point(645, 115),
                Size = new Size(150, 40),
                PasswordChar = '*'
            }; registerForm.Controls.Add(repeatPwdTextBox);
            Label errorLabelForRepeatPassword = new Label()
            {
                Location = new Point(800, 115),
                Size = new Size(55, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 7, FontStyle.Bold),
                Text = ".*",
                Visible = false
            }; registerForm.Controls.Add(errorLabelForRepeatPassword);



            Label mailLabel = new Label
            {
                Location = new Point(275, 215),
                Size = new Size(75, 40),
                Text = "Email:"
            }; registerForm.Controls.Add(mailLabel);
            TextBox mailTextBox = new TextBox
            {
                Location = new Point(370, 215),
                Size = new Size(150, 40),
            }; registerForm.Controls.Add(mailTextBox);
            Label errorLabelForMail = new Label()
            {
                Location = new Point(525, 215),
                Size = new Size(25, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 7, FontStyle.Bold),
                Text = ".*",
                Visible = false
            }; registerForm.Controls.Add(errorLabelForMail);



            Label repeatPwdLabel = new Label
            {
                Location = new Point(550, 215),
                Size = new Size(100, 40),
                Text = "Departament"
            }; registerForm.Controls.Add(repeatPwdLabel);

            repeatPwdLabel.Text = (currentUserType.Equals(UserType.Leader)) ? "Teams" :"Departament";
            ComboBox resourcesComboBox = new ComboBox()
            {
                Location = new Point(650, 215),
                Size = new Size(140, 40),
                DropDownHeight = 70,
                FormattingEnabled = true,
                TabIndex = 1,
                Margin = new Padding(3, 2, 3, 2)
            };

            if (currentUserType.Equals(UserType.Leader))
            {
                GetData getData = new GetData();
                string[] teams = getData.GedTeamsFromDataBase();
                resourcesComboBox.Items.AddRange(teams);
            }
            else
            {
                resourcesComboBox.Items.AddRange(new string[] { "Marketing", "Design", "FrontEnd", "BackEnd", "HR", "Finances", "Accounting", "BDAdmin", "Security", "Tester", "Director" });
            }
            registerForm.Controls.Add(resourcesComboBox);

                Label errorLabelForResourcesCombox = new Label()
                {
                    Location = new Point(800, 215),
                    Size = new Size(25, 30),
                    ForeColor = Color.Red,
                    Font = new Font("Arial", 7, FontStyle.Bold),
                    Text = ".*",
                    Visible = false
                }; registerForm.Controls.Add(errorLabelForResourcesCombox);
            

            var registerButton = new GelButton
            {
                Size = new Size(150, 30),
                Text = "Register"
            }; registerForm.Controls.Add(registerButton);



            Label leaderKeyLabel = new Label();
            TextBox leaderKeyTxxtBox = new TextBox();
            Label errorLabelLeaderKey = new Label();
            registerForm.Controls.Add(leaderKeyLabel);
            registerForm.Controls.Add(leaderKeyTxxtBox);
            registerForm.Controls.Add(errorLabelLeaderKey);

            if (currentUserType.Equals(UserType.Leader))
            {
                leaderKeyLabel.Location = new Point(275, 265);
                leaderKeyLabel.Size = new Size(75, 40);
                leaderKeyLabel.Text = "LeadKey:";

                leaderKeyTxxtBox.Location = new Point(370, 265);
                leaderKeyTxxtBox.Size = new Size(150, 40);

                errorLabelLeaderKey.Location = new Point(525, 265);
                errorLabelLeaderKey.Size = new Size(25, 30);
                errorLabelLeaderKey.ForeColor = Color.Red;
                errorLabelLeaderKey.Font = new Font("Arial", 7, FontStyle.Bold);
                errorLabelLeaderKey.Text = ".*";
                errorLabelLeaderKey.Visible = false;
                leaderKeyLabel.Visible = leaderKeyTxxtBox.Visible = true;

                registerButton.Location = new Point(650, 265);
            }
            else
            {
                leaderKeyLabel.Visible = leaderKeyTxxtBox.Visible = false;
                registerButton.Location = new Point(370, 265);
            }

            registerButton.Click += (s, ev) =>
            {
                errorLabelForName.Visible = string.IsNullOrEmpty(nameTextBox.Text);
                errorLabelForUserName.Visible = string.IsNullOrEmpty(userNameTextBox.Text);
                errorLabelForPassword.Visible = string.IsNullOrEmpty(pwdTextBox.Text);
                errorLabelForRepeatPassword.Visible = string.IsNullOrEmpty(repeatPwdTextBox.Text); errorLabelForRepeatPassword.Text = ".*";
                errorLabelForMail.Visible = string.IsNullOrEmpty(mailTextBox.Text);
                errorLabelForAaddress.Visible = string.IsNullOrEmpty(addressTextBox.Text);
                errorLabelForUserPhone.Visible = string.IsNullOrEmpty(phoneTextBox.Text);
                errorLabelForResourcesCombox.Visible = string.IsNullOrEmpty(resourcesComboBox.Text);
                errorLabelLeaderKey.Visible = currentUserType.Equals(UserType.Leader) && string.IsNullOrEmpty(leaderKeyTxxtBox.Text);


                if (string.IsNullOrEmpty(pwdTextBox.Text) || string.IsNullOrEmpty(nameTextBox.Text) ||
                    string.IsNullOrEmpty(resourcesComboBox.Text) || string.IsNullOrEmpty(phoneTextBox.Text) ||
                    string.IsNullOrEmpty(mailTextBox.Text) || string.IsNullOrEmpty(repeatPwdTextBox.Text) ||
                    string.IsNullOrEmpty(userNameTextBox.Text) || string.IsNullOrEmpty(userNameTextBox.Text))
                    return;

                if (errorLabelLeaderKey.Visible) return;

                else if (pwdTextBox.Text.Length < 5)
                {
                    MessageBox.Show("The password must contain at least 6 characters. Try again!", "Register", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else if (!(pwdTextBox.Text.Equals(repeatPwdTextBox.Text)))
                {
                    errorLabelForRepeatPassword.Text = "No Match";
                    errorLabelForRepeatPassword.Visible = true;
                    return;
                }
                else if (!leaderKeyTxxtBox.Text.Equals(Entity.LeaderKey) && currentUserType.Equals(UserType.Leader))
                {
                    errorLabelLeaderKey.Text = "Wrong-K";
                    errorLabelLeaderKey.Visible = true;
                    return;
                }
                else
                {
                    GetData getData = new GetData();
                    if (getData.CheckUserNameExistence(nameTextBox.Text))
                    {
                        MessageBox.Show("Such username already exists. Try another one!", "Register", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    CreateData createData = new CreateData();
                    if (createData.RegisterUser(nameTextBox.Text, userNameTextBox.Text, pwdTextBox.Text, mailTextBox.Text, addressTextBox.Text, phoneTextBox.Text, resourcesComboBox.Text, (currentUserType.Equals(UserType.Leader))))
                        MessageBox.Show("Register Succesful" + (currentUserType.Equals(UserType.Leader) ? " as Leader" : "") +"! \nPlease, login for enable acess to your projects from Data Base", "Register", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Upss. Something went wrong. Try again!", "Register", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
                CloseAccesForms();
            };

            MainForm.AddOwnedForm(registerForm);
            registerForm.ShowDialog();
        }



        public void CloseAccesForms()
        {
            var formsList = MainForm.OwnedForms;
            var accesForm = formsList.ToList().Find(form => form.Name == "acces");
            var login = formsList.ToList().Find(form => form.Name == "login");
            var register = formsList.ToList().Find(form => form.Name == "register");
            var userTypeForm = formsList.ToList().Find(form => form.Name == "userTypeForm");

            if (accesForm != null) accesForm.Close();
            if (login != null) login.Close();
            if (register != null) register.Close();
            if (userTypeForm != null) userTypeForm.Close();

        }



        public void DisplayAndProposeForSelectProjectfromDB( DataGridView dataGridView)
        {
            Form projectsListForm = new Form
            {
                Size = new Size(500, 270),
                Location = new Point(500, 300),
                Text = "Open Project",
                Name = "projectsView"
            };

            var projectImage = new PictureBox
            {
                ImageLocation = @Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\DAMS_Proiect\\Resources\\project.png",
                Location = new Point(50, 30),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.AutoSize
            }; projectsListForm.Controls.Add(projectImage);

            ComboBox projectListComboBox = new ComboBox()
            {
                Location = new Point(220, 85),
                Size = new Size(150, 40),
                DropDownHeight = 60,
                FormattingEnabled = true,
                TabIndex = 1,
                Margin = new Padding(3, 2, 3, 2)
            };

            Dictionary<int, string> projects = new GetData().GetUserProjectList();
            foreach (var item in projects)
                projectListComboBox.Items.Add(item.Value);           
            projectsListForm.Controls.Add(projectListComboBox);


            Label proposeToSelect = new Label
            {
                Location = new Point(220, 50),
                Size = new Size(300, 40),
                Text = "Choose one from your projects list:"
            }; projectsListForm.Controls.Add(proposeToSelect);

            var selectProjectButton = new GelButton
            {
                Location = new Point(220, 150),
                Size = new Size(150, 30),
                Text = "Select"
            }; projectsListForm.Controls.Add(selectProjectButton);


            selectProjectButton.Click += (s, ev) =>
            {
                if (string.IsNullOrEmpty(projectListComboBox.Text))
                {
                    MessageBox.Show("Select a project for opening!", "Open", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    foreach (var item in projects)
                        if (item.Value.Equals(projectListComboBox.Text))
                        {
                            if (Entity.project == null) Entity.project = new Project();
                            Entity.project.Id = item.Key;
                        }

                }
                projectsListForm.Close();
                GetData getData = new GetData();
                dataGridView = getData.OpenProjectFromDB(dataGridView);
            };
             MainForm.AddOwnedForm(projectsListForm);
             projectsListForm.ShowDialog();
        }


        public void CreateAccountInfoDialog()
        {
            Form userInfoForm = new Form
            {
                Size = new Size(550, 350),
                Location = new Point(600, 400),
                Text = "User Account Information",
                Name = "info"
            };

            var infoimg = new PictureBox
            {
                ImageLocation = @Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\DAMS_Proiect\\Resources\\account.png",
                Location = new Point(50, 75),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.AutoSize
            }; userInfoForm.Controls.Add(infoimg);


            Label accountName = new Label
            {
                Location = new Point(250, 75),
                Size = new Size(100, 25),
                Text = "Account:"
            }; userInfoForm.Controls.Add(accountName);

            Label accountName1 = new Label
            {
                Location = new Point(350, 75),
                Size = new Size(100, 25),
                Text = Entity.account.UserName
            }; userInfoForm.Controls.Add(accountName1);


            Label userName = new Label
            {
                Location = new Point(250, 100),
                Size = new Size(100, 25),
                Text = "Name:"
            }; userInfoForm.Controls.Add(userName);

            Label userName1 = new Label
            {
                Location = new Point(350, 100),
                Size = new Size(100, 25),
                Text = Entity.user.Name
            }; userInfoForm.Controls.Add(userName1);


            Label email = new Label
            {
                Location = new Point(250, 125),
                Size = new Size(100, 25),
                Text = "Email:"
            }; userInfoForm.Controls.Add(email);

            Label email1 = new Label
            {
                Location = new Point(350, 125),
                Size = new Size(100, 25),
                Text = Entity.user.Email
            }; userInfoForm.Controls.Add(email1);



            Label phone = new Label
            {
                Location = new Point(250, 150),
                Size = new Size(100, 25),
                Text = "Phone:"
            }; userInfoForm.Controls.Add(phone);

            Label phone1 = new Label
            {
                Location = new Point(350, 150),
                Size = new Size(100, 25),
                Text = Entity.user.PhoneNumber
            }; userInfoForm.Controls.Add(phone1);



            Label Address = new Label
            {
                Location = new Point(250, 175),
                Size = new Size(100, 25),
                Text = "Address:"
            }; userInfoForm.Controls.Add(Address);

            Label Address1 = new Label
            {
                Location = new Point(350, 175),
                Size = new Size(100, 25),
                Text = Entity.user.Addres
            }; userInfoForm.Controls.Add(Address1);



            Label departament = new Label
            {
                Location = new Point(250, 200),
                Size = new Size(100, 25),
                Text = "Departament:"
            }; userInfoForm.Controls.Add(departament);

            Label departament1 = new Label
            {
                Location = new Point(350, 200),
                Size = new Size(100, 25),
                Text = Entity.user.UserDepartamanet.ToString()
            }; userInfoForm.Controls.Add(departament1);


            Label teams = new Label
            {
                Location = new Point(250, 225),
                Size = new Size(100, 25),
                Text = "Teams:"
            }; userInfoForm.Controls.Add(teams);

            Label teams1 = new Label
            {
                Location = new Point(350, 225),
                Size = new Size(100, 25),
                Text = "---"
            }; userInfoForm.Controls.Add(teams1);

            Label isleader = new Label
            {
                Location = new Point(250, 250),
                Size = new Size(100, 25),
                Text = "Leader:"
            }; userInfoForm.Controls.Add(isleader);

            Label isleader1 = new Label
            {
                Location = new Point(350, 250),
                Size = new Size(100, 25),
                Text = "No"
            }; userInfoForm.Controls.Add(isleader1);

            if (Entity.Acces.IsUserLoggedIn && Entity.Acces.IsLeaderCurrentLoggedUser)
            {
                teams1.Text = new GetData().GetTeamID().ToString();
                isleader1.Text = "Yes";
            }

            MainForm.AddOwnedForm(userInfoForm);
            userInfoForm.ShowDialog();
        }
    }
}

