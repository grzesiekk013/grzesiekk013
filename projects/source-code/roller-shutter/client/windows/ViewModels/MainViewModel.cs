using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;


using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace WpfApp2.ViewModels
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        protected bool SetProperty<T>(ref T backingStore, T value,
          [CallerMemberName] string propertyName = "",
          Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }


        public MainViewModel()
        {

            ActivIndic = "Hidden";
            ProgressValue = 0;
            ServerIP = "";
            Polaczono = "Brak połączenia";
            ButtonsAvaliable = "True";
            FrameVisible = "False";
            RefreshTime = "10";
            CoreLocked = true;
            clearCells();
            #region file read
            //file
            readSettings();
            #endregion
            readData("/");
            #region thread
            //thread
            Thread InstanceCaller = new Thread(
            new ThreadStart(ThreadMethod));

            // Start the thread.
            InstanceCaller.Start();

            Thread InstanceCaller1 = new Thread(
            new ThreadStart(ThreadMethod1));

            // Start the thread.
            InstanceCaller1.Start();
            #endregion
        }


        public static class Call
        {
            public static string called;
        }
        #region thread
        public async void ThreadMethod1()
        {
            while (true)
            {
                if (Call.called != "")
                {
                    if (
                   Call.called == "brama" || Call.called == "garaz" || Call.called == "wszystkie/do_gory" || Call.called
                       == "wszystkie/na_dol" || Call.called == "wejsciowe/do_gory" || Call.called == "wejsciowe/na_dol" || Call.called == "refresh")
                    {
                        if (Call.called == "refresh")
                        {
                            btnRefreshAction("");
                        }
                        else
                        {
                            btnClickedAction(Call.called);
                        }
                        Call.called = "";
                    }
                }
               
                if (!CoreLocked)
                {
                    ProgressValue = 10;
                    readData(CoreString); //read data

                    RefreshTime = "10";

                    ServerIP = MyGlobals.serverAddress;
                    ServerPort = MyGlobals.serverPort;
                 
                    if (MyGlobals.responseArray[0].Length != 0)
                    {
                        SSIDRoutera = MyGlobals.responseArray[0];
                        if (MyGlobals.responseArray[1] == "ugory")
                        {
                            WszystkieStatus = "U góry";
                        }
                        if (MyGlobals.responseArray[1] == "nadole")
                        {
                            WszystkieStatus = "Na dole";
                        }

                        if (MyGlobals.responseArray[2] == "ugory")
                        {
                            WejscioweStatus = "U góry";
                        }
                        if (MyGlobals.responseArray[2] == "nadole")
                        {
                            WejscioweStatus = "Na dole";
                        }
                        if (MyGlobals.responseArray[5] != "")
                        {
                            if (MyGlobals.responseArray[3] == "0")
                            {
                                StatusCzujnik = "Noc";
                            }
                            if (MyGlobals.responseArray[3] == "1")
                            {
                                StatusCzujnik = "Dzień";
                            }
                            OtwarciePrzedzial = MyGlobals.responseArray[5] + ":00 - " + MyGlobals.responseArray[6] + ":00";
                            ZamknieciePrzedzial = MyGlobals.responseArray[7] + ":00 - " + MyGlobals.responseArray[8] + ":00";
                        }

                        for (int i = 0; i < MyGlobals.responseArray.Length; i++)
                        {
                            MyGlobals.responseArray[i] = "";
                        }
                    }
                    ActivIndic = "Hidden";
                    CoreLocked = true;
                }
            }
        }
        public async void ThreadMethod()
        {
            while (true)
            {
                Thread.Sleep(1000);

                if (RefreshTime == "Aktualizuję...")
                {
                    RefreshTime = "10";
                }
                if (RefreshTime != "0" && RefreshTime != "Aktualizuję...")
                {
                    int t = int.Parse(RefreshTime);
                    t--;
                    RefreshTime = t.ToString();
                }
                if (RefreshTime == "0")
                {
                    RefreshTime = "Aktualizuję...";
                    refreshAsync();
                }

            }
        }
        #endregion
        #region functions
        public bool establishConnection()
        {
            ProgressValue += 10;
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            MyGlobals.options.DontFragment = true;
            
            try
            {
                PingReply reply = MyGlobals.pingSender.Send(MyGlobals.serverAddress, 10000, buffer, MyGlobals.options);
                if (reply.Status != IPStatus.Success)
                {
                    clearCells();
                    Polaczono = "Brak połączenia";
                    return false;
                }
                else
                {
                    return true;
                }
            } catch (Exception ex){
                Console.WriteLine(ex.ToString());
                return false;
            }
            
        }
        public void clearCells() { StatusCzujnik = ""; RefreshTime = ""; SSIDRoutera = ""; WszystkieStatus = ""; WejscioweStatus = ""; OtwarciePrzedzial = ""; ZamknieciePrzedzial = ""; RefreshTime = "10"; }
        #endregion

        #region button actions



        public void btnClickedAction(string obj) //btn like do gory
        {
            Console.WriteLine(obj);
            ActivIndic = "Visible";

            if (obj == "brama" || obj == "garaz" || obj == "wszystkie/do_gory" || obj == "wszystkie/na_dol" || obj == "wejsciowe/do_gory" || obj == "wejsciowe/na_dol")
            {

                if (obj.ToString() == "brama" || obj.ToString() == "garaz")
                {
                    obj = obj.ToString() + "/" + SettingsViewModel.MyGlobalsSettings.key;
                }

                Console.WriteLine("/" + obj.ToString());


                CoreString = "/" + obj.ToString();
                CoreLocked = false;
            }

        }
        private void btnOkAction(object obj)
        {
            FrameVisible = "False";
        }
        #endregion

        #region public strings


        #region public strings

        private bool _coreLocked;
        public bool CoreLocked
        {
            get { return _coreLocked; }
            set { SetProperty(ref _coreLocked, value); }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get { return _progressValue; }
            set { SetProperty(ref _progressValue, value); }
        }

        private string _coreString;
        public string CoreString
        {
            get { return _coreString; }
            set { SetProperty(ref _coreString, value); }
        }


        private string _popupVisible;
        public string FrameVisible
        {
            get { return _popupVisible; }
            set { SetProperty(ref _popupVisible, value); }
        }
        private string _activcIndic;
        public string ActivIndic
        {
            get { return _activcIndic; }
            set { SetProperty(ref _activcIndic, value); }
        }

        private bool _refreshUnlocked;
        public bool refreshUnlocked
        {
            get { return _refreshUnlocked; }
            set { SetProperty(ref _refreshUnlocked, value); }
        }

        private string _serverIP;
        public string ServerIP
        {
            get { return _serverIP; }
            set { SetProperty(ref _serverIP, value); }
        }

        private string _refreshTime;
        public string RefreshTime
        {
            get { return _refreshTime; }
            set { SetProperty(ref _refreshTime, value); }
        }
        private string _statusCzujnik;
        public string StatusCzujnik
        {
            get { return _statusCzujnik; }
            set { SetProperty(ref _statusCzujnik, value); }
        }
        private string _ssidRoutera;
        public string SSIDRoutera
        {
            get { return _ssidRoutera; }
            set { SetProperty(ref _ssidRoutera, value); }
        }
        private string _wszystkieStatus;
        public string WszystkieStatus
        {
            get { return _wszystkieStatus; }
            set { SetProperty(ref _wszystkieStatus, value); }
        }
        private string _wejscioweStatus;
        public string WejscioweStatus
        {
            get { return _wejscioweStatus; }
            set { SetProperty(ref _wejscioweStatus, value); }
        }
        private string _otwarciePrzedzial;
        public string OtwarciePrzedzial
        {
            get { return _otwarciePrzedzial; }
            set { SetProperty(ref _otwarciePrzedzial, value); }
        }
        private string _zamknieciePrzedzial;
        public string ZamknieciePrzedzial
        {
            get { return _zamknieciePrzedzial; }
            set { SetProperty(ref _zamknieciePrzedzial, value); }
        }

        private string _polaczono;
        public string Polaczono
        {
            get { return _polaczono; }
            set { SetProperty(ref _polaczono, value); }
        }
        private string _serverPort;
        public string ServerPort
        {
            get { return _serverPort; }
            set { SetProperty(ref _serverPort, value); }
        }
        private string _buttonsAvaliable;
        public string ButtonsAvaliable
        {
            get { return _buttonsAvaliable; }
            set { SetProperty(ref _buttonsAvaliable, value); }
        }
        #endregion
        #endregion
        private async void btnRefreshAction(object obj) { RefreshTime = "0"; }
        #region public globals
        public class MyGlobals
        {
            public static string serverAddress = "";
            public static string serverPort = "";
            public static string[] responseArray = new string[16] {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};
            public static HttpClient client = new HttpClient();
            public static Ping pingSender = new Ping();
            public static PingOptions options = new PingOptions();
        }
        #endregion
        #region readSettings

        public void readSettings()
        {
            string[] arr = new string[2];
            if (!File.Exists(Path.Combine("key.txt")))
            {
                File.Create(Path.Combine("key.txt")).Close();
            }
            else
            {
                SettingsViewModel.MyGlobalsSettings.key = File.ReadAllText(Path.Combine("key.txt"));
            }

            if (!File.Exists(Path.Combine("settings.txt")))
            {
                Console.WriteLine("not exists");
                try
                {
                    File.Create(Path.Combine("settings.txt")).Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    arr[0] = "127.0.0.1";
                    arr[1] = "80";
                    File.WriteAllLines(Path.Combine("settings.txt"), arr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                try
                {
                    arr[0] = File.ReadAllLines(Path.Combine("settings.txt"))[0];
                    arr[1] = File.ReadAllLines(Path.Combine("settings.txt"))[1];
                    MyGlobals.serverAddress = arr[0];
                    MyGlobals.serverPort = arr[1];

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    arr[0] = "127.0.0.1";
                    arr[1] = "80";
                    File.WriteAllLines(Path.Combine("settings.txt"), arr);
                    arr[0] = File.ReadAllLines(Path.Combine("settings.txt"))[0];
                    arr[1] = File.ReadAllLines(Path.Combine("settings.txt"))[1];
                    MyGlobals.serverAddress = arr[0];
                    MyGlobals.serverPort = arr[1];
                }

            }
            ServerIP = arr[0];
            ServerPort = arr[1];
        }
        #endregion
        public void readData(string request)
        {
            readSettings();
            ProgressValue = 20;
            if (request != "/")
            {
                ActivIndic = "Visible";
            }
            if (establishConnection())
            {

                _ = readAsync(request);
            }
            else
            {
                if (request != "/")
                {
                
                    FrameVisible = "True";

                }
            }


        }
        #region refreshAsync using by thread
        public void refreshAsync()
        {
            //clear response
            for (int i = 0; i < MyGlobals.responseArray.Length; i++)
            {
                MyGlobals.responseArray[i] = "";
            }
            readData("/");//read data
            RefreshTime = "10";
        }
        #endregion
        #region readAsync used by buttons
        async Task readAsync(string req)
        {
            string responseString = "";
            string request;
            string message;
            for (int i = 0; i < MyGlobals.responseArray.Length; i++)
            {
                MyGlobals.responseArray[i] = "";
            }

            request = "http://" + MyGlobals.serverAddress + ":" + MyGlobals.serverPort + "/" + req;
            try
            {
                ProgressValue += 10;
                responseString = await MyGlobals.client.GetStringAsync(request);

                message = "Success";
            }
            catch (Exception ex)
            {
                message = "READ ERROR" + ex;
                ButtonsAvaliable = "False";
            }
            Console.WriteLine(message);


            if (responseString != "")
            {
                ProgressValue += 10;
                string[] arr = new string[16];
                responseString = responseString.Replace("\n", "");
                responseString = responseString.Replace("\r", "");
                responseString = responseString.Replace("<br>", "");
                responseString = responseString.Replace(" ", "");
                responseString = responseString.Replace("<!DOCTYPEHTML><html><head><linkrel=\"icon\"href=\"data:,\"></head><body>", "");
                responseString = responseString.Replace("PolaczonodoWiFi:", "");
                responseString = responseString.Replace("Statuswszystkichrolet:", "");
                responseString = responseString.Replace("Statuswejsciowychrolet:", "");
                responseString = responseString.Replace("Statusczujnikazmierzchu:", "");
                responseString = responseString.Replace("Lokalnyadresip:", "");
                responseString = responseString.Replace("Otwieraodgodziny:", "");
                responseString = responseString.Replace("Otwieradogodziny:", "");
                responseString = responseString.Replace("Zamykaodgodziny:", "");
                responseString = responseString.Replace("Zamykadogodziny:", "");
                responseString = responseString.Replace("Czyczasustawionypoprawnie:", "");
                responseString = responseString.Replace("</body></html>", "");
                arr = responseString.Split(';');
                for (int i = 0; i < arr.Length; i++)
                {
                    MyGlobals.responseArray[i] = arr[i];
                }

                if (MyGlobals.responseArray[0] != "")
                {
                    SSIDRoutera = MyGlobals.responseArray[0];
                    if (MyGlobals.responseArray[1] == "ugory")
                    {
                        WszystkieStatus = "U góry";
                    }
                    if (MyGlobals.responseArray[1] == "nadole")
                    {
                        WszystkieStatus = "Na dole";
                    }

                    if (MyGlobals.responseArray[2] == "ugory")
                    {
                        WejscioweStatus = "U góry";
                    }
                    if (MyGlobals.responseArray[2] == "nadole")
                    {
                        WejscioweStatus = "Na dole";
                    }

                    if (MyGlobals.responseArray[5] != "")
                    {
                        Polaczono = "Połączono";
                        ButtonsAvaliable = "True";
                        if (MyGlobals.responseArray[3] == "0")
                        {
                            StatusCzujnik = "Noc";
                        }
                        if (MyGlobals.responseArray[3] == "1")
                        {
                            StatusCzujnik = "Dzień";
                        }
                        OtwarciePrzedzial = MyGlobals.responseArray[5] + ":00 - " + MyGlobals.responseArray[6] + ":00";
                        ZamknieciePrzedzial = MyGlobals.responseArray[7] + ":00 - " + MyGlobals.responseArray[8] + ":00";
                    }
                    else
                    {
                        ButtonsAvaliable = "False";
                    }
                    for (int i = 0; i < MyGlobals.responseArray.Length; i++)
                    {
                        MyGlobals.responseArray[i] = "";
                    }
                }

            }
            else
            {
                Console.WriteLine("Response is empty");
                Polaczono = "Brak połączenia";
                ButtonsAvaliable = "False";
            }
            ProgressValue = 0;
        }
        #endregion
    }
}
