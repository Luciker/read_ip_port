
using OfficeOpenXml;
//using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace read_ip_port
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool isrunning;
        public bool Isrunning
        {
            get { return isrunning; }
            set
            {
                foreach (Control control in this.Controls)
                {
                    if (control is System.Windows.Forms.TextBox || control is System.Windows.Forms.Button)
                    {
                        if (control.Name != "txt_info")
                        {
                            control.Enabled = !value;
                        }
                    }
                }
                isrunning = value;
            }
        }

        private int totalRows = 0;

        public int TotalRows
        {
            get { return totalRows; }
            set { totalRows = value; }
        }

        private int totalInputFiles = 0;

        public int TotalInputFiles
        {
            get { return totalInputFiles; }
            set { totalInputFiles = value; }
        }

        private string xlsxPath = string.Empty;
        /// <summary>
        /// 导出的xlsx路径
        /// </summary>
        public string XlsxPath
        {
            get { return xlsxPath; }
            set { xlsxPath = value; }
        }

        private void SubmitInfo(string message)
        {
            Action<string> action = (msg) =>
            {
                this.txt_info.Text += msg;
                this.txt_info.Select(this.txt_info.Text.Length, 0);
                this.txt_info.ScrollToCaret();
            };
            if (InvokeRequired)
            {
                this.BeginInvoke(action, message);

            }
            else
            {
                action(message);
                //this.txt_info.Text += message;
                //this.txt_info.Select(this.txt_info.Text.Length, 0);
                //this.txt_info.ScrollToCaret();
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                var path = this.folderBrowserDialog1.SelectedPath;
                this.txt_import.Text = path;
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (txt_import.Text.Trim().Length > 0)
            {
                string basepath = this.txt_import.Text;
                if (!Directory.Exists(basepath))
                {
                    MessageBox.Show("目录不存在");
                }
                else
                {
                    Isrunning = true;
                    this.backgroundWorker1.RunWorkerAsync(basepath);
                }
            }
        }


        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.GetAllDirList(e.Argument.ToString());
            List<string> pathtolist = this.HostsDictoryPath;
            List<string[]> ipports = new List<string[]>();
            foreach (var item in pathtolist)
            {
                this.XlsxPath = item.Remove(item.LastIndexOf('\\')) + ".xlsx";
                DirectoryInfo di = new DirectoryInfo(item);
                var hostfiles = di.GetFiles();
                foreach (var hostfile in hostfiles)
                {
                    if (hostfile.Extension == ".html")
                    {
                        if (item.EndsWith("hosts"))
                        {
                            ipports.AddRange(ReadIPandPortV5(hostfile.FullName));
                        }
                        else if (item.EndsWith("host"))
                        {
                            ipports.AddRange(ReadIPandPortV6(hostfile.FullName));
                        }
                        this.TotalInputFiles++;
                        this.TotalRows += ipports.Count;
                        this.backgroundWorker1.ReportProgress(ipports.Count, hostfile);
                    }
                }
                if (ipports.Count > 0)
                {
                    this.backgroundWorker1.ReportProgress(-1, "正在写入到" + this.XlsxPath + "\r\n");
                    this.WriteXLS(ipports, this.XlsxPath);
                    ipports.Clear();
                }
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= 0)
            {
                SubmitInfo("正在处理" + e.UserState.ToString() + "文件，导入" + e.ProgressPercentage.ToString() + "行\r\n");
            }
            else
            {
                SubmitInfo(e.UserState.ToString());
            }

        }



        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            SubmitInfo("===================================\r\n");
            SubmitInfo("共处理" + this.TotalInputFiles.ToString() + "个文件，导入" + this.TotalRows.ToString() + "行\r\n");
            Isrunning = false;
            this.HostsDictoryPath = null;
        }

        private List<string> HostsDictoryPath = new List<string>();
        /// <summary>
        /// 递归列文件夹
        /// </summary>
        public void GetAllDirList(string strBaseDir)
        /// <param name="strBaseDir"></param>
        {
            DirectoryInfo di = new DirectoryInfo(strBaseDir);
            DirectoryInfo[] diA = di.GetDirectories();
            if (this.HostsDictoryPath == null)
            {
                this.HostsDictoryPath = new List<string>();
            }
            for (int i = 0; i < diA.Length; i++)
            {
                if (diA[i].Name == "host" || diA[i].Name == "hosts")
                {
                    HostsDictoryPath.Add(diA[i].FullName);
                }
                GetAllDirList(diA[i].FullName);
            }
        }

        /// <summary>
        /// 处理v5版报告
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        private List<string[]> ReadIPandPortV5(string host)
        {
            List<string[]> result = new List<string[]>();
            string htmltxt = File.ReadAllText(host).Trim();

            //获取IP
            string startflag = "<tr class=\"even\"><td>IP地址</td><td>";
            string tmphtml = htmltxt;
            tmphtml = tmphtml.Substring(tmphtml.IndexOf(startflag) + startflag.Length);
            string ip = tmphtml.Substring(0, tmphtml.IndexOf("</td>"));


            startflag = "<caption>端口信息</caption>";
            string endflag = "</table>";

            //如果没有端口信息直接返回
            if (htmltxt.IndexOf(startflag) < 0)
            {
                return result;
            }
            try
            {
                //读取“<caption>端口信息</caption>”之后的html
                htmltxt = htmltxt.Substring(htmltxt.IndexOf(startflag) + startflag.Length);
                //读取新子串的第一个</table>，里面包含部分的table
                htmltxt = htmltxt.Substring(0, htmltxt.IndexOf(endflag)).Trim();
                //获取tbody
                htmltxt = htmltxt.Substring(htmltxt.IndexOf("<tr class=\"odd\">")).Trim();
                //分行
                string[] splitflag1 = { "<tr class=\"even\">", "<tr class=\"odd\">" };
                string[] rows = htmltxt.Split(splitflag1, StringSplitOptions.RemoveEmptyEntries);
                //截取<td></td>中的内容,tbody没写错
                string[] splitflag2 = { "</td>", "<td>", "</tr>", "</tbody" };
                foreach (string row in rows)
                {
                    string tmprow = row;
                    tmprow = tmprow.Replace("\t", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\r", string.Empty);
                    List<string> tmpresult = new List<string>();
                    tmpresult.Add(ip);
                    string[] tmpresult2 = tmprow.Split(splitflag2, StringSplitOptions.RemoveEmptyEntries);
                    //部分表格“服务列”为空白
                    if (tmpresult2.Length == 3)
                    {
                        tmpresult.Add(tmpresult2[0]);
                        tmpresult.Add(tmpresult2[1]);
                        tmpresult.Add(string.Empty);
                        tmpresult.Add(tmpresult2[2]);
                    }
                    else
                    {
                        tmpresult.AddRange(tmpresult2);
                    }
                    result.Add(tmpresult.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理报告源文件失败:" + ex.Message + "\r\n请关闭重试");
            }
            return result;
        }




        /// <summary>
        /// 从v6文件中读取ip端口信息
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        private List<string[]> ReadIPandPortV6(string host)
        {
            List<string[]> result = new List<string[]>();
            string htmltxt = File.ReadAllText(host).Trim();

            //获取IP
            string startflag = "IP地址</th>";
            string tmphtml = htmltxt;
            tmphtml = tmphtml.Substring(tmphtml.IndexOf(startflag) + startflag.Length);
            tmphtml = tmphtml.Replace("\t", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\r", string.Empty);
            string ip = tmphtml.Substring(4, tmphtml.IndexOf("</td>") - 4);

            startflag = "端口信息</div>";
            string endflag = "</div>";
            //获取3.2
            //如果没有端口信息直接返回
            if (htmltxt.IndexOf(startflag) < 0)
            {
                return result;
            }
            try
            {
                //读取“端口信息</div>”之后的html
                htmltxt = htmltxt.Substring(htmltxt.IndexOf(startflag) + startflag.Length);
                //读取新子串的第一个</div>，里面包含一个table
                htmltxt = htmltxt.Substring(0, htmltxt.IndexOf(endflag)).Trim();
                //获取tbody
                htmltxt = htmltxt.Substring(htmltxt.IndexOf("<tbody>") + 7, htmltxt.IndexOf("</tbody>") - htmltxt.IndexOf("<tbody>")).Trim();
                //分行
                string[] splitflag1 = { "<tr class=\"even\">", "<tr class=\"odd\">" };
                string[] rows = htmltxt.Split(splitflag1, StringSplitOptions.RemoveEmptyEntries);
                //截取<td></td>中的内容,tbody没写错
                string[] splitflag2 = { "</td>", "<td>", "</tr>", "</tbody" };
                foreach (string row in rows)
                {
                    string tmprow = row;
                    tmprow = tmprow.Replace("\t", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);
                    List<string> tmpresult = new List<string>();
                    tmpresult.Add(ip);
                    string[] tmpresult2 = tmprow.Split(splitflag2, StringSplitOptions.RemoveEmptyEntries);
                    //部分表格“服务列”为空白
                    if (tmpresult2.Length == 3)
                    {
                        tmpresult.Add(tmpresult2[0]);
                        tmpresult.Add(tmpresult2[1]);
                        tmpresult.Add(string.Empty);
                        tmpresult.Add(tmpresult2[2]);
                    }
                    else
                    {
                        tmpresult.AddRange(tmpresult2);
                    }
                    result.Add(tmpresult.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理报告源文件失败:" + ex.Message + "\r\n请关闭重试");
            }
            return result;
        }

        /// <summary>
        /// 将信息写入excel
        /// </summary>
        /// <param name="ip_port"></param>
        private void WriteXLS(List<string[]> ip_port, string xlspath)
        {
            if (ip_port.Count > 0)
            {
                int pagesize = 1000000;
                try
                {
                    #region com法,可能有兼容性问题
                    //object misValue = System.Reflection.Missing.Value;
                    //Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                    //Microsoft.Office.Interop.Excel.Workbook xlWorkBook = xlApp.Workbooks.Add(misValue);
                    //Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                    //xlWorkSheet.Cells[1, 1] = "IP";
                    //xlWorkSheet.Cells[1, 2] = "端口";
                    //xlWorkSheet.Cells[1, 3] = "协议";
                    //xlWorkSheet.Cells[1, 4] = "服务";
                    //xlWorkSheet.Cells[1, 5] = "状态";

                    //for (int i = 0; i < ip_port.Count; i++)
                    //{
                    //    for (int j = 0; j < ip_port[i].Length; j++)
                    //    {
                    //        xlWorkSheet.Cells[i + 2, j + 1] = ip_port[i][j];
                    //    }
                    //}

                    //xlWorkBook.SaveAs(xlspath);

                    //xlWorkBook.Close(true, misValue, misValue);
                    //xlApp.Quit();

                    //releaseObject(xlApp);
                    //releaseObject(xlWorkBook);
                    //releaseObject(xlWorkBook);
                    #endregion

                    #region spire.xls法，有65535限制
                    //Workbook workbook = new Workbook();

                    ////获取第一个sheet，进行操作，下标是从0开始
                    //Worksheet sheet = workbook.Worksheets[0];

                    //DataTable table = new DataTable();
                    //table.Columns.Add("IP");
                    //table.Columns.Add("端口");
                    //table.Columns.Add("协议");
                    //table.Columns.Add("服务");
                    //table.Columns.Add("状态");
                    //foreach (string[] item in ip_port)
                    //{
                    //    table.Rows.Add(item);
                    //}

                    //sheet.InsertDataTable(table, true, 1, 1);

                    ////将Excel文件保存到指定文件
                    //ExcelVersion ver = new ExcelVersion();
                    //switch (version.Trim())
                    //{
                    //    case "Excel 97-2003": ver = ExcelVersion.Version97to2003; break;
                    //    case "Excel 2007": ver = ExcelVersion.Version2007; break;
                    //    case "Excel 2010": ver = ExcelVersion.Version2010; break;
                    //    case "Excel 2013": ver = ExcelVersion.Version2013; break;
                    //    default:
                    //        ver = ExcelVersion.Version97to2003;
                    //        break;
                    //}

                    ////控制文件后缀
                    //xlspath = xlspath.Remove(xlspath.LastIndexOf('.'));
                    //xlspath += ver == ExcelVersion.Version97to2003 ? ".xls" : ".xlsx";

                    //workbook.SaveToFile(xlspath, ver);
                    #endregion

                    FileInfo newFile = new FileInfo(xlspath);

                    if (File.Exists(xlspath))
                    {
                        File.Move(xlspath, xlspath + ".old");
                    }

                    using (ExcelPackage package = new ExcelPackage(newFile))
                    {
                        ip_port.TrimExcess();
                        int maxpage = Convert.ToInt32(Math.Ceiling((double)ip_port.Count / pagesize));
                        //int maxpage = Convert.ToInt32(Math.Ceiling((double)ip_port.Count / pagesize)+1);
                        int tail = ip_port.Count % pagesize;
                        for (int currentpage = 0; currentpage < maxpage; currentpage++)
                        {
                            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("IP端口" + (currentpage + 1).ToString());

                            sheet.Cells[1, 1].Value = "IP";
                            sheet.Cells[1, 2].Value = "端口";
                            sheet.Cells[1, 3].Value = "协议";
                            sheet.Cells[1, 4].Value = "服务";
                            sheet.Cells[1, 5].Value = "状态";

                            //如果不是最后一页，需要导入pagesize行；否则只导入尾数
                            int camp = maxpage - currentpage > 1 ? pagesize : tail; ;

                            for (int i = 0; i < camp; i++)
                            {
                                ///个别行可能只有3列
                                for (int j = 0; j < ip_port[i].Length; j++)
                                {
                                    //try
                                    //{
                                    sheet.Cells[i + 2, j + 1].Value = ip_port[i][j].ToString();
                                    //}
                                    //catch (Exception)
                                    //{
                                    //    Console.Write(i.ToString() + " " + j.ToString());
                                    //    throw;
                                    //}

                                }
                            }
                        }
                        package.Save();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("写入xls文件错误:" + ex.Message + "\r\n请关闭重试");
                }
            }

        }

        //#region 释放com
        //private void releaseObject(object obj)
        //{
        //    try
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        //        obj = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        obj = null;
        //        MessageBox.Show("释放Excel进程失败:" + ex.ToString());
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //}
        //#endregion





    }
}
