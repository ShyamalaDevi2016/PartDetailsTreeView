using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using PartDetails.DAL;
using System.Collections.ObjectModel;
using PartDetails.Models;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;
using PartDetails.Command;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Configuration;


namespace PartDetails.ViewModels
{
   public class PartListViewModel : INotifyPropertyChanged
    {
        PartDetailsDL objPartDL = new PartDetailsDL();
        private List<string> _Tabs = new List<string>();
        Dictionary<int, string> dicRootConfig = new Dictionary<int, string>();
     
        public ObservableCollection<Parts> PartSet { get; set; }

        List<TabItem> _TabItems = new List<TabItem>();
        private List<TabItem> _tabItems;


      
        private int _TabIndex;
        public static bool _IsShowPart =true;
        private static Dictionary<int, Parts> node_list = new Dictionary<int, Parts>();

        private ICommand BtnExpCommand;
        private ICommand PartCommand;
        private ICommand FileCommand;
        private ICommand TabSelectionCommand;

        public PartListViewModel()
        {

            btnExportClick = new RelayCommand(new Action<object>(ShowMessage));
            PartSelected = new RelayCommand(new Action<object>(ShowPartDetails));
            FileSelected = new RelayCommand(new Action<object>(ShowFileDetails));

            AddTabItem();
       
        }

        #region Properties
        public int TabIndex
        {
            get
            {
                return _TabIndex;
            }
            set
            {
                _TabIndex = value;
                OnPropertyChanged("TabIndex");
            }
        }

        public List<TabItem> Tabs
        {
            get
            {
                return _tabItems;
            }
            set
            {
                _tabItems = value;
                OnPropertyChanged("Tabs");
            }

        }

        public bool IsShowPart
        {
            get
            {
                return _IsShowPart;
            }
            set
            {
                _IsShowPart = value;
            }
        }

        #endregion


        #region Commands
        public ICommand btnExportClick
        {
            get { return BtnExpCommand; }
            set { BtnExpCommand = value; }
        }

        public ICommand PartSelected
        {
            get { return PartCommand; }
            set { PartCommand = value; }
        }

        public ICommand FileSelected
        {
            get { return FileCommand; }
            set { FileCommand = value; }
        }


        public void ShowMessage(object obj)
        {
            ExportJSON();

        }
        public void ShowPartDetails(object obj)
        {
            TabIndex = 1;
            IsShowPart = true;
            TabIndex = 0;
        }

        public void ShowFileDetails(object obj)
        {
            TabIndex = 1;
            IsShowPart = false;
            TabIndex = 0;
        }

        #endregion


        #region Methods
        /// <summary>
        ///  To creates tab items dynamically with the configuration names of root part item
        ///  Assigning the unique id of each tab item with the id of root part item. 
        /// </summary>
        public List<TabItem> AddTabItem()
        {
            _tabItems = new List<TabItem>();
            int count = _tabItems.Count;

            try
            {
                dicRootConfig = objPartDL.GetRootPartConfigNames();
                TabItem tab = null;
                dicRootConfig.ToList().ForEach
                (
                    pair =>
                    {

                        tab = new TabItem();
                        tab.Header = string.Format(pair.Value);
                        tab.Name = "Tab_" + pair.Key.ToString();

                        tab.Uid = pair.Key.ToString();

                        _tabItems.Insert(count, tab);
                        count++;

                    }
                );
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception occured." + ex.Message);
            }
            return _tabItems;


        }

        /// <summary>
        ///  Binding treeview dynamically based on the tab item(Uid)selected or id of root part and
        ///  also based on the option selected to show part/file names in treeview  
        /// </summary>
        public ObservableCollection<Parts> BindingTreeView(int iRootPartId,bool IsShowPart)
        {

            ObservableCollection<Parts> lstRootPart = new ObservableCollection<Parts>();

            try
            {
                //Getting flat hierarchical part details from db
                DataTable dtNodes = objPartDL.GetPartNodeDetails(iRootPartId, IsShowPart);

                if (dtNodes.Rows.Count > 0)
                {
                    //binding the model object with flat hierarchical data from db
                    List<Parts> lstTable = dtNodes.AsEnumerable().Select(m => new Parts()
                    {
                        id = m.Field<int>("ChildConfigIndex"),
                        parent_id = m.Field<int>("ParentConfigIndex"),

                        Qty = m.Field<double>("Qty"),
                        Name = m.Field<string>("ChildName"),
                        Description = m.Field<string>("Description"),
                        RootDescription = m.Field<string>("Root_Description")
                    }).ToList();

                    //binding the root element of treeview
                    var root = new Parts { Name = dtNodes.Rows[0][0].ToString(), parent_id = lstTable[0].parent_id, id = lstTable[0].id, Qty = lstTable[0].Qty, Description = lstTable[0].RootDescription };


                    //converting flat data in to hierarchical tree object
                    node_list = new Dictionary<int, Parts> { { lstTable[0].parent_id, root } };

                    foreach (var item in lstTable)
                    {
                        node_list.Add(item.id, item);
                        node_list[item.parent_id].SubParts.Add(node_list[item.id]);
                    }

                    //Assigning complete hierarchy to list
                    lstRootPart.Add(node_list.First().Value);

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception occured." + ex.Message);
            }
            return lstRootPart;


        }

        /// <summary>
        ///  Converting the part hierarchy object into JSON file using JSON serializer
        ///   
        /// </summary>
        public void ExportJSON()
        {
            string strExpFolder = ConfigurationManager.AppSettings["ExportPath"];

            string strExportFile = strExpFolder + ConfigurationManager.AppSettings["ExportFileName"];
            if (!Directory.Exists(strExpFolder))
            {
                Directory.CreateDirectory(strExpFolder);
            }
            else
            {
                File.Delete(strExportFile);
            }

            if (node_list.Count > 0)
            {
                var rootContent = node_list.First().Value;
                //Setting up Parent Id
                rootContent.id = rootContent.parent_id;
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                };
                string json = JsonConvert.SerializeObject(rootContent, settings);

                File.WriteAllText(strExportFile, json);
                MessageBox.Show("Exported Successfully in " + strExportFile);
            }
            else
            {
                MessageBox.Show("No data found to export");
            }
        }

        #endregion

        #region INotifyImplementation
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
            }
        }
        #endregion
    }
}
