using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace MediaCoinClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        Requests requests = new Requests();
        SpeedConvert Bitconverter = new SpeedConvert();
        Thread info_thread;
        string server = Properties.Settings.Default.server;
        public MainWindow()
        {
            logger.Info("Program started.");
            InitializeComponent();
            logger.Info("Init success.");

            
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            auth_key.Text = Properties.Settings.Default.Access_Key;
            machine_name.Text = Properties.Settings.Default.machine_name;
        }


        private void NetworkStat()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate { this.status_string.Content = "Подключаемся к MediaCoin..."; });
            while (true)
            {
                try
                {
                    var res = requests.Get("http://127.0.0.1:8102/-/info?type=debug");
                    if (res["status"].ToString() == "error") { }
                    else {
                        Application.Current.Dispatcher.Invoke((Action)delegate { this.status_string.Content = "Подключение установлено!"; });
                        logger.Debug("MediaCoin connected.");
                        break; }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(500);
            }
            Application.Current.Dispatcher.Invoke((Action)delegate { this.status_string.Content = "Подключаемся к серверу мониторинга..."; });
            while (true)
            {
                try
                {
                    requests.Get(server + "/robot.php?type=login&access_key=" + Properties.Settings.Default.Access_Key + "&machine_name=" + Properties.Settings.Default.machine_name.Replace("\n", "") + "&nickname=none");
                    Application.Current.Dispatcher.Invoke((Action)delegate { this.status_string.Content = "Подключение установлено!"; });
                    logger.Debug("Monitoring server connected!");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(500);
            }
            Thread.Sleep(1000);
            Application.Current.Dispatcher.Invoke((Action)delegate { this.status_string.Content = "В работе"; });
            while (true) {
                try
                { 
                    var res = requests.Get("http://127.0.0.1:8102/-/netinfo");
                    
                    string data_in = Bitconverter.ConvertFromBytes(Convert.ToInt32(res.GetValue("speedIn").ToString())).ToString();
                    string data_out = Bitconverter.ConvertFromBytes(Convert.ToInt32(res.GetValue("speedOut").ToString())).ToString();
                    string raw_in = Convert.ToInt32(res.GetValue("speedIn")).ToString();
                    string raw_out = Convert.ToInt32(res.GetValue("speedOut")).ToString();
                    Application.Current.Dispatcher.Invoke((Action)delegate { this.upload_speed_item.Content = "Отдача: " + data_out; });
                    Application.Current.Dispatcher.Invoke((Action)delegate { this.download_speed_item.Content = "Загрузка: " + data_in; });
                    var res2 = requests.Get("http://127.0.0.1:8102/-/info?type=debug");
                    Application.Current.Dispatcher.Invoke((Action)delegate { this.Balance.Content = "Баланс: " + (Convert.ToInt64(res2["sess"]["balance"]) / 1000000000).ToString() + " MDC"; });
                    Application.Current.Dispatcher.Invoke((Action)delegate { this.Login.Content = "Аккаунт: " + res2["sessNick"].ToString(); });
                    var fs_size = Bitconverter.ConvertFromBytesFs(Convert.ToInt64(res2["fs"]["total_size"])).ToString();
                    var fs_size_raw = Convert.ToInt64(res2["fs"]["total_size"]).ToString();
                    Application.Current.Dispatcher.Invoke((Action)delegate { this.total_files_size.Content = "Всего занято места: " + fs_size; });
                    Console.WriteLine("Get stat");
                    var res3 = requests.Get("http://127.0.0.1:8102/-/netinfo");
                    Console.WriteLine(server + "/robot.php?type=update&access_key=" + Properties.Settings.Default.Access_Key.Replace("\n", "") + "&machine_name=" + Properties.Settings.Default.machine_name.Replace("\r\n", "") + "&speed_in=" + data_in + "&speed_out=" + data_out + "&fs_size=" + fs_size + "&nickname=" + res2["sessNick"].ToString() + "&balance=" + (Convert.ToInt64(res2["sess"]["balance"]) / 1000000000).ToString() + "&charts=" + res3.ToString() + "&speed_in_raw=" + Convert.ToInt32(res.GetValue("speedIn").ToString()) + "&speed_out_raw=" + Convert.ToInt32(res.GetValue("speedOut").ToString()));
                  
                    requests.Get(server + "/robot.php?type=update&access_key=" + Properties.Settings.Default.Access_Key.Replace("\n", "") + "&machine_name=" + Properties.Settings.Default.machine_name.Replace("\r\n", "") + "&speed_in=" + raw_in + "&speed_out=" + raw_out + "&fs_size=" + fs_size_raw + "&nickname=" + res2["sessNick"].ToString() + "&balance=" + (Convert.ToInt64(res2["sess"]["balance"])).ToString() + "&charts=" + res3.ToString() + "&speed_in_raw=" + Convert.ToInt32(res.GetValue("speedIn").ToString()) + "&speed_out_raw=" + Convert.ToInt32(res.GetValue("speedOut").ToString()));
                    
                    Application.Current.Dispatcher.Invoke((Action)delegate { this.status_string.Content = "В работе"; });
                    Thread.Sleep(1000);
                }
                catch (System.Net.WebException ex)
                {
                    logger.Error(ex.ToString());
                    Application.Current.Dispatcher.Invoke((Action)delegate { this.status_string.Content = "Ошибка подключения к серверу. Переподключение..."; });
                    Thread.Sleep(500);
                }
            }
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Program started.");
            Properties.Settings.Default.Access_Key = auth_key.Text;
            Properties.Settings.Default.machine_name = machine_name.Text;
            Properties.Settings.Default.Save();
            info_thread = new Thread(new ThreadStart(NetworkStat));
            info_thread.Start();

        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            status_string.Content = "Ожидание остановки...";
            info_thread.Abort();
            while (info_thread.IsAlive)
            {
                //none
            }
            status_string.Content = "Остановлен";
            logger.Info("Program stopped.");
        }
    }
}
