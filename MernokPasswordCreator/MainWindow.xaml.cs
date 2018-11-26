using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MernokPasswords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string V = @"C:\Passwords\MernokPasswordMasterList.xml";
        public static MernokPasswordFile mernokPasswordFile = new MernokPasswordFile();
        private string PaswordString = "";
        private PasswordCreator PasswordCreator = null;
        private PasswordDecriptor passwordDecriptor = null;

        private ObservableCollection<string> _AccessLevelList;
        public ObservableCollection<string> AccessLevelList
        {
            get { return _AccessLevelList; }
            set { _AccessLevelList = value; PasswordCreator.OnPropertyChanged("AccessLevelList"); }
        }


        public uint LastUID = 0;



        public MainWindow()
        {
            _AccessLevelList = new ObservableCollection<string>();
            InitializeComponent();
            PasswordCreator = new PasswordCreator();
            passwordDecriptor = new PasswordDecriptor();
            this.DataContext = PasswordCreator;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
 //           PasswordCreator.RequestName = "hello";
 //           PasswordCreator.CreatorName = "daar";
            string path = V;


            foreach (PasswordCreator.AccessLevel_enum item in Enum.GetValues(typeof(PasswordCreator.AccessLevel_enum)))
            {
                AccessLevelList.Add(item.ToString().Replace("_", " "));                
            }

            AccessLevelCbx.ItemsSource = AccessLevelList;
            AccessLevelCbx.SelectedIndex = 0;

            if (File.Exists(path))
            {
                mernokPasswordFile = MernokPasswordManager.ReadMernokPasswordFile(path);
                LastUID = (uint)mernokPasswordFile.mernokPasswordList.Count();
            }
            else
            {
                mernokPasswordFile.createdBy = "NeilPretorius";
                mernokPasswordFile.dateCreated = DateTime.Now;
                mernokPasswordFile.version = 1;

                List<MernokPassword> mernokPasswords = new List<MernokPassword>();
                MernokPassword AdminPass = new MernokPassword
                {
                    CreatorName = "NEIL",
                    creatorUID = 45,
                    requestName = "MERNOK",
                    requesterUID = 1,
                    Password = PasswordCreator.PassWordCreate(45,"NEIL",1,"MERNOK"),
                    AccessLevel = (byte)(PasswordCreator.AccessLevel_enum)Enum.Parse(typeof(PasswordCreator.AccessLevel_enum), "Mernok_Engineer")
            };

                mernokPasswords.Add(AdminPass);
                mernokPasswordFile.mernokPasswordList = mernokPasswords;
                MernokPasswordManager.CreateMernokAssetFile(mernokPasswordFile);

                MernokPasswordManager.ReadMernokPasswordFile(path);
                LastUID = (uint)mernokPasswordFile.mernokPasswordList.Count();
            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordCreator.RequestName != null && PasswordCreator.CreatorName !=null && PasswordCreator.CreatorName != "" && PasswordCreator.RequestName != "")
            {
               PasswordCreator.PassWordOut = PasswordCreator.PassWordCreate(PasswordCreator.RequesterUID, PasswordCreator.RequestName,PasswordCreator.CreatorUID, PasswordCreator.CreatorName);
                LastUID++;

                mernokPasswordFile.mernokPasswordList.Add(new MernokPassword
                {
                    CreatorName = PasswordCreator.CreatorName,
                    creatorUID = PasswordCreator.CreatorUID,
                    requestName = PasswordCreator.RequestName,
                    requesterUID = LastUID
                }); 
                MernokPasswordManager.CreateMernokAssetFile(mernokPasswordFile);

                MernokPasswordManager.ReadMernokPasswordFile(V);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PasswordDecriptor.PasswordToDetails(PasswordCreator.PassWordOut);
        }
    }
}
