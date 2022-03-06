using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace rsa_deleteTempFiles
{
    internal class Program
    {
        string _Black_List { get; set; }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        static void Main(string[] args)
        {
            try
            {
                ShowWindow(GetConsoleWindow(), 0);
                new Thread(() => { new Program().DeleteFileFunction(); }) { Name = "ThreadDeleteFile", Priority = ThreadPriority.Highest, IsBackground = true }.Start();
                Console.ReadLine();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        
        private void DeleteFileFunction() {
            Again:
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("id", Type.GetType("System.Int32")) { AutoIncrement = true });
                dt.Columns.Add("black_List", typeof(string));
                dt.AcceptChanges();
                DateTime _Sleep = DateTime.Now;
                while (true)
                {
                    try
                    {
                        string[] _pathFiles = Directory.GetFiles(Path.GetTempPath(), "*.*", SearchOption.AllDirectories);
                        foreach ( string file in _pathFiles)
                        {
                            DataRow dr = dt.Select(string.Format("black_List = '{0}'", file)).FirstOrDefault();
                            if (dr == null)
                            {
                                _Black_List = file;
                                File.Delete(file);
                            }
                            else
                            {
                              TimeSpan sp = DateTime.Now - _Sleep;
                                if (sp.Seconds > 10)
                                {
                                    _Black_List = dr["black_List"].ToString();
                                    _Sleep = DateTime.Now;
                                    File.Delete(_Black_List);
                                    dt.Rows[dt.Columns.IndexOf("id")].Delete();
                                    dt.AcceptChanges();
                                    _Black_List = string.Empty;
                                }
                            }
                        }
                    }
                    catch 
                    {
                        if (dt.Select(string.Format("black_List = '{0}'", _Black_List)).FirstOrDefault() == null)
                        {
                            DataRow dr_insert = dt.NewRow();
                            dr_insert["black_List"] = _Black_List;
                            dt.Rows.InsertAt(dr_insert, dt.Rows.Count + 1);
                            _Black_List = string.Empty;
                        }
                        continue;
                    }
                    Thread.Sleep(1000);
                }

            }
            catch 
            {
                goto Again;
            }
        }
       
    }
}
