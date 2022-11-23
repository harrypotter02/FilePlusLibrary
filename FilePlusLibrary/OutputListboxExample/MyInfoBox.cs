using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FilePlusLibrary.OutputListboxExample
{
    public static class MyInfoBox
    {
        private static String TAG = "Info";
        private static int index = 0;
        private static bool _is_auto_scroll = true;

        public static Window win = null;
        public static ListBox box=null;
        public static ObservableCollection<string> infolist = new ObservableCollection<string>();

        //設定xaml中的window / 輸出訊息的listbox
        public static void SetRefWindowAndListBox(Window w,ListBox b)
        {
            Debug.WriteLine("SetRefListBox(.)", TAG);
            win = w;
            box = b;
            box.ItemsSource = infolist;
        }
        //新增字串顯示到listbox
        public static void AddInfo(String str)
        {          
            if (win != null && box!=null)
            {
                Debug.WriteLine(str, TAG);
                str = index.ToString() + ": "+str;

                if (win.Dispatcher.CheckAccess())
                {
                    infolist.Add(str);
                    if (_is_auto_scroll)
                    {
                        _scroll_to_bottom(box);
                    }
                }
                else
                {
                    win.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(AddInfo), str);
                }

                index++;
            }
            else
            {
                Debug.WriteLine("ERROR:box or win is null..", TAG);
            }
        }
        //清除listbox顯示
        public static void Clean()
        {
            Debug.WriteLine("Clean()");
            infolist.Clear();
        }
        public static void SetAutoScroll(bool value)
        {
            if(value)
            {
                _is_auto_scroll = true;
            }
            else
            {
                _is_auto_scroll = false;
            }
            Debug.WriteLine("SetAutoScroll(),_is_auto_scroll=" + _is_auto_scroll, TAG);
        }

        private static void _scroll_to_bottom(ListBox box)
        {
            if (box.Items.Count > 0)
            {
                box.Items.MoveCurrentToLast();
                object lastItem = box.Items[box.Items.Count - 1];
                box.ScrollIntoView(lastItem);
            }
        }
    }
}
