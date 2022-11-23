using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePlusLibrary
{
    public class FilePlus
    {
        private string TAG = "FP";
        public string LastError = "";

        private bool is_log_stream_valid;
        private string log_path = "";
        private StreamWriter dev_stream = null;

        private bool _is_record_valid;//3.22_10
        private StreamWriter record_stream = null;//3.22_10
        #region file_dialog
        private string path_and_file_name;
        #endregion

        #region timestamp
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        #endregion

        public FilePlus()
        {
            is_log_stream_valid = false;
            _is_record_valid = false;
            path_and_file_name = "";
            
        }

        #region dev_debug
        //應用:程式內呼叫Debug.WriteLine會自動導向檔案，方便開發者從檔案除錯
        //application: Direct output stream to file for programmer debug.
        //arg1:output filename
        public bool CreateDebug(string fname = @"RD_DEBUG.txt")
        {
            bool ret = true;
            string path = Environment.CurrentDirectory + "\\" + fname;
            try
            {
                //Create new file when open app
                TextWriterTraceListener listener;
                dev_stream = System.IO.File.CreateText(path);

                listener = new MTraceListener(dev_stream);
                Debug.Listeners.Add(listener);
                Debug.AutoFlush = true;
            }
            catch (Exception e)
            {

                string msg = "CreateDebug()" + e.Message;
                error_msg(msg);
                ret = false;
            }

            Debug.WriteLine("CreateDebug(),savepath=" + path, TAG);
            return ret;
        }

        //請在主視窗關閉時，關閉串流
        public void CloseDebug()
        {
            if (dev_stream != null)
            {
                Debug.WriteLine("Release stream.", TAG);
                dev_stream.Close();
                dev_stream.Dispose();
                dev_stream = null;
            }
            else
            {
                error_msg("dev_stream is null,pass close.");
            }
            
        }
        #endregion

        #region user_log
        //應用:創建一個檔案,可經由呼叫AddLog將字串輸出自此檔案
        //application:create file for save log.
        //arg1:outpout file name
        //arg2:the directory path. MUST HAVE last "\\"
        public bool CreateLog(string file_name, string path = "")
        {
            bool ret = true;
            string time = DateTime.Now.ToString();
            string final_path = Environment.CurrentDirectory+"\\";//default
            if(path.Equals("")==false)
            {
                final_path = path;
            }
            Debug.WriteLine("final_pth="+final_path);

            if (CreateDirectory(final_path))
            {
                final_path = final_path +file_name;
                log_path = final_path;
                try
                {
                    StreamWriter wtr = new StreamWriter(final_path);
                    if (wtr != null)
                    {
                        string msg = "Create Log path=" + final_path + ",time=" + time;          
                        wtr.Close();
                        addLog(msg, true);
                        is_log_stream_valid = true;
                    }
                    else
                    {
                        ret = false;
                        error_msg("steam is null..");
                    }
                }
                catch (Exception e)
                {
                    error_msg("CreateLog()" + e.Message);
                    ret = false;
                }
            }
            return ret;
        }
        //應用:將字串寫入到檔案
        //application:write the string to file.
        public void addLog(String str, bool ShowWriteLine = false, bool new_line = true)
        {
            if (is_log_stream_valid)
            {
                if (ShowWriteLine)
                {
                    Debug.WriteLine(str, TAG);
                }
                try
                {
                    using (StreamWriter sm = new StreamWriter(log_path, true))
                    {
                        if (sm != null)
                        {
                            if (new_line)
                            {
                                sm.WriteLine(str);
                            }
                            else
                            {
                                sm.Write(str);
                            }
                            sm.Flush();
                        }
                    }
                }
                catch (Exception e)
                {
                    error_msg("addLog(),"+e.Message);
                }
            }
        }

        #region record_time_sensitive
        //串流不會關閉,需自行呼叫關閉
        public bool CreateRecord(string file_name, string path = "")//3.22_10
        {
            Debug.WriteLine("CreateRecord(..)", TAG);
            bool ret = true;
            string time = DateTime.Now.ToString();
            string final_path = Environment.CurrentDirectory + "\\";//default
            if (path.Equals("") == false)
            {
                final_path = path;
            }
            Debug.WriteLine("final_pth=" + final_path);

            if (CreateDirectory(final_path))
            {
                final_path = final_path + file_name;
                log_path = final_path;
                try
                {
                    record_stream = new StreamWriter(final_path);
                    if (record_stream != null)
                    {
                        _is_record_valid = true;
                    }
                    else
                    {
                        ret = false;
                        error_msg("CreateRecord(),steam is null..");
                    }
                }
                catch (Exception e)
                {
                    error_msg("CreateRecord()" + e.Message);
                    ret = false;
                }
            }
            return ret;
        }
        //應用:將字串寫入到檔案
        //application:write the string to file.
        public void AddRecord(String str, bool ShowWriteLine = false)//3.22_10
        {
            if (_is_record_valid)
            {
                if (ShowWriteLine)
                {
                    Debug.WriteLine(str, TAG);
                }
                try
                {
                        if (record_stream != null)
                        {
                            record_stream.WriteLine(str);
                        }
                }
                catch (Exception e)
                {
                    error_msg("AddRecord()," + e.Message);
                }
            }
        }

        public void CloseRecord()//3.22_10
        {
            Debug.WriteLine("CloseRecord()", TAG);
            if (record_stream != null)
            {
                Debug.WriteLine("Release stream.", TAG);
                record_stream.Flush();//3.22_10
                record_stream.Close();
                record_stream.Dispose();
                record_stream = null;
            }
            else
            {
                error_msg("record_stream is null,pass close.");
            }
        }   
        #endregion

        #endregion
        #region file_dialog
        //arg1: 要保存檔案的名稱 , arg2:保存的檔案類型過濾器 
        public bool OpenSaveDialog(string fname, string filter = "")
        {
            bool ret = true;
            path_and_file_name = "";//reset

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save File";
            sfd.Filter = "All files(*.*) | *.* ";
            if (filter.Equals("") == false)
            {
                sfd.Filter = filter;
            }

            sfd.InitialDirectory = System.Environment.CurrentDirectory;
            sfd.FileName = fname;

            if (sfd.ShowDialog() == true)
            {
                if (File.Exists(sfd.FileName))
                {
                    ret = false;
                    error_msg("ERROR:File Existed It wasn't possible to write the data to the disk.");
                }
                else//3.22_8
                {
                    path_and_file_name = sfd.FileName;
                }
            }
            else
            {
                error_msg("ERROR:Save:User select the cancel option..");
                ret = false;
            }

            return ret;
        }
        public bool OpenFileDialog(string filter="")
        {
            bool ret = true;
            path_and_file_name = "";//reset

            OpenFileDialog lfd = new OpenFileDialog();
            lfd.Title = "Open File";
            //lfd.Filter = "All files(*.*) | *.* ";
            if (filter.Equals("") == false)
            {
                lfd.Filter = filter;
            }

            lfd.InitialDirectory = System.Environment.CurrentDirectory;
            if (lfd.ShowDialog() == true)
            {
                path_and_file_name = lfd.FileName;
            }
            else
            {
                ret = false;
            }
            return ret;
        }

        public string GetFileName()
        {
            return path_and_file_name;
        }
        #endregion
        #region normal
        public bool CreateDirectory(string dic_path)
        {
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(dic_path))
                {
                    Debug.WriteLine("Directory already existed!",TAG);
                    return true;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(dic_path);
                Debug.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(dic_path),TAG);
                return true;
            }
            catch (Exception e)
            {
                string msg = "The process failed: {0}"+e.ToString();
                error_msg(msg);
            }

            return false;
        }

        private void error_msg(string msg)
        {
            LastError = msg;
            Debug.WriteLine(msg, TAG);
        }

        #endregion
    }
}
