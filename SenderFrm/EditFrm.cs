﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Data.Data;
using Data.Enums;

namespace SenderFrm
{
    public delegate void UpGrid(Order o);
    public partial class EditFrm : DevExpress.XtraEditors.XtraForm
    {
        public static event UpGrid UpdateGrid;
        Order MainOrder;
        Guid SenderUserID;
        public EditFrm(Guid OrderID,Guid? UserID)
        {
            InitializeComponent();
            SenderUserID =(Guid) UserID;
         using ( DataContext coco = new DataContext())
            {
                MainOrder = coco?.Orders?.SingleOrDefault(s => s.ID == OrderID);
                DetailsLbl.Text = MainOrder.Details + ": تفاصيل الطلب";
                CustomerNameLbl.Text = MainOrder.CustomerName + ": اسم الزبون";
                LocationLbl.Text = MainOrder.Location + ": الموقع";
                VehicleLookUpEdit.Properties.DataSource = coco?.Vehicles?.ToList();
                VehicleLookUpEdit.Properties.ValueMember = "ID";
                VehicleLookUpEdit.Properties.DisplayMember = "Number";
                DevExpress.XtraEditors.Controls.LookUpColumnInfo col = new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Number");
                VehicleLookUpEdit.Properties.Columns.Add(col);
                EmployeeLookUpEdit.Properties.DataSource = coco?.Employees?.ToList();
               EmployeeLookUpEdit.Properties.ValueMember = "ID";
               EmployeeLookUpEdit.Properties.DisplayMember = "Name";
                DevExpress.XtraEditors.Controls.LookUpColumnInfo col2 = new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name");
                EmployeeLookUpEdit.Properties.Columns.Add(col2);
            }
        }

        private void EditFrm_Load(object sender, EventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
          
            using (DataContext coco = new DataContext())
            {
                var order = coco.Orders.SingleOrDefault(s => s.ID == MainOrder.ID);
                order.VehicleID = (Guid)VehicleLookUpEdit.EditValue;
                order.EmployeeID = (Guid)EmployeeLookUpEdit.EditValue;
                order.Timer = int.Parse(string.IsNullOrEmpty(textEdit1.Text) ? "0" : textEdit1.Text);
                order.Status = (int)Status.InProgress;
                order.StartTime = DateTime.Now;
                order.SenderUserID = SenderUserID;
                coco.SaveChanges();
                UpdateGrid(order);
                this.Close();
            }
            }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
           using(DataContext db = new DataContext())
            {
                if(DialogResult.OK==MessageBox.Show("حذف","هل أنت متأكد من أنك ستحذف هذا الطلب ؟", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    var DelOrder = db.Orders.SingleOrDefault(s => s.ID == MainOrder.ID);
                    db.Orders.Remove(DelOrder);
                    db.SaveChanges();
                }
               
            }
      
        }
    }
}