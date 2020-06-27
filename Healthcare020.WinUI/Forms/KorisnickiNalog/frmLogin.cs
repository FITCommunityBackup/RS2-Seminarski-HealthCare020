﻿using Healthcare020.WinUI.Forms.AdminDashboard;
using Healthcare020.WinUI.Forms.RadnikDashboard.DoktorDashboard;
using Healthcare020.WinUI.Helpers;
using Healthcare020.WinUI.Properties;
using HealthCare020.Core.Enums;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Healthcare020.WinUI.Forms.KorisnickiNalog
{
    public partial class frmLogin : Form
    {
        private static frmLogin _instance;

        private frmLogin()
        {
            InitializeComponent();
            txtUsername.Select();
        }

        public static frmLogin Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new frmLogin();
                }

                return _instance;
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            pnlBody.BringToFront();
        }

        private void frmLogin_Resize(object sender, EventArgs e)
        {
            pnlTop.Size = new Size(Width, this.Height / 2);
        }

        private void frmLogin_Shown(object sender, EventArgs e)
        {
        }

        private async void Login()
        {
            if (!ValidateInput())
            {
                return;
            }

            var username = txtUsername.Text;
            var password = txtPassword.Text;

            if (await Auth.AuthenticateWithPassword(username, password))
            {
                Form formToOpen = null;

                if (Auth.Role == RoleType.Administrator)
                    formToOpen = frmStartMenuAdmin.Instance;
                else if (Auth.Role == RoleType.Doktor || Auth.Role == RoleType.MedicinskiTehnicar)
                    formToOpen = frmDoktorMainDashboard.Instance;
                if (formToOpen == null)
                    return;

                formToOpen.OpenAsChildOfControl(Parent);
                this.Close();
            }
            else
            {
                Errors.SetError(txtPassword, Resources.WrongUsernameOrPasswordMessage);
            }
        }

        private void txtPassword_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                e.Handled = true;
                Login();
            }
        }

        private bool ValidateInput()
        {
            if (!txtUsername.ValidTextInput(Errors))
                return false;

            if (!txtPassword.ValidTextInput(Errors, Validation.TextInputType.Mixed))
                return false;

            Errors.Clear();
            return true;
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            Login();
        }

        private void frmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}