﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatApp.Classes.MyFunction;
using DevExpress.XtraEditors;
using ChatApp.Classes;
using Data.Data;
using TakeAway;

namespace ChatApp.Forms
{
    public partial class frmLogIn : DevExpress.XtraEditors.XtraForm
    {
        ErrorProvider er1 = new ErrorProvider();
        ErrorProvider er = new ErrorProvider();
        public frmLogIn()
        {
            InitializeComponent();
            using (DataContext con = new DataContext())
            {
                con.Database.CreateIfNotExists();
                var IsUser = con?.SenderUsers?.SingleOrDefault(s => s.Username == "admin");
                if (IsUser == null)
                {
                    CallUser soso = new CallUser() { Username = "admin", Password = "admin" };
                    con.CallUsers.Add(soso);
                    con.SaveChanges();
                }
            }

        }

        private void txtEmailLogIN_Validating(object sender, CancelEventArgs e)
        {
            
            if (string.IsNullOrEmpty(txtEmailLogIN.Text))
            {
                e.Cancel = true;
                txtEmailLogIN.Focus();
                er.SetError(txtEmailLogIN, "Please Enter Email");
            }
            else
            {
                e.Cancel = false;
               er.SetError(txtEmailLogIN, null);

            }
        }

        private void txtEmailLogIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Funcs.validateEmail(e);
        }

        private void txtPasswordLogIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            Funcs.checkSpace(e);
        }

        private void txtPasswordLogIN_Validating(object sender, CancelEventArgs e)
        {
           
            if (txtPasswordLogIN.Text.Length < 8)
            {
                e.Cancel = true;
                txtPasswordLogIN.Focus();
               er1.SetError(txtPasswordLogIN, "The Password must be greater than Or Equal 8 letters");
            }
            else er1.SetError(txtPasswordLogIN, null);
        }

        private void btnLogIN_Click(object sender, EventArgs e)
        {

            string emailUser = txtEmailLogIN.Text;
            using (var mes = new DataContext())
            {
                var logInUser = mes.CallUsers.Where(s => s.Username.ToLower() == emailUser.ToLower()).FirstOrDefault();
                if (logInUser == null)
                {
                    MessageBox.Show("UsernameIncorrect");
                }
                else
                {
                    if (logInUser.Password == txtPasswordLogIN.Text)
                    {
                        MessageBox.Show("Welcome");
                        //var contextChat = new DataContext();
                        //LogUser currentLog = new LogUser();
                        //currentLog.ID = Guid.NewGuid();
                        //currentLog.IsLogIn = true;
                        //currentLog.IsLogOut = false;
                        //currentLog.CreationDate = DateTime.UtcNow;
                        //currentLog.UpdatedDate = DateTime.UtcNow;
                        //currentLog.userId = logInUser.ID;
                        //contextChat.Entry(currentLog).State = System.Data.Entity.EntityState.Added;
                        //contextChat.SaveChanges();
                        //// update status User 
                        //statusUser stUser = mes.StatusUser.Where(s => s.UserID == logInUser.ID).FirstOrDefault();
                        //if(stUser!=null)
                        //{
                        //    if(!stUser.IsOnline)
                        //    {
                        //        stUser.IsOnline = true;
                        //        stUser.UpdateDate = DateTime.UtcNow;
                        //        mes.Entry(stUser).State = System.Data.Entity.EntityState.Modified;
                        //    }
                        //}
                     
                        //mes.SaveChanges();
                        // end update Status User 
                        this.Hide();
                        Form1 usertChat = new Form1(logInUser);
                        usertChat.ShowDialog();
                        this.Close();
                        
                    }
                    else
                    {
                        MessageBox.Show("Password InCorrect");
                    }
                }
            }
        }

        private void btnSingUP_Click(object sender, EventArgs e)
        {
            //er1.SetError(txtPasswordLogIN, null);
            //er.SetError(txtEmailLogIN, null);
            //this.Hide();
            //frmSingUp_ singup = new frmSingUp_();
            //singup.ShowDialog();
            //this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}