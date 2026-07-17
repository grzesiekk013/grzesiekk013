using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

using System.Diagnostics;

using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp2.ViewModels
{
    public class SettingsViewModel
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
        #region SettingsViewModel.MyGlobalsSettings
        public class MyGlobalsSettings
        {
            public static string IP = "";
            public static string Port = "";
            public static string key = "";
        }
        #endregion
        public SettingsViewModel()
        {
           
            Locked = true;
            LockedKey = true;
            ServerAddress = "";
            ServerPort = "";
            FrameVisible = "False";


            string[] arr = new string[2];
            if (!File.Exists(Path.Combine( "key.txt")))
            {
                File.Create(Path.Combine( "key.txt")).Close();
            }
            else
            {
                SettingsViewModel.MyGlobalsSettings.key = File.ReadAllText(Path.Combine( "key.txt"));
                Key = SettingsViewModel.MyGlobalsSettings.key;
            }

            if (!File.Exists(Path.Combine( "settings.txt")))
            {
                try
                {
                    File.Create(Path.Combine( "settings.txt")).Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    arr[0] = "127.0.0.1";
                    arr[1] = "80";
                    File.WriteAllLines(Path.Combine( "settings.txt"), arr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                arr[0] = File.ReadAllLines(Path.Combine( "settings.txt"))[0];
                arr[1] = File.ReadAllLines(Path.Combine( "settings.txt"))[1];
            }
            SettingsViewModel.MyGlobalsSettings.IP = arr[0];
            SettingsViewModel.MyGlobalsSettings.Port = arr[1];
            ServerAddress = arr[0];
            ServerPort = arr[1];
           // MyGlobals.serverPort = ServerPort;
           // MyGlobals.serverAddress = ServerAddress;

        }

        private void btnOkAction(object obj)
        {
            FrameVisible = "False";
        }
        #region button actions
        public async void btnSaveAction(object obj)
        {
            string[] arr = new string[2];
            arr[0] = ServerAddress;
            arr[1] = ServerPort;
            File.WriteAllLines(Path.Combine( "settings.txt"), arr);
            File.WriteAllText(Path.Combine( "key.txt"), Key);
            SettingsViewModel.MyGlobalsSettings.key = Key;
            LockedKey = true;
            Locked = true;
            FrameVisible = "True";
        }
        #endregion
        public void readSettings()
        {
            string[] arr = new string[2];
            if (!File.Exists(Path.Combine( "settings.txt")))
            {

                File.Create(Path.Combine( "settings.txt"));
                try
                {
                    File.WriteAllLines(Path.Combine( "settings.txt"), arr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {

                arr[0] = File.ReadAllLines(Path.Combine( "settings.txt"))[0];
                arr[1] = File.ReadAllLines(Path.Combine( "settings.txt"))[1];
                Console.WriteLine(arr[0] + arr[1]);
            }
            //MyGlobals.serverAddress = arr[0];
           // MyGlobals.serverPort = arr[1];
            Console.WriteLine("server: " + arr[0] + ", port: " + arr[1]);
        }

        #region public strings
        private string _popupVisible;
        public string FrameVisible
        {
            get { return _popupVisible; }
            set { SetProperty(ref _popupVisible, value); }
        }

        private string _popupText;
        public string PopupText
        {
            get { return _popupText; }
            set { SetProperty(ref _popupText, value); }
        }

        private string _serverAddress;
        public string ServerAddress
        {
            get { return _serverAddress; }
            set { SetProperty(ref _serverAddress, value); }
        }
        private string _serverPort;
        public string ServerPort
        {
            get { return _serverPort; }
            set { SetProperty(ref _serverPort, value); }
        }

        private bool _locked;
        public bool Locked
        {
            get { return _locked; }
            set { SetProperty(ref _locked, value); }
        }

        private bool _lockedKey;
        public bool LockedKey
        {
            get { return _lockedKey; }
            set { SetProperty(ref _lockedKey, value); }
        }

        private string _key;
        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }
        #endregion
    }
}


