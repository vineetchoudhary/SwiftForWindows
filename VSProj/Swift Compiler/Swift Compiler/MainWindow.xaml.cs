using Garlic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Swift_Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string swiftfilePath;
        string objecFilePath;
        string exeFilePath;

        AnalyticsSession analyticsSession = new AnalyticsSession("swiftforwindows.codeplex.com", "UA-66151383-10");

        public MainWindow()
        {
            InitializeComponent();
            if (Settings.Default.IsFirstTime)
            {
                resetSettingValues();
            }
            setSettingValuesToTextBox();
            try
            {
                analyticsSession.CreatePageViewRequest("/", "Home").Send();
            }
            catch (Exception)
            {
            }
        }

        /*********************************************************
                           Settings Handler
        **********************************************************/
        private void setSettingValuesToTextBox()
        {
            if (Settings.Default.Recent1.Length > 0)
            {
                createFileNameAndPath(Settings.Default.Recent1);
            }
            if (Settings.Default.ProcessorType.Length == 0)
            {
                Settings.Default.ProcessorType = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine).ToLower();
                Settings.Default.Save();
            }
            textBoxVCPPVS.Text = Settings.Default.VCVARSALL;
            textBoxSwiftCompiler.Text = Settings.Default.Swift;
            comboBoxProcessor.SelectedValue = Settings.Default.ProcessorType;
        }

        private void resetSettingValues()
        {
            string currentDir = Environment.CurrentDirectory.Substring(0, 1);
            Settings.Default.Linker = "link";
            Settings.Default.ProcessorType = "";
            Settings.Default.Recent1 = "";
            Settings.Default.Swift = currentDir + ":\\SwiftForWindows\\Swift\\";
            Settings.Default.ProgramPath = currentDir + ":\\SwiftForWindows\\My Programs";
            Settings.Default.VCVARSALL = currentDir + ":\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat";
            Settings.Default.Save();
        }
        


        /*********************************************************
                           Command Getter
        **********************************************************/
        private string getVCVarsall()
        {
            return "\"" +Settings.Default.VCVARSALL +"\" "+  Settings.Default.ProcessorType;
        }

        private string getLinkerCommand()
        {
            return  "\"" +Settings.Default.Linker + "\" /out:\"" + exeFilePath + "\" \"" + objecFilePath + "\" libswiftCore.lib libswiftSwiftOnoneSupport.lib /LIBPATH:\"" + Settings.Default.Swift + "lib\\swift_static\\windows\" /MERGE:.rdata=.rodata  /FORCE:MULTIPLE /IGNORE:4006,4049,4217";
        }

        private string getCompilerCommand()
        {
            return "\"" +Settings.Default.Swift + "bin\\swiftc\" -c \"" + swiftfilePath + "\" -o \"" + objecFilePath + "\"";
        }


        /*********************************************************
                          Files/Folder Handler
        **********************************************************/
        private void selectSwiftProgramFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = "*.*";
            dlg.Title = "Select a source code file...";
            dlg.Multiselect = false;
            dlg.InitialDirectory = Settings.Default.ProgramPath;
            dlg.Filter = "Swift Files (*.swift)|*.swift";
            Nullable<bool> result = dlg.ShowDialog();
            if (!(bool)result)
            {
                return;
            }

            if (!dlg.FileName.Contains(Settings.Default.ProgramPath))
            {
                MessageBox.Show("You can only open the program file available in " + Settings.Default.ProgramPath + " directory.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //crete file
            createFileNameAndPath(dlg.FileName);            
        }

        private OpenFileDialog selectFiles(string title, string filter)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "*.*";
            dlg.Title = title;
            dlg.Multiselect = false;
            dlg.Filter = filter;// "Swift Files (*.swift)|*.swift";
            Nullable<bool> result = dlg.ShowDialog();
            return dlg;
        }

        private void createFileNameAndPath(String fileName)
        {
            swiftfilePath = fileName;
            exeFilePath = fileName.Replace(".swift", ".exe");
            objecFilePath = fileName.Replace(".swift", ".obj");
            textBoxSelectedFile.Text = fileName;

            //save file
            Settings.Default.Recent1 = swiftfilePath;
            Settings.Default.Save();
        }

        /*********************************************************
                    Swift Program Compile and Run
        **********************************************************/
        private void compileProgram()
        {
            try
            {
                Process bat = new Process();
                bat.StartInfo.FileName = "cmd";
                bat.StartInfo.UseShellExecute = false;
                bat.StartInfo.RedirectStandardOutput = true;
                bat.StartInfo.RedirectStandardInput = true;
                bat.StartInfo.CreateNoWindow = false;
                
                bat.Start();

                StreamWriter sw = bat.StandardInput;
                StreamReader sr = bat.StandardOutput;
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("echo off");
                    sw.WriteLine(this.getVCVarsall());
                    sw.WriteLine(this.getCompilerCommand());
                    sw.WriteLine(this.getLinkerCommand());
                    sw.WriteLine("title Compile Process Completed. If no error then close this windows and hit run button.");
                    textBlockLog.Text = sr.ReadToEnd();
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void runProgram()
        {
            Process.Start("cmd", "/K \"" + exeFilePath +"\"");
        }
        

        /*******************************************************
                               Select File
        ********************************************************/
        private void buttonSelectFile_Click(object sender, RoutedEventArgs e)
        {
            selectSwiftProgramFile();
        }

        private void textBoxSelectedFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selectSwiftProgramFile();
        }


        /*******************************************************
                            Compile and Run
        ********************************************************/
        private void buttonCompile_Click(object sender, RoutedEventArgs e)
        {
            if (!(File.Exists(textBoxSelectedFile.Text)))
            {
                selectSwiftProgramFile();
            }
            if (File.Exists(textBoxSelectedFile.Text))
            {
                compileProgram();
            }
        }

        private void buttonRun_Click(object sender, RoutedEventArgs e)
        {
            if (!(File.Exists(textBoxSelectedFile.Text)))
            {
                selectSwiftProgramFile();
            }
            if (File.Exists(textBoxSelectedFile.Text))
            {
                runProgram();
            }
        }


        /*******************************************************
                            Compiler Settings
        ********************************************************/
        private void comboBoxProcessor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(comboBoxProcessor.SelectedValue == null))
            {
                Settings.Default.ProcessorType = comboBoxProcessor.SelectedValue.ToString();
            }
        }

        private void textBoxVCPPVS_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = selectFiles("Select vcvarsall.bat file", "vcvarsall.bat|vcvarsall.bat");

            if (dlg.FileName.Length > 0)
            {
                textBoxVCPPVS.Text = dlg.FileName;
                //save setting
                Settings.Default.VCVARSALL = textBoxVCPPVS.Text;
                Settings.Default.Linker = textBoxVCPPVS.Text.Replace("vcvarsall.bat", "bin\\link.exe");
                Settings.Default.Save();
            }
        }

        private void textBoxSwiftCompiler_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = selectFiles("Select swiftc.exe file", "swiftc.exe|swiftc.exe");

            if (dlg.FileName.Length > 0)
            {
                textBoxSwiftCompiler.Text = dlg.FileName.Replace("bin\\swiftc.exe", "");
                //save setting
                Settings.Default.Swift = textBoxSwiftCompiler.Text;
                Settings.Default.Save();
            }
        }

        private void buttonResetSetting_Click(object sender, RoutedEventArgs e)
        {
            resetSettingValues();
            setSettingValuesToTextBox();
        }

        private void buttonProjectWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://swiftforwindows.codeplex.com/");
            analyticsSession.CreatePageViewRequest("/", "Home").Send();
        }

        private void buttonProjectHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://swiftforwindows.codeplex.com/wikipage?title=Help");
            analyticsSession.CreatePageViewRequest("/wikipage?title=Help","Help").Send();
        }

        private void buttonProjectNews_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://swiftforwindows.codeplex.com/wikipage?title=News");
            analyticsSession.CreatePageViewRequest("/wikipage?title=News", "News").Send();
        }
    }
}
