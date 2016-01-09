using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gomoku_1312659_Ver4
{
    /// <summary>
    /// Interaction logic for ChatMessage.xaml
    /// </summary>
    public partial class ChatMessage : UserControl
    {
        private string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        private string time;

        public string Time
        {
            get { return time; }
            set { time = value; }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public ChatMessage()
        {
            InitializeComponent();
        }

        public ChatMessage(string playerName, string time, string message)
        {
            InitializeComponent();
            this.playerName = playerName;
            lblName.Content = playerName;
            this.time = time;
            lblTime.Content = time;
            this.message = message;
            lblMessage.Content = message;
        }
    }
}
