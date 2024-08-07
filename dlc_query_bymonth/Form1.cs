using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace dlc_query_bymonth
{
    public partial class Form1 : Form
    {
        //工号
        private String EmpnoString = null;

        //查询的月份
        private String QueryMonthString = null;

        //所有记录
        private List<List<String>> MonthList = null;

        //计算所有钱
        private int MoneyOfBreakfast = 0;
        private int MoneyOfLunch = 0;
        private int MoneyOfDinner = 0;

        //计算用餐次数
        private int CountOfBreakfast = 0;
        private int CountOfLunch = 0;
        private int CountOfDinner = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            //查询前清除旧数据
            MoneyOfBreakfast = 0;
            MoneyOfLunch = 0;
            MoneyOfDinner = 0;


            CountOfBreakfast = 0;
            CountOfLunch = 0;
            CountOfDinner = 0;
            label3.Text = "";
            label4.Text = "";


            if (!CheckInput())
            {
                return;
            }

            FormatMonth();
            query_sql();
            ShowData();
        }

        private Boolean CheckInput()
        {
            if (textBox1.Text.Equals("") || String.IsNullOrEmpty(textBox1.Text))
            {
                return false;
            }
            return true;

        }

        private void FormatMonth()
        {
            QueryMonthString = dateTimePicker1.Text;
        }


        void query_sql()
        {
            MonthList = null;
            MonthList = new List<List<String>>();
            MoneyOfBreakfast = MoneyOfLunch = MoneyOfDinner = 0;


            //准备连接数据库
            EmpnoString = textBox1.Text;
            textBox2.Text = "准备连接数据库...";
            textBox2.Text += "\r\n";

            //string SQLCONNECT = @"server=192.168.9.100;database=dlcpro;uid=sa;pwd=";
            string SQLCONNECT = @"server=127.0.0.1;database=dlcpro;uid=sa;pwd=";
            SqlConnection conn = new SqlConnection(SQLCONNECT);



            try
            {
                // 引起异常的语句
                conn.Open();
                textBox2.Text += "打开数据库...";
                textBox2.Text += "\r\n";


                //连接数据库之后就可以执行SQL命令了
                //使用SqlCommand类的ExecuteReader()返回执行的结果
                //select * from kq_cardlist where empno = '13030195'
                //string SQLCOMMAND = "select empno,empname,deptname,xfdate,xftime,occur_amt,user1,user2,operator  from xf_record   where (xf_record.empno="13101888") order by xfdate,xftime";
                string SQLCOMMAND = "select empno,empname,deptname,xfdate,xftime,occur_amt,operator  from xf_record   where (xf_record.empno= '" + textBox1.Text + "') order by xfdate,xftime";

                SqlCommand sqlcmd = new SqlCommand(SQLCOMMAND, conn);




                textBox2.Text += "查询数据库...";
                textBox2.Text += "\r\n";
                SqlDataReader sr = sqlcmd.ExecuteReader();
                int nSqlCol = sr.FieldCount;

                //for (int i = 0; i < nSqlCol; ++i){
                //    textBox2.Text += "\r\n";
                //    textBox2.Text += sr.GetFieldType(i);
                // }      ;


                int count = 0;
                while (sr.Read())
                {
                    //empno,empname,deptname,xfdate,xftime,occur_amt,operator

                    String xfdate = sr[3].ToString();
                    String xftime = sr[4].ToString();

                    if (xfdate.StartsWith(QueryMonthString))
                    {
                        xfdate = xfdate.Substring(0, 10);
                        xftime = xftime.Substring(11);

                    }
                    else
                    {
                        continue;
                    }


                    List<String> array = new List<String>();
                    array.Add(sr[0].ToString());
                    array.Add(sr[1].ToString());
                    array.Add(sr[2].ToString());
                    array.Add(xfdate);
                    array.Add(xftime);
                    array.Add(sr[5].ToString());
                    array.Add(sr[6].ToString());

                    MonthList.Add(array);

                    String MoneyType = sr[6].ToString();
                    if (MoneyType.Equals("中"))
                    {
                        MoneyOfLunch += 5;
                        CountOfLunch += 1;
                    }
                    else if (MoneyType.Equals("晚"))
                    {
                        MoneyOfDinner += 5;
                        CountOfDinner += 1;
                    }
                    else
                    {
                        MoneyOfBreakfast += 3;
                        CountOfBreakfast += 1;
                    }

                    count++;

                }

                sr.Close();

                if (count < 1)
                {
                    textBox2.Text = "没有查询到数据！！！！！！！！！";
                    textBox2.Text += "\r\n";
                    textBox2.Text += "没有查询到数据！！！！！！！！！";
                    textBox2.Text += "\r\n";
                    textBox2.Text += "没有查询到数据！！！！！！！！！";
                    textBox2.Text += "\r\n";
                }
                else
                {
                    textBox2.Text += "共" + count + "条数据：";
                    textBox2.Text += "\r\n";
                }


                ////通过SqlCommand 类的ExecuteNonQuery()来返回受影响的行数。
                //string SQLCOMMAND2 = "update dbo.Messages set MessageNum='15' where MessageID='2'";
                //SqlCommand sqlcmd2 = new SqlCommand(SQLCOMMAND2, conn);//也可以用sqlcmd. ConnectionString = SQLCOMMAND2 代替
                //int nResult = sqlcmd2.ExecuteNonQuery();
                //Console.WriteLine("受影响行数:" + nResult);

                conn.Close();
            }
            catch (Exception ex)
            {
                // 错误处理代码
                textBox2.Text += "打开数据库错误.";
                textBox2.Text += "\r\n";
                textBox2.Text += SQLCONNECT;
                textBox2.Text += "\r\n";
                textBox2.Text += ex.Message.ToString();
                textBox2.Text += "\r\n";
            }
            finally
            {
                // 要执行的语句
                conn.Close();
            }

        }

        void ShowData()
        {
            listView1.View = View.Details;//设置视图   获取或设置项在控件中的显示方式
            listView1.FullRowSelect = true;//设置是否行选择模式
            listView1.GridLines = true;//设置网格线
            listView1.AllowColumnReorder = true;//设置是否可拖动列标头来对改变列的顺序。
            listView1.MultiSelect = true;//设置是否可以选择多个项

            //添加列  empno,empname,deptname,xfdate,xftime,occur_amt,operator

            listView1.Columns.Clear();
            listView1.Columns.Add("序号", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("工号", 70, HorizontalAlignment.Center);

            listView1.Columns.Add("姓名", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("部门", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("日期", 90, HorizontalAlignment.Center);
            listView1.Columns.Add("时间", 90, HorizontalAlignment.Center);
            listView1.Columns.Add("金额", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("类型", 50, HorizontalAlignment.Center);

            listView1.BeginUpdate();
            listView1.EndUpdate();

            //显示到数据ListView
            AddDataToListView();

            label3.Text = "早=" + CountOfBreakfast + "次, 中=" + CountOfLunch + "次, 晚=" + CountOfDinner + "次";
            label4.Text = "早=" + MoneyOfBreakfast + "元, 中=" + MoneyOfLunch + "元, 晚=" + MoneyOfDinner + "元, 共计=" + (MoneyOfBreakfast + MoneyOfLunch + MoneyOfDinner) + "元"; ;


        }

        private void AddDataToListView()
        {
            //先清空显示列表,不然会重复显示
            listView1.Items.Clear();
            
            if (MonthList == null)
                return;

            if (MonthList.Count <= 0)
                return;

            int index = 0;
            int count = MonthList.Count;

            for (int i = 0; i < count; i++)
            {

                var item = new ListViewItem();
                index++;
                item.Text = index.ToString(); //序号

                List<String> array = MonthList[i];
                item.SubItems.Add(array[0]); //工号
                item.SubItems.Add(array[1]); //姓名  
                item.SubItems.Add(array[2]); //部门 
                item.SubItems.Add(array[3]); //日期
                item.SubItems.Add(array[4]); //时间 
                item.SubItems.Add(array[5]); //金额
                item.SubItems.Add(array[6]); //类型


                listView1.BeginUpdate();
                listView1.Items.Add(item);

                listView1.EndUpdate();
                item.EnsureVisible(); //关键的实现函数


                //}
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "yyyy-MM";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Value = DateTime.Today.AddMonths(-1);
        }
    }
}
