﻿using Logger.FileManagement.Xml;
using LoggerRead.Windows;
using LoggerSystem;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LoggerRead
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<LogXml> LogXml = new List<LogXml>();
        public static List<List<string>> FilesVirtual = new List<List<string>>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<Entry> logXmls = new List<Entry>();

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "xml files (*.xml)|*.XML";
            openFileDialog.RestoreDirectory = true;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Entry));

            if (openFileDialog.ShowDialog() == true)
            {
                var files = openFileDialog.FileNames;

                //Proccess each file
                foreach (var f in files)
                {

                    XDocument xDocument;

                    try
                    {
                        string data = File.ReadAllText(f);

                        logXmls.AddRange(DeserializeXml(data).entries);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        continue;
                    }




                }
                foreach (var entry in logXmls)
                {
                    LogXml.AddRange(entry.LogXml);
                }

                LogXml = LogXml.OrderBy(x => x.dateTime).ToList();

                DataSetLogs.ItemsSource = LogXml;
            }
        }


        public static Data DeserializeXml(string xmlString)
        {
            XElement root;
            try
            {
                root = XElement.Parse(xmlString);
            }catch(Exception ex)
            {
                return new Data();
            }
            var data = new Data
            {
                entries = root.Elements("entry").Select(entry => new Entry
                {
                    LogXml = entry.Elements("Log").Select(log => new LogXml
                    {
                        ApiToken = (string)log.Element("ApiToken"),
                        dateTime = (DateTime)log.Element("dateTime"),
                        SystemName = (string)log.Element("SystemName"),
                        Levels = (Levels)Enum.Parse<Levels>((string)log.Element("Levels"), true),
                        Message = (string)log.Element("Message")
                    }).ToList()
                }).ToList()
            };

            return data;
        }



        private void DataSet_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "HH:mm:ss:f dd-MM-yyyy";
        }

        private void Network_Click(object sender, RoutedEventArgs e)
        {
            new NetworkDownload().ShowDialog();

            List<Entry> logXmls = new List<Entry>();
            foreach (var f in FilesVirtual)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(f.ToArray());

                var data = DeserializeXml(string.Join(Environment.NewLine, f.ToArray()));
                if(data.entries == null)
                {
                    continue;
                }

                logXmls.AddRange(data.entries);
                foreach (var entry in logXmls)
                {
                    LogXml.AddRange(entry.LogXml);
                }
            }


            LogXml = LogXml.OrderBy(x => x.dateTime).ToList();

            DataSetLogs.ItemsSource = LogXml;

        }
    }
}