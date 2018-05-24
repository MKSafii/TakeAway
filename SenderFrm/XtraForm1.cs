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
using System.Drawing.Drawing2D;
using Data.Enums;
using Data.Data;
using ChatApp.Forms;
using SenderFrm;
using DevExpress.XtraGrid.Views.Tile;
using System.Media;
using DevExpress.XtraBars.Alerter;

namespace TakeAway
{
  
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
      
        List<Order> UpData = new List<Order>();
        public SenderUser SenderUser { get; set; }
        List<TimerOrder> TimerOrder = new List<TimerOrder>();
        List<Order> TimerWating = new List<Order>();
        DataContext _context = new DataContext();
        SoundPlayer UpdateSound = new SoundPlayer("UpdateOrder.wav");
        SoundPlayer FinishSound = new SoundPlayer("alert.wav");
        protected internal XtraForm1(SenderUser user)
        {
            InitializeComponent();
            //cardView1.OptionsView.ShowQuickCustomizeButton = false;
            
            //   lau.DataController.AllowIEnumerableDetails = true;
            SenderUser = user;

            MessageBox.Show("s2fds1");
            
            var order = _context.Orders?.Include("Customer")?.Include("Employee")?.Include("Vehicle").Where(S=>S.Status==(int)Status.Created|| S.Status == (int)Status.Seen || S.Status == (int)Status.Waiting).ToList();
            foreach (var item in order)
            {
                if (item.Time > DateTime.Now.TimeOfDay)
                {
                    item.Status = (int)Status.Waiting;
                }
                else
                {
                    item.Status = (int)Status.Seen;
                }

                if (!TimerWating.Any(s => s.ID == item.ID))
                {
                    TimerWating.Add(item);
                }
            }
            gridControl1.DataSource = order;


            LoadSendGrid();
            tileView1.ItemCustomize += (sender, e) =>
            {
                ColorTile(e.Item);
                
            };
            

            tileView1.ItemClick += (sender, e) =>
            {
                var row = tileView1.GetFocusedRow() as Order;
                EditFrm fofi = new EditFrm(row.ID, SenderUser.ID);
                fofi.ShowDialog();
            };
            tileView2.ItemClick += (sender, e)=>
            {
                var row = tileView2.GetFocusedRow() as SendOrder;
                if (DialogResult.Yes==MessageBox.Show("","لتعديل الطلب اضغط على تعديل "+"\n"+"إذا تم وصول الطلب بنجاح اضغط على : تم التوصيل", MessageBoxButtons.YesNo))
                {
                 

                    var Ord = _context?.Orders
                    ?.Include("Customer")
                    ?.Include("Employee")
                    ?.Include("Vehicle")
                    ?.Include("SenderUser")
                    ?.Include("SenderUser.Employee")
                    ?.Include("CallUser")
                    ?.Include("CallUser.Employee")
                    ?.SingleOrDefault(s => s.ID == row.ID);

                    var finishOrder = new FinishedOrder
                    {
                        Location = Ord?.Location,
                        SenderUserID = Ord?.SenderUserID,
                        StartTime = Ord?.StartTime,
                        CallUserID = Ord?.CallUserID,
                        CustomerID = Ord.CustomerID,
                        Date = Ord.Date,
                        Details = Ord?.Details,
                        Earn = Ord?.Earn,
                        EmployeeID = Ord?.EmployeeID,
                        EndTime = DateTime.Now,
                        VehicleID = Ord?.VehicleID,
                        SenderUserName = Ord?.SenderUser?.Employee?.Name,
                        CallUserName = Ord?.CallUser?.Employee?.Name,
                        CustomerName = Ord?.Customer?.Name,
                        EmployeeNaame = Ord?.Employee?.Name,
                    };
                    _context.FinishedOrders.Add(finishOrder);
                    if (TimerWating.Contains(Ord)) TimerWating.Remove(Ord);
                    var Tord = TimerOrder?.SingleOrDefault(s => s.order == Ord);
                    if (Tord != null) TimerOrder.Remove(Tord);
                    //     _context.SaveChanges();
                    _context.Orders.Remove(Ord);
                    _context.SaveChanges();
                    MessageBox.Show("تمت العملية بنجاح");
                    LoadSendGrid();
             
            }
                else
                {
                    EditFrm fofo = new EditFrm(row.ID, SenderUser.ID);
                    fofo.ShowDialog();
                }
            };
            
            #region this event work after set employee $  Vehicle
            EditFrm.UpdateGrid += (o) =>
            {
                LoadSendGrid();
                TimerOrder.Add(new TimerOrder { order = o, Time = o.BikeTime,IsNew=true });
                var ord = _context?.Orders?.Where(S => S.Status == (int)Status.Created || S.Status == (int)Status.Seen || S.Status == (int)Status.Waiting).ToList();
                gridControl1.DataSource = ord;

            };
            #endregion

            #region Finish click Event
            FinishBtn.Click += (sender, e) =>
            {
                var row = tileView2.GetFocusedRow() as SendOrder;
                
                var Ord = _context?.Orders
                ?.Include("Customer")
                ?.Include("Employee")
                ?.Include("Vehicle")
                ?.Include("SenderUser")
                ?.Include("SenderUser.Employee")
                ?.Include("CallUser")
                ?.Include("CallUser.Employee")
                ?.SingleOrDefault(s => s.ID == row.ID);

                var finishOrder = new FinishedOrder
                {
                    Location = Ord?.Location,
                    SenderUserID = Ord?.SenderUserID,
                    StartTime = Ord?.StartTime,
                    CallUserID = Ord?.CallUserID,
                    CustomerID = Ord.CustomerID,
                    Date = Ord.Date,
                    Details = Ord?.Details,
                    Earn = Ord?.Earn,
                    EmployeeID = Ord?.EmployeeID,
                    EndTime = DateTime.Now,
                    VehicleID = Ord?.VehicleID,
                    SenderUserName = Ord?.SenderUser?.Employee?.Name,
                    CallUserName = Ord?.CallUser?.Employee?.Name,
                    CustomerName = Ord?.Customer?.Name,
                    EmployeeNaame = Ord?.Employee?.Name,
                };
                _context.FinishedOrders.Add(finishOrder);
                //     _context.SaveChanges();
                _context.Orders.Remove(Ord);
                _context.SaveChanges();
                MessageBox.Show("تمت العملية بنجاح");
                LoadSendGrid();
            };
            #endregion

            #region send click Event
            SendButtonEdit.Click += (Sender, e) => {

                try
                {
                //   var v= VehicleLookUpEdit.value as Vehicle;
                    var row = tileView1.GetFocusedRow() as Order;
                    EditFrm fofo = new EditFrm(row.ID, SenderUser.ID);
                    fofo.ShowDialog();

                }
                catch (Exception ee)
                {}


            };
            #endregion

            

            #region intilize alert timer
            //Timer toto = new Timer();
            //toto.Interval = 1000;
            //toto.Tick += (sender, e) =>
            //{
            //    if(TimerOrder.Count>0)
            //    foreach (var item in TimerOrder)
            //    {
            //        if(item.Time==0)
            //        {
            //            var emp = _context.Employees.SingleOrDefault(s => s.ID == item.order.EmployeeID);
            //            alertControl1.Show(this, "هذا الموظف يجب أن يكون قد وصل", emp.Name);
            //            TimerOrder.Remove(item);
            //            continue;
            //            //var order2 = _context.Orders.SingleOrDefault(s => s.ID == item.order.ID);
            //            //_context.SaveChanges();
            //        }
            //        item.Time--;
            //    }
            //};
            //toto.Start();
            #endregion

            #region intilize Wating Timer
            Timer WaitBike = new Timer();
            WaitBike.Interval = 300000;
            WaitBike.Tick += (Sender, e) =>
            {
                if (TimerWating?.Count > 0)
                {
                    foreach (var item in TimerWating)
                    {
                    
                        if (item.BikeTime> DateTime.Now.TimeOfDay)
                        {
                            FinishSound.Play();
                            alertControl1.Show(this, ": تنبيه ", item.Details+": إن الطلبية \n"+"\n يجب أن تكون قد وصلت");
                        }
                        
                }
                }
               
            };
            WaitBike.Start();



            Timer Wait = new Timer();
            Wait.Interval = 300000;
            Wait.Tick += (Sender, E) =>
            {
                foreach (var item in TimerWating)
                {
                    var t = new TimeSpan(0, 45,0);
                    var total = DateTime.Now.TimeOfDay;
                //    MessageBox.Show("now: " + total + "\n " + "time: " + item.Time);
                    if (item.Time <= total && item.WithTimer)
                    {
                        var orderRadey = _context?.Orders?.SingleOrDefault(s => s.ID == item.ID);
                        orderRadey.Status = (int)Status.Seen;
                        _context.SaveChanges();
                        for(int i = 0; i < tileView1.RowCount; i++)
                        {
                            if (item.ID.Equals(tileView1.GetRowCellValue(i, "ID")))
                            {
                                
                            }
                        }
                        alertControl1.Show(this, "يجب إرسال الطلب ", item.Details);
                    }
                }
            };
            Wait.Start();
            #endregion

            backgroundWorker1.RunWorkerAsync();
        }


        /// <summary>
        /// Load Grid 2 (Send Grid) Data
        /// </summary>
        void LoadSendGrid()
        {
            var SendOrder = _context.Orders?.Include("Customer")?.Include("Employee")?.Include("Vehicle").Where(S => S.Status == (int)Status.InProgress).Select(
                s => new SendOrder { ID = s.ID, Details = s.Details, CustomerName = s.Customer.Name, EmployeeName = s.Employee.Name, Earn = s.Earn, Location = s.Location, StartTime = s.StartTime, Status = s.Status, Timer = s.Timer }
                ).ToList();


            gridControl2.DataSource = SendOrder;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(1000);
                    DataContext DB = new DataContext();
                    var ISNewdata = DB?.Orders?.Include("Customer")?.Include("Employee")?.Include("Vehicle")?.Where(S => S.Status == (int)Status.Created || S.Updated == 1).Any();

                    if ((bool)ISNewdata)
                    {
                         UpData = DB?.Orders?.Include("Customer")?.Where(S => S.Status == (int)Status.Created||S.Status==(int)Status.Seen || S.Status == (int)Status.Waiting ).ToList();
                        foreach (var item in UpData)
                        {
                            if (item.Time > DateTime.Now.TimeOfDay)
                            {
                                item.Status = (int)Status.Waiting;
                            }
                            else
                            {
                                item.Status = (int)Status.Seen;
                            }
                           if(! TimerWating.Any(s => s.ID == item.ID))
                            {
                                TimerWating.Add(item);
                            }
                            if (item.Updated == 1)
                            {
                                BeginInvoke(new MethodInvoker(delegate () {
                                    UpdateSound.Play();
                                    AlertInfo info = new AlertInfo(item.Details + ": تم تعديل الطلب", item.Customer.Name = ": اسم الزبون ");
                                    alertControl1.Show(this, info);
                                }));
                                item.Updated = 2;
                                //tileView1_ItemCustomize(tileView1, new TileViewItemCustomizeEventArgs(new TileViewItem()));
                             //   ShowAlert(item.Details + ": تم تعديل الطلب", item.Customer.Name = ": اسم الزبون ");
                                System.Threading.Thread.Sleep(5000);
                            }
                        }

                        this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate () { gridControl1.DataSource = UpData; });
                        DB.SaveChangesAsync();
                    }

                     
                    //backgroundWorker1.RunWorkerAsync();
                }
            }


        }
    

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //TestDataContext DB = new TestDataContext();
            //var data = DB?.Tests?.Where(S => S.Status > 0);
            this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate () { gridControl1.DataSource = UpData; });
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SenderUser = null;
            frmLogIn fofo = new frmLogIn();
            fofo.ShowDialog();
            this.Close();
            
           
          
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        void ColorTile(TileViewItem e)
        {
            if ((int)tileView1.GetRowCellValue(e.RowHandle, "Status") == (int)Status.Waiting)
            {
                e.Elements[3].Appearance.Normal.BackColor = Color.Red;
            }
            else if ((int)tileView1.GetRowCellValue(e.RowHandle, "Status") == (int)Status.Seen)
            {
                e.Elements[3].Appearance.Normal.BackColor = Color.Orange;
            }
            int Updated = (int)tileView1.GetRowCellValue(e.RowHandle, "Updated");
            if (Updated == 1 || Updated == 2)
            {
                e.Elements[3].Appearance.Normal.BackColor = Color.DarkTurquoise;
            }

        }


    }
    public class TimerOrder
    {
        public Order order { get; set; }
        public TimeSpan? Time { get; set; }
        public bool IsNew { get; set; }
    }
    public class SendOrder
    {
        public Guid ID { get; set; }
        public string Details { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public decimal? Earn { get; set; }
        public string Location { get; set; }
        public DateTime? StartTime { get; set; }
        public int Status { get; set; }
        public int? Timer { get; set; }
    }
    public class Message
    {
        public Message()
        {
        
        }
        public string Caption { get; set; }
        public string Text { get; set; }
        public Image Image { get; set; }
    }
}