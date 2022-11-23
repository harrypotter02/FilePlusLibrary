using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePlusDemo.SnackBarExample
{
    //1.參考FilePlusDemo加入SanckBar控件
    //2.各種警示顏色可在控件內設定
    //3.使用Snackbar需在Nuget安裝MaterialDesignThemes/MaterialDesignColors
    public class MySnack: INotifyPropertyChanged
    {
        public enum MessageToSnackLevel
        {
            NoLevel = 0,
            Error = 1,
            Warning = 2,
            Info = 3,
        }
        public class MyMessage
        {
            public string Content { get; set; } = "";
            public MessageToSnackLevel Level { get; set; } = 0;
            public TimeSpan? Duration { get; set; } = null; // if null it will use the default duration of material design -> 3s
            public bool WithCloseButton { get; set; } = true;
        }

        private SnackbarMessageQueue _message_queue;
        public SnackbarMessageQueue MessageQueue
        {
            set
            {
                _message_queue = value;
                NotifyPropertyChanged("MessageQueue");
            }
            get
            {
                return _message_queue;
            }
        }
        
        private SnackbarMessage _message;
        public SnackbarMessage Message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;                
                if (_message!=null)//前面我們已經插入訊息到List，找看既有的poll內有沒有相同內容項，要設定他的level
                {
                    //查找要新增的訊息是什麼level  //先前在(P1)新增
                    var localMessage = msg_poll.FirstOrDefault(m => m.Content.Equals(_message.Content.ToString()));//LineQ
                    if (localMessage != null)
                    {
                        CurrentMessageLevel = localMessage.Level;
                    }
                    else
                    {
                        CurrentMessageLevel = 0;
                    }
                }
                else
                {
                    CurrentMessageLevel = 0;
                }

                
            }
        }

        private MessageToSnackLevel _currentMessageLevel;
        public MessageToSnackLevel CurrentMessageLevel
        {
            get { return _currentMessageLevel; }
            set {
                _currentMessageLevel = value;
                NotifyPropertyChanged("CurrentMessageLevel");
            }
        }

        private List<MyMessage> msg_poll = new List<MyMessage>();
        public MySnack()
        {
            _message = new SnackbarMessage();
            _message_queue = new SnackbarMessageQueue();
        }

        public void AddMessage(string text,MessageToSnackLevel l=MessageToSnackLevel.NoLevel, double show_time_sec = 3)
        {
            MySnack.MyMessage msg = new MySnack.MyMessage();
            msg.Content = text;
            msg.Level = l;
            msg.Duration = TimeSpan.FromSeconds(show_time_sec);

            msg_poll.Clear();//清空 我們的List用來保存level等資訊
            msg_poll.Add(msg);//(P1) 加到我們自己保存的List內，之後查字典用

            //if (snakMsg.WithCloseButton)
            //{
            //    MessageQueue.Enqueue(snakMsg.Content, new PackIcon() { Kind = PackIconKind.Close }, (queue) => CloseCommand(queue), MessageQueue, false, false, snakMsg.Duration);
            //}
            //else
            //{
                MessageQueue.Enqueue(msg.Content, null, null, null, false, false, msg.Duration);          
            //}

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
