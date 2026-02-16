using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using System.Windows.Controls;
using MapVariantIDSwitcher.Windows;
using ManagedUFL;
using System.Diagnostics;

namespace MapVariantIDSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HaloEngineManager? haloEngineManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InputModButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Map JSON (.json)|*.json"
            };
            if (Properties.Settings.Default.DefaultModMapPath.Length > 0)
            {
                openFileDialog.InitialDirectory = Properties.Settings.Default.DefaultModMapPath;
            }
            if (openFileDialog.ShowDialog() == true) 
            {
                InputModMap.Text = openFileDialog.FileName;
                OutputTextBlock.Text += MapVariantManager.LoadModMap(openFileDialog.FileName);
            }
        }

        private void InputVariantButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Map Variant (.mvar)|*.mvar"
            };
            if (Properties.Settings.Default.DefaultMapVariantPath.Length > 0)
            {
                openFileDialog.InitialDirectory = Properties.Settings.Default.DefaultMapVariantPath;
            }
            if (openFileDialog.ShowDialog() == true)
            {
                InputMapVariant.Text = openFileDialog.FileName;
                OutputTextBlock.Text += MapVariantManager.LoadMapVariant(openFileDialog.FileName);
            }
        }

        private void OutputFolderButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
            if (Properties.Settings.Default.DefaultOutputPath.Length > 0)
            {
                vistaFolderBrowserDialog.SelectedPath = Properties.Settings.Default.DefaultOutputPath;
            }
            if (vistaFolderBrowserDialog.ShowDialog() == true) 
            {
                OutputFolder.Text = vistaFolderBrowserDialog.SelectedPath;
            }
        }

        private void ProcessVariantButton_Click(object sender, RoutedEventArgs e)
        {
            if (HaloEngineManager.SelectedEngine == null)
            {
                MessageBox.Show("You must select a game engine");
                return;
            }
            if (InputModMap.Text.Length <= 0 && UseBuiltInMapIDCheckBox.IsChecked != true)
            {
                MessageBox.Show("You must select an input mod json file");
                return;
            }
            if (BuiltInMapIDComboBox.SelectedIndex < 0 && UseBuiltInMapIDCheckBox.IsChecked == true)
            {
                MessageBox.Show("You must select a map id from the dropdown");
                return;
            }
            if (InputMapVariant.Text.Length <= 0)
            {
                MessageBox.Show("You must select an input map variant file");
                return;
            }
            if (OutputFolder.Text.Length <= 0)
            {
                MessageBox.Show("You must select an output folder");
                return;
            }
            if (UseBuiltInMapIDCheckBox.IsChecked == true)
            {
                if (MapVariantManager.SwitchMapIds(InputMapVariant.Text, OutputFolder.Text, (e_builtin_map_id)BuiltInMapIDComboBox.SelectedItem) == true)
                {
                    OutputTextBlock.Text += $"Successfully switched variant {(MapVariantManager.ManagedMapVariant != null ? MapVariantManager.ManagedMapVariant.Name : "")} to use {BuiltInMapIDComboBox.SelectedItem}\n";
                }
            }
            else
            {
                if (MapVariantManager.SwitchMapIds(InputMapVariant.Text, OutputFolder.Text) == true)
                {
                    OutputTextBlock.Text += $"Successfully switched variant {(MapVariantManager.ManagedMapVariant != null ? MapVariantManager.ManagedMapVariant.Name : "")} to use {((MapVariantManager.ModMap != null && MapVariantManager.ModMap.Title != null) ? MapVariantManager.ModMap.Title.GetValueOrDefault("Neutral") : "")}\n";
                }
            }
            
        }

        private void OutputTextBlock_Initialized(object sender, EventArgs e)
        {
            OutputTextBlock.Text += "Application loaded\n";
        }

        private void EngineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HaloEngineManager.SelectedEngine = (HaloEngine)((ComboBox)sender).SelectedItem;
            OutputTextBlock.Text += "Loaded " + HaloEngineManager.SelectedEngine.Name + " Dll\n";
            Properties.Settings.Default.DefaultEngine = EngineComboBox.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (!File.Exists("Excession.dll") || !File.Exists("ManagedUFL.dll") || !File.Exists("Ijwhost.dll"))
            {
                MessageBoxResult mbResult = MessageBox.Show("This application must be placed in the same folder as the Halo MCC Mod Uploader (ExcessionApp) application and Dll files in order to function correctly.", "Error", MessageBoxButton.OKCancel);
                if (mbResult != MessageBoxResult.OK) 
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                haloEngineManager = new HaloEngineManager();
                DataContext = haloEngineManager;
                OutputTextBlock.Text += "Loaded Excession Dlls\n";
            }
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            Preferences preferences = new Preferences();
            preferences.Show();
        }

        private void EngineComboBox_Initialized(object sender, EventArgs e)
        {
            EngineComboBox.SelectedIndex = Properties.Settings.Default.DefaultEngine;
        }

        private void UseBuiltInMapIDCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ModMapGrid.Visibility = Visibility.Collapsed;
            BuiltInMapIDComboBox.Visibility = Visibility.Visible;
        }

        private void BuiltInMapIDComboBox_Initialized(object sender, EventArgs e)
        {
            foreach (e_builtin_map_id id in Enum.GetValues(typeof(e_builtin_map_id)))
            {
                BuiltInMapIDComboBox.Items.Add(id);
            }
        }

        private void UseBuiltInMapIDCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ModMapGrid.Visibility = Visibility.Visible;
            BuiltInMapIDComboBox.Visibility = Visibility.Collapsed;
        }

        private void OutputTextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutputTextBlock.Focus();
            OutputTextBlock.CaretIndex = OutputTextBlock.Text.Length;
            OutputTextBlock.ScrollToEnd();
        }
    }

    public class HaloEngine
    {
        private Excession.HaloShellInterface.ManagedGameEngineDll _engine;
        public Excession.HaloShellInterface.ManagedGameEngineDll Engine
        {
            get { return _engine; }
            set { _engine = value; }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string DllName;

        public HaloEngine(Excession.HaloShellInterface.ManagedGameEngineDll engine, string name, string dllName)
        {
            _engine = engine;
            _name = name;
            DllName = dllName;
            engine.LoadDll($"bin\\{dllName}.dll");
        }
    }


    public class HaloEngineManager
    {
        private static Excession.HaloShellInterface.ManagedGameEngineDll HaloReachDll = new Excession.HaloShellInterface.ManagedGameEngineDll(ManagedUFL.EGameEngine.HaloReach);
        private static Excession.HaloShellInterface.ManagedGameEngineDll Halo4Dll = new Excession.HaloShellInterface.ManagedGameEngineDll(ManagedUFL.EGameEngine.Halo4);
        private static Excession.HaloShellInterface.ManagedGameEngineDll Halo2ADll = new Excession.HaloShellInterface.ManagedGameEngineDll(ManagedUFL.EGameEngine.Halo2A);
        private static Excession.HaloShellInterface.ManagedGameEngineDll Halo3Dll = new Excession.HaloShellInterface.ManagedGameEngineDll(ManagedUFL.EGameEngine.Halo3);
        private static Excession.HaloShellInterface.ManagedGameEngineDll Halo3ODSTDll = new Excession.HaloShellInterface.ManagedGameEngineDll(ManagedUFL.EGameEngine.Halo3ODST);

        public List<HaloEngine> HaloEngines { get; set; } = new List<HaloEngine>();
        private static HaloEngine? selectedEngine;
        public static HaloEngine? SelectedEngine
        {
            get 
            {
                return selectedEngine;
            }
            set 
            { 
                selectedEngine = value;
            }
        }
        
        public HaloEngineManager()
        {
            HaloEngines.Add(new HaloEngine(HaloReachDll, "Halo Reach", "haloreach"));
            HaloEngines.Add(new HaloEngine(Halo4Dll, "Halo 4", "halo4"));
            HaloEngines.Add(new HaloEngine(Halo2ADll, "Halo 2 Anniversary", "groundhog"));
            HaloEngines.Add(new HaloEngine(Halo3Dll, "Halo 3", "halo3"));
            HaloEngines.Add(new HaloEngine(Halo3ODSTDll, "Halo 3 ODST", "halo3odst"));
        }
    }

    public class ModMapJson
    {
        public Guid MapGuid { get; set; }
        public string? ScenarioFile { get; set; }
        public Dictionary<string, string>? Title { get; set; }
        //public Dictionary<string, string>? Description { get; set; }
        /*
        public Dictionary<string, string>? Images { get; set; }
        public string[]? Flags { get; set; }
        public string[]? InsertionPoints { get; set; }
        public Dictionary<string, string>? MaximumTeamsByGameCategory { get; set; }
        public int[]? ValidMultiplayerObjectTypes { get; set; }
        public string? MapDefaultPrimaryWeapon { get; set; }
        public string? MapDefaultPrimaryWeaponForge { get; set; }
        */
    }

    public class MapVariantManager
    {
        public static ManagedUFL.ManagedMapVariant? ManagedMapVariant;
        public static ModMapJson? ModMap;

        public static string? LoadMapVariant(string mvarPath)
        {
            if (HaloEngineManager.SelectedEngine == null) 
            {
                MessageBox.Show("Could not select an engine to use");
                return null;
            }
            ManagedMapVariant = HaloEngineManager.SelectedEngine.Engine.CreateMapVariantFromFile(mvarPath);

            return "Loaded " + ManagedMapVariant.Name + "\n";
        }

        public static string? LoadModMap(string modMapPath)
        {
            string modMapJsonText;
            using (FileStream fs = new FileStream(modMapPath, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    modMapJsonText = sr.ReadToEnd();
                }
            }
            if (modMapJsonText == null)
            {
                return null;
            }
            ModMap = JsonSerializer.Deserialize<ModMapJson>(modMapJsonText);
            if (ModMap == null)
            {
                return null;
            }
            if (ModMap.Title == null)
            { 
                return null;
            }
            else
            {
                string? modMapTitle;
                ModMap.Title.TryGetValue("Neutral", out modMapTitle);
                return "Loaded " + modMapTitle + "\n";
            }
        }

        public static bool SwitchMapIds(string mvarPath, string outputPath)
        {
            if (HaloEngineManager.SelectedEngine == null)
            {
                MessageBox.Show("Could not select an engine to use");
                return false;
            }

            if (ManagedMapVariant == null) 
            {
                MessageBox.Show("You must load a map variant before you can switch map Ids");
                return false;
            }

            if (ModMap == null)
            {
                MessageBox.Show("Could not parse mod JSON file");
                return false;
            }
            ManagedUFL.ManagedHaloMapId targetMapId = ManagedUFL.ManagedHaloMapId.FromGuid(ModMap.MapGuid);
            ManagedMapVariant.ForceSetHaloMapId(targetMapId);
            HaloEngineManager.SelectedEngine.Engine.SaveVariantToFile(ManagedMapVariant, Path.Combine(outputPath, Path.GetFileName(mvarPath)));
            return true;
        }

        public static bool SwitchMapIds(string mvarPath, string outputPath, e_builtin_map_id id)
        {
            if (HaloEngineManager.SelectedEngine == null)
            {
                MessageBox.Show("Could not select an engine to use");
                return false;
            }

            if (ManagedMapVariant == null)
            {
                MessageBox.Show("You must load a map variant before you can switch map Ids");
                return false;
            }

            ManagedUFL.ManagedHaloMapId targetMapId = ManagedUFL.ManagedHaloMapId.FromBuiltInMapAndInsertionPointKvp(new KeyValuePair<e_builtin_map_id, short>(id, 0));
            ManagedMapVariant.ForceSetHaloMapId(targetMapId);
            HaloEngineManager.SelectedEngine.Engine.SaveVariantToFile(ManagedMapVariant, Path.Combine(outputPath, Path.GetFileName(mvarPath)));
            return true;
        }
    }
}
