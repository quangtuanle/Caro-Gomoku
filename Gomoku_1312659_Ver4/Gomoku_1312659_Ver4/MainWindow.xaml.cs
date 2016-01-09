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
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Gomoku_1312659_Ver4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int ROW = 12;     // Số dòng bàn cờ
        const int COL = 12;     // Số cột bàn cờ
        const int WIN = 5;      // Số ô liên tiếp cùng màu để WIN

        ChessBoard board;                   // Lưu bàn cờ 
        int[][] arrSquare = new int[ROW][]; // Mảng các ô vuông bàn cờ

        bool player = true;     // Phân biệt người chơi (True/False | Người/Máy | Người/Server | Máy/Server) 
        int type;               // Kiểu chơi: 2 người tự chơi, người với máy, người với server, máy với server
        bool endGame = false;   // Kiểm tra xem đã kết thúc trò chơi chưa

        Player userPlayer1 = new Player();
        Player userPlayer2 = new Player();
        bool mesChanged = false;
        int guest; // Xem chỉ số đối thủ (Nếu đi đầu là 0, đi sau là 1)

        //Socket socket;
        bool holdTurn = false; // Biến này dùng để biết xem server đã thông báo lượt đi của từng người chơi chưa
                               // Tránh việc đối thủ gửi thông điệp có chứa "first" hoặc "second" làm cho rối loạn mạch chơi
        int rowServer, colServer; // Giữ bước đi của đối phương do Server gửi xuống

        BackgroundWorker bWorker = new BackgroundWorker();          // Xử lý tiến trình Người vs Máy
        BackgroundWorker bWorkerServer = new BackgroundWorker();    // Xử lý tiền trình Client vs Server   

        Socket socket;
        bool connectSuccess; // Biến này sẽ giữ giá trị xem thử đã kết nối thành công chưa (Nếu thành công mới cho Click)

        public MainWindow()
        {
            InitializeComponent();

            // Khởi tạo mảng các ô cờ: Dùng để đánh dấu tình trạng của ô cờ
            for (int i = 0; i < ROW; i++)
                arrSquare[i] = new int[COL];

            SetIsStatic();

            board = new ChessBoard();
            board.ClickSquare += ClickChange;
            DataContext = board;

            userPlayer1.Name = "Super Saiyan";

            showChatMessage("Trò chơi bắt đầu", "System");

            if (type == 1 || type == 2)
            {
                bWorker.DoWork += new DoWorkEventHandler(DoComputer);
                bWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompleteComputerTurn);
                bWorker.RunWorkerAsync();
            }
            else if (type == 3 || type == 4) // Nếu kiểu chơi là 3 hoăc 4 sẽ tiến hành kết nối Server
            {
                socket = IO.Socket("ws://gomoku-lajosveres.rhcloud.com:8000");
                socket.On(Socket.EVENT_CONNECT, () =>
                {
                    Thread t = new Thread(() => showChatMessage("Connected", "System"));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                });

                socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
                {
                    Thread t = new Thread(() => showChatMessage(data.ToString(), "Server"));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                });

                socket.On("ChatMessage", (data) =>
                {
                    Thread t = new Thread(() => showChatMessage(data.ToString(), "Server"));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();

                    if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!")
                    {
                        socket.Emit("MyNameIs", "Super Saiyan");
                        socket.Emit("ConnectToOtherPlayer");
                    }

                    if (!holdTurn)
                    {
                        if (data.ToString().Contains("first"))
                        {
                            player = true;
                            holdTurn = true;
                            connectSuccess = true;
                        }
                        else if (data.ToString().Contains("second"))
                        {
                            player = false;
                            holdTurn = true;
                            connectSuccess = true;
                        }     
                        
                        // Gửi qua bước đi trước
                        if (type == 4 && player && connectSuccess)
                        {
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                (ThreadStart)delegate ()
                                {
                                    Random rand = new Random();

                                    do
                                    {
                                        x = rand.Next(0, 11);
                                        y = rand.Next(0, 11);
                                    }
                                    while (arrSquare[x][y] == 2 || arrSquare[x][y] == 3);

                                    arrSquare[x][y] = 2;

                                    board = new ChessBoard(arrSquare);
                                    //board.ClickSquare += ClickChange;
                                    DataContext = board;

                                    if (IsEndGame(player))
                                    {
                                        MessageBox.Show("Người chơi 1 chiến thắng", "Thông báo");
                                        return;
                                    }

                                    // Gửi bước đi lên Server
                                    // Phương thức gửi bước đi của mình lên Server
                                    socket.Emit("MyStepIs", JObject.FromObject(new { row = x, col = y }));

                                    player = false;
                                });
                        }                   
                    }
                });

                socket.On(Socket.EVENT_ERROR, (data) =>
                {
                    Thread t = new Thread(() => showChatMessage(data.ToString(), "Server"));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                });

                socket.On("NextStepIs", (data) =>
                {
                    Thread t = new Thread(() => showChatMessage(data.ToString(), "Server"));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();

                    string[] partData = data.ToString().Split(',');
                    string[] user = partData[0].Split(' ');
                    if (type != 4)
                    {
                        if (int.Parse(user[3]) == 1)
                        {
                            string[] rowData = partData[1].Split(' ');
                            string[] colData = partData[2].Split(' ');
                            string colDataNum = "";
                            for (int i = 0; i < colData[3].Length; i++)
                            {
                                if (colData[3][i] >= '0' && colData[3][i] <= '9')
                                    colDataNum += colData[3][i];
                                else
                                    break;
                            }

                            rowServer = int.Parse(rowData[3]);
                            colServer = int.Parse(colDataNum);

                            //arrSquare[rowServer][colServer] = 3;

                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                (ThreadStart)delegate ()
                                {
                                    DoPlayerServer();

                                    if (IsEndGame(player))
                                    {
                                        MessageBox.Show("Người chơi 2 chiến thắng", "Thông báo");
                                        return;
                                    }

                                    if (isFullSquare())
                                    {
                                        MessageBox.Show("Gameover", "Thông báo");
                                        return;
                                    }

                                    player = true;
                                });
                        }
                    }
                    else if (type == 4 && connectSuccess)
                    {
                        if (!player)
                        {
                            if (int.Parse(user[3]) == 1)
                            {
                                string[] rowData = partData[1].Split(' ');
                                string[] colData = partData[2].Split(' ');
                                string colDataNum = "";
                                for (int i = 0; i < colData[3].Length; i++)
                                {
                                    if (colData[3][i] >= '0' && colData[3][i] <= '9')
                                        colDataNum += colData[3][i];
                                    else
                                        break;
                                }

                                rowServer = int.Parse(rowData[3]);
                                colServer = int.Parse(colDataNum);

                                //arrSquare[rowServer][colServer] = 3;

                                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                    (ThreadStart)delegate ()
                                    {
                                        DoPlayerServer();

                                        if (IsEndGame(player))
                                        {
                                            MessageBox.Show("Người chơi 2 chiến thắng", "Thông báo");
                                            return;
                                        }

                                        if (isFullSquare())
                                        {
                                            MessageBox.Show("Gameover", "Thông báo");
                                            return;
                                        }

                                        player = true;

                                        DoComputerServer();
                                    });
                            }
                        }
                    }
                });

                //bWorkerServer.DoWork += new DoWorkEventHandler(DoPlayerServer);
                //bWorkerServer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompleteComputerTurn);
                //bWorkerServer.RunWorkerAsync();
            }           
        }

        public MainWindow(int typeOption, string namePlayer)
        {
            InitializeComponent();

            // Khởi tạo mảng các ô cờ: Dùng để đánh dấu tình trạng của ô cờ
            for (int i = 0; i < ROW; i++)
                arrSquare[i] = new int[COL];

            SetIsStatic();

            board = new ChessBoard();
            board.ClickSquare += ClickChange;
            DataContext = board;

            userPlayer1.Name = namePlayer;
            type = typeOption;

            showChatMessage("Trò chơi bắt đầu", "System");

            if (type == 1 || type == 2)
            {
                bWorker.DoWork += new DoWorkEventHandler(DoComputer);
                bWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompleteComputerTurn);
                bWorker.RunWorkerAsync();
            }
            else if (type == 3 || type == 4) // Nếu kiểu chơi là 3 hoăc 4 sẽ tiến hành kết nối Server
            {
                socket = IO.Socket("ws://gomoku-lajosveres.rhcloud.com:8000");
                socket.On(Socket.EVENT_CONNECT, () =>
                {
                    Thread t = new Thread(() => showChatMessage("Connected", "System"));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                });

                socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
                {
                    Thread t = new Thread(() => showChatMessage(data.ToString(), "Server"));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                });

                socket.On("ChatMessage", (data) =>
                {
                    Thread t = new Thread(() => showChatMessage(data.ToString(), "Server"));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();

                    if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!")
                    {
                        socket.Emit("MyNameIs", userPlayer1.Name);
                        socket.Emit("ConnectToOtherPlayer");
                    }

                    if (!holdTurn)
                    {
                        if (data.ToString().Contains("first"))
                        {
                            player = true;
                            holdTurn = true;
                            connectSuccess = true;
                        }
                        else if (data.ToString().Contains("second"))
                        {
                            player = false;
                            holdTurn = true;
                            connectSuccess = true;
                        }

                        // Gửi qua bước đi trước
                        if (type == 4 && player && connectSuccess)
                        {
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                (ThreadStart)delegate ()
                                {
                                    Random rand = new Random();

                                    do
                                    {
                                        x = rand.Next(0, 11);
                                        y = rand.Next(0, 11);
                                    }
                                    while (arrSquare[x][y] == 2 || arrSquare[x][y] == 3);

                                    arrSquare[x][y] = 2;

                                    board = new ChessBoard(arrSquare);
                                    //board.ClickSquare += ClickChange;
                                    DataContext = board;

                                    if (IsEndGame(player))
                                    {
                                        MessageBox.Show("Người chơi 1 chiến thắng", "Thông báo");
                                        return;
                                    }

                                    // Gửi bước đi lên Server
                                    // Phương thức gửi bước đi của mình lên Server
                                    socket.Emit("MyStepIs", JObject.FromObject(new { row = x, col = y }));

                                    player = false;
                                });
                        }
                    }
                });

                socket.On(Socket.EVENT_ERROR, (data) =>
                {
                    Thread t = new Thread(() => showChatMessage(data.ToString(), "Server"));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                });

                socket.On("NextStepIs", (data) =>
                {
                    Thread t = new Thread(() => showChatMessage(data.ToString(), "Server"));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();

                    string[] partData = data.ToString().Split(',');
                    string[] user = partData[0].Split(' ');
                    if (type != 4)
                    {
                        if (int.Parse(user[3]) == 1)
                        {
                            string[] rowData = partData[1].Split(' ');
                            string[] colData = partData[2].Split(' ');
                            string colDataNum = "";
                            for (int i = 0; i < colData[3].Length; i++)
                            {
                                if (colData[3][i] >= '0' && colData[3][i] <= '9')
                                    colDataNum += colData[3][i];
                                else
                                    break;
                            }

                            rowServer = int.Parse(rowData[3]);
                            colServer = int.Parse(colDataNum);

                            //arrSquare[rowServer][colServer] = 3;

                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                (ThreadStart)delegate ()
                                {
                                    DoPlayerServer();

                                    if (IsEndGame(player))
                                    {
                                        MessageBox.Show("Người chơi 2 chiến thắng", "Thông báo");
                                        return;
                                    }

                                    if (isFullSquare())
                                    {
                                        MessageBox.Show("Gameover", "Thông báo");
                                        return;
                                    }

                                    player = true;
                                });
                        }
                    }
                    else if (type == 4 && connectSuccess)
                    {
                        if (!player)
                        {
                            if (int.Parse(user[3]) == 1)
                            {
                                string[] rowData = partData[1].Split(' ');
                                string[] colData = partData[2].Split(' ');
                                string colDataNum = "";
                                for (int i = 0; i < colData[3].Length; i++)
                                {
                                    if (colData[3][i] >= '0' && colData[3][i] <= '9')
                                        colDataNum += colData[3][i];
                                    else
                                        break;
                                }

                                rowServer = int.Parse(rowData[3]);
                                colServer = int.Parse(colDataNum);

                                //arrSquare[rowServer][colServer] = 3;

                                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                    (ThreadStart)delegate ()
                                    {
                                        DoPlayerServer();

                                        if (IsEndGame(player))
                                        {
                                            MessageBox.Show("Người chơi 2 chiến thắng", "Thông báo");
                                            return;
                                        }

                                        if (isFullSquare())
                                        {
                                            MessageBox.Show("Gameover", "Thông báo");
                                            return;
                                        }

                                        player = true;

                                        DoComputerServer();
                                    });
                            }
                        }
                    }
                });

                //bWorkerServer.DoWork += new DoWorkEventHandler(DoPlayerServer);
                //bWorkerServer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompleteComputerTurn);
                //bWorkerServer.RunWorkerAsync();
            }
        }

        // Khởi tạo tình trạng các ô ban đầu
        public void SetIsStatic()
        {
            for (int i = 0; i < ROW; i++)
                for (int j = 0; j < COL; j++)
                    arrSquare[i][j] = (i + j) % 2;
        }

        // Sự kiện click chuột vào ô vuông
        private void ClickChange(int row, int col)
        {
            if (!endGame)
            {
                switch (type)
                {
                    case 1: // 2 người chơi
                        {
                            if (player)
                            {
                                if (arrSquare[row][col] == 2 || arrSquare[row][col] == 3)
                                    break;

                                arrSquare[row][col] = 2;
                                board = new ChessBoard(arrSquare);
                                board.ClickSquare += ClickChange;
                                DataContext = board;

                                if (IsEndGame(player))
                                {
                                    MessageBox.Show("Người chơi 1 chiến thắng", "Thông báo");
                                    return;
                                }

                                if (isFullSquare())
                                {
                                    MessageBox.Show("Gameover", "Thông báo");
                                    return;
                                }

                                player = false;
                            }
                            else
                            {
                                if (arrSquare[row][col] == 2 || arrSquare[row][col] == 3)
                                    break;

                                arrSquare[row][col] = 3;
                                board = new ChessBoard(arrSquare);
                                board.ClickSquare += ClickChange;
                                DataContext = board;

                                if (IsEndGame(player))
                                {
                                    MessageBox.Show("Người chơi 2 chiến thắng", "Thông báo");
                                    return;
                                }

                                if (isFullSquare())
                                {
                                    MessageBox.Show("Gameover", "Thông báo");
                                    return;
                                }

                                player = true;
                            }

                            break;
                        }
                    case 2: // Người và máy 
                        {
                            if (player)
                            {
                                if (arrSquare[row][col] == 2 || arrSquare[row][col] == 3)
                                    break;

                                arrSquare[row][col] = 2;
                                board = new ChessBoard(arrSquare);
                                board.ClickSquare += ClickChange;
                                DataContext = board;

                                if (IsEndGame(player))
                                {
                                    MessageBox.Show("Người chơi 1 chiến thắng", "Thông báo");
                                    return;
                                }

                                if (isFullSquare())
                                {
                                    MessageBox.Show("Gameover", "Thông báo");
                                    return;
                                }

                                player = false;

                                bWorker.RunWorkerAsync();
                            }

                            break;
                        }
                    case 3: // 2 người đánh qua mạng
                        {
                            if (player && connectSuccess)
                            {
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                    (ThreadStart)delegate ()
                                    {
                                        if (arrSquare[row][col] == 2 || arrSquare[row][col] == 3)
                                            return;

                                        arrSquare[row][col] = 2;
                                        board = new ChessBoard(arrSquare);
                                        board.ClickSquare += ClickChange;
                                        DataContext = board;

                                        if (IsEndGame(player))
                                        {
                                            MessageBox.Show("Người chơi 1 chiến thắng", "Thông báo");
                                            return;
                                        }

                                        if (isFullSquare())
                                        {
                                            MessageBox.Show("Gameover", "Thông báo");
                                            return;
                                        }

                                        // Gửi bước đi lên Server
                                        // Phương thức gửi bước đi của mình lên Server
                                        socket.Emit("MyStepIs", JObject.FromObject(new { row = row, col = col }));

                                        player = false;
                                    });
                            }

                            break;
                        }
                    case 4: // Máy đánh qua mạng
                        {
                            if (player && connectSuccess)
                            {

                            }
                            else
                            {

                            }

                            break;
                        }
                }
            }
        }

        /*
         * ---------------------------
         * Xử lý máy chơi 
         * ---------------------------
         */
        int x, y;
        private void DoComputer(object sender, DoWorkEventArgs e)
        {
            Random rand = new Random();

            do
            {
                x = rand.Next(0, 11);
                y = rand.Next(0, 11);
            }
            while (arrSquare[x][y] == 2 || arrSquare[x][y] == 3);

            arrSquare[x][y] = 3;

            board = new ChessBoard(arrSquare);
            board.ClickSquare += ClickChange;
            DataContext = board;

        }

        /*
         * -----------------------------------
         * Xử lý người chơi trên Server
         * -----------------------------------
         */
        //private void DoPlayerServer(object sender, DoWorkEventArgs e)
        private void DoPlayerServer()
        {
            arrSquare[rowServer][colServer] = 3;

            board = new ChessBoard(arrSquare);
            board.ClickSquare += ClickChange;
            DataContext = board;
        }

        private void DoComputerServer()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    Random rand = new Random();

                    do
                    {
                        x = rand.Next(0, 11);
                        y = rand.Next(0, 11);
                    }
                    while (arrSquare[x][y] == 2 || arrSquare[x][y] == 3);

                    arrSquare[x][y] = 2;

                    board = new ChessBoard(arrSquare);
                    //board.ClickSquare += ClickChange;
                    DataContext = board;

                    if (IsEndGame(player))
                    {
                        MessageBox.Show("Người chơi 1 chiến thắng", "Thông báo");
                        return;
                    }

                    if (isFullSquare())
                    {
                        MessageBox.Show("Gameover", "Thông báo");
                        return;
                    }

                    // Gửi bước đi lên Server
                    // Phương thức gửi bước đi của mình lên Server
                    socket.Emit("MyStepIs", JObject.FromObject(new { row = x, col = y }));

                    player = false;
                });
        }

        private void CompleteComputerTurn(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsEndGame(player))
            {
                MessageBox.Show("Người chơi 2 chiến thắng", "Thông báo");
                return;
            }

            if (isFullSquare())
            {
                MessageBox.Show("Gameover", "Thông báo");
                return;
            }

            player = true;
        }

        // Kiểm tra kết thúc
        // Kiểm tra đã đi hết tất cả các ô chưa
        public bool isFullSquare()
        {
            int count = 0;

            for (int i = 0; i < ROW; i++)
                for (int j = 0; j < COL; j++)
                    if (arrSquare[i][j] == 2 || arrSquare[i][j] == 3)
                        count++;

            if (count == ROW * COL)
                return true;

            return false;
        }

        // Kiểm tra có ai thắng không?
        public bool IsEndGame(bool player)
        {
            int check;  // Biến giữ giá trị ô vuông mà player đánh
            if (player) // Nếu player = true (người) , player = false (máy) 
                check = 2;
            else
                check = 3;

            for (int i = 0; i < ROW; i++)
                for (int j = 0; j < COL; j++)
                {
                    if ((j < COL - 4) && (arrSquare[i][j] == check) && (arrSquare[i][j + 1] == check) && (arrSquare[i][j + 2] == check) &&
                        (arrSquare[i][j + 3] == check) && (arrSquare[i][j + 4] == check))
                        return endGame = true;

                    if ((i < ROW - 4) && arrSquare[i][j] == check && arrSquare[i + 1][j] == check && arrSquare[i + 2][j] == check &&
                        arrSquare[i + 3][j] == check && arrSquare[i + 4][j] == check)
                        return endGame = true;

                    if ((j < COL - 4) && (i < ROW - 4) && arrSquare[i][j] == check && arrSquare[i + 1][j + 1] == check && arrSquare[i + 2][j + 2] == check &&
                        arrSquare[i + 3][j + 3] == check && arrSquare[i + 4][j + 4] == check)
                        return endGame = true;

                    if (j >= 4 && (i < ROW - 4) && arrSquare[i][j] == check && arrSquare[i + 1][j - 1] == check && arrSquare[i + 2][j - 2] == check &&
                        arrSquare[i + 3][j - 3] == check && arrSquare[i + 4][j - 4] == check)
                        return endGame = true;
                }

            return false;
        }

        // ----

        /*
         * --------------------------------------------------------------------------------------
         * Xử lý Chat Message
         * Chức năng được phát huy khi đấu Gomoku với Server
         * --------------------------------------------------------------------------------------
         */
        // Chức năng: Khi người dùng chưa đặt tên YourName mà sau đó Click chuột ra ngoài thì để lại mặc đinh tên là Guest
        private void txtYourName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtYourName.Text == "")
            {
                txtYourName.Text = "Guest";
            }
        }

        // Chức năng: Kiểm tra người dùng đã nhập thông điệp để gửi (Dùng biến mesChange để kiểm tra nội dung trước khi Send)
        private void txtMessage_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMessage.Text == "")
            {
                mesChanged = false;
                txtMessage.Foreground = new SolidColorBrush(Color.FromRgb(135, 135, 135));
                txtMessage.Text = "Type your message here...";
            }
            else
            {
                mesChanged = true;
            }
        }

        // Chức năng: Khi nhấn vào để thay đổi thông điệp thì dòng chữ hướng dẫn sẽ biến mất (Placeholder)
        private void txtMessage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtMessage.Text == "Type your message here...")
            {
                txtMessage.Text = "";
                txtMessage.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        // Chức năng: Thay đổi tên người dùng
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (txtYourName.Text == "")
            {
                txtYourName.Text = "Guest";
            }
            else
            {
                userPlayer1.Name = txtYourName.Text;
            }

            if (type == 1 || type == 2)
            {

            }
            else
            {

            }
        }

        // Chức năng: Gửi thông điệp
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (type == 1 || type == 2)
            {
                if (txtMessage.Text != "" && mesChanged)
                {
                    ChatMessage chatMessage = new ChatMessage(userPlayer1.Name, DateTime.Now.ToString("hh:mm:ss tt"), txtMessage.Text);
                    chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    chatBox.Items.Add(chatMessage);
                }
            }
            else
            {
                if (txtMessage.Text != "" && mesChanged)
                {
                    ChatMessage chatMessage = new ChatMessage(userPlayer1.Name, DateTime.Now.ToString("hh:mm:ss tt"), txtMessage.Text);
                    chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    chatBox.Items.Add(chatMessage);

                    // -------- Bổ sung phương thức mình gửi message lên Server
                    socket.Emit("ChatMessage", txtMessage.Text);
                }
            }
        }

        public void showChatMessage(string message, string userName)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    ChatMessage chatMessage = new ChatMessage(userName, DateTime.Now.ToString("hh:mm:ss tt"), message);
                    chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    chatBox.Items.Add(chatMessage);
                });
        }
    }
}
